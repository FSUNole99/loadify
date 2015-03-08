using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SpotifySharp;

namespace loadify.Spotify
{
    public class LoadifySession : SpotifySessionListener
    {
        private SpotifySession _Session { get; set; }
        private PlaylistContainer _PlaylistContainer { get; set; }

        private SynchronizationContext _Synchronization { get; set; }
        private TrackDownloadService _TrackDownloadService { get; set; }

        private Action<SpotifyError> _LoggedInCallback = error => { };    

        public bool Connected
        {
            get
            {
                if (_Session == null) return false;
                return (_Session.Connectionstate() == ConnectionState.LoggedIn);
            }
        }

        public LoadifySession()
        {
            Setup();
        }

        ~LoadifySession()
        {
            Release();
        }

        public void Release()
        {
            if (_Session == null) return;
            _Session.Logout();


            _PlaylistContainer.Release();
        }

        private void Setup()
        {
            var cachePath = Properties.Settings.Default.CacheDirectory;
            if (!Directory.Exists(cachePath))
                Directory.CreateDirectory(cachePath);

            var config = new SpotifySessionConfig()
            {
                ApiVersion = 12,
                CacheLocation = cachePath,
                SettingsLocation = cachePath,
                ApplicationKey = Properties.Resources.spotify_appkey,
                UserAgent = "Loadify",
                Listener = this            
            };

            _Synchronization = SynchronizationContext.Current;
            _Session = SpotifySession.Create(config);
        }

        public void Login(string username, string password, Action<SpotifyError> callback)
        {
            if (Connected) return;
            _LoggedInCallback = callback;
            _Session.Login(username, password, false, null);
        }

        private async Task LoadPlaylistContainer()
        {
            _PlaylistContainer = _Session.Playlistcontainer();
            if (_PlaylistContainer == null) throw new SpotifyException(SpotifyError.SystemFailure, "Playlist container could not be retrieved from the library");
            await SpotifyObject.WaitForInitialization(_PlaylistContainer.IsLoaded);
        }

        public void AddPlaylist(Playlist playlist)
        {
            if (_PlaylistContainer == null) throw new ResourceException("The playlist container wasn't retrieved yet");
            _PlaylistContainer.AddPlaylist(Link.CreateFromPlaylist(playlist));
        }

        public async Task RemovePlaylist(Playlist playlist)
        {
            if (_PlaylistContainer == null) throw new ResourceException("The playlist container wasn't retrieved yet");

            for (var i = 0; i < _PlaylistContainer.NumPlaylists(); i++)
            {
                var unmanagedPlaylist = _PlaylistContainer.Playlist(i);
                await SpotifyObject.WaitForInitialization(unmanagedPlaylist.IsLoaded);

                if (unmanagedPlaylist.Name() == playlist.Name())
                {
                    _PlaylistContainer.RemovePlaylist(i);
                    break;
                }
            }
        }

        public async Task<IEnumerable<Playlist>> GetPlaylists()
        {
            if (_PlaylistContainer == null) throw new ResourceException("The playlist container wasn't retrieved yet");
            var playlists = new List<Playlist>();

            for (var i = 0; i < _PlaylistContainer.NumPlaylists(); i++)
            {
                var unmanagedPlaylist = _PlaylistContainer.Playlist(i);
                if (unmanagedPlaylist == null) continue;
                await SpotifyObject.WaitForInitialization(unmanagedPlaylist.IsLoaded);
                playlists.Add(unmanagedPlaylist);
            }

            return playlists;
        }

        public Image GetImage(ImageId imageId)
        {
            return Image.Create(_Session, imageId);
        }

        public async Task DownloadTrack(TrackDownloadService trackDownloadService, CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                try
                {
                    _TrackDownloadService = trackDownloadService;
                    _TrackDownloadService.Start();

                    _Session.PlayerLoad(trackDownloadService.Track);
                    _Session.PlayerPlay(true);

                    while (true)
                    {
                        if (!_TrackDownloadService.Active)
                            return;

                        if (cancellationToken.IsCancellationRequested)
                        {
                            _TrackDownloadService.Cancel(TrackDownloadService.CancellationReason.UserInteraction);
                            return;
                        }
                    }
                }
                catch (Exception)
                {
                    _TrackDownloadService.Cancel(TrackDownloadService.CancellationReason.Unknown);
                    throw;
                }
            }, cancellationToken);
        }

        public Playlist GetPlaylist(Link link)
        {
            if (link == null) throw new ArgumentException("The link object can't be null");

            var unmanagedPlaylist = Playlist.Create(_Session, link);
            return unmanagedPlaylist;
        }

        public Playlist GetPlaylist(string url)
        {
            var link = Link.CreateFromString(url);
            if (link == null) throw new InvalidSpotifyUrlException(url);
            return GetPlaylist(link);
        }

        public Track GetTrack(string url)
        {
            var link = Link.CreateFromString(url);
            if (link == null) throw new InvalidSpotifyUrlException(url);

            var track = link.AsTrack();
            if (track == null) throw new InvalidSpotifyUrlException(url);

            return track;
        }

        private void InvokeProcessEvents()
        {
            _Synchronization.Post(state => ProcessEvents(), null);
        }

        void ProcessEvents()
        {
            var timeout = 0;
            while (timeout == 0)
                _Session.ProcessEvents(ref timeout);
        }

        public override void NotifyMainThread(SpotifySession session)
        {
            InvokeProcessEvents();
            base.NotifyMainThread(session);
        }

        public override async void LoggedIn(SpotifySession session, SpotifyError error)
        {
            if (error == SpotifyError.Ok)
            {
                await SpotifyObject.WaitForInitialization(session.User().IsLoaded);
                _Session.PreferredBitrate(BitRate._320k);
                await LoadPlaylistContainer();
            }

            _LoggedInCallback(error);
        }

        public override int MusicDelivery(SpotifySession session, AudioFormat format, IntPtr frames, int num_frames)
        {
            if (num_frames == 0) return num_frames;

            if (_TrackDownloadService != null)
            {
                if (_TrackDownloadService.Active)
                    _TrackDownloadService.ProcessInput(format, frames, num_frames);
            }

            return num_frames;
        }

        public override void PlayTokenLost(SpotifySession session)
        {
            if (_TrackDownloadService != null)
            {
                if (_TrackDownloadService.Active)
                    _TrackDownloadService.Cancel(TrackDownloadService.CancellationReason.PlayTokenLost);
            }
        }

        public override void EndOfTrack(SpotifySession session)
        {
            _Session.PlayerPlay(false);
            _TrackDownloadService.Stop();
        }
    }
}
