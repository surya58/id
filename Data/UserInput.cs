
namespace Data;

/// <summary>
/// User input record for tracking raw input and parsed results
/// This entity stores both the original user input and the structured output from Groq API
/// </summary>
public class UserInput
{
    public int Id { get; set; }
    
    /// <summary>
    /// The original raw input from the user (e.g., "John Doe, 123 Main St, Anytown, CA 90210")
    /// </summary>
    public string RawInput { get; set; } = string.Empty;
    
    /// <summary>
    /// Full name as parsed by Groq API
    /// </summary>
    public string? FullName { get; set; }
    
    /// <summary>
    /// First name extracted from full name
    /// </summary>
    public string? FirstName { get; set; }
    
    /// <summary>
    /// Last name extracted from full name
    /// </summary>
    public string? LastName { get; set; }
    
    /// <summary>
    /// Street address line (e.g., "123 Main St")
    /// </summary>
    public string? AddressLine { get; set; }
    
    /// <summary>
    /// City name
    /// </summary>
    public string? City { get; set; }
    
    /// <summary>
    /// State code or name
    /// </summary>
    public string? State { get; set; }
    
    /// <summary>
    /// ZIP/Postal code
    /// </summary>
    public string? Zip { get; set; }
    
    /// <summary>
    /// Confidence score from Groq API parsing (0.0 to 1.0)
    /// </summary>
    public double? Confidence { get; set; }
    
    /// <summary>
    /// Any notes or warnings from the parsing process
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// When this input was processed
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// When this record was last updated (if re-parsed)
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
