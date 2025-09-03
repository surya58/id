namespace ApiService.Python.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Request model for parsing user input containing name and address details
/// </summary>
public record UserDetailsParseRequest(string Input);

/// <summary>
/// Response model containing structured user details parsed from raw input
/// </summary>
public record UserDetailsParseResponse(
    [property: JsonPropertyName("full_name")] string FullName,
    [property: JsonPropertyName("first_name")] string FirstName, 
    [property: JsonPropertyName("last_name")] string LastName,
    [property: JsonPropertyName("address_line")] string AddressLine,
    [property: JsonPropertyName("city")] string City,
    [property: JsonPropertyName("state")] string State,
    [property: JsonPropertyName("zip")] string Zip,
    [property: JsonPropertyName("confidence")] double? Confidence,
    [property: JsonPropertyName("notes")] string? Notes
);

/// <summary>
/// Alias for UserDetailsParseResponse to match the naming used in commands
/// </summary>
public record StructuredUserDetails(
    [property: JsonPropertyName("full_name")] string FullName,
    [property: JsonPropertyName("first_name")] string FirstName, 
    [property: JsonPropertyName("last_name")] string LastName,
    [property: JsonPropertyName("address_line")] string AddressLine,
    [property: JsonPropertyName("city")] string City,
    [property: JsonPropertyName("state")] string State,
    [property: JsonPropertyName("zip")] string Zip,
    [property: JsonPropertyName("confidence")] double? Confidence,
    [property: JsonPropertyName("notes")] string? Notes
);

/// <summary>
/// Legacy todo classification model - keeping for backward compatibility
/// </summary>
public record TodoClassification(string Description, string Category, double Confidence);