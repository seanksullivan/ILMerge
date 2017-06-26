using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using TestTools.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ILMerging;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ILMergeMsTest.UnitTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public sealed class ConsoleMSTests
    {
        #region Private Members
        private const string Extension = ".dll";
        private const string TestSnk = "test.snk";
        private const string TestPfx = "test.pfx";
        #endregion

        #region Public Properties
        public TestContext TestContext { get; set; }
        #endregion

        /// <summary>
        /// Demonstrating MSTest2 DataRowAttribute usage, 
        /// that partially replicates nUnit TestFixture and TestCase functionality.
        /// Test output and working directory is within the generated "TestResults" folder - it is cleansed for all "Release" test runs,
        /// and partially persists for "Debug" test runs (the NUnit tests are using the system's "Temp" directory, which is not preferable).
        /// </summary>
        /// <param name="withMscorsnInPath"></param>
        [TestMethod]
        [TestCategory("Integration")]
        [DeploymentItem("TestData")]
        [DataRow(true, DisplayName = "(with mscorsn in path)")]
        [DataRow(false, DisplayName = "(without mscorsn in path)")]
        public void No_DLL_load_crashes_when_given_PFX(bool withMscorsnInPath)
        {
            // Inconclusive for non-64 Bit OS'
            if (withMscorsnInPath && !Environment.Is64BitOperatingSystem) Assert.Inconclusive("This test can only be run on a 64-bit OS.");

            // ARRANGE
            var ilMergeExePath = typeof(ILMerge).Assembly.Location;
            var inputAssembly = Assembly.GetExecutingAssembly();
            var fileName = ShadowCopyUtils.GenerateILMergeLibCliSwitches(inputAssembly);
            var outputFile = Path.Combine(TestContext.DeploymentDirectory, 
                Path.ChangeExtension(Path.GetRandomFileName(), Extension));

            var startInfo = new ProcessStartInfo(
                ilMergeExePath, $"{fileName} /keyfile:\"{TestPfx}\" /out:\"{outputFile}\" \"{inputAssembly.Location}\"")
            {
                WorkingDirectory = Path.GetDirectoryName(inputAssembly.Location)
            };

            if (withMscorsnInPath)
            {
                startInfo.EnvironmentVariables["PATH"] = $"{Environment.GetEnvironmentVariable("PATH")};{RuntimeEnvironment.GetRuntimeDirectory()}";
            }

            // ACT
            // The system runs .NET executables as 64-bit no matter what the architecture of the calling process is.
            var result = ProcessUtils.Run(startInfo);

            // ASSERT
            Assert.AreEqual(0, result.ExitCode);
            Assert.IsFalse(result.ToString().Contains("Unable to load DLL 'mscorsn.dll'"));
            Assert.IsFalse(result.ToString().Contains("An attempt was made to load a program with an incorrect format."));

            // Test failures:
            Assert.IsFalse(result.ToString().Contains("Unhandled Exception: System.IO.FileNotFoundException"),
                "The test is not being run properly. If you are using ReSharper, disable shadow copy. " +
                "If you are using NCrunch, go to NCrunch's configuration for the ILMerge project and " +
                "make sure \"Copy referenced assemblies to workspace\" is set to True. " +
                "(Both ReSharper and NCrunch settings are saved in the repo, so this should not happen.)");
        }
    }
}
