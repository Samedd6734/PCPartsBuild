using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PCPartsAPI.Data;
using PCPartsAPI.Enums;
using PCPartsAPI.Models;
using PCPartsAPI.Services.Interfaces;

namespace PCPartsAPI.Services
{
    public class SessionManager : ISessionManager
    {
        private readonly ApplicationDbContext _context;

        public SessionManager(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AssistantSession> CreateSessionAsync(decimal budget, string purpose, string? userId = null)
        {
            var session = new AssistantSession
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TotalBudget = budget,
                RemainingBudget = budget,
                Purpose = purpose,
                CurrentStep = ComponentStep.CPU,
                SelectedComponentsJson = "{}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.AssistantSessions.Add(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<AssistantSession?> GetSessionAsync(Guid sessionId)
        {
            return await _context.AssistantSessions.FindAsync(sessionId);
        }

        public async Task UpdateSessionAsync(AssistantSession session)
        {
            session.UpdatedAt = DateTime.UtcNow;
            _context.AssistantSessions.Update(session);
            await _context.SaveChangesAsync();
        }

        public async Task AdvanceStepAsync(AssistantSession session)
        {
            if (session.CurrentStep < ComponentStep.Completed)
            {
                session.CurrentStep = (ComponentStep)((int)session.CurrentStep + 1);
            }
            await UpdateSessionAsync(session);
        }
    }
}
