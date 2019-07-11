using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
namespace Models
{
    public class SysTicket 
    {
        private string _fee_name;
        private DateTime _data_start, _data_end;
        private double _fee;
        private string _contract_id, _room, _addrss_id;
        private string _account_date;
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
        public string Account_Date
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
        public DateTime Data_start
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
        public DateTime Data_end
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
        public SysTicket()
        {
            Room = "";
            Addrss_id = "";
        }
    }
    public class Ticket_Group//某CID下某账期下某费用的总和
    {
        private string _fee_name, _account_date, _data_start, _data_end,_custName;
         [DisplayName("结算周期开始")]
        public string Data_start
        {
            get { return _data_start; }
            set { _data_start = value; }
        }
          [DisplayName("结算周期结束")]
        public string Data_end
        {
            get { return _data_end; }
            set { _data_end = value; }
        }
        private int _cid;private double _fee;
        private List<SysTicket> _systickets;
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
        public string Account_date
        {
            get
            {
                return _account_date;
            }
            set
            {
                _account_date= value ;
            }
        }
        [DisplayName("大合同编号")]
        public int Cid
        {
            get
            {
                return _cid;
            }
            set
            {
                _cid = value;
            }
        }
        [DisplayName("商户名称")]
        public string CustName
        {
            get
            {
                return _custName;
            }
            set
            {
                _custName = value;
            }
        }
        [Browsable(false)]
        public List<SysTicket> Systickets
        {
            get
            {
                return _systickets;
            }
            set
            {
                _systickets=value ;
            }
        }
        [DisplayName("费用合计")]
        public double Fee
        {
            get
            {
                return _fee;
            }

            set
            {
                _fee = value;
            }
        }
    }

    public class Clock
    {
        private string _clock_name, _sy_con_id, _clock_type, _Sy_Addr_Id,_CustName,_Account_Date,_Address;
        private int _CID;
        private DateTime _ldate, _rdate;
        private long _lmrd, _tmrd, _current_user;
        [DisplayName("客户名称")]
        public string CustName { get => _CustName; set => _CustName = value; }
        [DisplayName("地址")]
        public string Address { get => _Address; set => _Address = value; }
        [DisplayName("账期")]
        public string Account_Date { get => _Account_Date; set => _Account_Date = value; }
        [DisplayName("表名")]
        public string Clock_name
        {
            get
            {
                return _clock_name;
            }

            set
            {
                _clock_name = value;
            }
        }
        [DisplayName("表类型")]
        public string Clock_type
        {
            get
            {
                return _clock_type;
            }

            set
            {
                _clock_type = value;
            }
        }
        [DisplayName("本次用量")]
        public long Current_use
        {
            get
            {
                return _current_user;
            }

            set
            {
                _current_user = value;
            }
        }
        [DisplayName("上次抄表时间")]
        public DateTime Ldate
        {
            get
            {
                return _ldate;
            }

            set
            {
                _ldate = value;
            }
        }
        [DisplayName("本次抄表时间")]
        public DateTime Rdate
        {
            get
            {
                return _rdate;
            }

            set
            {
                _rdate = value;
            }
        }
        [DisplayName("上次抄表数")]
        public long Lmrd
        {
            get
            {
                return _lmrd;
            }

            set
            {
                _lmrd = value;
            }
        }
        [DisplayName("本次抄表数")]
        public long Tmrd
        {
            get
            {
                return _tmrd;
            }

            set
            {
                _tmrd = value;
            }
        }

        [Browsable(false)]
        public string Sy_Addr_Id
        {
            get
            {
                return _Sy_Addr_Id;
            }

            set
            {
                _Sy_Addr_Id = value;
            }
        }
        [Browsable(false)]
        public string Sy_con_id
        {
            get
            {
                return _sy_con_id;
            }

            set
            {
                _sy_con_id = value;
            }
        }
        [Browsable(false)]
        public int CID { get => _CID; set => _CID = value; }


    }
}
