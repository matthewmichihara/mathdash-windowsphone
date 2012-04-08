using System;
using System.Collections.Generic;
using System.Threading;
using MathDash.Model;

namespace MathDash.ViewModel
{
    public class GameViewModel : ViewModelBase
    {
        private Random random;
        private int timeRemaining;
        private Timer timer;
        private RandomEquation randomEquation;
        private int answer1;
        private int answer2;
        private int answer3;
        private int answer4;
        int correct;
        int wrong;

        public EventHandler TimeExpired;

        public GameViewModel()
        {
            TimeRemaining = 30;
            correct = wrong = 0;

            random = new Random();
            AutoResetEvent autoEvent = new AutoResetEvent(false);
            TimerCallback tcb = timerCallback;
            timer = new Timer(tcb, autoEvent, 1000, 1000);
            Next();
        }

        public void Next()
        {
            Equation = new RandomEquation();
            int answer = Equation.Answer;

            List<int> answers = new List<int>();
            answers.Add(answer);

            while (answers.Count < 4)
            {
                int randomAnswer = random.Next(Equation.MinAnswer, Equation.MaxAnswer);
                if (!answers.Contains(randomAnswer))
                    answers.Add(randomAnswer);
            }

            int index = random.Next(0, answers.Count);
            Answer1 = answers[index];
            answers.RemoveAt(index);

            index = random.Next(0, answers.Count);
            Answer2 = answers[index];
            answers.RemoveAt(index);

            index = random.Next(0, answers.Count);
            Answer3 = answers[index];
            answers.RemoveAt(index);

            index = random.Next(0, answers.Count);
            Answer4 = answers[index];
            answers.RemoveAt(index);
        }

        public int TimeRemaining
        {
            get { return timeRemaining; }
            set
            {
                timeRemaining = value;
                NotifyPropertyChanged("TimeRemaining");
            }
        }

        private void timerCallback(Object stateInfo)
        {
            System.Windows.Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                if (TimeRemaining == 0)
                {
                    DisposeTimer();
                    TimeExpired.Invoke(this, null);
                }
                else
                {
                    TimeRemaining = TimeRemaining - 1;
                }
            });
        }

        public void DisposeTimer()
        {
            timer.Dispose();
        }

        public RandomEquation Equation
        {
            get { return randomEquation; }
            set
            {
                randomEquation = value;
                NotifyPropertyChanged("Equation");
            }
        }

        public int Answer1
        {
            get { return answer1; }
            set
            {
                answer1 = value;
                NotifyPropertyChanged("Answer1");
            }
        }

        public int Answer2
        {
            get { return answer2; }
            set
            {
                answer2 = value;
                NotifyPropertyChanged("Answer2");
            }
        }

        public int Answer3
        {
            get { return answer3; }
            set
            {
                answer3 = value;
                NotifyPropertyChanged("Answer3");
            }
        }

        public int Answer4
        {
            get { return answer4; }
            set
            {
                answer4 = value;
                NotifyPropertyChanged("Answer4");
            }
        }

        public int Correct
        {
            get { return correct; }
            set
            {
                correct = value;
                NotifyPropertyChanged("Correct");
            }
        }

        public int Wrong
        {
            get { return wrong; }
            set
            {
                wrong = value;
                NotifyPropertyChanged("Wrong");
            }
        }
    }
}
