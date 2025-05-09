using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace quiz.Model
{
    public class Quiz
    {
        public string Name { get; set; }
        public ObservableCollection<Question> Questions { get; set; } = new ObservableCollection<Question>();

        public void AddQuestion(Question question)
        {
            Questions.Add(question);
           
        }


    }
}
