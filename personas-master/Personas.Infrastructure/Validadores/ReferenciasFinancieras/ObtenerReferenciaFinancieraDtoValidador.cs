using FluentValidation;
using Personas.Core.Dtos.ReferenciasFinancieras;

namespace Personas.Infrastructure.Validadores
{
    public class ObtenerReferenciaFinancieraDtoValidador : AbstractValidator<ObtenerReferenciaFinancieraDto>
    {
        public ObtenerReferenciaFinancieraDtoValidador()
        {
            RuleFor(x => x.numeroRegistro).NotNull().NotEmpty();
            RuleFor(x => x.codigoPersona).NotNull().NotEmpty();
        }
    }
}
