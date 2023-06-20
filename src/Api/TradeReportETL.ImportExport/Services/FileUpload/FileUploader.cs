using FluentStorage.Blobs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TradeReportETL.Pipeline.Modules.Extract.Services;

namespace TradeReportETL.ImportExport.Services.FileUpload
{
    public class FileUploader : IFileUploader
    {
        private readonly ILogger<FileUploader> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _workingPath;

        public FileUploader(
            ILogger<FileUploader> logger,
            IConfiguration configuration,
            IWebHostEnvironment hostingEnvironment
        )
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<string> UploadMultipartFile(HttpRequest request, CancellationToken cancellationToken)
        {

            if (!request.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
                string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
            {
                throw new ArgumentException("UnsupportedMediaType");
            }

            var reader = new MultipartReader(mediaTypeHeader.Boundary.Value, request.Body);
            var section = await reader.ReadNextSectionAsync(cancellationToken);

            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
                    out var contentDisposition);

                if (hasContentDispositionHeader && contentDisposition.DispositionType.Equals("form-data") &&
                    !string.IsNullOrEmpty(contentDisposition.FileName.Value))
                {

                    var fileName = Path.GetRandomFileName();

                    try
                    {

                        IBlobStorage storage = FluentStorageHelpers.CreateBlobStorage(
                            _configuration.GetValue<string>("Storage:ConnectionString"));

                        await storage.WriteAsync(fileName, section.Body);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(
                            $"Cannot write to BlobStorage with fileName in {fileName}. Make sure the Blob Storage ConnectionString is set correctly in appSettings.", e);
                    }

                    _logger.LogInformation("File uploaded to {fileName} ...", fileName);

                    return fileName;
                }

                section = await reader.ReadNextSectionAsync(cancellationToken);
            }

            throw new ArgumentException("No files in the request");
        }
    }
}