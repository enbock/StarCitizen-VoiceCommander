using System.Threading.Tasks;

namespace StarCitizen_Ai_CoPilot.Core.Services
{
    public interface IAudioTranscriptionService
    {
        Task<string> TransformAudioToTextAsync(byte[] audioData);
    }
}
