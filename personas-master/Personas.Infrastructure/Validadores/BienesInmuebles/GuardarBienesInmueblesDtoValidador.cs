using FluentValidation;
using Personas.Core.Dtos.BienesInmuebles;

namespace Personas.Infrastructure.Validadores.BienesInmuebles
{
    public class GuardarBienesInmueblesDtoValidador : AbstractValidator<GuardarBienesInmueblesDto>
    {
        public GuardarBienesInmueblesDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotEmpty();
            RuleFor(x => x.tipoBienInmueble).NotEmpty();
            RuleFor(x => x.codigoPais).NotEmpty();
            RuleFor(x => x.codigoProvincia).NotEmpty();
            RuleFor(x => x.codigoCiudad).NotEmpty();
            RuleFor(x => x.codigoParroquia).NotEmpty();

            RuleFor(x => x.sector).NotEmpty().MaximumLength(50);
            RuleFor(x => x.callePrincipal).NotEmpty().MaximumLength(200);
            RuleFor(x => x.calleSecundaria).NotEmpty().MaximumLength(200);
            RuleFor(x => x.numero).NotEmpty().MaximumLength(10);
            RuleFor(x => x.codigoPostal).NotEmpty().MaximumLength(10);

            RuleFor(x => x.tipoSector).NotNull().NotEmpty().Custom((value, context) =>
            {
                if (value.ToString().Equals('U') || value.ToString().Equals('R'))
                {
                    context.AddFailure("Debe ingresar un código del tipo estado válido.");
                }
            });

            When(x => x.tipoSector == 'R', () => {
                RuleFor(x => x.comunidad).NotNull().NotEmpty()
                .WithMessage("La comunidad no puede ser vacía cuando el tipo de sector es Rural");
                RuleFor(x => x.comunidad).MaximumLength(100)
                .WithMessage("La comunidad no puede ser vacía cuando el tipo de sector es Rural");
            });

            RuleFor(x => x.esMarginal).InclusiveBetween('0', '1');

            RuleFor(x => x.longitud).NotEmpty();
            RuleFor(x => x.latitud).NotEmpty();

            RuleFor(x => x.avaluoComercial).GreaterThan(0);
            RuleFor(x => x.avaluoCatastral).GreaterThan(0);
            RuleFor(x => x.valorTerrenoMetrosCuadrados).GreaterThan(0);
            RuleFor(x => x.areaTerreno).NotEmpty();
            RuleFor(x => x.areaConstruccion).NotEmpty();
            RuleFor(x => x.fechaConstruccion).NotEmpty();

            RuleFor(x => x.referencia).NotEmpty().MaximumLength(200);
            RuleFor(x => x.descripcion).NotEmpty().MaximumLength(100);

            /*
NotNull: Se requiere un valor (negativos, positivos o 0)
GreaterThanOrEqualTo: Valor minimo incluyendo el indicado. Ejemplo: GreaterThanOrEqualTo(0) => acepta el 0 y numeros mayores
GreaterThan: Valor minimo, excluye el indicado. Ejemplo: GreaterThan(0) => acepta numeros mayores a 0             
             */
        }
    }
}