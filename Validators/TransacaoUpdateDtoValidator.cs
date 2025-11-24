using API.Data;
using API.DTOs;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace API.Validators
{
    public class TransacaoUpdateDtoValidator : AbstractValidator<TransacaoUpdateDto>
    {
        public TransacaoUpdateDtoValidator(AppDbContext context)
        {
            RuleFor(x => x.Description)
            .NotEmpty().WithMessage("A descrição é obrigatória.")
            .MinimumLength(3).WithMessage("A descrição deve ter no mínimo 3 caracteres.")
            .MaximumLength(150).WithMessage("A descrição deve ter no máximo 150 caracteres.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("O valor deve ser maior que zero.");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("A data é obrigatória.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("A data não pode estar no futuro.");

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("O tipo da transação é obrigatório.")
                .Must(t => t == "Entrada" || t == "Saída")
                .WithMessage("O tipo deve ser 'Entrada' ou 'Saída'.");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("A categoria é obrigatória.");
            RuleFor(x => x.CategoryId)
                .MustAsync(async (categoryId, cancellationToken) =>
                {
                    // Verifica no banco se a categoria existe
                    return await context.Categories.AnyAsync(c => c.Id == categoryId, cancellationToken);
                })
                .WithMessage("A categoria informada não existe.");
        }
    }
}
