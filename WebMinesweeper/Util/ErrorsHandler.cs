using Games.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Games.Web.Util;

//обработчик ошибок
public static class ErrorsHandler
{
    public static IActionResult LogAndFormError400(ILogger logger, string? game_id, string errors)
    {
        logger.LogError($"{DateTime.Now}. Ошибка! {game_id ?? "Id неизвестно"}. {errors}");
        return new BadRequestObjectResult(new ErrorResponse(errors));
    }
}
