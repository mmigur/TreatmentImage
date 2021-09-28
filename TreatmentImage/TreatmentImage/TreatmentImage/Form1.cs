using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TreatmentImage
{
    public partial class Form1 : Form
    {
        private List<Bitmap> _bitmaps = new List<Bitmap>();
        private Random _random = new Random();

        public Form1()
        {
            InitializeComponent();
        }

        private async void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenFile.ShowDialog() == DialogResult.OK)
            {
                var stopWatch = Stopwatch.StartNew();
                Menu.Enabled = TrackBar.Enabled = false;
                ImageBox.Image = null;
                _bitmaps.Clear();
                var bitmap = new Bitmap(OpenFile.FileName);
                await Task.Run(() => { RunProccesing(bitmap); });
                Menu.Enabled = TrackBar.Enabled = true;
                stopWatch.Stop();
                Text = stopWatch.Elapsed.ToString();
            }
        }

        private void RunProccesing(Bitmap bitmap)
        {
            var pixels = GetPixels(bitmap);
            var pixelsInStep = (bitmap.Width * bitmap.Height) / 100;
            var currenrPixelsSet = new List<Pixel>(pixels.Count - pixelsInStep);

            for (int i = 0; i < TrackBar.Maximum; i++)
            {
                for (int j = 0; j < pixelsInStep; j++)
                {
                    var index = _random.Next(pixels.Count);
                    currenrPixelsSet.Add(pixels[index]);
                    pixels.RemoveAt(index);
                }
                var currentBitmap = new Bitmap(bitmap.Width, bitmap.Height);

                foreach (var pixel in currenrPixelsSet)
                    currentBitmap.SetPixel(pixel.Point.X, pixel.Point.Y, pixel.Color);
                _bitmaps.Add(currentBitmap);

                this.Invoke(new Action(() => { Text = $"{i}%"; }));
            }
            _bitmaps.Add(bitmap);
        }

        private List<Pixel> GetPixels(Bitmap bitmap)
        {
            var pixels = new List<Pixel>(bitmap.Width * bitmap.Height);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    pixels.Add(new Pixel()
                    {
                        Color = bitmap.GetPixel(x, y),
                        Point = new Point() { X = x, Y = y }
                    });
                }
            }
            return pixels;
        }

        private void TrackBar_Scroll(object sender, EventArgs e)
        {
            if (_bitmaps == null || _bitmaps.Count == 0)
            {
                return;
            }
            ImageBox.Image = _bitmaps[TrackBar.Value - 1];
        }
    }
}
