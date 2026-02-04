using Common.Database;
using Common.Resources.Xml.Descriptors;
using Common.Utilities;
using DbServer.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace DbServer.Database;

public class DbEntityCache<T> where T : DbModel, IDbQueryable
{
    private readonly ConcurrentDictionary<string, T> _cache = new();
    private readonly ConcurrentDictionary<T, HashSet<string>> _dirty = new(); // Tracks modified properties in a given entity
    private readonly HashSet<T> _new = new(); // Tracks new entities

    #region Query Methods

    public async Task<T?> GetAsync(string key)
    {
        if (_cache.TryGetValue(key, out var cached)) // Cache is the source of truth, search here first
            return cached; // Cache hit

        // Cache miss, find in MySQL
        await using var dbContext = await NetworkService.ContextFactory.CreateDbContextAsync(); // Don't worry about no tracking, it's already disabled by default

        var keyValues = ParseKey(key);
        var entity = await dbContext.Set<T>().FindAsync(keyValues); // Find entity in MySQL

        if (entity != null)
            _cache.TryAdd(key, entity); // Add to cache for future use

        return entity;
    }

    private object[] ParseKey(string key)
    {
        return key.Split('.').Skip(1).Select(i => (object)int.Parse(i)).ToArray();
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression)
    {
        var predicate = expression.Compile(); // Apparently not needed to cache...
        var match = _cache.Values.FirstOrDefault(predicate); // Find in cache first
        if (match != null)
            return match; // Hit

        await using var dbContext = await NetworkService.ContextFactory.CreateDbContextAsync();

        var query = dbContext.Set<T>().AsQueryable(); // MySQL fallback

        foreach (var path in T.GetIncludes()) // Need the model to tell us the path of the child models to include in the query
            query = query.Include(path);
        
        match = await query.FirstOrDefaultAsync(expression);

        if (match != null)
            _cache.TryAdd(match.Key, match);

        return match;
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
    {
        var predicate = expression.Compile();
        var match = _cache.Values.Any(predicate); // Find in cache first
        if (match)
            return match; // Hit

        await using var dbContext = await NetworkService.ContextFactory.CreateDbContextAsync();

        return await dbContext.Set<T>().AnyAsync(expression);
    }

    public async Task<int> CountAsync(Func<T, bool> predicate)
    {
        var matches = new HashSet<string>(); // Stores ID of matching entity
        
        foreach (var entity in _cache.Values) // Find matches in cache
            if (predicate(entity))
                matches.Add(entity.Key);
        
        await using var dbContext = await NetworkService.ContextFactory.CreateDbContextAsync();
        
        foreach (var entity in dbContext.Set<T>()) // Find matches in MySQL
            if (predicate(entity))
                matches.Add(entity.Key); // HashSet won't add the same (string) key twice
        
        return matches.Count;
    }

    #endregion

    public async Task AddAsync(T entity)
    {
        var newEntity = await GetAsync(entity.Key) == null;
        if (!newEntity)
            return;

        if (_new.Add(entity))
            entity.Version++;
    }
    
    // Update values from existing entities
    public void Update(T entity, params Expression<Func<T, object>>[] properties) // Properties need to be marked as dirty or they won't be updated in the database
    {
        foreach (var propertyExpression in properties)
        {
            var memberExpression = (MemberExpression)propertyExpression.Body;
            var property = (PropertyInfo)memberExpression.Member;

            DirtyProperty(entity, property.Name);
        }
        
        entity.Version++;
    }
    
    public void Update(T entity, params string[] properties) // If you need to update and you have the property names in a string[]
    {
        foreach (var propName in properties)
            DirtyProperty(entity, propName);

        entity.Version++;
    }

    private void DirtyProperty(T entity, string propertyName)
    {
        var props = _dirty.GetOrAdd(entity, new HashSet<string>());
        props.Add(propertyName);
    }

    public void Save(AlloyContext dbContext) // Returns true if changes were made to this table
    {
        var set = dbContext.Set<T>();
        foreach (var kvp in _dirty)
        {
            var entity = kvp.Key;
            var properties = kvp.Value;

            set.Attach(entity);
            foreach (var property in properties) // Mark the modified properties as changed. This is apparently the most efficient way 
                dbContext.Entry(entity).Property(property).IsModified = true;
        }

        foreach (var entity in _new)
            set.Add(entity);

        _dirty.Clear();
        _new.Clear();
    }
}