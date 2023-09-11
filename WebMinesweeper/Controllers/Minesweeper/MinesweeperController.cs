using Microsoft.AspNetCore.Mvc;
using Games.Models;
using Games.Validation;
using Games.MinesweeperGame.Models;
using Games.Web.MinesweeperGame.ViewModels;
using static Games.Web.Util.ErrorsHandler;
using Games.MinesweeperGame.Validation;

namespace Games.Web.Controllers;

// контроллер для сапера
[ApiController]
public class MinesweeperController : ControllerBase
{

    private readonly ILogger<MinesweeperController> _logger; // лог
    private readonly GamesProvider gamesProvider; // игровой центр
    public MinesweeperController(ILogger<MinesweeperController> logger, GamesProvider gamesProvider)
    {
        _logger = logger;
        this.gamesProvider = gamesProvider;
    }

    //новая игра
    [Route("minesweeper/new")]
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

    // игровой ход
    [Route("minesweeper/turn")]
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

            minesweeper.Start(gameTurnRequest.Row, gameTurnRequest.Col); //запуск игры
            _logger.LogInformation($"{DateTime.Now}. Cовершен ход: {minesweeper.Game_id}, {gameTurnRequest.Row}, " +
                $"{gameTurnRequest.Col}. Статус завершения игры: {minesweeper.Completed}");
            return new JsonResult(new GameInfoResponse(minesweeper)); // отправить поле сапера клиенту
        }
        else
            return LogAndFormError400(_logger, "Игре не присвоен ID", "В запросе отсутствует JSON");
    }
}
