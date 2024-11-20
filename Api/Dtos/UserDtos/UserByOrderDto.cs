using Api.Dtos.OrderDtos;
using Domain.Orders;
using Domain.Users;

namespace Api.Dtos.UserDtos;

public record UserByOrderDto(
    string FirstName,
    string LastName,
    string Email,
    List<OrderDto> Orders) 
{
   
    public static UserByOrderDto FromUser(User user, List<Order> orders)
        => new(
            FirstName: user.FirstName,
            LastName: user.LastName,
            Email: user.Email,
            Orders: orders.Select(order => OrderDto.FromDomainModel(order)).ToList()); 
}