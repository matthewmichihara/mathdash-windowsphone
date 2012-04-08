using System;

namespace MathDash.ViewModel
{
    public class ScoreViewModel : ViewModelBase
    {
        private int correct;
        private int wrong;

        public ScoreViewModel(int correct, int wrong)
        {
            this.correct = correct;
            this.wrong = wrong;
        }

        public int Correct
        {
            get { return correct; }
        }

        public int Wrong
        {
            get { return wrong; }
        }

        public int Total
        {
            get { return Correct + Wrong; }
        }

        public double Accuracy
        {
            get
            {
                double accuracy = 0.0;

                if (correct == 0 && wrong == 0)
                    return accuracy;

                accuracy = ((double)correct) / ((double)(correct + wrong));
                return accuracy;
            }
        }

        public String Rank
        {
            get
            {
                if (Score > 60)
                    return "GENIUS";
                if (Score > 50)
                    return "Brilliant";
                if (Score > 40)
                    return "Intelligent";
                if (Score > 30)
                    return "Average";

                return "Below Average";
            }
        }

        public String Grade
        {
            get
            {
                if (Score > 60)
                    return "A+";
                if (Score > 50)
                    return "A";
                if (Score > 40)
                    return "B";
                if (Score > 30)
                    return "C";
                if (Score > 20)
                    return "D";

                return "F";
            }
        }

        public int Score
        {
            get { return (int)(Correct * Accuracy); }
        }
    }
}
