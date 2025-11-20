namespace Binah.Contracts.Topics;

public static class KafkaTopics
{
    // Entity topics
    public const string EntityCreated = "binah.ontology.entity.created";
    public const string EntityUpdated = "binah.ontology.entity.updated";
    public const string EntityDeleted = "binah.ontology.entity.deleted";

    // Relationship topics
    public const string RelationshipCreated = "binah.ontology.relationship.created";
    public const string RelationshipUpdated = "binah.ontology.relationship.updated";
    public const string RelationshipDeleted = "binah.ontology.relationship.deleted";

    // System-level topics (domain-agnostic)
    public const string AlertTriggered = "binah.ontology.alert.triggered";

    // NOTE: Domain-specific topics (Property, Transaction, etc.) should be generated
    // from ontology YAML files via the Regen service, not hard-coded here.
    // Each tenant's ontology will define its own entity-specific event topics.

    // Query/enrichment topics
    public const string GraphQueryRequested = "binah.ontology.query.requested";
    public const string EnrichmentRequested = "binah.ontology.enrichment.requested";

    // Pipeline execution topics
    public const string PipelineStarted = "binah.pipeline.execution.started";
    public const string PipelineCompleted = "binah.pipeline.execution.completed";
    public const string PipelineFailed = "binah.pipeline.execution.failed";
    public const string DataIngested = "binah.pipeline.data.ingested";
}