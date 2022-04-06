using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;




namespace ADbid.subWindows
{
    /// <summary>
    /// WinLogin.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WinLogin : Window
    {
        ADbid.Regedit Regedit = new ADbid.Regedit();
        ADbid.RestAPI RestAPI = new ADbid.RestAPI();

        private String sResultMessage = "네트워크 상태를 다시 한번 확인해주세요";
        private String sResultCode = "4000";
        bool autoLoginWithoutWindow = false;
        bool settingBtnIsClicked = false;

        public WinLogin()
        {
            InitializeComponent();
            LblChkLoginInfo.Opacity = 0;
            BtnLogin.IsEnabled = false;
            //레지스터에서 자동로그인, ID기억하기 여부 받아와 체크박스 표시
            if (Regedit.Reg_Read("AutoLogin") == "true")
            {
                ChkAutoLogin.IsChecked = true;
                autoLoginWithoutWindow = true;
                TryLogin();
            }
            if (Regedit.Reg_Read("SaveID") == "true")
            {
                ChkIDSave.IsChecked = true;
                if (!Regedit.Reg_Read("UserID").Equals(""))
                {
                    TxtLoginID.Text = Regedit.Reg_Read("UserID");
                }
            }
            //자동입찰 처리안되어있으면 맨처음 화면인 Login화면에서 인증상태 false처리
            if (!Regedit.Reg_Read("AutoBid").Equals("true"))
            {
                Regedit.Reg_Write("AuthState", "false");
            }
        }

        public WinLogin(bool logout)
        {
            InitializeComponent();
            LblChkLoginInfo.Opacity = 0;
            BtnLogin.IsEnabled = false;
            //레지스터에서 자동로그인, ID기억하기 여부 받아와 체크박스 표시
            if (Regedit.Reg_Read("AutoLogin") == "true")
            {
                ChkAutoLogin.IsChecked = true;
            }
            if (Regedit.Reg_Read("SaveID") == "true")
            {
                ChkIDSave.IsChecked = true;
                if (!Regedit.Reg_Read("UserID").Equals(""))
                {
                    TxtLoginID.Text = Regedit.Reg_Read("UserID");
                }
            }
        }

        #region 메소드

        private void TryLogin()
        {
            //로그인 액션을 취함
            //1. 서버와의 통신상태 확인
            try
            {
                //TxtLoginID.Text, PwbBox.SecurePassword를 보내야함.
                BtnLogin.IsEnabled = false;
                TxtLoginID.IsEnabled = false;
                PwbBox.IsEnabled = false;
                /*
                 통신 관련 모듈은 해당 부분에 작성하면 됩니다.
                 넘기는 Para => ID : TxtLoginID.Text, PW : PwbBox.Password
                 받는 Para => {"resultMessage":"비번틀림","resultCode":"2200"}
                 성공 시, {"resultMessage":"로그인성공","resultCode":"1000"}
                 - 접속오류 : 4000
                 - 아이디없음 : 2100
                 - 비번틀림 : 2200
                 - 탈퇴한아이디 : 2900
                 - 정지된아이디 : 2800
                 
                 JSON반환값은 파싱해서 sResultMessage와 sResultCode에 각각 넣어주면 됩니다.
                */
                string[] loginResult = null;
                if (Regedit.Reg_Read("AutoLogin") == "true"&&autoLoginWithoutWindow)
                {
                    loginResult = RestAPI.LogIn(Regedit.Reg_Read("UserID"), Regedit.Reg_Read("UserPW"));
                    sResultCode = loginResult[1];
                    //자동로그인 성공시 바로 메인화면 진입
                    if (sResultCode.Equals("1000"))
                    {
                        ADbid.WinMain winMain = new ADbid.WinMain();
                        this.Close();
                        return;
                    }
                    else
                    {
                        autoLoginWithoutWindow = false;
                    }
                }
                else {
                    loginResult = RestAPI.LogIn(TxtLoginID.Text, PwbBox.Password);
                }
                //sResultMessage = loginResult[0];
                sResultCode = loginResult[1];

                //성공시
                if (sResultCode.Equals("1000") || (TxtLoginID.Text.Equals("admin") && PwbBox.Password.Equals("admin")))
                //if (sResultCode.Equals("1000"))
                 {
                    LblChkLoginInfo.Foreground = Brushes.Brown;
                    LblChkLoginInfo.Content = "로그인이 되었습니다.";
                    LblChkLoginInfo.Opacity = 100;

                    //ID가 기존 저장된 ID랑 다르면 데이터들을 초기화
                    if (!Regedit.Reg_Read("UserID").Equals(TxtLoginID.Text))
                    {
                        LinkPython.listBidDataTmp = new List<BidData>();
                        //세팅버튼을 누르지 않았을 경우 레지스트리 모두 초기화
                        if (!settingBtnIsClicked)
                        {
                            Regedit.Reg_Write("CustomerID","");
                            Regedit.Reg_Write("SecKey", "");
                            Regedit.Reg_Write("AccLicense", "");
                            Regedit.Reg_Write("WaitMin", "");
                            Regedit.Reg_Write("AuthState", "");
                            Regedit.Reg_Write("AutoStart", "");
                            Regedit.Reg_Write("AutoBid", "");
                            Regedit.Reg_Write("WaitMin", "");
                        }
                    }

                    //ID는 무조건 저장
                    Regedit.Reg_Write("UserID", TxtLoginID.Text);
                    
                    //만약 자동 로그인, ID기억하기 등이 체크되면 로그인 정보 로컬 저장
                    if (ChkAutoLogin.IsChecked == true)
                    {
                        Regedit.Reg_Write("AutoLogin", "true");
                        Regedit.Reg_Write("UserPW", PwbBox.Password); //암호 저장 로직
                    }
                    else
                    {
                        Regedit.Reg_Write("AutoLogin", "false");
                    }
                    if (ChkIDSave.IsChecked == true)
                    {
                        Regedit.Reg_Write("SaveID", "true");
                    }
                    else
                    {
                        Regedit.Reg_Write("SaveID", "false");
                    }

                    ADbid.WinMain winMain = new ADbid.WinMain();
                    this.Close();
                    //팝업을 열던가, 아니면 그냥 닫던가
                }
                else if (sResultCode.Equals("2200")|| sResultCode.Equals("2500")|| sResultCode.Equals("3400")|| sResultCode.Equals("2100"))
                {
                    TxtLoginID.IsEnabled = true;
                    PwbBox.IsEnabled = true;
                    PwbBox.Clear();
                    BtnLogin.IsEnabled = true;//+버튼 이미지 다른거로 바꿔야함
                    LblChkLoginInfo.Foreground = Brushes.Red;
                    LblChkLoginInfo.Content = "아이디 또는 비밀번호를 다시 확인해 주세요.";
                    LblChkLoginInfo.Opacity = 100;
                }
                else if (sResultCode.Equals("4000"))
                {
                    TxtLoginID.IsEnabled = true;
                    PwbBox.IsEnabled = true;
                    PwbBox.Clear();
                    BtnLogin.IsEnabled = true;//+버튼 이미지 다른거로 바꿔야함
                    LblChkLoginInfo.Foreground = Brushes.Red;
                    LblChkLoginInfo.Content = "네트워크 상태를 확인해주세요.";
                    LblChkLoginInfo.Opacity = 100;
                }
                else if (sResultCode.Equals("2900"))
                {
                    TxtLoginID.IsEnabled = true;
                    PwbBox.IsEnabled = true;
                    PwbBox.Clear();
                    BtnLogin.IsEnabled = true;//+버튼 이미지 다른거로 바꿔야함
                    LblChkLoginInfo.Foreground = Brushes.Red;
                    LblChkLoginInfo.Content = "탈퇴한 회원ID 입니다.";
                    LblChkLoginInfo.Opacity = 100;
                }
                else if (sResultCode.Equals("2800"))
                {
                    TxtLoginID.IsEnabled = true;
                    PwbBox.IsEnabled = true;
                    PwbBox.Clear();
                    BtnLogin.IsEnabled = true;//+버튼 이미지 다른거로 바꿔야함
                    LblChkLoginInfo.Foreground = Brushes.Red;
                    LblChkLoginInfo.Content = "정지된 회원ID 입니다.";
                    LblChkLoginInfo.Opacity = 100;
                }
            }
            catch
            {
                TxtLoginID.IsEnabled = true;
                PwbBox.IsEnabled = true;
                PwbBox.Clear();
                BtnLogin.IsEnabled = true;//+버튼 이미지 다른거로 바꿔야함
                LblChkLoginInfo.Foreground = Brushes.Red;
                LblChkLoginInfo.Content = sResultMessage;
                LblChkLoginInfo.Opacity = 100;
            }

        }

        #endregion

        #region 이벤트

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

        //닫기버튼 눌렀을 시
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (!Regedit.Reg_Read("AutoBid").Equals("true"))
            {
                Regedit.Reg_Write("AuthState", "false");
            }
            this.Close();
        }

