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
    public partial class Form1 : Form
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

        public Form1()
        {
            InitializeComponent();
            btnPause.Visible = false;
            // Инициализировать визуальные состояния переключателей
            ApplyToggleVisual(btnShuffle, isShuffle);
            ApplyToggleVisual(btnRepeat, isRepeat);

            timer.Interval = 500;
            timer.Tick += Timer_Tick;
        }

        private void ApplyToggleVisual(Button btn, bool active)
        {
            try
            {
                if (active)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.BackColor = Color.FromArgb(0, 120, 215); // accent blue
                    btn.ForeColor = Color.White;
                }
                else
                {
                    btn.FlatStyle = FlatStyle.System;
                    btn.BackColor = SystemColors.Control;
                    btn.ForeColor = SystemColors.ControlText;
                }
            }
            catch
            {
                // игнорируем ошибки при установке стилей
            }
        }

        void LoadTrack(int index)
        {
            if (playlist.Count == 0) return;

            outputDevice?.Stop();
            outputDevice?.Dispose();
            audioFile?.Dispose();

            audioFile = new AudioFileReader(playlist[index]);
            outputDevice = new WaveOutEvent();
            outputDevice.Init(audioFile);

            ShowInfo();

            // Подготовить индикаторы, но НЕ запускать воспроизведение.
            try
            {
                trackBar1.Maximum = Math.Max(1, (int)audioFile.TotalTime.TotalSeconds);
                trackBar1.Value = 0;
                lblTime.Text = "00:00 / " + audioFile.TotalTime.ToString(@"mm\:ss");
            }
            catch
            {
                // Без фатальной обработки — если нет данных о длительности.
            }
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

                    // Обложка
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

            trackBar1.Maximum = (int)audioFile.TotalTime.TotalSeconds;

            int current = (int)audioFile.CurrentTime.TotalSeconds;
            if (current <= trackBar1.Maximum)
                trackBar1.Value = current;

            lblTime.Text =
                audioFile.CurrentTime.ToString(@"mm\:ss") + " / " +
                audioFile.TotalTime.ToString(@"mm\:ss");

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

                    // Собрать информацию для отображения в списке треков
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
                            // Если не удалось прочитать теги — заполнить минимально
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

                    // Заполнить ListView в стиле медиаплеера
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
                        item.Tag = i; // store index
                        lstTracks.Items.Add(item);
                    }
                    lstTracks.EndUpdate();

                    // Подготовить первый трек, но не запускать воспроизведение.
                    LoadTrack(currentIndex);

                    // Обновить состояние кнопок
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

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (audioFile != null)
            {
                audioFile.CurrentTime = TimeSpan.FromSeconds(trackBar1.Value);
            }
        }

        private void lstTracks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTracks.SelectedIndices.Count > 0)
            {
                int idx = lstTracks.SelectedIndices[0];
                if (idx >= 0 && idx < playlist.Count)
                {
                    currentIndex = idx;
                    LoadTrack(currentIndex);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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
    }
}
