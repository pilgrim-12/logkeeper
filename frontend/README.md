# 🗂️ LogKeeper

**Централизованная система сбора и анализа логов для микросервисной архитектуры**

> 🎯 **Production-ready система логирования** с веб-интерфейсом, REST API и Docker контейнеризацией

## 📋 Описание

LogKeeper - это современная система для централизованного сбора логов от различных приложений и сервисов. В отличие от библиотек логирования (Serilog, Winston), LogKeeper **собирает и анализирует логи** в едином интерфейсе.

### 🎪 Возможности

- ✅ **Сбор логов** от .NET, Node.js, Python и других приложений
- ✅ **Веб-интерфейс** для просмотра и управления логами
- ✅ **Фильтрация** по уровню, источнику и времени
- ✅ **REST API** с Swagger документацией
- ✅ **Docker Compose** для простого развертывания
- ✅ **SQL Server** база данных для надежного хранения
- ✅ **Hot reload** для разработки
- ✅ **Responsive дизайн** для мобильных устройств

## 🏗️ Архитектура

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   React SPA     │    │   .NET Web API  │    │   SQL Server    │
│   (Frontend)    │────│   (Backend)     │────│   (Database)    │
│  Port: 3000     │    │   Port: 5000    │    │   Port: 1433    │
└─────────────────┘    └─────────────────┘    └─────────────────┘
        │                       ▲
        │              ┌────────┴────────┐
        │              │                 │
        ▼              ▼                 ▼
┌─────────────────┐ ┌─────────────────┐ ┌─────────────────┐
│   Your .NET     │ │   Your Node.js  │ │   Your Python   │
│   Application   │ │   Application   │ │   Application   │
└─────────────────┘ └─────────────────┘ └─────────────────┘
```

## 🚀 Быстрый старт

### Предварительные требования

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) 4.0+
- Git

### Запуск проекта

```bash
# 1. Клонируем репозиторий
git clone https://github.com/yourusername/logkeeper.git
cd logkeeper

# 2. Запускаем все сервисы одной командой
docker-compose --profile dev up --build

# 3. Открываем в браузере
# Frontend: http://localhost:3000
# API: http://localhost:5000/swagger
# Health: http://localhost:5000/health
```

**Вот и всё!** 🎉 Полная система готова к использованию.

### Остановка

```bash
docker-compose down
```

## 📊 Использование

### 1. Веб-интерфейс

Откройте http://localhost:3000 для:

- ➕ Создания новых логов
- 📋 Просмотра списка логов
- 🔍 Фильтрации по уровням
- 🐛 Просмотра стек трейсов

### 2. REST API

Документация API: http://localhost:5000/swagger

**Создание лога:**

```bash
curl -X POST http://localhost:5000/api/logs \
  -H "Content-Type: application/json" \
  -d '{
    "level": "Error",
    "message": "Payment processing failed",
    "source": "PaymentService",
    "userId": "user123",
    "exception": "System.TimeoutException: Database timeout"
  }'
```

**Получение логов:**

```bash
curl http://localhost:5000/api/logs
```

### 3. Интеграция с приложениями

#### .NET + Serilog

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Http("http://localhost:5000/api/logs")
    .CreateLogger();

Log.Error("Payment failed for order {OrderId}", orderId);
```

#### Node.js + Winston

```javascript
const winston = require("winston");

const logger = winston.createLogger({
  transports: [
    new winston.transports.Http({
      host: "localhost",
      port: 5000,
      path: "/api/logs",
    }),
  ],
});

logger.error("Authentication failed", { userId: 123 });
```

## 🛠️ Технологический стек

### Backend

- **.NET 8** Web API
- **Entity Framework Core** с SQL Server
- **Swagger/OpenAPI** документация
- **Docker** контейнеризация

### Frontend

- **React 19** + **TypeScript**
- **CSS3** адаптивный дизайн
- **Hot reload** для разработки

### Database

- **SQL Server 2022** Express Edition
- **Entity Framework** миграции
- **Индексы** для оптимизации поиска

### DevOps

- **Docker Compose** оркестрация
- **Multi-stage builds** оптимизация
- **Health checks** мониторинг

## 📁 Структура проекта

```
logkeeper/
├── backend/
│   └── LogKeeper.API/          # .NET 8 Web API
│       ├── Controllers/        # API контроллеры
│       ├── Models/            # Модели данных
│       ├── Data/              # Entity Framework
│       └── Dockerfile         # Docker образ API
├── frontend/                   # React приложение
│   ├── src/                   # Исходный код React
│   ├── Dockerfile.dev         # Dev образ
│   ├── Dockerfile.prod        # Production образ
│   └── nginx.conf             # Nginx конфигурация
├── examples/                   # Примеры интеграции
├── docker-compose.yml         # Оркестрация сервисов
├── ROADMAP.md                 # План развития
└── README.md                  # Этот файл
```