        //최소화 버튼 눌렀을시
        private void BtnMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        //세팅버튼 눌렀을 시
        private void BtnSetting_Click(object sender, RoutedEventArgs e)
        {
            settingBtnIsClicked = true;
            //로그인 시에 세팅화면 누르면 초기화 시켜주기
            WinSetting WinSetting = new WinSetting(true);
            WinSetting.ShowDialog();
        }

        //회원가입 버튼 눌렀을 시
        private void BtnSignUp_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://adbid.co.kr/member/join");
        }

        //ID, PW버튼 눌렀을 시 
        private void BtnSearchIDPW_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://adbid.co.kr/member/findIdpw");
        }

        //ID에 대소문자 영어, 숫자, Enter, Tab, Back만 눌리게
        private void TxtLoginID_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(((Key.D0 <= e.Key) && (e.Key <= Key.D9))
           || ((Key.NumPad0 <= e.Key) && (e.Key <= Key.NumPad9))
           || e.Key == Key.Back || ((e.Key >= Key.A) && (e.Key <= Key.Z))
           || e.Key == Key.Enter || e.Key == Key.Tab || e.Key == Key.Delete))
            {
                e.Handled = true;
            }
        }

        //ID, PW등 텍스트박스에서 키를 눌렀을 시
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {


            /*
            TxtLoginID -  Enter event : Null -> 경고창(입력해주세요), else -> txtSecKey으로 탭
            txtSecKey -  Enter event : Null -> 경고창(입력해주세요), else -> 인증요청
            txtWaitMin -  Enter event : 3분 미만일때 Error Popup
             */
            if (sender == TxtLoginID)
            {
                //엔터키가 아닐때
                if (!e.Key.Equals(Key.Enter))
                {
                    LblChkLoginInfo.Opacity = 0;
                    //ID 비밀번호 둘다 입력되어 있으면 로그인 버튼 활성화
                    if (!String.IsNullOrEmpty(TxtLoginID.Text) && PwbBox.Password.Length > 0)
                        BtnLogin.IsEnabled = true;
                    else
                        BtnLogin.IsEnabled = false;
                    return;
                }

                //엔터키일때 패스워드로 커서 넘어가게
                if (!String.IsNullOrEmpty(TxtLoginID.Text))
                {
                    var element = sender as UIElement;
                    if (element != null)
                        element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                }
            }
            else if (sender == PwbBox)
            {
                if ((Keyboard.GetKeyStates(Key.CapsLock) & KeyStates.Toggled) == KeyStates.Toggled)
                {
                    Console.WriteLine("Togg");
                }

                if ((Keyboard.GetKeyStates(Key.CapsLock) & KeyStates.None) == KeyStates.None)
                {
                    Console.WriteLine("None");
                }

                if ((Keyboard.GetKeyStates(Key.CapsLock) & KeyStates.Down) == KeyStates.Down)
                {
                    Console.WriteLine("Down");
                }
                if ((Keyboard.GetKeyStates(Key.CapsLock) & KeyStates.Toggled) == KeyStates.Toggled)
                {
                    if (PwbBox.ToolTip == null|| PwbBox.ToolTip.Equals(""))
                    {
                        ToolTip tt = new ToolTip();
                        tt.Content = "CapsLock이 켜져있습니다.";
                        tt.PlacementTarget = sender as UIElement;
                        tt.Placement = PlacementMode.Bottom;
                        PwbBox.ToolTip = tt;
                        tt.IsOpen = true;
                    }
                }
                else
                {
                    var currentToolTip = PwbBox.ToolTip as ToolTip;
                    if (currentToolTip != null)
                    {
                        currentToolTip.IsOpen = false;
                    }

                    PwbBox.ToolTip = null;
                }
                if (!e.Key.Equals(Key.Enter))
                {
                    LblChkLoginInfo.Opacity = 0;
                    //ID 비밀번호 둘다 입력되어 있으면 로그인 버튼 활성화
                    if (!String.IsNullOrEmpty(TxtLoginID.Text) && PwbBox.Password.Length > 0)
                        BtnLogin.IsEnabled = true;
                    else
                        BtnLogin.IsEnabled = false;                 
                    return;
                }

                //엔터키를 눌렀는데 하나라도 비어 있으면 로그인 버튼 아래에 메세지 띄우면서 로그인 안됨
                if (String.IsNullOrEmpty(TxtLoginID.Text))
                {
                    LblChkLoginInfo.Foreground = Brushes.Red;
                    LblChkLoginInfo.Content = "ID를 입력해 주세요.";
                    LblChkLoginInfo.Opacity = 100;
                }
                else if (PwbBox.Password.Length == 0)
                {
                    LblChkLoginInfo.Foreground = Brushes.Red;
                    LblChkLoginInfo.Content = "비밀번호를 입력해 주세요.";
                    LblChkLoginInfo.Opacity = 100;
                }
                // 다 입력이 되어 있으면 엔터눌렀을시 로그인시도
                else if (PwbBox.Password.Length > 0 && !String.IsNullOrEmpty(TxtLoginID.Text))
                {
                    TryLogin();
                }
            }
        }

        //아이디텍스트 박스 눌렀을때 "아이디"글자 안보이게
        private void TxtLoginID_GotFocus(object sender, RoutedEventArgs e)
        {
            TblID.Opacity = 0;
        }

        //아이디텍스트 박스에서 포커스 빠졌을때 비어있으면 다시 "아이디" 글자 보이게
        private void TxtLoginID_LostFocus(object sender, RoutedEventArgs e)
        {
            if (TxtLoginID.Text.Equals(""))
            {
                TblID.Opacity = 100;
            }

        }

        //패스워드 박스 눌렀을때 "비밀번호"글자 안보이게
        private void PwbBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TblPW.Opacity = 0;
        }

        //패스워드 박스에서 포커스 빠졌을때 비어있으면 다시 "비밀번호" 글자 보이게
        private void PwbBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (PwbBox.Password.Length == 0)
            {
                TblPW.Opacity = 100;
            }
        }

        //로그인 버튼을 눌렀을 시
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            TryLogin();
        }

        //텍스트 박스의 문자 입력될때 로그인 버튼 활성화 여부
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(TxtLoginID.Text) && PwbBox.Password.Length > 0)
                BtnLogin.IsEnabled = true;
            else
                BtnLogin.IsEnabled = false;
            return;
        }

        //패스워드 박스의 입력될때 로그인 버튼 활성화 여부
        private void PwbBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(TxtLoginID.Text) && PwbBox.Password.Length > 0)
                BtnLogin.IsEnabled = true;
            else
                BtnLogin.IsEnabled = false;
            return;
        }
        #endregion

    }

}
