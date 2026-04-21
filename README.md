# Telegram Toxic Monitor (C#/.NET 8)

Инфраструктура для автоматизированной модерации контента на базе алгоритмов машинного обучения.

## Технологический стек
* **Runtime:** .NET 8 (LTS)
* **Language:** C# 12
* **Web API:** ASP.NET Core
* **Database:** PostgreSQL + Entity Framework Core (Code First)
* **Client:** WPF, TelergamBot
* **ML Engine:** SVM (Support Vector Machine), TF-IDF, STOLP

## Архитектура решения
Проект представлен в трехуровневой архитектуре:
<img width="576" height="885" alt="image" src="https://github.com/user-attachments/assets/1a689e5c-38f6-41c2-919b-80fb53e57916" />

## Используемые алгоритмы
1.  **TF-IDF:** Векторизация текстовых данных (преобразование слов в числовые веса на основе частотности).
2.  **SVM (Linear Kernel):** Построение разделяющей гиперплоскости для бинарной классификации (Токсично / Нетоксично).
3.  **STOLP:** Алгоритм сжатия обучающей выборки и удаления шума для повышения качества опорных векторов.

## Потоки данных

### 1. Процесс обработки входящих сообщений
<img width="839" height="382" alt="image" src="https://github.com/user-attachments/assets/5d82a808-cce9-4737-9e67-989a71b2ef32" />


### 2. Процесс регистрации
<img width="974" height="432" alt="image" src="https://github.com/user-attachments/assets/19749d54-fc28-43df-96f7-c8c4f057d693" />


### 3. Процесс аутентификации в WPF-приложении
<img width="974" height="102" alt="image" src="https://github.com/user-attachments/assets/d1ce8d31-ef67-494e-a187-0319e5882b20" />


### 4. Процесс выбора данных для формирования статистики
<img width="974" height="102" alt="image" src="https://github.com/user-attachments/assets/404a8b45-308d-4f06-a91d-13157aa14479" />


## Структура базы данных
<img width="974" height="445" alt="image" src="https://github.com/user-attachments/assets/c8fe8bbc-5598-46c6-bfc5-54f603fd0b66" />
