using Microsoft.Extensions.Configuration;

namespace FeiniuBus.Grpc.Hosting
{
    public class GrpcHostBuilderContext
    {
        /// <summary>
        /// 
        /// </summary>
        public IHostingEnvironment HostingEnvironment { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; set; }
    }
}