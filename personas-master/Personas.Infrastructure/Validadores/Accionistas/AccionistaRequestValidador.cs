using FluentValidation;
using Personas.Core.Dtos.Accionistas;

namespace Personas.Infrastructure.Validadores.Accionistas
{
    public class AccionistaRequestValidador : AbstractValidator<AccionistaRequest>
    {
        public AccionistaRequestValidador()
        {
            RuleFor(x => x.codigoPersona).NotEmpty();
        }
    }
}
