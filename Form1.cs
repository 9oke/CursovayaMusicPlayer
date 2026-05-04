using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using NAudio.Wave;
using TagLib;

namespace MusicPlayer
{
    public partial class Form1 : PastelForm
    {
        List<string> playlist = new List<string>();
        List<TrackInfo> trackInfos = new List<TrackInfo>();
        int currentIndex = 0;

        private class TrackInfo
        {
            public string FilePath { get; set; }
            public string FileName { get; set; }
            public string Artist { get; set; }
            public string Album { get; set; }
            public string Genre { get; set; }
            public string Duration { get; set; }

            public override string ToString()
            {
                return $"{FileName} | {Artist} | {Album} | {Genre} | {Duration}";
            }
        }

        WaveOutEvent outputDevice;
        AudioFileReader audioFile;

        bool isShuffle = false;
        bool isRepeat = false;
        Random random = new Random();

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        // Fade-in анимация при смене трека
        System.Windows.Forms.Timer fadeTimer = new System.Windows.Forms.Timer();
        DateTime fadeStart;
        const float FadeDurationSec = 0.5f;
        Image pendingCover; // обложка, которую плавно показываем
        bool fadeActive = false;

        public Form1()
        {
            InitializeComponent();

            btnPause.Visible = false;
            ApplyToggleVisual(btnShuffle, isShuffle);
            ApplyToggleVisual(btnRepeat, isRepeat);

            timer.Interval = 250;
            timer.Tick += Timer_Tick;

            fadeTimer.Interval = 16;
            fadeTimer.Tick += FadeTimer_Tick;

            // Sidebar-навигация: клик переключает активный пункт
            btnNowPlaying.Click += SidebarNav_Click;
            btnLibrary.Click += SidebarNav_Click;
            btnPlaylists.Click += SidebarNav_Click;

            // Инициализация ListView в тёмном стиле
            StyleListView();
        }

        private void StyleListView()
        {
            lstTracks.OwnerDraw = true;
            lstTracks.DrawColumnHeader += LstTracks_DrawColumnHeader;
            lstTracks.DrawItem += LstTracks_DrawItem;
            lstTracks.DrawSubItem += LstTracks_DrawSubItem;
        }

        private void LstTracks_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            using (var bg = new SolidBrush(Color.FromArgb(24, 24, 24)))
                e.Graphics.FillRectangle(bg, e.Bounds);

            // Тонкий разделитель снизу
            using (var line = new Pen(Color.FromArgb(60, 60, 60)))
                e.Graphics.DrawLine(line, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);

            var headerFont = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            TextRenderer.DrawText(e.Graphics, e.Header.Text?.ToUpper() ?? "",
                headerFont,
                new Rectangle(e.Bounds.X + 8, e.Bounds.Y, e.Bounds.Width - 8, e.Bounds.Height),
                Color.FromArgb(140, 140, 140),
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            headerFont.Dispose();
        }

        private void LstTracks_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            // Фон строки
            Color bgColor;
            if (e.Item.Selected)
                bgColor = Color.FromArgb(50, 30, 215, 96); // зелёная подсветка
            else if ((e.ItemIndex & 1) == 0)
                bgColor = Color.FromArgb(28, 28, 28);
            else
                bgColor = Color.FromArgb(24, 24, 24);

            using (var bg = new SolidBrush(bgColor))
                e.Graphics.FillRectangle(bg, e.Bounds);

            e.DrawDefault = false;
        }

        private void LstTracks_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            Color textColor = e.Item.Selected
                ? Color.FromArgb(30, 215, 96)
                : Color.FromArgb(220, 220, 220);

            string text = e.SubItem.Text;
            var textRect = new Rectangle(e.Bounds.X + 8, e.Bounds.Y, e.Bounds.Width - 8, e.Bounds.Height);

