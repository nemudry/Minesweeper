using Microsoft.AspNetCore.Mvc;
using Games.Models;
using Games.MinesweeperGame.Models;
using Games.MinesweeperGame.Validation;
using Games.Web.MinesweeperGame.ViewModels;
using static Games.Web.Util.ErrorsHandler;

namespace Games.Web.Controllers;

// ������� ���
[Route("minesweeper/api/[controller]")]
[ApiController]
public class TurnController : ControllerBase
{
    private readonly ILogger<TurnController> _logger; // ���
    private readonly GamesProvider gamesProvider; // ������� �����
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

            minesweeper.OpenCell(gameTurnRequest.Row, gameTurnRequest.Col); //������� ������
            minesweeper.GameEndCheck(gameTurnRequest.Row, gameTurnRequest.Col); // ��������� �� ��������� ����

            _logger.LogInformation($"{DateTime.Now}. C������� ���: {minesweeper.Game_id}, {gameTurnRequest.Row}, " +
                $"{gameTurnRequest.Col}. ������ ���������� ����: {minesweeper.Completed}");
            return new JsonResult(new GameInfoResponse(minesweeper)); // ��������� ���� ������ �������
        }
        else 
            return LogAndFormError400(_logger, "���� �� �������� ID", "� ������� ����������� JSON");
    }
}
