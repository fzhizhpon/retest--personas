using FluentValidation;
using Personas.Core.Dtos.TelefonoMovil;

namespace Personas.Infrastructure.Validadores.TelefonosMoviles
{
    public class ActualizarTelefonoMovilDtoValidador : AbstractValidator<ActualizarTelefonoMovilDto>
    {
        public ActualizarTelefonoMovilDtoValidador()
        {
            RuleFor(x => x.codigoTelefonoMovil).NotNull().NotEmpty();
            RuleFor(x => x.codigoPersona).NotNull().NotEmpty();
            RuleFor(x => x.codigoOperadora).NotNull().NotEmpty();
            RuleFor(x => x.observaciones).MaximumLength(250);
            RuleFor(x => x.principal).InclusiveBetween('0', '1');
        }
    }
}
