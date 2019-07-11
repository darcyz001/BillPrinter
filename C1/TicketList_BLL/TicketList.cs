using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpConfig;
using InterFace;
using DB_BLL;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;
using Models;
namespace TicketList_BLL
{
    //public class syTicket:TicketList
    //{
    //    private Configuration Config;
    //    protected override object i_GetList()
    //    {
    //        var result = GetList();
    //        return (object)result;
    //    }
    //    public syTicket(string filename) : base(filename)
    //    {
    //        Configuration.ValidCommentChars = "#".ToArray();
    //        Config = Configuration.LoadFromFile(filename, Encoding.UTF8);
    //        base._sysName = Config["Generic"]["Sys_Name"].Value;
    //    }
    //    private SqlConnection Get_Connection()
    //    {

    //        DB<SqlConnection> sydb = new DB<SqlConnection>();
    //        SqlConnection sy_con = sydb.Get_Conn_Instance(Config["Connection"]["Conn"].Value);
    //        return sy_con;
    //    }
    //    public new List<SysTicket> GetList()
    //    {

    //        List<SysTicket> result = Load_Tickets();
    //        return result;
    //    }
    //    private List<SysTicket> Load_Tickets()
    //    {
    //        List<SysTicket> result = new List<SysTicket>();
    //        SqlConnection con = Get_Connection();
    //        string sql = Config["Sql"]["Load_Sy_Bill"].Value;
    //        if (con != null)
    //        {
    //            try
    //            {
    //                using (SqlDataReader sr = DB_BLL.SQLHelp.ExecuteReader(con, CommandType.Text, sql, null))
    //                {
    //                    while (sr.Read())
    //                    {
    //                        SysTicket s = new SysTicket();
    //                        s.Fee_name = sr["收费项目"].ToString().Trim();
    //                        s.Contract_id = sr["合同编号"].ToString().Trim();
    //                        s.Fee = Convert.ToSingle(sr["本金欠收"]);
    //                        s.Account_Date = Convert.ToDateTime(sr["所属账期"]);
    //                       // s.Account_Date = s.Account_Date.AddMonths(-1);//此处在ticketor中调整
    //                        s.Data_start = Convert.ToDateTime(sr["应收期间开始日期"].ToString().Trim()).ToString("yyyy/MM/dd");
    //                        s.Data_end = Convert.ToDateTime(sr["应收期间结束日期"].ToString().Trim()).ToString("yyyy/MM/dd");
    //                        //if (s.Fee_name.Contains("电费") || s.Fee_name.Contains("水费"))//此处在ticketor中调整
    //                        {
    //                           // s.Account_Date = s.Account_Date.AddMonths(2);
    //                        }
    //                        s.Addrss_id = sr["资源id"].ToString().Trim();
    //                        result.Add(s);
    //                    }
    //                }
    //                return result;
    //            }
    //            catch
    //            {
    //                return null;
    //            }
    //        }
    //        return null;
    //    }
    //}
    //public class kcTicket : TicketList
    //{
    //    private Configuration Config;
    //    protected override object i_GetList()
    //    {
    //        var result = GetList();
    //        return (object)result;
    //    }
    //    public kcTicket(string filename) : base(filename)
    //    {
    //        Configuration.ValidCommentChars = "#".ToArray();
    //        Config = Configuration.LoadFromFile(filename, Encoding.UTF8);
    //        base._sysName = Config["Generic"]["Sys_Name"].Value;
    //    }
    //    private OracleConnection Get_Connection()
    //    {

    //        DB<OracleConnection> db = new DB<OracleConnection>();
    //        OracleConnection con =  db.Get_Conn_Instance(Config["Connection"]["Conn"].Value);
    //        return  con;
    //    }
    //    public new List<SysTicket> GetList()
    //    {

