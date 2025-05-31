# Используем базовый образ для runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Создаем пользователя для безопасности
RUN addgroup --system --gid 1001 logkeeper && \
    adduser --system --uid 1001 --ingroup logkeeper logkeeper

# Используем SDK образ для сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Копируем файл проекта и восстанавливаем зависимости
COPY ["LogKeeper.API.csproj", "."]
RUN dotnet restore "LogKeeper.API.csproj"

# Копируем весь исходный код
COPY . .
WORKDIR "/src/."

# Собираем проект
RUN dotnet build "LogKeeper.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Публикуем приложение
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "LogKeeper.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Финальный этап - копируем собранное приложение
FROM base AS final
WORKDIR /app

# Копируем опубликованное приложение
COPY --from=publish /app/publish .

# Меняем владельца файлов
RUN chown -R logkeeper:logkeeper /app

# Переключаемся на непривилегированного пользователя
USER logkeeper

# Устанавливаем переменные окружения
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Добавляем health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Точка входа
ENTRYPOINT ["dotnet", "LogKeeper.API.dll"]