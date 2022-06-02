using FluentValidation;
using Personas.Application.CodigosEventos;
using Personas.Core.App;
using Personas.Core.Dtos.Personas;
using Personas.Core.Interfaces.IRepositories;

namespace Personas.Infrastructure.Validadores.Personas
{
    public class GuardarPersonaNaturalDtoValidador : AbstractValidator<GuardarPersonaNaturalDto>
    {

        protected readonly ConfiguracionApp _config;
        protected readonly IMensajesRespuestaRepository _textoInfoService;

        public GuardarPersonaNaturalDtoValidador(ConfiguracionApp config, IMensajesRespuestaRepository textoInfoService)
        {
            RuleFor(x => x.numeroIdentificacion).NotEmpty().MaximumLength(15).Custom((value, context) =>
            {
                 if (value.ToString().Equals('U') || value.ToString().Equals('R'))
                 {
                    _textoInfoService.ObtenerTextoInfo(
                        config.Idioma, 
                        PersonasNaturalesEventos.ACTUALIZAR_CONYUGUE_ERROR, 
                        config.Modulo);

                     context.AddFailure("Debe ingresar un código del tipo estado válido.");
                 }
            });

            RuleFor(x => x.observaciones).MaximumLength(500);
            RuleFor(x => x.codigoTipoIdentificacion).NotEmpty();
            RuleFor(x => x.codigoTipoPersona).NotEmpty();

            RuleFor(x => x.nombres).NotEmpty().MaximumLength(80);
            RuleFor(x => x.apellidoPaterno).NotEmpty().MaximumLength(40);
            RuleFor(x => x.apellidoMaterno).NotEmpty().MaximumLength(40);
            RuleFor(x => x.fechaNacimiento).NotEmpty();
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
