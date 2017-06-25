The purpose of this unit test project is to demonstrate that MSTest2 unit tests are very similar to nUnit tests.
Both frameworks have their strengths and weaknesses - but I prefer MSTest2 :).

I do appreciate nUnit's TestFixture, and TestClass functionality - 
	I think MSTest2 can provide the same benefits; in some cases in a simpler manner, other cases... not so simple.

With MSTest2 we can use the DataRow attribute, which is similar to nUnit's TestCase attribute - to provide data-driven unit test processing.

nUnit example:
        [TestCase(true, TestName = "{m}(with mscorsn in path)")]
        [TestCase(false, TestName = "{m}(without mscorsn in path)")]
        public void No_DLL_load_crashes_when_given_PFX(bool withMscorsnInPath)

MSTest2 example:
        [DataRow(true, DisplayName = "(with mscorsn in path)")]
        [DataRow(false, DisplayName = "(without mscorsn in path)")]
        public void No_DLL_load_crashes_when_given_PFX(bool withMscorsnInPath)