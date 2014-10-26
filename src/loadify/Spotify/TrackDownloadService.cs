﻿using System;
using System.Runtime.InteropServices;
using loadify.Audio;
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
            PlayTokenLost,
            Unknown,
            ConnectionLost
        };

        public bool Active { get; set; }
        public string OutputDirectory { get; set; }
        public string OutputFileName { get; set; }
        public AudioFileMetaData AudioFileMetaData { get; set; }
        public AudioMetaData AudioMetaData { get; set; }
        public AudioProcessor AudioProcessor { get; set; }
        public AudioConverter AudioConverter { get; set; }
        public IAudioFileDescriptor AudioFileDescriptor { get; set; }

        private Statistic _Statistic = new Statistic();
        private readonly Action<CancellationReason> _DownloadCompletedCallback = cancellationReason => { };
        private readonly Action<double> _DownloadProgressUpdatedCallback = progress => { };

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

        public TrackDownloadService(string outputDirectory, string outputFileName,
                                    AudioProcessor audioProcessor, AudioConverter audioConverter, IAudioFileDescriptor audioFileDescriptor,
                                    AudioFileMetaData audioFileMetaData,
                                    Action<CancellationReason> downloadCompletedCallback, Action<double> downloadProgressUpdatedCallback)
        {
            OutputDirectory = outputDirectory;
            OutputFileName = outputFileName;
            AudioProcessor = audioProcessor;
            AudioConverter = audioConverter;
            AudioFileDescriptor = audioFileDescriptor;
            AudioFileMetaData = audioFileMetaData;
            _DownloadCompletedCallback = downloadCompletedCallback;
            _DownloadProgressUpdatedCallback = downloadProgressUpdatedCallback;
            AudioMetaData = new AudioMetaData();
        }

        public void Start(TrackModel track)
        {
            _Statistic = new Statistic(track.Duration);
            AudioProcessor.Start(String.Format("{0}/{1}.{2}", OutputDirectory, OutputFileName, AudioProcessor.TargetFileExtension));
            Active = true;
        }

        public void Stop()
        {
            Active = false;
        }

        public void ProcessInput(AudioFormat format, IntPtr frames, int num_frames)
        {
            AudioMetaData.SampleRate = format.sample_rate;
            AudioMetaData.Channels = format.channels;

            var size = num_frames * format.channels * 2;
            var buffer = new byte[size];
            Marshal.Copy(frames, buffer, 0, size);
            AudioProcessor.Process(buffer);

            _Statistic.Processings++;
            _Statistic.AverageFrameSize = (_Statistic.AverageFrameSize + size) / 2;

            _DownloadProgressUpdatedCallback(Progress);
        }

        public void Finish()
        {
            Stop();
            AudioProcessor.Release();

            var outputFilePath = String.Format("{0}/{1}.{2}", OutputDirectory, OutputFileName, AudioProcessor.TargetFileExtension);
            if (AudioConverter != null)
                outputFilePath = AudioConverter.Convert(outputFilePath, String.Format("{0}/{1}.{2}", 
                                                                        OutputDirectory, 
                                                                        OutputFileName, 
                                                                        AudioConverter.TargetFileExtension));

            if (AudioFileDescriptor != null)
                AudioFileDescriptor.Write(AudioFileMetaData, outputFilePath);

            _DownloadCompletedCallback(CancellationReason.None);
        }

        public void Cancel(CancellationReason reason)
        {
            Stop();
            _DownloadCompletedCallback(reason);
        }
    }
}
