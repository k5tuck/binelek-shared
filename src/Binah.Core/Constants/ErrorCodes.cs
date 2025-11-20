namespace Binah.Core.Constants;

/// <summary>
/// Standardized error codes across the platform
/// </summary>
public static class ErrorCodes
{
    // Entity Errors (1000-1099)
    public const string EntityNotFound = "ENTITY_1001";
    public const string EntityCreationFailed = "ENTITY_1002";
    public const string EntityUpdateFailed = "ENTITY_1003";
    public const string EntityDeletionFailed = "ENTITY_1004";
    public const string EntityValidationFailed = "ENTITY_1005";

    // Relationship Errors (1100-1199)
    public const string RelationshipNotFound = "REL_1101";
    public const string RelationshipCreationFailed = "REL_1102";
    public const string RelationshipDeletionFailed = "REL_1103";

    // Query Errors (1200-1299)
    public const string UnauthorizedQuery = "QUERY_1201";
    public const string QueryExecutionFailed = "QUERY_1202";
    public const string QueryValidationFailed = "QUERY_1203";

    // Database Errors (1300-1399)
    public const string DatabaseConnectionFailed = "DB_1301";
    public const string DatabaseQueryFailed = "DB_1302";
    public const string DatabaseTransactionFailed = "DB_1303";

    // Event Errors (1400-1499)
    public const string EventPublishFailed = "EVENT_1401";
    public const string EventSerializationFailed = "EVENT_1402";

    // Service Communication Errors (1500-1599)
    public const string ServiceUnavailable = "SERVICE_1501";
    public const string ServiceTimeout = "SERVICE_1502";
    public const string ServiceAuthenticationFailed = "SERVICE_1503";

    // Validation Errors (1600-1699)
    public const string ValidationFailed = "VALIDATION_1601";
    public const string InvalidInput = "VALIDATION_1602";

    // Generic Errors
    public const string InternalError = "ERROR_9999";
    public const string NotImplemented = "ERROR_9998";
}
