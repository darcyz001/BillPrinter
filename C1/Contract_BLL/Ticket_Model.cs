using System;
using System.ComponentModel;

namespace Ticket_BLL
{
    public class Total
    {
        private string _fee_name, _data_start, _data_end;
        private double _fee;
        private string _contract_id, _room, _addrss_id;
        private DateTime _account_date;
        [DisplayName("费用名称")]
        public string Fee_name
        {
            get
            {
                return _fee_name;
            }

            set
            {
                _fee_name = value;
            }
        }
        [DisplayName("应收账期")]
        public DateTime Account_Date
        {
            get
            {
                return _account_date;
            }

            set
            {
                _account_date = value;
            }
        }
        [DisplayName("费用")]
        public double Fee
        {
            get
            {

                return Math.Round(_fee, 2);
            }

            set
            {
                _fee = value;
            }
        }
        [DisplayName("合同编号")]
        public string Contract_id
        {
            get
            {
                return _contract_id;
            }

            set
            {
                _contract_id = value;
            }
        }
        [DisplayName("应收期间开始日期")]
        public string Data_start
        {
            get
            {
                return _data_start;
            }

            set
            {
                _data_start = value;
            }
        }
        [DisplayName("应收期间结束日期")]
        public string Data_end
        {
            get
            {
                return _data_end;
            }

            set
            {
                _data_end = value;
            }
        }
        [DisplayName("账单地址")]
        public string Room
        {
            get
            {
                return _room;
            }

            set
            {
                _room = value;
            }
        }
        [Browsable(false)]
        public string Addrss_id
        {
            get
            {
                return _addrss_id;
            }

            set
            {
                _addrss_id = value;
            }
        }
        public Total()
        {
            Room = "";
            Addrss_id = "";
        }
    }
}
