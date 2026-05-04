using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MusicPlayer
{
    /// <summary>
    /// Тёмная Spotify-подобная форма. Фиксированный размер 1280x800,
    /// без ресайза, с тонким вертикальным градиентом для глубины.
    /// </summary>
    public class PastelForm : Form
    {
        // Тёмная палитра в стиле Spotify
        private static readonly Color BgTop = Color.FromArgb(255, 24, 24, 24);
        private static readonly Color BgBottom = Color.FromArgb(255, 18, 18, 18);

        private Bitmap _bgCache;
        private Size _cachedSize;

        public PastelForm()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw
                   | ControlStyles.UserPaint, true);
            DoubleBuffered = true;

            // Фиксированный размер окна
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = true;
            StartPosition = FormStartPosition.CenterScreen;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            DrawBackground(e.Graphics);
        }

        private void DrawBackground(Graphics gOut)
        {
            if (Width <= 0 || Height <= 0) return;

            if (_bgCache == null || _cachedSize != Size)
            {
                _bgCache?.Dispose();
                _bgCache = new Bitmap(Width, Height);
                _cachedSize = Size;

                using (var g = Graphics.FromImage(_bgCache))
                {
                    // Лёгкий вертикальный градиент сверху вниз
                    using (var brush = new LinearGradientBrush(
                        new Rectangle(0, 0, Width, Height),
                        BgTop, BgBottom, LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(brush, 0, 0, Width, Height);
                    }
                }
            }

            gOut.DrawImageUnscaled(_bgCache, 0, 0);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _bgCache?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
