using FluentValidation;
using Personas.Core.Dtos.TelefonoMovil;

namespace Personas.Infrastructure.Validadores.TelefonosMoviles
{
    public class ObtenerTelefonosMovilDtoValidador : AbstractValidator<ObtenerTelefonosMovilDto>
    {
        public ObtenerTelefonosMovilDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotEmpty();
        }
    }
}
