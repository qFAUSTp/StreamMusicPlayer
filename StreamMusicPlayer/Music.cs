using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace StreamMusicPlayer
{
    public enum States { NoMusic, Playing, Paused }
    public class Music
    {

        public List<string> PlayList { get; private set; }
        public int CurrentPlaying { get; set; }
        public int LastSong { get; private set; }

        public States MusicState { get; set; }

        private MediaPlayer songStream;

        public SongAlert mSongAlert { get; set; }

        public Music(string path, MainWindow mainWindow)
        {
            MusicState = States.NoMusic;
            LoadNewPlaylist(path, mainWindow);
        }

        public void LoadNewPlaylist(string path, MainWindow mainWindow)
        {
            PlayList = new List<string>();

            using (StreamReader sr = new StreamReader(path))
            {
                string textLine = null;
                while (!string.IsNullOrEmpty(textLine = sr.ReadLine()))
                    if (textLine.Contains(@"..\..\..\..\"))
                    {
                        int i = textLine.IndexOf(".mp3");
                        PlayList.Add(textLine.Substring(36, i - 36 + 4));
                    }
            }

            CurrentPlaying = 0;
            LastSong = PlayList.Count() - 1;

            songStream = new MediaPlayer();
            songStream.MediaEnded += (snd, EventArgs) => {
                CurrentPlaying++;
                MusicState = States.NoMusic;
                PlayMusic();
                mainWindow.UpdateCover();
                mSongAlert.RedrawWindow();
            };
        }

        public void PlayMusic()
        {
            //Console.WriteLine(musicState);
            switch (MusicState)
            {
                //Start playing new playlist.
                case States.NoMusic:
                    MusicState = States.Playing;
                    Console.WriteLine(MusicState);
                    PlaySong();
                    break;
                //Pause currently playing music.
                case States.Playing:
                    MusicState = States.Paused;
                    Console.WriteLine(MusicState);
                    songStream.Pause();
                    break;
                //Continue playing currently stopped music.
                case States.Paused:
                    MusicState = States.Playing;
                    Console.WriteLine(MusicState);
                    songStream.Play();
                    break;
            }
        }

        private void PlaySong()
        {
            mSongAlert.RedrawWindow();
            songStream.Stop();
            if (CurrentPlaying >= 0 && CurrentPlaying <= LastSong)
            {
                songStream.Open(new Uri(@"C:\" + PlayList[CurrentPlaying]));
                songStream.Play();
            }
            else return;
        }

        public string GetCurrentSong()
        {
            return PlayList[CurrentPlaying];
        }

    }
}
