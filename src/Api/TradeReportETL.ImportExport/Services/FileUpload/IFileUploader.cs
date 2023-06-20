using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TradeReportETL.ImportExport.Services.FileUpload
{
    public interface IFileUploader
    {
        Task<string> UploadMultipartFile(HttpRequest request, CancellationToken cancellationToken);
    }
}