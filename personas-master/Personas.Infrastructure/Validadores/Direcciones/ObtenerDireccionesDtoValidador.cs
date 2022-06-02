using FluentValidation;
using Personas.Core.Dtos.Direcciones;

namespace Personas.Infrastructure.Validadores.Direcciones
{
    public class ObtenerDireccionesDtoValidador : AbstractValidator<ObtenerDireccionesDto>
    {
        public ObtenerDireccionesDtoValidador()
        {
            RuleFor(x => x.codigoPersona)
                .NotNull().NotEmpty();
        }
    }
}