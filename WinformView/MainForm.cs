using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using CivPresenter;
using CivModel;
using CivModel.Common;

namespace WinformView
{
    public partial class MainForm : Form, IView
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int keyCode);

        private Presenter _presenter;

        private float _blockSize = 24;

        private bool _roundEarth = false;
        private int _sightDx = 0;
        private int _sightDy = 0;

        private Point? _selectedPoint;
        private Terrain.Point? _selectedTile;

        public DeploySelector _deploySelector;

        public MainForm()
        {
            InitializeComponent();
            MouseWheel += MainForm_MouseWheel;
        }

        public void Refocus()
        {
            // do nothing
        }

        public void Shutdown()
        {
            Close();
        }

        void IView.Invoke(Action act)
        {
            base.Invoke(act);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ClientSize = new Size(640, 480);

            _presenter = null;
            if (File.Exists("map.txt"))
            {
                if (MessageBox.Show("Save file is found. Do you want to load it?",
                    "Save file is found", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _presenter = new Presenter(this, "map.txt");
                }
            }
            if (_presenter == null)
                _presenter = new Presenter(this);

            RefreshTitle();

            InitPlayerColor();
        }

        private void RefreshTitle()
        {
            Text = "WinformView - Player " + _presenter.Game.PlayerNumberInTurn;
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(Brushes.SkyBlue, ClientRectangle);

            int sx = (int)Math.Ceiling(ClientSize.Width / (_blockSize * (float)Math.Sqrt(3)));
            int sy = (int)Math.Ceiling(ClientSize.Height / (_blockSize * 1.5f));

            int bx = _presenter.FocusedPoint.Position.X - sx / 2;
            int by = _presenter.FocusedPoint.Position.Y - sy / 2;

            for (int dy = -1; dy < sy + 1; ++dy)
            {
                for (int dx = -1; dx < sx + 1; ++dx)
                {
                    int x = bx + dx;
                    int y = by + dy;

                    if (!_roundEarth && (x < 0 || x >= _presenter.Game.Terrain.Width))
                        continue;
                    if (y < 0 || y >= _presenter.Game.Terrain.Height)
                        continue;

                    float ax = _blockSize * (float)Math.Sqrt(3) / 2.0f;
                    float ay = _blockSize / 2.0f;

                    float px = -_sightDx + dx * ax * 2 + (1 - y % 2) * ax;
                    float py = -_sightDy + dy * ay * 3;

                    PointF[] polygon = new PointF[] {
                        new PointF(px + ax, py),
                        new PointF(px + 2 * ax, py + ay),
                        new PointF(px + 2 * ax, py + 3 * ay),
                        new PointF(px + ax, py + 4 * ay),
                        new PointF(px, py + 3 * ay),
                        new PointF(px, py + ay),
                    };

                    var point = _presenter.Game.Terrain.GetPoint(x, y);

                    int[] tbl;
                    unchecked
                    {
                        tbl = new int[] {
                                (int)0xffdeb887,
                                (int)0xff1c6ba0,
                                (int)0xff303030,
                                (int)0xff00ff00,
                                (int)0xff007f00,
                                (int)0xff7f7f7f,
                                (int)0xffe5e5e0,
                                (int)0xffffa500,
                            };
                    }

                    e.Graphics.FillPolygon(new SolidBrush(Color.FromArgb(tbl[(int)point.Type])), polygon);
                    e.Graphics.DrawPolygon(Pens.AntiqueWhite, polygon);

                    if (point.TileOwner != null)
                    {
                        var color = GetPlayerColor(point.TileOwner, 0x3f);
                        var brush = new SolidBrush(color);
                        float cx = px + ax;
                        float cy = py + 2 * ay;
                        float radius = _blockSize * 0.625f;
                        e.Graphics.FillEllipse(brush, cx - radius, cy - radius, radius * 2, radius * 2);
                    }

                    if (point.TileBuilding is CityBase)
                    {
                        var color = GetPlayerColor(point.TileBuilding.Owner);
                        var brush = new SolidBrush(color);
                        var pen = new Pen(Color.White, 2);
                        float cx = px + ax;
                        float cy = py + 2 * ay;
                        float radius = _blockSize * 0.33f;
                        e.Graphics.FillEllipse(brush, cx - radius, cy - radius, radius * 2, radius * 2);
                        e.Graphics.DrawEllipse(pen, cx - radius, cy - radius, radius * 2, radius * 2);
                    }
                    else if (point.TileBuilding != null)
                    {
                        var color = GetPlayerColor(point.TileBuilding.Owner);
                        var brush = new SolidBrush(color);
                        var pen = new Pen(Color.HotPink, 2);
                        float cx = px + ax;
                        float cy = py + 2 * ay;
                        float radius = _blockSize * 0.25f;
                        e.Graphics.FillRectangle(brush, cx - radius, cy - radius, radius * 2, radius * 2);
                        e.Graphics.DrawRectangle(pen, cx - radius, cy - radius, radius * 2, radius * 2);
                    }

                    if (point.Unit != null)
                    {
                        var color = GetPlayerColor(point.Unit.Owner);
                        var brush = new SolidBrush(color);
                        var pen = new Pen(Color.Red, 2);
                        float cx = px + ax;
                        float cy = py + 2 * ay;
                        float radius = _blockSize * 0.20f;
                        var poly = new PointF[] {
                            new PointF(cx - radius, cy),
                            new PointF(cx, cy + radius),
                            new PointF(cx + radius, cy),
                            new PointF(cx, cy - radius)
                        };
                        e.Graphics.FillPolygon(brush, poly);
                        e.Graphics.DrawPolygon(pen, poly);
                    }

                    if (_selectedPoint is Point ptm && IsInPolygon(polygon, new PointF(ptm.X, ptm.Y)))
                    {
                        _selectedPoint = null;
                        _selectedTile = point;
                    }

                    if (_selectedTile.HasValue && _selectedTile.Value == point)
                    {
                        var brush = new SolidBrush(Color.FromArgb(0x1fffffff));
                        var pen = new Pen(new SolidBrush(Color.FromArgb(0x1f0000ff)), 3);

                        float cx = px + ax;
                        float cy = py + 2 * ay;
                        float radius = _blockSize * 0.75f;
                        e.Graphics.FillEllipse(brush, cx - radius, cy - radius, radius * 2, radius * 2);
                        e.Graphics.DrawEllipse(pen, cx - radius, cy - radius, radius * 2, radius * 2);
                    }
                }
            }
        }

        private Color[] playerColors_;
        private void InitPlayerColor()
        {
            playerColors_ = new Color[_presenter.Game.Players.Count];
            for (int idx = 0; idx < playerColors_.Length; ++idx)
            {
                unchecked
                {
                    int p = (idx % 6) + 1;
                    int color = (((p & 4) != 0 ? 0xff : 0) << 16) | (((p & 2) != 0 ? 0xff : 0) << 8) | ((p & 1) != 0 ? 0xff : 0);
                    playerColors_[idx] = Color.FromArgb((0xff << 24) | color);
                }
            }
        }
        private Color GetPlayerColor(Player player, int alpha = 0xff)
        {
            int idx = 0;
            for (; idx < _presenter.Game.Players.Count; ++idx)
                if (_presenter.Game.Players[idx] == player)
                    break;

            var c = playerColors_[idx];
            return Color.FromArgb(alpha, c.R, c.G, c.B);
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
            void foo(int i)
            { 
                if (_selectedTile.HasValue)
                {
                    var pt = _selectedTile.Value;
                    pt.Type = (TerrainType)i;
                }
            }

            switch (e.KeyCode)
            {
                case Keys.Enter:
                    _presenter.CommandApply();
                    RefreshTitle();
                    Invalidate();
                    break;

                case Keys.Escape:
                    _presenter.CommandCancel();
                    Invalidate();
                    break;

                case Keys.Oemcomma:
                    if (_presenter.SaveFile == null)
                        _presenter.SaveFile = "map.txt";
                    _presenter.CommandSave();
                    MessageBox.Show("Saved");
                    break;

                case Keys.F1:
                    if (_selectedTile.HasValue)
                    {
                        new TileInfo(_presenter.Game, _selectedTile.Value).ShowDialog();
                    }
                    break;

                case Keys.F2:
                    if (_deploySelector == null)
                    {
                        _deploySelector = new DeploySelector(
                            _presenter.Game, () => _deploySelector = null);
                        _deploySelector.Show();
                    }
                    break;

                case Keys.F3:
                    if (_selectedTile.HasValue && _deploySelector != null)
                    {
                        _deploySelector.Deploy(_selectedTile.Value);
                    }
                    break;

                case Keys.F11:
                    _roundEarth = !_roundEarth;
                    break;

                case Keys.F12:
                    {
                        string stridx = Microsoft.VisualBasic.Interaction.InputBox("몇번 플레이어?");
                        if (Int32.TryParse(stridx, out int idx) && 0 <= idx && idx < _presenter.Game.Players.Count)
                        {
                            var dialog = new ColorDialog();
                            dialog.Color = playerColors_[idx];
                            if (dialog.ShowDialog() == DialogResult.OK)
                            {
                                playerColors_[idx] = dialog.Color;
                            }
                        }
                        else
                        {
                            MessageBox.Show("invalid player index");
                        }
                    }
                    break;

                case Keys.C:
                    if (_selectedTile.HasValue)
                    {
                        if (_selectedTile.Value.TileBuilding is CityBase)
                        {
                            _selectedTile.Value.TileBuilding.PlacedPoint = null;
                        }
                        else
                        {
                            var city = new CityCenter(_presenter.Game.PlayerInTurn, null, _selectedTile.Value);
                        }
                        Invalidate();
                    }
                    break;

                case Keys.Left:
                    if (_selectedTile.HasValue)
                    {
                        var pos = _selectedTile.Value.Position;
                        pos.X -= 1;
                        if (_presenter.Game.Terrain.IsValidPosition(pos))
                            _selectedTile = _presenter.Game.Terrain.GetPoint(pos);
                    }
                    break;
                case Keys.Right:
                    if (_selectedTile.HasValue)
                    {
                        var pos = _selectedTile.Value.Position;
                        pos.X += 1;
                        if (_presenter.Game.Terrain.IsValidPosition(pos))
                            _selectedTile = _presenter.Game.Terrain.GetPoint(pos);
                    }
                    break;
                case Keys.Up:
                    if (_selectedTile.HasValue)
                    {
                        var pos = _selectedTile.Value.Position;
                        pos.Y -= 1;
                        if (_presenter.Game.Terrain.IsValidPosition(pos))
                            _selectedTile = _presenter.Game.Terrain.GetPoint(pos);
                    }
                    break;
                case Keys.Down:
                    if (_selectedTile.HasValue)
                    {
                        var pos = _selectedTile.Value.Position;
                        pos.Y += 1;
                        if (_presenter.Game.Terrain.IsValidPosition(pos))
                            _selectedTile = _presenter.Game.Terrain.GetPoint(pos);
                    }
                    break;

                case Keys.D1:
                    if (_selectedTile.HasValue)
                    {
                        string kind = Microsoft.VisualBasic.Interaction.InputBox("타일 종류");
                        string[] tbl = { "P", "O", "M", "F", "S", "T", "I", "H" };
                        int idx = Array.IndexOf(tbl, kind);
                        if (idx != -1)
                        {
                            try
                            {
                                string strn = Microsoft.VisualBasic.Interaction.InputBox("몇개");
                                int n = Convert.ToInt32(strn);
                                var pos = _selectedTile.Value.Position;
                                do
                                {
                                    var pt = _presenter.Game.Terrain.GetPoint(pos);
                                    pt.Type = (TerrainType)idx;
                                    pos.X += 1;
                                }
                                while (--n > 0 && _presenter.Game.Terrain.IsValidPosition(pos));
                            }
                            catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                            {
                                MessageBox.Show("invalid input");
                            }
                        }
                        else
                        {
                            MessageBox.Show("invalid tile type");
                        }
                    }
                    break;
                case Keys.D2:
                    if (_selectedTile.HasValue)
                    {
                        string kind = Microsoft.VisualBasic.Interaction.InputBox("타일 종류");
                        string[] tbl = { "P", "O", "M", "F", "S", "T", "I", "H" };
                        int idx = Array.IndexOf(tbl, kind);
                        if (idx != -1)
                        {
                            if (MessageBox.Show("채우기 고고혓?", "ㄱㄱ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                var type = _selectedTile.Value.Type;
                                if (type == (TerrainType)idx)
                                    break;
                                RecursiveFill(type, (TerrainType)idx, _selectedTile.Value);
                            }
                        }
                        else
                        {
                            MessageBox.Show("invalid tile type");
                        }
                    }
                    break;

                case Keys.D3:
                    if (_selectedTile.HasValue)
                    {
                        string stridx = Microsoft.VisualBasic.Interaction.InputBox("몇번 플레이어?");
                        if (Int32.TryParse(stridx, out int idx) && 0 <= idx && idx < _presenter.Game.Players.Count)
                        {
                            try
                            {
                                string strn = Microsoft.VisualBasic.Interaction.InputBox("몇개");
                                int n = Convert.ToInt32(strn);
                                var pos = _selectedTile.Value.Position;
                                do
                                {
                                    var pt = _presenter.Game.Terrain.GetPoint(pos);
                                    pt.TileOwner = _presenter.Game.Players[idx];
                                    pos.X += 1;
                                }
                                while (--n > 0 && _presenter.Game.Terrain.IsValidPosition(pos));
                            }
                            catch (Exception ex) when (ex is FormatException || ex is OverflowException)
                            {
                                MessageBox.Show("invalid input");
                            }
                        }
                        else
                        {
                            MessageBox.Show("invalid player index");
                        }
                    }
                    break;
                case Keys.D4:
                    if (_selectedTile.HasValue)
                    {
                        string stridx = Microsoft.VisualBasic.Interaction.InputBox("몇번 플레이어?");
                        if (Int32.TryParse(stridx, out int idx) && 0 <= idx && idx < _presenter.Game.Players.Count)
                        {
                            if (MessageBox.Show("채우기 고고혓?", "ㄱㄱ?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                var newOwner = _presenter.Game.Players[idx];
                                var origin = _selectedTile.Value.TileOwner;
                                if (newOwner == origin)
                                    break;
                                RecursiveOwn(origin, newOwner, _selectedTile.Value);
                            }
                        }
                        else
                        {
                            MessageBox.Show("invalid player index");
                        }
                    }
                    break;

                case Keys.B:
                    if (_selectedTile.HasValue)
                    {
                        var tile = _selectedTile.Value;
                        try
                        {
                            tile.TileOwner = _presenter.Game.PlayerInTurn;
                        }
                        catch (InvalidOperationException)
                        {
                            // ignore
                        }
                    }
                    break;
                case Keys.N:
                    if (_selectedTile.HasValue)
                    {
                        var tile = _selectedTile.Value;
                        try
                        {
                            tile.TileOwner = null;
                        }
                        catch (InvalidOperationException)
                        {
                            // ignore
                        }
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
        private void RecursiveFill(TerrainType origin, TerrainType newType, Terrain.Point pt)
        {
            if (pt.Type != origin)
                return;
            pt.Type = newType;
            foreach (var sub in pt.Adjacents())
                if (sub.HasValue)
                    RecursiveFill(origin, newType, sub.Value);
        }
        private void RecursiveOwn(Player origin, Player newOwner, Terrain.Point pt)
        {
            if (pt.TileOwner != origin)
                return;
            pt.TileOwner = newOwner;
            foreach (var sub in pt.Adjacents())
                if (sub.HasValue)
                    RecursiveOwn(origin, newOwner, sub.Value);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void SyncSight()
        {
            while (-_sightDx > _blockSize * (float)Math.Sqrt(3) / 2)
            {
                _sightDx += (int)Math.Floor(_blockSize * (float)Math.Sqrt(3));
                _presenter.CommandArrowKey(Direction.Left);
            }
            while (_sightDx > _blockSize * (float)Math.Sqrt(3) / 2)
            {
                _sightDx -= (int)Math.Floor(_blockSize * (float)Math.Sqrt(3));
                _presenter.CommandArrowKey(Direction.Right);
            }
            while (-_sightDy > _blockSize * 0.75f)
            {
                _sightDy += (int)Math.Floor(_blockSize * 1.5f);
                _presenter.CommandArrowKey(Direction.Up);
            }
            while (_sightDy * 2 > _blockSize * 0.75f)
            {
                _sightDy -= (int)Math.Floor(_blockSize * 1.5f);
                _presenter.CommandArrowKey(Direction.Down);
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

                _sightDx += -dx;
                _sightDy += -dy;
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
                _selectedPoint = e.Location;
            }
        }

        private void MainForm_MouseWheel(object sender, MouseEventArgs e)
        {
            _blockSize *= (float)Math.Pow(1.02, e.Delta / 64.0f);
        }
    }
}
