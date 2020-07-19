using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dominio;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistencia;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //encargado de crear aplicacion y ejecutarla
            var hostServer = CreateHostBuilder(args).Build();
            using(var ambiente=hostServer.Services.CreateScope())
            {
                var services= ambiente.ServiceProvider;
                try
                {
                    var usuarioManager= services.GetRequiredService<UserManager<Usuario>>();
                    var context= services.GetRequiredService<CursosOnlineContext>();
                    context.Database.Migrate();
                    DataPrueba.InsertarData(context,usuarioManager).Wait();
                }
                catch(Exception ex)
                {
                    var loggin= services.GetRequiredService<ILogger<Program>>();
                    loggin.LogError(ex,"Ocurrio un error en la migracion");

                }
            }
            hostServer.Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
