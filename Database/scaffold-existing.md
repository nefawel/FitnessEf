# Подключение к существующей базе Fitnes

Scaffold выполняется только после проверки, что \`FITNESS_EF_CONNECTION\` указывает на существующую базу \`Fitnes\`, а не на учебную \`FitnessEfTraining\`.

    dotnet ef dbcontext scaffold $env:FITNESS_EF_CONNECTION \`
      Npgsql.EntityFrameworkCore.PostgreSQL \`
      --context FitnessExistingContext \`
      --context-dir ExistingData \`
      --output-dir ExistingData/Models \`
      --table clients \`
      --no-onconfiguring

Сгенерированные файлы нужно просмотреть и не редактировать вручную, если планируется повторный scaffold. Для собственных изменений используйте partial-классы или отдельную конфигурацию. Команды \`database update\` для существующей базы выполняются только после анализа схемы и истории миграций.
