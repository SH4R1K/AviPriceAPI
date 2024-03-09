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
    int newCategoryLocation = idCategory;
    var location = context.Locations
                        .FirstOrDefault(l => l.IdLocation == idLocation);
    var category = context.Categories
                        .FirstOrDefault(l => l.IdCategory == idCategory);
    var oldCategory = category;
    CellMatrix? result = null;
    while (location != null)
    {
        do
        {
            result = baseLine.CellMatrices.FirstOrDefault(c => c.IdLocation == location.IdLocation && c.IdCategory == category.IdCategory);
            if (result != null)
                break;
            category = context.Categories.FirstOrDefault(l => l.IdCategory == category.IdParentCategory);
        } while (category != null);
        if (result != null)
            break;
        location = context.Locations.FirstOrDefault(l => l.IdLocation == location.IdParentLocation);
        category = oldCategory;
    }
    return result;
}

app.MapGet("/CellMatrixes", async ([FromQuery] int idLocation, [FromQuery] int idCategory, AviApiContext context) =>
{
    var baseLine = context.Matrices.Include(m => m.CellMatrices).OrderBy(m => m.IdMatrix).LastOrDefault(m => m.IdUserSegment == null);
    var cellMatrix = await GetPriceAsync(baseLine, idLocation, idCategory, context);
    if (cellMatrix != null)
        return new { baseLine.IdMatrix, cellMatrix.Price, cellMatrix.IdLocation, cellMatrix.IdCategory };
    return null;
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();