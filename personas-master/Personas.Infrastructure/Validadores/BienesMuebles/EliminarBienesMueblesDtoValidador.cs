using FluentValidation;
using Personas.Core.Dtos.BienesMuebles;


namespace Personas.Infrastructure.Validadores.BienesMuebles
{
    public class EliminarBienesMueblesDtoValidador : AbstractValidator<EliminarBienesMueblesDto>
    {
        public EliminarBienesMueblesDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotEmpty();
            RuleFor(x => x.numeroRegistro).NotEmpty();
        }
    }
}