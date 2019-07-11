using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterFace;
using ContractList_BLL;
using TicketList_BLL;
using System.ComponentModel;
using Models;
using SharpConfig;
namespace Bill_Form
{
    public class formTool
    {
        private static formTool ft=new formTool();
        private static ContractList sy, kc, mContractor;
        private static List<Sys_Contract> sy_contracts, kc_contracts;
        private static List<MainContract> msc;
        private static TicketList syor, kcor;
        private static List<SysTicket> sy_Tickets, kc_Tickets, All_Tickets;
        private static List<Addr_Area> _AreaList;
        private static List<formOption> _CurrentOption;
        private static List<string> _CurrentFeeName;
        private static Ticktor tkor;
        private static Clockor ckor;
        private static List<Clock> _Clocks;
        private static List<MainContract> _CurrentContracts;
        private static Configuration _Config;

        public static Configuration GlobleConfig
        {
            get { return _Config; }
        }
        public static List<formOption> CurrentOption
        {
            get { return formTool._CurrentOption; }
            set { formTool._CurrentOption = value; }
        }
        /// <summary>
        /// ///////////////////////////////////////////////////////////////////
        /// </summary>

        public static List<Addr_Area> AreaList
        {
            get
            {
                return _AreaList;
            }
        }
        public static List<string> CurrentFeeNames
        {
            get
            {
                return _CurrentFeeName;
            }
 
        }
        public static List<MainContract> CurrentContracts
        {
            get
            {
                return _CurrentContracts;
            }
        }

        public static void SetByArea(string AreaName)//影响：_Contracts
        {
            _CurrentContracts = GetContracts(AreaName);
        }
        private formTool()
        {
            Reload_All();
            _CurrentOption = new List<formOption>();
            _CurrentFeeName = new List<string>();
            _CurrentFeeName = getFeeNames(tkor.TicketGroups);
            Configuration.ValidCommentChars = "#".ToArray();
            _Config = Configuration.LoadFromFile(@".\globle.ini", Encoding.UTF8);

        }
        private void Reload_All()
        {
            sy = new Contract(@".\syContract.ini");//获取子系统中的合同号
            sy_contracts = (List<Sys_Contract>)sy.GetList();

            kc = new Contract(@".\kcContract.ini");//获取子系统中的合同号
            kc_contracts = (List<Sys_Contract>)kc.GetList();

            mContractor = new MainContractor(@".\mainContract.ini");
            msc = (List<MainContract>)mContractor.GetList();

            syor = new Ticket(@".\syContract.ini");
            sy_Tickets = (List<SysTicket>)syor.GetList();

            kcor = new Ticket(@".\kcContract.ini");
            kc_Tickets = (List<SysTicket>)kcor.GetList();
            All_Tickets = new List<SysTicket>();
            All_Tickets.AddRange(kc_Tickets);
            All_Tickets.AddRange(sy_Tickets);
            _AreaList = GetAllArea();
            tkor = new Ticktor(All_Tickets, msc);
            ckor = new Clockor(@".\syContract.ini");
            _Clocks = ckor.GetList();
           tkor.ImportClock(_Clocks);
        }
        private static List<MainContract> GetContracts(string AreaName)
        {
            List<MainContract> result = null;
            int AreaId = 0;
            Addr_Area t = AreaList.Find(c => c.Area_Name == AreaName);
            if(t!=null)
                AreaId = t.Area_Id;
            if (AreaId>= 0)
                result = msc.FindAll(c => c.Area_id == AreaId);
            else
                result = msc;
            return result;
        }
        
        private static List<MainContract> GetContractByCID(List<int> CID)
        {
            List<MainContract> cons = new List<MainContract>();
            foreach (int conid in CID)
                cons.Add(msc.Find(c => c.CID == conid));
            return cons;
        }
        public static List<Ticket_Group> GetTickets(List<int> CID)
        {
            List<Ticket_Group> result_l = new List<Ticket_Group>();
            
            foreach(int cid in CID)
                result_l.AddRange(tkor[cid]);
            _CurrentFeeName = getFeeNames(result_l);
            return result_l;
        }
        public static List<Ticket_Group> GetTickets(List<int> CID, List<string> FeeName)
        {
            List<Ticket_Group> result_l = new List<Ticket_Group>();
            List<Ticket_Group> temp = new List<Ticket_Group>();
            temp = GetTickets(CID).ToList<Ticket_Group>();
            foreach (string fn in FeeName)
                result_l.AddRange( temp.FindAll(c =>c.Fee_name == fn));
            //BindingCollection<Ticket_Group> result = new BindingCollection<Ticket_Group>(result_l);
            _CurrentFeeName = getFeeNames(result_l);
            return result_l;
        }
        public static List<Ticket_Group> GetTickets(List<int> CID, string FeeName, string AccountDate)
        {
            List<Ticket_Group> result_l = new List<Ticket_Group>();
            List<Ticket_Group> temp = new List<Ticket_Group>();
            temp = GetTickets(CID).ToList<Ticket_Group>();
            result_l.AddRange(temp.FindAll(c => c.Fee_name == FeeName).FindAll(x => x.Account_date == AccountDate));
            _CurrentFeeName = getFeeNames(result_l);
            return result_l;
        }
        public static List<Ticket_Group> GetTickets(List<int> CID, string AcountDate)
        {
            List<Ticket_Group> result_l = new List<Ticket_Group>();
            foreach (int cid in CID)
                result_l.AddRange(tkor[cid, AcountDate]);
            _CurrentFeeName = getFeeNames(result_l);
            return result_l;
        }
        public static List<Clock> GetClocks(List<int>CID,string AccountDate)
        {
            return tkor.GetClocks(CID, AccountDate);
        }
        public static List<Clock> GetClocks(List<int> CID)
        {
            return tkor.GetClocks(CID);
        }
        private static List<Addr_Area> GetAllArea()
        {
            List<Addr_Area> result = new List<Addr_Area>();
            List<string> Temp = new List<string>();
            foreach (MainContract mm in msc)
                if (!Temp.Contains(mm.Area_name))
                {
                    Temp.Add(mm.Area_name);
                    result.Add(new Addr_Area(mm.Area_id, mm.Area_name));
                }
            Addr_Area all = new Addr_Area(-1, "全部区域");
            result.Insert(0, all);
            return result;
        }
        private static List<string> getFeeNames(List<Ticket_Group> tgs)
        {
            List<string> result = new List<string>();
            foreach (Ticket_Group t in tgs)
                if (!result.Contains(t.Fee_name))
                    result.Add(t.Fee_name);
            return result;
        }
 
        public static  void Set_AddressComment(int cid,string Comment)
        {
            msc.Find(c => c.CID == cid).Address_Comment = Comment;
        }
        public static string Get_AddressComment(int cid)
        {
            return msc.Find(c => c.CID == cid).Address_Comment;
        }
    }
    public class formOption
    {
        private string _feeName, _accountDate;

        public string AccountDate
        {
            get { return _accountDate; }
            set { _accountDate = value; }
        }

        public string FeeName
        {
            get { return _feeName; }
            set { _feeName = value; }
        }
        
    }
}
