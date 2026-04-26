using System.Collections;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using Common.Database;
using Common.Utilities;
using DbServer.Service;
using Microsoft.EntityFrameworkCore;

namespace DbServer.Database;

public class DbEntityCache<T> where T : DbModel, IDbQueryable {
    private readonly ConcurrentDictionary<string, T> _cache = new();
    private readonly HashSet<T> _deleted = new(); // Tracks deleted entities

    private readonly ConcurrentDictionary<T, HashSet<string>>
        _dirty = new(); // Tracks modified properties in a given entity

    private readonly HashSet<T> _new = new(); // Tracks new entities

    public async Task AddAsync(T entity) {
        if (!_new.Add(entity))
            return;

        entity.Version++;
        await DbCache.AddChildrenAsync(entity);
    }

    public async Task DeleteAsync(T entity) {
        if (!_deleted.Add(entity))
            return;

        await DbCache.DeleteChildrenAsync(entity);
    }

    public void CacheEntity(T entity) {
        if (_cache.TryGetValue(entity.Key, out var cached))
            return;

        _cache.TryAdd(entity.Key, entity);
    }

    // Update values from existing entities
    public void
        Update(T entity,
            params Expression<Func<T, object>>[] properties) // Properties need to be marked as dirty or they won't be updated in the database
    {
        foreach (var propertyExpression in properties) {
            var memberExpression = GetMemberExpression(propertyExpression.Body);
            var property = (PropertyInfo)memberExpression.Member;

            DirtyProperty(entity, property.Name);
        }

        entity.Version++;
    }

    private MemberExpression GetMemberExpression(Expression body) {
        if (body is MemberExpression memberExpression)
            return memberExpression;
        if (body is UnaryExpression unaryExpression)
            return unaryExpression.Operand as MemberExpression;
        throw new ArgumentException($"Expression {body} is not supported");
    }

    public void
        Update(T entity,
            params string[] properties) // If you need to update and you have the property names in a string[]
    {
        foreach (var propName in properties)
            DirtyProperty(entity, propName);

        entity.Version++;
    }

    private void DirtyProperty(T entity, string propertyName) {
        var props = _dirty.GetOrAdd(entity, new HashSet<string>());
        props.Add(propertyName);
    }

    public void Save(AlloyContext dbContext) // Returns true if changes were made to this table
    {
        var set = dbContext.Set<T>();
        foreach (var kvp in _dirty) {
            var entity = kvp.Key;
            var properties = kvp.Value;

            set.Attach(entity);
            foreach (var property in
                     properties) // Mark the modified properties as changed. This is apparently the most efficient way 
                dbContext.Entry(entity).Property(property).IsModified = true;
        }

        foreach (var entity in _new) // Don't cache entities here, they don't have PKs assigned yet
            set.Add(entity);

        foreach (var entity in _deleted)
            set.Remove(entity);

        _dirty.Clear();
        _new.Clear();
    }

    #region Query Methods

    public async Task<T?> GetAsync(string key) {
        if (_cache.TryGetValue(key, out var cached)) // Cache is the source of truth, search here first
            return cached; // Cache hit

        // Cache miss, find in MySQL
        await using var
            dbContext = await NetworkService.ContextFactory
                .CreateDbContextAsync(); // Don't worry about no tracking, it's already disabled by default

        Logger.Debug($"Cache miss for: {key}");

        var keyValues = ParseKey(key);
        var entity = await dbContext.Set<T>().FindAsync(keyValues); // Find entity in MySQL

        if (entity != null) {
            _cache.TryAdd(entity.Key, entity); // Add the parent entity to this cache

            foreach (var path in T.GetIncludes()) // Load children models
                await LoadNavigationPath(dbContext, entity, path);

            DbCache.CacheChildren(entity);
        }

        return entity;
    }

    private static async Task LoadNavigationPath(DbContext dbContext, object entity, string path) {
        var parts = path.Split('.');
        var currentEntity = entity;

        for (var i = 0; i < parts.Length; i++) {
            if (currentEntity == null)
                break;

            var part = parts[i];
            var entry = dbContext.Entry(currentEntity);
            var navigation = entry.Navigation(part);

            if (!navigation.IsLoaded)
                await navigation.LoadAsync();

            var currentValue = navigation.CurrentValue;
            // If this is a collection and there are more parts in the path
            if (currentValue is IEnumerable enumerable && i < parts.Length - 1) {
                // Load the remaining path for each item in the collection
                var remainingPath = string.Join(".", parts.Skip(i + 1));

                foreach (var item in enumerable)
                    if (item != null)
                        await LoadNavigationPath(dbContext, item, remainingPath);

                break; // We've handled the rest recursively
            }

            currentEntity = currentValue;
        }
    }

    private object[] ParseKey(string key) {
        return key.Split('.').Skip(1).Select(i => (object)int.Parse(i)).ToArray();
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression) {
        var predicate = expression.Compile(); // Apparently not needed to cache...
        var match = _cache.Values.FirstOrDefault(predicate); // Find in cache first
        if (match != null)
            return match; // Hit

        await using var dbContext = await NetworkService.ContextFactory.CreateDbContextAsync();

        var query = dbContext.Set<T>().AsQueryable(); // MySQL fallback

        foreach (var path in
                 T.GetIncludes()) // Need the model to tell us the path of the child models to include in the query
            query = query.Include(path);

        match = await query.FirstOrDefaultAsync(expression);

        if (match != null) // Entity found
        {
            _cache.TryAdd(match.Key, match); // Add the parent entity to this cache
            DbCache.CacheChildren(match);
        }

        return match;
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression) {
        var predicate = expression.Compile();
        var match = _cache.Values.Any(predicate); // Find in cache first
        if (match)
            return match; // Hit

        await using var dbContext = await NetworkService.ContextFactory.CreateDbContextAsync();

        return await dbContext.Set<T>().AnyAsync(expression);
    }

    public async Task<int> CountAsync(Func<T, bool> predicate) {
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
}