using System.Text.Json.Serialization;

namespace MinesweeperGame.Web.ViewModels;

//ответ ошибка
public class ErrorResponse
{
    //описание ошибки
    public string Error { get; set; }
    public ErrorResponse(string text)
    {
        Error = text;    
    }

    [JsonConstructor]
    public ErrorResponse()
    { }
}
