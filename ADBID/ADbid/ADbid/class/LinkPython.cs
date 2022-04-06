using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace ADbid
{

    public class LinkPython
    {
        static public List<BidData> listBidDataTmp = new List<BidData>();

        public LinkPython()
        {

        }

        public LinkPython(string API_KEY, string SECRET_KEY, string CUSTOMER_ID, string Funtion)
        {

            //인증, GetData, Update를 나눌지 어떻게 할지 정해야함
            if (Funtion.Equals("Auth"))
            {
                //listBidDataTmp = getData(API_KEY, SECRET_KEY, CUSTOMER_ID);
            }else if (Funtion.Equals("GetData"))
            {
                //listBidDataTmp = getData(API_KEY, SECRET_KEY, CUSTOMER_ID);
            }
            else if (Funtion.Equals("Update"))
            {

            }
        }

        public static bool getData(string API_KEY, string SECRET_KEY, string CUSTOMER_ID)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = @"C:\Python34\python.exe";

            var script = @"D:\Programming\NAD\PyProcessWorker.py";
            var BASE_URL = "https://api.naver.com";
            var kwdData = "";
            psi.Arguments = $"\"{script}\" \"{BASE_URL}\" \"{API_KEY}\" \"{SECRET_KEY}\" \"{CUSTOMER_ID}\" \"{kwdData}\"";

            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            var result = "";
            var error = "";
            try
            {
                using (var process = Process.Start(psi))
                {
                    result = process.StandardOutput.ReadToEnd();
                    error = process.StandardError.ReadToEnd();
                }
                parseData(result);
            }
            catch
            {
                MessageBox.Show("API 데이터 Get Error발생");
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
                        CurrentBid = parsingData[k]["bidAmt"],
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
                        item.CurrentBid = parsingData[k]["bidAmt"];
                        item.ExpoCnt = int.Parse(parsingData[k]["impCnt"]);
                        item.ClickCnt = int.Parse(parsingData[k]["clkCnt"]);
                        item.Status = parsingData[k]["status"].Equals("ELIGIBLE") ? "" : "중지";
                        return item; }).ToList();
                }

            }
        }

        public static bool updateData(string API_KEY, string SECRET_KEY, string CUSTOMER_ID)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = @"C:\Python34\python.exe";

            var script = @"D:\Programming\NAD\PyProcessWorker.py";
            var BASE_URL = "https://api.naver.com";

            BidData[] autoBidOnItems = listBidDataTmp.Where(item => item.AutoBidYN.Equals("ON")).ToArray();

            BidUpdateData[] autoBidOnItemsParsed = autoBidOnItems.Select(item => new BidUpdateData {
                                                nccAdgroupId= item.nccAdgroupId, id = item.id,
                                                        IsPCYN = item.IsPCYN,
                                                        IsMobileYN = item.IsMobileYN,
                                                        TargetPCRank =item.TargetPCRank,
                                                        MaxBid = item.MaxBid,
                                                        CampaignTp =  item.CampaignTp,
                                                        AddMoney = item.AddMoney,
                                                        Type= item.Type }).ToArray();

            if (autoBidOnItemsParsed.Length > 0)
            {
                var kwdData = JArray.FromObject(autoBidOnItemsParsed);
                Console.WriteLine(kwdData);

                psi.Arguments = $"\"{script}\" \"{BASE_URL}\" \"{API_KEY}\" \"{SECRET_KEY}\" \"{CUSTOMER_ID}\" \"{kwdData}\"";

                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;

                var result = "result";
                var error = "";
                try
                {
                    using (var process = Process.Start(psi))
                    {
                        result = process.StandardOutput.ReadToEnd();
                        error = process.StandardError.ReadToEnd();
                    }
                    parseData(result);
                }
                catch
                {
                    MessageBox.Show("API 데이터 Update Error발생");
                    return false;
                }
                Console.WriteLine(result);
            }
            return true;
        }

    }
}
