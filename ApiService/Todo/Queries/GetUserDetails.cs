namespace ApiService.Todo.Queries;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Data;
using DTO;

public record GetUserDetailsQuery : IRequest<IEnumerable<UserDetailsItem>>;

public class GetUserDetailsQueryHandler(UserInputDbContext context) : IRequestHandler<GetUserDetailsQuery, IEnumerable<UserDetailsItem>>
{
    public async Task<IEnumerable<UserDetailsItem>> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
    {
        return await context.UserInputs
            .Select(x => new UserDetailsItem
            {
                Id = x.Id,
                RawInput = x.RawInput,
                FullName = x.FullName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                AddressLine = x.AddressLine,
                City = x.City,
                State = x.State,
                Zip = x.Zip,
                Confidence = x.Confidence,
                Notes = x.Notes,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .ToListAsync(cancellationToken);
    }
}

public record GetUserDetailsByIdQuery(int Id) : IRequest<UserDetailsItem?>;

public class GetUserDetailsByIdQueryHandler(UserInputDbContext context) : IRequestHandler<GetUserDetailsByIdQuery, UserDetailsItem?>
{
    public async Task<UserDetailsItem?> Handle(GetUserDetailsByIdQuery request, CancellationToken cancellationToken)
    {
        return await context.UserInputs
            .Where(x => x.Id == request.Id)
            .Select(x => new UserDetailsItem
            {
                Id = x.Id,
                RawInput = x.RawInput,
                FullName = x.FullName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                AddressLine = x.AddressLine,
                City = x.City,
                State = x.State,
                Zip = x.Zip,
                Confidence = x.Confidence,
                Notes = x.Notes,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
