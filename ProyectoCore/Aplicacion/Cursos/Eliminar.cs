using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using MediatR;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Eliminar
    {
        public class Ejecuta: IRequest
        {
            public Guid Id{get;set;}
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CursosOnlineContext context;
            public Manejador(CursosOnlineContext _context)
            {
                this.context=_context;
            }

            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //eliminar curso_instructores
                var instructoresBD= context.CursoInstructor.Where(x=>x.CursoId==request.Id).ToList();
                context.CursoInstructor.RemoveRange(instructoresBD);

                //eliminar comentarios
                var comentariosBD=context.Comentario.Where(x=>x.CursoId== request.Id).ToList();
                context.Comentario.RemoveRange(comentariosBD);

                //eliminar Precios
                var precioBD=context.Precio.Where(x=>x.CursoId==request.Id).FirstOrDefault();
                if(precioBD != null)
                    context.Precio.Remove(precioBD);

                var curso= await context.Curso.FindAsync(request.Id);

                if(curso is null)
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound,new{curso="No se encontro el curso"});

                context.Remove(curso);

                var resultado = await context.SaveChangesAsync();

                if(resultado>0)
                    return Unit.Value;
                throw new ManejadorExcepcion(HttpStatusCode.NotModified,new{curso="No se pudieron guardar los cambios"});
            }
        }
    }
}