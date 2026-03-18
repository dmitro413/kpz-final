# Сапер

Настільна гра "Сапер" реалізована на **WPF (.NET)** з архітектурою **MVVM**, системою рекордів, статистикою, досягненнями, історією ігор та окремим режимом Time Attack.

---

## Зміст

1. Інтерфейс користувача (UI)
2. Ігрова логіка
3. Збереження даних
4. Система досягнень
5. Рівні складності
6. Архітектура та патерни
7. Структура проекту
8. Programming Principles
9. Design Patterns
10. Refactoring Techniques
11. Запуск локально

---

## 1. Інтерфейс користувача (UI)

### Навігаційний хедер
- Кнопки переходу між екранами: **Game**, **Time Attack**, **History**, **Leaderboard**, **Achievements**, **Statistics**, **Settings**
- Кнопка **New Game** — починає нову гру з поточними налаштуваннями
- Визначено у [`MainWindow.xaml`](KPZ-FINAL/Views/MainWindow.xaml)

### Ігровий екран ([`GameView.xaml`](KPZ-FINAL/Views/GameView.xaml))
- Відображення кількості мін що залишились та таймеру
- Смайлик-кнопка для швидкого перезапуску гри
- Ігрове поле з клітинками: лівий клік — відкрити, правий клік — прапорець
- Банер перемоги з полем для введення імені та кнопкою збереження рекорду
- Банер програшу з кнопкою нової гри

### Екран Time Attack ([`TimeAttackView.xaml`](KPZ-FINAL/Views/TimeAttackView.xaml))
- Хедер з поточним рахунком, зворотнім відліком та кількістю мін
- Таймер підсвічується червоним при ≤ 10 секундах
- Flash-повідомлення про штраф після потрапляння на міну
- Кнопка **Restart** для повернення до початкового стану
- Банер старту та банер фінального рахунку

### Екран рекордів ([`LeaderboardView.xaml`](KPZ-FINAL/Views/LeaderboardView.xaml))
- Таблиця рекордів з ім'ям гравця, часом, складністю та датою
- Фільтрація за рівнем складності
- Кнопка очищення рекордів

### Екран історії ігор ([`GameHistoryView.xaml`](KPZ-FINAL/Views/GameHistoryView.xaml))
- Останні 20 ігор з кольоровим маркуванням (зелений — перемога, жовтий — поразка)
- Підсумок: загальна кількість, перемоги, поразки
- Кнопка очищення історії

### Екран досягнень ([`AchievementsView.xaml`](KPZ-FINAL/Views/AchievementsView.xaml))
- Прогрес-бар розблокованих досягнень
- Список усіх досягнень з іконкою, назвою, описом та датою розблокування
- Жовтий банер при розблокуванні нових досягнень
- Заблоковані досягнення відображаються напівпрозорими

### Екран статистики ([`StatisticsView.xaml`](KPZ-FINAL/Views/StatisticsView.xaml))
- Загальна статистика: ігри, перемоги, поразки, відсоток перемог, середній час
- Детальна статистика по кожному рівню складності з фільтром
- Кнопка скидання статистики

### Екран налаштувань ([`SettingsView.xaml`](KPZ-FINAL/Views/SettingsView.xaml))
- Вибір складності зі спадного списку
- Налаштування розміру поля для режиму Custom (з валідацією)
- Вибір теми (Light / Dark)
- Слайдер розміру клітинок (24–48 px)

---

## 2. Ігрова логіка

### Генерація мін ([`RandomMineGenerator.cs`](WpfLibrary/services/RandomMineGenerator.cs))
- Міни розміщуються лише після першого кліку
- Безпечна зона — весь блок 3×3 навколо першого кліку (перший клік завжди безпечний)
- Підрахунок сусідніх мін для кожної клітинки

### Ігровий сервіс ([`GameService.cs`](WpfLibrary/services/GameService.cs))
- Відкриття клітинки з каскадним розкриттям (`FloodReveal`) для порожніх зон
- Встановлення / зняття прапорця (не більше ніж кількість мін)
- Перевірка умови перемоги та програшу
- Події: `GameWon`, `GameLost`, `BoardChanged`

### Режим Time Attack ([`TimeAttackService.cs`](WpfLibrary/services/TimeAttackService.cs))
- 60 секунд на максимальний рахунок
- Потрапляння на міну: штраф = очки за поточну дошку + 10, нова дошка, таймер не зупиняється
- Окрема подія `BoardReset` для повної заміни дошки

### Таймер ([`TimerService.cs`](WpfLibrary/services/TimerService.cs))
- `DispatcherTimer` з точністю 1 секунда
- Методи `Start`, `Stop`, `Reset`
- Подія `OnTick` для підписки ViewModel

---

