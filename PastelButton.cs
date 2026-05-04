using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MusicPlayer
{
    /// <summary>
    /// Кнопка в Spotify-стиле. Три режима:
    ///  - Accent: круглая зелёная кнопка с scale-hover и glow (как Play в Spotify)
    ///  - SidebarItem: пункт меню с анимированной полоской слева при выборе
    ///  - Default: плоская иконка с лёгким hover-tint
    /// </summary>
    public class PastelButton : Control, IButtonControl
    {
        // Один таймер на все кнопки сразу
        private static readonly System.Windows.Forms.Timer _animTimer = new System.Windows.Forms.Timer { Interval = 16 };
        private static readonly List<PastelButton> _animated = new List<PastelButton>();
        private static readonly Stopwatch _sw = new Stopwatch();
        private static long _lastMs = 0;

        static PastelButton()
        {
            _animTimer.Tick += (s, e) =>
            {
                long now = _sw.ElapsedMilliseconds;
                float dt = Math.Min(0.1f, (now - _lastMs) / 1000f);
                _lastMs = now;

                var snapshot = _animated.ToArray();
                bool anyActive = false;
                foreach (var btn in snapshot)
                {
                    if (btn.IsDisposed) continue;
                    if (btn.AnimStep(dt)) anyActive = true;
                }
                if (!anyActive)
                {
                    _animTimer.Stop();
                    _sw.Stop();
                }
            };
        }

        private static void EnsureAnimating()
        {
            if (!_animTimer.Enabled)
            {
                _sw.Restart();
                _lastMs = 0;
                _animTimer.Start();
            }
        }

        // ====== Spotify-палитра ======
        // Акцент — фирменный зелёный
        private static readonly Color SpotifyGreen = Color.FromArgb(255, 30, 215, 96);   // #1ED760 (новый)
        private static readonly Color SpotifyGreenDeep = Color.FromArgb(255, 29, 185, 84); // #1DB954 (классика)
        private static readonly Color AccentText = Color.FromArgb(255, 0, 0, 0);

        // Sidebar
        private static readonly Color SidebarActiveBar = Color.FromArgb(255, 30, 215, 96);
        private static readonly Color SidebarActiveBg = Color.FromArgb(40, 255, 255, 255);
        private static readonly Color SidebarHoverBg = Color.FromArgb(20, 255, 255, 255);
        private static readonly Color SidebarText = Color.FromArgb(255, 179, 179, 179);
        private static readonly Color SidebarTextActive = Color.FromArgb(255, 255, 255, 255);

        // Default (плоские иконки управления)
        private static readonly Color DefaultText = Color.FromArgb(255, 179, 179, 179);
        private static readonly Color DefaultTextHover = Color.FromArgb(255, 255, 255, 255);
        private static readonly Color ToggledText = Color.FromArgb(255, 30, 215, 96);

        // Анимация
        private float _hoverProgress = 0f;
        private float _pressProgress = 0f;
        private float _activeProgress = 0f; // для sidebar/toggled
        private bool _hovered = false;
        private bool _pressed = false;

        private int _cornerRadius = 4;
        private bool _accent = false;
        private bool _toggled = false;
        private bool _sidebarItem = false;
        private bool _circular = false;
        private ContentAlignment _textAlign = ContentAlignment.MiddleCenter;
        private DialogResult _dialogResult = DialogResult.None;

        public PastelButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw
                   | ControlStyles.UserPaint
                   | ControlStyles.SupportsTransparentBackColor
                   | ControlStyles.Selectable, true);

            BackColor = Color.Transparent;
            ForeColor = DefaultText;
            Font = new Font("Segoe UI", 10F);
            Cursor = Cursors.Hand;
            DoubleBuffered = true;

            _animated.Add(this);
        }

        [DefaultValue(4)]
        public int CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = Math.Max(0, value); Invalidate(); }
        }

        /// <summary>Главная зелёная кнопка (Play). Круглая, со scale-hover и glow.</summary>
        [DefaultValue(false)]
        public bool Accent
        {
            get => _accent;
            set
            {
                _accent = value;
                if (value) _circular = true;
                ForeColor = value ? AccentText : (_toggled ? ToggledText : DefaultText);
                Invalidate();
            }
        }

        /// <summary>Toggled-состояние для shuffle/repeat — иконка становится зелёной.</summary>
        [DefaultValue(false)]
        public bool Toggled
        {
            get => _toggled;
            set
            {
                _toggled = value;
                if (!_accent && !_sidebarItem)
                    ForeColor = value ? ToggledText : DefaultText;
                Invalidate();
            }
        }

        /// <summary>Пункт sidebar: текст слева, индикатор слева при активации.</summary>
        [DefaultValue(false)]
        public bool SidebarItem
        {
            get => _sidebarItem;
            set
            {
                _sidebarItem = value;
                if (value)
                {
                    _textAlign = ContentAlignment.MiddleLeft;
                    ForeColor = SidebarText;
                }
                Invalidate();
            }
        }

        /// <summary>Активный sidebar-пункт.</summary>
        [DefaultValue(false)]
        public bool Active
        {
            get => _toggled;
            set
            {
                _toggled = value;
                if (_sidebarItem)
                    ForeColor = value ? SidebarTextActive : SidebarText;
                EnsureAnimating();
                Invalidate();
            }
        }

        [DefaultValue(false)]
        public bool Circular
        {
            get => _circular;
            set { _circular = value; Invalidate(); }
        }

        [DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
        public ContentAlignment TextAlign
        {
            get => _textAlign;
            set { _textAlign = value; Invalidate(); }
        }

        // === IButtonControl ===
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DialogResult DialogResult
        {
            get => _dialogResult;
            set => _dialogResult = value;
        }

        public void NotifyDefault(bool value) { /* not used */ }

        public void PerformClick()
        {
            OnClick(EventArgs.Empty);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            if (keyData == Keys.Space || keyData == Keys.Enter) return true;
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
                PerformClick();
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _hovered = true;
            if (!_sidebarItem && !_accent && !_toggled)
                ForeColor = DefaultTextHover;
            EnsureAnimating();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hovered = false;
            _pressed = false;
            if (!_sidebarItem && !_accent && !_toggled)
                ForeColor = DefaultText;
            EnsureAnimating();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                _pressed = true;
                Focus();
                EnsureAnimating();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _pressed = false;
            EnsureAnimating();
        }

        private bool AnimStep(float dt)
        {
            const float speed = 9f;
            float targetHover = _hovered ? 1f : 0f;
            float targetPress = _pressed ? 1f : 0f;
            float targetActive = (_sidebarItem && _toggled) ? 1f : 0f;

            float newHover = Approach(_hoverProgress, targetHover, speed * dt);
            float newPress = Approach(_pressProgress, targetPress, speed * 1.8f * dt);
            float newActive = Approach(_activeProgress, targetActive, speed * 0.9f * dt);

            bool changed = Math.Abs(newHover - _hoverProgress) > 0.001f
                        || Math.Abs(newPress - _pressProgress) > 0.001f
                        || Math.Abs(newActive - _activeProgress) > 0.001f;

            _hoverProgress = newHover;
            _pressProgress = newPress;
            _activeProgress = newActive;

            if (changed && IsHandleCreated && !IsDisposed)
                Invalidate();

            return _hoverProgress != targetHover
                || _pressProgress != targetPress
                || _activeProgress != targetActive;
        }

        private static float Approach(float current, float target, float maxStep)
        {
            float diff = target - current;
            if (Math.Abs(diff) <= maxStep) return target;
            return current + Math.Sign(diff) * maxStep;
        }

        // Easing для более «дорогого» ощущения
        private static float EaseOutCubic(float t) => 1f - (float)Math.Pow(1 - t, 3);

        protected override void OnPaint(PaintEventArgs pevent)
        {
            var g = pevent.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (_accent)
                DrawAccent(g);
            else if (_sidebarItem)
                DrawSidebarItem(g);
            else
                DrawDefault(g);

            DrawText(g);
        }

        private void DrawAccent(Graphics g)
        {
            // Spotify Play: круглая, scale при hover (1.0 -> 1.06), мягкое нажатие, glow
            float hoverEase = EaseOutCubic(_hoverProgress);
            float scale = 1f + hoverEase * 0.06f - _pressProgress * 0.04f;

            float w = (Width - 4) * scale;
            float h = (Height - 4) * scale;
            float ox = (Width - w) / 2f;
            float oy = (Height - h) / 2f;
            var rect = new RectangleF(ox, oy, w, h);

            // Glow за кнопкой
            if (hoverEase > 0.01f)
            {
                int glowAlpha = (int)(hoverEase * 70);
                for (int i = 6; i > 0; i--)
                {
                    var glowRect = RectangleF.Inflate(rect, i * 1.5f, i * 1.5f);
                    using (var glowPath = MakePath(glowRect))
                    using (var glowBrush = new SolidBrush(Color.FromArgb(
                        Math.Max(0, glowAlpha / (i + 1)),
                        SpotifyGreen.R, SpotifyGreen.G, SpotifyGreen.B)))
                    {
                        g.FillPath(glowBrush, glowPath);
                    }
                }
            }

            // Сама кнопка — лёгкий радиальный градиент чтобы не была плоской
            using (var path = MakePath(rect))
            {
                using (var brush = new SolidBrush(SpotifyGreen))
                {
                    g.FillPath(brush, path);
                }

                // Тонкий highlight сверху
                using (var clip = (GraphicsPath)path.Clone())
                {
                    var oldClip = g.Clip;
                    g.SetClip(clip);
                    using (var hl = new LinearGradientBrush(rect,
                        Color.FromArgb(40, 255, 255, 255),
                        Color.FromArgb(0, 255, 255, 255),
                        LinearGradientMode.Vertical))
                    {
                        g.FillRectangle(hl, rect);
                    }
                    g.Clip = oldClip;
                }
            }
        }

        private void DrawSidebarItem(Graphics g)
        {
            float activeEase = EaseOutCubic(_activeProgress);
            float hoverEase = EaseOutCubic(_hoverProgress);
            float bgAlpha = Math.Max(activeEase, hoverEase * 0.6f);

            var rect = new RectangleF(0, 1, Width - 1, Height - 2);

            // Фон
            if (bgAlpha > 0.01f)
            {
                using (var path = CardPanel.RoundedRect(rect, _cornerRadius))
                {
                    int alpha = (int)(activeEase * SidebarActiveBg.A
                                    + (1 - activeEase) * hoverEase * SidebarHoverBg.A);
                    using (var brush = new SolidBrush(Color.FromArgb(
                        Math.Min(255, alpha),
                        SidebarActiveBg.R, SidebarActiveBg.G, SidebarActiveBg.B)))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }

            // Анимированный индикатор слева
            if (activeEase > 0.01f)
            {
                float barH = (Height - 16) * activeEase;
                float barY = (Height - barH) / 2f;
                using (var brush = new SolidBrush(SidebarActiveBar))
                {
                    g.FillRectangle(brush, 0, barY, 3f, barH);
                }
            }
        }

        private void DrawDefault(Graphics g)
        {
            float scale = 1f - _pressProgress * 0.05f;
            float w = (Width - 1) * scale;
            float h = (Height - 1) * scale;
            float ox = (Width - 1 - w) / 2f;
            float oy = (Height - 1 - h) / 2f;
            var rect = new RectangleF(ox, oy, w, h);

            // Hover-tint (без border, плоско)
            if (_hoverProgress > 0.01f)
            {
                using (var path = MakePath(rect))
                {
                    int alpha = (int)(_hoverProgress * 25);
                    using (var brush = new SolidBrush(Color.FromArgb(alpha, 255, 255, 255)))
                    {
                        g.FillPath(brush, path);
                    }
                }
            }
        }

        private GraphicsPath MakePath(RectangleF rect)
        {
            if (_circular)
            {
                var p = new GraphicsPath();
                p.AddEllipse(rect);
                return p;
            }
            return CardPanel.RoundedRect(rect, _cornerRadius);
        }

        private void DrawText(Graphics g)
        {
            if (string.IsNullOrEmpty(Text)) return;

            var textRect = new Rectangle(0, 0, Width, Height);
            TextFormatFlags flags;
            switch (_textAlign)
            {
                case ContentAlignment.MiddleLeft:
                    flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine;
                    textRect = new Rectangle(textRect.X + 16, textRect.Y, textRect.Width - 16, textRect.Height);
                    break;
                case ContentAlignment.MiddleRight:
                    flags = TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine;
                    textRect = new Rectangle(textRect.X, textRect.Y, textRect.Width - 16, textRect.Height);
                    break;
                default:
                    flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine;
                    break;
            }

            TextRenderer.DrawText(g, Text, Font, textRect, ForeColor, flags);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _animated.Remove(this);
            base.Dispose(disposing);
        }
    }
}
