using System;
using System.Windows;
using Microsoft.Phone.Controls;
using MathDash.ViewModel;
using MathDash.Model;
using System.Security.Cryptography;
using System.Net;
using System.IO;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using System.Windows.Controls.Primitives;
using Coding4Fun.Phone.Controls;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Generic;

namespace MathDash.View
{
    public partial class ScorePage : PhoneApplicationPage
    {
        private ScoreViewModel scoreViewModel;
        private bool AlreadyDisplayedSubmitHighScoreButton;
        private string Player;
        private int Score;

        public ScorePage()
        {
            InitializeComponent();

            AlreadyDisplayedSubmitHighScoreButton = false;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            SubmitHighScoreButton.Visibility = System.Windows.Visibility.Collapsed;

            int correct = Convert.ToInt32(NavigationContext.QueryString["Correct"]);
            int wrong = Convert.ToInt32(NavigationContext.QueryString["Wrong"]);

            scoreViewModel = new ScoreViewModel(correct, wrong);
            LayoutRoot.DataContext = scoreViewModel;

            ScoreStorage scoreStorage = new ScoreStorage();
            if (scoreViewModel.Score > scoreStorage.HighScore)
                scoreStorage.HighScore = scoreViewModel.Score;

            if (!AlreadyDisplayedSubmitHighScoreButton)
            {
                AlreadyDisplayedSubmitHighScoreButton = true;
                HttpWebRequest request = WebRequest.CreateHttp("http://mathdash.heroku.com/scores.json");
                request.Method = "GET";
                request.BeginGetResponse(new AsyncCallback(GetHighScoresResponseCallback), request);
            }
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        private void PlayAgainButton_Clicked(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/GamePage.xaml", UriKind.Relative));
        }

        private void GetHighScoresResponseCallback(IAsyncResult asyncResult)
        {
            HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asyncResult);

            ObservableCollection<HighScore> highScores = new ObservableCollection<HighScore>();
            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                JArray ja = JArray.Parse(sr.ReadToEnd());

                // Populate high scores list.
                for (int i = 0; i < ja.Count; i++)
                {
                    JObject jo = (JObject)ja[i];
                    HighScore hs = new HighScore(i + 1,
                                                 (string)jo["player"],
                                                 (int)jo["score"],
                                                 DateTime.FromFileTimeUtc(long.Parse((string)jo["ts"])));
                    highScores.Add(hs);
                }
            }


            if (scoreViewModel.Score > highScores[highScores.Count - 1].Score || highScores.Count < 20)
            {
                SubmitHighScoreButton.Dispatcher.BeginInvoke(() =>
                {
                    SubmitHighScoreButton.Visibility = System.Windows.Visibility.Visible;
                });
            }
        }

        private void SubmitHighScoreButton_Clicked(object sender, RoutedEventArgs e)
        {
            InputPrompt inputPrompt = new InputPrompt();
            inputPrompt.Title = "New Leaderboard High Score!";
            inputPrompt.Message = "Enter name";
            inputPrompt.Completed += Input_Completed;
            inputPrompt.Show();
        }

        private void Input_Completed(object sender, PopUpEventArgs<string, PopUpResult> pea)
        {
            string player = pea.Result;
            string score = scoreViewModel.Score.ToString();
            string timestamp = DateTime.UtcNow.ToFileTimeUtc().ToString();
            string secretSalt = Keys.SERVER_SALT;

            Player = player;
            Score = int.Parse(score);

            SHA256 sha256 = new SHA256Managed();
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            byte[] b = enc.GetBytes(player + score + timestamp + secretSalt);

            StringBuilder sb = new StringBuilder();
            foreach (byte by in sha256.ComputeHash(b))
            {
                sb.Append(by.ToString("X2").ToLower());
            }

            string digest = sb.ToString();

                JObject ja = new JObject(
                new JProperty("score",
                    new JObject(
                        new JProperty("player", player),
                        new JProperty("score", score),
                        new JProperty("ts", timestamp))),
                new JProperty("digest", digest));

            string json = JsonConvert.SerializeObject(ja);

            HttpWebRequest request = HttpWebRequest.CreateHttp("http://mathdash.heroku.com/scores.json");
            request.Method = "POST";
            request.ContentType = "application/json";

            Tuple<string, HttpWebRequest> tuple = new Tuple<string, HttpWebRequest>(json, request);

            request.BeginGetRequestStream(new AsyncCallback(PostScoreRequestCallback), tuple);

        }

        private void PostScoreRequestCallback(IAsyncResult asyncResult)
        {
            Tuple<string, HttpWebRequest> tuple = (Tuple<string, HttpWebRequest>)(asyncResult.AsyncState);

            string json = (string)tuple.Item1;
            HttpWebRequest request = (HttpWebRequest)tuple.Item2;
            using (Stream postStream = request.EndGetRequestStream(asyncResult))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(json);
                postStream.Write(bytes, 0, bytes.Length);
            }
            request.BeginGetResponse(new AsyncCallback(PostScoreResponseCallback), request);
        }

        private void PostScoreResponseCallback(IAsyncResult asyncResult)
        {
            HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;

            // End the operation
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.EndGetResponse(asyncResult);
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    this.Dispatcher.BeginInvoke(() =>
                        {
                            NavigationService.Navigate(new Uri("/View/Leaderboard.xaml?Player=" + Player + "&Score=" + Score, UriKind.Relative));
                        });
                }
            }
            catch (Exception) { }
        }
    }
}