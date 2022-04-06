using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using Excel = Microsoft.Office.Interop.Excel;
//using Microsoft.Office.Interop.Excel;

namespace ADbid.subWindows
{
    /// <summary>
    /// WinBidLog.xaml에 대한 상호 작용 논리
    /// </summary>


    public partial class WinBidLog : Window
    {
        CollectionView view;
        ADbid.FileRW FileRW = new FileRW();
        List<BidLogData> BidLogData;

        //각 정렬기준에 대한 상태값 저장.
        private bool BlSortADGroupAsc = false;
        private bool BlSortKeywordAsc = false;
        private bool BlSortAverRankAsc = false;
        private bool BlSortIsPCYNAsc = false;
        private bool BlSortTargetRankAsc = false;
        private bool BlSortMaxBidAsc = false;
        private bool BlSortBidDate = false;
        private static String home = Directory.GetCurrentDirectory();

        public WinBidLog()
        {
            InitializeComponent();
            this.Left = SystemParameters.WorkArea.Width / 2 - this.Width / 2;
            this.Top = SystemParameters.WorkArea.Height / 2 - this.Height / 2;
            BidLogData = FileRW.LogRead();
            MainDataLogView.ItemsSource = BidLogData;
            view = (CollectionView)CollectionViewSource.GetDefaultView(MainDataLogView.ItemsSource);

        }

        #region 메소드
        #endregion

        #region 이벤트
        //닫기 버튼 눌렀을때
        private void BtnMainClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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


        #endregion
        //엑셀로 추출 버튼 눌렀을 시
        private void btnExcel_Click(object sender, RoutedEventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(home);
            if (di.Exists == false)
            {
                di.Create();
            }
            string path = di + "\\ADbidLog"+ DateTime.Now.ToString("yyyyMMdd") + ".xlsx";

            try
            {
                var excelExport = new Excel.Application();
                excelExport.Workbooks.Add();
                Excel._Worksheet Worksheet = (Excel._Worksheet)excelExport.ActiveSheet;

                Excel.Range Rg = excelExport.Range[excelExport.Cells[3,2], excelExport.Cells[BidLogData.Count+3, 8]];
                Rg.RowHeight = 30;
                Rg.Columns.AutoFit();
                Rg.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                excelExport.Columns[1].ColumnWidth = 3;
                excelExport.Columns[2].ColumnWidth = 25;
                excelExport.Columns[3].ColumnWidth = 25;
                excelExport.Columns[4].ColumnWidth = 17;
                excelExport.Columns[5].ColumnWidth = 12;
                excelExport.Columns[6].ColumnWidth = 10;
                excelExport.Columns[7].ColumnWidth = 15;
                excelExport.Columns[8].ColumnWidth = 25;

                excelExport.Cells[3, 2] = "캠패인 광고그룹";
                excelExport.Cells[3, 3] = "키워드 or 소재";
                excelExport.Cells[3, 4] = "PC/모바일 평균순위";
                excelExport.Cells[3, 5] = "입찰기준";
                excelExport.Cells[3, 6] = "목표순위";
                excelExport.Cells[3, 7] = "최대입찰가";
                excelExport.Cells[3, 8] = "입찰일";

                for (int rowIndex = 4; rowIndex < BidLogData.Count+4; rowIndex++)
                {
                    excelExport.Cells[rowIndex, 2] = BidLogData[rowIndex - 4].CmpAndGrpName;
                    excelExport.Cells[rowIndex, 3] = BidLogData[rowIndex - 4].Keyword + BidLogData[rowIndex - 4].Status;
                    excelExport.Cells[rowIndex, 4] = BidLogData[rowIndex - 4].AverRank;
                    excelExport.Cells[rowIndex, 5] = BidLogData[rowIndex - 4].BidMedium;
                    excelExport.Cells[rowIndex, 6] = BidLogData[rowIndex - 4].TargetRank;
                    excelExport.Cells[rowIndex, 7] = BidLogData[rowIndex - 4].MaxBid;
                    excelExport.Cells[rowIndex, 8] = BidLogData[rowIndex - 4].BidTime;
                }
                excelExport.Visible = false;


                FileInfo fi = new FileInfo(path);
                //FileInfo.Exists로 파일 존재유무 확인 "
                if (fi.Exists) {
                    fi.Delete();
                    Worksheet.SaveAs(path);
                }
                else {
                    Worksheet.SaveAs(path);
                }
                WinMessage popUp = new WinMessage("다운로드가 완료되었습니다.\n"+path);
                popUp.Left = SystemParameters.WorkArea.Width / 2 - popUp.Width / 2;
                popUp.Top = SystemParameters.WorkArea.Height / 2 - popUp.Height / 2;
                excelExport.Quit();
            }
            catch
            {

            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            view.Refresh();
        }

        //정렬기능 사용
        private void SortButton_Click(object sender, RoutedEventArgs e)
        {
            view.SortDescriptions.Clear();
            if (sender == BtnSortADGroup)
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
            else if (sender == BtnSortBidDate)
            {
                if (BlSortBidDate == false)
                {
                    view.SortDescriptions.Add(new SortDescription("BidTime", ListSortDirection.Ascending));
                    BlSortBidDate = true;
                }
                else
                {
                    view.SortDescriptions.Add(new SortDescription("BidTime", ListSortDirection.Descending));
                    BlSortBidDate = false;
                }
            }
            view.Refresh();
        }
    }
}