using AutoMapper;
using eshop_api.DTO;
using eshop_api.Exceptions;
using eshop_api.Interfaces;
using eshop_api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace eshop_api.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public CustomersController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await unitOfWork.Customers.GetOneByQueryAsync(q => q.Id == id);
            if (customer == null)
            {
                throw new EntityNotFoundException($"Customer with id {id} not found");
            }
            var result = mapper.Map<CustomerDTO>(customer);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerCreateDTO customerCreateDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customer = mapper.Map<Customer>(customerCreateDTO);
            await unitOfWork.Customers.InsertAsync(customer);
            await unitOfWork.SaveAsync();
            var result = mapper.Map<CustomerDTO>(customer);
            return Ok(result);
        }
    }
}
