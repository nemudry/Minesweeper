namespace MinesweeperGame.Models;

//ячейка поля
public class Cell
{
    //открыта ли
    public bool IsOpened { get; set; }
    //значение ячейки
    public string CellValue { get; set; }
    //координаты ячейки
    public int CellRow { get; }   
    public int CellColumn { get; }
    public Cell(int row, int column )
    {
        IsOpened = false;
        CellValue = "";
        CellRow = row;
        CellColumn = column;
    }
}