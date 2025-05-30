using Microsoft.AspNetCore.Mvc;
using LogKeeper.API.Models;

namespace LogKeeper.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        // Временное хранение в памяти для тестирования
        private static readonly List<LogEntry> _logs = new();
        private static int _nextId = 1;

        // GET: api/logs
        [HttpGet]
        public ActionResult<IEnumerable<LogEntry>> GetLogs([FromQuery] LogSearchRequest request)
        {
            var query = _logs.AsQueryable();

            // Фильтрация по уровню
            if (!string.IsNullOrEmpty(request.Level))
            {
                query = query.Where(l => l.Level == request.Level);
            }

            // Фильтрация по источнику
            if (!string.IsNullOrEmpty(request.Source))
            {
                query = query.Where(l => l.Source == request.Source);
            }

            // Поиск по тексту
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(l => l.Message.Contains(request.SearchTerm) ||
                                        (l.Exception != null && l.Exception.Contains(request.SearchTerm)));
            }

            // Сортировка по времени (новые первыми)
            var logs = query.OrderByDescending(l => l.Timestamp).ToList();

            return Ok(new
            {
                Data = logs,
                TotalCount = logs.Count,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = 1
            });
        }

        // GET: api/logs/{id}
        [HttpGet("{id}")]
        public ActionResult<LogEntry> GetLog(int id)
        {
            var log = _logs.FirstOrDefault(l => l.Id == id);

            if (log == null)
            {
                return NotFound();
            }

            return log;
        }

        // POST: api/logs
        [HttpPost]
        public ActionResult<LogEntry> CreateLog(CreateLogRequest request)
        {
            var logEntry = new LogEntry
            {
                Id = _nextId++,
                Level = request.Level,
                Message = request.Message,
                Source = request.Source,
                Exception = request.Exception,
                UserId = request.UserId,
                Properties = request.Properties != null ? System.Text.Json.JsonSerializer.Serialize(request.Properties) : null,
                Timestamp = DateTime.UtcNow
            };

            _logs.Add(logEntry);

            return CreatedAtAction(nameof(GetLog), new { id = logEntry.Id }, logEntry);
        }

        // DELETE: api/logs/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteLog(int id)
        {
            var log = _logs.FirstOrDefault(l => l.Id == id);
            if (log == null)
            {
                return NotFound();
            }

            _logs.Remove(log);
            return NoContent();
        }

        // GET: api/logs/stats
        [HttpGet("stats")]
        public ActionResult GetLogStats()
        {
            var stats = _logs
                .GroupBy(l => l.Level)
                .Select(g => new { Level = g.Key, Count = g.Count() })
                .ToList();

            var totalCount = _logs.Count;
            var recentCount = _logs.Count(l => l.Timestamp >= DateTime.UtcNow.AddHours(-24));

            return Ok(new
            {
                TotalLogs = totalCount,
                RecentLogs = recentCount,
                LogsByLevel = stats
            });
        }
    }
}