using ILMerging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using TestTools;

namespace ILMergeMsTest.UnitTests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public sealed class BaselineMSTests
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
        /// Replicating the "Single_Input" NUnit test, using MSTest2. 
        /// Test output and working directory is within the generated "TestResults" folder - it is cleansed for all "Release" test runs,
        /// and partially persists for "Debug" test runs (the NUnit tests are using the system's "Temp" directory, which is not preferable).
        /// </summary>
        /// <param name="withMscorsnInPath"></param>
        [TestMethod]
        public void Single_input()
        {
            var ilMerge = new ILMerge();
            ilMerge.SetUpInputAssemblyForTest(Assembly.GetExecutingAssembly());
            ilMerge.MergeToTempFileForTest(".dll");
        }
    }
}
