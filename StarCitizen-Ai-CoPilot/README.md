# StarCitizen AI Co-Pilot

## Projektbeschreibung

Das Projekt **StarCitizen AI Co-Pilot** integriert Sprachbefehle in das Spiel StarCitizen. Es nutzt Sprachaufnahmen, wandelt diese mithilfe von Whisper in Text um und sendet den Text an OpenAI. OpenAI entscheidet basierend auf einer vordefinierten Liste von Befehlen, welcher Befehl ausgeführt werden soll. Der entsprechende Befehl wird dann als Tastenkombination im Spiel ausgeführt.

## Projektstruktur

Das Projekt ist gemäß der Clean Architecture organisiert und besteht aus den folgenden Ebenen:

- **Application**: Beinhaltet die Interaktionen mit dem Benutzer.
  - `VoiceCommandController`: Steuert den Ablauf des Sprachbefehlsprozesses.
  - `MainWindow`: Hauptfenster der Anwendung, das beim Start angezeigt wird.

- **Core**: Enthält die Geschäftslogik und die Anwendungsfälle.
  - `UseCases`
    - `VoiceRecognitionUseCase`: Implementierung des Sprachbefehls-Anwendungsfalls.
  - `Services`
    - `IAudioRecordingService`: Interface für den Audioaufnahmeservice.
    - `IAudioTranscriptionService`: Interface für den Audiotranskriptionsservice.

- **Infrastructure**: Implementiert die Schnittstellen und bindet Fremdbibliotheken und SDKs ein.
  - `AudioRecordingService`: Implementierung des Audioaufnahmeservices.
  - `WhisperTranscriptionService`: Implementierung des Audiotranskriptionsservices.
  - `OpenAiClient`: Implementierung des OpenAI-Clients zur Verarbeitung der Textbefehle.

## Installation

1. **Repository klonen**:

    ```bash
    git clone https://github.com/your-repo/starcitizen-ai-copilot.git
    cd starcitizen-ai-copilot
    ```

2. **Abhängigkeiten installieren**:

    Stellen Sie sicher, dass Sie .NET 8.0 SDK installiert haben. Führen Sie dann den folgenden Befehl aus:

    ```bash
    dotnet restore
    ```

3. **Umgebungsvariablen konfigurieren**:

    Legen Sie die erforderlichen API-Keys und Konfigurationen als Umgebungsvariablen fest.

## Nutzung

1. **Projekt bauen und starten**:

    ```bash
    dotnet build
    dotnet run
    ```

2. **Sprachbefehle starten**:

    Der `VoiceCommandController` wird gestartet und hört auf Sprachbefehle. Diese werden aufgenommen, in Text umgewandelt und an OpenAI gesendet.

## Fortschritte

- Die Anwendung zeigt nun korrekt das Hauptfenster (`MainWindow`) an, ohne dass es zu einer Blockierung des UI-Threads kommt.
- Die Audioaufnahme und Transkription wird asynchron durchgeführt, um sicherzustellen, dass die Benutzeroberfläche reaktionsfähig bleibt.
- Fehlerbehebungen und Anpassungen in der Projektstruktur wurden vorgenommen, um die Stabilität der Anwendung zu verbessern.

## ToDo

- Erweiterung der Befehlsliste und Zuordnung zu Tastenkombinationen.
- Zusätzliche Tests und Fehlerbehebungen.

## AI-Assistent
Starte AI-Code-Writer mit:
```shell
npx ai-code-writer
```

---

Dieses Projekt steht unter der MIT-Lizenz. Weitere Details finden Sie in der Datei `LICENSE`.