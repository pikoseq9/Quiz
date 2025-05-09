using System;
using quiz.ViewModel.BaseClass;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Numerics;
using System.Windows.Documents;
using quiz.Model;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.IO;


namespace quiz.ViewModel
{
    public class CreatingViewModel : BaseViewModel
    {
        public string NameOfQuiz { get; set; } = "";

        private bool isQuizLoaded;
        public bool IsQuizLoaded
        {
            get => isQuizLoaded;
            set { isQuizLoaded = value; OnPropertyChanged(nameof(IsQuizLoaded)); }
        }

        private string questionText;
        public string QuestionText
        {
            get => questionText;
            set { questionText = value; OnPropertyChanged(nameof(QuestionText)); }
        }

        public ObservableCollection<string> AnswerTexts { get; set; } = new ObservableCollection<string> { "", "", "", "" };

        public ObservableCollection<bool> CorrectAnswers { get; set; } = new ObservableCollection<bool> { false, false, false, false };

        private CreateQuiz quizBuilder;
        private int currentIndex = -1;

        public ICommand SaveNameAndStart => new RelayCommand(
            p =>
            {
                if (quizBuilder != null)
                {
                    quizBuilder.quiz.Name = NameOfQuiz;
                    OnPropertyChanged(nameof(NameOfQuiz));
                }
                else
                {
                    quizBuilder = new CreateQuiz(NameOfQuiz);
                }
            }
        );
        public ICommand NextQuestion => new RelayCommand(
            p =>
            {
                if (quizBuilder == null) return;
                if (string.IsNullOrEmpty(QuestionText) || CorrectAnswers.Count(c => c) == 0 || AnswerTexts.Any(a => string.IsNullOrEmpty(a)))
                {
                    MessageBox.Show("Proszę wypełnić wszystkie pola przed dodaniem pytania.");
                    return;
                }

                if (currentIndex == quizBuilder.quiz.Questions.Count - 1)
                {
                    var question = new Question
                    {
                        Content = QuestionText,
                        Answers = new ObservableCollection<Answer>()

                    };
                    for (int i = 0; i < AnswerTexts.Count; i++)
                    {
                        question.Answers.Add(new Answer
                        {
                            Text = AnswerTexts[i],
                            IsCorrect = CorrectAnswers[i]

                        });
                    }

                    if (!quizBuilder.quiz.Questions.Any(q => q.Content == question.Content)){
                        quizBuilder.quiz.AddQuestion(question);
                        currentIndex = quizBuilder.quiz.Questions.Count - 1;
                    }
                    ClearInputs();
                }
                else if (currentIndex < quizBuilder.quiz.Questions.Count - 1)
                {

                    currentIndex++;
                    ClearInputs();
                    var nextQuestion = quizBuilder.quiz.Questions[currentIndex];
                    QuestionText = nextQuestion.Content;
                    for (int i = 0; i < nextQuestion.Answers.Count; i++)
                    {
                        if (i < AnswerTexts.Count)
                            AnswerTexts[i] = nextQuestion.Answers[i].Text;
                        if (nextQuestion.Answers[i].IsCorrect)
                            CorrectAnswers[i] = nextQuestion.Answers[i].IsCorrect;
                    }
                    OnPropertyChanged(nameof(QuestionText));
                    for (int i = 0; i < AnswerTexts.Count; i++)
                    {
                        OnPropertyChanged($"AnswerTexts[{i}]");
                    }
                    OnPropertyChanged(nameof(AnswerTexts));
                    OnPropertyChanged(nameof(CorrectAnswers));
                }
                CommandManager.InvalidateRequerySuggested();
            },
            p => quizBuilder != null && (currentIndex < quizBuilder.quiz.Questions.Count - 1 || !string.IsNullOrEmpty(QuestionText) || AnswerTexts.Any(a => !string.IsNullOrEmpty(a)))
        );

        public ICommand PreviousQuestion => new RelayCommand(
            p =>
            {
                if (quizBuilder == null || quizBuilder.quiz.Questions.Count == 0) return;

                currentIndex = 0;
                var firstQuestion = quizBuilder.quiz.Questions[currentIndex];
                QuestionText = firstQuestion.Content;

                for (int i = 0; i < firstQuestion.Answers.Count; i++)
                {
                    if (i < AnswerTexts.Count)
                        AnswerTexts[i] = firstQuestion.Answers[i].Text;

                    if (firstQuestion.Answers[i].IsCorrect)
                        CorrectAnswers[i] = firstQuestion.Answers[i].IsCorrect;
                }

                // Powiadamiamy o zmianach
                OnPropertyChanged(nameof(QuestionText));
                OnPropertyChanged(nameof(AnswerTexts));
                OnPropertyChanged(nameof(CorrectAnswers));

                CommandManager.InvalidateRequerySuggested();
            },
            p => quizBuilder != null && quizBuilder.quiz.Questions.Count > 0
        );

