using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using TestCase = System.ValueTuple<System.Collections.Generic.List<string>, string>;

namespace DocumentReconstructor
{
    /// <summary>
    /// This is the test fixture for <see cref="IDocumentReconstructor"/>
    ///
    /// Test written in advance of implementation.
    /// </summary>
    [TestFixture()]
    public class TestIterative
    {
        /// <summary>
        /// The reconstructor being tested.
        /// </summary>
        public IDocumentReconstructor Reconstructor;


        /// <summary>
        /// This method is used to assign the implementation of the <see cref="IDocumentReconstructor"/> interface to <see cref="DocumentConstructorImpl"/>
        /// </summary>
        [SetUp]
        public void SetupDocumentConstructorImpl()  // NOTE: This method of instantiating an implementation is pretty tightly coupled, I'd rather use an injection framework, but that's more work than needed here.
        {
            Reconstructor = new DocumentConstructorImpl(DocumentConstructorImpl.ByMutableIterative(DocumentConstructorImpl.GreedyMatchIntersection));
        }


        /// <summary>
        /// Since there's only one function to the <see cref="IDocumentReconstructor"/> only one test is required.
        /// </summary>
        /// <param name="testData"></param>
        /// <returns></returns>
        [Test(), TestCaseSource(typeof(TestDataProvider), "TestCases")]
        public string TestReconstructor(List<string> testData) => Reconstructor.Reconstruct(testData);
    }

    /// <summary>
    /// This is the test fixture for <see cref="IDocumentReconstructor"/>
    ///
    /// Test written in advance of implementation.
    /// </summary>
    [TestFixture()]
    public class TestRecursive
    {
        /// <summary>
        /// The reconstructor being tested.
        /// </summary>
        public IDocumentReconstructor Reconstructor;


        /// <summary>
        /// This method is used to assign the implementation of the <see cref="IDocumentReconstructor"/> interface to <see cref="DocumentConstructorImpl"/>
        /// </summary>
        [SetUp]
        public void SetupDocumentConstructorImpl()  // NOTE: This method of instantiating an implementation is pretty tightly coupled, I'd rather use an injection framework, but that's more work than needed here.
        {
            Reconstructor = new DocumentConstructorImpl(DocumentConstructorImpl.ByImmutableRecursive(DocumentConstructorImpl.GreedyMatchIntersection));
        }


        /// <summary>
        /// Since there's only one function to the <see cref="IDocumentReconstructor"/> only one test is required.
        /// </summary>
        /// <param name="testData"></param>
        /// <returns></returns>
        [Test(), TestCaseSource(typeof(TestDataProvider), "TestCases")]
        public string TestReconstructor(List<string> testData) => Reconstructor.Reconstruct(testData);
    }


    /// <summary>
    /// This class exposes the test data as provided.
    /// </summary>
    public class TestDataProvider
    {

        /// <summary>
        /// The test fragements as a list of string.  
        /// </summary>
        private static List<TestCase> tests = new List<TestCase>() {
            (
                new List<string>
                {
                    "m quaerat voluptatem.",
                    "pora incidunt ut labore et d",
                    ", consectetur, adipisci velit",
                    "olore magnam aliqua",
                    "idunt ut labore et dolore magn",
                    "uptatem.",
                    "i dolorem ipsum qu",
                    "iquam quaerat vol",
                    "psum quia dolor sit amet, consectetur, a",
                    "ia dolor sit amet, conse",
                    "squam est, qui do",
                    "Neque porro quisquam est, qu",
                    "aerat voluptatem.",
                    "m eius modi tem",
                    "Neque porro qui",
                    ", sed quia non numquam ei",
                    "lorem ipsum quia dolor sit amet",
                    "ctetur, adipisci velit, sed quia non numq",
                    "unt ut labore et dolore magnam aliquam qu",
                    "dipisci velit, sed quia non numqua",
                    "us modi tempora incid",
                    "Neque porro quisquam est, qui dolorem i",
                    "uam eius modi tempo",  // this test data will not allow the simple form of the algorithm to complete.  Hueristics could be applied at this stage, but the results would be inaccurate.
                    "pora inc",
                    "am al",
                },
                "Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem."
            ),
            (
                new List<string>
                {
                    "i dolorem ipsum qu",
                    "psum quia dolor sit amet, consectetur, a",
                    "ctetur, adipisci velit, sed quia non numq",
                },
                "i dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numq"
            ),
        };


        public static IEnumerable<TestCaseData> TestCases
        {
            get
            {
                return tests.Select(testCase => new TestCaseData(testCase.Item1).Returns(testCase.Item2));
            }
        }
    }
}