using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace ADbid.subWindows
{
    /// <summary>
    /// WinSetting.xaml에 대한 상호 작용 논리
    /// </summary>
    /// 

    public partial class WinSetting : Window
    {

        ADbid.Regedit Regedit = new ADbid.Regedit();
        ADbid.FileRW FileRW = new FileRW();
        private static string strCustomerID = null;
        private static string strAccLicense = null;
        private static string strSecKey = null;
        bool loginAccess = false;

        public WinSetting()
        {
            InitializeComponent();
            initSetting();
        }

        public WinSetting(bool login)
        {
            this.loginAccess = login;
            InitializeComponent();
            initSetting();
        }

        private void initSetting()
        {
            //창을 처음 띄울때 초기화 위치 잡음
            this.Left = SystemParameters.WorkArea.Width / 2 - this.Width / 2;
            this.Top = SystemParameters.WorkArea.Height / 2 - this.Height / 2;

            /*레지스터 탐색으로 
              자동실행, 자동 로그인, 자동 입찰, 엑세스 라이센스, 비밀키, 대기시간 등 가져와 화면애 뿌림 
            */
            if (Regedit.Reg_Read("AutoStart").Equals("true") && !loginAccess)
            {
                ChkAutoStart.IsChecked = true;
            }
            if (Regedit.Reg_Read("AutoLogin").Equals("true") && !loginAccess)
            {
                ChkAutoLogin.IsChecked = true;
            }
            if (Regedit.Reg_Read("AutoBid").Equals("true") && !loginAccess)
            {
                ChkAutoBid.IsChecked = true;
                if (Regedit.Reg_Read("AuthState").Equals("true"))
                {
                    AuthState.Content = "인증완료";
                }
            }
            if (!Regedit.Reg_Read("CustomerID").Equals("") && !loginAccess)
            {
                TxtCustomerID.Text = Regedit.Reg_Read("CustomerID");
            }
            if (!Regedit.Reg_Read("AccLicense").Equals("") && !loginAccess)
            {
                TxtAccLicense.Text = Regedit.Reg_Read("AccLicense");
            }
            if (!Regedit.Reg_Read("SecKey").Equals("") && !loginAccess)
            {
                TxtSecKey.Text = Regedit.Reg_Read("SecKey");
            }
            if (!Regedit.Reg_Read("WaitMin").Equals("") && !loginAccess)
            {
                TxtWaitMin.Text = Regedit.Reg_Read("WaitMin");
            }
        }


        #region 1. 메서드 - 인증, 팝업

        //액세스 라이센스, 비밀키를 가지고 인증하는 부분입니다.
        private bool AuthLicense()
        {
            //인증 성공<-작업->
            bool authResult = false;
            try {
                bool getData = LinkPython.getData(strAccLicense, strSecKey, strCustomerID);
                if (getData)
                {
                    FileRW.FileRead();
                    Regedit.Reg_Write("CustomerID", strCustomerID);
                    Regedit.Reg_Write("AccLicense", strAccLicense);
                    Regedit.Reg_Write("SecKey", strSecKey);
                    Regedit.Reg_Write("AuthState", "true");
                    authResult = true;
                }
                else
                {
                    //인증 실패
                    Regedit.Reg_Write("AuthState", "false");
                    authResult = false;
                }
            } catch
            {
                MessageBox.Show("Auth Error 발생");
            }

            return authResult;
        }

        //인증이 안되거나 등의 경고창을 띄울떄 초기화 위치 잡음
        private async void PopupMsg(String Content, bool isAuth)
        {
            WinMessage popUp = new WinMessage(Content);
            //위치 정해야함 <-작업->
            popUp.Left = SystemParameters.WorkArea.Width / 2;
            popUp.Top = SystemParameters.WorkArea.Height / 2;

            if (isAuth == true)
            {
                popUp.Show();
                strCustomerID = TxtCustomerID.Text;
                strAccLicense = TxtAccLicense.Text;
                strSecKey = TxtSecKey.Text;
                if (!Regedit.Reg_Read("CustomerID").Equals(strCustomerID)){
                    LinkPython.listBidDataTmp = new List<BidData>();
                }
                var updateTask = Task<bool>.Factory.StartNew(AuthLicense);
                await updateTask;
                if (updateTask.Result == true)
                {
                    popUp.TbxMsgContent.Text = "네이버 검색광고 API를 인증하였습니다.";
                    AuthState.Content = "인증완료";
                    AuthState.Foreground = Brushes.Black;
                    ((Border)AuthState.Parent).BorderBrush = Brushes.Black;
                }
                else
                {
                    popUp.TbxMsgContent.Text = "엑세스라이선스 또는 비밀키를 다시 확인해 주세요.";
                    AuthState.Content = "미인증";
                    AuthState.Foreground = Brushes.Red;
                    ((Border)AuthState.Parent).BorderBrush = Brushes.Red;
                }
            }
            else
            {
                popUp.ShowDialog();
            }
        }

        /*
         * 저장 버튼을 누를때 txtWaitMin가 비어있으면 Null Error 메세지 반환하는 메소드
         */
        private void FncTxtWaitMinError()
        {
            PopupMsg("전체 자동 입찰 성공 후 대기시간을 입력해 주세요.", false);
        }
        #endregion

        #region 2. 이벤트 
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (loginAccess)
            {
                Regedit.Reg_Del("CustomerID");
                Regedit.Reg_Del("AccLicense");
                Regedit.Reg_Del("SecKey");
                Regedit.Reg_Del("AuthState");
                Regedit.Reg_Del("AutoStart");
                Regedit.Reg_Del("AutoLogin");
                Regedit.Reg_Del("AutoBid");
                Regedit.Reg_Del("WaitMin");
            }
            this.Close();
        }

        private void SettingWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Mouse.OverrideCursor = Cursors.SizeAll;
            this.DragMove();
        }

        private void SettingWindow_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Mouse.OverrideCursor = Cursors.Arrow;
        }

        /// <summary>
        /// Key Down 이벤트에 대한 설정
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            /*
            txtAccLicense -  Enter event : Null -> 경고창(입력해주세요), else -> txtSecKey으로 탭
            txtSecKey -  Enter event : Null -> 경고창(입력해주세요), else -> 인증요청
            txtWaitMin -  Enter event : 3분 미만일때 Error Popup
             */
            if (sender == TxtAccLicense)
            {
                if (!e.Key.Equals(Key.Enter))
                    return;

                if (String.IsNullOrEmpty(TxtAccLicense.Text))
                {
                    PopupMsg("엑세스라이선스를 입력해 주세요.", false);
                }
                else
                {
                    var element = sender as UIElement;
                    if (element != null)
                        element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
            }
            else if (sender == TxtSecKey)
            {
                if (!e.Key.Equals(Key.Enter))
                    return;

                if (String.IsNullOrEmpty(TxtSecKey.Text))
                {
                    PopupMsg("비밀키를 입력해 주세요.", false);
                }
                else if (!String.IsNullOrEmpty(TxtAccLicense.Text))
                {
                    PopupMsg("엑세스라이선스와 비밀키를 등록중입니다. 잠시만 기다려주세요.", true);
                }
            }
            else if (sender == TxtWaitMin)
            {
                if (!e.Key.Equals(Key.Enter))
                    return;
                try
                {
                    int intWaitMin = Convert.ToInt32(TxtWaitMin.Text);
                    if (!String.IsNullOrEmpty(TxtWaitMin.Text) && intWaitMin < 3)
                    {
                        PopupMsg("전체 자동 입찰 성공 후 대기시간은 최소 3분 이상 입력해 주세요.", false);
                    }
                    else if (String.IsNullOrEmpty(TxtWaitMin.Text))
                    {
                        FncTxtWaitMinError();
                    }
                }
                catch
                {
                    PopupMsg("전체 자동 입찰 성공 후 대기시간에 숫자입력부탁드립니다", false);
                }

            }
        }

        //자동입찰 대기시간에 대하여 정규식으로 숫자만 입력가능하게
        private void TxtWaitMin_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //저장 버튼 눌렀을때 체크박스 참조하여 레지스터에 정보 저장
        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            int intWaitMin = Convert.ToInt32(TxtWaitMin.Text);
            if (ChkAutoStart.IsChecked == true)
            {
                //시작 프로세스 레지스터에 남김
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    try
                    {
                        if (key.GetValue("ADbid") == null) {
                            key.SetValue("ADbid", Environment.CurrentDirectory + "\\" + AppDomain.CurrentDomain.FriendlyName); 
                        }
                        key.Close();
                        Regedit.Reg_Write("AutoStart", "true");
                    }
                    catch (Exception ex) { 
                        MessageBox.Show("오류: " + ex.Message.ToString());
                    }
                }
            }
            else
            {
                //시작 프로세스 레지스터에서 지움
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    try
                    {
                        if (key.GetValue("ADbid") != null)
                        {
                            key.DeleteValue("ADbid",false);
                        }
                        key.Close();
                        Regedit.Reg_Write("AutoStart", "false");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("오류: " + ex.Message.ToString());
                    }
                }
            }
            if (ChkAutoLogin.IsChecked == true)
            {
                Regedit.Reg_Write("AutoLogin", "true");
            }
            else
            {
                Regedit.Reg_Write("AutoLogin", "false");
            }
            if (ChkAutoBid.IsChecked == true)
            {
                Regedit.Reg_Write("AutoBid", "true");
            }
            else
            {
                Regedit.Reg_Write("AutoBid", "false");
            }
            if (!String.IsNullOrEmpty(TxtWaitMin.Text) && !(intWaitMin < 3))
            {
                Regedit.Reg_Write("WaitMin", TxtWaitMin.Text);
            }
            else if (!String.IsNullOrEmpty(TxtWaitMin.Text) && intWaitMin < 3)
            {
                PopupMsg("전체 자동 입찰 성공 후 대기시간은 최소 3분 이상 입력해 주세요.", false);
                return;
            }
            else if (String.IsNullOrEmpty(TxtWaitMin.Text))
            {
                FncTxtWaitMinError();
                return;
            }
            PopupMsg("저장되었습니다.", false);
            this.Close();
        }

        //비밀키 옆의 인증버튼 눌렀을때
        private void BtnAuth_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(TxtAccLicense.Text))
            {
                PopupMsg("엑세스라이선스를 입력해 주세요.", false);
            }
            else if (String.IsNullOrEmpty(TxtSecKey.Text))
            {
                PopupMsg("비밀키를 입력해 주세요.", false);
            }
            else
            {
                PopupMsg("엑세스라이선스와 비밀키를 등록중입니다. 잠시만 기다려주세요.", true);
            }
        }

        #endregion

        private void Textbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.ImeProcessed)
            {
                e.Handled = true;
            }
        }
    }
}
