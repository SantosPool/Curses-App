using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace Persistencia.DapperConexion.Instructor
{
    public class InstructorRepositorio : IInstructor
    {
        public readonly IFactoryConnection factoryConnection;
        public InstructorRepositorio(IFactoryConnection _factoryConnection)
        {
            factoryConnection = _factoryConnection;

        }
        public async Task<int> Actualizar(Guid instructorId, string nombre, string apellidos, string grado)
        {
            int resultado = 0;
            var storeProcedure = "usp_instructor_editar";
            try
            {
                var connection = factoryConnection.GetConnection();
                resultado = await connection.ExecuteAsync(
                    storeProcedure,
                        new
                        {
                            InstructorId = instructorId,
                            Nombre = nombre,
                            Apellidos = apellidos,
                            Grado = grado
                        },
                        commandType: CommandType.StoredProcedure
                    );
            }
            catch (Exception e)
            {
                throw new Exception("No se pudo editar la data del inistructor", e);
            }
            finally
            {
                factoryConnection.CloseConnection();
            }
            return resultado;
        }

        public async Task<int> Eliminar(Guid id)
        {
            int resultado = 0;
            var storeProcedure = "usp_instructor_elimina";
            try
            {
                var connection = factoryConnection.GetConnection();
                resultado = await connection.ExecuteAsync(
                    storeProcedure,
                        new
                        {
                            InstructorId = id
                        },
                        commandType: CommandType.StoredProcedure
                    );
            }
            catch (Exception e)
            {
                throw new Exception("No se pudo eliminar el instructor", e);
            }
            finally
            {
                factoryConnection.CloseConnection();
            }
            return resultado;
        }

        public async Task<int> Nuevo(string nombre, string apellidos, string grado)
        {
            int resultado = 0;
            var storeProcedure = "usp_Instructor_Nuevo";
            try
            {
                var connection = factoryConnection.GetConnection();

                resultado = await connection.ExecuteAsync(storeProcedure, new
                {
                    InstructorId = Guid.NewGuid(),
                    Nombre = nombre,
                    Apellidos = apellidos,
                    Grado = grado
                },
                    commandType: CommandType.StoredProcedure
                    );
            }
            catch (Exception e)
            {
                throw new Exception("Error al crear el nuevo Instructor", e);
            }
            finally
            {
                factoryConnection.CloseConnection();
            }
            return resultado;
        }

        public async Task<IEnumerable<InstructorModel>> ObtenerLista()
        {
            IEnumerable<InstructorModel> instructorList = null;
            var storeProcedure = "usp_Obtener_Instructores";
            try
            {
                var connection = factoryConnection.GetConnection();
                instructorList = await connection.QueryAsync<InstructorModel>(storeProcedure, null, commandType: CommandType.StoredProcedure);

            }
            catch (Exception e)
            {
                throw new Exception("Error en la consulta de datos", e);
            }
            finally
            {
                factoryConnection.CloseConnection();
            }
            return instructorList;
        }

        public async Task<InstructorModel> ObtenerPorId(Guid id)
        {
            var storeProcedure = "usp_obtener_instructor_por_id";
            InstructorModel instructor=null;
            try
            {
                var connection = factoryConnection.GetConnection();
                instructor = await connection.QueryFirstAsync<InstructorModel>(
                    storeProcedure,
                        new
                        {
                            InstructorId = id
                        },
                        commandType: CommandType.StoredProcedure
                    );
            }
            catch (Exception e)
            {
                throw new Exception("No se pudo obtener el instructor", e);
            }
            finally
            {
                factoryConnection.CloseConnection();
            }
            return instructor;
        }
    }
}