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
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IRepositoryBase<Category> repository;
        private readonly IMapper mapper;

        public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            repository = unitOfWork.Categories;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await repository.GetAllAsync();
            var results = mapper.Map<IEnumerable<CategoryDTO>>(categories);
            return Ok(results);

        }

        [HttpGet("{id:int}", Name = "GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await repository.GetOneByQueryAsync(q => q.Id == id);
            if (category == null)
            {
                throw new EntityNotFoundException($"Category with Id {id} not found.");
            }

            var result = mapper.Map<CategoryDTO>(category);
            return Ok(result);
        }


        [HttpPost]
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDTO categoryDto)
        {
            var exist = await repository.GetOneByQueryAsync(q => q.Name == categoryDto.Name);
            if (exist != null)
            {
                ModelState.AddModelError(nameof(Category.Name), "Category name already exist");
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }
            var category = mapper.Map<Category>(categoryDto);
            await repository.InsertAsync(category);
            await unitOfWork.SaveAsync();
            var result = mapper.Map<CategoryDTO>(category);
            return CreatedAtRoute("GetById", new { id = category.Id }, result);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDTO categoryUpdateDTO)
        {
            if(id != categoryUpdateDTO.Id)
            {
                ModelState.AddModelError("Id", "Category Id is not valid");
            }
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var categoryExist = await repository.GetOneByQueryAsync(q => q.Id == id);
            if(categoryExist == null)
            {
                throw new EntityNotFoundException($"Category with id {id} not found");
            }

            mapper.Map(categoryUpdateDTO, categoryExist);
            repository.Update(categoryExist);
            await unitOfWork.SaveAsync();
            var result = mapper.Map<CategoryDTO>(categoryExist);
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await repository.GetOneByQueryAsync(i => i.Id == id);
            if (category == null)
            {
                throw new EntityNotFoundException($"Category with Id [{id}]not found.");
            }
            repository.Delete(category);
            await unitOfWork.SaveAsync();
            return Ok();
        }
    }
}
