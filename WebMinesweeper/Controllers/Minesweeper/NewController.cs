using Microsoft.AspNetCore.Mvc;
using Games.Models;
using Games.Validation;
using Games.MinesweeperGame.Models;
using Games.Web.MinesweeperGame.ViewModels;
using static Games.Web.Util.ErrorsHandler;

namespace Games.Web.Controllers;

// ����� ����
[Route("minesweeper/api/[controller]")]
[ApiController]
public class NewController : ControllerBase
{

    private readonly ILogger<NewController> _logger; // ���
    private readonly GamesProvider gamesProvider; // ������� �����
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
            //��������� ������� �� �������
            NewGameRequest? newGameRequest = null;
            try
            {
                newGameRequest = Request.ReadFromJsonAsync<NewGameRequest>().Result;
                _logger.LogInformation($"{DateTime.Now}. ������ �� �������� ����: {newGameRequest?.Mines_count}, {newGameRequest?.Height}, {newGameRequest?.Width}.");
            }
            catch (Exception e)
            {
                return LogAndFormError400(_logger, "���� �� �������� ID", e.Message);
            }
            if (newGameRequest == null)
                return LogAndFormError400(_logger, "���� �� �������� ID", "������ �� ��� ������������.");

            //�������� ����� ���� � ���������
            Minesweeper minesweeper = new Minesweeper(newGameRequest.Width, newGameRequest.Height, newGameRequest.Mines_count);
            string? errors = ValidationGame.ValidateNewGame(minesweeper);
            if (errors != null)
                return LogAndFormError400(_logger, "���� �� �������� ID", errors);

            //���������� ����� ���� � ������� �����
            gamesProvider.AddNewGame(minesweeper);
            _logger.LogInformation($"{DateTime.Now}. ������� ����� ���� Id {minesweeper.Game_id}");
            // ��������� ���� ������ �������
            return new JsonResult(new GameInfoResponse(minesweeper));
        }
        else return LogAndFormError400(_logger, "���� �� �������� ID", "� ������� ����������� JSON");
    }
}
