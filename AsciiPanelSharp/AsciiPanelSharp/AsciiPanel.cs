using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        public Color DefaultBackgroundColor = Black;
        public Color DefaultForegroundColor = White;

        public int WidthInCharacters { get; set; }
        public int HeightInCharacters { get; set; }

        private int _cursorX;
        public int CursorX
        {
            get
            { return _cursorX; }
            set
            {
                if (value < 0 || value >= WidthInCharacters) 
                    throw new ArgumentOutOfRangeException("cursorX must be within range [0," + WidthInCharacters + "].");
                _cursorX = value;
            }
        }

        private int _cursorY;
        public int CursorY
        {
            get { return _cursorY; }
            set
            {
                if (value < 0 || value >= HeightInCharacters)
                    throw new ArgumentOutOfRangeException("cursorY must be within range [0," + HeightInCharacters + "].");
                _cursorY = value;
            }
        }
        public Bitmap GlyphSprite { get; set; }
        public Bitmap[] Glyphs { get; set; }
        public char[,] Chars { get; set; }
        public Color[,] BackgroundColors { get; set; }
        public Color[,] ForegroundColors { get; set; }

        public static Color Black = Color.FromArgb(0, 0, 0);
        public static Color Red = Color.FromArgb(128, 0, 0);
        public static Color Green = Color.FromArgb(0, 128, 0);
        public static Color Yellow = Color.FromArgb(128, 128, 0);
        public static Color Blue = Color.FromArgb(0, 0, 128);
        public static Color Magenta = Color.FromArgb(128, 0, 128);
        public static Color Cyan = Color.FromArgb(0, 128, 128);
        public static Color White = Color.FromArgb(192, 192, 192);
        public static Color BrightBlack = Color.FromArgb(128, 128, 128);
        public static Color BrightRed = Color.FromArgb(255, 0, 0);
        public static Color BrightGreen = Color.FromArgb(0, 255, 0);
        public static Color BrightYellow = Color.FromArgb(255, 255, 0);
        public static Color BrightBlue = Color.FromArgb(0, 0, 255);
        public static Color BrightMagenta = Color.FromArgb(255, 0, 255);
        public static Color BrightCyan = Color.FromArgb(0, 255, 255);
        public static Color BrightWhite = Color.FromArgb(255, 255, 255);

        public AsciiPanel() : this(80, 24) {}

        public AsciiPanel(int width, int height)
        {
            DoubleBuffered = true;
            Paint += new PaintEventHandler(PaintScreen);

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

            this.Clear();

            InitializeComponent();
        }


        private void PaintScreen(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            for (int x = 0; x < WidthInCharacters; x++)
            {
                for (int y = 0; y < HeightInCharacters; y++)
                {
                    Color bg = BackgroundColors[x, y];
                    Color fg = BackgroundColors[x, y];

                    var imageAttributes = new ImageAttributes();
                    var backgroundColorMap = new ColorMap();
                    var foregroundColorMap = new ColorMap();

                    backgroundColorMap.OldColor = Color.FromArgb(255, 0, 0, 0);
                    backgroundColorMap.NewColor = bg;

                    foregroundColorMap.OldColor = Color.FromArgb(255, 168, 168, 168);
                    foregroundColorMap.NewColor = fg;

                    ColorMap[] remapTable = {backgroundColorMap, foregroundColorMap};

                    imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

                    e.Graphics.DrawImage(Glyphs[Chars[x,y]], new Rectangle((x * CharWidth), (y * CharHeight), CharWidth, CharHeight), 0,0, CharWidth, CharHeight, GraphicsUnit.Pixel, imageAttributes);
                }
            }
        }

        private void LoadGlyphs()
        {
            try
            {
                GlyphSprite = new Bitmap(Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cp437.png")));
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("FATAL ERROR: cp437.png was not found. Ensure cp437.png is in the root of the executable." + ex.Message);
                Application.Exit();
            }

            for (int i = 0; i < 256; i++)
            {
                int sx = (i%32)*CharWidth + 8;
                int sy = (i/32)*CharHeight + 8;

                var rect = new Rectangle(new Point(sx, sy), new Size(CharWidth, CharHeight) );
                Glyphs[i] = GlyphSprite.Clone(rect, GlyphSprite.PixelFormat);

                // save to disk for preview
                //Glyphs[i].Save(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "thingey"+i+".bmp"), ImageFormat.Png);
            }
        }

        public AsciiPanel Clear()
        {
            return Clear(' ', 0, 0, WidthInCharacters, HeightInCharacters, DefaultForegroundColor,
                DefaultBackgroundColor);
        }

        public AsciiPanel Clear(char character)
        {
            return Clear(character, 0, 0, WidthInCharacters, HeightInCharacters, DefaultForegroundColor,
                DefaultBackgroundColor);
        }

        public AsciiPanel Clear(char character, Color foreground, Color background)
        {
            return Clear(character, 0, 0, WidthInCharacters, HeightInCharacters, foreground, background);
        }

        public AsciiPanel Clear(char character, int x, int y, int width, int height)
        {
            return Clear(character, x, y, width, height, DefaultForegroundColor, DefaultBackgroundColor);
        }

        public AsciiPanel Clear(char character, int x, int y, int width, int height, Color foreground, Color background)
        {
            for (int xo = x; xo < x + width; xo++)
            {
                for (int yo = y; yo < y + height; yo++)
                {
                    Write(character, xo, yo, foreground, background);
                }
            }

            return this;
        }

        public AsciiPanel Write(char character)
        {
            return Write(character, _cursorX, _cursorY, DefaultForegroundColor, DefaultBackgroundColor);
        }

        public AsciiPanel Write(char character, Color foreground)
        {
            return Write(character, _cursorX, _cursorY, foreground, DefaultBackgroundColor);
        }

        public AsciiPanel Write(char character, Color foreground, Color background)
        {
            return Write(character, _cursorX, _cursorY, foreground, background);
        }

        public AsciiPanel Write(char character, int x, int y)
        {
            return Write(character, x, y, DefaultForegroundColor, DefaultBackgroundColor);
        }

        public AsciiPanel Write(char character, int x, int y, Color foreground)
        {
            return Write(character, x, y, foreground, DefaultBackgroundColor);
        }

        public AsciiPanel Write(char character, int x, int y, Color foreground, Color background)
        {
            Chars[x, y] = character;
            ForegroundColors[x, y] = foreground;
            BackgroundColors[x, y] = background;
            _cursorX = x + 1;
            _cursorY = y;
            return this;
        }

        public AsciiPanel Write(string str)
        {
            return Write(str, _cursorX, _cursorY, DefaultForegroundColor, DefaultBackgroundColor);
        }

        public AsciiPanel Write(string str, Color foreground)
        {
            return Write(str, _cursorX, _cursorY, foreground, DefaultBackgroundColor);
        }

        public AsciiPanel Write(string str, Color foreground, Color background)
        {
            return Write(str, _cursorX, _cursorY, foreground, background);
        }

        public AsciiPanel Write(string str, int x, int y)
        {
            return Write(str, x, y, DefaultForegroundColor, DefaultBackgroundColor);
        }

        public AsciiPanel Write(string str, int x, int y, Color foreground)
        {
            return Write(str, x, y, foreground, DefaultBackgroundColor);
        }

        public AsciiPanel Write(string str, int x, int y, Color foreground, Color background)
        {
            for (int i = 0; i < str.Length; i++)
            {
                Write(str[i], x + i, y, foreground, background);
            }

            return this;
        }
    }
}
