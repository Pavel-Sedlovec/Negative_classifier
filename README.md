# Telegram Toxic Monitor

<p align="left">
  <img src="https://img.shields.io/badge/.NET%208.0-512BD4?style=plastic&logo=.net&logoColor=white" alt=".NET 8" />
  <img src="https://img.shields.io/badge/C%23%2012-239120?style=plastic&logo=csharp&logoColor=white" alt="C#" />
  <img src="https://img.shields.io/badge/ASP.NET%20Core-512BD4?style=plastic&logo=dotnet&logoColor=white" alt="ASP.NET Core" />
  <img src="https://img.shields.io/badge/PostgreSQL-4169E1?style=plastic&logo=postgresql&logoColor=white" alt="PostgreSQL" />
  <img src="https://img.shields.io/badge/EF%20Core-512BD4?style=plastic&logo=dotnet&logoColor=white" alt="EF Core" />
  <img src="https://img.shields.io/badge/WPF-0078D4?style=plastic&logo=windows&logoColor=white" alt="WPF" />
  <img src="https://img.shields.io/badge/Telegram-26A5E4?style=plastic&logo=telegram&logoColor=white" alt="Telegram Bot" />
  <img src="https://img.shields.io/badge/ML-FF6F00?style=plastic&logo=tensorflow&logoColor=white" alt="Machine Learning" />
</p>

Инфраструктура для автоматизированной модерации контента в Telegram на базе алгоритмов машинного обучения. SVM-классификатор, обученный на реальных данных, определяет токсичность сообщений в реальном времени. Все ML-алгоритмы реализованы с нуля на C# без внешних зависимостей.

---

## Оглавление

