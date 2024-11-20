using Api.Dtos.OrderItemDtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.OrderItems.Commands;
using Domain.OrderItems;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("order-item")]
[ApiController]
public class OrderItemController(ISender sender,
    IOrderItemRepository orderItemRepository,
    IOrderItemQueries orderItemQueries) : ControllerBase
{
    
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OrderItemDto>>> GetAll(CancellationToken cancellationToken)
    {
        var orderItems = await orderItemQueries.GetAll(cancellationToken);
        return orderItems.Select(OrderItemDto.FromDomainModel).ToList();
    }
    
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderItemDto>> Get([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var orderItem = await orderItemRepository.GetById(new OrderItemId(id), cancellationToken);
        return orderItem.Match<ActionResult<OrderItemDto>>(
            s => Ok(OrderItemDto.FromDomainModel(s)),
            () => NotFound());
    }
    
    [HttpPost]
    public async Task<ActionResult<OrderItemDto>> Create([FromBody] OrderItemDto request, CancellationToken cancellationToken)
    {
        var input = new CreateOrderItemCommand()
        {
            OrderId = request.OrderId,
            SneakerWarehouseId = request.SneakerWarehouseId,
            Quantity = request.Quantity
        };
        
        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<OrderItemDto>>(
            s => Ok(OrderItemDto.FromDomainModel(s)),
            e => e.ToObjectResult());
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<OrderItemDto>> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var input = new DeleteOrderItemCommand()
        {
            Id = id
        };
        
        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<OrderItemDto>>(
            s => Ok(OrderItemDto.FromDomainModel(s)),
            e => e.ToObjectResult());
    }
    
    [HttpPut]
    public async Task<ActionResult<OrderItemDto>> Update( [FromBody] OrderItemDto request, CancellationToken cancellationToken)
    {
        var input = new UpdateOrderItemCommand()
        {
            Id = request.Id!.Value,
            Quantity = request.Quantity,
            TotalPrice = request.TotalPrice
        };
        
        var result = await sender.Send(input, cancellationToken);
        
        return result.Match<ActionResult<OrderItemDto>>(
            s => Ok(OrderItemDto.FromDomainModel(s)),
            e => e.ToObjectResult());
    }
}