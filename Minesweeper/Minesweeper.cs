namespace MinesweeperGame.Models; 

public class Minesweeper
{
    //id
    public string Game_id { get; }
    //ширина
    public int Width { get; }
    //высота
    public int Height { get; }
    //количество мин
    public int Mines_count { get; }
    //статус игры
    public bool Completed { get; private set; }
    //игровое поле
    public Cell[,] Field { get; private set; }

    public Minesweeper(int width, int height, int minesCount)
    {
        Game_id = Guid.NewGuid().ToString();
        Width = width;
        Height = height;
        Mines_count = minesCount;
        Completed = false;
        Field = new Cell[Height, Width];
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
            for (int column = 0; column < Width; column++)
            {
                Field[row, column] = new Cell(row, column);
            }
        }
    }

    //рандомно установить мины
    private void SetMines()
    {
        var rand = new Random();
        for (int i = 0; i < Mines_count; )
        {
            //рандомная клетка
            var cell = Field[rand.Next(0, Height), rand.Next(0, Width)];
            if (cell.CellValue != "X")
            {
                cell.CellValue = "X";
                i++;
            }
        }
    }

    //установка числовых ячеек
    private void SetMinesNumbers()
    {
        foreach (Cell cell in Field)
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
            aroundCells.Add(Field[cell.CellRow, previosColumn]);
        }
        if (nextColumn < Width)
        {
            aroundCells.Add(Field[cell.CellRow, nextColumn]);
        }
        if (previosRow >= 0)
        {
            aroundCells.Add(Field[previosRow, cell.CellColumn]);

            if (previosColumn >= 0)
            {
                aroundCells.Add(Field[previosRow, previosColumn]);
            }
            if (nextColumn < Width)
            {
                aroundCells.Add(Field[previosRow, nextColumn]);
            }
        }
        if (nextRow < Height)
        {
            aroundCells.Add(Field[nextRow, cell.CellColumn]);

            if (previosColumn >= 0)
            {
                aroundCells.Add(Field[nextRow, previosColumn]);
            }
            if (nextColumn < Width)
            {
                aroundCells.Add(Field[nextRow, nextColumn]);
            }
        }
        return aroundCells;
    }

    //Изменить значение мин с Х на М
    private void ChangeMinesValue()
    {        
        foreach (var cell in Field)
        {
            if (cell.CellValue == "X")
                cell.CellValue = "M";
        }
    }    
    
    //открыть все клетки
    private void OpenAllCells()
    {
        foreach (var cell in Field)
        {
            if (!cell.IsOpened)
                cell.IsOpened = true;
        }
    }

    //открыть клетку
    public void OpenCell(Cell cell)
    {
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
                    OpenCell(c);
            }        
        }   
    }

    //конец игры
    public void GameEndCheck(Cell cell)
    {
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
            foreach (var c in Field)
            {
                if (!c.IsOpened) unopenCells++;
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

    //итератор
    private IEnumerable<Cell> GetEnumerator()
    {
        for (int row = 0; row < Height; row++)
        {
            for (int column = 0; column < Width; column++)
            {
                yield return Field[row, column];
            }
        }
    }
}
