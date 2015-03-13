using System;
using System.Threading.Tasks;
using loadify.Spotify;
using SpotifySharp;

namespace loadify.Model
{
    public class AlbumModel : SpotifyObjectModel
    {
        public string Name { get; set; }
        public int ReleaseYear { get; set; }
        public AlbumType AlbumType { get; set; }
        public ImageId CoverImageID { get; set; }

        public AlbumModel()
        { }

        public static async Task<AlbumModel> FromLibrary(Album unmanagedAlbum)
        {
            var albumModel = new AlbumModel();
            if (unmanagedAlbum == null) return albumModel;
            await SpotifyObject.WaitForInitialization(unmanagedAlbum.IsLoaded);

            albumModel.Name = unmanagedAlbum.Name();
            albumModel.ReleaseYear = unmanagedAlbum.Year();
            albumModel.AlbumType = unmanagedAlbum.Type();
            albumModel.CoverImageID = unmanagedAlbum.Cover(ImageSize.Large);

            var albumLink = SpotifySharp.Link.CreateFromAlbum(unmanagedAlbum);
            albumModel.Link = albumLink.AsString();
            albumLink.Release();

            /*
            try
            {
                // retrieve the cover image of the album...
                var coverImage = session.GetImage(unmanagedAlbum.Cover(ImageSize.Large));
                await SpotifyObject.WaitForInitialization(coverImage.IsLoaded);
                albumModel.Cover = coverImage.Data();
            }
            catch (AccessViolationException)
            {
                // nasty work-around - swallow if the cover image could not be retrieved
                // since the ImageId class does not expose a property or function for checking if the buffer/handle is null/0
            }*/
            
            return albumModel;
        }
    }
}
