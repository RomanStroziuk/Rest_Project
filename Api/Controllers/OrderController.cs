using Api.Dtos;
using Api.Dtos.OrderDtos;
using Api.Modules.Errors;
using Application.Orders.Commands;
using Application.Common.Interfaces.Queries;
using Domain.Orders;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("orders")]
[ApiController]
public class OrderController(ISender sender, IOrderQueries orderQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetAll(CancellationToken cancellationToken)
    {
        var entities = await orderQueries.GetAll(cancellationToken);
        return entities.Select(OrderDto.FromDomainModel).ToList();
    }

    [HttpGet("{orderId:guid}")]
    public async Task<ActionResult<OrderDto>> Get([FromRoute] Guid orderId, CancellationToken cancellationToken)
    {
        var entity = await orderQueries.GetById(new OrderId(orderId), cancellationToken);
        
        return entity.Match<ActionResult<OrderDto>>(
            order => OrderDto.FromDomainModel(order),
            () => NotFound());
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto request, CancellationToken cancellationToken)
    {
        var input = new CreateOrderCommand
        {
            OrderDate = request.OrderDate,
            UserId = request.UserId,
            StatusId = request.StatusId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<OrderDto>>(
            order => OrderDto.FromDomainModel(order),
            e => e.ToObjectResult());
    }

    [HttpPut]
    public async Task<ActionResult<SetStatusOrderDto>> UpdateStatus([FromBody] SetStatusOrderDto request, CancellationToken cancellationToken)
    {
        var input = new SetStatusCommand
        {
            OrderId = request.OrderId,
            StatusId = request.StatusId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<SetStatusOrderDto>>(
            order => SetStatusOrderDto.FromDomainModel(order),
            e => e.ToObjectResult());
    }


    [HttpDelete("{orderId:guid}")]
    public async Task<ActionResult<OrderDto>> Delete([FromRoute] Guid orderId, CancellationToken cancellationToken)
    {
        var input = new DeleteOrderCommand 
        {
            OrderId = orderId
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<OrderDto>>(
            order => OrderDto.FromDomainModel(order),
            e => e.ToObjectResult());
    }
}
