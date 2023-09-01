using Microsoft.AspNetCore.Mvc;
using MinesweeperGame.Models;
using MinesweeperGame.Web.Util;
using MinesweeperGame.Web.ViewModels;

namespace MinesweeperGame.Web.Controllers;

[ApiController]
public class APIController : ControllerBase
{

    private readonly ILogger<APIController> _logger;

    private readonly MinesweeperProvider minesweeperProvider;
    public APIController(ILogger<APIController> logger, MinesweeperProvider minesweeperProvider)
    {   
        _logger = logger;
        this.minesweeperProvider = minesweeperProvider;
    }

    [Route("api/new")]
    [HttpPost]
    public async Task NewGame()
    {
        
        if (Request.HasJsonContentType())
        {
            var newGameRequest = new NewGameRequest();
            try
            {
                newGameRequest = await Request.ReadFromJsonAsync<NewGameRequest>();
                _logger.Log(LogLevel.Information, "Поступил жсон");
                _logger.Log(LogLevel.Information, $"{newGameRequest.Mines_count} {newGameRequest.Height} {newGameRequest.Width}");
            }
            catch (Exception e)
            {
               await SendErrors(e.Message);
            }

            var minesweeper = new Minesweeper(newGameRequest.Width, newGameRequest.Height, newGameRequest.Mines_count);
            string? errors = ValidationMinesweeper.ValidateNewMinesweeper(minesweeper);
            if (errors != null)
            {
                await SendErrors(errors);
            }
            else
            {
                minesweeperProvider.AddNewGame(minesweeper); 
                var gameInfoResponse = new GameInfoResponse(minesweeper);
                await Response.WriteAsJsonAsync(gameInfoResponse);
            }           
        }            
    }

    [Route("api/turn")]
    [HttpPost]
    public async Task TurnGame()
    {
        if (Request.HasJsonContentType())
        {
            var gameTurnRequest = new GameTurnRequest();
            try
            {
                gameTurnRequest = await Request.ReadFromJsonAsync<GameTurnRequest>();
                _logger.Log(LogLevel.Information, "Поступил жсон");
                _logger.Log(LogLevel.Information, $"{gameTurnRequest.Game_id} {gameTurnRequest.Row} {gameTurnRequest.Col}");
            }
            catch (Exception e)
            {
                await SendErrors(e.Message);
            }

            var minesweeper = minesweeperProvider.GetGameById(gameTurnRequest.Game_id);
            string? errors = ValidationMinesweeper.ValidateRequest(minesweeper, gameTurnRequest.Row, gameTurnRequest.Col);
            if (errors != null)
            {
                await SendErrors(errors);
            }
            else
            {
                minesweeper.OpenCell(gameTurnRequest.Row, gameTurnRequest.Col);
                minesweeper.GameEndCheck(gameTurnRequest.Row, gameTurnRequest.Col);
                var gameInfoResponse = new GameInfoResponse(minesweeper);
                await Response.WriteAsJsonAsync(gameInfoResponse);
            }
        }
    }

    private async Task SendErrors (string errors)
    {
        _logger.Log(LogLevel.Information, errors);
        Response.StatusCode = 400;
        await Response.WriteAsJsonAsync(new ErrorResponse(errors));
    }
}
