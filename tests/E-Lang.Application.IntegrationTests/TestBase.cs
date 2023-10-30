namespace E_Lang.Application.IntegrationTests
{
    [TestClass]
    public abstract class TestBase : Setup
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
