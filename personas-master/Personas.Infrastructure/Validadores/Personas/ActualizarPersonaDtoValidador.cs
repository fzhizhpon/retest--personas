using FluentValidation;
using Personas.Core.Dtos.Personas;

namespace Personas.Infrastructure.Validadores.Personas
{
    public class ActualizarPersonaDtoValidador : AbstractValidator<ActualizarPersonaDto>
    {
        public ActualizarPersonaDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotEmpty();
            RuleFor(x => x.numeroIdentificacion).NotEmpty().MaximumLength(15);
            RuleFor(x => x.observaciones).MaximumLength(500);
            RuleFor(x => x.codigoTipoIdentificacion).NotEmpty();
            RuleFor(x => x.codigoTipoPersona).NotEmpty();
            RuleFor(x => x.codigoTipoContribuyente).NotEmpty();
        }
    }
}
