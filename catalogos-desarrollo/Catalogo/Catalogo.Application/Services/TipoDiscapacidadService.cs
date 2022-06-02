﻿using Catalogo.Core.DTOs;
using Catalogo.Core.Enums;
using Catalogo.Core.Interfaces.IRepositories;
using Catalogo.Core.Interfaces.IServices;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalogo.Application.Services
{
    public class TipoDiscapacidadService : ITipoDiscapacidadService
    {
        private readonly ITipoDiscapacidadRepository _repository;
        private readonly IMensajeRespuestaRepository _mensajeRepository;

        public TipoDiscapacidadService(ITipoDiscapacidadRepository repository,
            IMensajeRespuestaRepository mensajeRepository)
        {
            _repository = repository;
            _mensajeRepository = mensajeRepository;
        }

        public async Task<Respuesta> ObtenerTiposDiscapacidades()
        {
            ERespuesta codigoRespuesta = ERespuesta.OK;
            int codigoEvento = 0;

            (int codigo, IEnumerable<ComboDto> discapacidades) = await _repository.SelectTiposDiscapacidades();

            if (codigo == CodigosLogicaInterna.CODIGO_GENERICO_OK_INTERNO)
            {
                if (discapacidades is null)
                {
                    codigoRespuesta = ERespuesta.OK;
                    codigoEvento = -1; // No se encontraron datos
                }
            }
            else
            {
                codigoRespuesta = ERespuesta.ERROR;
                codigoEvento = -2; //Error al obtener los datos
            }

            //Llama al servicio de idioma
            string textoInfo = await _mensajeRepository.ObtenerTextoInfo(
              "Idioma", codigoEvento, "Modulo");

            return new Respuesta()
            {
                codigo = codigoRespuesta,
                resultado = discapacidades,
                mensajeUsuario = textoInfo
            };
        }
    }
}
