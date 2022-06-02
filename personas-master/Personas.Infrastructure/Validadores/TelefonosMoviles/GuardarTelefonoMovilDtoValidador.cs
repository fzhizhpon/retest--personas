using FluentValidation;
using Personas.Core.Dtos.TelefonoMovil;

namespace Personas.Infrastructure.Validadores.TelefonosMoviles
{
    public class GuardarTelefonoMovilDtoValidador : AbstractValidator<GuardarTelefonoMovilDto>
    {
        public GuardarTelefonoMovilDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotNull().NotEmpty();
            RuleFor(x => x.numero).NotEmpty().MinimumLength(10).MaximumLength(10);
            RuleFor(x => x.codigoPais).NotNull().NotEmpty();
            RuleFor(x => x.codigoOperadora).NotNull().NotEmpty();
            RuleFor(x => x.observaciones).MaximumLength(250);
            RuleFor(x => x.principal).InclusiveBetween('0', '1');
        }
    }
}
