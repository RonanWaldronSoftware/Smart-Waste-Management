using Amazon.S3;
using Amazon.S3.Model;
using SmartWaste.Model.Configuration;
using SmartWaste.Model.Extensions;
using SmartWaste.Service.Contracts;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;

namespace SmartWaste.Service.Implementations
{
    public class AWS3FileService : IFileService
    {
        private readonly ILogger<AWS3FileService> _logger;
        private readonly AWSS3Configuration _config;
        private readonly AmazonS3Client _s3Client;

        public AWS3FileService(ILogger<AWS3FileService> logger, IOptions<Configuration> config)
        {
            _logger = logger;
            _config = config.Value.AWSS3Configuration;

            AmazonS3Config s3Config = new();
            s3Config.ServiceURL = _config.ServiceURL;
            s3Config.ForcePathStyle = true;

            _s3Client = new AmazonS3Client(_config.AccessKeyId, _config.SecretAccessKey, s3Config);
        }

        public async Task<string> UploadFile<T>(T file, string fileName, string mainFolder, string? subFolder = null)
        {
            string key = subFolder is null ? $"{mainFolder}/{fileName}" : $"{mainFolder}/{subFolder}/{fileName}";

            // Get content type and input stream
            (string contentType, Stream inputStream) = GetContentTypeAndStream(file, fileName);

            try
            {
                var request = new PutObjectRequest
                {
                    BucketName = _config.BucketName,
                    Key = key,
                    ContentType = contentType,
                    InputStream = inputStream,
                    CannedACL = S3CannedACL.PublicRead
                };

                var response = await _s3Client.PutObjectAsync(request);

                if (response.HttpStatusCode != HttpStatusCode.OK)
                    _logger.LogError($"Error while upload on s3 storage");

                return $"{_config.ServiceURL}/{_config.InstanceCode}:{_config.BucketName}/{key}";
            }
            catch(Exception ex)
            {
                _logger.LogError($"Exception during file upload {ex.Message}");
                return "";
            }
            finally
            {
                // Dispose stream
                inputStream.Dispose();
            }
        }

        /// <summary>
        /// Get content type and stream based on type of T file
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="file"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private (string contentType, Stream inputStream) GetContentTypeAndStream<T>(T file, string fileName)
        {
            string contentType;
            Stream inputStream;

            if (typeof(T) == typeof(IBrowserFile))
            {
                var browserFile = (IBrowserFile)(object)file;
                contentType = browserFile.ContentType;
                inputStream = browserFile.OpenReadStream(10485760);
            }
            else if (typeof(T) == typeof(byte[]))
            {
                var byteArray = (byte[])(object)file;
                contentType = fileName.GetContentType();
                inputStream = new MemoryStream(byteArray);
            }
            else if (typeof(T) == typeof(IFormFile))
            {
                var formFile = (IFormFile)(object)file;
                contentType = formFile.ContentType;
                var memoryStream = new MemoryStream();
                formFile.CopyTo(memoryStream);
                memoryStream.Position = 0;
                inputStream = memoryStream;

            }
            else
            {
                throw new ArgumentException("Unsupported file type");
            }

            return (contentType, inputStream);
        }
    }
}
