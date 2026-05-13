using Grpc.Core;

namespace ProductAPI.UnitTests.Helpers;

public class TestServerCallContext : ServerCallContext
{
    protected override string MethodCore => "TestMethod";

    protected override string HostCore => "localhost";

    protected override string PeerCore => "peer";

    protected override DateTime DeadlineCore =>
        DateTime.UtcNow.AddMinutes(30);

    protected override Metadata RequestHeadersCore =>
        new();

    protected override CancellationToken CancellationTokenCore =>
        CancellationToken.None;

    protected override Metadata ResponseTrailersCore =>
        new();

    protected override Status StatusCore
    {
        get;
        set;
    }

    protected override WriteOptions? WriteOptionsCore
    {
        get;
        set;
    }

    protected override AuthContext AuthContextCore =>
        new(
            "anonymous",
            new Dictionary<string, List<AuthProperty>>());

    protected override ContextPropagationToken CreatePropagationTokenCore(
        ContextPropagationOptions? options)
    {
        return null!;
    }

    protected override Task WriteResponseHeadersAsyncCore(
        Metadata responseHeaders)
    {
        return Task.CompletedTask;
    }
}