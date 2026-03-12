//-----------------------------------------------------------------------
// <copyright file="ISmsService.cs" company="Aepistle">
//     Aepistle copy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Services.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    ///  Email Service Interface
    /// </summary>
    public interface IDeviceService
    {
        /// <summary>
        /// Ping the Device asynchronous.
        /// </summary>
        /// <param name="IpAdress">The IpAdress.</param>
        /// <param name="Port">The Port.</param>
        /// <returns>return string value</returns>
        Task<bool> PingDevice(string IpAdress, int Port);
    }
}
