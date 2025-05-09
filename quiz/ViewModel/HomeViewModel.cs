using System;
using quiz.ViewModel.BaseClass;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using quiz.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;
using quiz.Service;

namespace quiz.ViewModel
{
    public class HomeViewModel : BaseViewModel
    {
        public ObservableCollection<Quiz> Quizes => QuizCollection.Instance.Quizes;

        //disi zmiany
       

    }
}
