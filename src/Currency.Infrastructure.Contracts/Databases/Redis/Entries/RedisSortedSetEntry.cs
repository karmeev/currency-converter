namespace Currency.Infrastructure.Contracts.Databases.Redis.Entries;

public readonly record struct RedisSortedSetEntry(
    string Value, double Score);
