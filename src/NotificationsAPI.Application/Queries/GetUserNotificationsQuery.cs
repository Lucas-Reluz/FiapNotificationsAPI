using MediatR;
using NotificationsAPI.Application.DTOs;

namespace NotificationsAPI.Application.Queries;

public class GetUserNotificationsQuery : IRequest<NotificationsListResponse>
{
    public Guid UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
