using FluentValidation;
using Personas.Core.Dtos.ReferenciasComerciales;
using Personas.Core.Dtos.ReferenciasFinancieras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personas.Infrastructure.Validadores.ReferenciasComerciales
{
    public class GuardarReferenciaFinancieraDtoValidador : AbstractValidator<GuardarReferenciaFinancieraDto>
    {
        public GuardarReferenciaFinancieraDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotNull().NotEmpty();
            RuleFor(x => x.codigoTipoCuentaFinanciera).NotNull().NotEmpty();
            RuleFor(x => x.codigoInstitucionFinanciera).NotNull().NotEmpty();            
            RuleFor(x => x.fechaCuenta).NotNull().NotEmpty();
            RuleFor(x => x.numeroCuenta).NotNull().NotEmpty();
            RuleFor(x => x.cifras).NotNull();
            RuleFor(x => x.saldo).NotNull();
            RuleFor(x => x.saldoObligacion).NotNull();
            RuleFor(x => x.obligacionMensual).NotNull();
        }
    }
}
