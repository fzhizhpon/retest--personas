using FluentValidation;
using Personas.Core.Dtos.CorreosElectronicos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Personas.Infrastructure.Validadores.CorreosElectronicos
{
    public class AregarCorreoElectronicoDtoValidador : AbstractValidator<AgregarCorreoElectronicoDto>
    {
        public AregarCorreoElectronicoDtoValidador()
        {
            RuleFor(x => x.codigoPersona).NotEmpty();
            RuleFor(s => s.correoElectronico).NotEmpty().Matches(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$");
            RuleFor(x => x.esPrincipal).NotEmpty();
        }
    }
}
