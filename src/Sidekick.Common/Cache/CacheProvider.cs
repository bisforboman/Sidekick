using System.Collections.Concurrent;

namespace Sidekick.Common.Cache;

/// <summary>
///     Implementation for the cache provider.
/// </summary>
public class CacheProvider() : ICacheProvider
{   
    private ConcurrentDictionary<string, object> Cache { get; } = new();

    /// <inheritdoc />
    public TModel? Get<TModel>(string key, Func<TModel, bool> cacheValidator)
        where TModel : class
    {
        if (Cache.TryGetValue(key, out var obj) && obj is TModel model)
        {
            if (cacheValidator.Invoke(model))
            {
                return model;
            }
        }
        
        return null;
    }

    /// <inheritdoc />
    public void Set<TModel>(string key, TModel data) where TModel : class => 
        Cache.AddOrUpdate(key, data, (_, _) => data);

    /// <inheritdoc />
    public void Clear() => Cache.Clear();

    /// <inheritdoc />
    public void Delete(string key) => Cache.Remove(key, out var _);

    /// <inheritdoc />
    public async Task<TModel> GetOrSet<TModel>(string key, Func<Task<TModel>> func, Func<TModel, bool> cacheValidator)
        where TModel : class
    {
        var result = Get(key, cacheValidator);

        if (result is not null)
        {
            return result;
        }

        var data = await func.Invoke();
        Set(key, data);
        return data;
    }
}
