//-----------------------------------------------------------------------
// <copyright file="BlobService.cs" company="Transfer">
//     Transfer copy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Services.Implementation
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using ERP.BusinessModels.ParameterVM;
    using ERP.BusinessModels.ResponseVM;
    using Microsoft.Extensions.Configuration;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.AspNetCore.Http;
    using EllipticCurve.Utils;
    using System.Text;
    using static System.Net.Mime.MediaTypeNames;
    using System.Net.Mime;
    using System.Buffers.Text;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json.Linq;
    using System.Linq.Expressions;
    using File = System.IO.File;
    using ERP.Services.Interfaces;

    /// <summary>
    /// Blob Storage
    /// </summary>
    public class BlobService : IBlobService
    {
        /// <summary>
        /// Gets Values from AppSettings
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// Cloud Blob Client reference
        /// </summary>
        private readonly CloudBlobClient client;

        /// <summary>
        /// Cloud Blob Container reference
        /// </summary>
        private readonly CloudBlobContainer container;

        /// <summary>
        /// Cloud Blob Container reference
        /// </summary>
        private readonly string Localcontainer;

        /// <summary>
        /// Cloud Blob Container reference
        /// </summary>
        private readonly string DeleteLocalcontainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobService"/> class
        /// </summary>
        /// <param name="configuration">Configuration setting</param>
        public BlobService(IConfiguration configuration)
        {
            this.configuration = configuration;
            var account = CloudStorageAccount.Parse(this.configuration["AzureBlob:BlobConnectionString"]);
            client = account.CreateCloudBlobClient();
            container = client.GetContainerReference(this.configuration["AzureBlob:BlobContainerName"]);
            Localcontainer = this.configuration["LocalBlob:BlobContainerName"];
            DeleteLocalcontainer = this.configuration["LocalBlob:DeleteBlobContainerName"];
        }

        /// <summary>
        /// Delete File from Blob
        /// </summary>
        /// <param name="fileUrl">file Url</param>
        /// <returns>boolean response</returns>
        public async Task<bool> DeleteBlobDataAsync(string fileUrl)
        {
            Uri uriObj = new Uri(fileUrl);
            var fileContainerName = "\\" + configuration["AzureBlob:BlobContainerName"] + "\\";
            string folderName = Path.GetDirectoryName(uriObj.LocalPath).Replace(fileContainerName, string.Empty);
            string blobName = Path.GetFileName(uriObj.LocalPath);
            string fullBlobName = string.Format("{0}\\{1}", folderName, blobName);

            // get block blob refarence  
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fullBlobName);

            ///// delete blob from container        
            if (await blockBlob.ExistsAsync())
            {
                return await blockBlob.DeleteIfExistsAsync();
            }

            return false;
        }

        /// <summary>
        /// Uploads the base64 file to BLOB asynchronous.
        /// </summary>
        /// <param name="blobModel">The BLOB model.</param>
        /// <returns>Return the file url</returns>
        //public async Task<string> UploadBase64FileToBlobAsync(BlobImageUploadModel blobModel, string extension = "")
        //{
        //    if (!string.IsNullOrEmpty(blobModel.File))
        //    {

        //        string convert = blobModel.File.Split("base64,")[1];
        //        byte[] imageBytes = Convert.FromBase64String(convert);

        //        if (imageBytes.Length > 0)
        //        {
        //            string FileExtension = blobModel.File.Split(';')[0].Split('/')[1]; 
        //            string fileName = this.GenerateFileName(Path.GetFileName("." + FileExtension));
        //            string filePath = Path.Combine(Localcontainer + blobModel.FolderName, fileName); 
        //            using (var imageFile = new FileStream(filePath, FileMode.Create))
        //            {
        //                await imageFile.WriteAsync(imageBytes, 0, imageBytes.Length);
        //                imageFile.Flush();
        //                return fileName;
        //            }
        //        }
        //    }

        //    return string.Empty;
        //}
        public async Task<string> UploadBase64FileToBlobAsync(BlobImageUploadModel blobModel, string extension = "")
        {
            if (!string.IsNullOrEmpty(blobModel.File))
            {

                string convert = blobModel.File.Split("base64,")[1];
                byte[] imageBytes = Convert.FromBase64String(convert);

                if (imageBytes.Length > 0)
                {
                    string FileExtension;
                    if (extension != "")
                    {
                        FileExtension = extension;
                    }
                    else
                    {
                        FileExtension = blobModel.File.Split(';')[0].Split('/')[1];
                    }
                    string fileName = GenerateFileName(Path.GetFileName("." + FileExtension));
                    string filePath = Path.Combine(Localcontainer + blobModel.FolderName, fileName);
                    using (var imageFile = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.WriteAsync(imageBytes, 0, imageBytes.Length);
                        imageFile.Flush();
                        return fileName;
                    }
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Uploads the base64 file to BLOB asynchronous.
        /// </summary>
        /// <param name="blobModel">The BLOB model.</param>
        /// <returns>Return the file url</returns>
        public async Task<string> UploadFileToBlobAsync(BlobFileUploadModel blobModel)
        {
            blobModel.FolderName = string.IsNullOrEmpty(blobModel.FolderName) ? "default" : blobModel.FolderName.ToLower();
            var fileName = this.GenerateFileName(Path.GetFileName(blobModel.FileItem.FileName));
            byte[] fileData = new byte[blobModel.FileItem.Length];
            using (var ms = new MemoryStream())
            {
                await blobModel.FileItem.CopyToAsync(ms);
                fileData = ms.ToArray();
            }

            CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(blobModel.FolderName + "/" + fileName);
            cloudBlockBlob.Properties.ContentType = blobModel.FileItem.ContentType;
            await cloudBlockBlob.UploadFromByteArrayAsync(fileData, 0, fileData.Length);
            await cloudBlockBlob.SetPropertiesAsync();
            return cloudBlockBlob.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Downloads the file asynchronous.
        /// </summary>
        /// <param name="fileUrl">The file URL.</param>
        /// <returns>the file stream</returns>
        public async Task<FileDownloadResponse> DownloadFileAsync(string fileUrl)
        {
            var memoryStream = new MemoryStream();
            Uri uriObj = new Uri(fileUrl);
            var fileContainerName = "\\" + configuration["AzureBlob:BlobContainerName"] + "\\";
            string folderName = Path.GetDirectoryName(uriObj.LocalPath).Replace(fileContainerName, string.Empty);
            string blobName = Path.GetFileName(uriObj.LocalPath);
            string fullBlobName = string.Format("{0}\\{1}", folderName, blobName);

            // get block blob refarence  
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fullBlobName);

            var response = new FileDownloadResponse();

            if (await blockBlob.ExistsAsync())
            {
                await blockBlob.DownloadToStreamAsync(memoryStream);
                var blobStream = await blockBlob.OpenReadAsync();
                response.FileContentType = blockBlob.Properties.ContentType;
                response.FileName = blockBlob.Name;
                response.FileStream = blobStream;
            }

            return response;
        }

        /// <summary>
        /// Generate File Name
        /// </summary>
        /// <param name="fileName">file Name</param>
        /// <returns>string FileName</returns>
        private string GenerateFileName(string fileName)
        {
            string uniqueFileName = Guid.NewGuid() + fileName;
            return uniqueFileName;
        }

        /// <summary>
        /// Downloads the file asynchronous.
        /// </summary>
        /// <param name="Id">Id.</param>
        /// <returns>the file stream</returns>
        public async Task<FileDownloadResponse> AuthorityPrint(long Id)
        {

            //var memoryStream = new MemoryStream();
            //Uri uriObj = new Uri(fileUrl);
            //var fileContainerName = "\\" + this.configuration["AzureBlob:BlobContainerName"] + "\\";
            //string folderName = Path.GetDirectoryName(uriObj.LocalPath).Replace(fileContainerName, string.Empty);
            //string blobName = Path.GetFileName(uriObj.LocalPath);
            //string fullBlobName = string.Format("{0}\\{1}", folderName, blobName);

            //// get block blob refarence  
            //CloudBlockBlob blockBlob = this.container.GetBlockBlobReference(fullBlobName);

            var response = new FileDownloadResponse();

            //if (await blockBlob.ExistsAsync())
            //{
            //    await blockBlob.DownloadToStreamAsync(memoryStream);
            //    var blobStream = await blockBlob.OpenReadAsync();
            //    response.FileContentType = blockBlob.Properties.ContentType;
            //    response.FileName = blockBlob.Name;
            //    response.FileStream = blobStream;
            //}

            return response;
        }

        /// <summary>
        /// Downloads the file asynchronous.
        /// </summary>
        /// <param name="fileUrl">The file URL.</param>
        /// <returns>the file stream</returns>
        public async Task<string> ConvertUrlToFile(string fileUrl, string FolderName)
        {
            byte[] bytes;
            string filePath = Path.Combine(Localcontainer + FolderName, fileUrl);
            using (var stream = File.OpenRead(filePath))
            {
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    bytes = memoryStream.ToArray();
                }
                string base64 = Convert.ToBase64String(bytes);
                //return new MemoryStream(Encoding.UTF8.GetBytes(base64));
                return Convert.ToBase64String(bytes);
            }
        }

        public async Task<string> UploadFileToLocalAsync(BlobFileUploadModel model)
        {
            string fileName = this.GenerateFileName(Path.GetFileName(model.FileItem.FileName));
            string filePath = Path.Combine(Localcontainer + model.FolderName, fileName);
            using (Stream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.FileItem.CopyToAsync(fileStream);
            }
            return fileName;
        }

        /// <summary>
        /// To demonstrate extraction of file extension from base64 string.
        /// </summary>
        /// <param name="base64String">base64 string.</param>
        /// <returns>Henceforth file extension from string.</returns>
        public static string GetFileExtension(string base64String)
        {
            var data = base64String.Substring(0, 5);

            switch (data.ToUpper())
            {
                case "IVBOR":
                    return "png";
                case "/9J/4":
                    return "jpg";
                case "AAAAF":
                    return "mp4";
                case "JVBER":
                    return "pdf";
                case "AAABA":
                    return "ico";
                case "UMFYI":
                    return "rar";
                case "E1XYD":
                    return "rtf";
                case "U1PKC":
                    return "txt";
                case "MQOWM":
                case "77U/M":
                    return "srt";
                default:
                    return string.Empty;
            }
        }
    }
}
