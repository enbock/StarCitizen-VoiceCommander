using StarCitizen_Ai_CoPilot.Core.Services;
using System;
using System.Threading.Tasks;

namespace StarCitizen_Ai_CoPilot.Core.UseCases
{
    public class VoiceRecognitionUseCase
    {
        private readonly IAudioRecordingService _audioRecordingService;
        private readonly IAudioTranscriptionService _audioTranscriptionService;

        public event EventHandler<string>? StatusChanged;
        public event EventHandler<string>? TranscriptionReceived;

        public VoiceRecognitionUseCase(IAudioRecordingService audioRecordingService, IAudioTranscriptionService audioTranscriptionService)
        {
            _audioRecordingService = audioRecordingService;
            _audioTranscriptionService = audioTranscriptionService;
            _audioRecordingService.RecordingStatusChanged += OnRecordingStatusChanged;
        }

        private void OnRecordingStatusChanged(object sender, string e)
        {
            StatusChanged?.Invoke(this, e);
        }

        public void Execute()
        {
            _audioRecordingService.RecordAudio(async audioData =>
            {
                var text = await _audioTranscriptionService.TransformAudioToTextAsync(audioData);
                TranscriptionReceived?.Invoke(this, text);
                Execute(); // Restart the recording process after transcription is received
            });
        }
    }
}
