using Microsoft.AspNetCore.Mvc;
using Games.Models;
using Games.Validation;
using Games.MinesweeperGame.Models;
using Games.Web.MinesweeperGame.ViewModels;
using static Games.Web.Util.ErrorsHandler;
using Games.MinesweeperGame.Validation;

namespace Games.Web.Controllers;

// ���������� ��� ������
[ApiController]
public class MinesweeperController : ControllerBase
{

    private readonly ILogger<MinesweeperController> _logger; // ���
    private readonly GamesProvider gamesProvider; // ������� �����
    public MinesweeperController(ILogger<MinesweeperController> logger, GamesProvider gamesProvider)
    {
        _logger = logger;
        this.gamesProvider = gamesProvider;
    }

    //����� ����
    [Route("minesweeper/new")]
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

    // ������� ���
    [Route("minesweeper/turn")]
    [HttpPost]
    public IActionResult TurnGame()
    {
        if (Request.HasJsonContentType())
        {
            //��������� ������� �� �������
            GameTurnRequest? gameTurnRequest = null;
            try
            {
                gameTurnRequest = Request.ReadFromJsonAsync<GameTurnRequest>().Result;
                _logger.LogInformation($"{DateTime.Now}. ������ �� ������� ���: {gameTurnRequest?.Game_id}, {gameTurnRequest?.Row}, {gameTurnRequest?.Col}.");
            }
            catch (Exception e)
            {
                return LogAndFormError400(_logger, gameTurnRequest?.Game_id, e.Message);
            }
            if (gameTurnRequest == null)
                return LogAndFormError400(_logger, null, "JSON �� ��� ������������.");

            //��������� ���� �� �������� ������
            Minesweeper? minesweeper = gamesProvider.GetGameById(gameTurnRequest.Game_id) as Minesweeper;
            if (minesweeper == null)
                return LogAndFormError400(_logger, gameTurnRequest.Game_id, "���� �� �������");

            //��������� �������
            string? errors = ValidationMinesweeper.ValidateInfoRequestMinesweeper(minesweeper, gameTurnRequest.Row, gameTurnRequest.Col);
            if (errors != null)
                return LogAndFormError400(_logger, minesweeper.Game_id, errors);

            minesweeper.Start(gameTurnRequest.Row, gameTurnRequest.Col); //������ ����
            _logger.LogInformation($"{DateTime.Now}. C������� ���: {minesweeper.Game_id}, {gameTurnRequest.Row}, " +
                $"{gameTurnRequest.Col}. ������ ���������� ����: {minesweeper.Completed}");
            return new JsonResult(new GameInfoResponse(minesweeper)); // ��������� ���� ������ �������
        }
        else
            return LogAndFormError400(_logger, "���� �� �������� ID", "� ������� ����������� JSON");
    }
}
