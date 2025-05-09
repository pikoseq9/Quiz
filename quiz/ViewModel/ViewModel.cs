using quiz.Service;
using quiz.ViewModel.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace quiz.ViewModel
{
    using Model;
    using System.Windows.Input;
    using System.ComponentModel;
    using quiz.View;

    class ViewModel : BaseViewModel
    {
        QuizCollection quizCollection = new QuizCollection();
        private readonly NavigationService _navigationService;
        private Quiz _selectedQuiz;
        public Quiz SelectedQuiz //disi
        {
            get => _selectedQuiz;
            set
            {
                _selectedQuiz = value;
                OnPropertyChanged(nameof(SelectedQuiz));
            }
        }

        // aktualny model widoku
        private BaseViewModel _currentViewModel;
        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set { _currentViewModel = value; OnPropertyChanged("CurrentViewModel"); }
        }


        // polecenia zmiany modelu widoku
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateCreatingCommand { get; }
        public ICommand NavigateUsingCommand { get; }

        public ViewModel(NavigationService navigationService)
        {
            _navigationService = navigationService;
            _navigationService.SetNavigator(vm => CurrentViewModel = vm);

            // tworzymy obiekty modelu widoku
            BaseViewModel hvm = new HomeViewModel();
            //BaseViewModel hvm = new HomeViewModel(_navigationService);
            BaseViewModel cvm = new CreatingViewModel();
            //var navigationService = new NavigationService(); // lub z DI
            //var usingViewModel = new UsingViewModel(navigationService);
             BaseViewModel uvm = new UsingViewModel();

            // utworzenie obiektów typu RelayCommand,
            // polecenia mają za zadanie zmienić aktualny model widoku
            // polecenia zawsze można wykonać
            // jeśli parametr nie jest wykorzystywany w metodzie, wówczas w funkcji
            // lambda można wpisać _ zamiast nazwy parametru
            // = new RelayCommand(_ => _navigationService.NavigateTo(hvm), _ => true);
            NavigateHomeCommand = new RelayCommand(_ =>
            {
                if (CurrentViewModel is UsingViewModel quizVm)
                {
                    quizVm.PauseQuiz();
                }

                _navigationService.NavigateTo(hvm);
            }, _ => true);

            NavigateCreatingCommand = new RelayCommand(_ =>
            { if (CurrentViewModel is UsingViewModel quizVm)
                {
                    quizVm.PauseQuiz();
                }
                _navigationService.NavigateTo(cvm);

            }, _ => true);
            //NavigateUsingCommand = new RelayCommand(_ => _navigationService.NavigateTo(uvm), _ => true);
            //disi
            NavigateUsingCommand = new RelayCommand(
            _ =>
             {
              var uvm = new UsingViewModel();
              uvm.LoadQuiz(SelectedQuiz); 
              _navigationService.NavigateTo(uvm);
            },
            _ => SelectedQuiz != null
            );

            CurrentViewModel = hvm;
        }
    }
}
