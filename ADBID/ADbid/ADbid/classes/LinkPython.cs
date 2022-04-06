using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using ADbid.subWindows;
using Newtonsoft.Json.Linq;

namespace ADbid
{

    public class LinkPython
    {
        static public List<BidData> listBidDataTmp = new List<BidData>();
        private static String home = Directory.GetCurrentDirectory();

        public LinkPython()
        {

        }

        public static bool getData(string API_KEY, string SECRET_KEY, string CUSTOMER_ID)
        {
            var psi = new ProcessStartInfo();

            psi.FileName = home + @"\PyProcessWorker.exe";
            //psi.FileName = @"C:\Users\E1-10990K-4\AppData\Local\Programs\Python\Python39\python.exe";
            //psi.FileName = @"C:\Users\E1-10990K-4\AppData\Local\Microsoft\WindowsApps\python.exe";

            var BASE_URL = "https://api.naver.com";
            var kwdData = "";
            //psi.Arguments = $"\"{script}\" \"{BASE_URL}\" \"{API_KEY}\" \"{SECRET_KEY}\" \"{CUSTOMER_ID}\" \"{kwdData}\"";
            psi.Arguments = $"\"{BASE_URL}\" \"{API_KEY}\" \"{SECRET_KEY}\" \"{CUSTOMER_ID}\" \"{kwdData}\"";

            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            var result = "";
            var error = "";
            try
            {
                Console.WriteLine("Get Start!!");

                using (var process = Process.Start(psi))
                {
                    result = process.StandardOutput.ReadToEnd();
                    error = process.StandardError.ReadToEnd();
                }
                Console.WriteLine(result);
                Console.WriteLine(error);
                parseData(result);
            }
            catch
            {
                MessageBox.Show("네이버AD의 데이터를 가져오는데 실패하였습니다.");
                return false;
            }
            return true;
        }


        private static void parseData(String strRawData)
        {
            //데이터 파싱하는 부분 
            string[] strArritems = strRawData.Split(new string[] { "^.^" }, StringSplitOptions.RemoveEmptyEntries);
            string[][] strAttribute = new string[strArritems.Length-1][];
            Dictionary<string, string>[] parsingData = new Dictionary<string, string>[strArritems.Length-1];

            // "/./"로 보내는 부분 파싱(속성별 파싱)
            for (int i=0; i<strArritems.Length-1; i++)
            {
                strAttribute[i] = strArritems[i].Split(new string[] { "/./" }, StringSplitOptions.RemoveEmptyEntries);
                parsingData[i] = new Dictionary<string, string>();
            }

            // "="속성과 값 분리
            for (int j = 0; j < strArritems.Length-1; j++)
            {
                for (int k = 0; k < strAttribute[j].Length; k++)
                {
                    string[] a = strAttribute[j][k].Split('=');
                    parsingData[j].Add(a[0], a[1]);
                }
            }

            for(int k=0; k< parsingData.Length; k++)
            {   
                //파싱된 데이터에 대하여 기존에 존재하는지 존재하지 않는 데이터인지 확인(insert/update 여부 확인)
                var existKeywordAlready = listBidDataTmp.Where(item => item.id == parsingData[k]["id"]);

                //insert
                if (existKeywordAlready.Count() == 0) {
                    listBidDataTmp.Add(new BidData()
                    {
                        LstItemWidth = 1000,
                        IsSelected = false,
                        No = listBidDataTmp.Count() + 1,
                        nccAdgroupId = parsingData[k]["nccAdgroupId"],
                        nccCampaignId = parsingData[k]["nccCampaignId"],
                        id = parsingData[k]["id"],
                        CmpAndGrpName = parsingData[k]["grpName"] + "_" + parsingData[k]["cmpName"],
                        GrpName = parsingData[k]["grpName"],
                        CmpName = parsingData[k]["cmpName"],
                        Keyword = parsingData[k]["kwdName"],
                        AverRank = parsingData[k]["pcNxAvgRnk"] + "/" + parsingData[k]["mblNxAvgRnk"],
                        IsPCYN = bool.Parse(parsingData[k]["pc"]),
                        IsMobileYN = bool.Parse(parsingData[k]["mobile"]),
                        CurrentBid = string.Format("{0:#,0}", parsingData[k]["bidAmt"]),
                        ExpoCnt = int.Parse(parsingData[k]["impCnt"]),
                        ClickCnt = int.Parse(parsingData[k]["clkCnt"]),
                        AutoBidYN = "OFF",
                        CampaignTp = parsingData[k]["campaignTp"],
                        AddMoney = "",
                        Type = parsingData[k]["type"],
                        Status = parsingData[k]["status"].Equals("ELIGIBLE") ? "" : "중지"
                    });
                }
                //update
                else
                {
                    existKeywordAlready.Select(item => {
                        item.AverRank = parsingData[k]["pcNxAvgRnk"] + "/" + parsingData[k]["mblNxAvgRnk"];
                        //item.IsPCYN = bool.Parse(parsingData[k]["pc"]);
                        //item.IsMobileYN = bool.Parse(parsingData[k]["mobile"]);
                        item.CurrentBid = string.Format("{0:#,0}", parsingData[k]["bidAmt"]);
                        item.ExpoCnt = int.Parse(parsingData[k]["impCnt"]);
                        item.ClickCnt = int.Parse(parsingData[k]["clkCnt"]);
                        item.Status = parsingData[k]["status"].Equals("ELIGIBLE") ? "" : "중지";
                        return item; }).ToList();
                }

            }
        }

