using System.Threading;
using System.Threading.Tasks;
using Dominio;
using MediatR;
using Persistencia;
using System.Linq;
using Aplicacion.ManejadorError;
using System.Net;
using AutoMapper;
using System;
using Microsoft.EntityFrameworkCore;

namespace Aplicacion.Cursos
{
    public class ConsultaId
    {
        public class CursoUnico : IRequest<CursoDto>
        {
            public Guid Id{get;set;}
        }
        public class Manejador : IRequestHandler<CursoUnico, CursoDto>
        {
            private readonly CursosOnlineContext context;
            private readonly IMapper mapper;
            public Manejador(CursosOnlineContext _context,IMapper _mapper)
            {
                this.context=_context;
                mapper=_mapper;
            }
            public async Task<CursoDto> Handle(CursoUnico request, CancellationToken cancellationToken)
            {
                var curso= await context.Curso
                .Include(x=>x.ComentarioLista)
                .Include(x=>x.PrecioPromocion)
                .Include(x=>x.InstructorLink)
                .ThenInclude(y=>y.Instructor)
                .FirstOrDefaultAsync(a=>a.CursoId==request.Id);
                if(curso is null)
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound,new{curso="No se encontro el curso"});
                var cursoDto=mapper.Map<Curso,CursoDto>(curso);
                return cursoDto;
            }
        }
    }
}