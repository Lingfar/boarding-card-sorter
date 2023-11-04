using Domain.BoardingCards;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Npgsql.NameTranslation;
using System.Text.RegularExpressions;

namespace Persistence.DbContexts.Extensions;

public static partial class Extensions
{
    public static void RegisterEnums(this NpgsqlDataSourceBuilder dataSourceBuilder)
    {
        dataSourceBuilder.MapEnum<BoardingCardType>(nameTranslator: new NpgsqlSnakeCaseNameTranslator());
    }

    public static void UseEnums(this ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresEnum<BoardingCardType>();
    }

    /// <summary>
    ///     Convert all the entities names to snake case (PostgreSQL default naming convention).
    /// </summary>
    /// <param name="modelBuilder">Model builder.</param>
    public static void ApplyPostgresNamingConventions(this ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Replace table names
            entity.SetTableName(entity.GetTableName()?.ToSnakeCase());

            // Replace column names
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.Name.ToSnakeCase());
            }

            // Replace keys names
            foreach (var key in entity.GetKeys())
            {
                key.SetName(key.GetName()?.ToSnakeCase());
            }

            // Replace foreign keys names
            foreach (var key in entity.GetForeignKeys())
            {
                key.SetConstraintName(key.GetConstraintName()?.ToSnakeCase());
            }

            // Replace indexes names
            foreach (var index in entity.GetIndexes())
            {
                index.SetDatabaseName(index.GetDatabaseName()?.ToSnakeCase());
            }
        }
    }

    private static string ToSnakeCase(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        var startUnderscores = StartUnderscoreRegex().Match(input);
        return startUnderscores + SnakeCaseRegex().Replace(input, "$1_$2").ToLowerInvariant();
    }

    [GeneratedRegex("^_+")]
    private static partial Regex StartUnderscoreRegex();

    [GeneratedRegex("([a-z0-9])([A-Z])")]
    private static partial Regex SnakeCaseRegex();
}
