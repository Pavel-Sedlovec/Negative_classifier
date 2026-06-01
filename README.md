# Telegram Toxic Monitor

> 🇷🇺 [Русская версия](./README.ru.md)

<p align="left">
  <img src="https://img.shields.io/badge/.NET%208.0-512BD4?style=for-the-badge&logo=.net&logoColor=white" alt=".NET 8" />
  <img src="https://img.shields.io/badge/C%23%2012-239120?style=for-the-badge&logo=csharp&logoColor=white" alt="C#" />
  <img src="https://img.shields.io/badge/ASP.NET%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="ASP.NET Core" />
  <img src="https://img.shields.io/badge/PostgreSQL-4169E1?style=for-the-badge&logo=postgresql&logoColor=white" alt="PostgreSQL" />
  <img src="https://img.shields.io/badge/EF%20Core-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt="EF Core" />
  <img src="https://img.shields.io/badge/WPF-512BD4?style=for-the-badge&logo=windows&logoColor=white" alt="WPF" />
  <img src="https://img.shields.io/badge/Telegram%20Bot-26A5E4?style=for-the-badge&logo=telegram&logoColor=white" alt="Telegram Bot" />
  <img src="https://img.shields.io/badge/Machine%20Learning-FF6F00?style=for-the-badge&logo=tensorflow&logoColor=white" alt="Machine Learning" />
</p>

Infrastructure for automated content moderation in Telegram based on machine learning algorithms. An SVM classifier trained on real data detects toxic messages in real time. All ML algorithms are implemented from scratch in C# with no external dependencies.

---

## Table of Contents

- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [Algorithms](#algorithms)
- [Model Metrics](#model-metrics)
- [Data Flows](#data-flows)
- [Database Schema](#database-schema)
- [Web API Specification](#web-api-specification)
- [Demo](#demo)
- [Quick Start](#quick-start)
- [License](#license)

---

## Tech Stack

| Layer | Technology |
| :--- | :--- |
| Runtime | .NET 8 (LTS) |
| Language | C# 12 |
| Web API | ASP.NET Core |
| Database | PostgreSQL + Entity Framework Core (Code First) |
| Client | WPF, Telegram Bot |
| ML Engine | SVM, TF-IDF, STOLP |

---

## Architecture

Three-tier architecture: the bot receives messages, the API classifies and persists them, the WPF client visualises analytics.

<img width="288" height="442" alt="Architecture" src="https://github.com/user-attachments/assets/1a689e5c-38f6-41c2-919b-80fb53e57916" />

| Component | Description | Technologies |
| :--- | :--- | :--- |
| **BotTelegram** | Listens for messages, sends them for classification, notifies about toxic posts | `Telegram.Bot` |
| **ASP.NET Core API** | REST endpoints for prediction, statistics, auth, and chat management | `Web API`, `C# 12` |
| **Core ML Engine** | SVM + TF-IDF + STOLP implemented from scratch with no external dependencies | `Algorithms` |
| **WPF Dashboard** | Desktop client for analytics, top toxic users, and chat management | `WPF (MVVM)` |
| **Trainer** | CLI tool for training the model, evaluating metrics, and saving weights | `CLI App` |
| **Database Layer** | Code-First migrations, storage of messages, users, chats, and admins | `PostgreSQL`, `EF Core` |

---

## Algorithms

| # | Algorithm | Description | File |
| :---: | :--- | :--- | :--- |
| 1 | **TF-IDF** | Converts words into numerical weights based on frequency in the document and corpus | `VectorizationData.cs` |
| 2 | **SVM Linear Kernel** | Builds a separating hyperplane for binary classification (Toxic / Non-toxic) with class balancing | `SVM.cs` |
| 3 | **STOLP** | Removes noisy and duplicate samples, keeping only reference support vectors | `STOLP.cs` |

---

## Model Metrics

The model was trained on 75% of the dataset; evaluation was performed on the remaining 25%.

| Accuracy | Precision | Recall | F1 Score |
| :---: | :---: | :---: | :---: |
| **85.1%** | **81.5%** | **72.6%** | **76.8%** |

---

## Data Flows

### 1. Incoming Message Processing

<p align="center">
  <img width="839" height="382" alt="Incoming message processing" src="https://github.com/user-attachments/assets/5d82a808-cce9-4737-9e67-989a71b2ef32" />
</p>

1. A user sends a message to a chat — it reaches the `Telegram server`.
2. `Telegram Bot` polls the server via Long Polling (`HTTPS GET /getUpdates`) and receives the `Update[]` array.
3. The bot forwards the text to the backend: `HTTP POST /api/Telegram/Message`.
4. `Core ML Engine` runs the text through TF-IDF and SVM, returning a label (`negative` / `positive`).
5. The `API` returns `HTTP 200 OK` with a `ClassifyMessage` object to the bot.
6. Simultaneously, the message and classification result are persisted to PostgreSQL.

### 2. Admin Registration

<p align="center">
  <img width="974" height="432" alt="Admin registration" src="https://github.com/user-attachments/assets/19749d54-fc28-43df-96f7-c8c4f057d693" />
</p>

1. The user sends `/start` in the Telegram client.
2. `Telegram Bot` picks up the event via Long Polling and builds a `SetAdminRequest`.
3. The bot sends `HTTP POST /api/Auth/SetAdmin` to the backend.
4. The `API` writes the new admin to PostgreSQL.
5. The bot receives `HTTP 200 OK` with `AdminRegistrationResponse` and confirms registration via `HTTPS POST /sendMessage`.

### 3. WPF Authentication

<p align="center">
  <img width="974" height="102" alt="WPF authentication" src="https://github.com/user-attachments/assets/d1ce8d31-ef67-494e-a187-0319e5882b20" />
</p>

1. The admin enters login and password into the WPF client form.
2. The client sends a `LoginRequest` via `HTTP POST /api/Auth/Login`.
3. The `API` verifies credentials against PostgreSQL.
4. On match: `HTTP 200 OK` with `AdminRegistrationResponse` — access granted. On mismatch: `401 Unauthorized`.

### 4. Statistics Query

<p align="center">
  <img width="974" height="102" alt="Statistics query" src="https://github.com/user-attachments/assets/404a8b45-308d-4f06-a91d-13157aa14479" />
</p>

1. The admin opens the statistics section in the WPF Dashboard.
2. The app sends `GET /api/Stats` with the required parameters.
3. The `API` aggregates data from PostgreSQL (top users, stats for a period).
4. `HTTP 200 OK (Data)` is serialised to JSON, WPF updates the ViewModel — charts re-render.

---

## Database Schema

Code-First model on Entity Framework Core. Four entities cover full logging, auth, and chat configuration management.

<p align="center">
  <img width="974" height="445" alt="Database schema" src="https://github.com/user-attachments/assets/c8fe8bbc-5598-46c6-bfc5-54f603fd0b66" />
</p>

#### Messages
History of intercepted messages and ML scoring results.
- `Id (PK)` — record identifier
- `TextLabel` — classification label (`positive` / `negative`)
- `Confidence` — model confidence score
- `Created_at` — message receipt timestamp
- `Chat_id (FK)` — reference to Chats
- `User_id (FK)` — reference to Users

#### Chats
Telegram groups and channels connected to the system.
- `Id (PK)` — internal identifier
- `Chat_id_tg` — real Telegram chat ID
- `Name` — group or channel name
- `Added_at` — date added to the system
- `Admin_id (FK)` — admin who connected the chat

#### Users
Chat participants whose activity is analysed.
- `Id (PK)` — internal identifier
- `User_id_tg` — real Telegram account ID
- `Username` — public handle (`@username`)
- `First_name` — name from profile

#### Admins
Credentials for WPF Dashboard access.
- `Id (PK)` — admin identifier
- `Login` — unique login
- `Password` — authentication password
- `TelegramId` — linked Telegram account

---

## Web API Specification

Base URL: `http://localhost:5146`

### Endpoints

| Controller | Method | Endpoint | Body / Params | Response | Description |
| :--- | :---: | :--- | :--- | :--- | :--- |
| **Auth** | `POST` | `/api/Auth/SetAdmin` | `SetAdminRequest` | `AdminRegistrationResponse` | Register admin by Telegram ID |
| **Auth** | `POST` | `/api/Auth/Login` | `LoginRequest` | `AdminRegistrationResponse` | WPF authentication (401 on failure) |
| **Chats** | `GET` | `/api/Chats/GetChats/{adminLogin}` | `adminLogin` (string) | `List<ChatInfo>` | Admin's chat list |
| **Stats** | `GET` | `/api/Stats/GeneralChat/{chatId}` | `chatId` (long) | `MessageStatsInChat` | Overall chat statistics |
| **Stats** | `GET` | `/api/Stats/ByDay/{chatId}/{day}` | `chatId`, `day` (DateTime) | `MessageStatsInChat` | Stats for a specific day |
| **Stats** | `GET` | `/api/Stats/TopNegative/{chatId}/{count}` | `chatId`, `count` (int) | `List<StatsTopNegativeUsersResponse>` | Top toxic users |
| **Telegram** | `POST` | `/api/Telegram/Message` | `TgMessageRequest` | `ClassifyMessage` | Receive message, ML scoring, persist |

### Request Models

<details open>
<summary><b>Requests</b></summary>

#### LoginRequest
```json
{
  "login": "admin_username",
  "password": "secure_password"
}
```
- **login** (`string`) — admin login
- **password** (`string`) — password

---

#### SetAdminRequest
```json
{
  "userIdTg": 123456789
}
```
- **userIdTg** (`long`) — Telegram user ID

---

#### TgMessageRequest
```json
{
  "text": "Message text to analyse",
  "chatIdTg": -100123456789,
  "chatName": "Chat name",
  "userIdTg": 987654321,
  "username": "user_nickname",
  "firstName": "Name"
}
```
- **text** (`string`) — message text
- **chatIdTg** (`long`) — Telegram chat ID
- **chatName** (`string`) — chat name
- **userIdTg** (`long`) — message author ID
- **username** (`string`) — handle without `@`
- **firstName** (`string`) — name from profile

</details>

### Response Models

<details open>
<summary><b>Responses</b></summary>

#### AdminRegistrationResponse
```json
{
  "message": "Registration successful / Authorisation successful",
  "login": "admin_login",
  "password": "generated_or_current_password"
}
```
- **message** (`string`) — operation status text
- **login** (`string`) — admin login
- **password** (`string`) — current password

---

#### ChatInfo
```json
{
  "chatIdTg": -100123456789,
  "name": "Chat name",
  "added_at": "2026-05-31T14:00:00Z"
}
```
- **chatIdTg** (`long`) — Telegram chat ID
- **name** (`string`) — chat name
- **added_at** (`DateTime`) — integration date, ISO 8601

---

#### ClassifyMessage
```json
{
  "text": "Message text to analyse",
  "label": 1,
  "sentiment": "negative",
  "confidence": 0.895
}
```
- **text** (`string`) — original message text
- **label** (`int`) — `0` neutral, `1` negative
- **sentiment** (`string`) — `negative` / `positive`
- **confidence** (`double`) — model confidence (0.0 – 1.0)

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
- **total** (`int`) — total messages analysed
- **negative** / **positive** (`int`) — count per class
- **negativePercent** / **positivePercent** (`double`) — share in percent

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
- **userIdTg** (`long`) — Telegram user ID
- **totalMes** (`int`) — total messages
- **negativeMes** / **positiveMes** (`int`) — count per class
- **negativePercent** / **positivePercent** (`double`) — share in percent

</details>

---

## Demo

### Video Overview (40 seconds)

End-to-end flow: from sending a message in Telegram to updating charts in the admin dashboard.

https://github.com/user-attachments/assets/51a10435-ff8b-46d5-a08f-fe77efac515a

---

### Telegram Bot Interface

The bot registers admins and intercepts the text stream in connected chats.

| Bot info | Registration |
| :---: | :---: |
| <img width="100%" alt="Bot profile" src="https://github.com/user-attachments/assets/d7870da2-a652-466e-b373-7104c9627fb8" /> | <img width="100%" height="695" alt="Registration" src="https://github.com/user-attachments/assets/7f15b25d-20bf-4bfc-9bf9-620ddac057ce" /> |
| Bot profile `@FazeFilterBot`: analyses sentiment and monitors order in the chat | Command `/start`: the bot generates a unique password for WPF Dashboard login |

---

### WPF Dashboard

#### Login Screen

<p align="center">
  <img width="1348" height="617" alt="Login" src="https://github.com/user-attachments/assets/b18ab9c3-9b8b-4134-9707-b647f4272fd6" />
</p>

Login using credentials issued by the bot at registration.

#### Main Analytics Screen

<p align="center">
  <img width="1365" height="655" alt="Dashboard" src="https://github.com/user-attachments/assets/e9124929-514c-4a59-838c-e56df2cfcd7f" />
</p>

- **Chat selector** — left-side navigation panel to switch between monitored resources
- **Overall statistics** — pie charts showing the ratio of normal to toxic content for all time
- **Stats by date** — calendar filter to track activity and spikes on a specific day
- **Top offenders** — ranking by percentage of negative messages for quick moderation decisions

---

## Quick Start

### Requirements

- .NET 8 SDK or later
- PostgreSQL
- Telegram bot token from `@BotFather`

### Deployment

#### 1. Clone the repository
```bash
git clone https://github.com/Pavel-Sedlovec/Negative_classifier.git
cd Negative_classifier
```

#### 2. Configure secrets (User Secrets)

PostgreSQL connection string (API project):
```bash
cd TelegramToxicMonitor.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=negative_db;Username=postgres;Password=your_password"
cd ..
```

Telegram bot token (Bot project):
```bash
cd TelegramToxicMonitor.Bot
dotnet user-secrets init
dotnet user-secrets set "BotConfiguration:BotToken" "1234567890:ABCdefGhIJKlmNoPQRsTUVwxyZ"
cd ..
```

#### 3. Apply migrations
```bash
dotnet ef database update --project TelegramToxicMonitor.Data --startup-project TelegramToxicMonitor.API
```

If `dotnet ef` is not installed:
```bash
dotnet tool install --global dotnet-ef
```

#### 4. Run components

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

### Retrain the model (optional)

```bash
dotnet run --project TelegramToxicMonitor.Trainer
```

Weight files are updated automatically in the Core engine directory.

---

## License

MIT License

Copyright (c) 2026 Pavel Sedlovec

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
