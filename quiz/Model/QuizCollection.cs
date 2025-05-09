using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quiz.Model
{
    public class QuizCollection
    {
        public static QuizCollection instance;
        public static QuizCollection Instance => instance ??= new QuizCollection();

        public ObservableCollection<Quiz> Quizes { get; set; } = new ObservableCollection<Quiz>();
    }
}
