using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Persistencia.DapperConexion.Instructor;

namespace Aplicacion.Instructores
{
    public class Consulta
    {
        public class Lista: IRequest<List<InstructorModel>>
        {
            
        }

        public class Manejador : IRequestHandler<Lista, List<InstructorModel>>
        {
            private readonly IInstructor instructorRepository;
            public Manejador(IInstructor _instructorRepository)
            {
                instructorRepository=_instructorRepository;
            }
            public async Task<List<InstructorModel>> Handle(Lista request, CancellationToken cancellationToken)
            {
               var resultado= await instructorRepository.ObtenerLista();
               return resultado.ToList();
            }
        }

    }
}