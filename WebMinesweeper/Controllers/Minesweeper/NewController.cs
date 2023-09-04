using Microsoft.AspNetCore.Mvc;
using Games.Models;
using Games.Validation;
using Games.MinesweeperGame.Models;
using Games.Web.ViewModels;
using Games.Web.MinesweeperGame.ViewModels;

namespace Games.Web.Controllers;

[Route("minesweeper/api/[controller]")]
[ApiController]
public class NewController : ControllerBase
{

    private readonly ILogger<NewController> _logger;

    private readonly GamesProvider gamesProvider;
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
            var newGameRequest = new NewGameRequest();
            try
            {
                newGameRequest = Request.ReadFromJsonAsync<NewGameRequest>().Result;
                _logger.Log(LogLevel.Information, "Поступил жсон");
                _logger.Log(LogLevel.Information, $"{newGameRequest.Mines_count} {newGameRequest.Height} {newGameRequest.Width}");
            }
            catch (Exception e)
            {
                return new BadRequestObjectResult(new ErrorResponse(e.Message));
            }

            var minesweeper = new Minesweeper(newGameRequest.Width, newGameRequest.Height, newGameRequest.Mines_count);
            string errors = ValidationGame.ValidateNewGame(minesweeper);
            if (errors != null)
            {
                return new BadRequestObjectResult(new ErrorResponse(errors));
            }
            else
            {
                gamesProvider.AddNewGame(minesweeper); 
                var gameInfoResponse = new GameInfoResponse(minesweeper); 
                return new JsonResult(gameInfoResponse);
            }
        }
        else return new BadRequestObjectResult(new ErrorResponse("В запросе отсутствует JSON"));
    }
}
