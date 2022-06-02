using CoopCrea.Cross.Event.Src.Bus;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Personas.Application.Messages.Commands;
using Personas.Application.Messages.Events;
using Personas.Core.Interfaces.IServices;
using Personas.Core.App;
using Personas.Core.Interfaces.IRepositories;

namespace Personas.Application.Messages.CommandHandlers
{
    public class AuditoriaCommandHandler : IRequestHandler<AuditoriaCommand, bool>
    {
        private readonly IEventBus _bus;

        public AuditoriaCommandHandler(
            IEventBus bus)
        {
            _bus = bus;
        }

        public Task<bool> Handle(AuditoriaCommand request, CancellationToken cancellationToken)
        {

            AuditoriaEvent auditoriaEvent;
            try
            {
                auditoriaEvent = new AuditoriaEvent()
                {
                    guid = request.guid,
                    codigoAgencia = request.codigoAgencia,
                    codigoSucursal = request.codigoSucursal,
                    codigoUsuario = request.codigoUsuario,
                    fechaRegistro = request.fechaRegistro,
                    ipPrivada = request.ipPrivada,
                    ipPublica = request.ipPublica,
                    modulo = request.modulo,
                    endPoint = request.endPoint,
                    entrada = request.entrada,
                    salida = request.salida,
                    navegador = request.navegador,
                };

                _bus.Publish(auditoriaEvent);
                return Task.FromResult(true);

            }
            catch (Exception exc)
            {
                Console.WriteLine($"\n\n\n Error al enviar objeto a RABBITMQ {exc}");
                return Task.FromResult(false);
            }

        }
    }
}
