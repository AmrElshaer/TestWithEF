using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestWithEF.Entities;
using TestWithEF.Filiters;
using TestWithEF.Models;

namespace TestWithEF.Controllers;

[ApiController]
[ApiExceptionFilter]
[Route("api/[controller]")]
public class WarehouseLayoutGroupController : ControllerBase
{
    private readonly TestDbContext _dbContext;

    public WarehouseLayoutGroupController(TestDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> Post(CreateWarehouseLayoutGroupRequest request)
    {
        var subcategories = await _dbContext.Subcategories.Where(s =>
            request.Subcategories.Contains(s.Id)).ToListAsync();

        var warehouseLayoutGroup = new WarehouseLayoutGroup(
            Guid.NewGuid(),
            request.Name,
            subcategories);

        await _dbContext.WarehouseLayoutGroups.AddAsync(warehouseLayoutGroup);
        await _dbContext.SaveChangesAsync();

        return Ok(warehouseLayoutGroup.Id);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Put(Guid id, UpdateWarehouseLayoutGroupRequest request)
    {
        var subcategories = await _dbContext.Subcategories.Where(s =>
            request.Subcategories.Contains(s.Id)).ToListAsync();

        var warehouseLayoutGroup = await _dbContext.WarehouseLayoutGroups
            .Include(w=>w.Subcategories)
            .FirstOrDefaultAsync(w=>w.Id==id);

        if (warehouseLayoutGroup is null)
        {
            return NotFound();
        }

        warehouseLayoutGroup.Update(request.Name, subcategories);
        await _dbContext.SaveChangesAsync();

        return Ok(warehouseLayoutGroup.Id);
    }
}
