using AviAPI.Classes;
using AviAPI.Data;
using AviAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProtoBuf;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AviApiContext>();
builder.Services.AddMemoryCache();
builder.Services.AddTransient<StorageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

async Task<CellMatrix?> GetPriceAsync(Matrix baseLine, int idLocation, int idCategory, AviApiContext context, StorageService storage)
{
    List<int>? locationParents, categoriesParents;
    if (storage.LocationTreePaths != null && storage.CategoryTreePaths != null)
    {
        locationParents = storage.LocationTreePaths
            .OrderBy(l => l.Depth)
            .Where(l => l.Descendant == idLocation)
            .Select(l => l.Ancestor).ToList();
        categoriesParents = storage.CategoryTreePaths
            .OrderBy(c => c.Depth)
            .Where(c => c.Descendant == idCategory)
            .Select(l => l.Ancestor).ToList();
        return baseLine.CellMatrices.Where(cm => locationParents.Contains(cm.IdLocation) && categoriesParents.Contains(cm.IdCategory)).FirstOrDefault();
    }
    try // ���� ����� �� ��������� ����� ����� ��������� � �����������, �� ���������� � ��
    {
        locationParents = await context.LocationTreePaths
            .AsNoTracking()
            .OrderBy(l => l.Depth)
            .Where(l => l.Descendant == idLocation).Select(l => l.Ancestor).ToListAsync();
        categoriesParents = await context.CategoryTreePaths
            .AsNoTracking()
            .OrderBy(c => c.Depth)
            .Where(c => c.Descendant == idCategory).Select(l => l.Ancestor).ToListAsync();
        return baseLine.CellMatrices.Where(cm => locationParents.Contains(cm.IdLocation) && categoriesParents.Contains(cm.IdCategory)).FirstOrDefault();
    }
    catch
    {
        return null;
    }
}

app.MapGet("/CellMatrixes", async ([FromQuery] int idLocation, [FromQuery] int idCategory, [FromQuery] int? idUserSegment, AviApiContext context, StorageService storage) =>
{
    if (storage.Matrices == null)
        return Results.Problem("�� �������� Storage");
    var baseLine = storage.Matrices.FirstOrDefault(m => m.IdUserSegment == null);
    if (baseLine == null)
        return Results.Problem("�� ������ BaseLine");
    var discountLines = storage.Matrices.Where(m => m.IdUserSegment != null && m.IdUserSegment == idUserSegment).OrderByDescending(m => m.IdMatrix).ToList();
    CellMatrix? cellMatrix = null;
    foreach (var discountLine in discountLines)
    {
        cellMatrix = await GetPriceAsync(discountLine, idLocation, idCategory, context, storage);
        if (cellMatrix != null)
            return Results.Ok(new { discountLine.IdMatrix, cellMatrix.Price, cellMatrix.IdLocation, cellMatrix.IdCategory, idUserSegment });
    }
    cellMatrix = await GetPriceAsync(baseLine, idLocation, idCategory, context, storage);
    if (cellMatrix != null)
    {
        return Results.Ok(new { baseLine.IdMatrix, cellMatrix.Price, cellMatrix.IdLocation, cellMatrix.IdCategory, idUserSegment });
    }
    return Results.NotFound(null);
});

app.MapPost("/Storages/Update", async ([FromBody] byte[] storage, StorageService storageService) =>
{
    using (var memoryStream = new MemoryStream(storage))
    {
        storageService.Matrices = Serializer.DeserializeItems<Matrix>(memoryStream, PrefixStyle.Fixed32, -1).ToList();
    }
    if (storageService.Matrices != null)
    {
        return Results.Ok();
    }
    else
        return Results.BadRequest();
});

app.MapPost("/Locations/Update", async ([FromBody] byte[] storage, StorageService storageService) =>
{
    using (var memoryStream = new MemoryStream(storage))
    {
        storageService.LocationTreePaths = Serializer.DeserializeItems<LocationTreePath>(memoryStream, PrefixStyle.Fixed32, -1).ToList();
    }
    if (storageService.LocationTreePaths != null)
    {
        return Results.Ok();
    }
    else
        return Results.BadRequest();
});

app.MapPost("/Categories/Update", async ([FromBody] byte[] storage, StorageService storageService) =>
{
    using (var memoryStream = new MemoryStream(storage))
    {
        storageService.CategoryTreePaths = Serializer.DeserializeItems<CategoryTreePath>(memoryStream, PrefixStyle.Fixed32, -1).ToList();
    }
    if (storageService.CategoryTreePaths != null)
    {
        return Results.Ok();
    }
    else
        return Results.BadRequest();
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();