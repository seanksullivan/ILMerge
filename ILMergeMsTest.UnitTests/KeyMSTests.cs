using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using ILMerging;
using System.Reflection;
using TestTools.Helpers;
using TestTools;

namespace ILMergeMsTest.UnitTests
{
    [TestClass]
    public class KeyMSTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [DeploymentItem("TestData")]
        public void Can_sign_using_keyfile()
        {
            const string Extension = ".dll";
            const string TestSnk = "test.snk";

            var outputFile = Path.Combine(TestContext.DeploymentDirectory, Path.ChangeExtension(Path.GetRandomFileName(), Extension));

            var ilMerge = new ILMerge { KeyFile = "test.snk", OutputFile = outputFile };
            ilMerge.SetUpInputAssemblyForTest(Assembly.GetExecutingAssembly());
            ilMerge.Merge();

            var expectedKeyBytes = new StrongNameKeyPair(File.ReadAllBytes(TestSnk)).PublicKey;
            var outputFileKeyBytes = AssemblyName.GetAssemblyName(outputFile).GetPublicKey();
            
            CollectionAssert.AreEqual(expectedKeyBytes, outputFileKeyBytes, "Expected PublicKey bytes do not match-up to the generated assembly PublicKey bytes");
        }

        //[TestMethod]
        //[DeploymentItem("TestData")]
        //public void Can_sign_using_keycontainer()
        //{
        //    var keyContainerName = Guid.NewGuid().ToString();
        //    CspContainerUtils.ImportBlob(true, keyContainerName, KeyNumber.Signature, File.ReadAllBytes(TestFiles.TestSnk));
        //    try
        //    {
        //        using (var outputFile = TempFile.WithExtension(".dll"))
        //        {
        //            var ilMerge = new ILMerge
        //            {
        //                KeyContainer = keyContainerName,
        //                OutputFile = outputFile
        //            };
        //            ilMerge.SetUpInputAssemblyForTest(Assembly.GetExecutingAssembly());
        //            ilMerge.Merge();

        //            Assert.That(
        //                AssemblyName.GetAssemblyName(outputFile).GetPublicKey(),
        //                Is.EqualTo(new StrongNameKeyPair(File.ReadAllBytes(TestFiles.TestSnk)).PublicKey));
        //        }
        //    }
        //    finally
        //    {
        //        CspContainerUtils.Delete(true, keyContainerName, KeyNumber.Signature);
        //    }
        //}
    }
}
