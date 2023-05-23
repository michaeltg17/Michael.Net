using ChinhDo.Transactions;
using Michael.Net.Persistence;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace Michael.Net.Persistence.AspNetCore
{
    public class AspNetCoreStorage : IObjectStorage
    {
        readonly string path;
        readonly string requestPath;
        readonly IServerAddressesFeature serverAddressesFeature;
        readonly IFileManager fileManager;

        public AspNetCoreStorage(
            string path,
            string requestPath,
            IServerAddressesFeature serverAddressesFeature,
            IFileManager fileManager)
        {
            this.path = path;
            this.requestPath = requestPath;
            this.serverAddressesFeature = serverAddressesFeature;
            this.fileManager = fileManager;
        }

        public Task Delete(string fullFileName)
        {
            throw new NotImplementedException();
        }

        public Task<Stream?> Get(string fullFileName)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> GetOrThrow(string fullFileName)
        {
            throw new NotImplementedException();
        }

        public Task<string> Upload(string fullFileName, Stream stream)
        {
            var filePath = Path.Combine(path, fullFileName);

            var temporalFilePath = fileManager.CreateTempFileName();
            using (var fs = new FileStream(temporalFilePath, FileMode.Create))
            {
                stream.CopyTo(fs);
            }

            fileManager.Copy(temporalFilePath, filePath, true);

            var baseUrl = serverAddressesFeature.Addresses.Single(url => url.Contains("https"));
            var url = $"{baseUrl}{requestPath}/{fullFileName}";

            return Task.FromResult(url);
        }
    }
}
