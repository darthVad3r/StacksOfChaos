using Microsoft.AspNetCore.Mvc;
using SOCApi.ViewModels;

namespace SOCApi.Controllers
{
    public interface IAccountController
    {
        Task<IActionResult> Register([FromBody] RegisterRequest request);
    }
}