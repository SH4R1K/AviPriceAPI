using AviPriceUI.Controllers;
using AviPriceUI.Data;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Net.Http;
using System.Security.Policy;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddMemoryCache();

builder.Services.AddDbContext<AviContext>();

var app = builder.Build();

app.UseExceptionHandler("/Home/Error");

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=CellMatrices}/{action=Index}/{id=-1}");

app.MapPost("/SendLastStorage", async ([FromBody] string url) =>
{
    var httpClient = new HttpClient { BaseAddress = new Uri(url) };
    if (MatricesController.ByteArray == null)
        return Results.NoContent();
    var request = await httpClient.PostAsJsonAsync("/Storages/Update", MatricesController.ByteArray);
    if (request.StatusCode == System.Net.HttpStatusCode.OK)
        return Results.Ok();
    else
        return Results.Problem();    
});

app.Run();
