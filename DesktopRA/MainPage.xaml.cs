using Newtonsoft.Json;
using RALIBRARY;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DesktopRA
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        Boolean fullscreen = false; double canvas_heig,canvas_wid;
        List<Product> produtos = new List<Product>();
        private double x_first_pick, y_first_pick;
        private bool linha = false, circulo = false, quadrado = false, retangulo = false, click = false;

        Drawing_Canvas drawingCanvas = new Drawing_Canvas();

        public MainPage()
        {
            this.InitializeComponent();

            Thread t1 = new Thread(new ThreadStart(t1Async));
            t1.Start();

            using (var client = new System.Net.WebClient())
            {
                var json = client.DownloadString("http://julianoblanco-001-site3.ctempurl.com/WebService/ProductList");
                List<Product> list = JsonConvert.DeserializeObject<List<Product>>(json);

                foreach (Product p in list)
                {
                    if (p.AR == true)
                        produtos.Add(p);
                }
            }
        }

        private async void t1Async()
        {

            Socket listenSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSock.Bind(new IPEndPoint(IPAddress.Any, 1209));
            listenSock.Listen(2);

            while (true)
            {
                byte[] recebida = new byte[1024];
                using (Socket newConnection = listenSock.Accept())
                {
                    newConnection.Receive(recebida);
                    string string_msg = Encoding.UTF8.GetString(recebida).Substring(0, Encoding.UTF8.GetString(recebida).IndexOf("\0"));

                    if (string_msg.Equals("handshake"))
                    {
                        byte[] msg_send = Encoding.UTF8.GetBytes("hi");
                        newConnection.Send(msg_send);
                    }
                }
            }
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SizeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {

        }

        private void click_save(object sender, RoutedEventArgs e)
        {

        }

        private bool first_pick = false;

        private void canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!menu_click)
            {
                if (quadrado)
                {
                    drawingCanvas.CreateSquare(canvas, e, 10);
                }
                else if (retangulo)
                {
                    drawingCanvas.Create_Rectangle(canvas, e);
                }
                else if (circulo)
                {
                    drawingCanvas.CreateCircle(canvas, e, 10);
                }else if (linha)
                {
                    drawingCanvas.CreateLine(canvas, 10, e);
                }
            }
        }

        private void canvas_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            
        }



        private void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            linha = true;
            quadrado = false;
            circulo = false;
            retangulo = false;
        }

        private bool menu_click = false;
        private void MenuFlyout_Opened(object sender, object e)
        {
        }

        private void MenuFlyout_Closed(object sender, object e)
        {
        }

        private void MenuFlyout_Opening(object sender, object e)
        {
            menu_click = false;
            if(circulo || quadrado)
            {
                drawingCanvas.Undo_Draw(canvas);
            }
        }

        private void canvas_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {

        }

        private void canvas_KeyDown(object sender, KeyRoutedEventArgs e)
        {
        }

        private void MenuFlyoutItem_Click_2(object sender, RoutedEventArgs e)
        {
            linha = false;
            quadrado = true; // true
            circulo = false;
            retangulo = false;
        }

        private void MenuFlyoutItem_Click_3(object sender, RoutedEventArgs e)
        {
            linha = false;
            quadrado = false;
            circulo = false;
            retangulo = true; // true
        }

        private void MenuFlyoutItem_Click_4(object sender, RoutedEventArgs e)
        {
            linha = false;
            quadrado = false;
            circulo = true;//true
            retangulo = false;
        }

        private void click_clear(object sender, RoutedEventArgs e)
        {

        }


        private void full_Screen(bool full)
        {

            ApplicationView view = ApplicationView.GetForCurrentView();
            Grid.SetRow(canvas, 0);
            Grid.SetRowSpan(canvas, 10);

            if (full)
            {
                view.TryEnterFullScreenMode();
                Grid.SetColumn(canvas, 0);
                Grid.SetColumnSpan(canvas, 10);
                Grid.SetRow(canvas, 0);
                Grid.SetRowSpan(canvas, 10);
                canvas.Margin = new Thickness(0, 0, 0, 0);
                canvas_wid = canvas.Width;
                canvas_heig = canvas.Height;
            }
            else
            {
                view.ExitFullScreenMode();
                Grid.SetColumn(canvas, 4);
                Grid.SetColumnSpan(canvas, 6);
                Grid.SetRow(canvas, 0);
                Grid.SetRowSpan(canvas, 9);
                canvas.Margin = new Thickness(0, 25, 10, 30);
                canvas.Width = canvas_wid;
                canvas.Height = canvas_heig;
            }
        }

        private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            fullscreen = !fullscreen;
            full_Screen(fullscreen);
        }
    }
}
