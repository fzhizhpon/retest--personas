using FluentValidation;
using Personas.Core.Dtos.BienesIntangibles;

namespace Personas.Infrastructure.Validadores.BienesIntangibles
{
    public class EliminarBienesIntangiblesDtoValidador : AbstractValidator<EliminarBienesIntangiblesDto>
    {
        public EliminarBienesIntangiblesDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotEmpty();
            RuleFor(x => x.numeroRegistro).NotEmpty();
        }
    }
}