using FluentValidation;
using Personas.Core.Dtos.BienesMuebles;

namespace Personas.Infrastructure.Validadores.BienesMuebles
{
    public class GuardarBienesMueblesDtoValidador  : AbstractValidator<GuardarBienesMueblesDto>
    {
        public GuardarBienesMueblesDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotEmpty();
            RuleFor(x => x.tipoBienMueble).NotEmpty();
            RuleFor(x => x.codigoReferencia).NotEmpty().MaximumLength(20);
            RuleFor(x => x.descripcion).NotEmpty().MaximumLength(200);
            RuleFor(x => x.avaluoComercial).GreaterThan(0);
        }
    }
}