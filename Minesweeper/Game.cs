namespace Games.Models;

public abstract class Game
{
    //id
    public string Game_id { get; }
    public Game ()
    {
        Game_id = Guid.NewGuid().ToString();
    }
}
