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
    [Route("api/shoppingcarts")]
    [ApiController]
    public class ShoppingCartsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ShoppingCartsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        [HttpGet]
        [Route("my_cart")]
        public async Task<IActionResult> GetCart([FromQuery] int customerId)
        {
            var customer = await unitOfWork.Customers.GetOneByQueryAsync(q => q.Id == customerId);
            if (customer == null)
            {
                throw new EntityNotFoundException($"Customer with id {customerId} not found");
            }
            var cart = await GetShoppingCart(customerId);

            if(cart == null || cart.Count <= 0)
            {
                throw new EntityNotFoundException("Cart is empty");
            }

            var result = new ShoppingCartDTO
            {
                Customer = mapper.Map<CustomerDTO>(customer),
                CartItems = mapper.Map<IEnumerable<ShoppingCartRecordDTO>>(cart)
            };
            return Ok(result);
        }


        [HttpPost]
        [Route("add_item")]
        public async Task<IActionResult> AddItemToCart([FromBody] ShoppingCartRecordCreateDTO item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = await unitOfWork.Customers.GetOneByQueryAsync(q => q.Id == item.CustomerId);
            if (customer == null)
            {
                throw new EntityNotFoundException($"Customer with id {item.CustomerId} not found");
            }

            var product = await unitOfWork.Products.GetOneByQueryAsync(q => q.Id == item.ProductId);
            if (product == null)
            {
                throw new EntityNotFoundException($"Product with id {item.ProductId} not found");
            }


            var lineItem = await unitOfWork.ShoppingCartRecords
                                .GetOneByQueryAsync(q => q.CustomerId == item.CustomerId && q.ProductId == item.ProductId);

            if (lineItem == null)
            {
                lineItem = new ShoppingCartRecord
                {
                    CustomerId = item.CustomerId,
                    ProductId = item.ProductId,
                    Quantity = 1,
                    LineItemTotal = product.CurrentPrice
                };
                await unitOfWork.ShoppingCartRecords.InsertAsync(lineItem);
            }
            else
            {
                lineItem.Quantity += 1;
                lineItem.LineItemTotal = lineItem.Quantity * product.CurrentPrice;
                unitOfWork.ShoppingCartRecords.Update(lineItem);
            }

            await unitOfWork.SaveAsync();

            var cart = await GetShoppingCart(customer.Id);
            var result = new ShoppingCartDTO
            {
                Customer = mapper.Map<CustomerDTO>(customer),
                CartItems = mapper.Map<IEnumerable<ShoppingCartRecordDTO>>(cart)
            };

            return Ok(result);
        }

        [HttpPut]
        [Route("remove_item")]
        public async Task<IActionResult> DeleteItemFromCart([FromBody] ShoppingCartRecordCreateDTO cartItemDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var customer = await unitOfWork.Customers.GetOneByQueryAsync(q => q.Id == cartItemDTO.CustomerId);
            if (customer == null)
            {
                throw new EntityNotFoundException($"Customer with id {cartItemDTO.CustomerId} not found");
            }

            var product = await unitOfWork.Products.GetOneByQueryAsync(q => q.Id == cartItemDTO.ProductId);
            if (product == null)
            {
                throw new EntityNotFoundException($"Product with id {cartItemDTO.ProductId} not found");
            }


            var lineItem = await unitOfWork.ShoppingCartRecords
                                .GetOneByQueryAsync(q => q.CustomerId == cartItemDTO.CustomerId && q.ProductId == cartItemDTO.ProductId);
            if (lineItem == null)
            {
                throw new EntityNotFoundException($"Product with id {cartItemDTO.ProductId} not in the cart");
            }

            if (lineItem.Quantity <= 1)
            {
                unitOfWork.ShoppingCartRecords.Delete(lineItem);
            }
            else
            {
                lineItem.Quantity -= 1;
                unitOfWork.ShoppingCartRecords.Update(lineItem);
            }
            await unitOfWork.SaveAsync();

            var cart = await GetShoppingCart(cartItemDTO.CustomerId);
            var result = new ShoppingCartDTO
            {
                Customer = mapper.Map<CustomerDTO>(customer),
                CartItems = mapper.Map<IEnumerable<ShoppingCartRecordDTO>>(cart)
            };
            return Ok(result);
        }

        [HttpDelete]
        [Route("empty_cart")]
        public async Task<IActionResult> DeleteCart([FromQuery] int customerId)
        {
            var customer = await unitOfWork.Customers.GetOneByQueryAsync(q => q.Id == customerId);
            if (customer == null)
            {
                throw new EntityNotFoundException($"Customer with id {customerId} not found");
            }

            var cart = await GetShoppingCart(customerId);

            if (cart == null || cart.Count <= 0)
            {
                throw new EntityNotFoundException($"Cart is empty.");
            }
            else
            {
                unitOfWork.ShoppingCartRecords.DeleteRange(cart);
            }

            await unitOfWork.SaveAsync();
            return Ok();
        }

        private async Task<IList<ShoppingCartRecord>> GetShoppingCart(int customerId)
        {
            var items = await unitOfWork.ShoppingCartRecords.GetAllAsync(q => q.CustomerId == customerId, includes: new List<string> { "Product" });
            if (items.Count > 0)
            {
                foreach (var item in items)
                {
                    item.LineItemTotal = item.Quantity * item.Product.CurrentPrice;
                }
            }
            return items;
        }
    }
}
