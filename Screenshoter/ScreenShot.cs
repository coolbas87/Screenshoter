using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Screenshoter
{
    public partial class ScreenShot : Form
    {
        Bitmap bmCurrentScreenshot;
        Bitmap bmScreenshot;
        Rectangle screenRect;
        Point startPoint;

        public ScreenShot()
        {
            InitializeComponent();
            this.Height = SystemInformation.VirtualScreen.Height;
            this.Width = SystemInformation.VirtualScreen.Width;
            DoFullScreenshot();
            this.BackgroundImage = (Image)bmScreenshot;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; //Double Buffer
                return cp;
            }
        }

        private void DoFullScreenshot()
        {
            bmScreenshot = new Bitmap(SystemInformation.VirtualScreen.Width, SystemInformation.VirtualScreen.Height,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            Graphics graph = Graphics.FromImage(bmScreenshot);
            graph.CopyFromScreen(SystemInformation.VirtualScreen.X, SystemInformation.VirtualScreen.Y,
                0, 0, SystemInformation.VirtualScreen.Size, CopyPixelOperation.SourceCopy);
            Icon cursor = Icon.FromHandle(Cursors.Default.Handle);
            graph.DrawIcon(cursor, Cursor.Position.X, Cursor.Position.Y);
            bmCurrentScreenshot = new Bitmap(bmScreenshot);
            using (SolidBrush brush = new SolidBrush(Color.FromArgb(90, 0, 0, 0)))
            {
                graph.FillRegion(brush, graph.Clip);
            }
            graph.Dispose();
        }

        private void ScreenShot_Paint(object sender, PaintEventArgs e)
        {
            using (Pen pen = new Pen(Color.FromArgb(255, 0, 0, 255), 3))
            {
                e.Graphics.DrawImage(bmCurrentScreenshot, screenRect, screenRect, GraphicsUnit.Pixel);
                e.Graphics.DrawRectangle(pen, screenRect);
                int W = screenRect.Right - screenRect.Left;
                int H = screenRect.Bottom - screenRect.Top;
                string sizeInfo = string.Format("{0:d} x {1:d}", W, H);
                Font sizeInfoFont = new Font("Tahoma", 8);
                SizeF sizeInfoWidth = e.Graphics.MeasureString(sizeInfo, sizeInfoFont);
                e.Graphics.DrawString(sizeInfo, sizeInfoFont, Brushes.Yellow, 
                    screenRect.Right - sizeInfoWidth.Width, screenRect.Bottom);
            }
        }

        private void ScreenShot_MouseUp(object sender, MouseEventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetImage(CropImage(bmCurrentScreenshot, screenRect));
            this.Close();
        }

        private Bitmap CropImage(Bitmap Screenshot, Rectangle cropRect)
        {
            Bitmap lBitmap = new Bitmap(cropRect.Width, cropRect.Height);
            Graphics lGraphic = Graphics.FromImage(lBitmap);
            lGraphic.DrawImage(Screenshot, 0, 0, cropRect, GraphicsUnit.Pixel);
            return lBitmap;
        }

        private Rectangle MakeRect(Point APt1, Point APt2)
        {
            int left, right, top, bottom = 0;

            if (APt1.X < APt2.X) 
            {
                left = APt1.X;
                right = APt2.X;
            } 
            else 
            {
                left = APt2.X;
                right = APt1.X;
            }
            if (APt1.Y < APt2.Y) 
            {
                top = APt1.Y;
                bottom = APt2.Y;
            } 
            else 
            {
                top = APt2.Y;
                bottom = APt1.Y;
            }

            int width = right - left;
            int height = bottom - top;

            return new Rectangle(left, top, width, height);
        }

        private void ScreenShot_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                startPoint = new Point(e.X, e.Y);
            }
            else if (e.Button == MouseButtons.Right)
            {
                this.Close();
            }
        }

        private void ScreenShot_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                screenRect = MakeRect(startPoint, new Point(e.X, e.Y));
            }
            this.Invalidate();
        }

        private void ScreenShot_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        } 
    }
}
