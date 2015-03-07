﻿using System.Collections.Generic;
using System.Threading.Tasks;
using SpotifySharp;

namespace loadify.Spotify
{
    public class PlaylistCollection
    {
        public PlaylistContainer UnmanagedPlaylistContainer { get; set; }

        public PlaylistCollection(PlaylistContainer playlistContainer)
        {
            UnmanagedPlaylistContainer = playlistContainer;
        }

        
        public async Task<IEnumerable<Playlist>> GetPlaylists()
        {
            var playlists = new List<Playlist>();
            if (UnmanagedPlaylistContainer == null) return playlists;
            await SpotifyObject.WaitForInitialization(UnmanagedPlaylistContainer.IsLoaded);

            for (var i = 0; i < UnmanagedPlaylistContainer.NumPlaylists(); i++)
            {
                var unmanagedPlaylist = UnmanagedPlaylistContainer.Playlist(i);
                if (unmanagedPlaylist == null) continue;
                await SpotifyObject.WaitForInitialization(unmanagedPlaylist.IsLoaded);
                playlists.Add(unmanagedPlaylist);
            }

            return playlists;
        }
    }
}