## 3. Збереження даних

Усі дані зберігаються локально у JSON файлах:

| Файл | Сервіс | Опис |
|------|--------|------|
| `records.json` | [`JsonRecordRepository.cs`](WpfLibrary/services/JsonRecordRepository.cs) | Рекорди гравців |
| `statistics.json` | [`JsonStatisticsService.cs`](WpfLibrary/services/JsonStatisticsService.cs) | Статистика по складностях |
| `achievements.json` | [`AchievementService.cs`](WpfLibrary/services/AchievementService.cs) | Розблоковані досягнення |
| `history.json` | [`JsonGameHistoryService.cs`](WpfLibrary/services/JsonGameHistoryService.cs) | Остання 20 ігор |
| `settings.json` | [`JsonSettingsService.cs`](WpfLibrary/services/JsonSettingsService.cs) | Налаштування гравця |

- Автоматичне завантаження при запуску
- При пошкодженому файлі — автоматичне відновлення з дефолтних значень
- Скидання даних через відповідні екрани (Statistics, Achievements, History, Leaderboard)

---

## 4. Система досягнень

9 досягнень що розблоковуються автоматично після кожної гри ([`AchievementService.cs`](WpfLibrary/services/AchievementService.cs)):

| Назва | Умова |
|-------|-------|
| First Blood | Перша перемога |
| Flagless | Перемога без прапорців |
| Speed Demon | Перемога менш ніж за 30 секунд |
| Quick Fingers | Перемога менш ніж за 60 секунд |
| Veteran | 10 перемог загалом |
| Legend | 50 перемог загалом |
| Fearless | Перемога на рівні Hard |
| Baby Steps | Перемога на Ultra Easy |
| Survivor | 5 перемог поспіль |

---

## 5. Рівні складності

Визначені у [`Difficulty.cs`](WpfLibrary/models/Difficulty.cs):

| Рівень | Поле | Міни |
|--------|------|------|
| Ultra Easy | 5×5 | 1 |
| Easy | 9×9 | 10 |
| Medium | 16×16 | 40 |
| Hard | 16×30 | 99 |
| Custom | До вибору | До вибору |

---

## 6. Архітектура та патерни (коротка версія)

### Патерни
- **MVVM** — повне розділення View / ViewModel / Model. [`BaseViewModel.cs`](WpfLibrary/viewmodels/BaseViewModel.cs) реалізує `INotifyPropertyChanged`
- **Observer** — події `GameWon`, `GameLost`, `BoardChanged`, `SettingsSaved` між шарами
- **Strategy** — [`IMineGenerator`](WpfLibrary/services/IMineGenerator.cs) та [`IGameService`](WpfLibrary/services/IGameService.cs) дозволяють підмінювати логіку (звичайна гра vs Time Attack)
- **Repository** — [`IRecordRepository`](WpfLibrary/services/IRecordRepository.cs), [`IStatisticsService`](WpfLibrary/services/IStatisticsService.cs), [`IGameHistoryService`](WpfLibrary/services/IGameHistoryService.cs) абстрагують збереження

### Принципи SOLID
- **SRP** — кожен клас має одну відповідальність (`GameService` — логіка гри, `TimerService` — час, `JsonRecordRepository` — збереження рекордів)
- **OCP** — нову складність або досягнення можна додати без зміни існуючого коду
- **DIP** — усі залежності через інтерфейси. Конкретні класи створюються лише у [`MainWindow.cs`](KPZ-FINAL/Views/MainWindow.xaml.cs) (Composition Root)
- **DRY** — спільна логіка у `BaseViewModel`, `RelayCommand`, сервісах
- **KISS** — Прості моделі без зайвої логіки `GameHistoryEntry`, `Cell`, сервісах

---

## 7. Структура проекту

```
Solution
├── KPZ-FINAL/                  # WPF застосунок
│   ├── Views/                  # UserControl файли та головне вікно
│   │   ├── MainWindow.xaml     # Головне вікно, навігація
│   │   └── MainWindow.cs       # Composition Root
│   └── App.xaml                # Глобальні стилі та ресурси
└── WpfLibrary/                 # Бібліотека логіки
    ├── models/                 # Моделі даних
    ├── services/               # Сервіси та інтерфейси
    ├── viewmodels/             # ViewModels
    └── Converters/             # XAML конвертери
```

---

## 8. Programming Principles

### SRP — Single Responsibility Principle
Кожен клас має одну чітко визначену відповідальність.
- [`GameService.cs`](WpfLibrary/services/GameService.cs) — виключно ігрова логіка (відкриття клітинок, перевірка умов перемоги)
- [`TimerService.cs`](WpfLibrary/services/TimerService.cs) — виключно відлік часу
- [`JsonRecordRepository.cs`](WpfLibrary/services/JsonRecordRepository.cs) — виключно читання/запис рекордів
- [`CellViewModel.cs`](WpfLibrary/viewmodels/CellViewModel.cs) — виключно стан однієї клітинки для UI

