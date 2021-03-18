using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using System;
using System.Threading.Tasks;
using Plugin.AudioRecorder;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Diagnostics;
using KBS.App.TaxonFinder.Services;
using Plugin.SimpleAudioPlayer;
using KBS.App.TaxonFinder.Models;
using System.IO;
using System.Timers;

namespace KBS.App.TaxonFinder.ViewModels
{
    public class TakeAudioViewModel : INotifyPropertyChanged
    {
        #region Fields

        private const string StartRecordingText = "Aufnahme starten";
        private const string ResetRecordingText = "Aufnahme ersetzen";
        private const string StopRecordingText = "Aufnahme stoppen";
        private const string StartPlayingText = "Wiedergabe starten";
        private const string ResumePlayingText = "Wiedergabe fortsetzen";
        private const string StopPlayingText = "Wiedergabe pausieren";
        private bool _enablePlaying;
        private bool _isTimerRunning;
        private bool _enableRecording = true;
        private bool _recordingPossible = true;
        private bool _recordLoaded = false;
        private int _timeRunning = 0;
        private string _time;
        private string _recordingText;
        private string _playingText;
        private Timer _timer;

        #endregion

        #region Properties

        public bool RecordExists { get; private set; }
        public bool IsTimerRunning
        {
            get { return _isTimerRunning; }
            set
            {
                _isTimerRunning = value;
                OnPropertyChanged(nameof(IsTimerRunning));
            }
        }
        public bool EnableRecording
        {
            get
            {
                return _enableRecording && _recordingPossible;
            }
            set
            {
                _enableRecording = value;
                OnPropertyChanged(nameof(EnableRecording));
            }
        }
        public bool EnablePlaying
        {
            get { return _enablePlaying; }
            set
            {
                _enablePlaying = value;
                OnPropertyChanged(nameof(EnablePlaying));
            }
        }
        public string Time
        {
            get { return _time; }
            set
            {
                _time = value;
                OnPropertyChanged(nameof(Time));
            }
        }
        public string RecordingText
        {
            get { return _recordingText; }
            set
            {
                _recordingText = value;
                OnPropertyChanged(nameof(RecordingText));
            }
        }
        public string PlayingText
        {
            get { return _playingText; }
            set
            {
                _playingText = value;
                OnPropertyChanged(nameof(PlayingText));
            }
        }
        public ISimpleAudioPlayer Player { get; set; }
        public AudioRecorderService Recorder { get; set; }

        #endregion

        #region Constructor

        public TakeAudioViewModel()
        {
            RecordingCommand = new Command(async () => await Recording());
            PlayingCommand = new Command(async () => Playing());
            SaveCommand = new Command(async () => await Save());
            CancelCommand = new Command(async () => await Cancel());
            RecordingText = StartRecordingText;
            PlayingText = StartPlayingText;
            Time = "00:00";
            _timer = new Timer();
            _timer.Interval = 250;
            _timer.Elapsed += Timer_Elapsed;

            Recorder = new AudioRecorderService()
            {
                StopRecordingAfterTimeout = false,
                StopRecordingOnSilence = false
            };
            Player = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            Player.PlaybackEnded -= Playback_Ended;
            Player.PlaybackEnded += Playback_Ended;
        }

        #endregion

        #region Recording Command