    //        List<SysTicket> result = Load_Contact();
    //        return result;
    //    }
    //    private List<SysTicket> Load_Contact()
    //    {
    //        List<SysTicket> result = new List<SysTicket>();
    //        OracleConnection con = Get_Connection();
    //        string sql = Config["Sql"]["Load_Kc_Bill"].Value;
    //        if (con != null)
    //        {
    //            try
    //            {
    //                using (OracleDataReader sr = DB_BLL.OraHelp.ExecuteReader(con, CommandType.Text, sql, null))
    //                {
    //                    while (sr.Read())
    //                    {
    //                        SysTicket s = new SysTicket();
    //                        s.Fee_name = sr["收费项目"].ToString().Trim();
    //                        s.Contract_id = sr["合同编号"].ToString().Trim();
    //                        s.Fee = Convert.ToSingle(sr["本金欠收"]);
    //                        s.Account_Date = Convert.ToDateTime(sr["所属账期"].ToString().Insert(4, "-"));
    //                        s.Data_start = Convert.ToDateTime(sr["应收期间开始日期"].ToString().Trim()).ToString("yyyy/MM/dd");
    //                        s.Data_end = Convert.ToDateTime(sr["应收期间结束日期"].ToString().Trim()).ToString("yyyy/MM/dd");
    //                        s.Addrss_id ="";
    //                        result.Add(s);
    //                    }
    //                }
    //                return result;
    //            }
    //            catch
    //            {
    //                return null;
    //            }
    //        }
    //        return null;
    //    }
    //}
    public class Ticket : TicketList
    {
        private Configuration Config;
        protected override object i_GetList()
        {
            var result = GetList();
            return (object)result;
        }
        public  Ticket(string filename) : base(filename)
        {
            Configuration.ValidCommentChars = "#".ToArray();
            Config = Configuration.LoadFromFile(filename, Encoding.UTF8);
            base._sysName = Config["Generic"]["Sys_Name"].Value;
        }
        public new List<SysTicket> GetList()
        {

            List<SysTicket> result = Load_Contact();
            return result;
        }
        private List<SysTicket> Load_Contact()
        {
            List<SysTicket> result = new List<SysTicket>();
            DbConnection con = null;
            DbDataReader sr = null;
            string sql = Config["Sql"]["Load_Bill"].Value;
            if (Config["Connection"]["Type"].Value == "Sqlserver")
            {
                con = DB2<SqlConnection>.GetConnection(Config["Connection"]["Conn"].Value);// Get_SqlConnection();
                sr = DB_BLL.SQLHelp.ExecuteReader((SqlConnection)con, CommandType.Text, sql, null);
            }
            if (Config["Connection"]["Type"].Value == "Oracle")
            {
                //con = Get_OracleConnection();
                con = DB2<OracleConnection>.GetConnection(Config["Connection"]["Conn"].Value);
                sr = DB_BLL.OraHelp.ExecuteReader((OracleConnection)con, CommandType.Text, sql, null);
            }
            if (con != null)
            {
                try
                {
                        while (sr.Read())
                        {
                            SysTicket s = new SysTicket();
                            s.Fee_name = sr["收费项目"].ToString().Trim();
                            s.Contract_id = sr["合同编号"].ToString().Trim();
                            s.Fee = Convert.ToDouble(sr["本金欠收"].ToString());
                            s.Account_Date = sr["所属账期"].ToString();//yyyy-mm;
                            s.Data_start = Convert.ToDateTime(sr["应收期间开始日期"]);
                            s.Data_end = Convert.ToDateTime(sr["应收期间结束日期"]);
                            s.Addrss_id = "";
                            result.Add(s);
                        }
                    sr.Close();
                    con.Close();
                    con.Dispose();
                    return result;
                }
                catch
                {
                    return null;
                }
                finally
                {
                    sr.Close();
                    con.Close();
                    con.Dispose();
                }
            }
            return null;
        }
        //private OracleConnection Get_OracleConnection()
        //{

        //    DB<OracleConnection> db = new DB<OracleConnection>();
        //    OracleConnection con = db.Get_Conn_Instance(Config["Connection"]["Conn"].Value);
        //    return con;
        //}
        //private SqlConnection Get_SqlConnection()
        //{

