using FluentValidation;
using Personas.Core.Dtos.BienesInmuebles;

namespace Personas.Infrastructure.Validadores.BienesInmuebles
{
    public class EliminarBienesInmueblesDtoValidador: AbstractValidator<EliminarBienesInmueblesDto>
    {
        public EliminarBienesInmueblesDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotEmpty();
            RuleFor(x => x.numeroRegistro).NotEmpty();
        }
    }
}