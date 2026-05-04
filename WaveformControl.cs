using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NAudio.Wave;
using System.ComponentModel;

namespace MusicPlayer
{
    /// <summary>
    /// Spotify/SoundCloud-стиль waveform:
    /// — Чёткие столбики, толще
    /// — Воспроизведённая часть — Spotify-зелёный
    /// — Оставшаяся — приглушённый серый
    /// — При загрузке трека столбики «пробегают» волной слева направо
    /// </summary>
    public class WaveformControl : Control
    {
        // Spotify-палитра
        private static readonly Color PlayedColor = Color.FromArgb(255, 30, 215, 96);
        private static readonly Color PlayedColorDeep = Color.FromArgb(255, 29, 185, 84);
        private static readonly Color UnplayedColor = Color.FromArgb(255, 90, 90, 90);
        private static readonly Color UnplayedColorHover = Color.FromArgb(255, 130, 130, 130);
        private static readonly Color HoverPreview = Color.FromArgb(255, 200, 200, 200);
        private static readonly Color PlayheadColor = Color.FromArgb(255, 255, 255, 255);

        private float[] _bars;
        private int _maximum = 100;
        private int _value = 0;
        private bool _isDragging = false;
        private int _hoverX = -1;
        private CancellationTokenSource _analysisCts;

        // Анимация появления (волна слева направо)
        private System.Windows.Forms.Timer _appearTimer;
        private float _appearProgress = 0f; // 0..1, насколько волна «пробежала»
        private DateTime _appearStart;

        public event EventHandler<int> Seek;

        public WaveformControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw
                   | ControlStyles.UserPaint
                   | ControlStyles.SupportsTransparentBackColor, true);

            BackColor = Color.Transparent;
            Height = 80;
            GenerateRandomBars(160);
            _appearProgress = 1f; // изначально без анимации

            _appearTimer = new System.Windows.Forms.Timer { Interval = 16 };
            _appearTimer.Tick += AppearTimer_Tick;
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

        public void GenerateRandomBars(int count, int seed = 0)
        {
            var rnd = new Random(seed == 0 ? Environment.TickCount : seed);
            _bars = new float[count];
            double freq1 = 0.05 + rnd.NextDouble() * 0.08;
            double freq2 = 0.15 + rnd.NextDouble() * 0.20;
            double phase = rnd.NextDouble() * Math.PI * 2;

            for (int i = 0; i < count; i++)
            {
                double envelope = 0.4 + 0.5 * Math.Sin(i * freq1 + phase) + 0.2 * Math.Sin(i * freq2);
                double noise = rnd.NextDouble() * 0.45;
                double v = Math.Abs(envelope) * 0.6 + noise;
                if (v < 0.1) v = 0.1;
                if (v > 1.0) v = 1.0;
                _bars[i] = (float)v;
            }
            Invalidate();
        }

        public void SetWaveData(float[] bars)
        {
            if (bars == null || bars.Length == 0) return;
            _bars = bars;
            StartAppearAnimation();
        }

        /// <summary>Запускает «волновую» анимацию появления столбиков.</summary>
        public void StartAppearAnimation()
        {
            _appearProgress = 0f;
            _appearStart = DateTime.UtcNow;
            _appearTimer.Start();
            Invalidate();
        }

        private void AppearTimer_Tick(object sender, EventArgs e)
        {
            const float durationSec = 0.7f;
            float elapsed = (float)(DateTime.UtcNow - _appearStart).TotalSeconds;
            _appearProgress = Math.Min(1f, elapsed / durationSec);
            if (_appearProgress >= 1f)
                _appearTimer.Stop();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            if (_bars == null || _bars.Length == 0 || Width <= 0 || Height <= 0)
                return;

            int barCount = _bars.Length;
            float gap = 2f;
            float totalGap = gap * (barCount - 1);
            float barWidth = (Width - totalGap) / (float)barCount;
            if (barWidth < 1.5f) barWidth = 1.5f;

            float midY = Height / 2f;
            float maxBarH = Height * 0.92f / 2f;

            float progress = (float)_value / _maximum;
            float progressX = Width * progress;

            // Easing для appear
            float appearEase = 1f - (float)Math.Pow(1f - _appearProgress, 3);

            for (int i = 0; i < barCount; i++)
            {
                float x = i * (barWidth + gap);

                // Stagger: каждый столбик появляется чуть позже предыдущего
                float barAppear = 1f;
                if (_appearProgress < 1f)
                {
                    float barT = (float)i / barCount;
                    // Окно длиной 0.3 пробегает по столбикам
                    float local = (appearEase - barT * 0.7f) / 0.3f;
                    barAppear = Math.Max(0f, Math.Min(1f, local));
                    barAppear = 1f - (float)Math.Pow(1f - barAppear, 2);
                }

                float barAmplitude = _bars[i] * barAppear;
                float topH = barAmplitude * maxBarH;
                float botH = barAmplitude * maxBarH * 0.65f;

                bool played = (x + barWidth / 2f) <= progressX;

                Color col;
                if (played)
                {
                    // Лёгкая разница оттенка по высоте — для глубины
                    col = PlayedColor;
                }
                else
                {
                    // Hover-предпросмотр будущей позиции
                    if (_hoverX >= 0 && x <= _hoverX)
                        col = HoverPreview;
                    else
                        col = UnplayedColor;
                }

                // Прямоугольники с минимальным скруглением (1px) — чёткий стиль
                var rectTop = new RectangleF(x, midY - topH, barWidth, topH);
                var rectBot = new RectangleF(x, midY, barWidth, botH);

                using (var brush = new SolidBrush(col))
                {
                    if (rectTop.Height > 0.5f) g.FillRectangle(brush, rectTop);
                    if (rectBot.Height > 0.5f) g.FillRectangle(brush, rectBot);
                }
            }

            // Плейхед — тонкая белая линия (как в Spotify при scrubbing)
            if (_hoverX >= 0)
            {
                using (var pen = new Pen(Color.FromArgb(180, 255, 255, 255), 1f))
                {
                    g.DrawLine(pen, _hoverX, 0, _hoverX, Height);
                }
            }
        }

