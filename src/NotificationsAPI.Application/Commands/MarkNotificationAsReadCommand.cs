using MediatR;

namespace NotificationsAPI.Application.Commands;

public class MarkNotificationAsReadCommand : IRequest<bool>
{
    public Guid NotificationId { get; set; }
}
