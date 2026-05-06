using System.Drawing;
using System.Windows.Forms;

namespace MusicPlayer
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlSidebar = new Panel();
            btnOpen = new PastelButton();
            btnNowPlaying = new PastelButton();
            btnLibrary = new PastelButton();
            btnPlaylists = new PastelButton();
            lblAppTitle = new Label();
            lblNavHeader = new Label();

            pnlMain = new Panel();
            pnlNowPlaying = new Panel();
            picCover = new PictureBox();
            lblTitle = new Label();
            lblArtist = new Label();
            lblNowPlayingTag = new Label();

            pnlPlaylist = new Panel();
            lblPlaylistHeader = new Label();
            lstTracks = new ListView();
            chNo = new ColumnHeader();
            chTitle = new ColumnHeader();
            chArtist = new ColumnHeader();
            chAlbum = new ColumnHeader();
            chGenre = new ColumnHeader();
            chDuration = new ColumnHeader();

            pnlBottomBar = new Panel();
            waveform = new WaveformControl();
            lblTimeCurrent = new Label();
            lblTimeTotal = new Label();
            btnShuffle = new PastelButton();
            btnPrev = new PastelButton();
            btnPlay = new PastelButton();
            btnPause = new PastelButton();
            btnNext = new PastelButton();
            btnRepeat = new PastelButton();

            pnlSidebar.SuspendLayout();
            pnlMain.SuspendLayout();
            pnlNowPlaying.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picCover).BeginInit();
            pnlPlaylist.SuspendLayout();
            pnlBottomBar.SuspendLayout();
            SuspendLayout();

            // 
            // pnlSidebar — 240px слева
            // 
            pnlSidebar.BackColor = Color.Transparent;


            pnlSidebar.Location = new Point(8, 8);
            pnlSidebar.Size = new Size(232, 692);
            pnlSidebar.Name = "pnlSidebar";
            pnlSidebar.Controls.Add(lblAppTitle);
            pnlSidebar.Controls.Add(btnOpen);
            pnlSidebar.Controls.Add(lblNavHeader);
            pnlSidebar.Controls.Add(btnNowPlaying);
            pnlSidebar.Controls.Add(btnLibrary);
            pnlSidebar.Controls.Add(btnPlaylists);

            // 
            // lblAppTitle
            // 
            lblAppTitle.AutoSize = true;
            lblAppTitle.BackColor = Color.Transparent;
            lblAppTitle.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblAppTitle.ForeColor = Color.FromArgb(255, 255, 255);
            lblAppTitle.Location = new Point(20, 22);
            lblAppTitle.Name = "lblAppTitle";
            lblAppTitle.Text = "♪  MusicPlayer";

            // 
            // btnOpen — главный CTA
            // 
            btnOpen.Accent = false;
            btnOpen.BackColor = Color.Transparent;
            btnOpen.CornerRadius = 4;
            btnOpen.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnOpen.ForeColor = Color.FromArgb(30, 215, 96);
            btnOpen.Location = new Point(16, 64);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(200, 40);
            btnOpen.TabIndex = 0;
            btnOpen.Text = "+  ОТКРЫТЬ ПАПКУ";
            btnOpen.SidebarItem = false;
            btnOpen.Click += btnOpen_Click;

            // 
            // lblNavHeader
            // 
            lblNavHeader.AutoSize = true;
            lblNavHeader.BackColor = Color.Transparent;
            lblNavHeader.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblNavHeader.ForeColor = Color.FromArgb(140, 140, 140);
            lblNavHeader.Location = new Point(20, 130);
            lblNavHeader.Name = "lblNavHeader";
            lblNavHeader.Text = "НАВИГАЦИЯ";

            // 
            // btnNowPlaying
            // 
            btnNowPlaying.SidebarItem = true;
            btnNowPlaying.Active = true;
            btnNowPlaying.CornerRadius = 4;
            btnNowPlaying.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnNowPlaying.Location = new Point(8, 156);
            btnNowPlaying.Name = "btnNowPlaying";
            btnNowPlaying.Size = new Size(216, 40);
            btnNowPlaying.TabIndex = 1;
            btnNowPlaying.Text = "♪    Сейчас играет";
            btnNowPlaying.TextAlign = ContentAlignment.MiddleLeft;

            // 
            // btnLibrary
            // 
            btnLibrary.SidebarItem = true;
            btnLibrary.CornerRadius = 4;
            btnLibrary.Font = new Font("Segoe UI", 10F);
            btnLibrary.Location = new Point(8, 200);
            btnLibrary.Name = "btnLibrary";
            btnLibrary.Size = new Size(216, 40);
            btnLibrary.TabIndex = 2;
            btnLibrary.Text = "▤    Библиотека";
            btnLibrary.TextAlign = ContentAlignment.MiddleLeft;

            // 
            // btnPlaylists
            // 
            btnPlaylists.SidebarItem = true;
            btnPlaylists.CornerRadius = 4;
            btnPlaylists.Font = new Font("Segoe UI", 10F);
            btnPlaylists.Location = new Point(8, 244);
            btnPlaylists.Name = "btnPlaylists";
            btnPlaylists.Size = new Size(216, 40);
            btnPlaylists.TabIndex = 3;
            btnPlaylists.Text = "≡    Плейлисты";
            btnPlaylists.TextAlign = ContentAlignment.MiddleLeft;

            // 
            // pnlMain — основная область
            // 
            pnlMain.BackColor = Color.Transparent;


            pnlMain.Location = new Point(248, 8);
            pnlMain.Size = new Size(1024, 692);
            pnlMain.Name = "pnlMain";
            pnlMain.Controls.Add(pnlNowPlaying);
            pnlMain.Controls.Add(pnlPlaylist);

            // 
            // pnlNowPlaying — обложка + название (часть pnlMain)
            // 
            pnlNowPlaying.BackColor = Color.Transparent;


            pnlNowPlaying.Location = new Point(0, 0);
            pnlNowPlaying.Size = new Size(1024, 240);
            pnlNowPlaying.Name = "pnlNowPlaying";
            pnlNowPlaying.Controls.Add(picCover);
            pnlNowPlaying.Controls.Add(lblNowPlayingTag);
            pnlNowPlaying.Controls.Add(lblTitle);
            pnlNowPlaying.Controls.Add(lblArtist);

            // 
            // picCover
            // 
            picCover.BackColor = Color.FromArgb(40, 40, 40);
            picCover.Location = new Point(28, 28);
            picCover.Name = "picCover";
            picCover.Size = new Size(184, 184);
            picCover.SizeMode = PictureBoxSizeMode.Zoom;
            picCover.TabStop = false;

            // 
            // lblNowPlayingTag
            // 
            lblNowPlayingTag.AutoSize = true;
            lblNowPlayingTag.BackColor = Color.Transparent;
            lblNowPlayingTag.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblNowPlayingTag.ForeColor = Color.FromArgb(180, 180, 180);
            lblNowPlayingTag.Location = new Point(232, 56);
            lblNowPlayingTag.Name = "lblNowPlayingTag";
            lblNowPlayingTag.Text = "СЕЙЧАС ИГРАЕТ";

            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Segoe UI", 32F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(255, 255, 255);
            lblTitle.Location = new Point(228, 80);
            lblTitle.Name = "lblTitle";
            lblTitle.Text = "Трек не выбран";
            lblTitle.Click += lblTitle_Click;

            // 
            // lblArtist
            // 
            lblArtist.AutoSize = true;
            lblArtist.BackColor = Color.Transparent;
            lblArtist.Font = new Font("Segoe UI", 13F);
            lblArtist.ForeColor = Color.FromArgb(180, 180, 180);
            lblArtist.Location = new Point(232, 150);
            lblArtist.Name = "lblArtist";
            lblArtist.Text = "Исполнитель";

            // 
            // pnlPlaylist
            // 
            pnlPlaylist.BackColor = Color.Transparent;


            pnlPlaylist.Location = new Point(0, 240);
            pnlPlaylist.Size = new Size(1024, 452);
            pnlPlaylist.Name = "pnlPlaylist";
            pnlPlaylist.Controls.Add(lblPlaylistHeader);
            pnlPlaylist.Controls.Add(lstTracks);

            // 
            // lblPlaylistHeader
            // 
            lblPlaylistHeader.AutoSize = true;
            lblPlaylistHeader.BackColor = Color.Transparent;
            lblPlaylistHeader.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblPlaylistHeader.ForeColor = Color.FromArgb(255, 255, 255);
            lblPlaylistHeader.Location = new Point(28, 16);
            lblPlaylistHeader.Name = "lblPlaylistHeader";
            lblPlaylistHeader.Text = "Очередь воспроизведения";

            // 
            // lstTracks
            // 
            lstTracks.BackColor = Color.FromArgb(24, 24, 24);
            lstTracks.BorderStyle = BorderStyle.None;
            lstTracks.Columns.AddRange(new ColumnHeader[] { chNo, chTitle, chArtist, chAlbum, chGenre, chDuration });
            lstTracks.Font = new Font("Segoe UI", 10F);
            lstTracks.ForeColor = Color.FromArgb(220, 220, 220);
            lstTracks.FullRowSelect = true;
            lstTracks.GridLines = false;
            lstTracks.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lstTracks.Location = new Point(28, 56);
            lstTracks.MultiSelect = false;
            lstTracks.Name = "lstTracks";
            lstTracks.Size = new Size(968, 380);
            lstTracks.UseCompatibleStateImageBehavior = false;
            lstTracks.View = View.Details;
            lstTracks.SelectedIndexChanged += lstTracks_SelectedIndexChanged;

            chNo.Text = "#";
            chNo.TextAlign = HorizontalAlignment.Center;
            chNo.Width = 50;
            chTitle.Text = "Название";
            chTitle.Width = 320;
            chArtist.Text = "Исполнитель";
            chArtist.Width = 200;
            chAlbum.Text = "Альбом";
            chAlbum.Width = 200;
            chGenre.Text = "Жанр";
            chGenre.Width = 100;
            chDuration.Text = "Время";
            chDuration.Width = 80;

            // 
            // pnlBottomBar — нижняя панель воспроизведения (Spotify-style)
            // Высота 92px: верхняя половина — кнопки, нижняя — waveform
            // 
            pnlBottomBar.BackColor = Color.Transparent;




            pnlBottomBar.Location = new Point(0, 708);
            pnlBottomBar.Size = new Size(1280, 92);
            pnlBottomBar.Name = "pnlBottomBar";
            pnlBottomBar.Controls.Add(btnShuffle);
            pnlBottomBar.Controls.Add(btnPrev);
            pnlBottomBar.Controls.Add(btnPlay);
            pnlBottomBar.Controls.Add(btnPause);
            pnlBottomBar.Controls.Add(btnNext);
            pnlBottomBar.Controls.Add(btnRepeat);
            pnlBottomBar.Controls.Add(lblTimeCurrent);
            pnlBottomBar.Controls.Add(waveform);
            pnlBottomBar.Controls.Add(lblTimeTotal);

            // 
            // Верхний ряд: 6 кнопок управления, по центру
            // Y = 6..50 для 44px кнопок, Y = 2..50 для 48px Play
            // 
            int controlsCenterX = 1280 / 2;

            // btnPlay — большая зелёная кругляшка по центру
            btnPlay.Accent = true;
            btnPlay.BackColor = Color.Transparent;
            btnPlay.Circular = true;
            btnPlay.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            btnPlay.ForeColor = Color.FromArgb(0, 0, 0);
            btnPlay.Location = new Point(controlsCenterX - 22, 4);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(44, 44);
            btnPlay.TabIndex = 4;
            btnPlay.Text = "▶";
            btnPlay.Click += btnPlay_Click;

            btnPause.Accent = true;
            btnPause.BackColor = Color.Transparent;
            btnPause.Circular = true;
            btnPause.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            btnPause.ForeColor = Color.FromArgb(0, 0, 0);
            btnPause.Location = new Point(controlsCenterX - 22, 4);
            btnPause.Name = "btnPause";
            btnPause.Size = new Size(44, 44);
            btnPause.TabIndex = 5;
            btnPause.Text = "❚❚";
            btnPause.Visible = false;
            btnPause.Click += btnPause_Click;

            btnPrev.BackColor = Color.Transparent;
            btnPrev.CornerRadius = 4;
            btnPrev.Font = new Font("Segoe UI", 12F);
            btnPrev.ForeColor = Color.FromArgb(179, 179, 179);
            btnPrev.Location = new Point(controlsCenterX - 70, 8);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(36, 36);
            btnPrev.TabIndex = 3;
            btnPrev.Text = "⏮";
            btnPrev.Click += btnPrev_Click;

            btnNext.BackColor = Color.Transparent;
            btnNext.CornerRadius = 4;
            btnNext.Font = new Font("Segoe UI", 12F);
            btnNext.ForeColor = Color.FromArgb(179, 179, 179);
            btnNext.Location = new Point(controlsCenterX + 34, 8);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(36, 36);
            btnNext.TabIndex = 6;
            btnNext.Text = "⏭";
            btnNext.Click += btnNext_Click;

            btnShuffle.BackColor = Color.Transparent;
            btnShuffle.CornerRadius = 4;
            btnShuffle.Font = new Font("Segoe UI", 11F);
            btnShuffle.ForeColor = Color.FromArgb(179, 179, 179);
            btnShuffle.Location = new Point(controlsCenterX - 116, 10);
            btnShuffle.Name = "btnShuffle";
            btnShuffle.Size = new Size(32, 32);
            btnShuffle.TabIndex = 2;
            btnShuffle.Text = "⇄";
            btnShuffle.Click += btnShuffle_Click;

            btnRepeat.BackColor = Color.Transparent;
            btnRepeat.CornerRadius = 4;
            btnRepeat.Font = new Font("Segoe UI", 11F);
            btnRepeat.ForeColor = Color.FromArgb(179, 179, 179);
            btnRepeat.Location = new Point(controlsCenterX + 84, 10);
            btnRepeat.Name = "btnRepeat";
            btnRepeat.Size = new Size(32, 32);
            btnRepeat.TabIndex = 7;
            btnRepeat.Text = "↻";
            btnRepeat.Click += btnRepeat_Click;

            // 
            // Нижний ряд: время слева, waveform по центру, время справа
            // Y = 54..86 (32px высота)
            // 
            lblTimeCurrent.BackColor = Color.Transparent;
            lblTimeCurrent.Font = new Font("Segoe UI", 8.5F);
            lblTimeCurrent.ForeColor = Color.FromArgb(160, 160, 160);
            lblTimeCurrent.Location = new Point(20, 60);
            lblTimeCurrent.Size = new Size(48, 20);
            lblTimeCurrent.Name = "lblTimeCurrent";
            lblTimeCurrent.Text = "00:00";
            lblTimeCurrent.TextAlign = ContentAlignment.MiddleRight;

            waveform.BackColor = Color.Transparent;
            waveform.Location = new Point(74, 52);
            waveform.Size = new Size(1132, 36);
            waveform.Name = "waveform";
            waveform.Seek += waveform_Seek;

            lblTimeTotal.BackColor = Color.Transparent;
            lblTimeTotal.Font = new Font("Segoe UI", 8.5F);
            lblTimeTotal.ForeColor = Color.FromArgb(160, 160, 160);
            lblTimeTotal.Location = new Point(1212, 60);
            lblTimeTotal.Size = new Size(48, 20);
            lblTimeTotal.Name = "lblTimeTotal";
            lblTimeTotal.Text = "00:00";
            lblTimeTotal.TextAlign = ContentAlignment.MiddleLeft;

            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1280, 800);
            Controls.Add(pnlSidebar);
            Controls.Add(pnlMain);
            Controls.Add(pnlBottomBar);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            Text = "MusicPlayer";

            pnlSidebar.ResumeLayout(false);
            pnlSidebar.PerformLayout();
            pnlMain.ResumeLayout(false);
            pnlNowPlaying.ResumeLayout(false);
            pnlNowPlaying.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picCover).EndInit();
            pnlPlaylist.ResumeLayout(false);
            pnlPlaylist.PerformLayout();
            pnlBottomBar.ResumeLayout(false);
            ResumeLayout(false);
        }

        private Panel pnlSidebar;
        private Label lblAppTitle;
        private Label lblNavHeader;
        private PastelButton btnOpen;
        private PastelButton btnNowPlaying;
        private PastelButton btnLibrary;
        private PastelButton btnPlaylists;

        private Panel pnlMain;
        private Panel pnlNowPlaying;
        private PictureBox picCover;
        private Label lblNowPlayingTag;
        private Label lblTitle;
        private Label lblArtist;

        private Panel pnlPlaylist;
        private Label lblPlaylistHeader;
        private ListView lstTracks;
        private ColumnHeader chNo;
        private ColumnHeader chTitle;
        private ColumnHeader chArtist;
        private ColumnHeader chAlbum;
        private ColumnHeader chGenre;
        private ColumnHeader chDuration;

        private Panel pnlBottomBar;
        private WaveformControl waveform;
        private Label lblTimeCurrent;
        private Label lblTimeTotal;
        private PastelButton btnPrev;
        private PastelButton btnPlay;
        private PastelButton btnPause;
        private PastelButton btnNext;
        private PastelButton btnShuffle;
        private PastelButton btnRepeat;
    }
}
