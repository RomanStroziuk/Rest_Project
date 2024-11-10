// using Domain.Orders;
// using Domain.SneakerWarehouses;
// using Domain.Users;
// using Application.Common.Interfaces.Repositories;
// using Application.Orders.Exceptions;
// using MediatR;
// using System.Linq;
// using System.Threading.Tasks;
// using System.Threading;
//
// namespace Application.Orders.Commands
// {
//     public class AddOrderItemAndUpdateTotalPriceCommand : IRequest<Result<Order, OrderException>>
//     {
//         public required OrderId OrderId { get; init; }
//         public required SneakerWarehouseId SneakerWarehouseId { get; init; }
//         public required int Quantity { get; init; }
//     }
//
//     public class AddOrderItemAndUpdateTotalPriceCommandHandler : IRequestHandler<AddOrderItemAndUpdateTotalPriceCommand, Result<Order, OrderException>>
//     {
//         private readonly IOrderRepository _orderRepository;
//         private readonly ISneakerWarehouseRepository _sneakerWarehouseRepository;
//         private readonly IUserRepository _userRepository;
//
//         public AddOrderItemAndUpdateTotalPriceCommandHandler(
//             IOrderRepository orderRepository,
//             ISneakerWarehouseRepository sneakerWarehouseRepository,
//             IUserRepository userRepository)
//         {
//             _orderRepository = orderRepository;
//             _sneakerWarehouseRepository = sneakerWarehouseRepository;
//             _userRepository = userRepository;
//         }
//
//         public async Task<Result<Order, OrderException>> Handle(AddOrderItemAndUpdateTotalPriceCommand request, CancellationToken cancellationToken)
//         {
//             // Завантажуємо замовлення по OrderId
//             var orderId = request.OrderId;
//             var existingOrder = await _orderRepository.GetById(orderId, cancellationToken);
//
//             return await existingOrder.Match(
//                 async order =>
//                 {
//                     // Перевіряємо чи існує склад
//                     var sneakerWarehouse = await _sneakerWarehouseRepository.GetById(request.SneakerWarehouseId, cancellationToken);
//                     if (sneakerWarehouse == null)
//                     {
//                         return new Result<Order, OrderException>(new SneakerWarehouseNotFoundException(request.SneakerWarehouseId));
//                     }
//
//                     // Перевіряємо чи існує користувач для замовлення
//                     var user = await _userRepository.GetById(order.UserId, cancellationToken);
//                     if (user == null)
//                     {
//                         return new Result<Order, OrderException>(new OrderUserNotFoundException(order.UserId));
//                     }
//
//                     // Створюємо новий OrderItem
//                     var newItem = OrderItem.New(new OrderItemId(Guid.NewGuid()), request.SneakerWarehouseId, orderId, request.Quantity);
//
//                     // Додаємо товар до замовлення
//                     order.AddItem(newItem);
//
//                     // Перераховуємо загальну ціну після додавання нового товару
//                     var totalPrice = order.OrderItems.Sum(item =>
//                         item.Quantity * sneakerWarehouse.GetPriceForSneaker(item.SneakerWarehouseId)); // Це залежить від вашої логіки ціноутворення
//
//                     // Оновлюємо загальну ціну замовлення
//                     order.UpdateTotalPrice(totalPrice);
//
//                     // Зберігаємо оновлене замовлення в репозиторії
//                     return await _orderRepository.Update(order, cancellationToken);
//                 },
//                 () => Task.FromResult<Result<Order, OrderException>>(new OrderNotFoundException(orderId))
//             );
//         }
//     }
// }
