using System;
using System.Collections.Generic;

namespace MathDash.Model
{
    public class RandomEquation
    {
        private static List<Char> operators = new List<Char> { '+', '-', '*', '/' };
        private int minOperand = 0;
        private int maxOperand = 10;
        private int a;
        private int b;
        private char op;

        public RandomEquation()
        {
            do
            {
                initialize();
            } while (op == '/' && (b == 0 || a % b != 0));
        }

        private void initialize()
        {
            Random random = new Random();
            a = random.Next(minOperand, maxOperand);
            b = random.Next(minOperand, maxOperand);
            op = operators[random.Next(0, operators.Count)];
        }

        public override string ToString()
        {
            return a + " " + op + " " + b;
        }

        public int Answer
        {
            get
            {
                int answer = 0;
                switch (op)
                {
                    case '+':
                        answer = a + b;
                        break;
                    case '-':
                        answer = a - b;
                        break;
                    case '*':
                        answer = a * b;
                        break;
                    case '/':
                        answer = a / b;
                        break;
                }
                return answer;
            }
        }

        public int MaxAnswer
        {
            get
            {
                if (op == '+')
                    return maxOperand + maxOperand;
                if (op == '-')
                    return maxOperand - minOperand;
                if (op == '*')
                    return maxOperand * maxOperand;

                return maxOperand;
            }
        }

        public int MinAnswer
        {
            get
            {
                if (op == '+')
                    return minOperand + minOperand;
                if (op == '-')
                    return minOperand - maxOperand;
                if (op == '*')
                    return minOperand * maxOperand;

                return minOperand;
            }
        }
    }
}
