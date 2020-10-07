using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

namespace NetworkWPF.Client
{
    /// <summary>
    /// ClientGamePage.xaml 的交互逻辑
    /// </summary>
    public partial class ClientGamePage : Page,INetwork
    {
        public User user;
        public ClientWindow clientWindow;

        public ClientGamePage()
        {
            InitializeComponent();



            //Image image;

            ////所有图片加载到一个数组中
            //string path = System.IO.Directory.GetCurrentDirectory();
            //FileInfo directory = new FileInfo(path);
            //FileInfo directory2 = new FileInfo(directory.DirectoryName);
            //string sss = directory2.DirectoryName + "/Image/bot.png";
            //BitmapImage bitmapImage = new BitmapImage(new Uri(sss, UriKind.Absolute));

            //for (int i = 0; i < 20; i++)
            //{
            //    image = new Image();
            //    image.MouseDown += Cards_Click;
            //    image.Tag = i;
            //    image.Source = bitmapImage;
            //    mCanvas.Children.Add(image);
            //    Canvas.SetLeft(image, i * 20);
            //    Canvas.SetTop(image, 20);
            //}

            Label label;

            for(int i = 0; i < 20; i++)
            {
                label= new Label();
                label.MouseDown += Cards_Click;
                label.Width = 30;
                label.Content = i;
                mCanvas.Children.Add(label);
                Canvas.SetLeft(label, i * 30);
                Canvas.SetTop(label, 20);
            }



            ////注册名称
            //mCanvas.RegisterName("btn", button);
            ////获取控件
            //Button findbnt = mCanvas.FindName("btn") as Button;
        }

        public void ProcessData(Package package, User sender)
        {
           
        }

        private void Cards_Click(object sender, RoutedEventArgs e)
        {
            UIElement ele = sender as UIElement;
            if (ele!=null)
            {
                if (Canvas.GetTop(ele) == 20)
                {
                    Canvas.SetTop(ele, 0);
                }
                else
                {
                    Canvas.SetTop(ele, 20);
                }
            }
        }
    }
}
