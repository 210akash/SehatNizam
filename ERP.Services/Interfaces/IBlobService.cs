////-----------------------------------------------------------------
////<copyright file="IBlobService.cs" company="Transfer">
////Transfer copy right.
//// </copyright>
////-----------------------------------------------------------------------

namespace ERP.Services.Interfaces
{
    using System.IO;
    using System.Threading.Tasks;
    using ERP.BusinessModels.ParameterVM;
    using ERP.BusinessModels.ResponseVM;

    /// <summary>
    /// Blob Storage Interface
    /// </summary>
    public interface IBlobService
    {
        /// <summary>
        /// User Login
        /// </summary>
        /// <param name="fileUrl">Login detail</param>
        /// <returns>boolean response</returns>
        Task<bool> DeleteBlobDataAsync(string fileUrl);

        /// <summary>
        /// Uploads the file to BLOB asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Return the file url</returns>
        Task<string> UploadFileToBlobAsync(BlobFileUploadModel model);

        /// <summary>
        /// Uploads the base64 file to BLOB asynchronous.
        /// </summary>
        /// <param name="blobModel">The BLOB model.</param>
        /// <returns>Return the file url</returns>
        Task<string> UploadBase64FileToBlobAsync(BlobImageUploadModel blobModel, string extension = "");

        /// <summary>
        /// Downloads the file asynchronous.
        /// </summary>
        /// <param name="fileUrl">The file URL.</param>
        /// <returns>the file stream</returns>
        Task<FileDownloadResponse> DownloadFileAsync(string fileUrl);

        /// <summary>
        /// Downloads the file asynchronous.
        /// </summary>
        /// <param name="Id">Id.</param>
        /// <returns>the file stream</returns>
        Task<FileDownloadResponse> AuthorityPrint(long Id);

        /// <summary>
        /// Uploads the file to Local asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>Return the file url</returns>
        Task<string> UploadFileToLocalAsync(BlobFileUploadModel model);

        /// <summary>
        /// Get the file asynchronous.
        /// </summary>
        /// <param name="fileUrl">The file URL.</param>
        /// <param name="FolderName">The file Folder.</param>
        /// <returns>the file</returns>
        Task<string> ConvertUrlToFile(string fileUrl, string FolderName);
    }
}
