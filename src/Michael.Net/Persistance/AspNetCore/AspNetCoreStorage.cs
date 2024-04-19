using ChinhDo.Transactions;

namespace Michael.Net.Persistence.AspNetCore
{
    public class AspNetCoreStorage : IObjectStorage
    {
        readonly string path;
        readonly string url;
        readonly IFileManager fileManager;

        public AspNetCoreStorage(string path, string url, IFileManager fileManager)
        {
            this.path = path;
            this.url = url;
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

            var fileUrl = $"{url}/{fullFileName}";

            return Task.FromResult(fileUrl);
        }
    }
}
