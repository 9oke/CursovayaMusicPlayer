using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MusicPlayer
{
    /// <summary>
    /// Простая кнопка в стиле Spotify без сложных анимаций
    /// </summary>
    public class PastelButton : Control
    {
        private bool _toggled = false;
        private bool _hovered = false;
        private bool _active = false;
        private int _cornerRadius = 4;
        private bool _circular = false;
        private ContentAlignment _textAlign = ContentAlignment.MiddleCenter;

        private static readonly Color SpotifyGreen = Color.FromArgb(30, 215, 96);
        private static readonly Color DefaultText = Color.FromArgb(179, 179, 179);
        private static readonly Color HoverText = Color.FromArgb(255, 255, 255);
        private static readonly Color ToggledText = Color.FromArgb(30, 215, 96);
        private static readonly Color SidebarActiveBg = Color.FromArgb(40, 255, 255, 255);
        private static readonly Color SidebarActiveBar = Color.FromArgb(30, 215, 96);

        private bool _sidebarItem = false;
        private bool _accent = false;

        public PastelButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw
                   | ControlStyles.UserPaint, true);

            BackColor = Color.Transparent;
            ForeColor = DefaultText;
            Font = new Font("Segoe UI", 10F);
            Cursor = Cursors.Hand;
            DoubleBuffered = true;
        }

        [DefaultValue(false)]
        public bool Toggled
        {
            get => _toggled;
            set { _toggled = value; Invalidate(); }
        }

        [DefaultValue(false)]
        public bool Active
        {
            get => _active;
            set { _active = value; Invalidate(); }
        }

        [DefaultValue(false)]
        public bool SidebarItem
        {
            get => _sidebarItem;
            set { _sidebarItem = value; Invalidate(); }
        }

        [DefaultValue(false)]
        public bool Accent
        {
            get => _accent;
            set { _accent = value; Invalidate(); }
        }

        [DefaultValue(false)]
        public bool Circular
        {
            get => _circular;
            set { _circular = value; Invalidate(); }
        }

        [DefaultValue(4)]
        public int CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = Math.Max(0, value); Invalidate(); }
        }

        [DefaultValue(ContentAlignment.MiddleCenter)]
        public ContentAlignment TextAlign
        {
            get => _textAlign;
            set { _textAlign = value; Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            if (_accent || _circular)
            {
                // Зелёная круглая кнопка Play
                using (var brush = new SolidBrush(SpotifyGreen))
                    e.Graphics.FillEllipse(brush, 0, 0, Width, Height);

                DrawCenteredText(e.Graphics, Text, ForeColor);
            }
            else if (_sidebarItem)
            {
                // Пункт меню
                if (_active)
                {
                    using (var brush = new SolidBrush(SidebarActiveBg))
                        e.Graphics.FillRectangle(brush, 0, 0, Width, Height);

                    // Зелёная полоска слева
                    using (var brush = new SolidBrush(SidebarActiveBar))
                        e.Graphics.FillRectangle(brush, 0, 0, 3, Height);
                }

                DrawCenteredText(e.Graphics, Text, _active ? HoverText : DefaultText);
            }
            else
            {
                // Обычная иконка
                Color textColor = _toggled ? ToggledText : (_hovered ? HoverText : DefaultText);
                DrawCenteredText(e.Graphics, Text, textColor);
            }
        }

        private void DrawCenteredText(Graphics g, string text, Color color)
        {
            if (string.IsNullOrEmpty(text)) return;
            using (var brush = new SolidBrush(color))
            {
                var fmt = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                g.DrawString(text, Font, brush, new RectangleF(0, 0, Width, Height), fmt);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _hovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _hovered = false;
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            Invalidate();
        }
    }
}
