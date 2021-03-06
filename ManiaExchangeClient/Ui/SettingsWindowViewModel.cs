﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using ManiaExchangeClient.DataObjects;
using WpfUtility.Services;

namespace ManiaExchangeClient.Ui
{
    public class SettingsWindowViewModel : ObservableObject
    {
        /// <summary>
        /// Contains the dialog coordinator
        /// </summary>
        private IDialogCoordinator _dialogCoordinator;

        /// <summary>
        /// The event to close the window
        /// </summary>
        public event CustomEvents.InfoEvent CloseWindow;

        /// <summary>
        /// Contains the value which indicates if the view model is still in the init phase
        /// </summary>
        private bool _init;

        /// <summary>
        /// Backing field for <see cref="TrackSearch"/>
        /// </summary>
        private string _trackSearch;

        /// <summary>
        /// Gets or sets the track search endpoint
        /// </summary>
        public string TrackSearch
        {
            get => _trackSearch;
            set => SetField(ref _trackSearch, value);
        }

        /// <summary>
        /// Backing field for <see cref="TrackDownload"/>
        /// </summary>
        private string _trackDownload;

        /// <summary>
        /// Gets or sets the track download endpoint
        /// </summary>
        public string TrackDownload
        {
            get => _trackDownload;
            set => SetField(ref _trackDownload, value);
        }

        /// <summary>
        /// Backing field for <see cref="Replays"/>
        /// </summary>
        private string _replays;

        /// <summary>
        /// Gets or sets the endpoint for the replays
        /// </summary>
        public string Replays
        {
            get => _replays;
            set => SetField(ref _replays, value);
        }

        /// <summary>
        /// Backing field for <see cref="EmbeddedObjects"/>
        /// </summary>
        private string _embeddedObjects;

        /// <summary>
        /// Gets or sets the endpoint for the embedded objects
        /// </summary>
        public string EmbeddedObjects
        {
            get => _embeddedObjects;
            set => SetField(ref _embeddedObjects, value);
        }

        /// <summary>
        /// Backing field for <see cref="Screenshots"/>
        /// </summary>
        private string _screenshots;

        /// <summary>
        /// Gets or sets the endpoints for the screenshots
        /// </summary>
        public string Screenshots
        {
            get => _screenshots;
            set => SetField(ref _screenshots, value);
        }

        /// <summary>
        /// Backing field for <see cref="Thumbnail"/>
        /// </summary>
        private string _thumbnail;

        /// <summary>
        /// Gets or sets the thumbnail endpoint
        /// </summary>
        public string Thumbnail
        {
            get => _thumbnail;
            set => SetField(ref _thumbnail, value);
        }

        /// <summary>
        /// Backing field for <see cref="TrackPage"/>
        /// </summary>
        private string _trackPage;

        /// <summary>
        /// Gets or sets the path of the track page
        /// </summary>
        public string TrackPage
        {
            get => _trackPage;
            set => SetField(ref _trackPage, value);
        }

        /// <summary>
        /// Backing field for <see cref="ThemeList"/>
        /// </summary>
        private ObservableCollection<string> _themeList = new ObservableCollection<string>();

        /// <summary>
        /// Gets or sets the theme list
        /// </summary>
        public ObservableCollection<string> ThemeList
        {
            get => _themeList;
            set => SetField(ref _themeList, value);
        }

        /// <summary>
        /// Backing field for <see cref="SelectedTheme"/>
        /// </summary>
        private string _selectedTheme;

        /// <summary>
        /// Gets or sets the selected theme
        /// </summary>
        public string SelectedTheme
        {
            get => _selectedTheme;
            set
            {
                if (SetField(ref _selectedTheme, value))
                    ChangeTheme();
            }
        }

        /// <summary>
        /// Backing field for <see cref="AccentList"/>
        /// </summary>
        private ObservableCollection<string> _accentList = new ObservableCollection<string>();

        /// <summary>
        /// Gets or sets the accent list
        /// </summary>
        public ObservableCollection<string> AccentList
        {
            get => _accentList;
            set => SetField(ref _accentList, value);
        }

        /// <summary>
        /// Backing field for <see cref="SelectedAccent"/>
        /// </summary>
        private string _selectedAccent;

        /// <summary>
        /// Gets or sets the selected accent
        /// </summary>
        public string SelectedAccent
        {
            get => _selectedAccent;
            set
            {
                if (SetField(ref _selectedAccent, value))
                    ChangeTheme();

            }
        }

