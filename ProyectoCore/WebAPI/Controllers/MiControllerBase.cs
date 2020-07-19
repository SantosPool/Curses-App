using MediatR;
using Microsoft.AspNetCore.Mvc;

using Microsoft.Extensions.DependencyInjection;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MiControllerBase: ControllerBase
    {
        private  IMediator mediator;

        protected IMediator _Mediator=> mediator ?? (mediator = HttpContext.RequestServices.GetService<IMediator>());
    }
}