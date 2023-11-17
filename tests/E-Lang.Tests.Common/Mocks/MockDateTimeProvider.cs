using E_Lang.Application.Interfaces;

namespace E_Lang.Tests.Common.Mocks;

public class MockDateTimeProvider : IDateTimeProvider
{
    private static IDateTimeProvider? _instance;
    
    public static DateTime MockNow = new DateTime(2022, 10, 10, 10, 00, 00);

    public DateTime Now => MockNow;
    public DateTime UtcNow => MockNow.ToUniversalTime();

    private MockDateTimeProvider()
    {
    }

    public static IDateTimeProvider GetInstance()
    {
        return _instance ??= new MockDateTimeProvider();
    }
}