        /// <summary>
        /// The command to save the settings
        /// </summary>
        public ICommand SaveCommand => new DelegateCommand(SaveSettings);

        /// <summary>
        /// The command to close the window
        /// </summary>
        public ICommand CloseCommand => new DelegateCommand(() =>
        {
            ChangeTheme(true);
            CloseWindow?.Invoke();
        });

        /// <summary>
        /// Init the view model
        /// </summary>
        /// <param name="dialogCoordinator"></param>
        public void InitViewModel(IDialogCoordinator dialogCoordinator)
        {
            _dialogCoordinator = dialogCoordinator;

            LoadSettings();
        }

        /// <summary>
        /// Loads the settings
        /// </summary>
        private void LoadSettings()
        {
            _init = true;
            var settings = Helper.LoadSettings();

            // Set the values...
            // Track search
            var trackSearch = settings.Endpoints.FirstOrDefault(f => f.Type == EndpointType.TrackSearch);
            TrackSearch = trackSearch != null ? trackSearch.Path : "";

            // Track Download
            var trackDownload = settings.Endpoints.FirstOrDefault(f => f.Type == EndpointType.TrackDownload);
            TrackDownload = trackDownload != null ? trackDownload.Path : "";

            // Replays
            var replays = settings.Endpoints.FirstOrDefault(f => f.Type == EndpointType.GetReplays);
            Replays = replays != null ? replays.Path : "";

            // Embedded objects
            var objects = settings.Endpoints.FirstOrDefault(f => f.Type == EndpointType.GetEmbeddedObjects);
            EmbeddedObjects = objects != null ? objects.Path : "";

            // Screenshot
            var screenshot = settings.Endpoints.FirstOrDefault(f => f.Type == EndpointType.Screenshot);
            Screenshots = screenshot != null ? screenshot.Path : "";

            // Thumbnail
            var thumbnail = settings.Endpoints.FirstOrDefault(f => f.Type == EndpointType.Thumbnail);
            Thumbnail = thumbnail != null ? thumbnail.Path : "";

            // TrackPage
            var trackPage = settings.Endpoints.FirstOrDefault(f => f.Type == EndpointType.TrackPage);
            TrackPage = trackPage != null ? trackPage.Path : "";

            ThemeList = new ObservableCollection<string>(ThemeManager.AppThemes.Select(s => s.Name));
            AccentList = new ObservableCollection<string>(ThemeManager.Accents.Select(s => s.Name));

            SelectedTheme = Properties.Settings.Default.Theme;
            SelectedAccent = Properties.Settings.Default.Accent;

            _init = false;
        }

        /// <summary>
        /// Saves the settings
        /// </summary>
        private async void SaveSettings()
        {
            var settings = new SettingsModel();

            settings.Endpoints.Add(new Endpoint(TrackSearch, EndpointType.TrackSearch));
            settings.Endpoints.Add(new Endpoint(TrackDownload, EndpointType.TrackDownload));
            settings.Endpoints.Add(new Endpoint(Replays, EndpointType.GetReplays));
            settings.Endpoints.Add(new Endpoint(EmbeddedObjects, EndpointType.GetEmbeddedObjects));
            settings.Endpoints.Add(new Endpoint(Screenshots, EndpointType.Screenshot));
            settings.Endpoints.Add(new Endpoint(Thumbnail, EndpointType.Thumbnail));
            settings.Endpoints.Add(new Endpoint(TrackPage, EndpointType.TrackPage));

            if (!Helper.SaveSettings(settings))
            {
                await _dialogCoordinator.ShowMessageAsync(this, "Save", "An error has occured while saving the settings");
            }

            
            if (!Properties.Settings.Default.Accent.Equals(SelectedAccent))
                Properties.Settings.Default.Accent = SelectedAccent;

            if (!Properties.Settings.Default.Theme.Equals(SelectedTheme))
                Properties.Settings.Default.Theme = SelectedTheme;

            Properties.Settings.Default.Save();

            ChangeTheme();
        }

        /// <summary>
        /// Changes the current theme
        /// </summary>
        public void ChangeTheme(bool toDefault = false)
        {
            if (_init)
                return;

            var theme = Properties.Settings.Default.Theme;
            var accent = Properties.Settings.Default.Accent;

            if (!toDefault)
            {
                if (!string.IsNullOrEmpty(SelectedAccent))
                    accent = SelectedAccent;

                if (!string.IsNullOrEmpty(SelectedTheme))
                    theme = SelectedTheme;
            }

            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(accent),
                ThemeManager.GetAppTheme(theme));
        }
    }
}
