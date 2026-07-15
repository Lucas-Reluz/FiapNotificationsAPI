using NotificationsAPI.Domain.Entities;

namespace NotificationsAPI.Domain.Interfaces;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(Guid id);
    Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, int page, int pageSize);
    Task<int> CountByUserIdAsync(Guid userId);
    Task<int> CountUnreadByUserIdAsync(Guid userId);
    Task AddAsync(Notification notification);
    Task UpdateAsync(Notification notification);
}
