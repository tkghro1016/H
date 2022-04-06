using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADbid
{
    public class BidData
    {
        private double _LstItemWidth;

        private bool _IsSelected;

        private int _No;

        private string _nccAdgroupId;

        private string _nccCampaignId;

        private string _id;

        private string _GrpName;

        private string _CmpName;

        private string _CmpAndGrpName;

        private string _Keyword;

        private string _AverRank;

        private string _AutoBidYN;

        private bool _IsPCYN;

        private bool _IsMobileYN;

        private int _TargetPCRank;

        private int _TargetMobileRank;

        private string _CurrentBid;

        private string _MaxBid;

        private int _ExpoCnt;

        private int _ClickCnt;

        private string _CampaignTp;

        private string _AddMoney;

        private string _Type;

        private string _Status;

        public double LstItemWidth { get { return _LstItemWidth; } set { _LstItemWidth = value; } }

        public bool IsSelected { get { return _IsSelected; } set { _IsSelected = value; } }

        public int No { get { return _No; } set { _No = value; } }

        public string nccAdgroupId { get { return _nccAdgroupId; } set { _nccAdgroupId = value; } }

        public string nccCampaignId { get { return _nccCampaignId; } set { _nccCampaignId = value; } }
       
        public string id { get { return _id; } set { _id = value; } }

        public string GrpName { get { return _GrpName; } set { _GrpName = value; } }

        public string CmpName { get { return _CmpName; } set { _CmpName = value; } }

        public string CmpAndGrpName { get { return _CmpAndGrpName; } set { _CmpAndGrpName = value; } }

        public string Keyword { get { return _Keyword; } set { _Keyword = value; } }

        public string AverRank { get { return _AverRank; } set { _AverRank = value; } }

        public string AutoBidYN { get { return _AutoBidYN; } set { _AutoBidYN = value; } }

        public bool IsPCYN { get { return _IsPCYN; } set { _IsPCYN = value; } }

        public bool IsMobileYN { get { return _IsMobileYN; } set { _IsMobileYN = value; } }

        public int TargetPCRank { get { return _TargetPCRank; } set { _TargetPCRank = value; } }

        public int TargetMobileRank { get { return _TargetMobileRank; } set { _TargetMobileRank = value; } }

        public string CurrentBid { get { return _CurrentBid; } set { _CurrentBid = value; } }

        public string MaxBid { get { return _MaxBid; } set { _MaxBid = value; } }

        public int ExpoCnt { get { return _ExpoCnt; } set { _ExpoCnt = value; } }

        public int ClickCnt { get { return _ClickCnt; } set { _ClickCnt = value; } }

        public string CampaignTp { get { return _CampaignTp; } set { _CampaignTp = value; } }

        public string AddMoney { get { return _AddMoney; } set { _AddMoney = value; } }

        public string Type { get { return _Type; } set { _Type = value; } }
        
        public string Status { get { return _Status; } set { _Status = value; } }

    }

    public class BidUpdateData
    {
        private string _nccAdgroupId;

        private string _id;

        private bool _IsPCYN;

        private bool _IsMobileYN;

        private int _TargetPCRank;

        private string _MaxBid;

        private string _CampaignTp;

        private string _AddMoney;

        private string _Type;

        public string nccAdgroupId { get { return _nccAdgroupId; } set { _nccAdgroupId = value; } }

        public string id { get { return _id; } set { _id = value; } }

        public bool IsPCYN { get { return _IsPCYN; } set { _IsPCYN = value; } }

        public bool IsMobileYN { get { return _IsMobileYN; } set { _IsMobileYN = value; } }

        public int TargetPCRank { get { return _TargetPCRank; } set { _TargetPCRank = value; } }

        public string MaxBid { get { return _MaxBid; } set { _MaxBid = value; } }

        public string CampaignTp { get { return _CampaignTp; } set { _CampaignTp = value; } }

        public string AddMoney { get { return _AddMoney; } set { _AddMoney = value; } }

        public string Type { get { return _Type; } set { _Type = value; } }

    }

    public class BidLogData
    {
        private string _CmpAndGrpName;

        private string _Keyword;

        private string _AverRank;

        private bool _IsPCYN;

        private bool _IsMobileYN;

        private string _BidMedium;

        private int _TargetPCRank;

        private int _TargetMobileRank;

        private string _TargetRank;

        private string _CurrentBid;

        private string _MaxBid;

        private string _BidTime;

        public string CmpAndGrpName { get { return _CmpAndGrpName; } set { _CmpAndGrpName = value; } }

        public string Keyword { get { return _Keyword; } set { _Keyword = value; } }

        public string AverRank { get { return _AverRank; } set { _AverRank = value; } }

        public bool IsPCYN { get { return _IsPCYN; } set { _IsPCYN = value; } }

        public bool IsMobileYN { get { return _IsMobileYN; } set { _IsMobileYN = value; } }

        public string BidMedium { get { return _BidMedium; } set { _BidMedium = value; } }

        public int TargetPCRank { get { return _TargetPCRank; } set { _TargetPCRank = value; } }

        public int TargetMobileRank { get { return _TargetMobileRank; } set { _TargetMobileRank = value; } }

        public string TargetRank { get { return _TargetRank; } set { _TargetRank = value; } }

        public string CurrentBid { get { return _CurrentBid; } set { _CurrentBid = value; } }

        public string MaxBid { get { return _MaxBid; } set { _MaxBid = value; } }

        public string BidTime { get { return _BidTime; } set { _BidTime = value; } }
    }
}
