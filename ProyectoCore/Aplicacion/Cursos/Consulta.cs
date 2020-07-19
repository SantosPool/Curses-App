using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Dominio;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Consulta
    {
        public class ListaCursos : IRequest<List<CursoDto>>
        {

        }

        public class Manejador : IRequestHandler<ListaCursos, List<CursoDto>>
        {
            private readonly CursosOnlineContext context;
            private readonly IMapper mapper;
            public Manejador(CursosOnlineContext _context,IMapper _mapper)
            {
                this.context=_context;
                mapper=_mapper;
            }
            public async Task<List<CursoDto>> Handle(ListaCursos request, CancellationToken cancellationToken)
            {
                var cursos= await context.Curso
                .Include(x=>x.ComentarioLista)
                .Include(x=>x.PrecioPromocion)
                .Include(x=>x.InstructorLink)
                .ThenInclude(x=>x.Instructor)
                .ToListAsync();

                var cursosDto=mapper.Map<List<Curso>,List<CursoDto>>(cursos);
                
                return cursosDto;
            }
        }
    }
}