namespace E_Lang.Application.UnitTests
{
    [TestClass]
    public class TestBase : Setup
    {
        [ClassInitialize]
        public virtual void InitializeClass()
        {
            InitClass();
        }

        [TestInitialize]
        public virtual void InitializeTest()
        {
            InitTest();
        }

        [TestCleanup]
        public virtual void CleanUp()
        {
            CleanupTest();
        }
    }
}
