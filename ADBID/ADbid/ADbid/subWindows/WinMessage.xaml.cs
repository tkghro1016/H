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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ADbid.subWindows
{
    /// <summary>
    /// WinMessage.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WinMessage : Window
    {
        public WinMessage()
        {
            InitializeComponent();
        }

        //부모 창에서 메세지 받은대로 경고창 띄움
        public WinMessage(String Msg)
        {
            InitializeComponent();
            TbxMsgContent.Text = Msg;
        }

        #region 메소드
        #endregion

        #region 이벤트
        //닫기 버튼 눌렀을 시
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        //창 옮기기 커서 활성화
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
        #endregion
    }
}
