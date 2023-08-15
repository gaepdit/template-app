using Microsoft.AspNetCore.Mvc;
using MyAppRoot.AppServices.Offices;

namespace MyAppRoot.WebApp.Api;

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

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OfficeViewDto>> GetOfficeAsync([FromRoute] Guid id)
    {
        var item = await _officeService.FindAsync(id);
        return item is null ? Problem("ID not found.", statusCode: 404) : Ok(item);
    }
}
