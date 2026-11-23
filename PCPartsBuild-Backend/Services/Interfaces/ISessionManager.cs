using System;
using System.Threading.Tasks;
using PCPartsAPI.Models;

namespace PCPartsAPI.Services.Interfaces
{
    public interface ISessionManager
    {
        Task<AssistantSession> CreateSessionAsync(decimal budget, string purpose, string? userId = null);
        Task<AssistantSession?> GetSessionAsync(Guid sessionId);
        Task UpdateSessionAsync(AssistantSession session);
        Task AdvanceStepAsync(AssistantSession session);
    }
}
