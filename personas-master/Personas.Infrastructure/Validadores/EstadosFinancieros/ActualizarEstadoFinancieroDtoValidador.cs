using FluentValidation;
using Personas.Core.Dtos.EstadosFinancieros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personas.Infrastructure.Validadores.EstadosFinancieros
{
    class ActualizarEstadoFinancieroDtoValidador : AbstractValidator<ActualizarEstadoFinancieroDto>
    {
        public ActualizarEstadoFinancieroDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotNull();
            RuleFor(x => x.cuentaContable).NotNull();
            RuleFor(x => x.valor).GreaterThan(0);
        }
    }
}
