using FluentValidation;
using Personas.Core.Dtos.ReferenciasFinancieras;

namespace Personas.Infrastructure.Validadores.ReferenciasComerciales
{
    public class EliminarReferenciaFinancieraDtoValidador : AbstractValidator<EliminarReferenciaFinancieraDto>
    {
        public EliminarReferenciaFinancieraDtoValidador()
        {
            RuleFor(x => x.numeroRegistro).NotNull().NotEmpty();
            RuleFor(x => x.codigoPersona).NotNull().NotEmpty();
        }
    }
}
