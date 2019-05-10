using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tesseract;
using WindowsInput;

namespace FarmerMan
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const UInt32 WM_KEYDOWN = 0x0100;
        const int VK_W = 0x57;

        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_LBUTTONUP = 0x0202;

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);


        public MainWindow()
        {
            InitializeComponent();
        }

        [STAThread]
        private void StartBtn_Click(object sender, RoutedEventArgs e)
        {
            Thread.Sleep(5000);
            StartRunAndHitAsync();
        }


        private void StartRunAndHitAsync()
        {
            var processes = Process.GetProcessesByName("javaw");
            var inputSimulator = new InputSimulator();
            bool loop = true;

            while (loop) {
                /*
                foreach (Process proc in processes)
                {
                    PostMessage(proc.MainWindowHandle, WM_KEYDOWN, VK_W, 0);
                    PostMessage(proc.MainWindowHandle, WM_LBUTTONDOWN, 1, 0);
                }
                */
                inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_W);
                inputSimulator.Mouse.LeftButtonDown();

                var currentScreenshot = Screenshot();

                using (var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default))
                {
                    var processedImage = engine.Process(currentScreenshot, PageSegMode.Auto);
                    var imageText = processedImage.GetText();

                    if (imageText.Contains("Block -l4866"))
                    {
                        inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_W);
                        inputSimulator.Mouse.LeftButtonUp();

                        inputSimulator.Mouse.MoveMouseBy(-355, 0);

                        inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_W);

                        Thread.Sleep(500);

                        inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_W);

                        Thread.Sleep(3000);
                        inputSimulator.Mouse.MoveMouseBy(-244, 0);


                        inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_W);
                        inputSimulator.Mouse.LeftButtonDown();
                    }
                    /*
                    else if (imageText.Contains("Block -l4999"))
                    {
                        inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_W);
                        inputSimulator.Mouse.LeftButtonUp();

                        inputSimulator.Mouse.MoveMouseBy(355, 0);

                        inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_W);

                        Thread.Sleep(150);

                        inputSimulator.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.VK_W);

                        Thread.Sleep(3000);
                        inputSimulator.Mouse.MoveMouseBy(244, 0);


                        inputSimulator.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_W);
                        inputSimulator.Mouse.LeftButtonDown();
                    }
                    */
                }

                Thread.Sleep(2000);
            }

            /*
            var testttt = BitmapToImageSource(test);

            imageTest.Source = testttt;
            */
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private Bitmap Screenshot()
        {
            var bmp = new Bitmap(450, 300);

            var gfx = Graphics.FromImage(bmp);

            gfx.CopyFromScreen(0, 100, 0, 100, new System.Drawing.Size(450, 300));

            return bmp;
        }
    }
}
