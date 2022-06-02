using FluentValidation;
using Personas.Core.Dtos.ReferenciasComerciales;

namespace Personas.Infrastructure.Validadores
{
    public class ObtenerReferenciaComercialDtoValidador : AbstractValidator<ObtenerReferenciaComercialDto>
    {
        public ObtenerReferenciaComercialDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotNull().NotEmpty();
            RuleFor(x => x.numeroRegistro).NotNull().NotEmpty();
        }
    }
}
