using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FcmController : ControllerBase
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly string _fcmEndpoint = "https://fcm.googleapis.com/fcm/send";
        private static readonly string _serverKey = "severkeyfirebase"; // Replace with your FCM server key

        [HttpPost]
        public async Task<IActionResult> SendNotification()
        {
            string jsonPayload = @"
            {
                ""to"": ""FCM Token of device"",
                ""notification"": {
                    ""body"": ""New announcement assigned"",
                    ""OrganizationId"": ""2"",
                    ""content_available"": true,
                    ""priority"": ""high"",
                    ""subtitle"": ""Elementary School"",
                    ""title"": ""hello"",
                    ""image"": ""imagelink""
                },
                ""data"": {
                    ""priority"": ""high"",
                    ""sound"": ""app_sound.wav"",
                    ""content_available"": true,
                    ""bodyText"": ""New assigned"",
                    ""organization"": ""Elementary school"",
                    ""image"": ""imagelink""
                }
            }";

            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"key={_serverKey}");

            var response = await _httpClient.PostAsync(_fcmEndpoint, content);

            if (response.IsSuccessStatusCode)
            {
                return Ok("FCM notification sent successfully.");
            }

            return BadRequest("Failed to send FCM notification.");
        }
    }
}
