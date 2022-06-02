using FluentValidation;
using Personas.Core.Dtos.BienesIntangibles;

namespace Personas.Infrastructure.Validadores.BienesIntangibles
{
    public class ActualizarBienesIntangiblesDtoValidador : AbstractValidator<ActualizarBienesIntangiblesDto>
    {
        public ActualizarBienesIntangiblesDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotEmpty();
            RuleFor(x => x.numeroRegistro).NotEmpty();
            RuleFor(x => x.descripcion).NotEmpty().MaximumLength(200);
            RuleFor(x => x.avaluoComercial).GreaterThan(0);

        }
    }
}