using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShootingAcademy.Models;

namespace ShootingAcademy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebugController : ControllerBase
    {
        private ApplicationDbContext dbContext;

        public DebugController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IResult TestDb()
        {
            return Results.Json(dbContext.Users.Take(1).ToList()); 
        }
    }
}
