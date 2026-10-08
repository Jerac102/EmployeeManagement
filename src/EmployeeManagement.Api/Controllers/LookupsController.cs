using EmployeeManagement.Api.Contracts;
using EmployeeManagement.Data.Entities;
using EmployeeManagement.Data.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Controllers;

[ApiController]
[Route("api")]
public class LookupsController : ControllerBase
{
    [HttpGet("departments")]
    public async Task<ActionResult<IEnumerable<LookupDto>>> GetDepartments(
        [FromServices] IRepository<Department> repository,
        CancellationToken cancellationToken)
    {
        var items = await repository.GetAllAsync(cancellationToken);
        return Ok(items.Select(d => new LookupDto(d.Id, d.Name)));
    }

    [HttpGet("positions")]
    public async Task<ActionResult<IEnumerable<LookupDto>>> GetPositions(
        [FromServices] IRepository<Position> repository,
        CancellationToken cancellationToken)
    {
        var items = await repository.GetAllAsync(cancellationToken);
        return Ok(items.Select(p => new LookupDto(p.Id, p.Title)));
    }
}
