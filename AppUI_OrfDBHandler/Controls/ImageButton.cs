using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AppUI_OrfDBHandler.Controls
{
    public class ImageButton : Button
    {
        private int ComCtlMajorVersion = -1;
        private Bitmap themedImage;

        public ImageButton()
        {
            FlatStyle = FlatStyle.System;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref BUTTON_IMAGELIST lParam);

        [DllImport("comctl32.dll", EntryPoint = "DllGetVersion")]
        private static extern int GetCommonControlDLLVersion(ref DLLVERSIONINFO dvi);

        [Category("Appearance")]
        [DefaultValue(null)]
        [Description("The image on the face of the button.")]
        public Bitmap ThemedImage
        {
            get => themedImage;
            set
            {
                if (value != null)
                    SetImage(value);
                themedImage = value;
            }
        }

        public void SetImage(Bitmap image)
        {
            SetImage(new[] { image }, Alignment.Center, 0, 0, 0, 0);
        }

        public void SetImage(Bitmap image, Alignment align)
        {
            SetImage(new[] { image }, align, 0, 0, 0, 0);
        }

        public void SetImage(Bitmap image, Alignment align, int leftMargin, int topMargin, int rightMargin, int bottomMargin)
        {
            SetImage(new[] { image }, align, leftMargin, topMargin, rightMargin, bottomMargin);
        }

        public void SetImage(Bitmap normalImage, Bitmap hoverImage, Bitmap pressedImage, Bitmap disabledImage, Bitmap focusedImage)
        {
            SetImage(new[]
            {
                normalImage,
                hoverImage,
                pressedImage,
                disabledImage,
                focusedImage
            }, Alignment.Center, 0, 0, 0, 0);
        }

        public void SetImage(Bitmap normalImage, Bitmap hoverImage, Bitmap pressedImage, Bitmap disabledImage, Bitmap focusedImage, Alignment align)
        {
            SetImage(new[]
            {
                normalImage,
                hoverImage,
                pressedImage,
                disabledImage,
                focusedImage
            }, align, 0, 0, 0, 0);
        }

        public void SetImage(Bitmap normalImage, Bitmap hoverImage, Bitmap pressedImage, Bitmap disabledImage, Bitmap focusedImage, Alignment align, int leftMargin, int topMargin, int rightMargin, int bottomMargin)
        {
            SetImage(new[]
            {
                normalImage,
                hoverImage,
                pressedImage,
                disabledImage,
                focusedImage
            }, align, leftMargin, topMargin, rightMargin, bottomMargin);
        }

        [DefaultValue(false)]
        [Description("Determines whether the image for the disabled state will be generated automatically from the normal one.")]
        [Category("Appearance")]
        public bool GenerateDisabledImage { get; set; } = false;

        [DllImport("UxTheme")]
        private static extern bool IsThemeActive();

        [DllImport("UxTheme")]
        private static extern bool IsAppThemed();

        private static bool IsVisualStylesEnabled
        {
            get
            {
                if (OSFeature.Feature.IsPresent(OSFeature.Themes) && IsAppThemed())
                    return IsThemeActive();

                return false;
            }
        }

        public void SetImage(Bitmap[] images, Alignment align, int leftMargin, int topMargin, int rightMargin, int bottomMargin)
        {
            if (GenerateDisabledImage)
            {
                if (images.Length == 1)
                {
                    var image = images[0];
                    images = new[]
                    {
                        image,
                        image,
                        image,
                        image,
                        image
                    };
                }
                images[3] = DrawImageDisabled(images[3]);
            }

            if (ComCtlMajorVersion < 0)
            {
                var dvi = new DLLVERSIONINFO
                {
                    cbSize = Marshal.SizeOf(typeof(DLLVERSIONINFO))
                };
                GetCommonControlDLLVersion(ref dvi);
                ComCtlMajorVersion = dvi.dwMajorVersion;
            }

            if (ComCtlMajorVersion >= 6 && FlatStyle == FlatStyle.System && IsVisualStylesEnabled)
            {
                var rect = new RECT
                {
                    left = leftMargin,
                    top = topMargin,
                    right = rightMargin,
                    bottom = bottomMargin
                };

                var lParam = new BUTTON_IMAGELIST
                {
                    margin = rect,
                    uAlign = (int)align
                };
                ImageList = GenerateImageList(images);
                lParam.himl = ImageList.Handle;
                SendMessage(Handle, 5634, 0, ref lParam);
                return;
            }

            FlatStyle = FlatStyle.Standard;
            if (images.Length > 0)
                Image = images[0];

            switch (align)
            {
                case Alignment.Left:
                    ImageAlign = ContentAlignment.MiddleLeft;
                    break;
                case Alignment.Right:
                    ImageAlign = ContentAlignment.MiddleRight;
                    break;
                case Alignment.Top:
                    ImageAlign = ContentAlignment.TopCenter;
                    break;
                case Alignment.Bottom:
                    ImageAlign = ContentAlignment.BottomCenter;
                    break;
                case Alignment.Center:
                    ImageAlign = ContentAlignment.MiddleCenter;
                    break;
            }
        }

        private Bitmap DrawImageDisabled(Image image)
        {
            var disabledImage = new Bitmap(image);
            var sourceImage = new Bitmap(disabledImage.Width, disabledImage.Height);

            var graphics = Graphics.FromImage(sourceImage);

            graphics.DrawImage(disabledImage, 0, 0);
            ControlPaint.DrawImageDisabled(graphics, disabledImage, 0, 0, Color.Empty);
            graphics.Dispose();

            return sourceImage;
        }

        private unsafe ImageList GenerateImageList(IList<Bitmap> sourceImages)
        {
            var imageList = new ImageList
            {
                ColorDepth = ColorDepth.Depth32Bit
            };

            if (sourceImages.Count > 0)
            {
                for (var index = 0; index < sourceImages.Count; ++index)
                {
                    var controlHeight = Height;
                    var controlWidth = Width;

                    if (controlHeight > 256)
                        controlHeight = 256;
                    if (controlWidth > 256)
                        controlWidth = 256;

                    if (sourceImages[index].Width > controlWidth || sourceImages[index].Height > controlHeight)
                    {
                        var widthScalar = controlWidth / (double)sourceImages[index].Width;
                        var heightScalar = controlHeight / (double)sourceImages[index].Height;
                        var minScalar = widthScalar >= heightScalar ? heightScalar : widthScalar;

                        var imageWidth = (int)(sourceImages[index].Width * minScalar);
                        var imageHeight = (int)(sourceImages[index].Height * minScalar);

                        var bitmap = new Bitmap(imageWidth, imageHeight);
                        var graphics = Graphics.FromImage(bitmap);

                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.DrawImage(sourceImages[index], 0, 0, imageWidth, imageHeight);

                        sourceImages[index] = bitmap;
                    }
                }

                imageList.ImageSize = new Size(sourceImages[0].Width, sourceImages[0].Height);

                foreach (var image in sourceImages)
                {
                    imageList.Images.Add(image);

                    var lastImageInList = (Bitmap)imageList.Images[imageList.Images.Count - 1];

                    var rect = new Rectangle(new Point(0, 0), image.Size);
                    var bitmapdata1 = image.LockBits(rect, ImageLockMode.ReadOnly, image.PixelFormat);
                    var bitmapdata2 = lastImageInList.LockBits(rect, ImageLockMode.WriteOnly, image.PixelFormat);

                    try
                    {
                        var scan0_1 = (int*)(void*)bitmapdata1.Scan0;
                        var scan0_2 = (int*)(void*)bitmapdata2.Scan0;

                        for (var i = 0; i < lastImageInList.Height; ++i)
                        {
                            for (var j = 0; j < lastImageInList.Width; ++j)
                            {
                                scan0_2[j] = scan0_1[j];
                            }

                            scan0_1 += bitmapdata1.Stride >> 2;
                            scan0_2 += bitmapdata2.Stride >> 2;
                        }
                    }
                    finally
                    {
                        lastImageInList.UnlockBits(bitmapdata2);
                        image.UnlockBits(bitmapdata1);
                    }
                }
            }
            return imageList;
        }

        [Description("Determines whether a small dropdown arrow will painted.")]
        [DefaultValue(false)]
        [Category("Appearance")]
        public bool DropDown { get; set; } = false;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (!DropDown)
                return;

            if (m.Msg != 15)
            {
                if (m.Msg != 10)
                    return;
            }

            try
            {
                var graphics = CreateGraphics();
                var font = new Font("Marlett", Font.Size);
                var sizeF = graphics.MeasureString("6", font);
                var brush = Enabled ? SystemBrushes.ControlText : new SolidBrush(SystemColors.GrayText);
                graphics.DrawString("6", font, brush, Width - 4 - sizeF.Width, (Height - sizeF.Height) / 2.0f);
            }
            catch
            {
                Trace.WriteLine("Problem drawing dropdown arrow");
            }
        }

        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public enum Alignment
        {
            Left,
            Right,
            Top,
            Bottom,
            Center,
        }

        private struct BUTTON_IMAGELIST
        {
            public IntPtr himl;
            public RECT margin;
            public int uAlign;
        }

        private struct DLLVERSIONINFO
        {
            public int cbSize;
            public int dwMajorVersion;
            public int dwMinorVersion;
            public int dwBuildNumber;
            public int dwPlatformID;
        }
    }
}
