namespace FeiniuBus.Grpc.Hosting
{
    public interface IHostingEnvironment
    {
        string EnvironmentName { get; set; }
        
        string ContentRootPath { get; set; }
    }
}