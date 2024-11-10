using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public class UpdateUserFirstAndLastNameCommand : IRequest<Result<User, UserException>>
{
    public required Guid UserId { get; init; }
    
    public required string FirstName { get; init; }
    
    public required string LastName { get; init; }

}

public class UpdateUserFirstAndLastNameCommandHandler(IUserRepository repository)
    : IRequestHandler<UpdateUserFirstAndLastNameCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(UpdateUserFirstAndLastNameCommand request,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var exsitingUser = await repository.GetById(userId, cancellationToken);

        return await exsitingUser.Match(
            async u => await UpdateEntity(u, request.FirstName, request.LastName, cancellationToken),
            () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(userId)));
    }

    private async Task<Result<User, UserException>> UpdateEntity(
        User user,
        string firstName,
        string lastName,
        CancellationToken cancellationToken)
    {
        try
        {
            user.UpdateUserLastAndFirstName(firstName, lastName);

            return await repository.Update(user, cancellationToken);
        }
        catch (Exception e)
        {
            return new UserUnknownException(user.Id, e);
        }
    }
}