using System;
using System.Threading.Tasks;
using System.Net.Http;

namespace Camera
{
    public class FaceAccessClient
    {
        private HttpClient _client;

        public FaceAccessClient()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri("http://localhost:5000/api/");
        }

        public async Task<CheckResponse> CheckAssess(byte[] jpegAsByteArray)
        {
            var content = new ByteArrayContent(jpegAsByteArray);
            var result = await _client.PostAsync("check", content);
            var json = System.Json.JsonValue.Parse(result.Content.ToString());
            var name = json["Name"];
            var access = bool.Parse(json["Access"]);
            return new CheckResponse
            {
                Access = access,
                Name = name
            };
        }

    }

    public class CheckResponse
    {
        public bool Access { get; set; }
        public Guid PersonId { get; set; }
        public double Confidence { get; set; }
        public string Name { get; set; }
    }
}
