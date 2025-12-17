using API.DTOs;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoryController :  ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IValidator<CategoryCreateDto> _createValidator;
        private readonly IValidator<CategoryUpdateDto> _updateValidator;

        public CategoryController(ICategoryService categoryService, IValidator<CategoryCreateDto> createValidator,
            IValidator<CategoryUpdateDto> updateValidator)
        {
            _categoryService = categoryService;
            _createValidator = createValidator; 
            _updateValidator = updateValidator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound(new {message = "Categoria não encontrada."});
            }
            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CategoryCreateDto dto)
        {
            var validationResult = await _createValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            var createdCategory = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdCategory.Id }, createdCategory);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(string id, CategoryUpdateDto dto)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            var update = await _categoryService.UpdateAsync(id, dto);
            
            if(update == null)
            {
                return NotFound(new {message = "Categoria não encontrada."});
            }
            return Ok(update);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _categoryService.DeleteAsync(id);
            if(!deleted)
            {
                return NotFound(new {message = "Categoria não encontrada."});
            }
            return NoContent();
        }
    }
}
