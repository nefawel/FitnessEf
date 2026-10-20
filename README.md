# FitnessEf

Консольное учебное приложение на .NET 8 и Entity Framework Core для работы с клиентами фитнес-клуба в PostgreSQL.

## Автор

ФИО: **[укажите полное ФИО]**

## Требования

- .NET SDK 8 или совместимый SDK;
- PostgreSQL 14+;
- доступ к NuGet.

## Быстрый старт

1. Восстановить локальный инструмент EF Core:

   ```powershell
   dotnet tool restore
   ```

2. Задать строку подключения к учебной базе, не сохраняя пароль в Git:

   ```powershell
   $env:FITNESS_EF_CONNECTION = "Host=localhost;Port=5432;Database=FitnessEfTraining;Username=postgres;Password=YOUR_PASSWORD"
   ```

3. Собрать и запустить приложение:

   ```powershell
   dotnet build
   dotnet run
   ```

4. После создания модели применить миграции:

   ```powershell
   dotnet ef database update
   ```

Учебная база `FitnessEfTraining` отделена от существующей базы `Fitnes`. Реальные пароли и локальные файлы базы не должны попадать в публичный репозиторий.

## Практические задания

История Git содержит отдельный коммит для каждого задания из методички. Коммиты помечены номерами заданий и содержат краткое описание выполненной работы.
