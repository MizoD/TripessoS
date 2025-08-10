using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tripesso.Areas.Customer
{
    [Route("api/[area]/[controller]")]
    [Area("Customer")]
    [ApiController]
    public class HomeDataController : ControllerBase
    {
    }
}