### OCP — Open/Closed Principle
Код відкритий для розширення, закритий для змін.
- Нову складність можна додати в [`Difficulty.cs`](WpfLibrary/models/Difficulty.cs) без зміни `GameService`
- Нове досягнення — один новий рядок у `BuildDefaultAchievements()` в [`AchievementService.cs`](WpfLibrary/services/AchievementService.cs)
- Новий спосіб генерації мін — новий клас що реалізує [`IMineGenerator`](WpfLibrary/services/IMineGenerator.cs)

### DIP — Dependency Inversion Principle
Залежності через абстракції, не через конкретні класи.
- [`GameViewModel.cs`](WpfLibrary/viewmodels/GameViewModel.cs) залежить від `IGameService` та `ITimerService`
- [`SettingsViewModel.cs`](WpfLibrary/viewmodels/SettingsViewModel.cs) залежить від `ISettingsService`
- Конкретні реалізації створюються лише в [`MainWindow.cs`](KPZ-FINAL/Views/MainWindow.xaml.cs) (Composition Root)

### DRY — Don't Repeat Yourself
Спільна логіка винесена в одне місце.
- [`BaseViewModel.cs`](WpfLibrary/viewmodels/BaseViewModel.cs) — `OnPropertyChanged` та `SetProperty` для всіх ViewModel
- [`RelayCommand.cs`](WpfLibrary/Commands/RelayCommand.cs) — реалізація `ICommand` для всіх команд
- [`CellBackgroundConverter.cs`](WpfLibrary/Converters/CellBackgroundConverter.cs) — логіка кольору клітинки в одному місці, використовується в `GameView` та `TimeAttackView`

### KISS — Keep It Simple, Stupid
Прості моделі без зайвої логіки.
- [`GameHistoryEntry.cs`](WpfLibrary/models/GameHistoryEntry.cs) — простий клас з даними, тільки обчислювані властивості для форматування
- [`DifficultyStats.cs`](WpfLibrary/models/GameStatistics.cs) — мінімальний клас з методами `RecordWin` та `RecordLoss`
- [`Cell.cs`](WpfLibrary/models/Cell.cs) — зберігає лише необхідний стан клітинки
---

## 9. Design Patterns

### MVVM — Model-View-ViewModel
Основний архітектурний патерн всього застосунку.

**Де використано:** усі ViewModel та View файли.
- **Model** — [`Cell.cs`](WpfLibrary/models/Cell.cs), [`GameRecord.cs`](WpfLibrary/models/GameRecord.cs), [`Achievement.cs`](WpfLibrary/models/Achievement.cs) — чисті дані без залежностей від UI
- **ViewModel** — [`GameViewModel.cs`](WpfLibrary/viewmodels/GameViewModel.cs), [`StatisticsViewModel.cs`](WpfLibrary/viewmodels/StatisticsViewModel.cs) та інші — надають дані для UI через `INotifyPropertyChanged`
- **View** — XAML файли розмітка

**Навіщо:** повне розділення UI та логіки. View можна повністю замінити без зміни ViewModel.

---

### Observer — Спостерігач
Сповіщення між компонентами через події без прямих залежностей.

**Де використано:** [`GameService.cs`](WpfLibrary/services/GameService.cs), [`GameViewModel.cs`](WpfLibrary/viewmodels/GameViewModel.cs), [`MainViewModel.cs`](WpfLibrary/viewmodels/MainViewModel.cs)

```csharp
// GameService оголошує події
public event Action GameWon;
public event Action GameLost;
public event Action BoardChanged;

// GameViewModel підписується
_gameService.GameWon    += OnGameWon;
_gameService.GameLost   += OnGameLost;
_gameService.BoardChanged += RefreshAllCells;

// MainViewModel підписується на GameViewModel
GameViewModel.GameWon  += OnGameWon;
GameViewModel.GameLost += OnGameLost;
```

**Навіщо:** `GameService` не знає про існування ViewModel. Компоненти слабо зв'язані між собою.

---

### Strategy — Стратегія
Взаємозамінні реалізації через спільний інтерфейс.

**Де використано:** [`IGameService.cs`](WpfLibrary/services/IGameService.cs), [`GameService.cs`](WpfLibrary/services/GameService.cs), [`TimeAttackService.cs`](WpfLibrary/services/TimeAttackService.cs), [`IMineGenerator.cs`](WpfLibrary/services/IMineGenerator.cs)

