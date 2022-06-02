using FluentValidation;
using Personas.Core.Dtos.ReferenciasFinancieras;

namespace Personas.Infrastructure.Validadores.ReferenciasComerciales
{
    public class ActualizarReferenciaFinancieraDtoValidador : AbstractValidator<ActualizarReferenciaFinancieraDto>
    {
        public ActualizarReferenciaFinancieraDtoValidador()
        {
            RuleFor(x => x.numeroRegistro).NotNull().NotEmpty();
            RuleFor(x => x.codigoPersona).NotNull().NotEmpty();
            RuleFor(x => x.saldo).NotNull();
            RuleFor(x => x.cifras).NotNull();
            RuleFor(x => x.saldoObligacion).NotNull();
            RuleFor(x => x.obligacionMensual).NotNull();
        }
    }
}
