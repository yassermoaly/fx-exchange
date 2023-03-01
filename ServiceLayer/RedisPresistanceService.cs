using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using ServiceLayer.Interfaces;
using StackExchange.Redis;

namespace ServiceLayer
{
    public class RedisPresistanceService : IPresistanceService
    {
        private readonly IConfiguration _configuration;
        public RedisPresistanceService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        ConnectionMultiplexer Connection => ConnectionMultiplexer.Connect(new ConfigurationOptions { 
            EndPoints = { _configuration["Redis:Server"] ?? "Redis Connection is not configured" },
            User = _configuration["Redis:User"],
            Password = _configuration["Redis:Password"],
        });
        private IDatabase DataBase => Connection.GetDatabase();
        public async Task<T?> Get<T>(string Key)
        {
            string? Value = DataBase != null? (await DataBase.StringGetAsync(Key)):string.Empty;
            if (string.IsNullOrEmpty(Value))
                return default;

            return JsonConvert.DeserializeObject<T?>(Value);
        }
        public async Task Remove(string Key)
        {
            await DataBase.KeyDeleteAsync(Key);
        }
        public async Task Set(string Key, object Value, TimeSpan Expiry)
        {
            await DataBase.StringSetAsync(Key, JsonConvert.SerializeObject(Value), Expiry);

        }
        public async Task<long> Increment(string Key, DateTimeOffset ExpirationTime)
        {
            TimeSpan expiryTime = ExpirationTime.DateTime.Subtract(DateTime.Now);
            long IncValue = await DataBase.StringIncrementAsync(Key);
            DataBase.KeyExpire(Key, expiryTime);
            return IncValue;
        }      
        public async Task<bool> KeyExists(string Key)
        {
            return await DataBase.KeyExistsAsync(Key);
        }

        public async Task AcquireLock(string key, TimeSpan expiration,long MaxWaitTimeInMilliseconds,Func<Task<bool>>? CheckRelease = null)
        {
            string value = "true";
            bool flag = false;
            long WaitTimeCounter = 0;
            while (!flag && WaitTimeCounter <= MaxWaitTimeInMilliseconds)
            {
                try
                {
                    if (CheckRelease != null)
                    {
                        if (await CheckRelease())
                            break;
                    }
                    flag = await DataBase.StringSetAsync(key, value, expiration, when:When.NotExists);
                    if(flag)
                        break;
                }
                catch {}
                WaitTimeCounter += 100;
                Thread.Sleep(100);
            }
        }
        public async Task ReleaseLock(string Key)
        {
            await Remove(Key);
        }
    }
}
