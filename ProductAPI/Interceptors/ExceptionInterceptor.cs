using Grpc.Core;
using Grpc.Core.Interceptors;
using ProductAPI.Exceptions;

namespace ProductAPI.Interceptors;

public class ExceptionInterceptor : Interceptor
{
    private readonly ILogger<ExceptionInterceptor> _logger;

    public ExceptionInterceptor(
        ILogger<ExceptionInterceptor> logger)
    {
        _logger = logger;
    }

    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            return await continuation(request, context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, ex.Message);

            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    ex.Message
                ));
        }
        catch (ProductNotFoundException ex)
        {
            _logger.LogWarning(ex, ex.Message);

            throw new RpcException(
                new Status(
                    StatusCode.NotFound,
                    ex.Message
                ));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled server error.");

            throw new RpcException(
                new Status(
                    StatusCode.Internal,
                    "An internal server error occurred."
                ));
        }
    }
}