        //    DB<SqlConnection> sydb = new DB<SqlConnection>();
        //    SqlConnection sy_con = sydb.Get_Conn_Instance(Config["Connection"]["Conn"].Value);
        //    return sy_con;
        //}
    }
    public class Clockor:TicketList
    {
        private Configuration Config;
        protected override object i_GetList()
        {
            var result = GetList();
            return (object)result;
        }
        public Clockor(string filename) : base(filename)
        {
            Configuration.ValidCommentChars = "#".ToArray();
            Config = Configuration.LoadFromFile(filename, Encoding.UTF8);
            base._sysName = Config["Generic"]["Sys_Name"].Value;
        }
        public new List<Clock> GetList()
        {
            List<Clock> result = new List<Clock>();
            DbConnection con = null;
            DbDataReader sr = null;
            string sql = Config["Sql"]["Load_Clock"].Value;
            if (Config["Connection"]["Type"].Value == "Sqlserver")
            {
                con = DB2<SqlConnection>.GetConnection(Config["Connection"]["Conn"].Value);
                sr = DB_BLL.SQLHelp.ExecuteReader((SqlConnection)con, CommandType.Text, sql, null);
            }
            if (Config["Connection"]["Type"].Value == "Oracle")
            {
                con = DB2<OracleConnection>.GetConnection(Config["Connection"]["Conn"].Value);
                sr = DB_BLL.OraHelp.ExecuteReader((OracleConnection)con, CommandType.Text, sql, null);
            }
            if (con != null)
            {
                try
                {
                    while (sr.Read())
                    {
                        if(Convert.ToInt64(sr["本次用量"])>0)
                        { 
                            Clock s = new Clock();
                            s.Sy_con_id = sr["合同编号"].ToString().Trim();
                            s.Sy_Addr_Id = sr["资源编号"].ToString().Trim();
                            s.Clock_name = sr["表名"].ToString().Trim();
                            s.Lmrd = Convert.ToInt64(Convert.ToDouble(sr["上次抄表数"].ToString()));
                            s.Tmrd = Convert.ToInt64(Convert.ToDouble(sr["本次抄表数"].ToString())); 
                            s.Current_use = Convert.ToInt64(Convert.ToDouble(sr["本次用量"]));
                            s.Ldate = Convert.ToDateTime(sr["上次抄表时间"]);
                            s.Rdate = Convert.ToDateTime(sr["本次抄表时间"]);
                            s.Clock_type = Convert.ToString(sr["表类型"]);
                            s.Account_Date = Convert.ToString(sr["所属账期"]);
                            s.Address= sr["室号"].ToString().Trim();
                            result.Add(s);
                        }
                    }
                    sr.Close();
                    con.Close();
                    con.Dispose();
                    return result;
                }
                catch(Exception ex)
                {
                    return null;
                }
                finally
                {
                    sr.Close();
                    con.Close();
                    con.Dispose();
                }
            }
            return null;
        }
        //private OracleConnection Get_OracleConnection()
        //{

        //    DB<OracleConnection> db = new DB<OracleConnection>();
        //    OracleConnection con = db.Get_Conn_Instance(Config["Connection"]["Conn"].Value);
        //    return con;
        //}
        //private SqlConnection Get_SqlConnection()
        //{

        //    DB<SqlConnection> sydb = new DB<SqlConnection>();
        //    SqlConnection sy_con = sydb.Get_Conn_Instance(Config["Connection"]["Conn"].Value);
        //    return sy_con;
        //}
    }
    public class Ticktor : IEnumerable<Ticket_Group>
    {
        private List<Ticket_Group> _TG;
        private List<Clock> _CK;
        private List<MainContract> _MSC;
        public List<Ticket_Group> TicketGroups
        {
            get
            {
                return _TG;
            }

            set
            {
                TicketGroups = value;
            }
        }