## 🎯 Уровни логов

| Уровень     | Описание              | Цвет       | Пример использования      |
| ----------- | --------------------- | ---------- | ------------------------- |
| **Error**   | Критические ошибки    | 🔴 Красный | Исключения, сбои системы  |
| **Warning** | Предупреждения        | 🟡 Желтый  | Подозрительная активность |
| **Info**    | Информационные        | 🔵 Синий   | Успешные операции         |
| **Debug**   | Отладочная информация | ⚪ Серый   | Техническая диагностика   |

## 🔧 Конфигурация

### Переменные окружения

```bash
# API Configuration
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://+:8080
ConnectionStrings__DefaultConnection=Server=database,1433;Database=LogKeeperDB;User Id=sa;Password=LogKeeper2025!;TrustServerCertificate=true

# Database Configuration
ACCEPT_EULA=Y
SA_PASSWORD=LogKeeper2025!
MSSQL_PID=Express

# Frontend Configuration
REACT_APP_API_URL=http://localhost:5000
CHOKIDAR_USEPOLLING=true
```

### Порты

- **3000** - React Frontend (dev)
- **5000** - .NET API
- **1433** - SQL Server
- **80** - Nginx (production)

## 🚀 Развертывание

### Разработка

```bash
docker-compose --profile dev up --build
```

### Продакшен

```bash
docker-compose --profile prod up --build
```

### Только база данных

```bash
docker-compose up database
```

## 🔍 Мониторинг и отладка

### Логи контейнеров

```bash
# API логи
docker logs logkeeper-api -f

# Frontend логи
docker logs logkeeper-frontend-dev -f

# Database логи
docker logs logkeeper-database -f
```

### Health checks

- **API**: http://localhost:5000/health
- **Database**: http://localhost:5000/api/logs/health

### Проверка статуса

```bash
docker ps
docker-compose ps
```

## 🤝 Разработка

### Локальная разработка

```bash
# Запуск с hot reload
docker-compose --profile dev up

# Перестройка после изменений
docker-compose build api
docker-compose up api -d
```

### Добавление новых функций

1. Fork репозиторий
2. Создайте feature branch (`git checkout -b feature/amazing-feature`)
3. Внесите изменения
4. Commit (`git commit -m 'Add amazing feature'`)
5. Push в branch (`git push origin feature/amazing-feature`)
6. Откройте Pull Request

## 🗺️ Roadmap

Смотрите [ROADMAP.md](ROADMAP.md) для планов развития:

### Ближайшие фазы:

- 🔍 **Advanced Search** - полнотекстовый поиск, фильтры по дате
- 📊 **Dashboard** - графики, аналитика, статистика
- 📁 **Export** - экспорт логов в CSV/JSON
- 🔔 **Alerts** - уведомления о критических ошибках

### Долгосрочные планы:

- ☁️ **Cloud deployment** - AWS, Azure, Kubernetes
- 🔐 **Authentication** - JWT, RBAC, multi-tenancy
- 📡 **Real-time** - SignalR, live updates
- 🤖 **ML Analytics** - аномалии, предиктивная аналитика

## 📝 Changelog

### v1.1.0 (Current)

- ✅ Docker Compose контейнеризация
- ✅ SQL Server база данных
- ✅ Hot reload для разработки
- ✅ Production Dockerfile
- ✅ Health checks

### v1.0.0

- ✅ .NET 8 Web API
- ✅ React 19 + TypeScript Frontend
- ✅ Базовый CRUD для логов
- ✅ Responsive UI дизайн
- ✅ In-memory хранение

## 📄 Лицензия

Этот проект распространяется под MIT лицензией. См. [LICENSE](LICENSE) файл для деталей.

## 👨‍💻 Автор

**Ваше Имя** - [@yourusername](https://github.com/yourusername)

**Проект URL:** https://github.com/yourusername/logkeeper

---

## 🆘 Поддержка

Если у вас возникли проблемы:

1. Проверьте [Issues](https://github.com/yourusername/logkeeper/issues)
2. Создайте новый Issue с описанием проблемы
3. Приложите логи: `docker logs logkeeper-api`

## ⭐ Поддержите проект

Если LogKeeper оказался полезным, поставьте ⭐ звездочку на GitHub!

---

_Создано с ❤️ для сообщества разработчиков_
