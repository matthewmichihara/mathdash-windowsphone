using System;
using System.IO;
using System.Net;
using Microsoft.Phone.Controls;
using Newtonsoft.Json.Linq;
using MathDash.ViewModel;
using System.Collections.ObjectModel;
using MathDash.Model;
using System.Windows.Controls;

namespace MathDash.View
{
    public partial class Leaderboard : PhoneApplicationPage
    {
        private string NewPlayer;
        private int? Score;

        public Leaderboard()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            try
            {
                NewPlayer = NavigationContext.QueryString["Player"];
                Score = int.Parse(NavigationContext.QueryString["Score"]);
            }
            catch (Exception) { }

            HttpWebRequest request = WebRequest.CreateHttp("http://mathdash.heroku.com/scores.json");
            request.Method = "GET";

            request.BeginGetResponse(new AsyncCallback(ResponseCallback), request);
        }

        private void ResponseCallback(IAsyncResult asyncResult)
        {
            HttpWebRequest request = (HttpWebRequest)asyncResult.AsyncState;
            HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asyncResult);

            using (StreamReader sr = new StreamReader(response.GetResponseStream()))
            {
                ObservableCollection<HighScore> highScores = new ObservableCollection<HighScore>();
                JArray ja = JArray.Parse(sr.ReadToEnd());

                // Populate high scores list.
                for (int i=0; i<ja.Count; i++)
                {
                    JObject jo = (JObject)ja[i];
                    HighScore hs = new HighScore(i+1,
                                                 (string)jo["player"],
                                                 (int)jo["score"],
                                                 DateTime.FromFileTimeUtc(long.Parse((string)jo["ts"])));
                    highScores.Add(hs);
                }

                LeaderboardViewModel lbvm = new LeaderboardViewModel(highScores);
                LayoutRoot.Dispatcher.BeginInvoke(new Action(() => { LayoutRoot.DataContext = lbvm; }));

                if (!string.IsNullOrEmpty(NewPlayer) && Score != null)
                {
                    for (int i = 0; i < lbvm.HighScores.Count; i++)
                    {
                        HighScore hs = lbvm.HighScores[i];
                        if (hs.Player.Equals(NewPlayer) && hs.Score == Score)
                        {
                            ScoresListBox.Dispatcher.BeginInvoke(new Action(() => { ScoresListBox.SelectedIndex = i; }));
                            break;
                        }
                    }
                }
            }
        }

        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}