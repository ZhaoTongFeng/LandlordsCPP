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
    /// CardsWidget.xaml 的交互逻辑
    /// </summary>
    public partial class CardsWidget : UserControl
    {

        public bool isUp;


        public void SetNum(string str) {
            mTxt.Content = str;
            SetUp(false);
            Show();
        }

        public CardsWidget()
        {
            InitializeComponent();
            SetUp(false);

            this.MouseDown += mouseDown;
            this.MouseUp += mouseUp;
            this.MouseEnter += mouseEnter;
        }

        public static bool isMouseDown = false;

        private static bool dirction = false;


        public void Show()
        {
            this.Visibility = Visibility.Visible;
        }
        public void Hide()
        {
            this.Visibility = Visibility.Hidden;
            SetUp(false);
        }



        private void mouseEnter(object sender, MouseEventArgs e)
        {
            CardsWidget wi = sender as CardsWidget;
            if (isMouseDown&& dirction !=wi.isUp)
            {
                ChangeState(sender);
            }
        }

        private void mouseUp(object sender, MouseButtonEventArgs e)
        {
            isMouseDown = false;

        }

        private void mouseDown(object sender, MouseButtonEventArgs e)
        {
            CardsWidget wi = sender as CardsWidget;
            dirction = !wi.isUp;

            isMouseDown = true;
            ChangeState(sender);
        }

        private void ChangeState(object sender)
        {
            CardsWidget wi = sender as CardsWidget;
            wi.SetUp(!wi.isUp);
        }
        private void SetUp(bool isUp)
        {
            this.isUp = isUp;
            Canvas.SetBottom(this, isUp == true ? 20 : 0);
        }


    }
}
