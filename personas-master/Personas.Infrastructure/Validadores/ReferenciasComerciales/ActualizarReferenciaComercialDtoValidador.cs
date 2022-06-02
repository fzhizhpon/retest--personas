using FluentValidation;
using Personas.Core.Dtos.ReferenciasComerciales;

namespace Personas.Infrastructure.Validadores.ReferenciasComerciales
{
    public class ActualizarReferenciaComercialDtoValidador : AbstractValidator<ActualizarReferenciaComercialDto>
    {
        public ActualizarReferenciaComercialDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotNull().NotEmpty();
            RuleFor(x => x.codigoPais).NotNull().NotEmpty();
            RuleFor(x => x.codigoProvincia).NotNull().NotEmpty();
            RuleFor(x => x.codigoCiudad).NotNull().NotEmpty();
            RuleFor(x => x.codigoParroquia).NotNull().NotEmpty();
            //RuleFor(x => x.establecimiento).NotNull().NotEmpty();
            //RuleFor(x => x.fechaRelacion).NotNull().NotEmpty();
            RuleFor(x => x.montoCredito).NotNull().NotEmpty().ScalePrecision(2, 14);
            RuleFor(x => x.plazo).NotNull().NotEmpty();
            RuleFor(x => x.codigoTipoTiempo).NotNull().NotEmpty();
            RuleFor(x => x.telefono).NotNull().NotEmpty().MinimumLength(7).MaximumLength(20);

        }
    }
}
