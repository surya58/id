namespace ApiService.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using ApiService.Todo.Commands;
using ApiService.Todo.Queries;
using ApiService.Todo.DTO;
using ApiService.Python;
using ApiService.Python.Models;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class UserDetailsController(IMediator mediator) : ControllerBase
{
    [HttpGet(Name = nameof(GetUserDetails))]
    public async Task<IEnumerable<UserDetailsItem>> GetUserDetails()
    {
        return await mediator.Send(new GetUserDetailsQuery());
    }

    [HttpGet("{id}", Name = nameof(GetUserDetailsById))]
    public async Task<ActionResult<UserDetailsItem>> GetUserDetailsById(int id)
    {
        var result = await mediator.Send(new GetUserDetailsByIdQuery(id));
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }

    [HttpPost(Name = nameof(CreateUserDetails))]
    public async Task<ActionResult<int>> CreateUserDetails(CreateUserDetailsRequest request)
    {
        return await mediator.Send(new CreateUserDetailsCommand(request.RawInput));
    }

    [HttpPut("{id}", Name = nameof(UpdateUserDetails))]
    public async Task<IActionResult> UpdateUserDetails(int id, UpdateUserDetailsDto dto)
    {
        await mediator.Send(new UpdateUserDetailsCommand(id, new UpdateUserDetailsCommandDto(dto.RawInput)));
        return NoContent();
    }

    [HttpDelete("{id}", Name = nameof(DeleteUserDetails))]
    public async Task<IActionResult> DeleteUserDetails(int id)
    {
        await mediator.Send(new DeleteUserDetailsCommand(id));
        return NoContent();
    }

    [HttpPost("parse", Name = nameof(ParseUserDetails))]
    public async Task<IActionResult> ParseUserDetails([FromBody] UserDetailsParseRequest request)
    {
        try
        {
            // Use the CreateUserDetailsCommand which handles parsing and saving
            var userDetailsId = await mediator.Send(new CreateUserDetailsCommand(request.Input));

            // Get the saved user details from the database to return
            var savedUserDetails = await mediator.Send(new GetUserDetailsByIdQuery(userDetailsId));
            
            if (savedUserDetails == null)
            {
                return BadRequest("Failed to save user details to database");
            }

            // Map to the response format expected by the frontend
            var response = new UserDetailsParseResponse(
                savedUserDetails.FullName ?? "",
                savedUserDetails.FirstName ?? "",
                savedUserDetails.LastName ?? "",
                savedUserDetails.AddressLine ?? "",
                savedUserDetails.City ?? "",
                savedUserDetails.State ?? "",
                savedUserDetails.Zip ?? "",
                savedUserDetails.Confidence,
                savedUserDetails.Notes
            );

            return Ok(response);
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to parse user details: {ex.Message}");
        }
    }
}