        public static bool updateData(string API_KEY, string SECRET_KEY, string CUSTOMER_ID)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            
            
            psi.FileName = home + @"\PyProcessWorker.exe";            
            //var script = @"C:\Users\E1-10990K-4\workspace\NAD\PyProcessWorker.py";
            var BASE_URL = "https://api.naver.com";
            Console.WriteLine(home);
            int updateItemsQ = 0;

            int updateItemsR = 0;

            BidData[] autoBidOnItems = listBidDataTmp.Where(item => item.AutoBidYN.Equals("ON")).ToArray();

            BidUpdateData[] autoBidOnItemsParsed = autoBidOnItems.Select(item => new BidUpdateData {
                                                nccAdgroupId= item.nccAdgroupId, id = item.id,
                                                        IsPCYN = item.IsPCYN,
                                                        IsMobileYN = item.IsMobileYN,
                                                        TargetPCRank =item.TargetPCRank,
                                                        TargetMobileRank = item.TargetMobileRank,
                                                        MaxBid = item.MaxBid,
                                                        CampaignTp =  item.CampaignTp,
                                                        AddMoney = item.AddMoney,
                                                        Type= item.Type }).ToArray();

            if (autoBidOnItemsParsed.Length > 0)
            {
                //100개씩 쪼개서 보내기 위해서 작업 진행 Q몫 R나머지
                updateItemsQ = autoBidOnItemsParsed.Length / 2;//갯수 수정필요

                updateItemsR = autoBidOnItemsParsed.Length % 2;//갯수 수정필요

                for (int i = 0;i< updateItemsQ+1; i++)
                {
                    BidUpdateData[] autoBidOnItemsSplit;
                    int splitSize = 0;
                    int copySize = 0;

                    Console.WriteLine(updateItemsQ+"개 중" + i + "번쨰 실행" + i +"/" + updateItemsQ);
                    if (!i.Equals(updateItemsQ))
                    {
                        splitSize = 2;//갯수 수정필요
                        copySize = 2;//갯수 수정필요
                    }
                    else
                    {
                        splitSize = autoBidOnItemsParsed.Length - 2 * updateItemsQ;//갯수 수정필요
                        copySize = updateItemsR;
                    }

                    autoBidOnItemsSplit = new BidUpdateData[splitSize];
                    for (int j = 0; j < copySize; j++)
                    {
                        autoBidOnItemsSplit[j] = autoBidOnItemsParsed[i * 2 + j];//갯수 수정필요
                    }

                    var kwdData = JArray.FromObject(autoBidOnItemsSplit);

                    //psi.Arguments = $"\"{script}\" \"{BASE_URL}\" \"{API_KEY}\" \"{SECRET_KEY}\" \"{CUSTOMER_ID}\" \"{kwdData}\"";
                    psi.Arguments = $"\"{BASE_URL}\" \"{API_KEY}\" \"{SECRET_KEY}\" \"{CUSTOMER_ID}\" \"{kwdData}\"";

                    psi.UseShellExecute = false;
                    psi.CreateNoWindow = true;
                    psi.RedirectStandardOutput = true;
                    psi.RedirectStandardError = true;

                    var result = "result";
                    var error = "";
                    try
                    {
                        Console.WriteLine("Update Start!!");
                        Console.WriteLine(kwdData);

                        using (var process = Process.Start(psi))
                        {
                            result = process.StandardOutput.ReadToEnd();
                            error = process.StandardError.ReadToEnd();
                            process.WaitForExit();
                            process.Close();
                        }
                        Console.WriteLine(result);
                        Console.WriteLine(error);
                        
                        if (result != null && !result.Equals(""))
                        {
                            parseData(result);
                            Console.WriteLine(updateItemsQ + "개 중" + i + "번쨰 실행 완료" + i + "/" + updateItemsQ);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("네이버AD에 데이터를 업데이트하는데 실패하였습니다.");
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
