using System.Text.Json.Serialization;

namespace Games.Web.MinesweeperGame.ViewModels;

//запрос на новую игру
public class NewGameRequest
{
    //ширина поля
    public int Width { get; set; }
    //высота поля
    public int Height { get; set; }
    //количество мин
    public int Mines_count { get; set; }

    [JsonConstructor]
    public NewGameRequest()
    { }
}
