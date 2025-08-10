using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Tripesso.Areas.Admin
{
    [Route("api/[area]/[controller]")]
    [Area("Admin")]
    [Authorize(Roles = $"{SD.SuperAdmin},{SD.Admin}")]
    [ApiController]
    public class HomeDataController : ControllerBase
    {
    }
}
