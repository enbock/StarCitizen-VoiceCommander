using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using StarCitizen_Ai_CoPilot.Application;
using StarCitizen_Ai_CoPilot.Core.Services;
using StarCitizen_Ai_CoPilot.Core.UseCases;
using StarCitizen_Ai_CoPilot.Infrastructure;

namespace StarCitizen_Ai_CoPilot
{
    public partial class App : System.Windows.Application
    {
        private ServiceProvider? _serviceProvider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            var voiceCommandController = _serviceProvider.GetRequiredService<VoiceCommandController>();
            mainWindow.Initialize(voiceCommandController);
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<VoiceCommandController>();
            services.AddSingleton<IAudioRecordingService, AudioRecordingService>();
            services.AddSingleton<IAudioTranscriptionService, WhisperTranscriptionService>();
            services.AddSingleton<VoiceRecognitionUseCase>();
        }
    }
}