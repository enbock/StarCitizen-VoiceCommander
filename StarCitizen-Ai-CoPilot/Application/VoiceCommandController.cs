using StarCitizen_Ai_CoPilot.Core;
using StarCitizen_Ai_CoPilot.Core.UseCases;
using System;

namespace StarCitizen_Ai_CoPilot.Application
{
    public class VoiceCommandController
    {
        private readonly VoiceRecognitionUseCase _voiceRecognitionUseCase;

        public event EventHandler<string>? StatusChanged;
        public event EventHandler<string>? TranscriptionReceived;

        public VoiceCommandController(VoiceRecognitionUseCase voiceRecognitionUseCase)
        {
            _voiceRecognitionUseCase = voiceRecognitionUseCase;
            _voiceRecognitionUseCase.StatusChanged += VoiceRecognitionUseCase_StatusChanged;
            _voiceRecognitionUseCase.TranscriptionReceived += VoiceRecognitionUseCase_TranscriptionReceived;
        }

        private void VoiceRecognitionUseCase_StatusChanged(object sender, string e)
        {
            StatusChanged?.Invoke(this, e);
        }

        private void VoiceRecognitionUseCase_TranscriptionReceived(object sender, string e)
        {
            TranscriptionReceived?.Invoke(this, e);
        }

        public void StartListening()
        {
            _voiceRecognitionUseCase.Execute();
        }
    }
}
