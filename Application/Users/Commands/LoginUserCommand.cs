using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public class LoginUserCommand : IRequest<Result<string, UserException>>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand, Result<string, UserException>>
{
    private readonly IUserRepository _repository;
    private readonly ITokenGenerator _tokenGenerator;

    public LoginUserCommandHandler(IUserRepository repository, ITokenGenerator tokenGenerator)
    {
        _repository = repository;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<string, UserException>> Handle(LoginUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = await _repository.GetByEmailAndPassword(request.Email, request.Password, cancellationToken);

        return await user.Match(
            async u => await GenerateToken(u),
            () => Task.FromResult<Result<string, UserException>>(new UserPasswordOrEmailNotFoundExceptions(UserId.Empty())));
     }

    private async Task<Result<string, UserException>> GenerateToken(User user)
    {
        try
        {
            var token = _tokenGenerator.GenerateToken(user);
            return token;  
        }
        catch (Exception e)
        {
            return new UserUnknownException(user.Id, e);
        }
    }
}