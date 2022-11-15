using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ATGCustReg_MVC.DataModel
{
    public class ExchangeRate
    {
        public int ID { get; set; }
     
        public string SymbolBaseCurr { get; set; }

        public string SymbolTargetCurr { get; set; }

        public float ExchangeRateValue { get; set; }

        public DateTime ExchangeRatetDateTime { get; set; }
    }
}
