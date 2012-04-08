using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MathDash.Model
{
    public class HighScore
    {
        public HighScore(int rank, string player, int score, DateTime date)
        {
            Rank = rank;
            Player = player;
            Score = score;
            Date = date;
        }

        public int Rank { get; set; }
        public string Player { get; set; }
        public int Score { get; set; }
        public DateTime Date { get; set; }
    }
}
