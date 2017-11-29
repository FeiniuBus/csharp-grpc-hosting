namespace FeiniuBus.Grpc.Hosting.Builder
{
    public interface IApplicationBuilderFactory
    {
        IApplicationBuilder CreateBuilder();
    }
}