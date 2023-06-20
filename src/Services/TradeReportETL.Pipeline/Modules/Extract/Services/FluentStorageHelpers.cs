using FluentFTP;
using FluentStorage;
using FluentStorage.Blobs;
using FluentStorage.ConnectionString;
using System.Net;

namespace TradeReportETL.Pipeline.Modules.Extract.Services
{
    public static class FluentStorageHelpers
    {

        /// <summary>
        /// Making sure to use PASSIVE mode when creating FTP connection to make it compatible with docker
        /// </summary>
        public static IBlobStorage CreateBlobStorage(string connectionString)
        {
            var storageConnectionString = new StorageConnectionString(connectionString);
            IBlobStorage storage;
            if (storageConnectionString.Prefix == "ftp")
            {
                storageConnectionString.GetRequired("host", true, out string host);
                storageConnectionString.GetRequired("user", true, out string user);
                storageConnectionString.GetRequired("password", true, out string password);

                storage = StorageFactory.Blobs.Ftp(host, new NetworkCredential(user, password), FtpDataConnectionType.AutoPassive);
            }
            else
            {
                storage = StorageFactory.Blobs.FromConnectionString(connectionString);
            }

            return storage;
        }
    }
}