using System.ComponentModel.DataAnnotations;
using Games.Models;

namespace Games.Validation;

public class ValidationGame
{    
    //валидация новой игры
    public static string? ValidateNewGame(Game game)
    {
        string? errors = null;
        var results = new List<ValidationResult>();
        var context = new ValidationContext(game);
        if (!Validator.TryValidateObject(game, context, results, true))
            errors = string.Join("\n", results);
        return errors;
    }
}
