using FluentValidation;
using Personas.Core.Dtos.BienesInmuebles;

namespace Personas.Infrastructure.Validadores.BienesInmuebles
{
    public class ActualizarBienesInmueblesDtoValidador : AbstractValidator<ActualizarBienesInmueblesDto>
    {
        public ActualizarBienesInmueblesDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotEmpty();
            RuleFor(x => x.numeroRegistro).NotEmpty();
            RuleFor(x => x.tipoBienInmueble).NotEmpty();
            //RuleFor(x => x.codigoPais).NotEmpty();
            //RuleFor(x => x.codigoProvincia).NotEmpty();
            //RuleFor(x => x.codigoCiudad).NotEmpty();
            //RuleFor(x => x.codigoParroquia).NotEmpty();

            //RuleFor(x => x.sector).NotEmpty().MaximumLength(50);
            RuleFor(x => x.callePrincipal).NotEmpty().MaximumLength(200);
            RuleFor(x => x.calleSecundaria).NotEmpty().MaximumLength(200);
            //RuleFor(x => x.numero).NotEmpty().MaximumLength(10);
            //RuleFor(x => x.codigoPostal).NotEmpty().MaximumLength(10);

            //RuleFor(x => x.tipoSector).NotNull().NotEmpty().Custom((value, context) =>
            //{
            //    if (value.ToString().Equals('U') || value.ToString().Equals('R'))
            //    {
            //        context.AddFailure("Debe ingresar un código del tipo estado válido.");
            //    }
            //});
            //RuleFor(x => x.esMarginal).InclusiveBetween('0', '1');

            //RuleFor(x => x.longitud).NotEmpty();
            //RuleFor(x => x.latitud).NotEmpty();

            RuleFor(x => x.avaluoComercial).NotEmpty();
            RuleFor(x => x.avaluoCatastral).NotEmpty();
            //RuleFor(x => x.areaTerreno).NotEmpty();
            RuleFor(x => x.areaConstruccion).NotEmpty();
            RuleFor(x => x.valorTerrenoMetrosCuadrados).NotEmpty();

            RuleFor(x => x.fechaConstruccion).NotEmpty();

            RuleFor(x => x.referencia).NotEmpty().MaximumLength(200);
            RuleFor(x => x.descripcion).NotEmpty().MaximumLength(100);

            RuleFor(x => x.avaluoComercial).GreaterThan(0);
            RuleFor(x => x.avaluoCatastral).GreaterThan(0);
            RuleFor(x => x.valorTerrenoMetrosCuadrados).GreaterThan(0);
        }
    }
}