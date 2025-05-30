using System.ComponentModel.DataAnnotations;

namespace LogKeeper.API.Models
{
    public class LogEntry
    {
        public int Id { get; set; }

        [Required]
        public string Level { get; set; } = string.Empty; // Info, Warning, Error, Debug

        [Required]
        public string Message { get; set; } = string.Empty;

        public string? Source { get; set; } // Источник лога (имя сервиса, компонента)

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string? Exception { get; set; } // Текст исключения, если есть

        public string? UserId { get; set; } // ID пользователя, если применимо

        public string? Properties { get; set; } // JSON строка с дополнительными свойствами
    }

    public class CreateLogRequest
    {
        [Required]
        public string Level { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public string? Source { get; set; }

        public string? Exception { get; set; }

        public string? UserId { get; set; }

        public Dictionary<string, object>? Properties { get; set; }
    }

    public class LogSearchRequest
    {
        public string? Level { get; set; }
        public string? Source { get; set; }
        public string? SearchTerm { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }
}