# Topic definitions (naming + purpose + schema + retention / partitions)


source.<provider>.raw.v1

Purpose: raw payloads (files, provider JSON).

Schema: provider-specific JSON / opaque.

Partitions: 12 (start)

Retention: 30d (store raw longer in object store)

Consumer groups: parser-service, ingest-audit


ingest.parsed.<type>.v1 (e.g. ingest.parsed.permit.v1)

Purpose: parsed normalized JSON from raw files.

Schema: ingest.parsed.<type>.json

Partitions: 8

Retention: 7d

Consumers: normalizer, indexer


ingest.normalized.property.v1

Purpose: canonical property items (see schema above)

Schema: ingest.normalized.property.v1.json

Partitions: 16 (shard by state or county)

Retention: 14d

Consumers: context-server, staging-dw


ingest.normalized.entity.v1

Purpose: canonical entity items

Partitions: 8

Consumers: resolver-service


resolve.entity_candidate.v1

Purpose: candidate pairs for matching (produced by resolver)

Schema: candidate pair + features

Partitions: 12

Consumers: resolver-ml, human-review-queue


graph.upsert.nodes.v1

Purpose: upsert node commands for Ontology Server / Neo4j

Partitions: 16

Reliability: acks=all


graph.upsert.relationships.v1

Purpose: upsert relationships

Partitions: 16


enrich.geo.features.v1

Purpose: geospatial enrichment outputs (distance, flood flags)

Partitions: 8


model.feature_table.<model_name>.v1

Purpose: feature rows for model training / scoring

Partitions: 24


model.score.<model_name>.v1

Purpose: persisted model scores

Partitions: 12

Retention: 90d (longer for auditing)


audit.provenance.v1

Purpose: lineage/audit events (node/edge changes)

Partitions: 24

Retention: 365d


alerts.realtime.v1

Purpose: real-time alerts pushed to workflows (Slack, email, webhooks)

Partitions: 8

Consumers: notifier-service

<!--Temp Reminder: Partitioning guidance: Partition by geographic shard (state/county) or by uid % partitions to keep related data co-located. Use Kafka Streams or KSQL for re-keying when necessary. -->