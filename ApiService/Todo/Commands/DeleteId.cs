namespace ApiService.Todo.Commands;

using MediatR;
using Data;

public record DeleteUserDetailsCommand(int Id) : IRequest<Unit>;

public class DeleteUserDetailsCommandHandler(UserInputDbContext context) : IRequestHandler<DeleteUserDetailsCommand, Unit>
{
    public async Task<Unit> Handle(DeleteUserDetailsCommand request, CancellationToken cancellationToken)
    {
        // Find the user details entry by Id
        var userDetails = await context.UserInputs.FindAsync([request.Id], cancellationToken: cancellationToken);

        if (userDetails == null)
        {
            // Entry not found, return gracefully
            return Unit.Value;
        }

        // Remove the entity
        context.UserInputs.Remove(userDetails);

        // Persist changes to the database
        await context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
