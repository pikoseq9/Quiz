using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace quiz.Model
{
    class CreateQuiz
    {
        public Quiz quiz { get; } = new Quiz();

        public CreateQuiz(string quizName)
        {
            quiz.Name = quizName;

        }

    }
}
