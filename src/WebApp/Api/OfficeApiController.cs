using Microsoft.AspNetCore.Mvc;
using MyApp.AppServices.Offices;

namespace MyApp.WebApp.Api;

[ApiController]
[Route("api/offices")]
[Produces("application/json")]
public class OfficeApiController : Controller
{
    private readonly IOfficeService _officeService;
    public OfficeApiController(IOfficeService officeService) => _officeService = officeService;

    [HttpGet]
    public async Task<IReadOnlyList<OfficeViewDto>> ListOfficesAsync() =>
        (await _officeService.GetListAsync());
}
