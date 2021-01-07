using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SpotifyAPI.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifyIsrcSearch
{
    public class SpotifyIsrcSearch
    {
        private readonly SpotifyAuthClient authClient;

        public SpotifyIsrcSearch(SpotifyAuthClient authClient)
        {
            this.authClient = authClient;
        }

        [FunctionName("SpotifyIsrcSearch")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            string albumId = req.Query["id"];
            string type = req.Query["type"] == "json" ? "json" : "html";

            if (string.IsNullOrWhiteSpace(albumId))
            {
                return ShowError();
            }

            var client = new SpotifyClient(authClient);
            var result = await client.GetAlbumTracks(albumId);
            var tracks = result.Select(track => MapToDto(track));

            if (type == "json")
            {
                return new OkObjectResult(tracks);
            }
            else
            {
                return ShowHtmlTracks(tracks);
            }
        }

        private static TrackDto MapToDto(FullTrack track) =>
            new TrackDto
            {
                TrackNumber = track.TrackNumber,
                Name = track.Name,
                Isrc = track.ExternalIds["isrc"]
            };

        private static ContentResult ShowHtmlTracks(IEnumerable<TrackDto> tracks)
        {
            var content = "<html><head><meta charset='utf-8'></head><body><h1 style=\"font-family: 'Arial', 'Helvetica'\"></head><body>";
            content += "<table style=\"min-width: 800px;\"><tr><td><b>Númer</b></td><td><b>Nafn lags</b></td><td><b>ISRC</b></td></tr>";

            foreach (var track in tracks)
            {
                content += $"<tr><td>{track.TrackNumber}</td><td>{track.Name}</td><td>{track.Isrc}</td></tr>";
            }

            content += "</table></body></html>";

            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = 200,
                Content = content
            };
        }

        private static IActionResult ShowError() =>
            new ContentResult
            {
                ContentType = "text/html",
                StatusCode = 500,
                Content = "Vantar spotify album id"
            };
    }
}
