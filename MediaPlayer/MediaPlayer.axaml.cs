using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using LibVLCSharp.Shared;


namespace AOSMvvm.Views
{
    public partial class MediaPlayer : UserControl
    {
        private readonly LibVLC _libVlc = new();
        private LibVLCSharp.Shared.MediaPlayer mediaPlayer;
        private bool isDragging = false;

        public MediaPlayer()
        {
            InitializeComponent();
            mediaPlayer = new(_libVlc);
            videoView.MediaPlayer = mediaPlayer;

            SeekSlider.PointerPressed += (s, e) => isDragging = true;
            SeekSlider.PointerReleased += (s, e) => isDragging = false;
            SeekSlider.ValueChanged += (s, e) =>
            {
                if (e.NewValue != mediaPlayer.Time)
                    SetTime(Convert.ToInt64(Math.Floor(e.NewValue)));
            };

            mediaPlayer.LengthChanged += (s, e) =>
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    SetSliderDuration(e.Length); // set maximum properly
                });
            };
            mediaPlayer.TimeChanged += (s, e) =>
            {
                if (isDragging)
                    return;

                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    if (e.Time != SeekSlider.Value)
                        SetSeekSlider(e.Time);
                });
            };
            mediaPlayer.EndReached += (s, e) =>
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    mediaPlayer.Stop();
                    mediaPlayer.Play();
                });
            };

            VolumeSlider.ValueChanged += (s, e) => SetVolume(Convert.ToInt32(e.NewValue));
        }

        public void TogglePlayPause(object? sender=null, RoutedEventArgs? e=null)
        {
            if (mediaPlayer != null && mediaPlayer.IsPlaying) mediaPlayer.Pause();
            else if (mediaPlayer != null && mediaPlayer.WillPlay) mediaPlayer.Play();
        }
        public void OpenVideoAndPlay(string Path)
        {
            imageView.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            imageView.IsVisible = false;

            mediaPlayer.Stop();
            videoView.IsVisible = true;

            using var media = new Media(_libVlc, new Uri(Path));
            mediaPlayer!.Play(media);
        }
        public void StopVideo()
        {
            mediaPlayer!.Stop();
        }
        public void SetVolume(int volume) => mediaPlayer!.Volume = volume;
        public void SetTime(long time) => mediaPlayer!.Time = time;
        public void SetSeekSlider(double time) => SeekSlider.Value = time;
        public void SetSliderDuration(double time) => SeekSlider.Maximum = time;

        public void OpenImage(string Path)
        {
            imageView.Text = "";
            imageView.Background = new ImageBrush(new Bitmap(Path));

            mediaPlayer.Stop();
            imageView.IsVisible = true;
            videoView.IsVisible = false;
        }
    }
}