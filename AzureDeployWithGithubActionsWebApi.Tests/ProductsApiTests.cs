using AzureDeployWithGithubActionForMinimalAPI.Models;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DeployWithAzure.Tests;

public class ProductsApiTests : IDisposable
{
	private readonly WebApplicationFactory<Program> _factory;
	private readonly HttpClient _client;

	public ProductsApiTests()
	{
		_factory = new WebApplicationFactory<Program>();
		_client = _factory.CreateClient();
	}

	[Fact]
	public async Task Get_Products_UcUrunDoner()
	{
		var products = await _client.GetFromJsonAsync<List<Products>>("/products");
		Assert.NotNull(products);
		Assert.Equal(3, products!.Count);
	}

	[Fact]
	public async Task Get_ProductById_VarOlanId_UrunuDoner()
	{
		var response = await _client.GetAsync("/products/1");
		Assert.Equal(HttpStatusCode.OK, response.StatusCode);

		var product = await response.Content.ReadFromJsonAsync<Products>();
		Assert.NotNull(product);
		Assert.Equal("Klavye", product!.Name);
	}

	[Fact]
	public async Task Get_ProductById_OlmayanId_NotFoundDoner()
	{
		var response = await _client.GetAsync("/products/99");
		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	[Fact]
	public async Task Post_Product_CreatedDoner()
	{
		var yeniUrun = new Products(4, "Kulaklik", 900);
		var response = await _client.PostAsJsonAsync("/products", yeniUrun);

		Assert.Equal(HttpStatusCode.Created, response.StatusCode);
		Assert.Contains("/products/4", response.Headers.Location?.ToString());

		var products = await _client.GetFromJsonAsync<List<Products>>("/products");
		Assert.Equal(4, products!.Count);
	}

	[Fact]
	public async Task Delete_VarOlanUrun_NoContentDoner()
	{
		var response = await _client.DeleteAsync("/products/2");
		Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
	}

	[Fact]
	public async Task Delete_OlmayanUrun_NotFoundDoner()
	{
		var response = await _client.DeleteAsync("/products/99");
		Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
	}

	public void Dispose()
	{
		_client.Dispose();
		_factory.Dispose();
	}
}