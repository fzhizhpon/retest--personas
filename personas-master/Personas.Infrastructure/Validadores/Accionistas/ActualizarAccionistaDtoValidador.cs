using FluentValidation;
using Personas.Core.Dtos.Accionistas;

namespace Personas.Infrastructure.Validadores.Accionistas
{
    public class ActualizarAccionistaDtoValidador : AbstractValidator<ActualizarAccionistaDto>
    {
        public ActualizarAccionistaDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotEmpty();
            RuleFor(x => x.codigoAccionista).NotEmpty();
            RuleFor(x => x.porcentajeAcciones).NotEmpty().ScalePrecision(2, 5);
        }
    }
}
