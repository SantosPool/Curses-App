using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aplicacion.Cursos;
using Dominio;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Persistencia.DapperConexion.Paginacion;

namespace WebAPI.Controllers
{

    //http://localhost:5000/cursos
    public class CursosController: MiControllerBase
    {

        [HttpGet]
        public async Task<ActionResult<List<CursoDto>>> GetTask()
        {
            return await _Mediator.Send(new Consulta.ListaCursos());
        }

        //http://localhost:5000/api/Cursos/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CursoDto>> Detalle(Guid id)
        {
            return await _Mediator.Send(new ConsultaId.CursoUnico{Id=id});
        }

        [HttpPost]
        public async Task<ActionResult<Unit>> Crear(Nuevo.Ejecuta data)
        {
          return await _Mediator.Send(data);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Unit>> Editar(Guid id,Editar.Ejecuta data)
        {
            data.CursoId=id;
            return await _Mediator.Send(data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> Eliminar(Guid id)
        {
            return await _Mediator.Send(new Eliminar.Ejecuta{Id=id});
        }

        [HttpPost("report")]
        public async Task<ActionResult<PaginacionModel>> Report(PaginacionCurso.Ejecuta data)
        {
            return await _Mediator.Send(data);
        }

    }
}