```csharp
// Один інтерфейс — дві різні реалізації
public interface IGameService { ... }

public class GameService      : IGameService { /* звичайна гра */ }
public class TimeAttackService : IGameService { /* time attack */ }
```

**Навіщо:** `GameViewModel` та `TimeAttackViewModel` не знають про деталі реалізації. Режим Time Attack додано без жодних змін у базовій логіці.

---

### Repository — Репозиторій
Абстракція над збереженням даних.

**Де використано:** [`IRecordRepository.cs`](WpfLibrary/services/IRecordRepository.cs), [`IStatisticsService.cs`](WpfLibrary/services/IStatisticsService.cs), [`IGameHistoryService.cs`](WpfLibrary/services/IGameHistoryService.cs)

```csharp
public interface IRecordRepository
{
    IReadOnlyList<GameRecord> GetByDifficulty(DifficultyLevel difficulty);
    void Save(GameRecord record);
    void Clear();
}

// Реалізація — JSON файл
public class JsonRecordRepository : IRecordRepository { ... }
```

**Навіщо:** решта коду не знає як саме зберігаються дані. Реалізацію можна замінити з JSON на SQLite без зміни ViewModel.

---

## 10. Refactoring Techniques

### Extract Interface
Виділення інтерфейсів з конкретних класів для зменшення зв'язності.

**Приклад:** з `GameService` виділено [`IGameService`](WpfLibrary/services/IGameService.cs), з `TimerService` — [`ITimerService`](WpfLibrary/services/ITimerService.cs), з `JsonSettingsService` — [`ISettingsService`](WpfLibrary/services/ISettingsService.cs). `GameViewModel` тепер залежить від інтерфейсів, що дозволило додати `TimeAttackService` без зміни ViewModel.

---

### Extract Method
Виділення повторюваних блоків коду в окремі методи.

**Приклад:** у [`GameService.cs`](WpfLibrary/services/GameService.cs) логіка каскадного розкриття виділена в `FloodReveal()`, підрахунок мін — в `CheckWinCondition()`, відкриття всіх мін — в `RevealAllMines()`. У [`AchievementService.cs`](WpfLibrary/services/AchievementService.cs) перевірка кожної умови виділена в `TryUnlock()`.

---

### Replace Magic Number with Constant
Заміна числових констант на іменовані.

**Приклад:** у [`TimeAttackService.cs`](WpfLibrary/services/TimeAttackService.cs):
```csharp
// До рефакторингу
Score = Math.Max(0, Score - (RevealedThisRound + 5));

// Після
public const int RoundDuration = 60;
public const int BasePenalty   = 10;
Score = Math.Max(0, Score - (RevealedThisRound + BasePenalty));
```

---

### Introduce Parameter Object
Групування пов'язаних параметрів в окремий об'єкт.

**Приклад:** замість передачі окремих `rows`, `columns`, `mineCount` по всьому коду створено клас [`Difficulty.cs`](WpfLibrary/models/Difficulty.cs) який інкапсулює всі параметри складності. Аналогічно [`GameSettings.cs`](WpfLibrary/models/GameSettings.cs) групує всі налаштування гравця.

---

### Separate Query from Modifier
Методи або повертають дані, або змінюють стан — не одночасно.

**Приклад:** у [`JsonRecordRepository.cs`](WpfLibrary/services/JsonRecordRepository.cs) `GetByDifficulty()` тільки читає, `Save()` тільки записує. У [`DifficultyStats.cs`](WpfLibrary/models/GameStatistics.cs) `RecordWin()` і `RecordLoss()` змінюють стан, а `FormattedBestTime` і `WinRate` тільки повертають обчислені значення.

---

### Replace Conditional with Polymorphism
Заміна умовних конструкцій на поліморфізм.

**Приклад:** замість перевірки типу режиму гри (`if (isTimeAttack) ... else ...`) у `GameViewModel` — два окремих класи [`GameService`](WpfLibrary/services/GameService.cs) та [`TimeAttackService`](WpfLibrary/services/TimeAttackService.cs) що реалізують один інтерфейс `IGameService`. Кожен клас сам визначає що робити при потраплянні на міну.

---

## 11. Запуск локально

### Вимоги
- Visual Studio 2022+
- .NET 8.0 або вище
- Windows (WPF підтримується тільки на Windows)

### Кроки
1. Клонувати репозиторій
```bash
git clone https://github.com/dmitro413/kpz-final
```
2. Відкрити `KPZ-FINAL.sln` у Visual Studio
3. Переконатись що `KPZ-FINAL` встановлений як Startup Project
4. Переконатись що є Project Reference: `KPZ-FINAL` → `WpfLibrary`
5. Запустити

> Дані зберігаються автоматично у папці з виконуваним файлом: `records.json`, `statistics.json`, `achievements.json`, `history.json`, `settings.json`
