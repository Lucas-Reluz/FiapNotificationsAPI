using Microsoft.EntityFrameworkCore;
using NotificationsAPI.Domain.Entities;
using NotificationsAPI.Domain.Enums;
using NotificationsAPI.Domain.Interfaces;
using NotificationsAPI.Infrastructure.Data;

namespace NotificationsAPI.Infrastructure.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationsDbContext _context;

    public NotificationRepository(NotificationsDbContext context)
    {
        _context = context;
    }

    public async Task<Notification?> GetByIdAsync(Guid id)
    {
        return await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, int page, int pageSize)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountByUserIdAsync(Guid userId)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId);
    }

    public async Task<int> CountUnreadByUserIdAsync(Guid userId)
    {
        return await _context.Notifications
            .CountAsync(n => n.UserId == userId && n.Status == NotificationStatus.Unread);
    }

    public async Task AddAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Notification notification)
    {
        _context.Notifications.Update(notification);
        await _context.SaveChangesAsync();
    }
}
