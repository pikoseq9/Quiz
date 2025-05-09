using quiz.Model;
using quiz.ViewModel.BaseClass;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using quiz.Service;
using System.ComponentModel;
using System.Windows.Navigation;
using Microsoft.Win32;
using System.IO;
using System.Windows.Threading;


namespace quiz.ViewModel
{
    public class UsingViewModel : BaseViewModel
    {

        private Quiz _currentQuiz;
        private int _currentQuestionIndex = 0;
        private int _score = 0;
        private DispatcherTimer _timer;
        private TimeSpan _timeLeft;
        private bool _isQuizActive = true;

        public bool IsQuizActive
        {
            get => _isQuizActive;
            set
            {
                _isQuizActive = value;
                OnPropertyChanged(nameof(IsQuizActive));
            }
        }
        public string TimeLeftDisplay => _timeLeft.ToString(@"mm\:ss");
        public ObservableCollection<Quiz> Quizes => QuizCollection.Instance.Quizes;
        private Quiz _selectedQuiz;
        public Quiz SelectedQuiz
        {
            get => _selectedQuiz;
            set
            {
                _selectedQuiz = value;
                OnPropertyChanged(nameof(SelectedQuiz));
            }
        }
        public Quiz CurrentQuiz
        {
            get => _currentQuiz;
            set
            {
                _currentQuiz = value;
                OnPropertyChanged(nameof(CurrentQuiz));
                OnPropertyChanged(nameof(CurrentQuestion));
            }
        }
        public Question CurrentQuestion
        {
            get
            {
                if (CurrentQuiz != null && CurrentQuiz.Questions.Count > 0 && _currentQuestionIndex < CurrentQuiz.Questions.Count)
                    return CurrentQuiz.Questions[_currentQuestionIndex];
                return null;
            }
        }

        public void LoadQuiz(Quiz quiz)
        {
            _timer?.Stop();
            if (quiz != null)
            {
                _currentQuestionIndex = 0;
                _score = 0;
                CurrentQuiz = CreateDeepCopyOfQuiz(quiz);

                StartTimer(TimeSpan.FromMinutes(1)); // uruchom licznik
                IsQuizActive = true;

                MessageBox.Show($"Załadowano quiz: {CurrentQuiz.Name} z {CurrentQuiz.Questions.Count} pytaniami!");
            }
        }
        private void StartTimer(TimeSpan duration)
        {
            _timeLeft = duration;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            _timer.Tick += (s, e) =>
            {
                _timeLeft = _timeLeft.Subtract(TimeSpan.FromSeconds(1));
                OnPropertyChanged(nameof(TimeLeftDisplay));

                if (_timeLeft.TotalSeconds <= 0)
                {
                    _timer.Stop();
                    IsQuizActive = false;
                    MessageBox.Show("Czas minął! Quiz zostaje zakończony.");
                    CompleteQuiz(); // <--- Dodane tutaj
                }
            };

            _timer.Start();
        }

        //kopiowania quizu
        private Quiz CreateDeepCopyOfQuiz(Quiz original)
        {
            if (original == null) return null;

            Quiz copy = new Quiz
            {
                Name = original.Name,
                Questions = new ObservableCollection<Question>()
            };

            foreach (var question in original.Questions)
            {
                Question newQuestion = new Question
                {
                    Content = question.Content,
                    Answers = new ObservableCollection<Answer>()
                };

                foreach (var answer in question.Answers)
                {
                    newQuestion.Answers.Add(new Answer
                    {
                        Text = answer.Text,
                        IsCorrect = answer.IsCorrect
                    });
                }

                copy.Questions.Add(newQuestion);
            }

            return copy;
        }

        ///NOWE ZMIANY PO ZAŁADOWANIU
        public ICommand NextQuestion => new RelayCommand(p =>
        {
            if (!IsQuizActive) return;
            if (CurrentQuiz == null || CurrentQuestion == null) return;

            CurrentQuestion.WasVisited = true;
            _currentQuestionIndex++;

            if (_currentQuestionIndex < CurrentQuiz.Questions.Count)
            {
                OnPropertyChanged(nameof(CurrentQuestion));
            }
            else
            {
                MessageBox.Show("Koniec quizu!");
            }
        });

        public ICommand PreviousQuestion => new RelayCommand(p =>
        {
            if (!IsQuizActive) return;
            if (CurrentQuiz == null || _currentQuestionIndex == 0) return;

            _currentQuestionIndex--;
            OnPropertyChanged(nameof(CurrentQuestion));
        });


        private void CompleteQuiz()
        {
            if (CurrentQuiz == null) return;

            // Sprawdź, czy na wszystkie pytania udzielono odpowiedzi
            var unanswered = CurrentQuiz.Questions.Any(q => !q.Answers.Any(a => a.IsUserSelected));
            if (IsQuizActive && unanswered)
            {
                MessageBox.Show("Nie odpowiedziano na wszystkie pytania. Proszę uzupełnić brakujące odpowiedzi.");
                return;
            }

            // Oblicz wynik
            _score = 0;
            for (int i = 0; i < CurrentQuiz.Questions.Count; i++)
            {
                var question = CurrentQuiz.Questions[i];
                bool allCorrect = question.Answers.All(a => a.IsCorrect == a.IsUserSelected);
                if (allCorrect) //Punkt tylko, jeśli na wszystkie odpowiedziano wszystkimi dobrymi c:
                {
                    _score++;
                }
            }

            // Pokaż wynik użytkownikowi
            MessageBox.Show($"Quiz zakończony!\nTwój wynik: {_score}/{CurrentQuiz.Questions.Count}");

            // Zapisz wynik do pliku
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Pliki tekstowe (*.txt)|*.txt",
                FileName = $"{CurrentQuiz.Name}_wynik.txt"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                {
                    // Zapisujemy nagłówek quizu
                    writer.WriteLine($"Quiz: {CurrentQuiz.Name}");
                    writer.WriteLine($"Wynik: {_score}/{CurrentQuiz.Questions.Count}\n");

                    // Zmienna pomocnicza do numeracji pytań
                    int questionNumber = 1;

                    for (int i = 0; i < CurrentQuiz.Questions.Count; i++)
                    {
                        var question = CurrentQuiz.Questions[i];
                        writer.WriteLine($"Pytanie {questionNumber}: {question.Content}");
                        for (int j = 0; j < question.Answers.Count; j++)
                        {
                            var answer = question.Answers[j];
                            string prefix = answer.IsCorrect ? "[✔]" : "[ ]";

                            if (answer.IsUserSelected && !answer.IsCorrect)
                                prefix = "[✘]";
                            else if (answer.IsUserSelected && answer.IsCorrect)
                                prefix = "[✔*]";

                            writer.WriteLine($"{prefix} {answer.Text}");
                        }

                        writer.WriteLine();
                        questionNumber++;
                    }
                }
            }

            // Zatrzymaj timer i wyczyść quiz
            _timer?.Stop();
            IsQuizActive = false;
            _currentQuestionIndex = 0;
            CurrentQuiz = null;
            
            OnPropertyChanged(nameof(CurrentQuestion));
        }
        public ICommand FinishQuiz => new RelayCommand(
        p =>
        {
            if (!IsQuizActive)
            {
                MessageBox.Show("Czas minął – nie możesz już rozwiązywać quizu.");
                return;
            }

            CompleteQuiz();
        },
        p => CurrentQuiz != null
        );
        public void PauseQuiz()
        {
            _timer?.Stop();
            IsQuizActive = false;
        }

    }
}
