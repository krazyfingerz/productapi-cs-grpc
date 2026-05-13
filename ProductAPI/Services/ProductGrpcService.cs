using AutoMapper;
using Grpc.Core;
using ProductAPI.Caching;
using ProductAPI.Exceptions;
using ProductAPI.Models;
using ProductAPI.Protos;
using ProductAPI.Repositories;

namespace ProductAPI.Services;

public class ProductGrpcService : ProductService.ProductServiceBase
{
    private readonly IProductRepository _repository;
    private readonly IProductCacheService _cacheService;
    private readonly IMapper _mapper;

    public ProductGrpcService(
        IProductRepository repository,
        IProductCacheService cacheService,
        IMapper mapper)
    {
        _repository = repository;
        _cacheService = cacheService;
        _mapper = mapper;
    }


    public override async Task<ProductResponse> CreateProduct(
        CreateProductRequest request,
        ServerCallContext context)
    {
        ValidateCreateRequest(request);

        var product = _mapper.Map<Product>(request);

        product.Id = Guid.NewGuid();

        await _repository.CreateAsync(product);

        await _cacheService.SetAsync(product);

        var productMessage =
            _mapper.Map<ProductMessage>(product);

        return new ProductResponse
        {
            Product = productMessage
        };
    }


    public override async Task<ProductResponse> GetProduct(
        GetProductRequest request,
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var productId))
        {
            // throw new RpcException(
            //     new Status(
            //         StatusCode.InvalidArgument,
            //         "Invalid product ID format."
            //     ));
            throw new ProductNotFoundException(productId); 
        }

        // Try cache first
        var cachedProduct =
            await _cacheService.GetAsync(productId);

        if (cachedProduct is not null)
        {
            return new ProductResponse
            {
                Product = _mapper.Map<ProductMessage>(cachedProduct)
            };
        }

        // Fallback to repository
        var product =
            await _repository.GetByIdAsync(productId);

        if (product is null)
        {
            throw new ProductNotFoundException(productId); 
        }

        // Store in cache
        await _cacheService.SetAsync(product);

        return new ProductResponse
        {
            Product = _mapper.Map<ProductMessage>(product)
        };
    }


    public override async Task<ListProductsResponse> ListProducts(
        ListProductsRequest request,
        ServerCallContext context)
    {
        var products = await _repository.GetAllAsync();

        var productMessages =
            _mapper.Map<IEnumerable<ProductMessage>>(products);

        var response = new ListProductsResponse();

        response.Products.AddRange(productMessages);

        return response;
    }


    public override async Task<ProductResponse> UpdateProduct(
        UpdateProductRequest request,
        ServerCallContext context)
    {
        ValidateUpdateRequest(request);

        var product =
            _mapper.Map<Product>(request);

        var updatedProduct =
            await _repository.UpdateAsync(product);

        if (updatedProduct is null)
        {
            throw new RpcException(
                new Status(
                    StatusCode.NotFound,
                    $"Product with ID '{request.Id}' was not found."
                ));
        }

        // Refresh cache
        await _cacheService.SetAsync(updatedProduct);

        return new ProductResponse
        {
            Product = _mapper.Map<ProductMessage>(updatedProduct)
        };
    }


    public override async Task<DeleteProductResponse> DeleteProduct(
        DeleteProductRequest request,
        ServerCallContext context)
    {
        if (!Guid.TryParse(request.Id, out var productId))
        {
            throw new ProductNotFoundException(productId); 
        }

        var deleted =
            await _repository.DeleteAsync(productId);

        if (!deleted)
        {
            throw new ProductNotFoundException(productId); 
        }

        // Remove from cache
        await _cacheService.RemoveAsync(productId);

        return new DeleteProductResponse
        {
            Success = true
        };
    }


    private static void ValidateCreateRequest(
        CreateProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new ValidationException(
                "Product name is required."
            );
        }

        if (request.Price < 0)
        {
            throw new ValidationException(
                "Price cannot be negative."
            );
        }
    }

    private static void ValidateUpdateRequest(
        UpdateProductRequest request)
    {
        if (!Guid.TryParse(request.Id, out _))
        {
            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    "Invalid product ID format."
                ));
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    "Product name is required."
                ));
        }

        if (request.Price < 0)
        {
            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    "Price cannot be negative."
                ));
        }
    }
}