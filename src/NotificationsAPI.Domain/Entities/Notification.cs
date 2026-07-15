using NotificationsAPI.Domain.Enums;

namespace NotificationsAPI.Domain.Entities;

public class Notification
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid? OrderId { get; private set; }
    public NotificationType Type { get; private set; }
    public string Title { get; private set; }
    public string Message { get; private set; }
    public NotificationStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReadAt { get; private set; }

    private Notification() { }

    public Notification(
        Guid userId,
        Guid? orderId,
        NotificationType type,
        string title,
        string message)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        OrderId = orderId;
        Type = type;
        Title = title;
        Message = message;
        Status = NotificationStatus.Unread;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsRead()
    {
        if (Status == NotificationStatus.Unread)
        {
            Status = NotificationStatus.Read;
            ReadAt = DateTime.UtcNow;
        }
    }
}
