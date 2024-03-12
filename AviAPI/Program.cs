using AviAPI.Data;
using AviAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AviApiContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

async Task<CellMatrix?> GetPriceAsync(Matrix baseLine, int idLocation, int idCategory, AviApiContext context)
{
    CellMatrix? result = null;
    var locationParents = context.LocationTreePaths.OrderBy(l => l.Depth).Where(l => l.Descendant == idLocation).ToList();
    var categoriesParents = context.CategoryTreePaths.OrderBy(c => c.Depth).Where(c => c.Descendant == idCategory).ToList();
    result = baseLine.CellMatrices.FirstOrDefault(cm => locationParents.Any(l => cm.IdLocation == l.Descendant) && categoriesParents.Any(c => cm.IdCategory == c.Descendant));
    return result;
}

app.MapGet("/CellMatrixes", async ([FromQuery] int idLocation, [FromQuery] int idCategory, [FromQuery] int? idUserSegment, AviApiContext context) =>
{
    var discountLines = context.Matrices.Include(m => m.CellMatrices).Where(m => m.IdUserSegment != null && m.IdUserSegment == idUserSegment).OrderByDescending(m => m.IdMatrix).ToList();
    CellMatrix? cellMatrix = null;
    foreach (var discountLine in discountLines)
    {
        cellMatrix = await GetPriceAsync(discountLine, idLocation, idCategory, context);
        if (cellMatrix != null)
            return Results.Ok(new { discountLine.IdMatrix, cellMatrix.Price, cellMatrix.IdLocation, cellMatrix.IdCategory, idUserSegment });
    }
    var baseLine = context.Matrices.Include(m => m.CellMatrices).OrderBy(m => m.IdMatrix).LastOrDefault(m => m.IdUserSegment == null);
    cellMatrix = await GetPriceAsync(baseLine, idLocation, idCategory, context);
    if (cellMatrix != null)
    {
        return Results.Ok(new { baseLine.IdMatrix, cellMatrix.Price, cellMatrix.IdLocation, cellMatrix.IdCategory, idUserSegment });
    }
    return Results.NotFound(null);
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();