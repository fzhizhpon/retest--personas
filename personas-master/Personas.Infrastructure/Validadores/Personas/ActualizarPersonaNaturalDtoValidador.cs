using FluentValidation;
using Personas.Core.Dtos.Personas;

namespace Personas.Infrastructure.Validadores.Personas
{
    public class ActualizarPersonaNaturalDtoValidador : AbstractValidator<ActualizarPersonaNaturalDto>
    {
        public ActualizarPersonaNaturalDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotEmpty();
            RuleFor(x => x.nombres).NotEmpty().MaximumLength(80);
            RuleFor(x => x.apellidoPaterno).NotEmpty().MaximumLength(40);
            RuleFor(x => x.apellidoMaterno).NotEmpty().MaximumLength(40);
            RuleFor(x => x.fechaNacimiento).NotEmpty();
            RuleFor(x => x.tieneDiscapacidad).InclusiveBetween('0', '1');
            RuleFor(x => x.codigoPaisNacimiento).NotEmpty();
            RuleFor(x => x.codigoProvinciaNacimiento).NotEmpty();
            RuleFor(x => x.codigoCiudadNacimiento).NotEmpty();
            RuleFor(x => x.codigoParroquiaNacimiento).NotEmpty();
            RuleFor(x => x.codigoTipoSangre).NotEmpty();
            RuleFor(x => x.codigoEstadoCivil).NotEmpty();
            RuleFor(x => x.codigoGenero).NotEmpty();
            RuleFor(x => x.codigoProfesion).NotEmpty();
            RuleFor(x => x.codigoTipoEtnia).NotEmpty();
        }
    }
}