            if (e.ColumnIndex == 0)
            {
                if (e.ItemIndex == currentIndex && playlist.Count > 0)
                {
                    // Зелёный треугольник вместо номера для активного трека
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    using (var brush = new SolidBrush(Color.FromArgb(30, 215, 96)))
                    {
                        var cy = e.Bounds.Y + e.Bounds.Height / 2;
                        var pts = new[]
                        {
                            new Point(e.Bounds.X + 18, cy - 5),
                            new Point(e.Bounds.X + 26, cy),
                            new Point(e.Bounds.X + 18, cy + 5)
                        };
                        e.Graphics.FillPolygon(brush, pts);
                    }
                }
                else
                {
                    var numColor = e.Item.Selected
                        ? Color.FromArgb(30, 215, 96)
                        : Color.FromArgb(140, 140, 140);
                    TextRenderer.DrawText(e.Graphics, text, e.Item.Font, textRect, numColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
                }
            }
            else
            {
                TextRenderer.DrawText(e.Graphics, text, e.Item.Font, textRect, textColor,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }
        }

        private void SidebarNav_Click(object sender, EventArgs e)
        {
            // Переключение активного пункта sidebar
            btnNowPlaying.Active = sender == btnNowPlaying;
            btnLibrary.Active = sender == btnLibrary;
            btnPlaylists.Active = sender == btnPlaylists;
        }

        private void ApplyToggleVisual(PastelButton btn, bool active)
        {
            try
            {
                btn.Toggled = active;
            }
            catch { /* ignore */ }
        }

        void LoadTrack(int index)
        {
            if (playlist.Count == 0) return;

            outputDevice?.Stop();
            outputDevice?.Dispose();
            audioFile?.Dispose();

            try
            {
                audioFile = new AudioFileReader(playlist[index]);
                outputDevice = new WaveOutEvent();
                outputDevice.Init(audioFile);
            }
            catch
            {
                // Если файл повреждён — пробуем следующий
                return;
            }

            ShowInfo();
            StartFadeIn();

            try
            {
                waveform.Maximum = Math.Max(1, (int)audioFile.TotalTime.TotalSeconds);
                waveform.Value = 0;
                _ = waveform.LoadFromAudioFileAsync(playlist[index], 160);
                lblTimeCurrent.Text = "00:00";
                lblTimeTotal.Text = audioFile.TotalTime.ToString(@"mm\:ss");
            }
            catch { /* ignore */ }

            // Перерисовать ListView чтобы обновить маркер активного трека
            lstTracks.Invalidate();
        }

        // ===== Fade-in эффект =====
        private void StartFadeIn()
        {
            fadeStart = DateTime.UtcNow;
            fadeActive = true;
            fadeTimer.Start();
            ApplyFadeAlpha(0f);
        }

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            float elapsed = (float)(DateTime.UtcNow - fadeStart).TotalSeconds;
            float t = Math.Min(1f, elapsed / FadeDurationSec);
            // EaseOutQuad
            float eased = 1f - (1f - t) * (1f - t);
            ApplyFadeAlpha(eased);
            if (t >= 1f)
            {
                fadeTimer.Stop();
                fadeActive = false;
            }
        }

        private void ApplyFadeAlpha(float a)
        {
            // Цвета title и artist становятся прозрачными → насыщенными
            int titleA = (int)(a * 255);
            int artistA = (int)(a * 255);

            lblTitle.ForeColor = Color.FromArgb(
                Math.Min(255, titleA),
                255, 255, 255);
            lblArtist.ForeColor = Color.FromArgb(
                Math.Min(255, artistA),
                180, 180, 180);

            // Альфа-блендинг обложки через временный Bitmap
            if (pendingCover != null)
            {
                var oldImage = picCover.Image;
                picCover.Image = ApplyAlphaToImage(pendingCover, a);
                oldImage?.Dispose();
            }
        }

