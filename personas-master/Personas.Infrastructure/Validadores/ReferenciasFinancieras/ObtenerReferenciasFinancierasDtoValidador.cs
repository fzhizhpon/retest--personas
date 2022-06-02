using FluentValidation;
using Personas.Core.Dtos.ReferenciasFinancieras;

namespace Personas.Infrastructure.Validadores
{
    public class ObtenerReferenciasFinancierasDtoValidador : AbstractValidator<ObtenerReferenciasFinancierasDto>
    {
        public ObtenerReferenciasFinancierasDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotNull().NotEmpty();
        }
    }
}
