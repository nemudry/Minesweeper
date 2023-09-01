using System.ComponentModel.DataAnnotations;
using MinesweeperGame.Models;

namespace MinesweeperGame.Web.Util;

//валидация сапера
public static class ValidationMinesweeper
{
    //валидация новой игры
    public static string? ValidateNewMinesweeper(Minesweeper minesweeper)
    {
        string? errors;
        var results = new List<ValidationResult>();
        var context = new ValidationContext(minesweeper);
        if (Validator.TryValidateObject(minesweeper, context, results, true))
            errors = null;
        else
            errors = string.Join("\n", results);
        return errors;
    }

    //валидация запроса 
    public static string? ValidateRequest(Minesweeper minesweeper, int row, int col)
    {
        string? errors;
        errors = ValidateCompliteMinesweeper(minesweeper);
        if (errors != null)
            return errors;
        errors = ValidateOpenCell(minesweeper.Field[row][col]);
        return errors;
    }

    //валидация на завершение игры
    public static string? ValidateCompliteMinesweeper(Minesweeper minesweeper)
    {
        string? errors;
        if (!minesweeper.Completed)
            errors = null;
        else
            errors = "Игра завершена";
        return errors;
    }

    //валидация на уже открытую клетку
    public static string? ValidateOpenCell(Cell cell)
    {
        string? errors;
        if (!cell.IsOpened)
            errors = null;
        else
            errors = "Уже открытая ячейка";
        return errors;
    }
}
