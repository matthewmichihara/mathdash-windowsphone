using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using MathDash.ViewModel;

namespace MathDash.View
{
    public partial class GamePage : PhoneApplicationPage
    {
        private GameViewModel gameViewModel;
        private bool gameInProgress;

        // Constructor
        public GamePage()
        {
            InitializeComponent();
            gameInProgress = true;

            gameViewModel = new GameViewModel();
            gameViewModel.TimeExpired = this.ViewScore;
            LayoutRoot.DataContext = gameViewModel;
        }

        private void Answer_Click(object sender, RoutedEventArgs e)
        {
            int answer = (int)((Button)sender).Content;
            if (answer == gameViewModel.Equation.Answer)
            {
                gameViewModel.Correct += 1;
            }
            else
            {
                gameViewModel.Wrong += 1;
            }
            gameViewModel.Next();
        }

        private void ViewScore(object sender, EventArgs e)
        {
            this.gameInProgress = false;
            this.Answer1.IsEnabled = false;
            this.Answer2.IsEnabled = false;
            this.Answer3.IsEnabled = false;
            this.Answer4.IsEnabled = false;

            String navigationString = "/View/ScorePage.xaml";
            navigationString += "?Correct=" + gameViewModel.Correct;
            navigationString += "&Wrong=" + gameViewModel.Wrong;
            NavigationService.Navigate(new Uri(navigationString, UriKind.Relative));
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!this.gameInProgress)
            {
                NavigationService.GoBack();
            }
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            gameViewModel.DisposeTimer();
        }
    }




}