using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ADbid.subWindows
{
    /// <summary>
    /// WinToastMsg.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WinToastMsg : Window
    {
        public WinToastMsg()
        {
            InitializeComponent();
        }

        public WinToastMsg(string msg)
        {
            InitializeComponent();
            lblToastMsg.Content = msg;

            this.AllowsTransparency = true;
            this.Topmost = true;

            this.Visibility = Visibility.Visible;
            DoubleAnimation dba1 = new DoubleAnimation();
            dba1.From = 0;
            dba1.To = 3;
            dba1.Duration = new Duration(TimeSpan.FromSeconds(0.8));
            dba1.AutoReverse = true;
            // 애니메이션 종료 이벤트 ( ※ BeginAnimation 이전에 있어야 동작함)
            dba1.Completed += (s, a) =>
            {
                Close();
            };
            this.BeginAnimation(OpacityProperty, dba1);
        }

        private void SettingWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Mouse.OverrideCursor = Cursors.SizeAll;
            this.DragMove();
        }

        //창 늘리기 커서 활성화
        private void SettingWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Mouse.OverrideCursor = Cursors.Arrow;
        }
    }
}
