using System;
using System.Windows;
using Microsoft.Phone.Controls;
using MathDash.Model;

namespace MathDash.View
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Score.Text = string.Format("High Score: {0}", (new ScoreStorage()).HighScore);
        }

        private void LeaderboardButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/Leaderboard.xaml", UriKind.Relative));
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/GamePage.xaml", UriKind.Relative));
        }
    }
}