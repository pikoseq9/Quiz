# Quiz Application

## 👤 Autor

Patryk Weklicz - https://github.com/pikoseq9

Aplikacja WPF do tworzenia i rozwiązywania quizów, napisana w C# z użyciem wzorca MVVM.

## 📋 Opis

Ten projekt pozwala użytkownikowi:
- Tworzyć własne quizy z pytaniami i odpowiedziami.
- Przeglądać zapisane quizy.
- Rozwiązywać quizy w przyjaznym interfejsie i wyświetlanie otrzymanego wyniku i zestawienia odpowiedzi użytkownika do poprawnych.
- Zarządzać kolekcją quizów (zapis, odczyt).
- Eksport i import quizów z zaszyfrowanych algorytmem AES plików .txt.

Projekt wykorzystuje:
- **WPF** z XAML.
- **MVVM** (Model-View-ViewModel).
- Szyfrowanie danych (AESHelper).

## 🛠️ Struktura projektu

- **Model/** → logika danych (Quiz, Question, Answer, QuizCollection).
- **View/** → widoki XAML (HomeView, CreatingView, UsingView).
- **ViewModel/** → logika widoków, komendy (RelayCommand, BaseViewModel).
- **Service/** → nawigacja między widokami.

## 🚀 Jak uruchomić

1️⃣ Sklonuj repozytorium oraz uruchom solucję quiz.sln:
```bash
git clone https://github.com/TwojaNazwaUżytkownika/quiz.git
```
2️⃣ Otwórz projekt w Visual Studio.

3️⃣ Ustaw projekt jako startowy (kliknij prawym na projekt → „Set as Startup Project”).

4️⃣ Kliknij Start lub naciśnij F5.

## 💾 Wymagania

- Visual Studio 2022 (lub nowsze).

- .NET 6 (lub kompatybilna wersja frameworka).

- Windows (ze względu na WPF).
