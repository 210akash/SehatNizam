//-----------------------------------------------------------------------
// <copyright file="AtendanceService.cs" company="Aepistle">
//     Aepistle copy right.
// </copyright>
//-----------------------------------------------------------------------

namespace ERP.Services.Implementation
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using ERP.Services.Interfaces;
    using Microsoft.Extensions.Configuration;
    using ERP.Core.Provider;
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Net.Http.Headers;

    /// <summary>
    /// Atendanceentication and Atendanceorization service
    /// </summary>
    public class AttendanceService : IAttendanceService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl;
        private readonly string _securityToken;

        /// <summary>
        /// The session provider
        /// </summary>
        private readonly SessionProvider sessionProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtendanceService"/> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="config">The configuration.</param>
        /// <param name="emailService">The email service.</param>
        /// <param name="smsService">The SMS service.</param>
        public AttendanceService(IConfiguration configuration, SessionProvider sessionProvider)
        {
            _httpClient = new();
            _apiUrl = configuration["Api:Attendance:Address"];  // API URL from appsettings.json
            _securityToken = configuration["Api:Attendance:Key"];  // API key from appsettings.json
            this.sessionProvider = sessionProvider;
        }


        public async Task<List<Tuple<long, string>>> SyncAttendanceByDate(DateTime fromDate, DateTime toDate)
        {
            try
            {
                // Get current datetime and format it
                string currentDateTime = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

                // Concatenate token with current datetime
                string tokenWithDateTime = $"{_securityToken}:{currentDateTime}";

                // Base64 encode the concatenated token and datetime
                string encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenWithDateTime));

                // Construct the request URL for SyncAttendanceByEmployee
                var requestUrl = $"{_apiUrl}/Attendance/SyncAttendanceByDate?FromDate={fromDate:yyyy-MM-dd}&ToDate={toDate:yyyy-MM-dd}";

                // Create the request
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("ApiKey", encodedToken);
                // Add CurrentUserId as a custom header
                request.Headers.Add("CurrentUserId", sessionProvider.Session.LoggedInUserId.ToString());
                // Send the request
                var response = await _httpClient.SendAsync(request);

                // Check if the response is successful
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    // Deserialize response content to a List of Tuples
                    var result = JsonConvert.DeserializeObject<List<Tuple<long, string>>>(responseContent);
                    return result;
                }
                else
                {
                    // Handle unsuccessful status codes
                    throw new Exception($"Error: {response.StatusCode}. {await response.Content.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {

                // Optionally, log the exception or handle it as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Sync Attendance By Device.
        /// </summary>
        /// <param name="DeviceId">The DeviceId.</param>
        /// <returns>return the long</returns>
        /// 
        public async Task<List<Tuple<long, string>>> SyncAttendanceByEmployeeAsync(string employeeId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                // Get current datetime and format it
                string currentDateTime = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

                // Concatenate token with current datetime
                string tokenWithDateTime = $"{_securityToken}:{currentDateTime}";

                // Base64 encode the concatenated token and datetime
                string encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(tokenWithDateTime));

                // Construct the request URL for SyncAttendanceByEmployee
                var requestUrl = $"{_apiUrl}/Attendance/SyncAttendanceByEmployee?EmployeeId={employeeId}&FromDate={fromDate:yyyy-MM-dd}&ToDate={toDate:yyyy-MM-dd}";

                // Create the request
                var request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("ApiKey", encodedToken);
                // Add CurrentUserId as a custom header
                request.Headers.Add("CurrentUserId", sessionProvider.Session.LoggedInUserId.ToString());
                // Send the request
                var response = await _httpClient.SendAsync(request);

                // Check if the response is successful
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    // Deserialize response content to a List of Tuples
                    var result = JsonConvert.DeserializeObject<List<Tuple<long, string>>>(responseContent);
                    return result;
                }
                else
                {
                    // Handle unsuccessful status codes
                    throw new Exception($"Error: {response.StatusCode}. {await response.Content.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {
                // Optionally, log the exception or handle it as needed
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
        }
    }
}