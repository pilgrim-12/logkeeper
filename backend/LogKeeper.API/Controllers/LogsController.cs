using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogKeeper.API.Data;
using LogKeeper.API.Models;
using System.Text.Json;

namespace LogKeeper.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LogsController : ControllerBase
    {
        private readonly LogContext _context;

        public LogsController(LogContext context)
        {
            _context = context;
        }

        // GET: api/logs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LogEntry>>> GetLogs([FromQuery] LogSearchRequest request)
        {
            var query = _context.LogEntries.AsQueryable();

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

            // Фильтрация по дате
            if (request.FromDate.HasValue)
            {
                query = query.Where(l => l.Timestamp >= request.FromDate.Value);
            }

            if (request.ToDate.HasValue)
            {
                query = query.Where(l => l.Timestamp <= request.ToDate.Value);
            }

            // Сортировка по времени (новые первыми)
            query = query.OrderByDescending(l => l.Timestamp);

            // Пагинация
            var totalCount = await query.CountAsync();
            var logs = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return Ok(new
            {
                Data = logs,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
            });
        }

        // GET: api/logs/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<LogEntry>> GetLog(int id)
        {
            var log = await _context.LogEntries.FindAsync(id);

            if (log == null)
            {
                return NotFound();
            }

            return log;
        }

        // POST: api/logs
        [HttpPost]
        public async Task<ActionResult<LogEntry>> CreateLog(CreateLogRequest request)
        {
            var logEntry = new LogEntry
            {
                Level = request.Level,
                Message = request.Message,
                Source = request.Source,
                Exception = request.Exception,
                UserId = request.UserId,
                Properties = request.Properties != null ? JsonSerializer.Serialize(request.Properties) : null,
                Timestamp = DateTime.UtcNow
            };

            _context.LogEntries.Add(logEntry);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLog), new { id = logEntry.Id }, logEntry);
        }

        // DELETE: api/logs/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLog(int id)
        {
            var log = await _context.LogEntries.FindAsync(id);
            if (log == null)
            {
                return NotFound();
            }

            _context.LogEntries.Remove(log);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/logs/stats
        [HttpGet("stats")]
        public async Task<ActionResult> GetLogStats()
        {
            var stats = await _context.LogEntries
                .GroupBy(l => l.Level)
                .Select(g => new { Level = g.Key, Count = g.Count() })
                .ToListAsync();

            var totalCount = await _context.LogEntries.CountAsync();
            var recentCount = await _context.LogEntries
                .Where(l => l.Timestamp >= DateTime.UtcNow.AddHours(-24))
                .CountAsync();

            return Ok(new
            {
                TotalLogs = totalCount,
                RecentLogs = recentCount,
                LogsByLevel = stats
            });
        }

        // GET: api/logs/health - для проверки работы контроллера
        [HttpGet("health")]
        public async Task<ActionResult> GetHealth()
        {
            try
            {
                var count = await _context.LogEntries.CountAsync();
                return Ok(new
                {
                    Status = "Healthy",
                    DatabaseConnected = true,
                    LogsCount = count,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Status = "Unhealthy",
                    DatabaseConnected = false,
                    Error = ex.Message,
                    Timestamp = DateTime.UtcNow
                });
            }
        }
    }
}