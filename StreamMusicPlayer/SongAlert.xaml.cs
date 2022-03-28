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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace StreamMusicPlayer
{
    public partial class SongAlert : Window
    {
        MainWindow mainWindow;

        private ThicknessAnimation borderAnimationAppear;
        private DoubleAnimation coverAnimationAppear;
        private DoubleAnimation layerAnimationAppear;
        private DoubleAnimation layerAnimationDisappear;

        private bool beginAnimationIsEnded;
        public bool BindingState { get; set; } 

        public SongAlert(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;

            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
            this.Background = Brushes.Transparent;

            //https://www.youtube.com/watch?v=TAJ7ZXdfCv8
            //https://professorweb.ru/my/WPF/binding_and_styles_WPF/level11/11_8.php

            SetAnimation();
            Show();
            //RedrawWindow();
        }
        private void UpdateCoverImg()
        {
            //  Update cover image in thi main window;
            using (var file = TagLib.File.Create(@"C:\" + mainWindow.music.GetCurrentSong()))
            {
                TagLib.IPicture pic = file.Tag.Pictures[0];
                MemoryStream ms = new MemoryStream(pic.Data.Data);
                ms.Seek(0, SeekOrigin.Begin);

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.EndInit();

                Image.Source = bitmap;

                tagGroupLbl.Content = null;
               // StringBuilder performers = new StringBuilder();
                foreach(string performer in file.Tag.Performers)
                    tagGroupLbl.Content += performer;
                tagAlbomLbl.Content = file.Tag.Album;
                tagNameLbl.Content = file.Tag.Title;
                
            }

        }

        private void SetAnimation()
        {   
            // Set appear animation
            borderAnimationAppear = new ThicknessAnimation();
            borderAnimationAppear.From = new Thickness(-20, 25, 20, 25);
            borderAnimationAppear.To = new Thickness(-40, 25, 40, 25);
            borderAnimationAppear.Duration = TimeSpan.FromSeconds(2);

            coverAnimationAppear = new DoubleAnimation();
            coverAnimationAppear.From = 160;
            coverAnimationAppear.To = 180;
            coverAnimationAppear.Duration = TimeSpan.FromSeconds(2);

            layerAnimationAppear = new DoubleAnimation();
            layerAnimationAppear.From = 0;
            layerAnimationAppear.To = 1;
            layerAnimationAppear.Duration = TimeSpan.FromSeconds(1);
            
            coverAnimationAppear.Completed += async (s, e) =>
            {
                if (!beginAnimationIsEnded)
                {
                    beginAnimationIsEnded = true;
                    Console.WriteLine("Animation completed.");
                    Console.WriteLine("{Thread.Sleep(2000);} - Start.");
                    await Task.Delay(2000);
                    Console.WriteLine("{Thread.Sleep(2000);} - Done.");
                    AnimationDisappear();
                }
            };

            // Set disappear animation
            layerAnimationDisappear = new DoubleAnimation();
            layerAnimationDisappear.From = 1;
            layerAnimationDisappear.To = 0;
            layerAnimationDisappear.Duration = TimeSpan.FromSeconds(1);
        } 

        private void AnimationAppear()
        {
            Show();
            beginAnimationIsEnded = false;
            songAlert.BeginAnimation(OpacityProperty, layerAnimationAppear);
            Image.BeginAnimation(WidthProperty, coverAnimationAppear);
            Image.BeginAnimation(HeightProperty, coverAnimationAppear);
            BorderData.BeginAnimation(MarginProperty, borderAnimationAppear);   
        }

        public void AnimationDisappear()
        {
            songAlert.BeginAnimation(OpacityProperty, layerAnimationDisappear);
            //Image.BeginAnimation(WidthProperty, coverAnimationDisappear);
            //Image.BeginAnimation(HeightProperty, coverAnimationDisappear);
            //BorderData.BeginAnimation(MarginProperty, borderAnimationDisappear);
        }
    
        public void RedrawWindow()
        {
            UpdateCoverImg();
            AnimationAppear();     
        }

        private void Mouse_Click_DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (BindingState == false)
            {
                this.DragMove();
            }
        }
    }
}
