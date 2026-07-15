using MediatR;
using NotificationsAPI.Application.DTOs;
using NotificationsAPI.Application.Queries;
using NotificationsAPI.Domain.Interfaces;

namespace NotificationsAPI.Application.Handlers;

public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, NotificationsListResponse>
{
    private readonly INotificationRepository _repository;

    public GetUserNotificationsQueryHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<NotificationsListResponse> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
    {
        var notifications = await _repository.GetByUserIdAsync(request.UserId, request.Page, request.PageSize);
        var totalCount = await _repository.CountByUserIdAsync(request.UserId);
        var unreadCount = await _repository.CountUnreadByUserIdAsync(request.UserId);

        return new NotificationsListResponse
        {
            Notifications = notifications.Select(n => new NotificationResponse
            {
                Id = n.Id,
                UserId = n.UserId,
                OrderId = n.OrderId,
                Type = n.Type.ToString(),
                Title = n.Title,
                Message = n.Message,
                Status = n.Status.ToString(),
                CreatedAt = n.CreatedAt,
                ReadAt = n.ReadAt
            }).ToList(),
            TotalCount = totalCount,
            UnreadCount = unreadCount,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
