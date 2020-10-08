using LandlordsCS;
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

namespace NetworkWPF.Client
{
    /// <summary>
    /// ClientGameCallPage.xaml 的交互逻辑
    /// </summary>
    public partial class ClientGameCallPage : Page,INetwork
    {
        public User user;

        public ClientWindow clientWindow;


        CardsWidget[] mCardsWidgets;
        
        public ClientGameCallPage()
        {
            InitializeComponent();
            //五个Canvas
            //topCanvas;
            //centerCanvas;
            //leftCanvas;
            //rightCanvas;
            //bottomCanvas;

            //叫牌
            //buttonCallNull;
            //buttonCall;
            //buttonCallRob;

            //打牌
            //buttonOutNull;
            //buttonOut;


            
        }

        public void ProcessData(Package package, User sender)
        {
            
        }

        private void buttonCall_Click(object sender, RoutedEventArgs e)
        {
            mCardsWidgets = new CardsWidget[54];
            for (int i = 0; i < mCardsWidgets.Length; i++)
            {
                mCardsWidgets[i] = new CardsWidget();
                
                

                mCardsWidgets[i].SetNum(CardsBuf.GetCardName(i+1));
            }

            //手牌
            for (int i = 0; i < 17; i++)
            {
                CardsWidget wi = mCardsWidgets[i];
                Canvas.SetLeft(wi, 20 * i);
                Canvas.SetBottom(wi, 0);
                bottomCanvas.Children.Add(wi);
            }
            //左边
            for (int i = 17; i < 34; i++)
            {
                CardsWidget wi = mCardsWidgets[i];
                Canvas.SetTop(wi, 20 * i);
                Canvas.SetLeft(wi, 0);
                leftCanvas.Children.Add(wi);
            }
            //右边
            for (int i = 34; i < 51; i++)
            {
                CardsWidget wi = mCardsWidgets[i];
                Canvas.SetTop(wi, 20 * i);
                Canvas.SetRight(wi, 0);
                rightCanvas.Children.Add(wi);
            }

            ////底牌
            //for (int i = 0; i < 3; i++)
            //{
            //    CardsWidget wi = mCardsWidgets[i];
            //    Canvas.SetLeft(wi, 20 * i);
            //    Canvas.SetTop(wi, 0);
            //    topCanvas.Children.Add(wi);
            //}
            ////中间
            //for (int i = 34; i < 51; i++)
            //{
            //    CardsWidget wi = mCardsWidgets[i];
            //    Canvas.SetLeft(wi, 20 * i);
            //    Canvas.SetTop(wi, 0);
            //    centerCanvas.Children.Add(wi);
            //}

        }


    }
}
