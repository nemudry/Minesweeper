using MinesweeperGame.Models;

namespace MinesweeperGame.ConsoleTest;

 public static class Programm
{
    static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("Hello, World!");
            var game = new Minesweeper(10, 10, 15);

            while (!game.Completed)
            {
                Console.Clear();                
                for (int i = 0; i < game.Height; i++)
                {
                    for (int j = 0; j < game.Width; j++)
                    {
                        var cell = game.Field[i][j];
                        if (cell.IsOpened)
                            Color.GreenShort(cell.CellValue);
                        else
                            Console.Write(cell.CellValue);
                        Console.Write("\t");
                    }
                    Console.WriteLine();
                }

                int row = ValidatorInput.GetChechedAnswer("Введите номер ряда:", new string[game.Height]);
                int column = ValidatorInput.GetChechedAnswer("Введите номер колонки:", new string[game.Width]);

                var choosenCell = game.Field[row][column];
                game.OpenCell(choosenCell.CellRow, choosenCell.CellColumn);
                game.GameEndCheck(choosenCell.CellRow, choosenCell.CellColumn);
                Feedback.AcceptPlayer();
            }
            Console.WriteLine("Игра закончена");
            for (int i = 0; i < game.Height; i++)
            {
                for (int j = 0; j < game.Width; j++)
                {
                    var cell = game.Field[i][j];
                    if (cell.IsOpened)
                        Color.GreenShort(cell.CellValue);
                    else
                        Console.Write(cell.CellValue);
                    Console.Write("\t");
                }
                Console.WriteLine();
            }
        }


        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine(ex.TargetSite);
            Console.WriteLine(ex.Source);
            Console.WriteLine(ex.InnerException?.Message);
        }
    }
}