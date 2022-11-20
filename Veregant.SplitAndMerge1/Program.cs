using System;
using System.Collections.Generic;


namespace Veregant.SplitAndMerge1
{

    internal class Program
    {

        const char START_ARG = '(';
        const char END_ARG = ')';
        const char END_LINE = '\0';

        // Creating a new list of cells
        static List<Cell> Cells = new List<Cell>(); 

        static void Main(string[] args)
        {

            string stri = "(2+3)^(1+2 )-(100+(11-(10))) +cos(90+ 90)";
            LoadString(stri, ref Cells);
            CalculateString(ref Cells);
            Console.WriteLine(CellsToString(Cells));
            Console.ReadKey();

        }

        static void LoadString(string data, ref List<Cell> Cells) 
        {
            // Iterating given string
            for (int from = 0; from < data.Length; from++) 
            {
                if (data[from].ToString() ==  " ") from++;

                Cell cell = new Cell();

                // Checking if substring is an integer
                if (int.TryParse(data[from].ToString(), out int Current)) 
                {
                    cell.Value = Current;

                    if (from < data.Length - 1)
                    {
                        from++;
                    }

                    // Getting the whole number
                    while ( (from < data.Length) && (int.TryParse(data[from].ToString(), out int result)) ) 
                    {
                        Current *= 10;
                        Current += result;
                        cell.Value = Current;
                        if (from < data.Length)
                        {
                            from++;
                        }

                    }
                }
                if (from < data.Length)
                {
                    switch (data[from])
                    {
                        case '+':
                        case '-':
                        case '*':
                        case '/':
                        case '^':
                            cell.Action = data[from].ToString();
                            break;
                        case START_ARG:
                            cell.Action = data[from].ToString();
                            break;
                        case END_ARG:
                            cell.Action = data[from].ToString();
                            break;
                        case 'c':
                            cell.Action = "cos";
                            from += 2;
                            break;
                        case 's':
                            cell.Action = "sin";
                            from += 2;
                            break;
                        default:
                            Exception NotValidStringException = new Exception("Inputed string is not valid.");
                            throw NotValidStringException;

                    }
                }
                Cells.Add(cell);
            }
        }

        static void DeleteEmptyCell(ref List<Cell> cells)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                if (!cells[i])
                {
                    cells.RemoveAt(i);
                }
            }
        }

        static void CalculateTrigonometricFunctions(ref List<Cell> cells)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                try
                {
                    switch (cells[i].Action)
                    {
                        case "sin":
                            cells[i].Value = Math.Sin(cells[i + 1].Value / 180 * Math.PI);
                            cells[i].Action = cells[i + 1].Action;
                            cells[i + 1].Action = Cell.NO_ACTION;
                            break;
                        case "cos":
                            cells[i].Value = Math.Cos(cells[i + 1].Value / 180 * Math.PI);
                            cells[i].Action = cells[i + 1].Action;
                            cells[i + 1].Action = Cell.NO_ACTION;
                            cells[i + 1].Value = Cell.NO_VALUE;
                            break;
                    }
                }
                catch
                {
                    throw new OverflowException("Too big argument for trigonometric function.");
                }
            }

            DeleteEmptyCell(ref cells);
        }

        static void CheckPriority(ref List<Cell> Cells)
        {
            foreach (Cell cell in Cells)
            {
                switch (cell.Action)
                {
                    case "NULL":
                        cell.Priority = Cell.NO_PRIORITY;
                        break;
                    case "+":
                    case "-":
                        cell.Priority = 1;
                        break;
                    case "*":
                    case "/":
                        cell.Priority = 2;
                        break;
                    case "^":
                        cell.Priority = 3;
                        break;
                    case "sin":
                    case "cos":
                        cell.Priority = 4;
                        break;
                    case "(":
                    case ")":
                        cell.Priority = 0;
                        break;
                    default:
                        Exception WrongActionExeption = new Exception("Wrong action");
                        throw WrongActionExeption;

                }
            }
        }

        public static void DivideListByBrackets (ref List<Cell> cells)
        {
            int start = -1, end = -1;

            for (int i = 0; i < cells.Count; i++)
            {
                if (cells[i].Action == "(")
                {
                    start = i;
                }
                if (cells[i].Action == ")" && end < 0)
                {
                    end = i;
                }
                if (start >= 0 && end >= 0) break;
            }
            if (start >= 0 && end >= 0)
            {
                Cell cell = end < cells.Count - 1 ? new Cell(cells[end + 1].Action) : new Cell();

                List<Cell> NewCells = new List<Cell>();

                cells[end].Action = Cell.NO_ACTION;

                for (int i = (start + 1); i <= end; i++)
                {
                    NewCells.Add(cells[i]);
                }

                cells.RemoveRange(start, (end - start + 1) );
                if (end < cells.Count - 1) cells.RemoveAt(start);

                cell.Value = CalculateString(ref NewCells);

                cells.Insert(start, cell);

                DivideListByBrackets(ref cells);
            }
            else return;

        }

        static double CalculateString(ref List<Cell> cells)
        {
            DivideListByBrackets(ref cells);

            CalculateTrigonometricFunctions(ref cells);

            CheckPriority(ref cells);

            while (true)
            {
                if (cells.Count == 1) break;
                else MergeCells(ref cells);
            }
            return cells[0].Value;
        }

        static Cell DoTheAction (Cell first, Cell second)
        {
            switch (first.Action)
            {
                case "+":
                    first.Value += second.Value;
                    break;
                case "-":
                    first.Value -= second.Value;
                    break;
                case "*":
                    first.Value *= second.Value;
                    break;
                case "/":
                    first.Value /= second.Value;
                    break;
                case "^":
                    first.Value = Math.Pow(first.Value, second.Value);
                    break;
            }

            first.Action = second.Action;
            first.Priority = second.Priority;
            return first;
        }

        static void MergeCells (ref List<Cell> cells)
        {
            int i = 0;
            while (cells.Count > 1)
            {
                if ((i < cells.Count - 1) && cells[i].Priority >= cells[i + 1].Priority)
                {
                    cells[i] = DoTheAction(cells[i], cells[i + 1]);
                    cells.RemoveAt(i+1);
                    i = 0;
                    continue;
                }
                if ((i < cells.Count - 1) && cells[i+1].Action == Cell.NO_ACTION)
                {
                    cells[i] = DoTheAction(cells[i], cells[i + 1]);
                    cells.RemoveAt(i + 1);
                }
                i++;
                if (i >= cells.Count) i = 0;
            }
        }




        static string CellsToString (List<Cell> cells)
        {
            string str = "";
            for (int i = 0; i < cells.Count; i++)
            {
                str += cells[i].ToString();
            }

            return str;
        }
       
    }
}
