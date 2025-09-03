namespace ApiService.Todo.Commands;

using MediatR;
using System.Text.Json;
using ApiService.Python;
using ApiService.Python.Models;
using Data;

public record CreateUserDetailsCommand(string RawInput) : IRequest<int>;

public class CreateUserDetailsCommandHandler(UserInputDbContext context, PythonClient pythonClient) 
    : IRequestHandler<CreateUserDetailsCommand, int>
{
    public async Task<int> Handle(CreateUserDetailsCommand request, CancellationToken cancellationToken)
    {
        StructuredUserDetails? parsedDetails = null;

        try
        {
            if (!string.IsNullOrWhiteSpace(request.RawInput))
            {
                // Call your Python API, which uses Groq internally
                var result = await pythonClient.ParseUserDetailsAsync(request.RawInput);

                parsedDetails = JsonSerializer.Deserialize<StructuredUserDetails>(
                    result,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
        }
        catch
        {
            parsedDetails = null; // continue and store raw input
        }

        var entity = new Data.UserInput
        {
            RawInput   = request.RawInput,
            FullName   = parsedDetails?.FullName,
            FirstName  = parsedDetails?.FirstName,
            LastName   = parsedDetails?.LastName,
            AddressLine= parsedDetails?.AddressLine,
            City       = parsedDetails?.City,
            State      = parsedDetails?.State,
            Zip        = parsedDetails?.Zip,
            Confidence = parsedDetails?.Confidence,
            Notes      = parsedDetails?.Notes
        };

        context.UserInputs.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}
