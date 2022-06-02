using FluentValidation;
using Personas.Core.Dtos.TablasComunes;

namespace Personas.Infrastructure.Validadores.InformacionAdicional
{
    public class ActualizarInformacionAdicionalDtoValidador : AbstractValidator<ActualizarInformacionAdicionalDto>
    {
        public ActualizarInformacionAdicionalDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotEmpty();
            RuleFor(x => x.codigoTabla).NotNull();
            RuleFor(x => x.codigoElemento).GreaterThanOrEqualTo(0);
            RuleFor(x => x.observacion).NotNull();
        }
    }
}