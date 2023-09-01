﻿using System.ComponentModel.DataAnnotations;

namespace MinesweeperGame.Models; 

public class Minesweeper
{
    //id
    public string Game_id { get; }
    //ширина
    [Range(2, 30, ErrorMessage = "Ширина поля должна быть не менее {1} и не более {2}")]
    public int Width { get; }
    //высота
    [Range(2, 30, ErrorMessage = "Высота поля должна быть не менее {1} и не более {2}")]
    public int Height { get; }
    //количество мин
    public int Mines_count { get; }
    //статус игры
    public bool Completed { get; private set; }
    //игровое поле
    public List<List<Cell>> Field { get; private set; }

    public Minesweeper(int width, int height, int minesCount)
    {
        Game_id = Guid.NewGuid().ToString();
        Width = width;
        Height = height;
        Mines_count = minesCount;
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
        var rand = new Random();
        for (int i = 0; i < Mines_count; )
        {
            //рандомная клетка
            var cell = Field[rand.Next(0, Height)][rand.Next(0, Width)];
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
    public void OpenCell(int row, int col)
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
    public void GameEndCheck(int row, int col)
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
}
