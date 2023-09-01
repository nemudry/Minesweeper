using System.Text.Json.Serialization;

namespace MinesweeperGame.Web.ViewModels;

//запрос на ход
public class GameTurnRequest
{
    //id игры
    public string Game_id { get; set; }
    //номер ряда
    public int Row { get; set; }
    //номер колонки
    public int Col { get; set; }

    [JsonConstructor]
    public GameTurnRequest()
    { }
}
