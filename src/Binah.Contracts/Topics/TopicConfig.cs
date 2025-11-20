namespace Binah.Contracts.Topics;

public class TopicConfig
{
    public int Partitions { get; set; }
    public int ReplicationFactor { get; set; }
    public long RetentionMs { get; set; }
    public string CleanupPolicy { get; set; } = "delete";

    public static Dictionary<string, TopicConfig> GetTopicConfigs()
    {
        return new Dictionary<string, TopicConfig>
        {
            [KafkaTopics.EntityCreated] = new TopicConfig
            {
                Partitions = 10,
                ReplicationFactor = 3,
                RetentionMs = 604800000, // 7 days
                CleanupPolicy = "delete"
            },
            [KafkaTopics.EntityUpdated] = new TopicConfig
            {
                Partitions = 10,
                ReplicationFactor = 3,
                RetentionMs = 604800000, // 7 days
                CleanupPolicy = "delete"
            },
            [KafkaTopics.EntityDeleted] = new TopicConfig
            {
                Partitions = 10,
                ReplicationFactor = 3,
                RetentionMs = 604800000, // 7 days
                CleanupPolicy = "delete"
            },
            [KafkaTopics.RelationshipCreated] = new TopicConfig
            {
                Partitions = 5,
                ReplicationFactor = 3,
                RetentionMs = 604800000, // 7 days
                CleanupPolicy = "delete"
            },
            [KafkaTopics.RelationshipUpdated] = new TopicConfig
            {
                Partitions = 5,
                ReplicationFactor = 3,
                RetentionMs = 604800000, // 7 days
                CleanupPolicy = "delete"
            },
            [KafkaTopics.RelationshipDeleted] = new TopicConfig
            {
                Partitions = 5,
                ReplicationFactor = 3,
                RetentionMs = 604800000, // 7 days
                CleanupPolicy = "delete"
            }
        };
    }
}