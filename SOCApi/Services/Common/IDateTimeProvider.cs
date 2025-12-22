namespace SOCApi.Services.Common
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}