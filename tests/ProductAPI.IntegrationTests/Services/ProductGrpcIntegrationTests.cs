using FluentAssertions;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using ProductAPI.IntegrationTests.Helpers;
using ProductAPI.Protos;

namespace ProductAPI.IntegrationTests.Services;

public class ProductGrpcIntegrationTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly ProductService.ProductServiceClient _client;

    public ProductGrpcIntegrationTests(
        CustomWebApplicationFactory factory)
    {
        var httpClient = factory.CreateDefaultClient();

        var channel = GrpcChannel.ForAddress(
            httpClient.BaseAddress!,
            new GrpcChannelOptions
            {
                HttpClient = httpClient
            });

        _client = new ProductService.ProductServiceClient(channel);
    }


    // CREATE PRODUCT
    [Fact]
    public async Task CreateProduct_Should_Create_Product()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "Laptop",
            Description = "Gaming Laptop",
            Price = 2500,
            Available = true
        };

        // Act
        var response =
            await _client.CreateProductAsync(request);

        // Assert
        response.Should().NotBeNull();

        response.Product.Name.Should().Be("Laptop");

        response.Product.Id.Should().NotBeNullOrWhiteSpace();
    }


    // GET PRODUCT
    [Fact]
    public async Task GetProduct_Should_Return_Product()
    {
        // Arrange
        var createResponse =
            await _client.CreateProductAsync(
                new CreateProductRequest
                {
                    Name = "Phone",
                    Description = "Smartphone",
                    Price = 1200,
                    Available = true
                });

        // Act
        var response =
            await _client.GetProductAsync(
                new GetProductRequest
                {
                    Id = createResponse.Product.Id
                });

        // Assert
        response.Product.Name.Should().Be("Phone");
    }


    // LIST PRODUCTS
    [Fact]
    public async Task ListProducts_Should_Return_Products()
    {
        // Arrange
        await _client.CreateProductAsync(
            new CreateProductRequest
            {
                Name = "Tablet",
                Description = "Android Tablet",
                Price = 800,
                Available = true
            });

        // Act
        var response =
            await _client.ListProductsAsync(
                new ListProductsRequest());

        // Assert
        response.Products.Should().NotBeEmpty();
    }


    // UPDATE PRODUCT
    [Fact]
    public async Task UpdateProduct_Should_Update_Product()
    {
        // Arrange
        var created =
            await _client.CreateProductAsync(
                new CreateProductRequest
                {
                    Name = "Monitor",
                    Description = "4K Monitor",
                    Price = 500,
                    Available = true
                });

        // Act
        var updated =
            await _client.UpdateProductAsync(
                new UpdateProductRequest
                {
                    Id = created.Product.Id,
                    Name = "Updated Monitor",
                    Description = "Updated Description",
                    Price = 650,
                    Available = false
                });

        // Assert
        updated.Product.Name.Should().Be("Updated Monitor");

        updated.Product.Price.Should().Be(650);

        updated.Product.Available.Should().BeFalse();
    }


    // DELETE PRODUCT
    [Fact]
    public async Task DeleteProduct_Should_Delete_Product()
    {
        // Arrange
        var created =
            await _client.CreateProductAsync(
                new CreateProductRequest
                {
                    Name = "Keyboard",
                    Description = "Mechanical Keyboard",
                    Price = 150,
                    Available = true
                });

        // Act
        var deleted =
            await _client.DeleteProductAsync(
                new DeleteProductRequest
                {
                    Id = created.Product.Id
                });

        // Assert
        deleted.Success.Should().BeTrue();

        Func<Task> action = async () =>
            await _client.GetProductAsync(
                new GetProductRequest
                {
                    Id = created.Product.Id
                });

        await action.Should()
            .ThrowAsync<RpcException>();
    }


    // VALIDATION ERROR
    [Fact]
    public async Task CreateProduct_Should_Return_InvalidArgument()
    {
        // Arrange
        var request = new CreateProductRequest
        {
            Name = "",
            Description = "Invalid Product",
            Price = 100
        };

        // Act
        Func<Task> action = async () =>
            await _client.CreateProductAsync(request);

        // Assert
        var exception = await action.Should()
            .ThrowAsync<RpcException>();

        exception.Which.StatusCode
            .Should().Be(StatusCode.InvalidArgument);
    }


    // NOT FOUND ERROR
    [Fact]
    public async Task GetProduct_Should_Return_NotFound()
    {
        // Arrange
        var request = new GetProductRequest
        {
            Id = Guid.NewGuid().ToString()
        };

        // Act
        Func<Task> action = async () =>
            await _client.GetProductAsync(request);

        // Assert
        var exception = await action.Should()
            .ThrowAsync<RpcException>();

        exception.Which.StatusCode
            .Should().Be(StatusCode.NotFound);
    }
}