        private Image ApplyAlphaToImage(Image src, float alpha)
        {
            if (src == null) return null;
            alpha = Math.Max(0f, Math.Min(1f, alpha));

            var bmp = new Bitmap(src.Width, src.Height);
            using (var g = Graphics.FromImage(bmp))
            {
                var matrix = new System.Drawing.Imaging.ColorMatrix
                {
                    Matrix33 = alpha
                };
                using (var attr = new System.Drawing.Imaging.ImageAttributes())
                {
                    attr.SetColorMatrix(matrix);
                    g.DrawImage(src,
                        new Rectangle(0, 0, src.Width, src.Height),
                        0, 0, src.Width, src.Height,
                        GraphicsUnit.Pixel, attr);
                }
            }
            return bmp;
        }

        void ShowInfo()
        {
            if (playlist.Count == 0 || currentIndex < 0 || currentIndex >= playlist.Count) return;

            try
            {
                using (var tagFile = TagLib.File.Create(playlist[currentIndex]))
                {
                    lblTitle.Text = tagFile.Tag.Title ?? Path.GetFileNameWithoutExtension(playlist[currentIndex]);
                    lblArtist.Text = tagFile.Tag.FirstPerformer ?? "Unknown";

                    try
                    {
                        var pictures = tagFile.Tag.Pictures;
                        if (pictures != null && pictures.Length > 0)
                        {
                            var picData = pictures[0].Data.Data;
                            using (var ms = new MemoryStream(picData))
                            {
                                pendingCover?.Dispose();
                                pendingCover = Image.FromStream(ms);
                            }
                        }
                        else
                        {
                            pendingCover?.Dispose();
                            pendingCover = null;
                            picCover.Image?.Dispose();
                            picCover.Image = null;
                        }
                    }
                    catch
                    {
                        pendingCover?.Dispose();
                        pendingCover = null;
                        picCover.Image?.Dispose();
                        picCover.Image = null;
                    }
                }
            }
            catch
            {
                lblTitle.Text = Path.GetFileNameWithoutExtension(playlist[currentIndex]);
                lblArtist.Text = "Unknown";
                pendingCover?.Dispose();
                pendingCover = null;
                picCover.Image?.Dispose();
                picCover.Image = null;
            }
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (audioFile == null) return;

            waveform.Maximum = (int)audioFile.TotalTime.TotalSeconds;

            int current = (int)audioFile.CurrentTime.TotalSeconds;
            if (current <= waveform.Maximum)
                waveform.Value = current;

            lblTimeCurrent.Text = audioFile.CurrentTime.ToString(@"mm\:ss");
            lblTimeTotal.Text = audioFile.TotalTime.ToString(@"mm\:ss");

            if (audioFile.CurrentTime >= audioFile.TotalTime)
                NextTrack();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    var files = Directory.EnumerateFiles(fbd.SelectedPath, "*.*", SearchOption.TopDirectoryOnly)
                        .Where(f => f.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase) ||
                                    f.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (files.Count == 0) return;

                    playlist = files;
                    currentIndex = 0;

                    trackInfos = new List<TrackInfo>();
                    foreach (var f in files)
                    {
                        try
                        {
                            using (var t = TagLib.File.Create(f))
                            {
                                var duration = t.Properties?.Duration ?? TimeSpan.Zero;
                                trackInfos.Add(new TrackInfo
                                {
                                    FilePath = f,
                                    FileName = Path.GetFileName(f),
                                    Artist = t.Tag.FirstPerformer ?? "Unknown",
                                    Album = t.Tag.Album ?? "",
                                    Genre = t.Tag.FirstGenre ?? "",
                                    Duration = duration.ToString(@"mm\:ss")
                                });
                            }
                        }
                        catch
                        {
                            trackInfos.Add(new TrackInfo
                            {
                                FilePath = f,
                                FileName = Path.GetFileName(f),
                                Artist = "Unknown",
                                Album = "",
                                Genre = "",
                                Duration = "00:00"
                            });
                        }
                    }

                    lstTracks.BeginUpdate();
                    lstTracks.Items.Clear();
                    for (int i = 0; i < trackInfos.Count; i++)
                    {
                        var t = trackInfos[i];
                        var item = new ListViewItem((i + 1).ToString());
                        item.SubItems.Add(t.FileName);
                        item.SubItems.Add(t.Artist);
                        item.SubItems.Add(t.Album);
                        item.SubItems.Add(t.Genre);
                        item.SubItems.Add(t.Duration);
                        item.Tag = i;
                        lstTracks.Items.Add(item);
                    }
                    lstTracks.EndUpdate();

                    LoadTrack(currentIndex);

                    btnPlay.Visible = true;
                    btnPause.Visible = false;
                }
            }
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            if (audioFile == null)
            {
                if (playlist.Count == 0) return;
                LoadTrack(currentIndex);
            }

            outputDevice?.Play();

            btnPlay.Visible = false;
            btnPause.Visible = true;

            timer.Start();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            outputDevice?.Pause();

            btnPause.Visible = false;
            btnPlay.Visible = true;

            timer.Stop();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            NextTrack();
        }

        private void btnPrev_Click(object sender, EventArgs e)
        {
            bool wasPlaying = btnPause.Visible;

            currentIndex--;
            if (currentIndex < 0)
                currentIndex = playlist.Count - 1;

            LoadTrack(currentIndex);

            if (wasPlaying)
            {
                outputDevice?.Play();
                timer.Start();
                btnPlay.Visible = false;
                btnPause.Visible = true;
            }
            else
            {
                btnPlay.Visible = true;
                btnPause.Visible = false;
            }
        }

        private void btnShuffle_Click(object sender, EventArgs e)
        {
            isShuffle = !isShuffle;
            ApplyToggleVisual(btnShuffle, isShuffle);
        }

        private void btnRepeat_Click(object sender, EventArgs e)
        {
            isRepeat = !isRepeat;
            ApplyToggleVisual(btnRepeat, isRepeat);
        }

        void NextTrack()
        {
            if (playlist.Count == 0) return;

            bool wasPlaying = btnPause.Visible;

            if (isRepeat)
            {
                // currentIndex остаётся прежним
            }
            else if (isShuffle)
            {
                currentIndex = random.Next(playlist.Count);
            }
            else
            {
                currentIndex++;
                if (currentIndex >= playlist.Count)
                    currentIndex = 0;
            }

            LoadTrack(currentIndex);

            if (wasPlaying)
            {
                outputDevice?.Play();
                timer.Start();
                btnPlay.Visible = false;
                btnPause.Visible = true;
            }
            else
            {
                btnPlay.Visible = true;
                btnPause.Visible = false;
            }
        }

        private void waveform_Seek(object sender, int newSeconds)
        {
            if (audioFile != null)
            {
                try
                {
                    audioFile.CurrentTime = TimeSpan.FromSeconds(
                        Math.Min(newSeconds, audioFile.TotalTime.TotalSeconds));
                }
                catch { }
            }
        }

        private void lstTracks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTracks.SelectedIndices.Count > 0)
            {
                int idx = lstTracks.SelectedIndices[0];
                if (idx >= 0 && idx < playlist.Count)
                {
                    bool wasPlaying = btnPause.Visible;
                    currentIndex = idx;
                    LoadTrack(currentIndex);

                    if (wasPlaying)
                    {
                        outputDevice?.Play();
                        timer.Start();
                        btnPlay.Visible = false;
                        btnPause.Visible = true;
                    }
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            timer.Stop();
            fadeTimer.Stop();
            outputDevice?.Stop();
            outputDevice?.Dispose();
            audioFile?.Dispose();
            picCover.Image?.Dispose();
            pendingCover?.Dispose();
            base.OnFormClosing(e);
        }

        private void lblTitle_Click(object sender, EventArgs e)
        {
            // зарезервировано
        }
    }
}
