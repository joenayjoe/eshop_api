using AutoMapper;
using eshop_api.DTO;
using eshop_api.Exceptions;
using eshop_api.Interfaces;
using eshop_api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eshop_api.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public OrdersController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDTO orderDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await unitOfWork.Customers.GetOneByQueryAsync(q => q.Id == orderDTO.CustomerId);
            if (customer == null)
            {
                throw new EntityNotFoundException($"Customer not found");
            }

            var cartItems = await unitOfWork.ShoppingCartRecords.GetAllAsync(q => q.CustomerId == orderDTO.CustomerId, includes: new List<string> { "Product" });
            if (cartItems == null || cartItems.Count <= 0)
            {
                throw new EntityNotFoundException($"Cart is empty.");
            }

            var order = new Order
            {
                CustomerId = customer.Id,
            };

            List<OrderDetail> orderDetails = new();
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    Quantity = item.Quantity,
                    ProductId = item.Product.Id,
                    Order = order,
                    UnitCost = item.Product.CurrentPrice,
                };
                orderDetails.Add(orderDetail);
            }
            order.OrderDetails = orderDetails;

            await unitOfWork.Orders.InsertAsync(order);
            unitOfWork.ShoppingCartRecords.DeleteRange(cartItems);
            await unitOfWork.SaveAsync();
            order.Customer = customer;
            var result = mapper.Map<OrderDTO>(order);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int customerId)
        {
            var customer = await unitOfWork.Customers.GetOneByQueryAsync(q => q.Id == customerId, includes: new List<string> { "Orders.OrderDetails.Product" });
            if (customer == null)
            {
                throw new EntityNotFoundException($"Customer not found");
            }
            var result = mapper.Map<IEnumerable<OrderDTO>>(customer.Orders);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOrderById(int id, [FromQuery] int customerId)
        {
            var customer = await unitOfWork.Customers.GetOneByQueryAsync(q => q.Id == customerId);
            if (customer == null)
            {
                throw new EntityNotFoundException($"Customer not found");
            }
            var order = await unitOfWork.Orders.GetOneByQueryAsync(q => q.Id == id && q.CustomerId == customerId, includes: new List<string> { "Customer", "OrderDetails" });
            if (order == null)
            {
                throw new EntityNotFoundException($"Order with id {id} not found");
            }
            var result = mapper.Map<OrderDTO>(order);
            return Ok(result);
        }
    }
}
