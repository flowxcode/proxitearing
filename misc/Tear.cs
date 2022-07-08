namespace AntiTearingTests
{
    using NUnit.Framework;

    [TestFixture]
    class AntiTearingTests
    {
        public AntiTearingTests()

        #region constants
        private const uint x = 1;
        #endregion

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        //[TestCase(50000)]
        //[TestCase(350000)]
        public void TearAMR([Range(5000, 500000, 5000)]int cycles)
        {
            }
            else
            {
            }
        }

        [Test]
        public void TearAmrBase()
        {
        }
    }
}
