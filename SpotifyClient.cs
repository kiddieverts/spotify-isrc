using Newtonsoft.Json;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyIsrcSearch
{
    public class SpotifyClient
    {
        private readonly SpotifyAuthClient authClient;
        public SpotifyClient(SpotifyAuthClient authClient)
        {
            this.authClient = authClient;
        }

        public async Task<List<FullTrack>> GetAlbumTracks(string albumId)
        {
            albumId = albumId.Replace("spotify:album:", "").Trim();

            var token = authClient.Authorize();
            var spotify = new SpotifyAPI.Web.SpotifyClient(token);

            var tmpTracks = await spotify.Albums.GetTracks(albumId, new AlbumTracksRequest
            {
                Limit = 50
            });

            var trackIds = tmpTracks.Items.Select(r => r.Id).ToList();

            var tracks = new List<FullTrack>();
            foreach (var trackId in trackIds)
            {
                var track = await spotify.Tracks.Get(trackId);
                tracks.Add(track);
            }

            return tracks.OrderBy(r => r.TrackNumber).ToList();
        }
    }

    public class SpotifyAuthClient
    {
        private string _accessToken;
        private DateTime _timeStamp;

        public SpotifyAuthClient() { }

        private static string _Base64Encode(string text)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Convert.ToBase64String(bytes);
        }

        public string Authorize()
        {
            var now = DateTime.Now;
            var then = _timeStamp;
            var diff = now - then;
            var refresh = diff > TimeSpan.FromMinutes(50);

            if (string.IsNullOrWhiteSpace(_accessToken) || refresh == true)
            {
                var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
                var secret = Environment.GetEnvironmentVariable("SPOTIFY_SECRET");

                var base64string = _Base64Encode($"{clientId}:{secret}");

                var _httpClient = new HttpClient();

                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {base64string}");

                var data = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = _httpClient.PostAsync("https://accounts.spotify.com/api/token", data).GetAwaiter().GetResult();

                var ressponseString = response.Content.ReadAsStringAsync().Result;

                var obj = JsonConvert.DeserializeObject<dynamic>(ressponseString);

                var accessToken = obj["access_token"];

                _accessToken = accessToken;
                _timeStamp = DateTime.Now;
                return _accessToken;
            }
            else
            {
                return _accessToken;
            }
        }
    }
}
