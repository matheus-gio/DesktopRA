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
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
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
        List<Produtos> produtos = new List<Produtos>();
        private bool linha = false, circulo = false, quadrado = false, retangulo = false;
        private double size = 10;
        Drawing_Canvas drawingCanvas = new Drawing_Canvas();
        private List<StackPanel> binders = new List<StackPanel>(); // lista de quadrados

        public MainPage()
        {
            this.InitializeComponent();

            Thread t1 = new Thread(new ThreadStart(t1Async));
            t1.Start();
            Thread t_produtos = new Thread(new ThreadStart(produtosAsync));
            t_produtos.Start();

        }
        public async void getImagesAsync(string url, StackPanel stackpanel)
        {
            Debug.WriteLine("INICIO GET IMAGEMS");
            foreach (var art in produtos)
            {
                try
                {
                    try
                    {
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            HttpStatusCode result = default(HttpStatusCode);
                            var request = HttpWebRequest.Create(url + art.PictureMap);
                            request.Method = "HEAD";
                            using (var response = request.GetResponse() as HttpWebResponse)
                            {
                                if (response != null)
                                {
                                    result = response.StatusCode;
                                    if(result == HttpStatusCode.OK)
                                    {
                                        Image image = new Image();
                                        image.Source = new BitmapImage(
                                    new Uri(url + art.PictureMap, UriKind.Absolute));
                                        imagens.Add(image);
                                        prod_list.Add(art);
                                    }
                                    response.Close();
                                }
                            }
                                
                        });
                    }
                    catch
                    {
                        Debug.WriteLine("IMAGEM ERRO");
                    }
                }
                catch { Debug.WriteLine("ERROR"); }
            }

            for(int l = 0; l < imagens.Count;l++)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    StackPanel panel = new StackPanel()
                    {
                        Name = "NumPad",
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(5, 5, 5, 0),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch
                    };

                    Windows.UI.Xaml.Controls.TextBlock label = new Windows.UI.Xaml.Controls.TextBlock()
                    {
                        Text = prod_list[l].Name,
                        Margin = new Thickness(0, 40, 40, 0),
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        VerticalAlignment = VerticalAlignment.Stretch
                    };
                    Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image()
                    {
                        Source = imagens[l].Source,
                        Height = 100,
                        Width = 153,
                        Stretch = Windows.UI.Xaml.Media.Stretch.Fill,
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Margin = new Thickness(5, 5, 5, 5)
                    };

                    panel.Children.Add(image);
                    panel.Children.Add(label);
                    stackpanel.Children.Add(panel);

                });
            }
        }
        List<RALIBRARY.Produtos> prod_list = new List<Produtos>();
        List<Image> imagens = new List<Image>();
        private async void produtosAsync()
        {
            Objetos objetos = new Objetos();
            produtos = await objetos.getListAsync();
            getImagesAsync("http://julianoblanco-001-site3.ctempurl.com/Images/MapsAR/", Binder);
        }

        public async void AddImgtoCanvasAsync(Canvas canvas, Image image_)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                // abrir imagem
                canvas.Children.Clear();

                var bounds = Window.Current.Bounds;
                double height = bounds.Height;
                Debug.WriteLine(height);
                double width = bounds.Width;
                Debug.WriteLine(width);

                Image image = new Image()
                {
                    Height = height,
                    Width = width,
                    Source = image_.Source

                };
                Canvas.SetLeft(image, 0);
                Canvas.SetTop(image, 0);
                canvas.Children.Add(image);
            });
        }

        private async void t1Async()
        {

            Socket listenSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSock.Bind(new IPEndPoint(IPAddress.Any, 1209));
            listenSock.Listen(2);
            Objetos objetos = new Objetos();
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
                    else
                    {
                        for(int i=0;i<prod_list.Count;i++)
                        {
                            if (("selectbyname_" + prod_list[i].Name).Equals(string_msg)){
                                fullscreen = true;
                                setFull(fullscreen);
                                AddImgtoCanvasAsync(canvas,imagens[i]);
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SizeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            size = e.NewValue;
        }

        private void click_save(object sender, RoutedEventArgs e)
        {

        }

        private void canvas_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!menu_click)
            {
                if (quadrado)
                {
                    drawingCanvas.CreateSquare(canvas, e, size);
                }
                else if (retangulo)
                {
                    drawingCanvas.Create_Rectangle(canvas, e);
                }
                else if (circulo)
                {
                    drawingCanvas.CreateCircle(canvas, e, size);
                }else if (linha)
                {
                    drawingCanvas.CreateLine(canvas, size, e);
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
        
        private bool ferramentas = false;
        private void MenuFlyoutItem_Click_5(object sender, RoutedEventArgs e)
        {
            ferramentas = !ferramentas;
            if (!fullscreen)
            {
                var dialog = new MessageDialog("Coloque em tela cheia para desenhar");
                SizeSlider.Visibility = Visibility.Collapsed;
                TextBlockSlider.Visibility = Visibility.Collapsed;
                dialog.ShowAsync();
            }
            else
            {
                if (ferramentas)
                {
                    SizeSlider.Visibility = Visibility.Visible;
                    TextBlockSlider.Visibility = Visibility.Visible;
                }
                else
                {
                    SizeSlider.Visibility = Visibility.Collapsed;
                    TextBlockSlider.Visibility = Visibility.Collapsed;
                }
            }
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

        private async void setFull(bool status)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                ApplicationView view = ApplicationView.GetForCurrentView();
                Grid.SetRow(canvas, 0);
                Grid.SetRowSpan(canvas, 10);
                if (status)
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
            });
        }
 
        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (fullscreen)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    canvas.Children.Clear();
                });
            }
            fullscreen = !fullscreen;
            setFull(fullscreen);
        }
    }
}
