using FluentValidation;
using Personas.Core.Dtos.Accionistas;
using System.Collections.Generic;

namespace Personas.Infrastructure.Validadores.Accionistas
{
    public class GuardarAccionistaDtoValidador : AbstractValidator<GuardarAccionistaDto>
    {
        public GuardarAccionistaDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotEmpty();
            RuleFor(x => x.codigoAccionista).NotEmpty();
            RuleFor(x => x.porcentajeAcciones).NotEmpty().ScalePrecision(2, 5);
        }
    }

    public class GuardarListaAccionistaDtoValidador : AbstractValidator<List<GuardarAccionistaDto>>
    {
        public GuardarListaAccionistaDtoValidador()
        {
            RuleForEach(x => x).SetValidator(new GuardarAccionistaDtoValidador());
        }
    }
}
