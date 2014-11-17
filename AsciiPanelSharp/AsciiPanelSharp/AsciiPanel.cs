using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace AsciiPanelSharp
{
    public partial class AsciiPanel : Form
    {
        private const int CharWidth = 9;
        private const int CharHeight = 16;
        //private const Color DefaultBackgroundColor = Black;
        //private const Color DefaultForegroundColor = White;

        public int WidthInCharacters { get; set; }
        public int HeightInCharacters { get; set; }
        public int CursorX { get; set; }
        public int CursorY { get; set; }
        public Bitmap GlyphSprite { get; set; }
        public Bitmap[] Glyphs { get; set; }
        public char[,] Chars { get; set; }
        public Color[,] BackgroundColors { get; set; }
        public Color[,] ForegroundColors { get; set; }

        public AsciiPanel() : this(80, 24) {}

        public AsciiPanel(int width, int height)
        {
            if (width < 1 || height < 1)
            {
                throw new ArgumentOutOfRangeException("width and height must be greater than 0");
            }
            WidthInCharacters = width;
            HeightInCharacters = height;
            base.Size = new Size(CharWidth * WidthInCharacters, CharHeight * HeightInCharacters);

            Chars = new char[WidthInCharacters, HeightInCharacters];
            BackgroundColors = new Color[WidthInCharacters, HeightInCharacters];
            ForegroundColors = new Color[WidthInCharacters, HeightInCharacters];

            Glyphs = new Bitmap[256];

            this.LoadGlyphs();

            //this.Clear();

            InitializeComponent();

        }

        private void LoadGlyphs()
        {
            try
            {
                GlyphSprite = new Bitmap(Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cp437.png")));
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("FATAL ERROR: cp437.png was not found. Ensure cp437.png is in the root of the executable. Application will exit.");
                Application.Exit();
            }

            for (int i = 0; i < 256; i++)
            {
                int sx = (i%32)*CharWidth + 8;
                int sy = (i/32)*CharHeight + 8;

                var rect = new Rectangle(new Point(sx, sy), new Size(CharWidth, CharHeight) );
                Glyphs[i] = GlyphSprite.Clone(rect, GlyphSprite.PixelFormat);

                // save to disk for preview
                //Glyphs[i].Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "thingey.bmp"), ImageFormat.Png);
            }
        }

        public AsciiPanel Clear()
        {
            throw new NotImplementedException();
        }
    }
}
