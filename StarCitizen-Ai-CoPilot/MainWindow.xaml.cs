using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using StarCitizen_Ai_CoPilot.Application;

namespace StarCitizen_Ai_CoPilot
{
    public partial class MainWindow : Window
    {
        private VoiceCommandController? _voiceCommandController;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void Initialize(VoiceCommandController voiceCommandController)
        {
            _voiceCommandController = voiceCommandController;
            _voiceCommandController.StatusChanged += OnStatusChanged;
            _voiceCommandController.TranscriptionReceived += OnTranscriptionReceived;
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() => _voiceCommandController?.StartListening());
        }

        private void OnStatusChanged(object sender, string status)
        {
            Dispatcher.Invoke(() =>
            {
                switch (status)
                {
                    case "Aufnahme gestartet":
                        RecordingIndicator.Fill = Brushes.Blue;
                        break;
                    case "Sprache erkannt":
                        SpeechDetectedIndicator.Fill = Brushes.Green;
                        break;
                    case "Aufnahme gestoppt":
                        RecordingIndicator.Fill = Brushes.Gray;
                        SpeechDetectedIndicator.Fill = Brushes.Gray;
                        break;
                }
            });
        }

        private void OnTranscriptionReceived(object sender, string transcription)
        {
            Dispatcher.Invoke(() => TranscriptionTextBlock.Text = transcription);
        }
    }
}
