using FluentValidation;
using Personas.Core.Dtos.Direcciones;

namespace Personas.Infrastructure.Validadores.Direcciones
{
    public class EliminarDireccionDtoValidador : AbstractValidator<EliminarDireccionDto>
    {
        public EliminarDireccionDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotNull().NotEmpty();
            RuleFor(x => x.numeroRegistro).NotNull().NotEmpty();
        }
    }
}