using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    public class AuthController: Controller
    {
        
        public IAuthRepository _repo { get; }
        public AuthController(IAuthRepository repo)
        {
            _repo = repo;
        }

    }
}