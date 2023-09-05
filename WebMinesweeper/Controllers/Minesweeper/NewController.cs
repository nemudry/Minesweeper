using Microsoft.AspNetCore.Mvc;
using Games.Models;
using Games.Validation;
using Games.MinesweeperGame.Models;
using Games.Web.MinesweeperGame.ViewModels;
using static Games.Web.Util.ErrorsHandler;

namespace Games.Web.Controllers;

// новая игра
[Route("minesweeper/api/[controller]")]
[ApiController]
public class NewController : ControllerBase
{

    private readonly ILogger<NewController> _logger; // лог
    private readonly GamesProvider gamesProvider; // игровой центр
    public NewController(ILogger<NewController> logger, GamesProvider gamesProvider)
    {
        _logger = logger;
        this.gamesProvider = gamesProvider;
    }

    [HttpPost]
    public IActionResult NewMinesweeperGame()
    {        
        if (Request.HasJsonContentType())
        {
            //получение запроса от клиента
            NewGameRequest? newGameRequest = null;
            try
            {
                newGameRequest = Request.ReadFromJsonAsync<NewGameRequest>().Result;
                _logger.LogInformation($"{DateTime.Now}. Запрос на создание игры: {newGameRequest?.Mines_count}, {newGameRequest?.Height}, {newGameRequest?.Width}.");
            }
            catch (Exception e)
            {
                return LogAndFormError400(_logger, "Игре не присвоен ID", e.Message);
            }
            if (newGameRequest == null)
                return LogAndFormError400(_logger, "Игре не присвоен ID", "Запрос на ход некорректный.");

            //создание новой игры и валидация
            Minesweeper minesweeper = new Minesweeper(newGameRequest.Width, newGameRequest.Height, newGameRequest.Mines_count);
            string? errors = ValidationGame.ValidateNewGame(minesweeper);
            if (errors != null)
                return LogAndFormError400(_logger, "Игре не присвоен ID", errors);

            //добавление новой игры в игровой центр
            gamesProvider.AddNewGame(minesweeper);
            _logger.LogInformation($"{DateTime.Now}. Создана новая игра Id {minesweeper.Game_id}");
            // отправить поле сапера клиенту
            return new JsonResult(new GameInfoResponse(minesweeper));
        }
        else return LogAndFormError400(_logger, "Игре не присвоен ID", "В запросе отсутствует JSON");
    }
}
