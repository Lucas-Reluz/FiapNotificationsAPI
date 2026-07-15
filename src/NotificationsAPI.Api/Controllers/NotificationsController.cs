using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationsAPI.Application.Commands;
using NotificationsAPI.Application.Queries;

namespace NotificationsAPI.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(IMediator mediator, ILogger<NotificationsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetNotificationById(Guid id)
    {
        try
        {
            var query = new GetNotificationByIdQuery { NotificationId = id };
            var notification = await _mediator.Send(query);

            if (notification == null)
                return NotFound(new { message = "Notificação não encontrada" });

            return Ok(notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar notificação {NotificationId}", id);
            return StatusCode(500, new { message = "Erro interno ao buscar notificação" });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserNotifications(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var query = new GetUserNotificationsQuery
            {
                UserId = userId,
                Page = page,
                PageSize = pageSize
            };

            var response = await _mediator.Send(query);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar notificações do usuário {UserId}", userId);
            return StatusCode(500, new { message = "Erro interno ao buscar notificações" });
        }
    }

    [HttpPatch("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        try
        {
            var command = new MarkNotificationAsReadCommand { NotificationId = id };
            var success = await _mediator.Send(command);

            if (!success)
                return NotFound(new { message = "Notificação não encontrada" });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao marcar notificação {NotificationId} como lida", id);
            return StatusCode(500, new { message = "Erro interno ao marcar notificação como lida" });
        }
    }
}
