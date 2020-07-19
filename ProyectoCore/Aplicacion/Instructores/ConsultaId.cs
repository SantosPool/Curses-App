using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using MediatR;
using Persistencia.DapperConexion.Instructor;

namespace Aplicacion.Instructores
{
    public class ConsultaId
    {
        public class Ejecuta : IRequest<InstructorModel>
        {
            public Guid InstructorId { get; set; }
        }

        public class Manejador : IRequestHandler<Ejecuta, InstructorModel>
        {
            private readonly IInstructor instructorRepository;
            public Manejador(IInstructor _instructorRepository)
            {
                instructorRepository = _instructorRepository;
            }
            public async Task<InstructorModel> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var instructor = await instructorRepository.ObtenerPorId(request.InstructorId);
                if(instructor is null)
                    throw new ManejadorExcepcion(HttpStatusCode.NotFound, new {mensaje= "No se encontro el instructor"});
                return instructor;
            }
        }
    }
}