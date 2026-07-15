using MediatR;
using NotificationsAPI.Application.Commands;
using NotificationsAPI.Domain.Interfaces;

namespace NotificationsAPI.Application.Handlers;

public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand, bool>
{
    private readonly INotificationRepository _repository;

    public MarkNotificationAsReadCommandHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var notification = await _repository.GetByIdAsync(request.NotificationId);

        if (notification == null)
            return false;

        notification.MarkAsRead();
        await _repository.UpdateAsync(notification);

        return true;
    }
}
