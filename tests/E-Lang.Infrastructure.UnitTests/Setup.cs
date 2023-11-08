namespace E_Lang.Infrastructure.UnitTests
{
    public abstract class Setup
    {
        public static void InitClass()
        {
        }

        public static void InitTest()
        {
        }

        public static void CleanupTest()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
