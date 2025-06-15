using Currency.Data.Contracts.Exceptions;
using Currency.Infrastructure.Contracts.Databases.Redis;

namespace Currency.Data.Locks;

internal class DataLock(IRedisLockContext context) : IAsyncDisposable
{
    private string _lockKey;
    private string _lockId;
    
    public async Task AcquireLockAsync(string key)
    {
        _lockKey = key;
        _lockId = Guid.NewGuid().ToString();
        
        bool lockAcquired = false;
        for (int i = 0; i < context.RetryCount; i++)
        {
            var isAcquired = await context.AcquireLockAsync(key, _lockId);
            if (isAcquired)
            {
                lockAcquired = true;
                break;
            }
            
            await Task.Delay(TimeSpan.FromMilliseconds(context.RetryDelayMilliseconds));
        }

        if (!lockAcquired)
            ConcurrencyException.Throw("Could not acquire lock!");
    }

    public async ValueTask DisposeAsync()
    {
        if (!string.IsNullOrEmpty(_lockKey) && !string.IsNullOrEmpty(_lockId))
            await context.ReleaseLockAsync(_lockKey, _lockId);
    }
}