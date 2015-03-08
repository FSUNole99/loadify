using System;
using System.IO;
using System.Runtime.InteropServices;
using loadify.Audio;
using loadify.Configuration;
using loadify.Model;
using SpotifySharp;

namespace loadify.Spotify
{
    public class TrackDownloadService
    {
        private class Statistic
        {
            public TimeSpan TargetDuration { get; set; }
            public uint Processings { get; set; }
            public double AverageFrameSize { get; set; }

            public Statistic(TimeSpan targetDuration)
            {
                TargetDuration = targetDuration;
            }

            public Statistic()
                : this(new TimeSpan())
            { }
        }

        public enum CancellationReason
        {
            None,
            UserInteraction,
            PlayTokenLost,
            Unknown,
            ConnectionLost
        };

        public bool Active { get; set; }
        public CancellationReason Cancellation { get; set; }
        public AudioMetaData AudioMetaData { get; set; }
        public AudioProcessor AudioProcessor { get; set; }
        public Track Track { get; set; }
        public Action<double> DownloadProgressUpdated = progress => { };
        public string DownloadFilePath { get; set; }

        private Statistic _Statistic = new Statistic();

        public double Progress
        {
            get
            {
                var trackDuration = _Statistic.TargetDuration.TotalMilliseconds;
                return (trackDuration != 0)
                        ? 100 / _Statistic.TargetDuration.TotalMilliseconds * (46.4 * _Statistic.Processings)
                        : 100;
            }
        }

        public TrackDownloadService(Track track, AudioProcessor audioProcessor, string downloadFilePath)
        {
            Track = track;
            AudioProcessor = audioProcessor;
            AudioMetaData = new AudioMetaData();
            DownloadFilePath = downloadFilePath;
        }

        public void Start()
        {
            _Statistic = new Statistic(TimeSpan.FromMilliseconds(Track.Duration()));
            
            AudioProcessor.Start(DownloadFilePath);
            Active = true;
        }

        public void ProcessInput(AudioFormat format, IntPtr frames, int numFrames)
        {
            AudioMetaData.SampleRate = format.sample_rate;
            AudioMetaData.Channels = format.channels;

            var size = numFrames * format.channels * 2;
            var buffer = new byte[size];
            Marshal.Copy(frames, buffer, 0, size);
            AudioProcessor.Process(buffer);

            _Statistic.Processings++;
            _Statistic.AverageFrameSize = (_Statistic.AverageFrameSize + size) / 2;

            DownloadProgressUpdated(Progress);
        }

        public void Stop()
        {
            AudioProcessor.Release();
            Active = false;
        }

        public void Cancel(CancellationReason reason)
        {
            Stop();
            Cancellation = reason;
        }
    }
}
