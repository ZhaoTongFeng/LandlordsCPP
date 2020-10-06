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
    public partial class ClientGamePage : Page
    {
        public ClientGamePage()
        {
            InitializeComponent();

            //Button button = new Button();
            //button.Content = "123";
            //mCanvas.Children.Add(button);

            Image image;

            //所有图片加载到一个数组中
            string path = System.IO.Directory.GetCurrentDirectory();
            FileInfo directory = new FileInfo(path);
            FileInfo directory2 = new FileInfo(directory.DirectoryName);
            string sss = directory2.DirectoryName + "/Image/bot.png";
            BitmapImage bitmapImage = new BitmapImage(new Uri(sss, UriKind.Absolute));


            for (int i = 0; i < 20; i++)
            {
                image = new Image();
                image.Source = bitmapImage;
                mCanvas.Children.Add(image);
                Canvas.SetLeft(image, i * 20);
            }



            ////注册名称
            //mCanvas.RegisterName("btn", button);
            ////获取控件
            //Button findbnt = mCanvas.FindName("btn") as Button;
        }
    }
}
