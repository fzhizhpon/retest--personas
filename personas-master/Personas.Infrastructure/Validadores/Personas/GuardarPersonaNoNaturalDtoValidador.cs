using FluentValidation;
using Personas.Core.Dtos.Personas;

namespace Personas.Infrastructure.Validadores.Personas
{
    public class GuardarPersonaNoNaturalDtoValidador : AbstractValidator<GuardarPersonaNoNaturalDto>
    {
        public GuardarPersonaNoNaturalDtoValidador()
        {
            RuleFor(x => x.numeroIdentificacion).NotEmpty().MaximumLength(15);
            RuleFor(x => x.observaciones).MaximumLength(500);
            RuleFor(x => x.codigoTipoIdentificacion).NotEmpty();
            RuleFor(x => x.codigoTipoPersona).NotEmpty();

            RuleFor(x => x.razonSocial).NotEmpty().MaximumLength(500);
            RuleFor(x => x.fechaConstitucion).NotEmpty();
            RuleFor(x => x.objetoSocial).MaximumLength(150);
            RuleFor(x => x.finalidadLucro).InclusiveBetween('0', '1');
            RuleFor(x => x.obligadoLlevarContabilidad).InclusiveBetween('0', '1');
            RuleFor(x => x.agenteRetencion).InclusiveBetween('0', '1');
        }
    }
}
