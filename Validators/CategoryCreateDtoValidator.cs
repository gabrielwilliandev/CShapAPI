using API.DTOs;
using FluentValidation;

namespace API.Validators
{
    public class CategoryCreateDtoValidator : AbstractValidator<CategoryCreateDto>
    {
        public CategoryCreateDtoValidator() 
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MinimumLength(2).WithMessage("O nome deve ter pelo menos 2 caracteres.")
                .MaximumLength(50).WithMessage("O nome deve ter no máximo 50 caracteres.");
        }
    }
}
