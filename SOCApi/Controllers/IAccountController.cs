using Microsoft.AspNetCore.Mvc;
using SOCApi.Models;

namespace SOCApi.Controllers
{
    public interface IAccountController
    {
        Task<IActionResult> Register([FromBody] RegisterRequest request);
    }
}