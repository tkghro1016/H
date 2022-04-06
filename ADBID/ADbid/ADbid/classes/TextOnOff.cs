using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ADbid.subWindows;
using System.Data;
using System.Windows;

namespace ADbid
{
    class FileRW
    {
        ADbid.Regedit Regedit = new ADbid.Regedit();
        private static String home = Directory.GetCurrentDirectory();

        //AutoBidData => 사용자가 저장한 자동입찰 On/Off기능 기억했다가 불러오는 기능
        public void FileRead()
        {
            try
            {
                int rowNum = 1;
                
                string[] textLines = System.IO.File.ReadAllLines(home + @"\AutoBidData.txt");  //경로지정, 파일이름 필요, 메모리에 context 올리고 파일 자동으로 닫힘
                //string[] textLines = System.IO.File.ReadAllLines(@"C:\Users\E1-10990K-4\workspace\NAD\AutoBidData.txt");  //경로지정, 파일이름 필요, 메모리에 context 올리고 파일 자동으로 닫힘
                foreach (string testLine in textLines)
                {
                    if (rowNum == 1)
                    {
                        if (!Regedit.Reg_Read("CustomerID").Equals(testLine))
                        {
                            WinMessage popUp = new WinMessage("Customer ID를 확인해주세요");
                            popUp.Left = SystemParameters.WorkArea.Width / 2 - popUp.Width / 2;
                            popUp.Top = SystemParameters.WorkArea.Height / 2 - popUp.Height / 2;
                            break;
                        }
                    }
                    else
                    {
                        string[] arrAutoBidYN = testLine.Split('|');
                        var existKeywordAlready = LinkPython.listBidDataTmp.Where(item => item.id == arrAutoBidYN[0]);

                        if (existKeywordAlready.Count()>0)
                        {
                            existKeywordAlready.Select(item =>
                            {
                                item.AutoBidYN = arrAutoBidYN[1];
                                return item;
                            }).ToList();
                        }
                    }
                    rowNum += 1;
                }
            }
            catch
            {
            }
        }

        public static int Find(String[] arr, String str)
        {
            string[] tmp;
            string tmp1;
            for (int i = 0; i < arr.Length; i++)
            {
                tmp = arr[i].Split(' ');
                tmp1 = tmp[0];
                if (tmp1.Equals(str)) return i;
            }
            return -1;
        }


