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
    var location = context.Locations
                        .FirstOrDefault(l => l.IdLocation == idLocation);
    var category = context.Categories
                        .FirstOrDefault(l => l.IdCategory == idCategory);
    var oldCategory = category;
    CellMatrix? result = null;
    while (location != null)
    {
        while (category != null) 
        {
            result = baseLine.CellMatrices.FirstOrDefault(c => c.IdLocation == location.IdLocation && c.IdCategory == category.IdCategory);
            if (result != null)
                break;
            category = context.Categories.FirstOrDefault(l => l.IdCategory == category.IdParentCategory);
        }
        if (result != null)
            break;
        location = context.Locations.FirstOrDefault(l => l.IdLocation == location.IdParentLocation);
        category = oldCategory;
    }
    return result;
}

app.MapGet("/CellMatrixes", async ([FromQuery] int idLocation, [FromQuery] int idCategory, [FromQuery] int? idUserSegment, AviApiContext context) =>
{
    var baseLine = context.Matrices.Include(m => m.CellMatrices).OrderBy(m => m.IdMatrix).LastOrDefault(m => m.IdUserSegment == null);
    var discountLines = context.Matrices.Include(m => m.CellMatrices).Where(m => m.IdUserSegment == idUserSegment).OrderByDescending(m => m.IdMatrix).ToList();
    CellMatrix? cellMatrix = null;
    if (discountLines.Count > 0)
    {
        foreach (var discountLine in discountLines)
        {
            cellMatrix = await GetPriceAsync(discountLine, idLocation, idCategory, context);
            if (cellMatrix != null)
                return Results.Ok(new { discountLine.IdMatrix, cellMatrix.Price, cellMatrix.IdLocation, cellMatrix.IdCategory, idUserSegment });
        }
    }
    cellMatrix = await GetPriceAsync(baseLine, idLocation, idCategory, context);
    if (cellMatrix != null)
    {
        return Results.Ok(new { baseLine.IdMatrix, cellMatrix.Price, cellMatrix.IdLocation, cellMatrix.IdCategory,  });
    }
    return Results.NotFound(null); 
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();