using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADbid
{
    class BizMoney
    {
        public long customerId { get; set; }

        public long bizmoney { get; set; }

        public bool budgetLock { get; set; }

        public bool refundLock { get; set; }
    }
}
