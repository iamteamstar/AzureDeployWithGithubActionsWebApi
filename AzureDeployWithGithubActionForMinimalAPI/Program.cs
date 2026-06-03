using AzureDeployWithGithubActionForMinimalAPI.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerGen().AddEndpointsApiExplorer();

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

var products = new List<Products>
{
	new(1, "Klavye", 450),
	new(2, "Mouse", 250),
	new(3, "Monitör", 3200)
};

app.MapGet("/", () => "Hello World!");
app.MapGet("/products", () => products);

app.MapGet("/products/{id:int}", (int id) =>
{
	var product = products.FirstOrDefault(p => p.Id == id);
	return product is not null
		? Results.Ok(product)
		: Results.NotFound();
});

app.MapPost("/products", (Products product) =>
{
	products.Add(product);
	return Results.Created($"/products/{product.Id}", product);
});

app.MapDelete("/products/{id:int}", (int id) =>
{
	var product = products.FirstOrDefault(p => p.Id == id);
	if (product is null) return Results.NotFound();

	products.Remove(product);
	return Results.NoContent();
});


app.Run();
public partial class Program { }