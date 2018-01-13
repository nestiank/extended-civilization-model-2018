using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using CivPresenter;
using CivModel;

namespace WinformView
{
    public partial class MainForm : Form, IView
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int keyCode);

        private Presenter presenter_;

        private int blockSize_ = 24;

        private int sightx_ = 0;
        private int sighty_ = 0;

        public MainForm()
        {
            InitializeComponent();
        }

        public void MoveSight(int dx, int dy)
        {
            sightx_ += dx;
            sighty_ += dy;
            Invalidate();
        }

        public void Refocus()
        {
            var unit = presenter_.FocusedActor;
            if (unit != null && unit.PlacedPoint.HasValue)
            {
                var pos = unit.PlacedPoint.Value.Position;
                sightx_ = pos.X;
                sighty_ = pos.Y;
                Invalidate();
            }
        }

        public void Shutdown()
        {
            Close();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ClientSize = new Size(640, 480);

            presenter_ = new Presenter(this);
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.SkyBlue, ClientRectangle);

            int sx = (int)Math.Ceiling(ClientSize.Width / (blockSize_ * (float)Math.Sqrt(3)));
            int sy = (int)Math.Ceiling(ClientSize.Height / (blockSize_ * 2.0f));

            int bx = sightx_ - sx / 2 - 1;
            int by = sighty_ - sy / 2 - 1;

            for (int dy = 0; dy < sy; ++dy)
            {
                for (int dx = 0; dx < sx; ++dx)
                {
                    int x = bx + dx;
                    int y = by + dy;

                    if (x < 0 || x >= presenter_.Game.Terrain.Width)
                        continue;
                    if (y < 0 || y >= presenter_.Game.Terrain.Height)
                        continue;

                    float ax = blockSize_ * (float)Math.Sqrt(3) / 2.0f;
                    float ay = blockSize_ / 2.0f;

                    float px = dx * ax * 2 + (1 - y % 2) * ax;
                    float py = dy * ay * 3;

                    PointF[] polygon = new PointF[] {
                        new PointF(px + ax, py),
                        new PointF(px + 2 * ax, py + ay),
                        new PointF(px + 2 * ax, py + 3 * ay),
                        new PointF(px + ax, py + 4 * ay),
                        new PointF(px, py + 3 * ay),
                        new PointF(px, py + ay),
                    };

                    var point = presenter_.Game.Terrain.GetPoint(x, y);

                    if (point.Unit != null)
                        e.Graphics.FillPolygon(Brushes.Blue, polygon);
                    else
                        e.Graphics.FillPolygon(Brushes.LightGreen, polygon);

                    e.Graphics.DrawPolygon(Pens.White, polygon);
                }
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    presenter_.CommandApply();
                    break;

                case Keys.F:
                    presenter_.CommandRefocus();
                    break;

                case Keys.Escape:
                    presenter_.CommandCancel();
                    break;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (GetAsyncKeyState((int)Keys.Left) != 0)
                presenter_.CommandArrowKey(Direction.Left);
            if (GetAsyncKeyState((int)Keys.Right) != 0)
                presenter_.CommandArrowKey(Direction.Right);
            if (GetAsyncKeyState((int)Keys.Up) != 0)
                presenter_.CommandArrowKey(Direction.Up);
            if (GetAsyncKeyState((int)Keys.Down) != 0)
                presenter_.CommandArrowKey(Direction.Down);
        }
    }
}
