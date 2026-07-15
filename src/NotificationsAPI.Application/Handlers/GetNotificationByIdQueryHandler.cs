using MediatR;
using NotificationsAPI.Application.DTOs;
using NotificationsAPI.Application.Queries;
using NotificationsAPI.Domain.Interfaces;

namespace NotificationsAPI.Application.Handlers;

public class GetNotificationByIdQueryHandler : IRequestHandler<GetNotificationByIdQuery, NotificationResponse?>
{
    private readonly INotificationRepository _repository;

    public GetNotificationByIdQueryHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<NotificationResponse?> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
    {
        var notification = await _repository.GetByIdAsync(request.NotificationId);

        if (notification == null)
            return null;

        return new NotificationResponse
        {
            Id = notification.Id,
            UserId = notification.UserId,
            OrderId = notification.OrderId,
            Type = notification.Type.ToString(),
            Title = notification.Title,
            Message = notification.Message,
            Status = notification.Status.ToString(),
            CreatedAt = notification.CreatedAt,
            ReadAt = notification.ReadAt
        };
    }
}