        public ICommand DeleteQuestionCommand => new RelayCommand(
            p =>
            {
                if (quizBuilder == null || quizBuilder.quiz.Questions.Count == 0 || currentIndex < 0 || currentIndex >= quizBuilder.quiz.Questions.Count)
                {
                    MessageBox.Show("Nie ma pytania do usunięcia.");
                    return;
                }
                var result = MessageBox.Show("Czy na pewno chcesz usunąć to pytanie?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    quizBuilder.quiz.Questions.RemoveAt(currentIndex);   

                    if (quizBuilder.quiz.Questions.Count == 0)
                    {
                        // Jeżeli usunięto ostatnie pytanie, wyczyść widok.
                        ClearInputs();
                        currentIndex = -1;
                    }
                    else
                    {
                        // Przejdź do poprzedniego pytania lub pierwszego.
                        currentIndex = Math.Max(0, currentIndex - 1);
                        var question = quizBuilder.quiz.Questions[currentIndex];
                        QuestionText = question.Content;
                        for (int i = 0; i < question.Answers.Count; i++)
                        {
                            if (i < AnswerTexts.Count)
                                AnswerTexts[i] = question.Answers[i].Text;
                            if (question.Answers[i].IsCorrect)
                                CorrectAnswers[i] = question.Answers[i].IsCorrect;
                        }
                    }

                    OnPropertyChanged(nameof(QuestionText));
                    OnPropertyChanged(nameof(AnswerTexts));
                    OnPropertyChanged(nameof(CorrectAnswers));
                }
            },
            p => quizBuilder != null && quizBuilder.quiz.Questions.Count > 0 && currentIndex >= 0
        );
        public ICommand LoadQuizCommand => new RelayCommand(
        p =>
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Quiz files (*.txt)|*.txt|All files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var encryptedContent = File.ReadAllText(openFileDialog.FileName);
                    
                    var decryptedJson = AESHelper.Decrypt(encryptedContent);

                    var loadedQuiz = System.Text.Json.JsonSerializer.Deserialize<Quiz>(decryptedJson);

                    if (loadedQuiz != null)
                    {
                        quizBuilder = new CreateQuiz(loadedQuiz.Name);
                        foreach (var question in loadedQuiz.Questions)
                        {
                            quizBuilder.quiz.AddQuestion(question);
                        }

                        NameOfQuiz = loadedQuiz.Name;
                        OnPropertyChanged(nameof(NameOfQuiz));

                        if (loadedQuiz.Questions.Count > 0)
                        {
                            var lastQuestion = loadedQuiz.Questions.Last();
                            QuestionText = lastQuestion.Content;
                            for (int i = 0; i < lastQuestion.Answers.Count; i++)
                            {
                                if (i < AnswerTexts.Count)
                                    AnswerTexts[i] = lastQuestion.Answers[i].Text;
                                if (lastQuestion.Answers[i].IsCorrect)
                                    CorrectAnswers[i] = lastQuestion.Answers[i].IsCorrect;
                            }
                            OnPropertyChanged(nameof(AnswerTexts));
                            OnPropertyChanged(nameof(CorrectAnswers));
                        }

                        MessageBox.Show($"Quiz \"{loadedQuiz.Name}\" został wczytany z {loadedQuiz.Questions.Count} pytaniami.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas wczytywania quizu: {ex.Message}");
                }
            }
        }
    );
        public ICommand SaveQuizCommand => new RelayCommand(
            p =>
            {
                try
                {
                    if (quizBuilder.quiz.Questions.Any(q => string.IsNullOrWhiteSpace(q.Content) || q.Answers.Any(a => string.IsNullOrWhiteSpace(a.Text))))
                    {
                        MessageBox.Show("Wszystkie pytania i odpowiedzi muszą być wypełnione.");
                        return;
                    }
                    var json = System.Text.Json.JsonSerializer.Serialize(quizBuilder.quiz);

                    var encrypted = AESHelper.Encrypt(json);

                    var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                    {
                        FileName = $"{quizBuilder.quiz.Name}.txt",
                        Filter = "Quiz files (*.txt)|*.txt|All files (*.*)|*.*"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        File.WriteAllText(saveFileDialog.FileName, encrypted);
                        MessageBox.Show($"Quiz zapisany do pliku: {saveFileDialog.FileName}");
                    }

                    QuizCollection.Instance.Quizes.Add(quizBuilder.quiz);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Błąd podczas zapisywania quizu: {ex.Message}");
                }
            },
            p => quizBuilder != null && quizBuilder.quiz.Questions.Count > 0 &&
                 !string.IsNullOrWhiteSpace(quizBuilder.quiz.Name)
        );
        private void ClearInputs()
        {
            QuestionText = "";
            for (int i = 0; i < AnswerTexts.Count; i++) AnswerTexts[i] = "";
            for (int i = 0; i < CorrectAnswers.Count; i++) CorrectAnswers[i] = false;
        }
    }

}
