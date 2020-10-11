using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
    /// ClientGameResultPage.xaml 的交互逻辑
    /// </summary>
    public partial class ClientGameResultPage : Page,INetwork
    {
        public User user;
        public ClientWindow clientWindow;


        public Label[] nameLabels = new Label[3];
        public Label[] pointLabels = new Label[3];

        public ClientGameResultPage()
        {
            InitializeComponent();

            nameLabels[0] = name_1;
            nameLabels[1] = name_2;
            nameLabels[2] = name_3;

            pointLabels[0] = point_1;
            pointLabels[1] = point_2;
            pointLabels[2] = point_3;
        }

        public void Show(Page frompage)
        {
            App.Current.Dispatcher.InvokeAsync(() => {
                clientWindow.mFrameLeft.Content = this;
            });
        }

        public void ProcessData(Package package, User sender)
        {
            if (!package.clsName.Equals("GameResultPage"))
            {
                return;
            }
            switch (package.funName)
            {
                case "onGameFinish":
                    onGameFinish(package.data, sender);
                    break;

                default:
                    break;
            }
        }

        public void onGameFinish(string data, User sender)
        {
            Dictionary<string, string> dic = JsonSerializer.Deserialize<Dictionary<string, string>>(data);

            int[] points = JsonSerializer.Deserialize<int[]>(dic["points"]);
            string[] userNames = JsonSerializer.Deserialize<string[]>(dic["userNames"]);
            bool isWin = Boolean.Parse(dic["isWin"]);
            bool isLand = Boolean.Parse(dic["isLand"]);


            App.Current.Dispatcher.InvokeAsync(() => {
                if (isWin)
                {
                    isWinLabel.Content = "胜利";
                }
                else
                {
                    isWinLabel.Content = "失败";
                }

                for (int i = 0; i < 3; i++)
                {
                    nameLabels[i].Content = userNames[i];
                    if (isLand)
                    {
                        pointLabels[i].Content = points[0].ToString();
                    }
                    else
                    {
                        pointLabels[i].Content = points[1].ToString();
                    }
                }
            });
        }



    }
}
