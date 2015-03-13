using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using loadify.Model;
using loadify.Spotify;

namespace loadify.Configuration
{
    public class PlaylistRepositoryPathConfigurator : IDownloadPathConfigurator
    {
        public static string ValidateFileName(string fileName)
        {
            return !String.IsNullOrEmpty(fileName) ? Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), "-")) : "";
        }

        public string Configure(string basePath, string targetFileExtension, TrackModel track)
        {
            var validatedTrackName = ValidateFileName(track.Name);
            var completePath = Path.Combine(basePath, String.Format("{0}.{1}", validatedTrackName, targetFileExtension));

            var maxPathField = typeof(Path).GetField("MaxPath",
                    BindingFlags.Static |
                    BindingFlags.GetField |
                    BindingFlags.NonPublic);

            var maxPathLength = (int) maxPathField.GetValue(null);

            if (track.Playlist != null && track.Playlist.Name.Length != 0)
            {
                var playlistRepositoryDirectory = Path.Combine(basePath, ValidateFileName(track.Playlist.Name));
                if (Path.Combine(playlistRepositoryDirectory, validatedTrackName).Length <= maxPathLength)
                {
                    completePath = Path.Combine(playlistRepositoryDirectory, 
                        String.Format("{0}.{1}", ValidateFileName(track.Name), targetFileExtension));

                    try
                    {
                        if (!Directory.Exists(playlistRepositoryDirectory))
                            Directory.CreateDirectory(playlistRepositoryDirectory);
                    }
                    catch (UnauthorizedAccessException exception)
                    {
                        throw new ConfigurationException(
                            String.Format("Loadify is not authorized to create the download directory ({0})",
                                playlistRepositoryDirectory), exception);
                    }
                    catch (Exception exception)
                    {
                        throw new ConfigurationException("An unhandled configuration error occured", exception);
                    }
                }
            }

            if(completePath.Length > maxPathLength)
                throw new ConfigurationException(String.Format("The path for track {0} exceeds the maximum path length. " +
                                                               "Please choose another download directory with a shorter path", track.Name));

            return completePath;
        }
    }
}
