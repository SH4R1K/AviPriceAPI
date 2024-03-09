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

async Task GetPriceAsync(Matrix baseLine, int idLocation, int idCategory, AviApiContext context)
{
    while (true)
    {
        
    }
}

app.MapGet("/CellMatrixes", async ([FromQuery] int idLocation, [FromQuery] int idCategory, AviApiContext context) =>
{
    var baseLine = context.Matrices.Include(m => m.CellMatrices).OrderBy(m => m.IdMatrix).LastOrDefault(m => m.IdUserSegment == null);
    var cellMatrix = baseLine.CellMatrices.FirstOrDefault(c => c.IdLocation == idLocation && c.IdCategory == idCategory);
    var location = context.Locations
                        .Include(l => l.IdParentLocationNavigation)
                        .ThenInclude(l => l.IdParentLocationNavigation)
                        .FirstOrDefault(l => l.IdLocation == idLocation);
    var category = context.Categories
                        .Include(l => l.IdParentCategoryNavigation)
                        .ThenInclude(l => l.IdParentCategoryNavigation)
                        .FirstOrDefault(l => l.IdCategory == idCategory);
    if (cellMatrix != null)
        return new { baseLine.IdMatrix, cellMatrix.Price, cellMatrix.IdLocation, cellMatrix.IdCategory };
    return null;
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();