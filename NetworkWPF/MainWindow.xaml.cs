using System;
using System.Collections.Generic;
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

namespace NetworkWPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Top = 0;
            Left = 0;
        }

        public static List<ClientWindow> clientWindows = new List<ClientWindow>();
        public static ServerWindow serverWindow;


        public void OpenClients()
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    ClientWindow w1 = new ClientWindow();
                    w1.Title = "客户端" + i + "-" + j;
                    w1.Left = Width + (i % 2) * w1.Width;
                    w1.Top = (j % 2) * w1.Height;
                    w1.Owner = this;
                    w1.Show();
                    clientWindows.Add(w1);
                }
            }
        }

        public void OpenServer()
        {
            serverWindow = new ServerWindow();
            serverWindow.Title = "服务端";
            serverWindow.Left = 0;
            serverWindow.Top = Height;
            serverWindow.Owner = this;
            serverWindow.Show();
        }

        public void CloseClient()
        {
            foreach (Window item in clientWindows)
            {
                item.Close();
            }
        }

        public void CloseServer()
        {
            if (serverWindow!=null)
            {
                serverWindow.Close();
            }
            
        }


        private void btnOpenClient_Click(object sender, RoutedEventArgs e)
        {
            OpenClients();
        }

        private void btnOpenServer_Click(object sender, RoutedEventArgs e)
        {
            OpenServer();
        }

        private void btnOpenBoth_Click(object sender, RoutedEventArgs e)
        {
            OpenClients();
            OpenServer();
        }

        private void btnCloseClient_Click(object sender, RoutedEventArgs e)
        {
            CloseClient();

        }

        private void btnCloseServer_Click(object sender, RoutedEventArgs e)
        {
            CloseServer();
        }

        private void btnCloseBoth_Click(object sender, RoutedEventArgs e)
        {
            CloseClient();
            CloseServer();
            Close();
        }
    }
}
