using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quiz.Model
{
    public class Question
    {
        public string Content {  get; set; }

        public bool WasVisited { get; set; } = false; //disi
        public ObservableCollection<Answer> Answers { get; set; } = new ObservableCollection<Answer>();

        public void AddAnswer(Answer answer)
        {
            Answers.Add(answer); 
        }
    }
}
