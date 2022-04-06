using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ADbid.subWindows;
using System.Data;

namespace ADbid
{
    class FileRW
    {
        ADbid.Regedit Regedit = new ADbid.Regedit();

        public void FileRead()
        {
            try
            {
                int rowNum = 1;
                string[] textLines = System.IO.File.ReadAllLines(@"C:\Git\ADBid\NAD\AutoBidData.txt");  //경로지정, 파일이름 필요, 메모리에 context 올리고 파일 자동으로 닫힘
                foreach (string testLine in textLines)
                {
                    if (rowNum == 1)
                    {
                        if (!Regedit.Reg_Read("CustomerID").Equals(testLine))
                        {
                            WinMessage popUp = new WinMessage("Customer ID를 확인해주세요");
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
                using (StreamWriter outputFile = new StreamWriter(@"C:\Git\ADBid\NAD\AutoBidData.txt"))
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
                using (StreamWriter outputFile = new StreamWriter(@"C:\Git\ADBid\NAD\bidLogData.txt"))
                {
                    if (!Regedit.Reg_Read("CustomerID").Equals(""))
                    {
                        outputFile.WriteLine(Regedit.Reg_Read("CustomerID"));
                    }//CustomerID가 없을 경우 예외처리 하긴해야함

                    foreach (DataRow bidLog in bidLogData.Rows)
                    {
                        outputFile.WriteLine(bidLog["Log"].ToString());
                    }
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
                string[] textLines = System.IO.File.ReadAllLines(@"C:\Git\ADBid\NAD\bidLogData.txt");  //경로지정, 파일이름 필요, 메모리에 context 올리고 파일 자동으로 닫힘
                foreach (string testLine in textLines)
                {
                    if (rowNum == 1)
                    {
                        if (!Regedit.Reg_Read("CustomerID").Equals(testLine))
                        {
                            WinMessage popUp = new WinMessage("Customer ID를 확인해주세요");
                            break;
                        }
                    }
                    else
                    {
                        string[] arrBidLogData = testLine.Split('|');
                        BidLogData LogData = new BidLogData();
                        LogData.CmpAndGrpName = arrBidLogData[0];
                        LogData.Keyword = arrBidLogData[1];
                        LogData.AverRank = arrBidLogData[2];
                        if (arrBidLogData[3].Equals("True")){
                            LogData.IsPCYN = true;
                            LogData.BidMedium = "PC";
                            LogData.TargetRank = arrBidLogData[5];
                        }
                        if (arrBidLogData[4].Equals("True")){
                            LogData.IsMobileYN = true;

                            if (LogData.BidMedium != null)
                                LogData.BidMedium += "/Mobile";
                            else
                                LogData.BidMedium = "Mobile";

                            if (LogData.TargetRank != null)
                                LogData.TargetRank += "/"+arrBidLogData[6];
                            else
                                LogData.TargetRank = arrBidLogData[6];
                        }
                        LogData.TargetPCRank = Int32.Parse(arrBidLogData[5]);
                        LogData.TargetMobileRank = Int32.Parse(arrBidLogData[6]);
                        LogData.MaxBid = arrBidLogData[7];
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
    }
}