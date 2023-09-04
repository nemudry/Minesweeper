using Microsoft.AspNetCore.Mvc;
using Games.Models;
using Games.MinesweeperGame.Models;
using Games.MinesweeperGame.Validation;
using Games.Web.ViewModels;
using Games.Web.MinesweeperGame.ViewModels;


namespace Games.Web.Controllers;

[Route("minesweeper/api/[controller]")]
[ApiController]
public class TurnController : ControllerBase
{

    private readonly ILogger<TurnController> _logger;

    private readonly GamesProvider gamesProvider;
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
            var gameTurnRequest = new GameTurnRequest();
            try
            {
                gameTurnRequest = Request.ReadFromJsonAsync<GameTurnRequest>().Result;
                _logger.Log(LogLevel.Information, "Поступил жсон");
                _logger.Log(LogLevel.Information, $"{gameTurnRequest.Game_id} {gameTurnRequest.Row} {gameTurnRequest.Col}");
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new ErrorResponse(e.Message));
            }

            var minesweeper = (Minesweeper)gamesProvider.GetGameById(gameTurnRequest.Game_id);
            string? errors = ValidationMinesweeper.ValidateInfoRequestMinesweeper(minesweeper, gameTurnRequest.Row, gameTurnRequest.Col);
            if (errors != null)
            {
                return new BadRequestObjectResult(new ErrorResponse(errors));
            }

            minesweeper.OpenCell(gameTurnRequest.Row, gameTurnRequest.Col);
            minesweeper.GameEndCheck(gameTurnRequest.Row, gameTurnRequest.Col);
            var gameInfoResponse = new GameInfoResponse(minesweeper);
            return new JsonResult(gameInfoResponse);
        }

        else return new BadRequestObjectResult(new ErrorResponse("В запросе отсутствует JSON"));
    }

}
