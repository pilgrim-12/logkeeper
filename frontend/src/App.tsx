import React, { useState, useEffect } from "react";
import "./App.css";

interface LogEntry {
  id: number;
  level: string;
  message: string;
  source?: string;
  timestamp: string;
  exception?: string;
  userId?: string;
}

// Используем proxy из package.json, поэтому просто /api
const API_BASE = "";

function App() {
  const [logs, setLogs] = useState<LogEntry[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string>("");

  // Форма для создания нового лога
  const [newLog, setNewLog] = useState({
    level: "Info",
    message: "",
    source: "",
    exception: "",
    userId: "",
  });

  // Загрузка логов
  const loadLogs = async () => {
    setLoading(true);
    setError("");
    try {
      const response = await fetch(`${API_BASE}/api/logs`);
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      const data = await response.json();
      setLogs(data.data || []);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to load logs");
      console.error("Error loading logs:", err);
    } finally {
      setLoading(false);
    }
  };

  // Создание нового лога
  const createLog = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newLog.message.trim()) {
      alert("Сообщение не может быть пустым");
      return;
    }

    try {
      const response = await fetch(`${API_BASE}/api/logs`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          level: newLog.level,
          message: newLog.message,
          source: newLog.source || null,
          exception: newLog.exception || null,
          userId: newLog.userId || null,
        }),
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      // Очищаем форму
      setNewLog({
        level: "Info",
        message: "",
        source: "",
        exception: "",
        userId: "",
      });

      // Перезагружаем логи
      loadLogs();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Failed to create log");
      console.error("Error creating log:", err);
    }
  };

  // Получение цвета для уровня лога
  const getLevelColor = (level: string) => {
    switch (level.toLowerCase()) {
      case "error":
        return "bg-red-100 text-red-800 border-red-200";
      case "warning":
        return "bg-yellow-100 text-yellow-800 border-yellow-200";
      case "info":
        return "bg-blue-100 text-blue-800 border-blue-200";
      case "debug":
        return "bg-gray-100 text-gray-800 border-gray-200";
      default:
        return "bg-gray-100 text-gray-800 border-gray-200";
    }
  };

  // Форматирование даты
  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleString("ru-RU");
  };

  // Загружаем логи при монтировании компонента
  useEffect(() => {
    loadLogs();
  }, []);

  return (
    <div className="app">
      <header className="app-header">
        <h1>🗂️ LogKeeper</h1>
        <p>Система управления логами</p>
      </header>

      {error && <div className="error-message">❌ Ошибка: {error}</div>}

      {/* Форма создания лога */}
      <section className="create-log-section">
        <h2>➕ Добавить новый лог</h2>
        <form onSubmit={createLog} className="create-log-form">
          <div className="form-row">
            <div className="form-group">
              <label>Уровень:</label>
              <select
                value={newLog.level}
                onChange={(e) =>
                  setNewLog({ ...newLog, level: e.target.value })
                }
              >
                <option value="Debug">Debug</option>
                <option value="Info">Info</option>
                <option value="Warning">Warning</option>
                <option value="Error">Error</option>
              </select>
            </div>

            <div className="form-group">
              <label>Источник:</label>
              <input
                type="text"
                placeholder="Например: PaymentService"
                value={newLog.source}
                onChange={(e) =>
                  setNewLog({ ...newLog, source: e.target.value })
                }
              />
            </div>

            <div className="form-group">
              <label>User ID:</label>
              <input
                type="text"
                placeholder="ID пользователя"
                value={newLog.userId}
                onChange={(e) =>
                  setNewLog({ ...newLog, userId: e.target.value })
                }
              />
            </div>
          </div>

          <div className="form-group">
            <label>Сообщение:*</label>
            <textarea
              placeholder="Описание события или ошибки..."
              value={newLog.message}
              onChange={(e) =>
                setNewLog({ ...newLog, message: e.target.value })
              }
              required
              rows={3}
            />
          </div>

          <div className="form-group">
            <label>Исключение (Exception):</label>
            <textarea
              placeholder="Стек трейс ошибки (если есть)..."
              value={newLog.exception}
              onChange={(e) =>
                setNewLog({ ...newLog, exception: e.target.value })
              }
              rows={3}
            />
          </div>

          <button type="submit" className="submit-btn">
            📝 Создать лог
          </button>
        </form>
      </section>

      {/* Список логов */}
      <section className="logs-section">
        <div className="section-header">
          <h2>📋 Список логов</h2>
          <button onClick={loadLogs} disabled={loading} className="refresh-btn">
            {loading ? "⏳ Загружается..." : "🔄 Обновить"}
          </button>
        </div>

        {loading && logs.length === 0 ? (
          <div className="loading">⏳ Загрузка логов...</div>
        ) : logs.length === 0 ? (
          <div className="no-logs">📝 Логов пока нет. Создайте первый лог!</div>
        ) : (
          <div className="logs-list">
            {logs.map((log) => (
              <div key={log.id} className="log-entry">
                <div className="log-header">
                  <span className={`log-level ${getLevelColor(log.level)}`}>
                    {log.level}
                  </span>
                  <span className="log-timestamp">
                    🕒 {formatDate(log.timestamp)}
                  </span>
                  {log.source && (
                    <span className="log-source">📦 {log.source}</span>
                  )}
                  {log.userId && (
                    <span className="log-user">👤 {log.userId}</span>
                  )}
                </div>

                <div className="log-message">{log.message}</div>

                {log.exception && (
                  <details className="log-exception">
                    <summary>🐛 Исключение</summary>
                    <pre>{log.exception}</pre>
                  </details>
                )}
              </div>
            ))}
          </div>
        )}
      </section>
    </div>
  );
}

export default App;
