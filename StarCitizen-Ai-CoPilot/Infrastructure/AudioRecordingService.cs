using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using NAudio.Wave;
using StarCitizen_Ai_CoPilot.Core.Services;
using System.Collections.Generic;

namespace StarCitizen_Ai_CoPilot.Infrastructure
{
    public class AudioRecordingService : IAudioRecordingService
    {
        public event EventHandler<string>? RecordingStatusChanged;
        private bool _isListening;
        private MemoryStream? _memoryStream;
        private WaveInEvent? _waveIn;
        private bool _detectedStartOfSpeech;
        private float _highestRms;
        private const float RmsStartThresholdFactor = 0.8f;
        private const float RmsStopThresholdFactor = 0.8f;
        private DateTime? _silenceStartTime;
        private float _rmsFirstSecond;
        private DateTime _recordStartTime;
        private const int AvgMeasureTime = 2000;
        private const int StartMeasureTime = 500;
        private const int ChunkSize = 4096;
        private readonly Queue<byte[]> _preSpeechChunks = new();

        public void RecordAudio(Func<byte[], Task> audioDataHandler)
        {
            if (_isListening) throw new InvalidOperationException("Already recording.");

            ResetRecordingState();

            _memoryStream = new MemoryStream();
            _waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(44100, 1)
            };
            _waveIn.DataAvailable += OnDataAvailable;
            _waveIn.RecordingStopped += async (s, a) =>
            {
                _isListening = false;
                _waveIn.Dispose();
                _waveIn = null;

                byte[] wavData;
                using (MemoryStream wavStream = new())
                {
                    await using (WaveFileWriter writer = new(wavStream, new WaveFormat(44100, 1)))
                    {
                        _memoryStream.Position = 0;
                        await _memoryStream.CopyToAsync(writer);
                    }

                    wavData = wavStream.ToArray();
                }

                await _memoryStream.DisposeAsync();
                _memoryStream = null;

                PlayAudio(wavData);
                await audioDataHandler(wavData);
            };
            _waveIn.StartRecording();
            _isListening = true;

            RecordingStatusChanged?.Invoke(this, "Aufnahme gestartet");
        }

        private void OnDataAvailable(object? sender, WaveInEventArgs e)
        {
            if (_memoryStream == null) return;

            float rms = CalculateRms(e.Buffer, e.BytesRecorded);

            if (rms > _highestRms)
            {
                _highestRms = rms;
            }

            float factor = _detectedStartOfSpeech ? RmsStopThresholdFactor : RmsStartThresholdFactor;
            float rmsThresholdHigh = _highestRms * factor;
            float rmsThresholdLow = (rms * (2 - factor)) * 2;
            float diffThreshold = 1 / rmsThresholdLow * _highestRms;
            bool minGainDetected = diffThreshold > 1;
            Console.WriteLine(
                $"DE:{minGainDetected} F:{factor} TH:{rmsThresholdHigh} TL:{rmsThresholdLow} DIF:{diffThreshold}"
            );
            DateTime now = DateTime.Now;

            if (_recordStartTime == default)
            {
                _recordStartTime = now;
            }

            if ((now - _recordStartTime).TotalMilliseconds <= StartMeasureTime)
            {
                if (rms > _rmsFirstSecond)
                {
                    _rmsFirstSecond = rms;
                }
            }
            else
            {
                if (_rmsFirstSecond > rmsThresholdHigh && minGainDetected)
                {
                    _detectedStartOfSpeech = true;
                }
            }

            _detectedStartOfSpeech = _detectedStartOfSpeech || minGainDetected;

            if (_detectedStartOfSpeech)
            {
                RecordingStatusChanged?.Invoke(this, "Sprache erkannt");
            }

            if (!_detectedStartOfSpeech)
            {
                if (_preSpeechChunks.Count >= 10)
                {
                    _preSpeechChunks.Dequeue();
                }
                _preSpeechChunks.Enqueue((byte[])e.Buffer.Clone());
                return;
            }

            if (_preSpeechChunks.Count > 0)
            {
                while (_preSpeechChunks.Count > 0)
                {
                    byte[] chunk = _preSpeechChunks.Dequeue();
                    _memoryStream.Write(chunk, 0, chunk.Length);
                }
            }
            _memoryStream.Write(e.Buffer, 0, e.BytesRecorded);

            if (rms < rmsThresholdHigh)
            {
                if (!_silenceStartTime.HasValue)
                {
                    _silenceStartTime = now;
                }
                else if ((now - _silenceStartTime.Value).TotalMilliseconds >= AvgMeasureTime)
                {
                    StopRecording();
                }
            }
            else
            {
                _silenceStartTime = null;
            }
        }

        private float CalculateRms(byte[] buffer, int bytesRecorded)
        {
            int count = bytesRecorded / 2;
            double sumOfSquares = 0;

            for (int i = 0; i < count; i++)
            {
                short sample = BitConverter.ToInt16(buffer, i * 2);
                double sample32 = sample / 32768.0;
                sumOfSquares += sample32 * sample32;
            }

            return (float)Math.Sqrt(sumOfSquares / count);
        }

        private void StopRecording()
        {
            if (_waveIn == null) return;

            _waveIn.StopRecording();
        }

        private void ResetRecordingState()
        {
            _isListening = false;
            _memoryStream = null;
            _waveIn = null;
            _detectedStartOfSpeech = false;
            _highestRms = 0;
            _silenceStartTime = null;
            _rmsFirstSecond = 0;
            _recordStartTime = default;
            _preSpeechChunks.Clear();
        }

        private void PlayAudio(byte[] audioData)
        {
            using var playbackStream = new MemoryStream(audioData);
            using var waveOut = new WaveOutEvent();
            using var waveReader = new WaveFileReader(playbackStream);

            waveOut.Init(waveReader);
            waveOut.Play();
            while (waveOut.PlaybackState == PlaybackState.Playing)
            {
                Thread.Sleep(100);
            }
        }
    }
}