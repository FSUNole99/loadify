using System.Threading.Tasks;
using loadify.Spotify;
using SpotifySharp;

namespace loadify.Model
{
    public class ArtistModel : SpotifyObjectModel
    {
        public string Name { get; set; }

        public static async Task<ArtistModel> FromLibrary(Artist unmanagedArtist)
        {
            var artistModel = new ArtistModel();
            if (unmanagedArtist == null) return artistModel;
            await SpotifyObject.WaitForInitialization(unmanagedArtist.IsLoaded);

            artistModel.Name = unmanagedArtist.Name();

            var artistLink = SpotifySharp.Link.CreateFromArtist(unmanagedArtist);
            artistModel.Link = artistLink.AsString();
            artistLink.Release();

            return artistModel;
        }
    }
}
