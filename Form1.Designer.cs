namespace MusicPlayer
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnOpen = new Button();
            btnPlay = new Button();
            btnPause = new Button();
            btnNext = new Button();
            btnPrev = new Button();
            btnShuffle = new Button();
            btnRepeat = new Button();
            lblTitle = new Label();
            lblArtist = new Label();
            lblTime = new Label();
            trackBar1 = new TrackBar();
            lstTracks = new ListView();
            chNo = new ColumnHeader();
            chTitle = new ColumnHeader();
            chArtist = new ColumnHeader();
            chAlbum = new ColumnHeader();
            chGenre = new ColumnHeader();
            chDuration = new ColumnHeader();
            picCover = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)trackBar1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picCover).BeginInit();
            SuspendLayout();
            // 
            // btnOpen
            // 
            btnOpen.BackColor = SystemColors.ControlLightLight;
            btnOpen.Location = new Point(12, 12);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(75, 23);
            btnOpen.TabIndex = 0;
            btnOpen.Text = " Открыть";
            btnOpen.UseVisualStyleBackColor = false;
            btnOpen.Click += btnOpen_Click;
            // 
            // btnPlay
            // 
            btnPlay.Anchor = AnchorStyles.Bottom;
            btnPlay.FlatStyle = FlatStyle.Flat;
            btnPlay.Font = new Font("Segoe UI", 25F);
            btnPlay.Location = new Point(578, 716);
            btnPlay.Name = "btnPlay";
            btnPlay.Size = new Size(75, 75);
            btnPlay.TabIndex = 1;
            btnPlay.Text = "▶";
            btnPlay.UseVisualStyleBackColor = true;
            btnPlay.Click += btnPlay_Click;
            // 
            // btnPause
            // 
            btnPause.Anchor = AnchorStyles.Bottom;
            btnPause.BackColor = Color.Transparent;
            btnPause.FlatStyle = FlatStyle.System;
            btnPause.Font = new Font("Segoe UI", 40F);
            btnPause.ForeColor = Color.Transparent;
            btnPause.Location = new Point(572, 703);
            btnPause.Name = "btnPause";
            btnPause.RightToLeft = RightToLeft.No;
            btnPause.Size = new Size(93, 94);
            btnPause.TabIndex = 2;
            btnPause.Text = "⏸";
            btnPause.UseVisualStyleBackColor = false;
            btnPause.Click += btnPause_Click;
            // 
            // btnNext
            // 
            btnNext.Anchor = AnchorStyles.Bottom;
            btnNext.BackColor = Color.Transparent;
            btnNext.FlatStyle = FlatStyle.System;
            btnNext.Font = new Font("Times New Roman", 40F, FontStyle.Bold);
            btnNext.ForeColor = Color.Transparent;
            btnNext.Location = new Point(700, 711);
            btnNext.Name = "btnNext";
            btnNext.Size = new Size(87, 75);
            btnNext.TabIndex = 3;
            btnNext.Text = "⏭";
            btnNext.UseVisualStyleBackColor = false;
            btnNext.Click += btnNext_Click;
            // 
            // btnPrev
            // 
            btnPrev.Anchor = AnchorStyles.Bottom;
            btnPrev.BackColor = Color.Transparent;
            btnPrev.FlatStyle = FlatStyle.System;
            btnPrev.Font = new Font("Times New Roman", 40F, FontStyle.Bold);
            btnPrev.ForeColor = Color.Transparent;
            btnPrev.Location = new Point(451, 711);
            btnPrev.Name = "btnPrev";
            btnPrev.Size = new Size(87, 75);
            btnPrev.TabIndex = 4;
            btnPrev.Text = "⏮";
            btnPrev.UseVisualStyleBackColor = false;
            btnPrev.Click += btnPrev_Click;
            // 
            // btnShuffle
            // 
            btnShuffle.Anchor = AnchorStyles.Bottom;
            btnShuffle.BackColor = Color.Transparent;
            btnShuffle.FlatStyle = FlatStyle.System;
            btnShuffle.Font = new Font("Times New Roman", 40F, FontStyle.Bold);
            btnShuffle.ForeColor = Color.Transparent;
            btnShuffle.Location = new Point(340, 711);
            btnShuffle.Name = "btnShuffle";
            btnShuffle.Size = new Size(87, 75);
            btnShuffle.TabIndex = 5;
            btnShuffle.Text = "🔀";
            btnShuffle.UseVisualStyleBackColor = false;
            btnShuffle.Click += btnShuffle_Click;
            // 
            // btnRepeat
            // 
            btnRepeat.Anchor = AnchorStyles.Bottom;
            btnRepeat.BackColor = Color.Transparent;
            btnRepeat.FlatStyle = FlatStyle.System;
            btnRepeat.Font = new Font("Times New Roman", 40F, FontStyle.Bold);
            btnRepeat.ForeColor = Color.Transparent;
            btnRepeat.Location = new Point(813, 711);
            btnRepeat.Name = "btnRepeat";
            btnRepeat.Size = new Size(87, 75);
            btnRepeat.TabIndex = 6;
            btnRepeat.Text = "🔁";
            btnRepeat.UseVisualStyleBackColor = false;
            btnRepeat.Click += btnRepeat_Click;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Berlin Sans FB", 24F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = SystemColors.ActiveCaptionText;
            lblTitle.Location = new Point(340, 562);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(36, 35);
            lblTitle.TabIndex = 7;
            lblTitle.Text = "...";
            // 
            // lblArtist
            // 
            lblArtist.AutoSize = true;
            lblArtist.BackColor = Color.Transparent;
            lblArtist.Font = new Font("Berlin Sans FB", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblArtist.ForeColor = SystemColors.ActiveCaptionText;
            lblArtist.Location = new Point(345, 605);
            lblArtist.Name = "lblArtist";
            lblArtist.Size = new Size(22, 21);
            lblArtist.TabIndex = 8;
            lblArtist.Text = "...";
            // 
            // lblTime
            // 
            lblTime.AutoSize = true;
            lblTime.BackColor = Color.Transparent;
            lblTime.Font = new Font("Berlin Sans FB", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTime.ForeColor = SystemColors.ActiveCaptionText;
            lblTime.Location = new Point(764, 605);
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(22, 21);
            lblTime.TabIndex = 9;
            lblTime.Text = "...";
            // 
            // trackBar1
            // 
            trackBar1.Location = new Point(340, 652);
            trackBar1.Name = "trackBar1";
            trackBar1.Size = new Size(560, 45);
            trackBar1.TabIndex = 10;
            trackBar1.TickStyle = TickStyle.None;
            trackBar1.Scroll += trackBar1_Scroll;
            // 
            // lstTracks
            // 
            lstTracks.Columns.AddRange(new ColumnHeader[] { chNo, chTitle, chArtist, chAlbum, chGenre, chDuration });
            lstTracks.Font = new Font("Montserrat Medium", 12F, FontStyle.Regular, GraphicsUnit.Point, 204);
            lstTracks.ForeColor = Color.Transparent;
            lstTracks.FullRowSelect = true;
            lstTracks.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lstTracks.Location = new Point(146, 11);
            lstTracks.MultiSelect = false;
            lstTracks.Name = "lstTracks";
            lstTracks.Size = new Size(950, 232);
            lstTracks.TabIndex = 11;
            lstTracks.UseCompatibleStateImageBehavior = false;
            lstTracks.View = View.Details;
            lstTracks.SelectedIndexChanged += lstTracks_SelectedIndexChanged;
            // 
            // chNo
            // 
            chNo.Text = "#";
            chNo.TextAlign = HorizontalAlignment.Center;
            chNo.Width = 40;
            // 
            // chTitle
            // 
            chTitle.Text = "Название";
            chTitle.Width = 250;
            // 
            // chArtist
            // 
            chArtist.Text = "Испольнитель";
            chArtist.Width = 200;
            // 
            // chAlbum
            // 
            chAlbum.Text = "Альбом";
            chAlbum.Width = 200;
            // 
            // chGenre
            // 
            chGenre.Text = "Жанр";
            chGenre.Width = 140;
            // 
            // chDuration
            // 
            chDuration.Text = "Время";
            chDuration.Width = 80;
            // 
            // picCover
            // 
            picCover.Location = new Point(345, 309);
            picCover.Name = "picCover";
            picCover.Size = new Size(250, 250);
            picCover.TabIndex = 12;
            picCover.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ButtonFace;
            ClientSize = new Size(1214, 803);
            Controls.Add(picCover);
            Controls.Add(lstTracks);
            Controls.Add(trackBar1);
            Controls.Add(lblTime);
            Controls.Add(lblArtist);
            Controls.Add(lblTitle);
            Controls.Add(btnRepeat);
            Controls.Add(btnShuffle);
            Controls.Add(btnPrev);
            Controls.Add(btnNext);
            Controls.Add(btnPause);
            Controls.Add(btnPlay);
            Controls.Add(btnOpen);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)trackBar1).EndInit();
            ((System.ComponentModel.ISupportInitialize)picCover).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnOpen;
        private Button btnPlay;
        private Button btnPause;
        private Button btnNext;
        private Button btnPrev;
        private Button btnShuffle;
        private Button btnRepeat;
        private Label lblTitle;
        private Label lblArtist;
        private Label lblTime;
        private TrackBar trackBar1;
        private ListView lstTracks;
        private ColumnHeader chNo;
        private ColumnHeader chTitle;
        private ColumnHeader chArtist;
        private ColumnHeader chAlbum;
        private ColumnHeader chGenre;
        private ColumnHeader chDuration;
        private PictureBox picCover;
    }
}
