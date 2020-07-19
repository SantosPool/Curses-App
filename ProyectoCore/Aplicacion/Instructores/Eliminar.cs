using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Persistencia.DapperConexion.Instructor;

namespace Aplicacion.Instructores
{
    public class Eliminar
    {

        public class Ejecuta : IRequest
        {
            public Guid Id { get; set; }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private IInstructor instructorRepository;
            public Manejador(IInstructor _instructorRepository)
            {
                instructorRepository = _instructorRepository;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var resultado = await instructorRepository.Eliminar(request.Id);
                if (resultado > 0)
                    return Unit.Value;
                throw new Exception("No se pudo eliminar el instructor");
            }
        }

    }
}