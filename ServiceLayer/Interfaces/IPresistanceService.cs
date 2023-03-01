using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface IPresistanceService
    {
        Task Remove(string Key);
        Task Set(string Key, object Value,TimeSpan Expiry);
        Task<T?> Get<T>(string Key);
        Task<bool> KeyExists(string Key);
        Task<long> Increment(string Key, DateTimeOffset ExpirationTime);
        Task AcquireLock(string key, TimeSpan expiration, long MaxWaitTimeInMilliseconds, Func<Task<bool>>? CheckRelease = null);
        Task ReleaseLock(string Key);
    }
}
