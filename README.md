# Spotify ISRC search

## Running locally
Needs Azure function emulator. For info on how to develop using VS Code:

https://docs.microsoft.com/en-us/azure/azure-functions/functions-develop-vs-code?tabs=csharp

### Steps

1. Start function emulator:
`func start`

2. Navigate to: http://localhost:7071/api/SpotifyIsrcSearch?id={spotifyAlbumId}&type={json|html}

## Examples

http://localhost:7071/api/SpotifyIsrcSearch?id=3I3PmRvn5iFY8i6zzvEcci

http://localhost:7071/api/SpotifyIsrcSearch?id=3I3PmRvn5iFY8i6zzvEcci&type=json

where `3I3PmRvn5iFY8i6zzvEcci` is the spotify album id.

## How to get Spotify album id
You can get spotify album id in the spotify app by right clicking album's artwork and then click `Share` and then `Copy Spotify URI`
