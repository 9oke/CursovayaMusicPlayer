using System.Drawing;
using System.Windows.Forms;

namespace MusicPlayer
{
    /// <summary>
    /// Простая тёмная форма в стиле Spotify
    /// </summary>
    public class PastelForm : Form
    {
        public PastelForm()
        {
            BackColor = Color.FromArgb(24, 24, 24);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            MinimizeBox = true;
            StartPosition = FormStartPosition.CenterScreen;
            DoubleBuffered = true;
        }
    }
}
