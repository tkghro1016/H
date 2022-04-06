using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using ADbid.subWindows;
using WinForms = System.Windows.Forms;
using System.Windows.Threading;
using System.Data;
using System.Threading.Tasks;
using RestSharp;
using System.IO;
using System.Windows.Media.Effects;
using System.Text.RegularExpressions;

namespace ADbid
{
    //KeyBinding keyBinding = new KeyBinding(ApplicationCommands.NotACommand, Key.Down, ModifierKeys.Windows);
    //this.CommandBindings.Add(new KeyBinding(ApplicationCommands.NotACommand, Key.Down, ModifierKeys.Windows));

    /// <summary>
    /// winMain.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WinMain : Window
    {
        //this.InputBindings.Add(keyBinding);


        /// <summary>
        /// 메인 화면에서 타이틀.
        /// OK - 프로그램 종료 Cancel - 팝업 꺼짐
        /// </summary>
        CollectionView view;
        ADbid.Regedit Regedit = new ADbid.Regedit();
        ADbid.FileRW FileRW = new FileRW();
        int clickNum = 0;   // 클릭 횟수
        long bizMoney = 0;
        long Firsttime = 0;   // 첫번째 클릭시간
        string selectedNccGroupID = "";
        bool batchChecked = false;
        public bool isProgramOn = false;

        //각 정렬기준에 대한 상태값 저장.
        private bool BlSortNoAsc = false;
        private bool BlSortADGroupAsc = false;
        private bool BlSortKeywordAsc = false;
        private bool BlSortAverRankAsc = false;
        private bool BlSortAutoBidYNAsc = false;
        private bool BlSortIsPCYNAsc = false;
        private bool BlSortTargetRankAsc = false;
        private bool BlSortMaxBidAsc = false;
        private bool BlSortExpoCntAsc = false;
        private bool BlSortClickCntAsc = false;
        private static String home = Directory.GetCurrentDirectory();
        

        //LogData생성을 위한 것           
        DataTable bidLogData = new DataTable();
        DataColumn logColumn = new DataColumn("Log", typeof(string));

        //트레이 아이콘 생성을 위한 윈폼 설정
        public System.Windows.Forms.NotifyIcon notify = new System.Windows.Forms.NotifyIcon();

        //REST API
        ADbid.RestAPI RestAPI = new ADbid.RestAPI();

        public WinMain()
        {            
            InitializeComponent();
            CenterWindowOnScreen();
            MainDataView.ItemsSource = LinkPython.listBidDataTmp;

            view = (CollectionView)CollectionViewSource.GetDefaultView(MainDataView.ItemsSource);
            LblSearchKeyword.Visibility = Visibility.Hidden;
            bidLogData.Columns.Add(logColumn);
            LblUsrID.Content = Regedit.Reg_Read("UserID");

            if (!Regedit.Reg_Read("WaitMin").Equals(""))
            {
                DispatcherTimer timer = new DispatcherTimer();
                //timer.Interval = TimeSpan.FromTicks(int.Parse(Regedit.Reg_Read("WaitMin")) * 600000000);   // ticks 10000000 = 1초 600000000
                timer.Interval = TimeSpan.FromTicks(600000000);   // ticks 10000000 = 1초 600000000

                timer.Tick += OnTimedEvent;
                timer.Start();
            }

            DispatcherTimer webtimer = new DispatcherTimer();
            webtimer.Interval = TimeSpan.FromTicks(600000000);   // ticks 10000000 = 1초 600000000
            webtimer.Tick += WebOnTimedEvent;
            webtimer.Start();

            // 남은 일자 구하기 - 노송
            string[] strarr = RestAPI.Period(Regedit.Reg_Read("UserID")).Split(new Char[] {'-',' '});
            DateTime dt = new DateTime(int.Parse(strarr[0]), int.Parse(strarr[1]), int.Parse(strarr[2]));
            TimeSpan ts = dt - DateTime.Now;
            strarr = ts.ToString().Split(new Char[] { '.' });
            int restDays = 0;
            int.TryParse(strarr[0],out restDays);
            if (restDays < 0) { restDays = 0; }

            LblServiceDate.Content = "서비스 만료일 " + RestAPI.Period(Regedit.Reg_Read("UserID")) + " (" + String.Format("{0:#,0}",restDays) + "일 남음" + ")";
            getBizMoney();
            if (LinkPython.listBidDataTmp.Count > 0 || Regedit.Reg_Read("AutoBid").Equals("true"))
            {
                ImgNaver.Visibility = Visibility.Visible;
                lblbarImg.Visibility = Visibility.Visible;
                LblNaverInfo2.Visibility = Visibility.Visible;
                LblNaverInfo.Content = Regedit.Reg_Read("CustomerID") + " (비즈머니 " + string.Format("{0:#,0}", bizMoney) + "원)";
            }
            else
            {
                ImgNaver.Visibility = Visibility.Hidden;
                lblbarImg.Visibility = Visibility.Hidden;
                LblNaverInfo2.Visibility = Visibility.Hidden;
                LblNaverInfo.Content = "※우측 환경설정에서 인증 후 시스템사용 가능합니다.";
                LblNaverInfo.Foreground = Brushes.Red;
            }

            // 환경설정 "프로그램 시작시 자동입찰 On" => 자동으로 인증하고 환경설정
            if (Regedit.Reg_Read("AutoBid").Equals("true")) {
                AuthLicense();
            }


        }

        //배치 동기화 이벤트
        private async void OnTimedEvent(object sender, EventArgs e)
        {
            if (isProgramOn)
            {
                var updateTask = Task.Factory.StartNew(UpdateDataAndLog);
                await updateTask;
                LblSyncInfo.Content = "네이버 광고시스템과 " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 동기화를 완료하였습니다.";
                PnlAutoBidState.Visibility = Visibility.Visible;
                LblAutoBidState.Content = "(전체 키워드 " + LinkPython.listBidDataTmp.Count + "개 중 " +
                            LinkPython.listBidDataTmp.Where(item => item.AutoBidYN.Equals("ON")).ToArray().Length + "개 자동 입찰 중)";
                LblNaverInfo.Content = Regedit.Reg_Read("CustomerID") + " (비즈머니 " + string.Format("{0:#,0}", bizMoney) + "원)";
                view.Refresh();
            }
        }

        private void WebOnTimedEvent(object sender, EventArgs e)
        {
            if (isProgramOn)
            {
                Console.WriteLine(RestAPI.WebLog(Regedit.Reg_Read("UserID"))[0]);
                // 남은 일자 구하기 - 노송
                string[] strarr = RestAPI.Period(Regedit.Reg_Read("UserID")).Split(new Char[] { '-', ' ' });
                DateTime dt = new DateTime(int.Parse(strarr[0]), int.Parse(strarr[1]), int.Parse(strarr[2]));
                TimeSpan ts = dt - DateTime.Now;
                strarr = ts.ToString().Split(new Char[] { '.' });
                int restDays = 0;
                int.TryParse(strarr[0], out restDays);
                if (restDays < 0) { restDays = 0; }

                LblServiceDate.Content = "서비스 만료일 " + RestAPI.Period(Regedit.Reg_Read("UserID")) + " (" + String.Format("{0:#,0}", restDays) + "일 남음" + ")";
            }
        }

        private void UpdateDataAndLog()
        {
            bidLogData.Rows.Clear();
            //자동입찰 켜진 것들 로그로 남기기위해 전처리
            List<BidData> autoBidOnItems = LinkPython.listBidDataTmp.Where(item => item.AutoBidYN.Equals("ON")).ToList();
            foreach (BidData item in autoBidOnItems)
            {
                bidLogData.Rows.Add(item.CmpAndGrpName + "|" + item.Keyword + "/" + item.Status + "|" +
                item.AverRank + "|" + item.IsPCYN + "|" + item.IsMobileYN + "|" +
                item.TargetPCRank + "|" + item.TargetMobileRank + "|" + item.MaxBid + "|" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            //Get해오는 부분은 AccLincense, Seckey, CustomerID 등 유효성 검사하고 get함
            //네이버에 자동입찰
            LinkPython.updateData(Regedit.Reg_Read("AccLicense"), Regedit.Reg_Read("SecKey"), Regedit.Reg_Read("CustomerID"));
            //비즈머니 동기화
            getBizMoney();
            //자동 입찰 후 로그 남김
            FileRW.LogWrite(bidLogData);
        }

        private void getBizMoney()
        {
            try
            {
                if (!Regedit.Reg_Read("AccLicense").Equals("") && !Regedit.Reg_Read("SecKey").Equals("") && !Regedit.Reg_Read("CustomerID").Equals(""))
                {
                    var rest = new SearchAdApi("https://api.naver.com", Regedit.Reg_Read("AccLicense"), Regedit.Reg_Read("SecKey"));
                    var request = new RestRequest("/billing/bizmoney", Method.GET);
                    List<BizMoney> apiRequestRlt = rest.Execute<List<BizMoney>>(request, long.Parse(Regedit.Reg_Read("CustomerID")));
                    bizMoney = apiRequestRlt[0].bizmoney;
                }
            }
            catch
            {
                WinMessage popUp = new WinMessage("네이버 서버와의 통신이 원활하지 않습니다.");
                popUp.Left = SystemParameters.WorkArea.Width / 2 - popUp.Width / 2;
                popUp.Top = SystemParameters.WorkArea.Height / 2 - popUp.Height / 2;
            }
        }

        //ToastMsg 팝업
        private void popUpToastMsg(String msg)
        {
            WinToastMsg winToastMsg = new WinToastMsg(msg);
            //최대화 시에 위치 조정
            if (this.Width > 1060)
            {
                winToastMsg.Left = this.Width - winToastMsg.Width - 35;
                winToastMsg.Top = this.Height - winToastMsg.Height - 50;
            }
            else
            {
                winToastMsg.Left = this.Left + this.Width - winToastMsg.Width - 35;
                winToastMsg.Top = this.Top + this.Height - winToastMsg.Height - 50;
            }
        }

        #region 메소드
        //화면 위치 설정
        private void CenterWindowOnScreen()

        {
            this.Left = SystemParameters.WorkArea.Width - this.Width - 10;
            this.Top = SystemParameters.WorkArea.Height - this.Height;
        }


        // 0.4초 이내의 클릭횟수(Nth)인지 반환(로고이미지 더블클릭시 창 닫히게 하기 위해)
        private int ClickNth()
        {
            clickNum++;  // 클릭 횟수 증가
            // 현재시각 CurrentTime에 저장 
            long CurrentTime = DateTime.Now.Ticks;
            // 원클릭 시 실행
            if (CurrentTime - Firsttime > 4000000) // 0.4초 ( MS에서는 더블클릭 평균 시간을 0.4초로 보는거 같다.)
            {
                clickNum = 1;
                Firsttime = CurrentTime;
            }
            return clickNum;
        }

        //키워드 검색결과로 필터링해서 Grid에 띄우기
        public bool FilterByKeyword(object item)

        {
            BidData p = item as BidData;
            LblSearchKeyword.Content = "Result Of " + TbxKeyWord.Text;
            return p.Keyword.Contains(TbxKeyWord.Text);
        }

        #endregion


        #region 이벤트
        private void BtnMainClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            //MessageBoxResult result = MessageBox.Show("프로그램을 종료하시겠습니까?", strAppVer, MessageBoxButton.OKCancel);
            //if (result == MessageBoxResult.OK)
            //    System.Diagnostics.Process.GetCurrentProcess().Kill();
        }

        //최소화 버튼 눌렀을때
        private void BtnToMini_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        //최대화 버튼 눌렀을때, Normal버튼 눌렀을 때
        private void BtnToMax_Click(object sender, RoutedEventArgs e)
        {
            //OuterGrid.Margin = new Thickness(0);
            if (MainOuterBorder.Width != 1040)
            {
                // 축소 시
                MainOuterBorder.Width = 1040;
                MainOuterBorder.Height = 700;
                MainOuterBorder.BorderThickness = new Thickness(10);
                //MainDataGrid.Width = Double.NaN;
                //MainDataGrid.Height = Double.NaN;
                //MainInnerPanel.Width = Double.NaN;
                //MainInnerPanel.Height = Double.NaN;
                //MainDataView.Width = Double.NaN;
                //MainDataView.Height = Double.NaN;

                MainInnerPanel.Width = Double.NaN;
                MainInnerPanel.Height = Double.NaN;

                Thickness margin = MainInnerPanel.Margin;
                margin.Right = 0;
                margin.Top = -10;
                margin.Left = 0;
                margin.Bottom = -10;
                MainInnerPanel.Margin = margin;

                BtdtitleBar.Width = Double.NaN;
                Brd.Width = Double.NaN;
                subTitleBar.Width = Double.NaN;
            }
            else
            {
                // 최대 시
                //MainOuterBorder.Width = Double.NaN;
                //MainOuterBorder.Height = Double.NaN;
                //MainDataGrid.Width = MainOuterBorder.Width;
                //MainDataGrid.Height = MainOuterBorder.Height;
                //MainInnerPanel.Width = MainOuterBorder.Width;
                //MainInnerPanel.Height = MainOuterBorder.Height;
                //MainDataView.Width = this.ActualWidth;
                //MainDataView.Height = this.ActualHeight;
                MainOuterBorder.Width = SystemParameters.PrimaryScreenWidth;
                MainOuterBorder.Height = SystemParameters.MaximizedPrimaryScreenHeight;
                MainOuterBorder.BorderThickness = new Thickness(0);
                MainInnerPanel.Width = SystemParameters.PrimaryScreenWidth;
                MainInnerPanel.Height = SystemParameters.MaximizedPrimaryScreenHeight;

                Thickness margin = MainInnerPanel.Margin;
                //margin.Right = -10;
                margin.Top = 0;
                //margin.Left = -10;
                margin.Bottom = 0;
                MainInnerPanel.Margin = margin;

                BtdtitleBar.Width = SystemParameters.PrimaryScreenWidth;
                Brd.Width = SystemParameters.PrimaryScreenWidth;
                subTitleBar.Width = SystemParameters.PrimaryScreenWidth;
            }

            double widthOfLstItem = 0;
            widthOfLstItem = (this.WindowState == WindowState.Normal) ? SystemParameters.WorkArea.Width : 1000;
            this.WindowState = (this.WindowState == WindowState.Normal) ? WindowState.Maximized : WindowState.Normal;
            foreach (var item in LinkPython.listBidDataTmp)
            {
                item.LstItemWidth = widthOfLstItem;
            }
            view.Refresh();
        }

        //환경설정 버튼 눌렀을 때
        private void BtnSetting_Click(object sender, RoutedEventArgs e)
        {
            string CustomerID = Regedit.Reg_Read("CustomerID");
            ADbid.subWindows.WinSetting winSetting = new ADbid.subWindows.WinSetting();
            winSetting.ShowDialog();

            //인증완료를 다시 한 경우가 있을 수 있으므로 아이템 재처리
            if (!CustomerID.Equals(Regedit.Reg_Read("CustomerID")))
            {
                MainDataView.ItemsSource = LinkPython.listBidDataTmp;
                view = (CollectionView)CollectionViewSource.GetDefaultView(MainDataView.ItemsSource);
                view.Refresh();
                getBizMoney();
            }
            if (Regedit.Reg_Read("AuthState").Equals("true"))
            {
                LblNaverInfo.Content = Regedit.Reg_Read("CustomerID") + " (비즈머니 " + string.Format("{0:#,0}", bizMoney) + "원)";
                LblNaverInfo.Foreground = Brushes.White;
                LblNaverInfo2.Visibility = Visibility.Visible;
                lblbarImg.Visibility = Visibility.Visible;
                ImgNaver.Visibility = Visibility.Visible;
            }
        }

        //창 옮기기 커서 활성화
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Mouse.OverrideCursor = Cursors.SizeAll;
            this.DragMove();
        }

        //창 늘리기 커서 활성화
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Mouse.OverrideCursor = Cursors.Arrow;
        }

