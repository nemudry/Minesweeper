using Microsoft.AspNetCore.Mvc;
using Games.Models;
using Games.MinesweeperGame.Models;
using Games.MinesweeperGame.Validation;
using Games.Web.MinesweeperGame.ViewModels;
using static Games.Web.Util.ErrorsHandler;

namespace Games.Web.Controllers;

// игровой ход
[Route("minesweeper/api/[controller]")]
[ApiController]
public class TurnController : ControllerBase
{
    private readonly ILogger<TurnController> _logger; // лог
    private readonly GamesProvider gamesProvider; // игровой центр
    public TurnController(ILogger<TurnController> logger, GamesProvider gamesProvider)
    {   
        _logger = logger;
        this.gamesProvider = gamesProvider;
    }

    [HttpPost]
    public IActionResult TurnGame()
    {
        if (Request.HasJsonContentType())
        {
            //получение запроса от клиента
            GameTurnRequest? gameTurnRequest = null;
            try
            {
                gameTurnRequest = Request.ReadFromJsonAsync<GameTurnRequest>().Result;
                _logger.LogInformation($"{DateTime.Now}. Запрос на игровой ход: {gameTurnRequest?.Game_id}, {gameTurnRequest?.Row}, {gameTurnRequest?.Col}.");
            }
            catch (Exception e)
            {
                return LogAndFormError400(_logger, gameTurnRequest?.Game_id, e.Message);
            }
            if (gameTurnRequest == null) 
                return LogAndFormError400(_logger, null, "JSON на ход некорректный.");

            //получение игры из игрового центра
            Minesweeper? minesweeper = gamesProvider.GetGameById(gameTurnRequest.Game_id) as Minesweeper;
            if (minesweeper == null) 
                return LogAndFormError400(_logger, gameTurnRequest.Game_id, "Игра не найдена");

            //валидация запроса
            string? errors = ValidationMinesweeper.ValidateInfoRequestMinesweeper(minesweeper, gameTurnRequest.Row, gameTurnRequest.Col);
            if (errors != null)
                return LogAndFormError400(_logger, minesweeper.Game_id, errors);

            minesweeper.OpenCell(gameTurnRequest.Row, gameTurnRequest.Col); //открыть ячейку
            minesweeper.GameEndCheck(gameTurnRequest.Row, gameTurnRequest.Col); // проверить на окончание игры

            _logger.LogInformation($"{DateTime.Now}. Cовершен ход: {minesweeper.Game_id}, {gameTurnRequest.Row}, " +
                $"{gameTurnRequest.Col}. Статус завершения игры: {minesweeper.Completed}");
            return new JsonResult(new GameInfoResponse(minesweeper)); // отправить поле сапера клиенту
        }
        else 
            return LogAndFormError400(_logger, "Игре не присвоен ID", "В запросе отсутствует JSON");
    }
}
