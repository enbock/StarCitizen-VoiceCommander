using System;
using System.Threading.Tasks;

namespace StarCitizen_Ai_CoPilot.Core.Services
{
    public interface IAudioRecordingService
    {
        event EventHandler<string> RecordingStatusChanged;
        void RecordAudio(Func<byte[], Task> audioDataHandler);
    }
}
