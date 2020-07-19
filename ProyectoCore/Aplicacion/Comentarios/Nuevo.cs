using System;
using System.Threading;
using System.Threading.Tasks;
using Dominio;
using FluentValidation;
using MediatR;
using Persistencia;

namespace Aplicacion.Comentarios
{
    public class Nuevo
    {
        public class Ejecuta : IRequest
        {
            public string Alumno { get; set; }
            public int Puntaje { get; set; }
            public string Comentario { get; set; }
            public Guid CursoId { get; set; }
        }
        public class EjecutaValidacion: AbstractValidator<Ejecuta>{
            public EjecutaValidacion()
            {
                RuleFor(x=>x.Alumno).NotEmpty();
                RuleFor(x=>x.Puntaje).NotEmpty();
                RuleFor(x=>x.Comentario).NotEmpty();
                RuleFor(x=>x.CursoId).NotEmpty();
            }
        }
        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CursosOnlineContext context;
            public Manejador(CursosOnlineContext _context)
            {
                this.context = _context;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var comentario = new Comentario
                {
                    ComentarioId = Guid.NewGuid(),
                    Alumno = request.Alumno,
                    Puntaje= request.Puntaje,
                    ComentarioTexto = request.Comentario,
                    CursoId = request.CursoId,
                    FechaCreacion =DateTime.UtcNow
                };

                context.Comentario.Add(comentario);

                var resultado = await context.SaveChangesAsync();
                if (resultado > 0)
                    return Unit.Value;
                throw new Exception("No se pudo guardar el comentario en la BD");
            }
        }
    }
}