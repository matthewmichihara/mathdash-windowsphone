using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.ComponentModel;

namespace MathDash.Model
{
    public class ScoreStorage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void saveHighScore(int score)
        {
            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream file = isoStorage.OpenFile("highscore", FileMode.Create);
            TextWriter writer = new StreamWriter(file);
            writer.Write(score);
            writer.Close();
        }

        private int loadHighScore()
        {
            IsolatedStorageFile isoStorage = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream file = isoStorage.OpenFile("highscore", FileMode.OpenOrCreate);
            TextReader reader = new StreamReader(file);
            int score = 0;
            try
            {
                score = Int32.Parse(reader.ReadLine().Trim());
            }
            catch { }

            reader.Close();

            return score;
        }

        public int HighScore
        {
            get { return loadHighScore(); }
            set
            {
                saveHighScore(value);
                NotifyPropertyChanged("HighScore");
            }
        }

        private void NotifyPropertyChanged(String property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
