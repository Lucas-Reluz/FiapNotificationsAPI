using MediatR;
using NotificationsAPI.Application.DTOs;

namespace NotificationsAPI.Application.Queries;

public class GetNotificationByIdQuery : IRequest<NotificationResponse?>
{
    public Guid NotificationId { get; set; }
}
