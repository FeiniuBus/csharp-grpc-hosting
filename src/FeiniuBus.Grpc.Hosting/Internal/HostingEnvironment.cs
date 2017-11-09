namespace FeiniuBus.Grpc.Hosting.Internal
{
    public class HostingEnvironment : IHostingEnvironment
    {
        public string EnvironmentName { get; set; } = Hosting.EnvironmentName.Production;
        public string ContentRootPath { get; set; }
    }
}