namespace Games.Models;

//игровой центр
public class GamesProvider
{
    private readonly List<Game> games = new List<Game>();
    public GamesProvider()
    { }

    //загрузка игры по ID
    public Game? GetGameById(string id)
    {
        var game = games.Where(g => g.Game_id == id).FirstOrDefault();
        return game;
    }    
    
    //добавление новой игры 
    public void AddNewGame(Game game)
    {
        games.Add(game);
    }

    //удаление игры
    public Game? DeleteGame(string id)
    {
        var game = games.Where(g => g.Game_id == id).FirstOrDefault();
        if (game != null)
            games.Remove(game);
        return game;
    }
}
