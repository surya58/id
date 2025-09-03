namespace ApiService.Todo.DTO;

public class UserDetailsItem
{
    public int Id { get; set; }
    public string? RawInput { get; set; }
    public string? FullName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? AddressLine { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public double? Confidence { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public record CreateUserDetailsRequest(string RawInput);

public record UpdateUserDetailsDto(string? RawInput);
