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
    public class ObtenerReferenciasPersonalesDtoValidador : AbstractValidator<ObtenerReferenciasPersonalesDto>
    {
        public ObtenerReferenciasPersonalesDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotNull().NotEmpty();
            RuleFor(x => x.paginacion.indiceInicial).NotNull().NotEmpty();
            RuleFor(x => x.paginacion.numeroRegistros).NotNull().NotEmpty();
        }
    }
}