        public ICommand RecordingCommand { get; set; }
        private async Task Recording()
        {
            var statusRecording = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Microphone);
            if (statusRecording != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Microphone))
                {
                    await Application.Current.MainPage.DisplayAlert("Benötige Mikrofon-Berechtigung", "Zum Aufnehmen von Audiodateien wird die Mikrofon-Berechtigung benötigt.", "Okay");
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Microphone });
                statusRecording = results[Permission.Microphone];
            }

            if (statusRecording == PermissionStatus.Granted)
            {
                if (!Recorder.IsRecording)
                {
                    if (Recorder.FilePath != null && File.Exists(Recorder.FilePath))
                        File.Delete(Recorder.FilePath);
                    var title = $"Audio_{DateTime.Now:yyyyMMdd_HHmmss}.wav";

                    var fileHelper = DependencyService.Get<IFileHelper>();
                    var documentsPath = fileHelper.GetLocalAppPath(title);
                    Recorder.FilePath = documentsPath;
                    RecordExists = false;
                    _recordLoaded = false;
                    OnPropertyChanged(nameof(RecordExists));
                    Time = "00:00";
                    RecordingText = StopRecordingText;
                    _timeRunning = 0;
                    var timerRunning = 0;
                    IsTimerRunning = true;
                    EnablePlaying = false;
                    Device.StartTimer(TimeSpan.FromMilliseconds(250), () =>
                    {
                        timerRunning++;
                        if (timerRunning % 4 == 0)
                        {
                            _timeRunning++;
                            var seconds = _timeRunning % 60;
                            var minutes = _timeRunning / 60;
                            Time = $"{minutes:00}:{seconds:00}";
                        }
                        return IsTimerRunning;
                    });

                    await Recorder.StartRecording();
                    OnPropertyChanged(nameof(Recorder));

                }
                else
                {
                    IsTimerRunning = false;
                    await Recorder.StopRecording();
                    EnablePlaying = true;
                    RecordExists = true;
                    RecordingText = ResetRecordingText;
                    OnPropertyChanged(nameof(Recorder));
                    OnPropertyChanged(nameof(RecordExists));
                }
            }
            else if (statusRecording != PermissionStatus.Unknown)
            {
                await Application.Current.MainPage.DisplayAlert("Berechtigung verweigert", "Ohne Berechtigungen können keine Audiodateien aufgenommen werden.", "Okay");
            }
        }

        #endregion

        #region Playing Command

        public ICommand PlayingCommand { get; set; }

        private void Playing()
        {
            try
            {
                if (!_recordLoaded)
                {
                    var filePath = Recorder.FilePath;
                    try
                    {
                        using (var audioFileStream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                        {
                            Player.Load(audioFileStream);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    _recordLoaded = true;
                }

                if (!Player.IsPlaying)
                {
                    Player.Play();
                    Time = $"00:00/{(int)Player.Duration / 60:00}:{(int)Player.Duration % 60:00}";
                    PlayingText = StopPlayingText;
                    EnableRecording = false;
                    _timer.Start();

                }
                else if (Player.IsPlaying)
                {
                    Player.Pause();
                    _timer.Stop();
                    PlayingText = ResumePlayingText;
                    EnableRecording = true;
                }
                OnPropertyChanged(nameof(Player));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Time = $"{(int)Player.CurrentPosition / 60:00}:{(int)Player.CurrentPosition % 60:00}/{(int)Player.Duration / 60:00}:{(int)Player.Duration % 60:00}";
        }

        #endregion

        #region Save Command

        public ICommand SaveCommand { get; set; }
        private async Task Save()
        {
            try
            {
                ((App)App.Current).AudioRecording = new MediaFileModel
                {
                    Path = Recorder.FilePath,
                    Media = MediaFileModel.MediaType.Audio
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        #endregion

        #region Cancel Command

        public ICommand CancelCommand { get; set; }
        private async Task Cancel()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        #endregion

        #region Methods

        public void LoadAudio(string mediaPath)
        {
            try
            {
                using (var audioFileStream = File.Open(mediaPath, FileMode.Open, FileAccess.Read))
                {
                    Player.Load(audioFileStream);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            _recordLoaded = true;
            _recordingPossible = false;
            OnPropertyChanged(nameof(EnableRecording));
            EnablePlaying = true;
            RecordingText = StartRecordingText;
            //Time = $"00:00/{(int)Player.Duration / 60:00}:{(int)Player.Duration % 60:00}";
        }
        private void Playback_Ended(object sender, EventArgs e)
        {
            Player.Seek(0.001);
            _timer.Stop();
            Time = $"00:00/{(int)Player.Duration / 60:00}:{(int)Player.Duration % 60:00}";
            PlayingText = StartPlayingText;
            EnableRecording = true;
            OnPropertyChanged(nameof(Player));
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}