        public Ticktor(List<SysTicket> tickets, List<MainContract> msc)//传入未经整理的所有tickets
        {
            _MSC = msc;
            _TG = Group_Tickets(tickets, msc);
        }
        public void ImportClock(List<Clock> Clocks)
        {
            
            if(_MSC!=null&&Clocks!=null)
            {
                _CK = new List<Clock>();
                    foreach(Clock cl in Clocks)
                    {
                        if (_MSC.Find(c => c.SubSys.Values.Contains(cl.Sy_con_id)) != null)
                        {
                            cl.CID = _MSC.Find(c => c.SubSys.Values.Contains(cl.Sy_con_id)).CID;
                            cl.CustName = _MSC.Find(c => c.CID == cl.CID).Cust_Name;
                            _CK.Add(cl);
                        }
                    }
            }
        }
        private List<Ticket_Group> Group_Tickets(List<SysTicket> tickets, List<MainContract> msc)
        {
            List<Ticket_Group> result = new List<Ticket_Group>();
            foreach (MainContract _msc in msc)//所有大合同CID
            {
                foreach (string sys_contractCode in _msc.SubSys.Values)//大合同下subContractId
                {
                    List<SysTicket> t_cid = tickets.FindAll(c => c.Contract_id == sys_contractCode);//subContractId下的tickets
                    List<string> acc_date = Group_Tickets_byDate(t_cid);
                    List<string> fee_t = Group_Tickets_byFeeName(t_cid);
                    foreach (string ad in acc_date)
                    {
                       // List<SysTicket> temp = new List<SysTicket>();
                        foreach (string ft in fee_t)
                        {
                            List<SysTicket> temp = new List<SysTicket>();
                            double total_fee = 0; DateTime start = Convert.ToDateTime("2900/1/1"); DateTime end = Convert.ToDateTime("1900/1/1") ;
                            foreach (SysTicket tt in t_cid)
                            {
                                if ((tt.Fee_name == ft) && (tt.Account_Date == ad))//某账期下某费用类型下的费用合计
                                {
                                    total_fee += tt.Fee;
                                    temp.Add(tt);
                                    start = tt.Data_start < start ? tt.Data_start : start;
                                    end = tt.Data_end > end ? tt.Data_end : end;
                                }
                            }
                            if (total_fee > 0)
                            {
                                Ticket_Group tg = new Ticket_Group();
                                tg.Cid = _msc.CID;
                                tg.CustName = _msc.Cust_Name;
                                tg.Account_date = temp[0].Account_Date;
                                tg.Fee_name = temp[0].Fee_name;
                                tg.Fee = total_fee;
                                tg.Fee = double.Parse(tg.Fee.ToString("F2"));
                                tg.Systickets = temp;
                                tg.Data_start = start.ToString("yyyy-MM-dd");
                                tg.Data_end = end.ToString("yyyy-MM-dd");
                                result.Add(tg);
                            }
                        }
                    }
                }
            }
            return result;
        }
        private List<string> Group_Tickets_byDate(List<SysTicket> tickets)
        {
            List<string> result = new List<string>();
            foreach (SysTicket t in tickets)
                if (!result.Contains(t.Account_Date))
                    result.Add(t.Account_Date);
            return result;
        }
        private List<string> Group_Tickets_byFeeName(List<SysTicket> tickets)
        {
            List<string> result = new List<string>();
            foreach (SysTicket t in tickets)
                if (!result.Contains(t.Fee_name))
                    result.Add(t.Fee_name);
            return result;
        }
        public IEnumerator<Ticket_Group> GetEnumerator()
        {
            return _TG.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public List<Ticket_Group> this[int mainContractId]
        {
            get { return _TG.FindAll(c => c.Cid == mainContractId); }
        }
        public List<Ticket_Group> this[int mainContractId, string account_date]
        {
            get { return _TG.FindAll(c => c.Cid == mainContractId).FindAll(c => c.Account_date == account_date); }
        }
        public List<Ticket_Group> this[int mainContractId, string account_date, string feeName]
        {
            get { return _TG.FindAll(c => c.Cid == mainContractId).FindAll(c => c.Account_date == account_date).FindAll(c => c.Fee_name == feeName); }
        }
        public List<Clock> GetClocks(List<int> CID,string account_date)
        {
            List<Clock> result = new List<Clock>();
            foreach(int i in CID)
                result.AddRange(_CK.FindAll(c => c.CID == i).FindAll(x=>x.Account_Date==account_date));
            return result;
        }
        public List<Clock> GetClocks(List<int> CID)
        {
            List<Clock> result = new List<Clock>();
            foreach (int i in CID)
                result.AddRange(_CK.FindAll(c => c.CID == i));
            return result;
        }
    }
}
