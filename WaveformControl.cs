using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MusicPlayer
{
    /// <summary>
    /// Простой прогресс-бар в стиле Spotify/Яндекс.Музыки
    /// </summary>
    public class WaveformControl : Control
    {
        private int _maximum = 100;
        private int _value = 0;
        private bool _isDragging = false;

        private static readonly Color PlayedColor = Color.FromArgb(30, 215, 96);      // Зелёный (Spotify)
        private static readonly Color UnplayedColor = Color.FromArgb(60, 60, 60);    // Серый

        public event EventHandler<int> Seek;

        public WaveformControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw
                   | ControlStyles.UserPaint, true);
            BackColor = Color.Transparent;
            Height = 6;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Maximum
        {
            get => _maximum;
            set
            {
                _maximum = Math.Max(1, value);
                if (_value > _maximum) _value = _maximum;
                Invalidate();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int Value
        {
            get => _value;
            set
            {
                int v = Math.Max(0, Math.Min(_maximum, value));
                if (v != _value)
                {
                    _value = v;
                    Invalidate();
                }
            }
        }

        public void LoadFromAudioFileAsync(string filePath, int barCount)
        {
            // Для совместимости - просто перерисовываем
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Width <= 0 || Height <= 0) return;

            float progress = (float)_value / _maximum;
            float playedWidth = Width * progress;

            // Фон (непроигранная часть)
            using (var brush = new SolidBrush(UnplayedColor))
                e.Graphics.FillRectangle(brush, 0, 0, Width, Height);

            // Проигранная часть
            if (playedWidth > 0)
            {
                using (var brush = new SolidBrush(PlayedColor))
                    e.Graphics.FillRectangle(brush, 0, 0, playedWidth, Height);
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _isDragging = true;
            UpdateValue(e.X);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_isDragging)
                UpdateValue(e.X);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isDragging = false;
        }

        private void UpdateValue(int x)
        {
            if (Width > 0)
            {
                int newValue = (int)((float)x / Width * _maximum);
                newValue = Math.Max(0, Math.Min(_maximum, newValue));
                Value = newValue;
                Seek?.Invoke(this, newValue);
            }
        }
    }
}
