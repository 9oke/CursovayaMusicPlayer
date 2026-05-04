using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MusicPlayer
{
    /// <summary>
    /// Карточка в Spotify-стиле: чёткие небольшие углы, опциональная кайма.
    /// </summary>
    public class CardPanel : Panel
    {
        private int _cornerRadius = 6;
        private Color _surfaceColor = Color.FromArgb(255, 24, 24, 24);
        private Color _borderColor = Color.FromArgb(40, 255, 255, 255);
        private bool _showBorder = false;

        public CardPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw
                   | ControlStyles.UserPaint
                   | ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent;
            DoubleBuffered = true;
        }

        [DefaultValue(6)]
        public int CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = Math.Max(0, value); Invalidate(); }
        }

        [DefaultValue(false)]
        public bool ShowBorder
        {
            get => _showBorder;
            set { _showBorder = value; Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color SurfaceColor
        {
            get => _surfaceColor;
            set { _surfaceColor = value; Invalidate(); }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Color GlassBorderColor
        {
            get => _borderColor;
            set { _borderColor = value; Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var rect = new RectangleF(0.5f, 0.5f, Width - 1f, Height - 1f);
            using (var path = RoundedRect(rect, _cornerRadius))
            {
                using (var brush = new SolidBrush(_surfaceColor))
                {
                    g.FillPath(brush, path);
                }
                if (_showBorder)
                {
                    using (var pen = new Pen(_borderColor, 1f))
                    {
                        g.DrawPath(pen, path);
                    }
                }
            }

            base.OnPaint(e);
        }

        public static GraphicsPath RoundedRect(RectangleF r, int radius)
        {
            return RoundedRect(r.X, r.Y, r.Width, r.Height, radius);
        }

        public static GraphicsPath RoundedRect(float x, float y, float w, float h, int radius)
        {
            var path = new GraphicsPath();
            float d = Math.Min(radius * 2f, Math.Min(w, h));
            if (d <= 0)
            {
                path.AddRectangle(new RectangleF(x, y, w, h));
                return path;
            }
            path.AddArc(x, y, d, d, 180, 90);
            path.AddArc(x + w - d, y, d, d, 270, 90);
            path.AddArc(x + w - d, y + h - d, d, d, 0, 90);
            path.AddArc(x, y + h - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
