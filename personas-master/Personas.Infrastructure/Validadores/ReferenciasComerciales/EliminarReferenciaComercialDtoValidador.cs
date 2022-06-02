using FluentValidation;
using Personas.Core.Dtos.ReferenciasComerciales;

namespace Personas.Infrastructure.Validadores.ReferenciasComerciales
{
    public class EliminarReferenciaComercialDtoValidador : AbstractValidator<EliminarReferenciaComercialDto>
    {
        public EliminarReferenciaComercialDtoValidador()
        {
            RuleFor(x => x.numeroRegistro).NotNull().NotEmpty();
            RuleFor(x => x.codigoPersona).NotNull().NotEmpty();
        }
    }
}
