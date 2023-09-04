using Games.MinesweeperGame.Models;
using System.Text.Json.Serialization;

namespace Games.Web.MinesweeperGame.ViewModels;

//информация об игре
public class GameInfoResponse
{
    //id игры
    public string Game_id { get; set; }
    //ширина поля
    public int Width { get; set; }
    //высота поля
    public int Height { get; set; }
    //количество мин
    public int Mines_count { get; set; }
    //статус игры
    public bool Completed { get; set; }
    //игровое поле
    public List<List<string>> Field { get; set; }

    public GameInfoResponse(Minesweeper minesweeper)
    {
        Game_id = minesweeper.Game_id;
        Width = minesweeper.Width;
        Height = minesweeper.Height;
        Mines_count = minesweeper.Mines_count;
        Completed = minesweeper.Completed;
        Field = new List<List<string>>(Height);
        GetField(minesweeper.Field);
    }

    [JsonConstructor]
    public GameInfoResponse()
    {    }

    //преобразование значения клеток Cell в string, сокрытие закрытых клеток
    private void GetField(List<List<Cell>> minesweeperField)
    {
        for (int row = 0; row < Height; row++)
        {
            List<string> column = new List<string>(Width);
            for (int col = 0; col < Width; col++)
            {
                var cell = minesweeperField[row][col];
                if (!cell.IsOpened)
                    column.Add(" "); //сокрытие закрытых клеток
                else
                    column.Add(cell.CellValue);
            }
            Field.Add(column);
        }
    }
}
