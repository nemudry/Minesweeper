using System.ComponentModel.DataAnnotations;
using Games.Models;

namespace Games.MinesweeperGame.Models; 

public class Minesweeper: Game
{
    //ширина
    [Range(2, 30, ErrorMessage = "Ширина поля должна быть не менее {1} и не более {2}")]
    public int Width { get; }
    //высота
    [Range(2, 30, ErrorMessage = "Высота поля должна быть не менее {1} и не более {2}")]
    public int Height { get; }
    //количество мин
    [Range(1, 899, ErrorMessage = "Количество мин должно быть больше 1 и не более количества клеток - 1")]
    public int Mines_count { get; }
    //статус игры
    public bool Completed { get; private set; }
    //игровое поле
    public List<List<Cell>> Field { get; private set; }

    public Minesweeper(int width, int height, int minesCount)
        : base ()
    {
        Width = width;
        Height = height;
        Mines_count = minesCount < (Height * Width) ? minesCount : 0; // если кол-во мин больше количества клеток - зануление мин
        Completed = false;
        Field = new List<List<Cell>>(Height);
        SetMinesweeper();
    }

    //установка значений игрового поля
    private void SetMinesweeper()
    {
        SetEmptyCells(); //установить пустые клетки
        SetMines();//рандомно установить мины
        SetMinesNumbers();//установка числовых ячеек
    }

    //установить пустые клетки
    private void SetEmptyCells()
    {
        for (int row = 0; row < Height; row++)
        {
            List<Cell> column = new List<Cell>(Width);
            for (int col = 0; col < Width; col++)
            {
                column.Add(new Cell(row, col));
            }
            Field.Add(column);
        }
    }

    //рандомно установить мины
    private void SetMines()
    {
        //все клетки
        List<Cell> allCels = new();
        foreach (var row in Field)
            foreach (var cell in row)
                allCels.Add(cell);

        //клетки без мин
        var withoutMines = allCels.Where(c => c.CellValue != "X");

        var rand = new Random();
        for (int i = 0; i < Mines_count; i++)
        {
            //рандомная клетка
            var cell = withoutMines.ElementAt(rand.Next(0, withoutMines.Count()));
            cell.CellValue = "X";
        }
    }

    //установка числовых ячеек
    private void SetMinesNumbers()
    {
        foreach (var row in Field)
        {
            foreach (var cell in row)
            {
                if (cell.CellValue != "X")
                {
                    //получить все доступные смежные клетки
                    var aroundcells = GetAroundCells(cell);
                    //число мин в смежных ячейках
                    cell.CellValue = aroundcells.Where(c => c.CellValue == "X").Count().ToString();
                }
            }
        }
    }

    //получить все доступные смежныек клетки поля вокруг определенной клетки
    private List<Cell> GetAroundCells(Cell cell)
    {
        List<Cell> aroundCells = new();
        int nextRow = cell.CellRow + 1;
        int nextColumn = cell.CellColumn + 1;
        int previosRow = cell.CellRow - 1;
        int previosColumn = cell.CellColumn - 1;

        if (previosColumn >= 0)
        {
            aroundCells.Add(Field[cell.CellRow][previosColumn]);
        }
        if (nextColumn < Width)
        {
            aroundCells.Add(Field[cell.CellRow][nextColumn]);
        }
        if (previosRow >= 0)
        {
            aroundCells.Add(Field[previosRow][cell.CellColumn]);

            if (previosColumn >= 0)
            {
                aroundCells.Add(Field[previosRow][previosColumn]);
            }
            if (nextColumn < Width)
            {
                aroundCells.Add(Field[previosRow][nextColumn]);
            }
        }
        if (nextRow < Height)
        {
            aroundCells.Add(Field[nextRow][cell.CellColumn]);

            if (previosColumn >= 0)
            {
                aroundCells.Add(Field[nextRow][previosColumn]);
            }
            if (nextColumn < Width)
            {
                aroundCells.Add(Field[nextRow][nextColumn]);
            }
        }
        return aroundCells;
    }

    //Изменить значение мин с Х на М
    private void ChangeMinesValue()
    {        
        foreach (var row in Field)
        {
            foreach (var cell in row)
            {
                if (cell.CellValue == "X")
                    cell.CellValue = "M";
            }
        }
    }    
    
    //открыть все клетки
    private void OpenAllCells()
    {
        foreach (var row in Field)
        {
            foreach (var cell in row)
            {
                if (!cell.IsOpened)
                    cell.IsOpened = true;
            }
        }
    }

    //открыть клетку
    private void OpenCell(int row, int col)
    {
        var cell = Field[row][col];
        cell.IsOpened = true;
        //нулевое значение - открыть смежные клетки
        if (cell.CellValue == "0")
        {                    
            //получить все доступные смежные клетки
            var aroundCells = GetAroundCells(cell);
            //открыть все клетки без мин
            foreach (var c in aroundCells.Where(c => c.CellValue != "X"))
            {
                if (!c.IsOpened)
                    OpenCell(c.CellRow, c.CellColumn);
            }        
        }   
    }

    //конец игры
    private void GameEndCheck(int row, int col)
    {
        var cell = Field[row][col];
        //подорвался
        if (cell.CellValue == "X")
        {
            Completed = true;
            OpenAllCells();
        }
        else
        {
            //количество неоткрытых клеток
            int unopenCells = 0;
            foreach (var ro in Field)
            {
                foreach (var c in ro)
                {
                    if (!c.IsOpened) unopenCells++;
                }
            }
            //все разминировано
            if (unopenCells == Mines_count)
            {
                Completed = true; 
                ChangeMinesValue(); //Изменить значение мин с Х на М
                OpenAllCells();
            }
        }
    }

    //запуск игры
    public void Start(int row, int col)
    {
        OpenCell(row, col); //открыть ячейку
        GameEndCheck(row, col); // проверить на окончание игры
    }
}
