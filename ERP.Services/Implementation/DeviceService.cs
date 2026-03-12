//-----------------------------------------------------------------------
// <copyright file="DeviceService.cs" company="Aepistle">
//     Aepistle copy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Services.Implementation
{
    using System.Threading.Tasks;
    using ERP.Services.Interfaces;
    using zkemkeeper;

    /// <summary>
    /// Email service for sending and receiving email
    /// </summary>
    public class DeviceService : IDeviceService
    {
        /// <summary>
        /// The Zkt settings
        /// </summary>
        private readonly CZKEM CtrlBioComm;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceService"/> class.
        /// </summary>
        /// <param name="ZktSetting">The Zkt setting.</param>
        public DeviceService(CZKEM CtrlBioComm)
        {
            this.CtrlBioComm = CtrlBioComm;
        }

        /// <summary>
        /// Ping the Device asynchronous.
        /// </summary>
        /// <param name="IpAdress">The IpAdress.</param>
        /// <param name="Port">The Port.</param>
        /// <returns>return string value</returns>
        public async Task<bool> PingDevice(string IpAdress,int Port)
        {
            return CtrlBioComm.Connect_Net(IpAdress, Port);
        }
    }
}