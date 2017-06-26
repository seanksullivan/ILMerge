using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using ILMerging;
using System.Reflection;
using TestTools.Helpers;
using TestTools;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ILMergeMsTest.UnitTests
{
    /// <summary>
    /// The purpose of this class is to demonstrate that MSTest/MSTest2 is just as simple to use as nUnit.
    /// MSTest and NUnit both have their own strengths and weaknesses, but for the most part - these tests are simple.
    /// Test output and working directory is within the generated "TestResults" folder - it is cleansed for all "Release" test runs,
    /// and partially persists for "Debug" test runs (the NUnit tests are using the system's "Temp" directory, which is not preferable).
    /// </summary>
    [ExcludeFromCodeCoverage]
    [TestClass]
    public sealed class KeyMSTests
    {
        #region Private Members
        private const string Extension = ".dll";
        private const string TestSnk = "test.snk";
        private const string TestPfx = "test.pfx";
        private static List<string> _keyContainerNameList = new List<string>();
        #endregion

        #region Public Properties
        public TestContext TestContext { get; set; }
        #endregion

        #region Init and Cleanup
        /// <summary>
        /// Let's cleanup Csp Containers after all tests have executed (successfully or unsuccessfully).
        /// </summary>
        [ClassCleanup]
        public static void ClassCleanup()
        {
            _keyContainerNameList.ForEach(containerName => CspContainerUtils.Delete(true, containerName, KeyNumber.Signature));
        }
        #endregion

        [TestMethod]
        [DeploymentItem("TestData")]
        public void Can_sign_using_keyfile()
        {
            // ARRANGE
            var outputFile = Path.Combine(TestContext.DeploymentDirectory, 
                Path.ChangeExtension(Path.GetRandomFileName(), Extension));

            var ilMerge = new ILMerge { KeyFile = "test.snk", OutputFile = outputFile };

            ilMerge.SetUpInputAssemblyForTest(Assembly.GetExecutingAssembly());

            // ACT
            ilMerge.Merge();

            // ASSERT
            var expectedKeyBytes = new StrongNameKeyPair(File.ReadAllBytes(TestSnk)).PublicKey;
            var outputFileKeyBytes = AssemblyName.GetAssemblyName(outputFile).GetPublicKey();          
            CollectionAssert.AreEqual(expectedKeyBytes, outputFileKeyBytes, "Expected PublicKey bytes do not match-up to the generated assembly PublicKey bytes");
        }

        [TestMethod]
        [DeploymentItem("TestData")]
        public void Can_sign_using_keycontainer()
        {
            // ARRANGE
            var keyContainerName = Guid.NewGuid().ToString();
            _keyContainerNameList.Add(keyContainerName);
            
            CspContainerUtils.ImportBlob(true, keyContainerName, KeyNumber.Signature, File.ReadAllBytes(TestSnk));

            var outputFile = Path.Combine(TestContext.DeploymentDirectory, 
                Path.ChangeExtension(Path.GetRandomFileName(), Extension));

            var ilMerge = new ILMerge
            {
                KeyContainer = keyContainerName,
                OutputFile = outputFile
            };
            ilMerge.SetUpInputAssemblyForTest(Assembly.GetExecutingAssembly());

            // ACT
            ilMerge.Merge();

            // ASSERT
            var expectedKeyBytes = new StrongNameKeyPair(File.ReadAllBytes(TestSnk)).PublicKey;
            var outputFileKeyBytes = AssemblyName.GetAssemblyName(outputFile).GetPublicKey();
            CollectionAssert.AreEqual(expectedKeyBytes, outputFileKeyBytes, "Expected PublicKey bytes do not match-up to the generated assembly PublicKey bytes");
        }

        [TestMethod]
        [DeploymentItem("TestData")]
        public void Bad_keyfile_gives_diagnostic_warning()
        {
            // ARRANGE
            var outputFile = Path.Combine(TestContext.DeploymentDirectory,
                Path.ChangeExtension(Path.GetRandomFileName(), Extension));

            var logFile = Path.Combine(TestContext.DeploymentDirectory,
                Path.ChangeExtension(Path.GetRandomFileName(), ".log"));

            var ilMerge = new ILMerge
            {
                KeyFile = TestPfx,
                OutputFile = outputFile,
                LogFile = logFile
            };
            ilMerge.SetUpInputAssemblyForTest(Assembly.GetExecutingAssembly());

            //ACT
            ilMerge.Merge();

            //ASSERT
            var logText = File.ReadAllText(logFile);
            Assert.IsTrue(logText.Contains("Unable to obtain public key for StrongNameKeyPair."));
            Assert.IsTrue(logText.Contains("PFX"));
            Assert.IsTrue(logText.Contains("key container"));
        }
    }
}
