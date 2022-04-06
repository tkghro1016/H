using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// WinBatchSelect.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WinBatchSelect : Window
    {
        static bool isGroup = false;
        static string groupID = "";
        //체크박스로 선택된 경우 타게 된다.
        public WinBatchSelect()
        {
            InitializeComponent();
            //창을 처음 띄울때 초기화 위치 잡음
            this.Left = SystemParameters.WorkArea.Width / 2 - this.Width / 2;
            this.Top = SystemParameters.WorkArea.Height / 2 - this.Height / 2;
            LblSelectedKeyword.Content = "선택된 키워드";
            var selectedKewords = LinkPython.listBidDataTmp.Where(item => item.IsSelected == true);
            LblParentCnt.Content = selectedKewords.Count() + "개";
        }

        //캠페인 그룹 선택했을때 타게된다.
        public WinBatchSelect(string selectedNccGroupID)
        {
            InitializeComponent();
            //창을 처음 띄울때 초기화 위치 잡음
            this.Left = SystemParameters.WorkArea.Width / 2 - this.Width / 2;
            this.Top = SystemParameters.WorkArea.Height / 2 - this.Height / 2;
            isGroup = true;
            groupID = selectedNccGroupID;
            LblSelectedKeyword.Content = "선택된 키워드";
            var selectedGroup = LinkPython.listBidDataTmp.Where(item => item.nccAdgroupId.Equals(selectedNccGroupID));
            String StrCmpGroupName = selectedGroup.Select(p => new
            {
                CmpAndGrpName = p.CmpAndGrpName
            }).Distinct().ToString();
            LblParentCnt.Content = selectedGroup.Count() + "개";
        }

        #region 메소드
        #endregion

        #region 이벤트
        private void BtnClose_Click(object sender, RoutedEventArgs e)

        {
            this.Close();
        }
        #endregion

        private void TextBlock_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Textbox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.ImeProcessed)
            {
                e.Handled = true;
            }
        }

        private void BtnAutoBidYN_Click(object sender, RoutedEventArgs e)
        {
            if (BtnAutoBidYN.Content.Equals("ON"))
            {
                BtnAutoBidYN.Content = "OFF";
                AutoBidYNToggleImg.Source = new BitmapImage(new Uri(@"D:\Programming\adbid_winapp-master\adbid_winapp-master\ADbid\ADbid\Resources\images\batchSelect\off_radio.png"));
            }
            else if(BtnAutoBidYN.Content.Equals("OFF"))
            {
                BtnAutoBidYN.Content = "ON";
                AutoBidYNToggleImg.Source = new BitmapImage(new Uri(@"D:\Programming\adbid_winapp-master\adbid_winapp-master\ADbid\ADbid\Resources\images\batchSelect\on_radio.png"));
            }
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!TxtTargetRank.Text.Equals(""))
            {
                if (RadPC.IsChecked == true)
                {
                    if (int.Parse(TxtTargetRank.Text) > 15 || int.Parse(TxtTargetRank.Text) < 1)
                    {
                        WinMessage popUp = new WinMessage("PC순위는 1위에서 15위까지만 가능합니다.");
                        popUp.Left = SystemParameters.WorkArea.Width / 2;
                        popUp.Top = SystemParameters.WorkArea.Height / 2;
                        return;
                    }
                }
                else
                {
                    if (int.Parse(TxtTargetRank.Text) > 5 || int.Parse(TxtTargetRank.Text) < 1)
                    {
                        WinMessage popUp = new WinMessage("모바일 순위는 1위에서 5위까지만 가능합니다.");
                        popUp.Left = SystemParameters.WorkArea.Width / 2;
                        popUp.Top = SystemParameters.WorkArea.Height / 2;
                        return;
                    }
                }
            }

            if (isGroup)
            {
                var selectedGroup = LinkPython.listBidDataTmp.Where(item => item.nccAdgroupId.Equals(groupID));
                selectedGroup.Select(item => {
                    item.AutoBidYN = BtnAutoBidYN.Content.ToString();
                    if (!TxtTargetRank.Text.Equals(""))
                    {
                        if (RadPC.IsChecked == true)
                            item.TargetPCRank = int.Parse(TxtTargetRank.Text);
                        else
                            item.TargetMobileRank = int.Parse(TxtTargetRank.Text);
                    }

                    if (!TxtMaxBid.Text.Equals(""))
                        item.MaxBid = TxtMaxBid.Text;

                    if (!TxtAddBid.Text.Equals(""))
                        item.AddMoney = TxtAddBid.Text;

                    return item;
                }).ToList();
            }
            else
            {
                var selectedGroup = LinkPython.listBidDataTmp.Where(item => item.IsSelected.Equals(true));
                selectedGroup.Select(item => {
                    item.AutoBidYN = BtnAutoBidYN.Content.ToString();
                    if (!TxtTargetRank.Text.Equals(""))
                    {
                        if (RadPC.IsChecked == true)
                            item.TargetPCRank = int.Parse(TxtTargetRank.Text);
                        else
                            item.TargetMobileRank = int.Parse(TxtTargetRank.Text);
                    }

                    if (!TxtMaxBid.Text.Equals(""))
                        item.MaxBid = TxtMaxBid.Text;

                    if (!TxtAddBid.Text.Equals(""))
                        item.AddMoney = TxtAddBid.Text;
                    return item;
                }).ToList();
            }
            this.Close();
        }

        private void RadPC_Click(object sender, RoutedEventArgs e)
        {
            LblRank.Content = "15";
        }

        private void RadMobile_Click(object sender, RoutedEventArgs e)
        {
            LblRank.Content = "5";
        }
    }
}
