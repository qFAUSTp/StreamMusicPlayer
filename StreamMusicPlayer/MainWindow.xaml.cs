using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StreamMusicPlayer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Music music;
        public SongAlert songAlert;
        public MainWindow()
        {
            InitializeComponent();

            music = new Music(@"C:\playlist.wpl", this);

            UpdateCover();

            playListLB.ItemsSource = music.PlayList;

            songAlert = new SongAlert(this);
            songAlert.Show();

            music.mSongAlert = songAlert;
        }

        private void Button_Click_BindAlerWindow(object sender, RoutedEventArgs e)
        {
            if (songAlert.BindingState == true)
                songAlert.BindingState = false;
            else
                songAlert.BindingState = true;
            Console.WriteLine(songAlert.BindingState);
        }

        private void Button_Click_Play(object sender, RoutedEventArgs e)
        {
            songAlert.Hide();
            songAlert.Show();
            music.PlayMusic();
        }

        internal void Button_Click_NextSong(object sender, RoutedEventArgs e)
        {
            if (music.CurrentPlaying + 1 <= music.LastSong)
            {
                music.CurrentPlaying++;
                music.MusicState = States.NoMusic;
                music.PlayMusic();
                UpdateCover();

                songAlert.RedrawWindow();
            }
        }

        internal void Button_Click_PreviousSong(object sender, RoutedEventArgs e)
        {
            if (music.CurrentPlaying - 1 >= 0)
            {
                music.CurrentPlaying--;
                music.MusicState = States.NoMusic;
                music.PlayMusic();
                UpdateCover();

                songAlert.RedrawWindow();
            }
        }

        internal void UpdateCover()
        {
            var file = TagLib.File.Create(@"C:\" + music.GetCurrentSong());
            TagLib.IPicture pic = file.Tag.Pictures[0];
            MemoryStream ms = new MemoryStream(pic.Data.Data);
            ms.Seek(0, SeekOrigin.Begin);

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = ms;
            bitmap.EndInit();

            img.Source = bitmap;
        }

        
    }
}
