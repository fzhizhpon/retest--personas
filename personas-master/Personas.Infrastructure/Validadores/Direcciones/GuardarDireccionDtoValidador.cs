using FluentValidation;
using Personas.Core.Dtos.Direcciones;

namespace Personas.Infrastructure.Validadores.Direcciones
{
    public class GuardarDireccionDtoValidador : AbstractValidator<GuardarDireccionDto>
    {
        public GuardarDireccionDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotNull().NotEmpty();
            RuleFor(x => x.codigoPais).NotNull().NotEmpty();
            RuleFor(x => x.codigoProvincia).NotNull().NotEmpty();
            RuleFor(x => x.codigoCiudad).NotNull().NotEmpty();
            RuleFor(x => x.codigoParroquia).NotNull().NotEmpty();
            RuleFor(x => x.callePrincipal).NotNull().NotEmpty();
            RuleFor(x => x.calleSecundaria).NotNull().NotEmpty();
            RuleFor(x => x.referencia).NotNull().NotEmpty();

            RuleFor(x => x.sector).NotNull().NotEmpty();

            RuleFor(x => x.esMarginal).NotNull().NotEmpty().Custom((value, context) =>
            {
                if (value != '0' && value != '1')
                {
                    context.AddFailure("El campo esMarginal debe tener el valor '1' o '0'.");
                }
            });

            RuleFor(x => x.tipoSector).NotNull().NotEmpty().Custom((value, context) =>
            {
                if (value != 'U' && value != 'R')
                {
                    context.AddFailure("El campo tipoSector debe tener el valor 'U' (urbano) o 'R' (rural).");
                }
            });

            RuleFor(x => x.principal).NotNull().NotEmpty().Custom((value, context) =>
            {
                if (value != '0' && value != '1')
                {
                    context.AddFailure("El campo principal debe tener el valor '1' o '0'.");
                }
            });
        }
    }
}