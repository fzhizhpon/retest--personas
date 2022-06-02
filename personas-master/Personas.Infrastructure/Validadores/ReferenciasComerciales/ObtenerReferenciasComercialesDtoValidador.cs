using FluentValidation;
using Personas.Core.Dtos.ReferenciasComerciales;

namespace Personas.Infrastructure.Validadores
{
    public class ObtenerReferenciasComercialesDtoValidador : AbstractValidator<ObtenerReferenciasComercialesDto>
    {
        public ObtenerReferenciasComercialesDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotNull().NotEmpty();
            RuleFor(x => x.paginacion.indiceInicial).NotNull().NotEmpty();
            RuleFor(x => x.paginacion.numeroRegistros).NotNull().NotEmpty();
        }
    }
}
