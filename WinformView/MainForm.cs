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

        private int sightDx_ = 0;
        private int sightDy_ = 0;

        private Point? selectedPoint_;
        private Terrain.Point? selectedTile_;

        public MainForm()
        {
            InitializeComponent();
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
            int sy = (int)Math.Ceiling(ClientSize.Height / (blockSize_ * 1.5f));

            int bx = presenter_.FocusedPoint.Position.X - sx / 2;
            int by = presenter_.FocusedPoint.Position.Y - sy / 2;

            for (int dy = -1; dy < sy + 1; ++dy)
            {
                for (int dx = -1; dx < sx + 1; ++dx)
                {
                    int x = bx + dx;
                    int y = by + dy;

                    if (x < 0 || x >= presenter_.Game.Terrain.Width)
                        continue;
                    if (y < 0 || y >= presenter_.Game.Terrain.Height)
                        continue;

                    float ax = blockSize_ * (float)Math.Sqrt(3) / 2.0f;
                    float ay = blockSize_ / 2.0f;

                    float px = -sightDx_ + dx * ax * 2 + (1 - y % 2) * ax;
                    float py = -sightDy_ + dy * ay * 3;

                    PointF[] polygon = new PointF[] {
                        new PointF(px + ax, py),
                        new PointF(px + 2 * ax, py + ay),
                        new PointF(px + 2 * ax, py + 3 * ay),
                        new PointF(px + ax, py + 4 * ay),
                        new PointF(px, py + 3 * ay),
                        new PointF(px, py + ay),
                    };

                    var point = presenter_.Game.Terrain.GetPoint(x, y);

                    int[] tbl;
                    unchecked
                    {
                        tbl = new int[] {
                                (int)0xffdeb887,
                                (int)0xff0000ff,
                                (int)0xff000000,
                                (int)0xff00ff00,
                                (int)0xff007f00,
                                (int)0xff7f7f7f,
                                (int)0xffffffff,
                                (int)0xffffa500,
                            };
                    }

                    e.Graphics.FillPolygon(new SolidBrush(Color.FromArgb(tbl[(int)point.Type])), polygon);
                    e.Graphics.DrawPolygon(Pens.AntiqueWhite, polygon);

                    if (selectedPoint_ is Point ptm && IsInPolygon(polygon, new PointF(ptm.X, ptm.Y)))
                    {
                        selectedPoint_ = null;
                        selectedTile_ = point;
                    }

                    if (selectedTile_.HasValue && selectedTile_.Value == point)
                    {
                        var brush = new SolidBrush(Color.FromArgb(0x1fffffff));
                        var pen = new Pen(new SolidBrush(Color.FromArgb(0x1f0000ff)), 3);

                        float cx = px + ax;
                        float cy = py + 2 * ay;
                        float radius = blockSize_ * 0.75f;
                        e.Graphics.FillEllipse(brush, cx - radius, cy - radius, radius * 2, radius * 2);
                        e.Graphics.DrawEllipse(pen, cx - radius, cy - radius, radius * 2, radius * 2);
                    }
                }
            }
        }

        private static bool IsInPolygon(PointF[] poly, PointF point)
        {
            var coef = poly.Skip(1).Select((p, i) =>
                                            (point.Y - poly[i].Y) * (p.X - poly[i].X)
                                          - (point.X - poly[i].X) * (p.Y - poly[i].Y))
                                    .ToList();

            if (coef.Any(p => p == 0))
                return true;

            for (int i = 1; i < coef.Count(); i++)
            {
                if (coef[i] * coef[i - 1] < 0)
                    return false;
            }
            return true;
        }


        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            Action<int> foo = i => {
                if (selectedTile_.HasValue)
                {
                    var pt = selectedTile_.Value;
                    pt.Type = (TerrainType)i;
                }
            };

            switch (e.KeyCode)
            {
                case Keys.Enter:
                    presenter_.CommandApply();
                    Invalidate();
                    break;

                case Keys.Escape:
                    presenter_.CommandCancel();
                    Invalidate();
                    break;

                case Keys.Oemcomma:
                    presenter_.Game.Terrain.Save();
                    MessageBox.Show("Saved");
                    break;

                case Keys.Left:
                    if (selectedTile_.HasValue)
                    {
                        var pos = selectedTile_.Value.Position;
                        pos.X -= 1;
                        if (presenter_.Game.Terrain.IsValidPosition(pos))
                            selectedTile_ = presenter_.Game.Terrain.GetPoint(pos);
                    }
                    break;
                case Keys.Right:
                    if (selectedTile_.HasValue)
                    {
                        var pos = selectedTile_.Value.Position;
                        pos.X += 1;
                        if (presenter_.Game.Terrain.IsValidPosition(pos))
                            selectedTile_ = presenter_.Game.Terrain.GetPoint(pos);
                    }
                    break;
                case Keys.Up:
                    if (selectedTile_.HasValue)
                    {
                        var pos = selectedTile_.Value.Position;
                        pos.Y -= 1;
                        if (presenter_.Game.Terrain.IsValidPosition(pos))
                            selectedTile_ = presenter_.Game.Terrain.GetPoint(pos);
                    }
                    break;
                case Keys.Down:
                    if (selectedTile_.HasValue)
                    {
                        var pos = selectedTile_.Value.Position;
                        pos.Y += 1;
                        if (presenter_.Game.Terrain.IsValidPosition(pos))
                            selectedTile_ = presenter_.Game.Terrain.GetPoint(pos);
                    }
                    break;

                case Keys.P:
                    foo(0);
                    break;
                case Keys.O:
                    foo(1);
                    break;
                case Keys.M:
                    foo(2);
                    break;
                case Keys.F:
                    foo(3);
                    break;
                case Keys.S:
                    foo(4);
                    break;
                case Keys.T:
                    foo(5);
                    break;
                case Keys.I:
                    foo(6);
                    break;
                case Keys.H:
                    foo(7);
                    break;
            }

            Invalidate();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void SyncSight()
        {
            while (-sightDx_ > blockSize_ * (float)Math.Sqrt(3) / 2)
            {
                sightDx_ += (int)Math.Floor(blockSize_ * (float)Math.Sqrt(3));
                presenter_.CommandArrowKey(Direction.Left);
            }
            while (sightDx_ > blockSize_ * (float)Math.Sqrt(3) / 2)
            {
                sightDx_ -= (int)Math.Floor(blockSize_ * (float)Math.Sqrt(3));
                presenter_.CommandArrowKey(Direction.Right);
            }
            while (-sightDy_ > blockSize_ * 0.75f)
            {
                sightDy_ += (int)Math.Floor(blockSize_ * 1.5f);
                presenter_.CommandArrowKey(Direction.Up);
            }
            while (sightDy_ * 2 > blockSize_ * 0.75f)
            {
                sightDy_ -= (int)Math.Floor(blockSize_ * 1.5f);
                presenter_.CommandArrowKey(Direction.Down);
            }

            Invalidate();
        }

        private Point? prevMouse_;
        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Capture = true;
                prevMouse_ = e.Location;
            }
        }

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (prevMouse_.HasValue)
            {
                var dx = e.Location.X - prevMouse_.Value.X;
                var dy = e.Location.Y - prevMouse_.Value.Y;

                sightDx_ += -dx;
                sightDy_ += -dy;
                prevMouse_ = e.Location;

                SyncSight();
            }
        }

        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && prevMouse_.HasValue)
            {
                Capture = false;
                prevMouse_ = null;
            }
            else if (e.Button == MouseButtons.Right)
            {
                selectedPoint_ = e.Location;
            }
        }
    }
}
