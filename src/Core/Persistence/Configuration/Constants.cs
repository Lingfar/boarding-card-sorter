namespace Persistence.Configuration;

internal static class Constants
{
    public const string WriteConnectionString = "WriteDatabase";
    public const string ReadConnectionString = "ReadDatabase";

    public const string DefaultSchema = "public";
    public const string PostgresUuidExtension = "uuid-ossp";
    public const string DefaultGuidValueSql = "uuid_generate_v4()";
}
