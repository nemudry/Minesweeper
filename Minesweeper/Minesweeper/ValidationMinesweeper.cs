using Games.MinesweeperGame.Models;

namespace Games.MinesweeperGame.Validation;

//валидация сапера
public static class ValidationMinesweeper
{
    //валидация запроса 
    public static string? ValidateInfoRequestMinesweeper(Minesweeper minesweeper, int row, int col)
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
