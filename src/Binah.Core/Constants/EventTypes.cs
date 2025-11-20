namespace Binah.Core.Constants;

/// <summary>
/// Standardized event types across the platform
/// </summary>
public static class EventTypes
{
    // Entity Events
    public const string EntityCreated = "entity.created";
    public const string EntityUpdated = "entity.updated";
    public const string EntityDeleted = "entity.deleted";

    // Relationship Events
    public const string RelationshipCreated = "relationship.created";
    public const string RelationshipDeleted = "relationship.deleted";

    // Query Events
    public const string QueryExecuted = "query.executed";

    // Version Events
    public const string VersionCreated = "version.created";
    public const string VersionRestored = "version.restored";
}