        //로고버튼 더블 클릭시 창 닫힘
        private void BtnLogo_Click(object sender, RoutedEventArgs e)
        {
            if (ClickNth() == 2)
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            }
        }

        //필터링(돋보기) 버튼 눌렀을 때
        void BtnFilter_Click(object sender, RoutedEventArgs e)
        {
            if (!String.IsNullOrEmpty(TbxKeyWord.Text))
                view.Filter = new Predicate<object>(FilterByKeyword); //Text있으면 필터링 
            else
            {
                view.Filter = null; //텍스트 없으면 필터링 해제
            }
        }

        //로그인창 눌렀을때 로그인 창 뜨게 -> 로그아웃 구현해야함
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            Regedit.Reg_Write("LoginState", "false");
            if (!Regedit.Reg_Read("AutoBid").Equals("true"))
            {
                Regedit.Reg_Write("AuthState", "false");
            }
            RestAPI.LogOut(Regedit.Reg_Read("UserID"));
            ADbid.subWindows.WinLogin winLogin = new ADbid.subWindows.WinLogin(true);
            this.Close();
        }

        //선택 일괄 변경 버튼이 눌렸을 경우
        private void BtnBatch_Click(object sender, RoutedEventArgs e)
        {
            ADbid.subWindows.WinBatchSelect winBatchSelect;
            var selectedKewords = LinkPython.listBidDataTmp.Where(item => item.IsSelected == true);

            if (selectedKewords.Count() == 0 && CboSelected.SelectedIndex > -1)
            {
                winBatchSelect = new ADbid.subWindows.WinBatchSelect(selectedNccGroupID);
                winBatchSelect.ShowDialog();
            }
            else if (selectedKewords.Count() > 0)
            {
                winBatchSelect = new ADbid.subWindows.WinBatchSelect();
                winBatchSelect.ShowDialog();
            }
            else
            {
                WinMessage popUp = new WinMessage("키워드나 캠패인그룹을 선택해주세요.");
                popUp.Left = SystemParameters.WorkArea.Width / 2 - popUp.Width / 2;
                popUp.Top = SystemParameters.WorkArea.Height / 2 - popUp.Height / 2;
            }
        }

        //선택 일괄 변경 버튼이 눌렸을 경우 - Url 연결필요
        private void BtnAppService_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://adbid.co.kr/service/serviceApply");
        }

        //새로고침 버튼이 눌렸을 경우
        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            view.Refresh();
        }

        //키워드 검색에서 Enter키가 입력되었을때 조회실행
        private void TbxKeyWord_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender == TbxKeyWord)
            {
                if (!e.Key.Equals(Key.Enter))
                {
                    return;
                }

                if (!String.IsNullOrEmpty(TbxKeyWord.Text) && e.Key.Equals(Key.Enter))
                {
                    view.Filter = new Predicate<object>(FilterByKeyword); //Text있으면 필터링 
                    LblSearchKeyword.Visibility = Visibility.Visible;
                }
                else
                {
                    view.Filter = null; //텍스트 없으면 필터링 해제
                }
            }
        }

        //변경 버튼 눌렀을 시
        private void BtnChangeSelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BidData item = (BidData)MainDataView.SelectedItem;
                MessageBox.Show("You Clicked : " + item.CmpAndGrpName + "\r\nDescription : ");
                //This is the code which will show the button click row data. Thank you.
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        //입찰로그 눌렀을 때
        private void BtnViewBidLog_Click(object sender, RoutedEventArgs e)
        {
            ADbid.subWindows.WinBidLog winBidLog = new ADbid.subWindows.WinBidLog();
            winBidLog.ShowDialog();
        }

        //자동 입찰 눌렀을 때
        private void BtnAutoBid_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BidData item = (BidData)MainDataView.SelectedItem;
                Button btn = (Button)sender;
                if (item.AutoBidYN != null && item.AutoBidYN.Equals("ON"))
                {
                    item.AutoBidYN = "OFF";
                    // FileWrite(item.nccKeywordId, item.IsSelected); 1. 해당키워드의 on/off여부만 넘기는 경우
                    //var autoBidData = from p in LinkPython.listBidDataTmp select new { p.nccKeywordId, p.AutoBidYN };
                    //ToList();
                    FileRW.FileWrite();
                    //FileWrite("aaa",autoBidData);
                }
                else
                {
                    item.AutoBidYN = "ON";
                    FileRW.FileWrite();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
            view.Refresh();
        }

        //정렬기능 사용
        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            view.SortDescriptions.Clear();
            if (sender == BtnSortNo)
            {
                if (BlSortNoAsc == false)
                {
                    view.SortDescriptions.Add(new SortDescription("No", ListSortDirection.Ascending));
                    BlSortNoAsc = true;
                }
                else
                {
                    view.SortDescriptions.Add(new SortDescription("No", ListSortDirection.Descending));
                    BlSortNoAsc = false;
                }
            }
            else if (sender == BtnSortADGroup)
            {
                if (BlSortADGroupAsc == false)
                {
                    view.SortDescriptions.Add(new SortDescription("CmpAndGrpName", ListSortDirection.Ascending));
                    BlSortADGroupAsc = true;
                }
                else
                {
                    view.SortDescriptions.Add(new SortDescription("CmpAndGrpName", ListSortDirection.Descending));
                    BlSortADGroupAsc = false;
                }
            }
            else if (sender == BtnSortKeyword)
            {
                if (BlSortKeywordAsc == false)
                {
                    view.SortDescriptions.Add(new SortDescription("Keyword", ListSortDirection.Ascending));
                    BlSortKeywordAsc = true;
                }
                else
                {
                    view.SortDescriptions.Add(new SortDescription("Keyword", ListSortDirection.Descending));
                    BlSortKeywordAsc = false;
                }
            }
            else if (sender == BtnSortAverRank)
            {
                if (BlSortAverRankAsc == false)
                {
                    view.SortDescriptions.Add(new SortDescription("AverRank", ListSortDirection.Ascending));
                    BlSortAverRankAsc = true;
                }
                else
                {
                    view.SortDescriptions.Add(new SortDescription("AverRank", ListSortDirection.Descending));
                    BlSortAverRankAsc = false;
                }
            }
            else if (sender == BtnSortAutoBidYN)
            {
                if (BlSortAutoBidYNAsc == false)
                {
                    view.SortDescriptions.Add(new SortDescription("AutoBidYN", ListSortDirection.Ascending));
                    BlSortAutoBidYNAsc = true;
                }
                else
                {
                    view.SortDescriptions.Add(new SortDescription("AutoBidYN", ListSortDirection.Descending));
                    BlSortAutoBidYNAsc = false;
                }
            }
            else if (sender == BtnSortIsPCYN)
            {
                if (BlSortIsPCYNAsc == false)
                {
                    view.SortDescriptions.Add(new SortDescription("IsPCYN", ListSortDirection.Ascending));
                    BlSortIsPCYNAsc = true;
                }
                else
                {
                    view.SortDescriptions.Add(new SortDescription("IsPCYN", ListSortDirection.Descending));
                    BlSortIsPCYNAsc = false;
                }
            }
            else if (sender == BtnSortTargetRank)
            {
                if (BlSortTargetRankAsc == false)
                {
                    view.SortDescriptions.Add(new SortDescription("TargetPCRank", ListSortDirection.Ascending));
                    BlSortTargetRankAsc = true;
                }
                else
                {
                    view.SortDescriptions.Add(new SortDescription("TargetPCRank", ListSortDirection.Descending));
                    BlSortTargetRankAsc = false;
                }
            }
            else if (sender == BtnSortMaxBid)
            {
                if (BlSortMaxBidAsc == false)
                {
                    view.SortDescriptions.Add(new SortDescription("MaxBid", ListSortDirection.Ascending));
                    BlSortMaxBidAsc = true;
                }
                else
                {
                    view.SortDescriptions.Add(new SortDescription("MaxBid", ListSortDirection.Descending));
                    BlSortMaxBidAsc = false;
                }
            }
            else if (sender == BtnSortExpoCnt)
            {
                if (BlSortExpoCntAsc == false)
                {
                    view.SortDescriptions.Add(new SortDescription("ExpoCnt", ListSortDirection.Ascending));
                    BlSortExpoCntAsc = true;
                }
                else
                {
                    view.SortDescriptions.Add(new SortDescription("ExpoCnt", ListSortDirection.Descending));
                    BlSortExpoCntAsc = false;
                }
            }
            else if (sender == BtnSortClickCnt)
            {
                if (BlSortClickCntAsc == false)
                {
                    view.SortDescriptions.Add(new SortDescription("ClickCnt", ListSortDirection.Ascending));
                    BlSortClickCntAsc = true;
                }
                else
                {
                    view.SortDescriptions.Add(new SortDescription("ClickCnt", ListSortDirection.Descending));
                    BlSortClickCntAsc = false;
                }
            }
            view.Refresh();
        }

        //키워드 검색에서 텍스트 변화가 있을때 비어 있으면 실행
        private void TbxKeyWord_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(TbxKeyWord.Text))
            {
                LblSearchKeyword.Visibility = Visibility.Hidden;
                return;
            }
            else
            {
                view.Filter = null; //텍스트 없으면 필터링 해제
            }
        }
        #endregion

        //콤보박스 초기화 및 Editable False로 변경(ComboBox 문구관련)
        private void CboSelected_DropDownOpened(object sender, EventArgs e)
        {
            CboSelected.IsEditable = false;
            var cboGroup = LinkPython.listBidDataTmp.
            OrderBy(profile => profile.GrpName).
            Select(p => new
            {
                CmpAndGrpName = p.CmpAndGrpName,
                nccAdgroupId = p.nccAdgroupId
                //사실 Group ID 만 넘기면 문제 없음
            }).Distinct().ToList();
            CboSelected.ItemsSource = cboGroup;
        }

        //ComboBox에서 그룹이 정해졌을때 nccGroupID를 정함
        private void CboSelected_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LinkPython.listBidDataTmp.Select(item => { item.IsSelected = false; return item; }).ToList();
            if (CboSelected.SelectedValue != null)
            {
                selectedNccGroupID = CboSelected.SelectedValue.ToString();
                ChkBatch.IsChecked = false;
            }
            view.Refresh();
        }

        //윈도우 로드되었을때
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.ContextMenu menu = new System.Windows.Forms.ContextMenu();
                System.Windows.Forms.MenuItem item1 = new System.Windows.Forms.MenuItem();
                System.Windows.Forms.MenuItem item2 = new System.Windows.Forms.MenuItem();
                menu.MenuItems.Add(item1);
                menu.MenuItems.Add(item2);
                item1.Index = 0;
                item1.Text = "프로그램 종료";
                item1.Click += delegate (object click, EventArgs eClick)
                {
                    if (!Regedit.Reg_Read("AutoBid").Equals("true"))
                    {
                        Regedit.Reg_Write("AuthState", "false");
                    }
                    RestAPI.LogOut(Regedit.Reg_Read("UserID"));
                    System.Windows.Application.Current.Shutdown();
                };
                item2.Index = 0;
                item2.Text = "프로그램 열기";
                item2.Click += delegate (object click, EventArgs eClick)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
                notify.Visible = true;
                notify.DoubleClick += delegate (object senders, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
                notify.Icon = new System.Drawing.Icon(home + @"\ADBID_128.ico");
                notify.Visible = true;
                notify.DoubleClick += delegate (object senders, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
                notify.ContextMenu = menu;
                notify.Text = "ADBid";

                if (Regedit.Reg_Read("AutoBid").Equals("true"))
                {
                    var updateTask = Task<bool>.Factory.StartNew(AuthLicense);
                    await updateTask;
                    bool result = updateTask.Result;
                    if (result)
                    {
                        view.Refresh();
                        Regedit.Reg_Write("AuthState", "true");
                    }
                }
            }
            catch
            {

            }
        }

        private bool AuthLicense()
        {
            return LinkPython.getData(Regedit.Reg_Read("AccLicense"), Regedit.Reg_Read("SecKey"), Regedit.Reg_Read("CustomerID"));
        }

        //Activated되었을때 View refresh -- 부하가 걸릴수도 있음
        private void Window_Activated(object sender, EventArgs e)
        {
            view.Refresh();
        }

        private void BtnAutoBidState_Click(object sender, RoutedEventArgs e)
        {
            if (isProgramOn == false)
            {
                if (LinkPython.listBidDataTmp.Count.Equals(0))
                {
                    WinMessage popUp = new WinMessage("환경설정에서 인증 후 입찰 부탁드립니다.");
                    popUp.Left = SystemParameters.WorkArea.Width / 2 - popUp.Width / 2;
                    popUp.Top = SystemParameters.WorkArea.Height / 2 - popUp.Height / 2;
                }
                else
                {
                    isProgramOn = true;
                    //BtnAutoBidState.Content = "자동 입찰 중지";
                    //BtnAutoBidState.Foreground = Brushes.Red;
                    BtnAutoBidState.Style = Application.Current.Resources["StyleBtnStopAutoBid"] as Style;
                    PnlAutoBidState.Visibility = Visibility.Visible;
                    //MainDataView.RowBackground = new SolidColorBrush(Color.FromRgb(byte.Parse("248"), byte.Parse("248"), byte.Parse("248")));
                    MainDataView.RowStyle = (Style)Application.Current.Resources["StyDisableDataGridRow"];
                    LblAutoBidState.Content = "(전체 키워드 " + LinkPython.listBidDataTmp.Count + "개 중 " +
                                                LinkPython.listBidDataTmp.Where(item => item.AutoBidYN.Equals("ON")).ToArray().Length + "개 자동 입찰 중)";
                    popUpToastMsg("자동입찰을 시작하였습니다.");
                }
            }
            else if (isProgramOn == true)
            {
                isProgramOn = false;
                //BtnAutoBidState.Content = "자동 입찰 시작";
                //BtnAutoBidState.Foreground = Brushes.Blue;
                BtnAutoBidState.Style = Application.Current.Resources["StyleBtnStartAutoBid"] as Style;
                PnlAutoBidState.Visibility = Visibility.Hidden;
                //MainDataView.RowBackground = Brushes.White;
                MainDataView.RowStyle = (Style)Application.Current.Resources["StyDataGridRow"];
                LblAutoBidState.Content = "";
                popUpToastMsg("자동입찰을 중지하였습니다.");
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = sender as FrameworkElement;
            var myObject = selectedItem.DataContext as BidData;
            var selectedKeyWord = LinkPython.listBidDataTmp.Where(item => item.id.Equals(myObject.id));
            if (myObject.IsSelected)
            {
                selectedKeyWord.Select(item =>
                {
                    item.IsSelected = false; return item;
                }).ToList();
            }
            else
            {
                selectedKeyWord.Select(item =>
                {
                    item.IsSelected = true; return item;
                }).ToList();
            }
            CboSelected.SelectedIndex = -1;
            CboSelected.SelectedValue = "--Select Campaign / Group--";
            view.Refresh();
        }

        private void ChkBatch_Click(object sender, RoutedEventArgs e)
        {
            if (batchChecked)
            {
                //일괄 체크박스가 뗴어졌을 경우
                batchChecked = false;
                LinkPython.listBidDataTmp.Select(item => { item.IsSelected = false; return item; }).ToList();
                view.Refresh();
            }
            else
            {
                //일괄 체크박스가 눌렸을 경우
                batchChecked = true;
                LinkPython.listBidDataTmp.Select(item => { item.IsSelected = true; return item; }).ToList();
                var autoBidData = from p in LinkPython.listBidDataTmp
                                  select new { p.id, p.AutoBidYN };
                CboSelected.SelectedIndex = -1;
                CboSelected.SelectedValue = "--Select Campaign / Group--";
                view.Refresh();
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.LWin || e.Key == Key.RWin)
            //{
            //    e.Handled = true;
            //    //return;
            //}
            if (Keyboard.IsKeyDown(Key.Down) && (Keyboard.IsKeyDown(Key.LWin) || Keyboard.IsKeyDown(Key.RWin)))
            {
                e.Handled = true;
            }
        }

        //정규식 표현으로 숫자만 들어가게
        private void TextBlock_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
