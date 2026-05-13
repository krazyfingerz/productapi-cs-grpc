using AutoMapper;
using FluentAssertions;
using Moq;
using ProductAPI.Caching;
using ProductAPI.Exceptions;
using ProductAPI.Models;
using ProductAPI.Protos;
using ProductAPI.Repositories;
using ProductAPI.Services;
using ProductAPI.UnitTests.Helpers;

namespace ProductAPI.UnitTests.Services;

public class ProductGrpcServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<IProductCacheService> _cacheMock;
    private readonly Mock<IMapper> _mapperMock;

    private readonly ProductGrpcService _service;

    private readonly TestServerCallContext _context;

    public ProductGrpcServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();

        _cacheMock = new Mock<IProductCacheService>();

        _mapperMock = new Mock<IMapper>();

        _service = new ProductGrpcService(
            _repositoryMock.Object,
            _cacheMock.Object,
            _mapperMock.Object);

        _context = new TestServerCallContext();
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

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Available = request.Available
        };

        var productMessage = new ProductMessage
        {
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Available = product.Available
        };

        _mapperMock
            .Setup(m => m.Map<Product>(request))
            .Returns(product);

        _mapperMock
            .Setup(m => m.Map<ProductMessage>(
                It.IsAny<Product>()))
            .Returns(productMessage);

        // Act
        var response =
            await _service.CreateProduct(request, _context);

        // Assert
        response.Should().NotBeNull();

        response.Product.Name.Should().Be("Laptop");

        _repositoryMock.Verify(
            r => r.CreateAsync(It.IsAny<Product>()),
            Times.Once);

        _cacheMock.Verify(
            c => c.SetAsync(It.IsAny<Product>()),
            Times.Once);
    }


    // GET PRODUCT - CACHE HIT
    [Fact]
    public async Task GetProduct_Should_Return_From_Cache()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var request = new GetProductRequest
        {
            Id = productId.ToString()
        };

        var product = new Product
        {
            Id = productId,
            Name = "Phone",
            Description = "Smartphone",
            Price = 1000,
            Available = true
        };

        var productMessage = new ProductMessage
        {
            Id = product.Id.ToString(),
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Available = product.Available
        };

        _cacheMock
            .Setup(c => c.GetAsync(productId))
            .ReturnsAsync(product);

        _mapperMock
            .Setup(m => m.Map<ProductMessage>(product))
            .Returns(productMessage);

        // Act
        var response =
            await _service.GetProduct(request, _context);

        // Assert
        response.Product.Name.Should().Be("Phone");

        _repositoryMock.Verify(
            r => r.GetByIdAsync(It.IsAny<Guid>()),
            Times.Never);
    }


    // GET PRODUCT - NOT FOUND
    [Fact]
    public async Task GetProduct_Should_Throw_When_NotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();

        var request = new GetProductRequest
        {
            Id = productId.ToString()
        };

        _cacheMock
            .Setup(c => c.GetAsync(productId))
            .ReturnsAsync((Product?)null);

        _repositoryMock
            .Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        // Act
        Func<Task> action = async () =>
            await _service.GetProduct(request, _context);

        // Assert
        await action.Should()
            .ThrowAsync<ProductNotFoundException>();
    }


    // VALIDATION TEST
    [Fact]
    public async Task CreateProduct_Should_Throw_When_Name_Is_Empty()
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
            await _service.CreateProduct(request, _context);

        // Assert
        await action.Should()
            .ThrowAsync<ValidationException>();
    }
}