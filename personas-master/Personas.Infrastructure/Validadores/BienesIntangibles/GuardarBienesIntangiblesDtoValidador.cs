using FluentValidation;
using Personas.Core.Dtos.BienesIntangibles;

namespace Personas.Infrastructure.Validadores.BienesIntangibles
{
    public class GuardarBienesIntangiblesDtoValidador : AbstractValidator<GuardarBienesIntangiblesDto>
    {
        public GuardarBienesIntangiblesDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotEmpty();
            RuleFor(x => x.tipoBienIntangible).NotEmpty();
            RuleFor(x => x.codigoReferencia).NotEmpty().MaximumLength(20);
            RuleFor(x => x.descripcion).NotEmpty().MaximumLength(200);
            RuleFor(x => x.avaluoComercial).GreaterThan(0);

        }
    }
}