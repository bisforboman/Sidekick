using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Sidekick.Common.Database;
using Sidekick.Common.Database.Tables;
using Sidekick.Common.Enums;

namespace Sidekick.Common.Settings;

public class SettingsService(DbContextOptions<SidekickDbContext> dbContextOptions) : ISettingsService
{
    public event Action? OnSettingsChanged;

    public async Task<bool> GetBool(string key, bool defaultValue)
    {
        await using var dbContext = new SidekickDbContext(dbContextOptions);
        var dbSetting = await dbContext
                              .Settings.Where(x => x.Key == key)
                              .FirstOrDefaultAsync();
        if (dbSetting != null && bool.TryParse(dbSetting.Value, out var boolValue))
        {
            return boolValue;
        }

        return defaultValue;
    }

    public async Task<string> GetString(string key, string defaultValue)
    {
        await using var dbContext = new SidekickDbContext(dbContextOptions);
        var dbSetting = await dbContext
                              .Settings.Where(x => x.Key == key)
                              .FirstOrDefaultAsync();
        if (dbSetting?.Value is not null)
        {
            return dbSetting.Value ?? defaultValue;
        }
        
        return defaultValue;
    }

    public async Task<int> GetInt(string key, int defaultValue)
    {
        await using var dbContext = new SidekickDbContext(dbContextOptions);
        var dbSetting = await dbContext
                              .Settings.Where(x => x.Key == key)
                              .FirstOrDefaultAsync();
        if (dbSetting != null && int.TryParse(dbSetting.Value, out var intValue))
        {
            return intValue;
        }

        return defaultValue;
    }

    public async Task<DateTimeOffset> GetDateTime(string key, DateTimeOffset defaultValue)
    {
        await using var dbContext = new SidekickDbContext(dbContextOptions);
        var dbSetting = await dbContext
                              .Settings.Where(x => x.Key == key)
                              .FirstOrDefaultAsync();
        if (dbSetting != null && DateTimeOffset.TryParse(dbSetting.Value, out var dateTimeValue))
        {
            return dateTimeValue;
        }

        return defaultValue;
    }

    public async Task<TValue> GetObject<TValue>(string key, TValue defaultValue)
    {
        await using var dbContext = new SidekickDbContext(dbContextOptions);
        var dbSetting = await dbContext
                              .Settings.Where(x => x.Key == key)
                              .FirstOrDefaultAsync();

        if (dbSetting?.Value is not null)
        {
            return JsonSerializer.Deserialize<TValue>(dbSetting.Value)
                ?? defaultValue;
        }

        return defaultValue;
    }

    public async Task<TEnum> GetEnum<TEnum>(string key, TEnum defaultValue)
        where TEnum : struct, Enum
    {
        await using var dbContext = new SidekickDbContext(dbContextOptions);
        var dbSetting = await dbContext
                              .Settings.Where(x => x.Key == key)
                              .FirstOrDefaultAsync();
        if (dbSetting != null && Enum.TryParse<TEnum>(dbSetting.Value, out var enumValue))
        {
            return enumValue;
        }

        return defaultValue;
    }

    public async Task Set(
        string key,
        object? value)
    {
        var stringValue = GetStringValue(value);

        await using var dbContext = new SidekickDbContext(dbContextOptions);
        var dbSetting = await dbContext
                              .Settings.Where(x => x.Key == key)
                              .FirstOrDefaultAsync();

        if (stringValue == null)
        {
            if (dbSetting == null)
            {
                return;
            }

            dbContext.Settings.Remove(dbSetting);
            await dbContext.SaveChangesAsync();
            return;
        }

        if (dbSetting == null)
        {
            dbSetting = new Setting()
            {
                Key = key,
                Value = stringValue,
            };
            dbContext.Add(dbSetting);
        }
        else
        {
            dbSetting.Value = stringValue;
        }

        await dbContext.SaveChangesAsync();

        OnSettingsChanged?.Invoke();
    }

    private static string? GetStringValue(object? value)
    {
        if (value == null)
        {
            return null;
        }

        return value switch
        {
            double x => x.ToString(CultureInfo.InvariantCulture),
            int x => x.ToString(),
            DateTimeOffset x => x.ToString(),
            Enum x => x.GetValueAttribute(),
            string x => x,
            _ => JsonSerializer.Serialize(value),
        };
    }
}