- [Технологический стек](#технологический-стек)
- [Архитектура проекта](#архитектура-проекта)
- [Алгоритмы](#алгоритмы)
- [Метрики модели](#метрики-модели)
- [Потоки данных](#потоки-данных)
- [Структура базы данных](#структура-базы-данных)
- [Спецификация Web API](#спецификация-web-api)
- [Демонстрация](#демонстрация)
- [Быстрый старт](#быстрый-старт)

---

## Технологический стек

| Слой | Технология |
| :--- | :--- |
| Runtime | .NET 8 (LTS) |
| Language | C# 12 |
| Web API | ASP.NET Core |
| Database | PostgreSQL + Entity Framework Core (Code First) |
| Client | WPF, Telegram Bot |
| ML Engine | SVM, TF-IDF, STOLP |

---

## Архитектура проекта

Трёхуровневая архитектура: бот принимает сообщения, API классифицирует и сохраняет, WPF-клиент визуализирует аналитику.

<img width="288" height="442" alt="Архитектура" src="https://github.com/user-attachments/assets/1a689e5c-38f6-41c2-919b-80fb53e57916" />

| Компонент | Описание | Технологии |
| :--- | :--- | :--- |
| **BotTelegram** | Слушает сообщения, отправляет на классификацию, уведомляет о токсичных постах | `Telegram.Bot` |
| **ASP.NET Core API** | REST-эндпоинты для предсказания, статистики, авторизации и управления чатами | `Web API`, `C# 12` |
| **Core ML Engine** | SVM + TF-IDF + STOLP реализованы с нуля без внешних зависимостей | `Алгоритмы` |
| **WPF Dashboard** | Десктопный клиент для аналитики, топа токсичных пользователей и управления чатами | `WPF (MVVM)` |
| **Trainer** | Консольный инструмент для обучения модели, оценки метрик и сохранения весов | `CLI App` |
| **Database Layer** | Code-First миграции, хранение сообщений, пользователей, чатов и администраторов | `PostgreSQL`, `EF Core` |

---

## Алгоритмы

| № | Алгоритм | Описание | Реализация |
| :---: | :--- | :--- | :--- |
| 1 | **TF-IDF** | Преобразует слова в числовые веса на основе частотности в документе и корпусе | `VectorizationData.cs` |
| 2 | **SVM Linear Kernel** | Строит разделяющую гиперплоскость для бинарной классификации (Токсично / Нетоксично) с балансировкой классов | `SVM.cs` |
| 3 | **STOLP** | Удаляет шумные и дублирующие примеры, оставляет только эталонные опорные векторы | `STOLP.cs` |

---

## Метрики модели

Модель обучена на 75% датасета, тестирование проводилось на оставшихся 25%.

| Accuracy | Precision | Recall | F1 Score |
| :---: | :---: | :---: | :---: |
| **85.1%** | **81.5%** | **72.6%** | **76.8%** |

---

## Потоки данных

### 1. Обработка входящих сообщений

<p align="center">
  <img width="839" height="382" alt="Процесс обработки входящих сообщений" src="https://github.com/user-attachments/assets/5d82a808-cce9-4737-9e67-989a71b2ef32" />
</p>

1. Пользователь отправляет сообщение в чат — оно поступает на `Telegram server`.
2. `Telegram Bot` опрашивает сервер через Long Polling (`HTTPS GET /getUpdates`) и получает массив обновлений `Update[]`.
3. Бот передаёт текст в бэкенд: `HTTP POST /api/Telegram/Message`.
4. `Core ML Engine` прогоняет текст через TF-IDF и SVM, возвращает метку (`negative` / `positive`).
5. `API` возвращает боту `HTTP 200 OK` с объектом `ClassifyMessage`.
6. Параллельно сообщение и результат классификации сохраняются в PostgreSQL.

### 2. Регистрация администратора

<p align="center">
  <img width="974" height="432" alt="Процесс регистрации" src="https://github.com/user-attachments/assets/19749d54-fc28-43df-96f7-c8c4f057d693" />
</p>

1. Пользователь отправляет команду `/start` в Telegram-клиенте.
2. `Telegram Bot` забирает событие через Long Polling и формирует `SetAdminRequest`.
3. Бот отправляет `HTTP POST /api/Auth/SetAdmin` в бэкенд.
4. `API` записывает нового администратора в PostgreSQL.
5. Бот получает `HTTP 200 OK` с `AdminRegistrationResponse` и отправляет подтверждение пользователю через `HTTPS POST /sendMessage`.

### 3. Аутентификация в WPF-приложении

<p align="center">
  <img width="974" height="102" alt="Процесс аутентификации в WPF-приложении" src="https://github.com/user-attachments/assets/d1ce8d31-ef67-494e-a187-0319e5882b20" />
</p>

1. Администратор вводит логин и пароль в форму WPF-клиента.
2. Клиент отправляет `LoginRequest` через `HTTP POST /api/Auth/Login`.
3. `API` сверяет данные с PostgreSQL.
4. При совпадении возвращается `HTTP 200 OK` с `AdminRegistrationResponse` — доступ открыт. При несовпадении — `401 Unauthorized`.

### 4. Формирование статистики

<p align="center">
  <img width="974" height="102" alt="Процесс выбора данных для формирования статистики" src="https://github.com/user-attachments/assets/404a8b45-308d-4f06-a91d-13157aa14479" />
</p>

1. Администратор открывает раздел статистики в WPF Dashboard.
2. Приложение отправляет `GET /api/Stats` с нужными параметрами.
3. `API` агрегирует данные из PostgreSQL (топ пользователей, статистика за период).
4. Ответ `HTTP 200 OK (Data)` сериализуется в JSON, WPF обновляет ViewModel — графики перерисовываются.

---

## Структура базы данных

Code-First модель на Entity Framework Core. Четыре сущности обеспечивают полное логирование, авторизацию и управление чатами.

<p align="center">
  <img width="974" height="445" alt="Структура базы данных" src="https://github.com/user-attachments/assets/c8fe8bbc-5598-46c6-bfc5-54f603fd0b66" />
</p>

#### Messages
Хранит историю перехваченных сообщений и результаты ML-скоринга.
- `Id (PK)` — идентификатор записи
- `TextLabel` — метка классификации (`positive` / `negative`)
- `Confidence` — уверенность модели в вердикте
- `Created_at` — дата и время получения сообщения
- `Chat_id (FK)` — связь с таблицей чатов
- `User_id (FK)` — связь с таблицей пользователей

#### Chats
Telegram-группы и каналы, подключённые к системе.
- `Id (PK)` — внутренний идентификатор
- `Chat_id_tg` — реальный ID чата в Telegram
- `Name` — название группы или канала
- `Added_at` — дата подключения к системе
- `Admin_id (FK)` — администратор, привязавший чат

#### Users
Участники чатов, чья активность анализируется.
- `Id (PK)` — внутренний идентификатор
- `User_id_tg` — реальный ID аккаунта в Telegram
- `Username` — публичный никнейм (`@username`)
- `First_name` — имя из профиля

#### Admins
Учётные данные для входа в WPF Dashboard.
- `Id (PK)` — идентификатор администратора
- `Login` — уникальный логин
- `Password` — пароль для аутентификации
- `TelegramId` — привязанный Telegram-аккаунт

---

## Спецификация Web API

Базовый URL: `http://localhost:5146`

### Эндпоинты

| Контроллер | Метод | Эндпоинт | Тело / Параметры | Ответ | Описание |
| :--- | :---: | :--- | :--- | :--- | :--- |
| **Auth** | `POST` | `/api/Auth/SetAdmin` | `SetAdminRequest` | `AdminRegistrationResponse` | Регистрация администратора по Telegram ID |
| **Auth** | `POST` | `/api/Auth/Login` | `LoginRequest` | `AdminRegistrationResponse` | Аутентификация для WPF (401 при ошибке) |
| **Chats** | `GET` | `/api/Chats/GetChats/{adminLogin}` | `adminLogin` (string) | `List<ChatInfo>` | Список чатов администратора |
| **Stats** | `GET` | `/api/Stats/GeneralChat/{chatId}` | `chatId` (long) | `MessageStatsInChat` | Общая статистика чата |
| **Stats** | `GET` | `/api/Stats/ByDay/{chatId}/{day}` | `chatId`, `day` (DateTime) | `MessageStatsInChat` | Статистика за конкретный день |
| **Stats** | `GET` | `/api/Stats/TopNegative/{chatId}/{count}` | `chatId`, `count` (int) | `List<StatsTopNegativeUsersResponse>` | Топ токсичных пользователей |
| **Telegram** | `POST` | `/api/Telegram/Message` | `TgMessageRequest` | `ClassifyMessage` | Приём сообщения, ML-скоринг, сохранение |

### Модели запросов

<details open>
<summary><b>Requests</b></summary>

#### LoginRequest
```json
{
  "login": "admin_username",
  "password": "secure_password"
}
```
- **login** (`string`) — логин администратора
- **password** (`string`) — пароль

---

#### SetAdminRequest
```json
{
  "userIdTg": 123456789
}
```
- **userIdTg** (`long`) — Telegram ID пользователя

---

#### TgMessageRequest
```json
{
  "text": "Текст сообщения для анализа",
  "chatIdTg": -100123456789,
  "chatName": "Название чата",
  "userIdTg": 987654321,
  "username": "user_nickname",
  "firstName": "Имя"
}
```
- **text** (`string`) — текст сообщения
- **chatIdTg** (`long`) — ID чата в Telegram
- **chatName** (`string`) — название чата
- **userIdTg** (`long`) — ID автора сообщения
- **username** (`string`) — никнейм без `@`
- **firstName** (`string`) — имя из профиля

</details>

### Модели ответов

<details open>
<summary><b>Responses</b></summary>

#### AdminRegistrationResponse
```json
{
  "message": "Успешная регистрация / Авторизация успешна",
  "login": "admin_login",
  "password": "generated_or_current_password"
}
```
- **message** (`string`) — текстовый статус операции
- **login** (`string`) — логин администратора
- **password** (`string`) — действующий пароль

---

#### ChatInfo
```json
{
  "chatIdTg": -100123456789,
  "name": "Название чата",
  "added_at": "2026-05-31T14:00:00Z"
}
```
- **chatIdTg** (`long`) — ID чата в Telegram
- **name** (`string`) — название чата
- **added_at** (`DateTime`) — дата интеграции, ISO 8601

---

#### ClassifyMessage
```json
{
  "text": "Текст сообщения для анализа",
  "label": 1,
  "sentiment": "negative",
  "confidence": 0.895
}
```
- **text** (`string`) — исходный текст сообщения
- **label** (`int`) — `0` нейтральное, `1` негативное
- **sentiment** (`string`) — `negative` / `positive`
- **confidence** (`double`) — уверенность модели (0.0 – 1.0)

---

#### MessageStatsInChat
```json
{
  "total": 1500,
  "negative": 300,
  "positive": 1200,
  "negativePercent": 20.0,
  "positivePercent": 80.0
}
```
- **total** (`int`) — всего проанализировано сообщений
- **negative** / **positive** (`int`) — количество по классам
- **negativePercent** / **positivePercent** (`double`) — доля в процентах

---

#### StatsTopNegativeUsersResponse
```json
{
  "userIdTg": 987654321,
  "totalMes": 50,
  "negativeMes": 42,
  "positiveMes": 8,
  "negativePercent": 84.0,
  "positivePercent": 16.0
}
```
- **userIdTg** (`long`) — Telegram ID пользователя
- **totalMes** (`int`) — всего сообщений
- **negativeMes** / **positiveMes** (`int`) — количество по классам
- **negativePercent** / **positivePercent** (`double`) — доля в процентах

</details>

---

## Демонстрация

### Видеообзор (40 секунд)

Сквозной процесс: от отправки сообщения в Telegram до обновления графиков в панели администратора.

https://github.com/user-attachments/assets/51a10435-ff8b-46d5-a08f-fe77efac515a

---

### Интерфейс Telegram-бота

Бот регистрирует администраторов и перехватывает текстовый поток в подключённых чатах.

| Информация о боте | Процесс регистрации |
| :---: | :---: |
| <img width="100%" alt="Профиль бота" src="https://github.com/user-attachments/assets/d7870da2-a652-466e-b373-7104c9627fb8" /> | <img width="100%" height="695" alt="Регистрация" src="https://github.com/user-attachments/assets/7f15b25d-20bf-4bfc-9bf9-620ddac057ce" /> |
| Профиль бота `@FazeFilterBot`: анализирует тональность и следит за порядком в чате | Команда `/start`: бот генерирует уникальный пароль для входа в WPF Dashboard |

---

### WPF Dashboard

#### Экран авторизации

<p align="center">
  <img width="1348" height="617" alt="Авторизация" src="https://github.com/user-attachments/assets/b18ab9c3-9b8b-4134-9707-b647f4272fd6" />
</p>

Вход через логин и пароль, выданные ботом при регистрации.

#### Главный экран аналитики

<p align="center">
  <img width="1365" height="655" alt="Dashboard" src="https://github.com/user-attachments/assets/e9124929-514c-4a59-838c-e56df2cfcd7f" />
</p>

- **Выбор чата** — навигационная панель слева для переключения между модерируемыми ресурсами
- **Общая статистика** — круговые диаграммы соотношения нормального контента к токсичному за всё время
- **Статистика по дате** — календарный фильтр для отслеживания динамики за конкретный день
- **Топ нарушителей** — рейтинг по проценту негативных сообщений для принятия решений о блокировках

---

## Быстрый старт

### Требования

- .NET 8 SDK или выше
- PostgreSQL
- Токен Telegram-бота от `@BotFather`

### Развёртывание

#### 1. Клонирование репозитория
```bash
git clone https://github.com/Pavel-Sedlovec/Negative_classifier.git
cd Negative_classifier
```

#### 2. Настройка конфигурации (User Secrets)

Строка подключения к PostgreSQL (проект API):
```bash
cd TelegramToxicMonitor.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=negative_db;Username=postgres;Password=your_password"
cd ..
```

Токен Telegram-бота (проект Bot):
```bash
cd TelegramToxicMonitor.Bot
dotnet user-secrets init
dotnet user-secrets set "BotConfiguration:BotToken" "1234567890:ABCdefGhIJKlmNoPQRsTUVwxyZ"
cd ..
```

#### 3. Применение миграций
```bash
dotnet ef database update --project TelegramToxicMonitor.Data --startup-project TelegramToxicMonitor.API
```

Если `dotnet ef` не установлен:
```bash
dotnet tool install --global dotnet-ef
```

#### 4. Запуск компонентов

Backend API:
```bash
dotnet run --project TelegramToxicMonitor.API
```

Telegram Bot:
```bash
dotnet run --project TelegramToxicMonitor.Bot
```

WPF Dashboard:
```bash
dotnet run --project TelegramToxicMonitor.WPF
```

### Переобучение модели (опционально)

```bash
dotnet run --project TelegramToxicMonitor.Trainer
```

Файлы весов обновятся автоматически в директории Core-движка.
