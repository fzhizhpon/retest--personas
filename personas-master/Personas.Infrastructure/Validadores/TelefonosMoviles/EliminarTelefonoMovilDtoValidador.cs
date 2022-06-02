using FluentValidation;
using Personas.Core.Dtos.TelefonoMovil;

namespace Personas.Infrastructure.Validadores.TelefonosMoviles
{
    public class EliminarTelefonoMovilDtoValidador : AbstractValidator<EliminarTelefonoMovilDto>
    {
        public EliminarTelefonoMovilDtoValidador()
        {
            RuleFor(x => x.codigoTelefonoMovil).NotNull().NotEmpty();
            RuleFor(x => x.codigoPersona).NotNull().NotEmpty();
        }
    }
}
