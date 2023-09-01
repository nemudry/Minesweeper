namespace MinesweeperGame.Models;

//игровой центр
public class MinesweeperProvider
{
    private readonly List<Minesweeper> minesweepers = new List<Minesweeper>();
    public MinesweeperProvider()
    { }

    //загрузка игры по ID
    public Minesweeper? GetGameById(string id)
    {
        var game = minesweepers.Where(g => g.Game_id == id).FirstOrDefault();
        return game;
    }    
    
    //добавление новой игры 
    public void AddNewGame(Minesweeper minesweeper)
    {
        minesweepers.Add(minesweeper);
    }

    //удаление игры
    public Minesweeper? DeleteGame(string id)
    {
        var game = minesweepers.Where(g => g.Game_id == id).FirstOrDefault();
        if (game != null)
            minesweepers.Remove(game);
        return game;
    }
}
