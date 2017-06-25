﻿using System.Reflection;
using NUnit.Framework;
using TestTools;

namespace ILMerging.Tests
{
    [TestFixture]
    public sealed class BaselineTests
    {
        [Test]
        public void Single_input()
        {
            var ilMerge = new ILMerge();
            ilMerge.SetUpInputAssemblyForTest(Assembly.GetExecutingAssembly());
            ilMerge.MergeToTempFileForTest(".dll");
        }
    }
}
