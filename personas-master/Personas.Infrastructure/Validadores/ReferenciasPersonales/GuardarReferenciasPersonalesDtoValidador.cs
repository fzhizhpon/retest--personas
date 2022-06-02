using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Personas.Core.Dtos;
using Personas.Core.Dtos.ReferenciasPersonales;

namespace Personas.Infrastructure.Validadores
{
    public class GuardarReferenciasPersonalesDtoValidador : AbstractValidator<GuardarReferenciaPersonalDto>
    {
        public GuardarReferenciasPersonalesDtoValidador()
        {
            RuleFor(x => x.codigoPersonaReferida).NotNull().NotEmpty();
            RuleFor(x => x.codigoPersona).NotNull().NotEmpty();
            RuleFor(x => x.FechaConoce).NotNull().NotEmpty();
        }
    }
}
