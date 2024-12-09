using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace myControls
{
    public partial class SmoothProgressBar : UserControl
    {

        /// <summary>Конструктор</summary>
        public SmoothProgressBar()
        {
            InitializeComponent();
        }

        int min = 0;	// Minimum value for progress range
        int max = 100;	// Maximum value for progress range
        int val = 0;		// Current progress
        bool mark;
        int mark_start; //Стартовый маркер
        int mark_stop;  //Стоповый маркер
        Color BarColor1 = Color.Blue;		// Color of progress meter
        Color BarColor2 = Color.Blue;		// Color of progress meter

        protected override void OnResize(EventArgs e)
        {
            this.Invalidate();
        }

        /// <summary>Перерисовка контрола</summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            Graphics g = e.Graphics;
            float percent = (float)(val - min) / (float)(max - min);
            Rectangle rect = this.ClientRectangle;
            Color color1;
            Color color2;
            if (this.Enabled)
            {
                color1 = BarColor1;
                color2 = BarColor2;
            }
            else
            {
                color1 = Color.LightGray;
                color2 = Color.LightGray;
            }
            if (this.Enabled)
            {
                //Кисть текущей позиции
                LinearGradientBrush brush = new LinearGradientBrush(new Point(0, 0), new Point(0, rect.Height), color1, color2);
                //Расчет площади полосы прогресса
                rect.Width = (int)((float)rect.Width * percent);
                //Рисование полосы прогресса
                g.FillRectangle(brush, rect);
                //Очистка
                brush.Dispose();
            }
            //Кисть выделенной области
            if (mark)
            {
                Brush br = new SolidBrush(Color.FromArgb(100, Color.Red));//color1));
                float percent_start = (float)(mark_start - min) / (float)(max - min);
                float percent_stop = (float)(mark_stop - min) / (float)(max - min);
                int start = (int)((float)this.ClientRectangle.Width * percent_start);
                int stop = (int)((float)this.ClientRectangle.Width * percent_stop);
                Rectangle rect_mark = new Rectangle(start, this.ClientRectangle.X, (stop - start),this.ClientRectangle.Height);
                g.FillRectangle(br, rect_mark);
            }

            //Рисование границ 3Д
            Draw3DBorder(g);
            g.Dispose();
        }

        public int Minimum
        {
            get
            {
                return min;
            }

            set
            {
                // Prevent a negative value.
                if (value < 0)
                {
                    min = 0;
                }

                // Make sure that the minimum value is never set higher than the maximum value.
                if (value > max)
                {
                    min = value;
                    min = value;
                }

                // Ensure value is still in range
                if (val < min)
                {
                    val = min;
                }

                // Invalidate the control to get a repaint.
                this.Invalidate();
            }
        }

        public int Maximum
        {
            get
            {
                return max;
            }

            set
            {
                // Make sure that the maximum value is never set lower than the minimum value.
                if (value < min)
                {
                    min = value;
                }

                max = value;

                // Make sure that value is still in range.
                if (val > max)
                {
                    val = max;
                }

                //Value = val;

                // Invalidate the control to get a repaint.
                this.Invalidate();
            }
        }

        public int Value
        {
            get
            {
                return val;
            }

            set
            {
                int oldValue = val;

                //Проверка граничных значений
                if (value < min)
                {
                    val = min;
                }
                else if (value > max)
                {
                    val = max;
                }
                else
                {
                    val = value;
                }

                // Invalidate only the changed area.
                float percent;

                Rectangle newValueRect = this.ClientRectangle;
                Rectangle oldValueRect = this.ClientRectangle;

                // Use a new value to calculate the rectangle for progress.
                percent = (float)(val - min) / (float)(max - min);
                newValueRect.Width = (int)((float)newValueRect.Width * percent);

                // Use an old value to calculate the rectangle for progress.
                percent = (float)(oldValue - min) / (float)(max - min);
                oldValueRect.Width = (int)((float)oldValueRect.Width * percent);

                Rectangle updateRect = new Rectangle();

                // Find only the part of the screen that must be updated.
                if (newValueRect.Width > oldValueRect.Width)
                {
                    updateRect.X = oldValueRect.Size.Width;
                    updateRect.Width = newValueRect.Width - oldValueRect.Width;
                }
                else
                {
                    updateRect.X = newValueRect.Size.Width;
                    updateRect.Width = oldValueRect.Width - newValueRect.Width;
                }

                updateRect.Height = this.Height;

                // Invalidate the intersection region only.
                this.Invalidate(updateRect);
            }
        }

        public bool Mark
        {
            get { return mark; }
            set 
            { 
                mark = value;
                this.Invalidate(this.ClientRectangle);
            }
        }

        /// <summary>Стартовая метка выделения</summary>
        public int MarkStart
        {
            get { return mark_start; }
            set
            {
                //Проверка граничных значений
                if (value < min) mark_start = min;
                else 
                    if (value > max) mark_start = max;
                    else mark_start = value;
                this.Invalidate(this.ClientRectangle);
            }
        }

        /// <summary>СтаКонечная метка выделения</summary>
        public int MarkStop
        {
            get { return mark_stop; }
            set
            {
                //Проверка граничных значений
                if (value < min) mark_stop = min;
                else
                    if (value > max) mark_stop = max;
                    else mark_stop = value;
                this.Invalidate(this.ClientRectangle);
            }
        }

        public Color ProgressBarColor1
        {
            get
            {
                return BarColor1;
            }

            set
            {
                BarColor1 = value;
                // Invalidate the control to get a repaint.
                this.Invalidate();
            }
        }

        public Color ProgressBarColor2
        {
            get
            {
                return BarColor2;
            }

            set
            {
                BarColor2 = value;

                // Invalidate the control to get a repaint.
                this.Invalidate();
            }
        }

        void Draw3DBorder(Graphics g)
        {
            int PenWidth = (int)Pens.White.Width;

            g.DrawLine(Pens.DarkGray,
                new Point(this.ClientRectangle.Left, this.ClientRectangle.Top),
                new Point(this.ClientRectangle.Width - PenWidth, this.ClientRectangle.Top));
            g.DrawLine(Pens.DarkGray,
                new Point(this.ClientRectangle.Left, this.ClientRectangle.Top),
                new Point(this.ClientRectangle.Left, this.ClientRectangle.Height - PenWidth));
            g.DrawLine(Pens.White,
                new Point(this.ClientRectangle.Left, this.ClientRectangle.Height - PenWidth),
                new Point(this.ClientRectangle.Width - PenWidth, this.ClientRectangle.Height - PenWidth));
            g.DrawLine(Pens.White,
                new Point(this.ClientRectangle.Width - PenWidth, this.ClientRectangle.Top),
                new Point(this.ClientRectangle.Width - PenWidth, this.ClientRectangle.Height - PenWidth));
        } 

    }
}