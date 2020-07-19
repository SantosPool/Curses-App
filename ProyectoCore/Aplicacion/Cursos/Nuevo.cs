using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Dominio;
using FluentValidation;
using MediatR;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Nuevo
    {

        public class Ejecuta: IRequest
        {
            public Guid? CursoId{get;set;}
            public string Titulo{get;set;}
            public string Descripcion{get;set;}
            public DateTime FechaPublicacion{get;set;}
            public List<Guid> ListaInstructor {get;set;}
            public decimal Precio{get;set;}
            public decimal Promocion{get;set;}
        }

        public class EjecutaValidacion: AbstractValidator<Ejecuta>
        {
            public EjecutaValidacion()
            {
                RuleFor(x=> x.Titulo).NotEmpty();
                RuleFor(x=> x.Descripcion).NotEmpty();
                RuleFor(x=> x.FechaPublicacion).NotEmpty();
            }
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
                // Guid _cursoId= Guid.NewGuid();
                // if(request.CursoId != null)
                var _cursoId= request.CursoId ?? Guid.NewGuid();

                var curso= new Curso{
                    CursoId= _cursoId,
                    Titulo= request.Titulo,
                    Descripcion=request.Descripcion,
                    FechaPublicacion=request.FechaPublicacion,
                    FechaCreacion =DateTime.UtcNow
                };

                context.Curso.Add(curso);
                if(request.ListaInstructor!=null){
                    CursoInstructor cursoInstructor=null;
                    foreach(var id in request.ListaInstructor){
                        cursoInstructor= new CursoInstructor{
                            CursoId= curso.CursoId,
                            InstructorId= id
                        };
                        context.CursoInstructor.Add(cursoInstructor);
                        cursoInstructor=null;
                    }
                }

                //agregar logica pra insertar precio
                var precioEntidad= new Precio{
                    CursoId= _cursoId,
                    PrecioActual=request.Precio,
                    Promocion=request.Promocion,
                    PrecioId= Guid.NewGuid()
                };

                context.Precio.Add(precioEntidad);

               var valor= await context.SaveChangesAsync();
               if(valor>0){

                   return Unit.Value;
               }

               throw new Exception("No se  pudo instertar el curso");
            }
        }

    }
}