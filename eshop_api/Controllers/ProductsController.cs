using AutoMapper;
using eshop_api.DTO;
using eshop_api.Exceptions;
using eshop_api.Interfaces;
using eshop_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eshop_api.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await unitOfWork.Products.GetAllAsync(includes: new List<string> { "Category" });
            var results = mapper.Map<IEnumerable<ProductDTO>>(products);
            return Ok(results);
        }

        [HttpGet("{id:int}", Name = "GetProductById")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await unitOfWork.Products.GetOneByQueryAsync(q => q.Id == id, new List<string> { "Category" });
            if (product == null)
            {
                throw new EntityNotFoundException($"Product with id {id} not found");
            }
            var result = mapper.Map<ProductDTO>(product);
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDTO productDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var categoryExist = await unitOfWork.Categories.GetOneByQueryAsync(q => q.Id == productDTO.CategoryId);
            if (categoryExist == null)
            {
                throw new EntityNotFoundException($"Category does not exist");
            }

            var product = mapper.Map<Product>(productDTO);

            await unitOfWork.Products.InsertAsync(product);
            await unitOfWork.SaveAsync();

            var result = mapper.Map<ProductDTO>(product);
            return CreatedAtRoute("GetProductById", new { Id = product.Id }, result);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDTO productUpdateDTO)
        {
            if (id != productUpdateDTO.Id)
            {
                ModelState.AddModelError("Id", "Invalid Product id");
            }
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }
            var productExist = await unitOfWork.Products.GetOneByQueryAsync(q => q.Id == id);
            if (productExist == null)
            {
                throw new EntityNotFoundException($"Product with id {id} not found");
            }

            mapper.Map(productUpdateDTO, productExist);
            unitOfWork.Products.Update(productExist);
            await unitOfWork.SaveAsync();
            var result = mapper.Map<ProductDTO>(productExist);
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await unitOfWork.Products.GetOneByQueryAsync(q => q.Id == id);
            if (product == null)
            {
                throw new EntityNotFoundException($"Product with id {id} not found");
            }
            var lineItems = await unitOfWork.ShoppingCartRecords.GetAllAsync(q => q.ProductId == q.Id);
            if (lineItems != null || lineItems.Count > 0)
            {
                var error = new ApiError(422, "Forbidden Error", "Cannot delete product that has line items");
                return UnprocessableEntity(error.ToString());
            }
            unitOfWork.Products.Delete(product);
            return Ok();
        }
    }
}
