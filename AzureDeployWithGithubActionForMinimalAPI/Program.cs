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
app.MapPut("/products/{id:int}", (int id, Products input) =>
{
	var index = products.FindIndex(p => p.Id == id);
	if (index == -1) return Results.NotFound();
	var errors = Validate(input);
	if (errors is not null) return Results.ValidationProblem(errors);
	products[index] = input with { Id = id };
	return Results.Ok(products[index]);
});

app.MapDelete("/products/{id:int}", (int id) =>
{
	var product = products.FirstOrDefault(p => p.Id == id);
	if (product is null) return Results.NotFound();

	products.Remove(product);
	return Results.NoContent();
});

static Dictionary<string, string[]>? Validate(Products p)
{
	var errors = new Dictionary<string, string[]>();
	if (string.IsNullOrWhiteSpace(p.Name)) errors["Name"] = ["Ýsim boþ olamaz."];
	if (p.Price <= 0) errors["Price"] = ["Fiyat 0'dan büyük olmalý."];
	return errors.Count > 0 ? errors : null;
}

app.Run();
public partial class Program { }