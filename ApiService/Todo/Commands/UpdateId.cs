namespace ApiService.Todo.Commands;

using MediatR;
using System.Text.Json;
using ApiService.Python;
using ApiService.Python.Models;
using Data;
using Microsoft.EntityFrameworkCore;

public record UpdateUserDetailsCommandDto(string? RawInput);

public record UpdateUserDetailsCommand(int Id, UpdateUserDetailsCommandDto Dto) : IRequest<Unit>;

public class UpdateUserDetailsCommandHandler(UserInputDbContext context, PythonClient pythonClient) 
    : IRequestHandler<UpdateUserDetailsCommand, Unit>
{
    public async Task<Unit> Handle(UpdateUserDetailsCommand request, CancellationToken cancellationToken)
    {
        var userDetails = await context.UserInputs.FindAsync([request.Id], cancellationToken: cancellationToken);
        
        if (userDetails == null)
        {
            throw new KeyNotFoundException($"UserInput with ID {request.Id} not found.");
        }

        var inputChanged = userDetails.RawInput != request.Dto.RawInput;
        userDetails.RawInput = request.Dto.RawInput ?? string.Empty;
        userDetails.UpdatedAt = DateTime.UtcNow;

        // Re-parse if user input changed
        if (inputChanged && !string.IsNullOrWhiteSpace(request.Dto.RawInput))
        {
            try
            {
                var result = await pythonClient.ParseUserDetailsAsync(request.Dto.RawInput);

                var parsed = JsonSerializer.Deserialize<StructuredUserDetails>(
                    result,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (parsed != null)
                {
                    userDetails.FullName    = parsed.FullName;
                    userDetails.FirstName   = parsed.FirstName;
                    userDetails.LastName    = parsed.LastName;
                    userDetails.AddressLine = parsed.AddressLine;
                    userDetails.City        = parsed.City;
                    userDetails.State       = parsed.State;
                    userDetails.Zip         = parsed.Zip;
                }
            }
            catch
            {
                // Keep existing structured data on failure
            }
        }

        await context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
