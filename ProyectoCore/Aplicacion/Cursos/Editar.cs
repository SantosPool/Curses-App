using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Editar
    {
        public class Ejecuta: IRequest
        {
            public Guid CursoId{get; set;}
            public string Titulo{get;set;}
            public string Descripcion{get;set;}
            public DateTime? FechaPublicacion{get;set;}

            public List<Guid> ListaInstructor{get;set;}
            public decimal? Precio{get;set;}
            public decimal? Promocion{get;set;}
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
                var curso= await context.Curso.FindAsync(request.CursoId);
                if(curso is null){
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound,new{curso="El Curso no existe"});
                }
                curso.Titulo=request.Titulo ?? curso.Titulo;
                curso.Descripcion= request.Descripcion ?? curso.Descripcion;
                curso.FechaPublicacion = request.FechaPublicacion ?? curso.FechaPublicacion;
                curso.FechaCreacion =DateTime.UtcNow;
                //actualizar el precio de curso
                var precioEntidad=context.Precio.Where(x=>x.CursoId==curso.CursoId).FirstOrDefault();
                if(precioEntidad!=null){
                    precioEntidad.Promocion=request.Promocion ??  precioEntidad.Promocion;
                    precioEntidad.PrecioActual=request.Precio ?? precioEntidad.PrecioActual;
                }else{
                    precioEntidad=new Precio{
                        PrecioId=Guid.NewGuid(),
                        Promocion=request.Promocion ??  0,
                        PrecioActual=request.Precio ?? 0,
                        CursoId= curso.CursoId
                    };
                    await context.Precio.AddAsync(precioEntidad);
                }

                //actuzalizar lista instructores
                if(request.ListaInstructor!=null){
                    if(request.ListaInstructor.Count()>0){
                        var instructoresBD= context.CursoInstructor.Where(x=>x.CursoId== request.CursoId).ToList();

                        context.CursoInstructor.RemoveRange(instructoresBD);
                        
                        CursoInstructor cursoInstructor=null;
                        foreach(var id in request.ListaInstructor){
                            cursoInstructor= new CursoInstructor{
                                CursoId= request.CursoId,
                                InstructorId= id
                            };
                            context.CursoInstructor.Add(cursoInstructor);
                            cursoInstructor=null;
                        }
                    }
                }

                var resultado= await context.SaveChangesAsync();
                if(resultado>0)
                    return Unit.Value;
                throw new Exception("No se Guardo en el curso Correctamente");
            }
        }
    }
}