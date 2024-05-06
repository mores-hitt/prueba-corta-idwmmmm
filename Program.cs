using System.Linq.Expressions;
using chairs_dotnet7_api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DataContext>(opt => opt.UseInMemoryDatabase("chairlist"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

var chairs = app.MapGroup("api/chair");

//TODO: ASIGNACION DE RUTAS A LOS ENDPOINTS

chairs.MapPost("/", CreateChair);

chairs.MapGet("/", GetChairs);

chairs.MapGet("/{name}", GetChairByName);

chairs.MapPut("/{id}", UpdateChairById);

chairs.MapPut("/{id}/stock", UpdateChairStock);

chairs.MapPost("/purchase/", BuyChair);

chairs.MapDelete("/{id}", DeleteChair);


app.Run();

//TODO: ENDPOINTS SOLICITADOS


static async Task<IResult> CreateChair(DataContext db, Chair chair)
{
    try
    {
        var c = await db.Chairs.FindAsync(chair.Nombre);
        if(!(c is null)){
            return TypedResults.BadRequest("mala reqe");
        } else{
            await db.Chairs.AddAsync(chair);
            await db.SaveChangesAsync();
            return TypedResults.Ok();
        }
    }
    catch (System.Exception)
    {
        return TypedResults.BadRequest();
    }
}



static async Task<IResult> GetChairs(DataContext db)
{
    var chairs = await db.Chairs.ToListAsync();
    return TypedResults.Ok(chairs);
}


static async Task<IResult> GetChairByName(DataContext db, string nombre)
{
    var c = db.Chairs.Where(c => c.Nombre == nombre);
    if(c is null)
    {
        return TypedResults.NotFound();
    } else {
        return TypedResults.Ok(c);
    }
}


static async Task<IResult> UpdateChairById(DataContext db, Chair chair)
{
    var c = await db.Chairs.FindAsync();

    if(c is null)
    {
        return TypedResults.NotFound();
    }

    c.Altura = chair.Altura;
    c.Anchura = chair.Anchura;
    c.Color = chair.Color;
    c.Material = chair.Material;
    c.Nombre = chair.Nombre;
    c.Precio = chair.Precio;
    c.Profundidad = chair.Profundidad;

    await db.SaveChangesAsync();

    return TypedResults.Ok();


}


static async Task<IResult> UpdateChairStock(DataContext db, int stock, int id)
{
    var c = await db.Chairs.FindAsync(id);

    if(c is null)
    {
        return TypedResults.NotFound();
    }

    c.Stock = stock;

    await db.SaveChangesAsync();

    return TypedResults.Ok();

}


static async Task<IResult> BuyChair(DataContext db, int id, int cantidad, int precioTotal)
{
    var c = await db.Chairs.FindAsync(id);

    if(c is null)
    {
        return TypedResults.BadRequest();
    } else if(c.Stock > cantidad)
    {
        return TypedResults.BadRequest();
    } else if(c.Precio * c.Stock != precioTotal)
    {
        return TypedResults.BadRequest();
    }

    c.Stock = c.Stock - cantidad;

    await db.SaveChangesAsync();

    return TypedResults.Ok();
}



static async Task<IResult> DeleteChair(DataContext db, int id)
{
    var c = await db.Chairs.FindAsync(id);

    if(c is null)
    {
        return TypedResults.BadRequest();
    }

    db.Remove(c);
    await db.SaveChangesAsync();

    return TypedResults.Ok();


}




