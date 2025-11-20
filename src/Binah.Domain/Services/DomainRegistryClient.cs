using Binah.Contracts.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;
using System.Text.Json;

namespace Binah.Core.Domain.Services;

public interface IDomainRegistryClient
{
    Task<DomainInfo?> GetDomainAsync(string domainId);
    Task<List<EntityDefinition>> GetEntitiesAsync(string domainId);
    Task<EntityDefinition?> GetEntityDefinitionAsync(string domainId, string entityName);
    Task<List<DomainInfo>> GetAllDomainsAsync();
    Task<bool> ValidateDomainAsync(string domainId);
}

public class DomainRegistryClient : IDomainRegistryClient
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<DomainRegistryClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public DomainRegistryClient(
        HttpClient httpClient,
        IMemoryCache cache,
        ILogger<DomainRegistryClient> logger)
    {
        _httpClient = httpClient;
        _cache = cache;
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
    }

    public async Task<DomainInfo?> GetDomainAsync(string domainId)
    {
        var cacheKey = $"domain:{domainId}";

        if (_cache.TryGetValue(cacheKey, out DomainInfo? cached))
        {
            return cached;
        }

        try
        {
            var response = await _httpClient.GetAsync($"/api/domains/{domainId}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get domain {DomainId}: {StatusCode}",
                    domainId, response.StatusCode);
                return null;
            }

            var domain = await response.Content.ReadFromJsonAsync<DomainInfo>(_jsonOptions);

            if (domain != null)
            {
                _cache.Set(cacheKey, domain, TimeSpan.FromMinutes(15));
            }

            return domain;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving domain {DomainId}", domainId);
            return null;
        }
    }

    public async Task<List<EntityDefinition>> GetEntitiesAsync(string domainId)
    {
        var domain = await GetDomainAsync(domainId);
        return domain?.Ontology?.Entities ?? new List<EntityDefinition>();
    }

    public async Task<EntityDefinition?> GetEntityDefinitionAsync(string domainId, string entityName)
    {
        var cacheKey = $"entity:{domainId}:{entityName}";

        if (_cache.TryGetValue(cacheKey, out EntityDefinition? cached))
        {
            return cached;
        }

        try
        {
            var response = await _httpClient.GetAsync($"/api/domains/{domainId}/entities/{entityName}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var entity = await response.Content.ReadFromJsonAsync<EntityDefinition>(_jsonOptions);

            if (entity != null)
            {
                _cache.Set(cacheKey, entity, TimeSpan.FromMinutes(15));
            }

            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving entity {EntityName} from domain {DomainId}",
                entityName, domainId);
            return null;
        }
    }

    public async Task<List<DomainInfo>> GetAllDomainsAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/domains");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get all domains: {StatusCode}", response.StatusCode);
                return new List<DomainInfo>();
            }

            var domains = await response.Content.ReadFromJsonAsync<List<DomainInfo>>(_jsonOptions);
            return domains ?? new List<DomainInfo>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all domains");
            return new List<DomainInfo>();
        }
    }

    public async Task<bool> ValidateDomainAsync(string domainId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/domains/{domainId}/validate");
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<ValidationResult>(_jsonOptions);
            return result?.IsValid ?? false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating domain {DomainId}", domainId);
            return false;
        }
    }
}

// DTOs for domain registry responses

public class DomainInfo
{
    public DomainMetadata? Metadata { get; set; }
    public DomainOntology? Ontology { get; set; }
    public DomainUIConfig? UIConfig { get; set; }
    public string? DirectoryPath { get; set; }
}

public class DomainMetadata
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Version { get; set; } = null!;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
}

public class DomainOntology
{
    public List<EntityDefinition> Entities { get; set; } = new();
    public List<RelationshipDefinition>? Relationships { get; set; }
}

public class EntityDefinition
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? Color { get; set; }
    public List<AttributeDefinition> Attributes { get; set; } = new();
}

public class AttributeDefinition
{
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string? Description { get; set; }
    public bool Required { get; set; }
    public bool Indexed { get; set; }
    public bool Unique { get; set; }
    public object? Default { get; set; }
    public List<string>? Values { get; set; }  // For enums
    public string? Unit { get; set; }
}

public class RelationshipDefinition
{
    public string Name { get; set; } = null!;
    public string From { get; set; } = null!;
    public string To { get; set; } = null!;
    public string Type { get; set; } = "one-to-many";
    public string? Description { get; set; }
}

public class DomainUIConfig
{
    // Simplified for now - can be expanded later
    public Dictionary<string, object>? Theme { get; set; }
    public Dictionary<string, object>? Navigation { get; set; }
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public DateTime ValidatedAt { get; set; }
}
