using AviAPI.Data;
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

app.MapGet("/CellMatrixes", ([FromQuery] int idLocation, [FromQuery] int idCategory, AviApiContext context) =>
{
    var baseLine = context.Matrices.Include(m => m.CellMatrices).OrderBy(m => m.IdMatrix).LastOrDefault(m => m.IdUserSegment == null);
    var cellMatrix = baseLine.CellMatrices.FirstOrDefault(c => c.IdLocation == idLocation && c.IdCategory == idCategory);
    if (cellMatrix != null)
        return new { baseLine.IdMatrix, cellMatrix.Price, cellMatrix.IdLocation, cellMatrix.IdCategory };
    return null;
});


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();