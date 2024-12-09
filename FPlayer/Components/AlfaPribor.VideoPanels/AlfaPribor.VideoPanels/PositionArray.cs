using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlfaPribor.VideoPanels
{
    /// <summary>Строка таблицы связи индекса окна и его положения в TableLayoutPanel</summary>
    public class TableRowIndex
    {
        public List<int> Colls { get; set; }
        public TableRowIndex() { Colls = new List<int>(); }
    }

    public class MatrixPosition
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public MatrixPosition()
        {
            Row = -1;
            Col = -1;
        }

        public MatrixPosition(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }

    /// <summary>Связывает индекс окна изображени и её позицию в компоненте TableLayoutPanel</summary>
    public class PositionArray
    {
        public List<TableRowIndex> Rows { get; set; }

        public PositionArray() { Rows = new List<TableRowIndex>(); }

        public void Add(int row, int col, int value)
        {
            while (row > Rows.Count - 1 && row>-1) Rows.Add(new TableRowIndex());

            if (col > Rows[row].Colls.Count - 1) 
                 Rows[row].Colls.Add(value);
            else Rows[row].Colls[col] = value;
        }

        public int GetElement(int row, int column)
        {
            int value = -1;
            try { value = Rows[row].Colls[column]; }
            catch { value = -1; }
            return value;
        }

        public int this[int row, int column]
        {
            get { return GetElement(row, column); }
            set { Add(row, column, value); }
        }

        public List<int> GetRowElements(int row)
        {
            List<int> result = new List<int>();
            try { result = Rows[row].Colls; }
            catch { result = new List<int>(); }
            return result;
        }

        /// <summary>Получить максимальное значения соотношения элемента для строки таблицы</summary>
        /// <param name="rowIndex">Индекс строки</param>
        /// <param name="ratio">Список пропорций</param>
        /// <param name="rotationState">Список флагов поворота</param>
        /// <param name="isCellRotate">Флаг того, что при развороте происходит изменение размеров ячейки а не окна</param>
        /// <returns></returns>
        public float GetMaxRatio(int rowIndex,List<float> ratio, List<bool> rotationState,bool isCellRotate)
        {
            List<int> videoPannels = GetRowElements(rowIndex);
            float resultRatio = ratio[0]; 
            foreach (int videoWindow in videoPannels)
            {
                float rat = ratio[videoWindow];
                if (rotationState[videoWindow] && isCellRotate) rat = 1 / rat;
                if (rat > resultRatio) resultRatio = rat;
            }
            return resultRatio;
        }

        /// <summary>Получить максимальное значения соотношения элемента для строки таблицы</summary>
        /// <param name="rowIndex"></param>
        /// <param name="ratio"></param>
        /// <param name="rotationState"></param>
        /// <returns>Возвращает наибольшее значение соотношения в строке при этом 1/ratio[i] в случае поворота уже учтено</returns>
        public float GetMaxRatio(int rowIndex, List<float> ratio, List<bool> rotationState)
        {
            return GetMaxRatio(rowIndex, ratio, rotationState, true);
        }

        public MatrixPosition GetPositionByValue(int value)
        {
            MatrixPosition position = new MatrixPosition();
            for (int i = 0; i < Rows.Count; i++)
            {
                for (int j = 0; j < Rows[i].Colls.Count; j++)
                {
                    if (Rows[i].Colls[j] == value)
                    {
                        position = new MatrixPosition { Row = i, Col = j };
                        return position;
                    }
                }
            }
            return position;
        }
    }
}