        // AD_ID On/Off, sep=" "으로; 키는 AD_ID만 
        public void FileWrite()
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(home);
                if(di.Exists == false)
                {
                    di.Create();
                }
                using (StreamWriter outputFile = new StreamWriter(home + @"\AutoBidData.txt"))
                //using (StreamWriter outputFile = new StreamWriter(@"C:\Users\E1-10990K-4\workspace\NAD\AutoBidData.txt"))
                {
                    if (!Regedit.Reg_Read("CustomerID").Equals(""))
                    {
                        outputFile.WriteLine(Regedit.Reg_Read("CustomerID"));
                    }//CustomerID가 없을 경우 예외처리 하긴해야함

                    foreach (BidData items in LinkPython.listBidDataTmp)
                    {
                        outputFile.WriteLine(items.id+"|"+items.AutoBidYN);
                    }
                }
            }
            catch
            {
                //dir 접근 안되는 등 에러 발생시 
            }
        }

        public void LogWrite(DataTable bidLogData)
        {
            try
            {
                //디렉토리 없으면 디렉토리 생성
                DirectoryInfo di = new DirectoryInfo(home);
                if (di.Exists == false)
                {
                    di.Create();
                }

                //FileInfo.Exists로 파일 존재유무 확인 "
                FileInfo fi = new FileInfo(home + @"\bidLogData.txt");
                if (fi.Exists)
                {
                    //있으면 파일에 저장된 row수 센 뒤에 저장
                    var lineCount = File.ReadLines(home + @"\bidLogData.txt").Count();
                    if (lineCount > 0)
                    {
                        string[] textLines = File.ReadAllLines(home + @"\bidLogData.txt");  //경로지정, 파일이름 필요, 메모리에 context 올리고 파일 자동으로 닫힘
                                                                                                                           //string[] textLines = System.IO.File.ReadAllLines(@"C:\Users\E1-10990K-4\workspace\NAD\bidLogData.txt");  //경로지정, 파일이름 필요, 메모리에 context 올리고 파일 자동으로 닫힘

                         //1열 CustomerID가 다르면 다른 사용자가 해당 컴퓨터에서 사용을 시작한 것이므로 엎어친다.
                        if (!Regedit.Reg_Read("CustomerID").Equals(textLines[0]))
                        {
                            MakeIntLogWrite(bidLogData);
                        }
                        else
                        {
                            //같은 사용자고, 1000줄이 안넘는지 신규로그데이터, 기존 로그데이터로 합산계산
                            int tmpLineCnt = lineCount + bidLogData.Rows.Count;
                            //합이 1000이 안넘는 경우 뒤에 이어 쓰기
                            if (tmpLineCnt < 1002)//Customer ID가 1줄이므로 1002
                            {
                                using (StreamWriter outputFile = new StreamWriter(home + @"\bidLogData.txt", true))
                                {
                                    foreach (DataRow bidLog in bidLogData.Rows)
                                    {
                                        outputFile.WriteLine(bidLog["Log"].ToString());
                                    }
                                }
                            }
                            else if (bidLogData.Rows.Count>1000)//신 데이터가 1000개가 넘어버리면
                            {
                                using (StreamWriter outputFile = new StreamWriter(home + @"\bidLogData.txt"))
                                {
                                    //Customer 1열
                                    outputFile.WriteLine(Regedit.Reg_Read("CustomerID"));

                                    for (int i = 0; i < 1000; i++)
                                    {
                                        outputFile.WriteLine(bidLogData.Rows[i]["Log"].ToString());
                                    }
                                }
                            }
                            else
                            {//합이 1000이 넘는 경우 뒤에서 신규+ 오래된 것 중 최신의 것을 합쳐서 다시 씀.
                                using (StreamWriter outputFile = new StreamWriter(home + @"\bidLogData.txt"))
                                {
                                    //Customer 1열
                                    outputFile.WriteLine(Regedit.Reg_Read("CustomerID"));
                                    for(int lineNum=0; lineNum < lineCount; lineNum++) 
                                    {
                                        if (lineNum > bidLogData.Rows.Count+lineCount-1000 -1)
                                        {
                                            outputFile.WriteLine(textLines[lineNum].ToString());
                                        }
                                    }
                                    foreach (DataRow bidLog in bidLogData.Rows)
                                    {
                                        outputFile.WriteLine(bidLog["Log"].ToString());
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        MakeIntLogWrite(bidLogData);
                    }
                }
                else
                {
                    MakeIntLogWrite(bidLogData);
                }
            }
            catch
            {
                //dir 접근 안되는 등 에러 발생시 
            }
        }

        public List<BidLogData> LogRead()
        {
            List<BidLogData> LogBidData = new List<BidLogData>();
            try
            {
                int rowNum = 1;
                string[] textLines = System.IO.File.ReadAllLines(home + @"\bidLogData.txt");  //경로지정, 파일이름 필요, 메모리에 context 올리고 파일 자동으로 닫힘
                //string[] textLines = System.IO.File.ReadAllLines(@"C:\Users\E1-10990K-4\workspace\NAD\bidLogData.txt");  //경로지정, 파일이름 필요, 메모리에 context 올리고 파일 자동으로 닫힘
                foreach (string testLine in textLines)
                {
                    if (rowNum == 1)
                    {
                        if (!Regedit.Reg_Read("CustomerID").Equals(testLine))
                        {
                            WinMessage popUp = new WinMessage("접속하신 ID로 기록된 로그가 없습니다.");
                            popUp.Left = SystemParameters.WorkArea.Width / 2 - popUp.Width / 2;
                            popUp.Top = SystemParameters.WorkArea.Height / 2 - popUp.Height / 2;
                            break;
                        }
                    }
                    else
                    {
                        string[] arrBidLogData = testLine.Split('|');
                        BidLogData LogData = new BidLogData();
                        LogData.CmpAndGrpName = arrBidLogData[0];
                        string[] arrKeywordStatus = arrBidLogData[1].Split('/');
                        LogData.Keyword = arrKeywordStatus[0].Trim();
                        LogData.Status = arrKeywordStatus[1].Trim();
                        LogData.AverRank = arrBidLogData[2].Split('/')[0].Trim()+"위/"+ arrBidLogData[2].Split('/')[1].Trim()+"위";
                        if (arrBidLogData[3].Equals("True")){
                            LogData.IsPCYN = true;
                            LogData.BidMedium = "PC";
                            LogData.TargetRank = arrBidLogData[5] + "위";
                        }
                        if (arrBidLogData[4].Equals("True")){
                            LogData.IsMobileYN = true;

                            if (LogData.BidMedium != null)
                                LogData.BidMedium += "/Mobile";
                            else
                                LogData.BidMedium = "Mobile";

                            if (LogData.TargetRank != null)
                                LogData.TargetRank += "/"+arrBidLogData[6] + "위";
                            else
                                LogData.TargetRank = arrBidLogData[6];
                        }
                        LogData.TargetPCRank = Int32.Parse(arrBidLogData[5]);
                        LogData.TargetMobileRank = Int32.Parse(arrBidLogData[6]);
                        if (!arrBidLogData[7].Equals(""))
                        {
                            LogData.MaxBid = string.Format("{0:#,0}", int.Parse(arrBidLogData[7].Trim()));
                        }
                        else
                        {
                            LogData.MaxBid = "0";
                        }
                        LogData.BidTime = arrBidLogData[8];
                        LogBidData.Add(LogData);
                    }
                    rowNum += 1;
                }
            }
            catch
            {
            }
            return LogBidData;
        }

        private void MakeIntLogWrite(DataTable bidLogData)
        {
            //없으면 파일 생성
            using (StreamWriter outputFile = new StreamWriter(home + @"\bidLogData.txt"))
            {
                if (!Regedit.Reg_Read("CustomerID").Equals(""))
                {
                    outputFile.WriteLine(Regedit.Reg_Read("CustomerID"));
                }
                foreach (DataRow bidLog in bidLogData.Rows)
                {
                    outputFile.WriteLine(bidLog["Log"].ToString());
                }
            }
        }
    }
}