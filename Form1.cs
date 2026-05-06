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
        int currentIndex = 0;

        WaveOutEvent outputDevice;
        AudioFileReader audioFile;

        bool isShuffle = false;
        bool isRepeat = false;
        Random random = new Random();

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        public Form1()
        {
            InitializeComponent();

            btnPause.Visible = false;
            ApplyToggleVisual(btnShuffle, isShuffle);
            ApplyToggleVisual(btnRepeat, isRepeat);

            timer.Interval = 250;
            timer.Tick += Timer_Tick;

            btnNowPlaying.Click += SidebarNav_Click;
            btnLibrary.Click += SidebarNav_Click;
            btnPlaylists.Click += SidebarNav_Click;

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
            Color bgColor;
            if (e.Item.Selected)
                bgColor = Color.FromArgb(50, 30, 215, 96);
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
            catch { }
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
                return;
            }

            ShowInfo();

            try
            {
                waveform.Maximum = Math.Max(1, (int)audioFile.TotalTime.TotalSeconds);
                waveform.Value = 0;
                lblTimeCurrent.Text = "00:00";
                lblTimeTotal.Text = audioFile.TotalTime.ToString(@"mm\:ss");
            }
            catch { }

            lstTracks.Invalidate();
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
                                picCover.Image?.Dispose();
                                picCover.Image = Image.FromStream(ms);
                            }
                        }
                        else
                        {
                            picCover.Image?.Dispose();
                            picCover.Image = null;
                        }
                    }
                    catch
                    {
                        picCover.Image?.Dispose();
                        picCover.Image = null;
                    }
                }
            }
            catch
            {
                lblTitle.Text = Path.GetFileNameWithoutExtension(playlist[currentIndex]);
                lblArtist.Text = "Unknown";
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

                    lstTracks.BeginUpdate();
                    lstTracks.Items.Clear();
                    for (int i = 0; i < files.Count; i++)
                    {
                        var f = files[i];
                        try
                        {
                            using (var t = TagLib.File.Create(f))
                            {
                                var duration = t.Properties?.Duration ?? TimeSpan.Zero;
                                var item = new ListViewItem((i + 1).ToString());
                                item.SubItems.Add(Path.GetFileName(f));
                                item.SubItems.Add(t.Tag.FirstPerformer ?? "Unknown");
                                item.SubItems.Add(t.Tag.Album ?? "");
                                item.SubItems.Add(duration.ToString(@"mm\:ss"));
                                lstTracks.Items.Add(item);
                            }
                        }
                        catch
                        {
                            var item = new ListViewItem((i + 1).ToString());
                            item.SubItems.Add(Path.GetFileName(f));
                            item.SubItems.Add("Unknown");
                            item.SubItems.Add("");
                            item.SubItems.Add("00:00");
                            lstTracks.Items.Add(item);
                        }
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
                // остаёмся на текущем
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
            outputDevice?.Stop();
            outputDevice?.Dispose();
            audioFile?.Dispose();
            picCover.Image?.Dispose();
            base.OnFormClosing(e);
        }

        private void lblTitle_Click(object sender, EventArgs e)
        {
            // зарезервировано
        }
    }
}
