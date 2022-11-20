using System;

namespace Veregant.SplitAndMerge1
{
    public class Cell
    {


        public static string NO_ACTION = "NULL";
        public static int NO_VALUE = Int32.MaxValue; 
        public static int NO_PRIORITY = Int32.MinValue;


        public double Value { get; set; }
        public string Action { get; set; }
        public int Priority { get; set; }

        public Cell(double value, string action)
        {
            this.Value = value;
            this.Action = action;
        }

        public Cell()
        {
            this.Value = NO_VALUE;
            this.Action = NO_ACTION;
            this.Priority = NO_PRIORITY;
        }

        public Cell(double value)
        {
            this.Value = value;
            this.Action = NO_ACTION;
            this.Priority = NO_PRIORITY;
        }

        public Cell(string action)
        {
            this.Action = action;
            this.Value = NO_VALUE;
            this.Priority = NO_PRIORITY;
        }
        public static bool operator ! (Cell cell)
        {
            return (cell.Action == NO_ACTION && cell.Value == NO_VALUE) ? true : false;
        }
        public override string ToString()
        {
            string str = "";
            if (this.Value != NO_VALUE) str += this.Value.ToString();
            if (this.Action != NO_ACTION) str += this.Action;
            return str;
        }
    }
}