        private void UpdateValueFromMouse(int mouseX)
        {
            if (Width <= 0) return;
            int x = Math.Max(0, Math.Min(Width, mouseX));
            int newVal = (int)Math.Round(((double)x / Width) * _maximum);
            Value = newVal;
            Seek?.Invoke(this, newVal);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                _isDragging = true;
                UpdateValueFromMouse(e.X);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _hoverX = e.X;
            if (_isDragging && e.Button == MouseButtons.Left)
                UpdateValueFromMouse(e.X);
            else
                Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _isDragging = false;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hoverX = -1;
            Invalidate();
        }

        public async Task LoadFromAudioFileAsync(string filePath, int barCount = 160)
        {
            _analysisCts?.Cancel();
            _analysisCts = new CancellationTokenSource();
            var token = _analysisCts.Token;

            int seed = filePath?.GetHashCode() ?? 0;
            GenerateRandomBars(barCount, seed);
            StartAppearAnimation();

            try
            {
                float[] bars = await Task.Run(() => AnalyzeFile(filePath, barCount, token), token);
                if (token.IsCancellationRequested) return;
                if (bars != null && bars.Length > 0)
                {
                    if (IsHandleCreated && InvokeRequired)
                        BeginInvoke(new Action(() => SetWaveData(bars)));
                    else
                        SetWaveData(bars);
                }
            }
            catch (OperationCanceledException) { }
            catch { }
        }

        private static float[] AnalyzeFile(string filePath, int barCount, CancellationToken token)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath)) return null;

            using (var reader = new AudioFileReader(filePath))
            {
                long totalSamples = reader.Length / sizeof(float);
                if (totalSamples <= 0) return null;

                int channels = reader.WaveFormat.Channels;
                long framesTotal = totalSamples / channels;
                if (framesTotal <= 0) return null;

                long framesPerBar = Math.Max(1, framesTotal / barCount);

                var result = new float[barCount];
                var buffer = new float[4096];

                int currentBar = 0;
                long framesInCurrentBar = 0;
                float peakInCurrentBar = 0f;

                int read;
                while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (token.IsCancellationRequested) return null;

                    for (int i = 0; i < read; i += channels)
                    {
                        float frameMax = 0f;
                        for (int c = 0; c < channels && i + c < read; c++)
                        {
                            float s = Math.Abs(buffer[i + c]);
                            if (s > frameMax) frameMax = s;
                        }

                        if (frameMax > peakInCurrentBar) peakInCurrentBar = frameMax;
                        framesInCurrentBar++;

                        if (framesInCurrentBar >= framesPerBar && currentBar < barCount)
                        {
                            result[currentBar++] = peakInCurrentBar;
                            framesInCurrentBar = 0;
                            peakInCurrentBar = 0f;
                        }
                    }
                }

                if (currentBar < barCount && peakInCurrentBar > 0f)
                    result[currentBar++] = peakInCurrentBar;

                float max = 0f;
                for (int i = 0; i < result.Length; i++)
                    if (result[i] > max) max = result[i];

                if (max > 0.0001f)
                {
                    for (int i = 0; i < result.Length; i++)
                    {
                        float norm = result[i] / max;
                        norm = (float)Math.Sqrt(norm);
                        if (norm < 0.08f) norm = 0.08f;
                        if (norm > 1f) norm = 1f;
                        result[i] = norm;
                    }
                }
                else
                {
                    for (int i = 0; i < result.Length; i++) result[i] = 0.1f;
                }

                return result;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _appearTimer?.Stop();
                _appearTimer?.Dispose();
                _analysisCts?.Cancel();
                _analysisCts?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
