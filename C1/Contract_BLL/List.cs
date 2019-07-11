using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpConfig;
using InterFace;
using DB_BLL;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using Oracle.DataAccess.Client;
namespace List_BLL
{
    public class syContract : iList
    {
        private Configuration Config;
        protected override object i_GetList()
        {
            var result = GetList();
            return (object)result;
        }
        public syContract(string filename):base(filename)
        {
            Configuration.ValidCommentChars = "#".ToArray();
            Config = Configuration.LoadFromFile(filename, Encoding.UTF8);
        }

        public new List<Sys_Contract> GetList()
        {
            
            List<Sys_Contract> result = Load_Contact();
            return  result;
        }
        private List<Sys_Contract> Load_Contact()
        {
            List<Sys_Contract> result = new List<Sys_Contract>();
            SqlConnection con = Get_Connection();
            string sql = Config["Sql"]["Load_Contract"].Value;
            if (con != null)
            {
                try
                {
                    using (SqlDataReader sr = DB_BLL.SQLHelp.ExecuteReader(con, CommandType.Text, sql, null))
                    {
                        while (sr.Read())
                        {
                            Sys_Contract s = new Sys_Contract(Config["Generic"]["Sys_Name"].Value);
                            s.Cust_Name = sr["客户名称"].ToString().Trim();
                            s.ContractCode = sr["合同编号"].ToString().Trim();
                            s.Add_id = sr["资源编号"].ToString().Trim();
                            s.Add_name = sr["租用单元"].ToString().Trim();
                            result.Add(s);
                        }
                    }
                    return result;
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }
        private SqlConnection Get_Connection()
        {
           
            DB<SqlConnection> sydb = new DB<SqlConnection>();
            SqlConnection sy_con = sydb.Get_Conn_Instance(Config["Connection"]["Conn"].Value);
            return sy_con;
        }
    }
    public class kcContract : iList
    {
        private Configuration Config;
        protected override object i_GetList()
        {
            var result = GetList();
            return (object)result;
        }
        public kcContract(string filename):base(filename)
        {
            Configuration.ValidCommentChars = "#".ToArray();
            Config = Configuration.LoadFromFile(filename, Encoding.UTF8);
        }
        public new List<Sys_Contract> GetList()
        {
            List<Sys_Contract> result = Load_Contact();
            return result;
        }
        private List<Sys_Contract> Load_Contact()
        {
            List<Sys_Contract> result = new List<Sys_Contract>();
            OracleConnection con = Get_Connection();
            string sql = Config["Sql"]["Load_Contract"].Value;
            if (con != null)
            {
                try
                {
                    using (OracleDataReader sr = DB_BLL.OraHelp.ExecuteReader(con, CommandType.Text, sql, null))
                    {
                        while (sr.Read())
                        {
                            Sys_Contract s = new Sys_Contract(Config["Generic"]["Sys_Name"].Value);
                            s.Cust_Name = sr["客户名称"].ToString().Trim();
                            s.ContractCode = sr["合同编号"].ToString().Trim();
                            s.Add_id = sr["资源编号"].ToString().Trim();
                            s.Add_name = sr["租用单元"].ToString().Trim();
                            result.Add(s);
                        }
                    }
                    return result;
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }
        private OracleConnection Get_Connection()
        {

            DB<OracleConnection> kcdb = new DB<OracleConnection>();
            OracleConnection kc_con = kcdb.Get_Conn_Instance(Config["Connection"]["Conn"].Value);
            return kc_con;
        }
    }
    public class MainContractor:iList
    {
        private Configuration Config;
        List<MainContract> MainContracts;
        protected override object i_GetList()
        {
            var result = GetList();
            return (object)result;
        }
        public MainContractor(string filename):base(filename)
        {
            Configuration.ValidCommentChars = "#".ToArray();
            Config = Configuration.LoadFromFile(filename, Encoding.UTF8);
            MainContracts = Load_Contact(filename);
        }
        public new List<MainContract> GetList()
        {
           // List<MC> result = Load_Contact();
            return MainContracts;
        }
        private List<MainContract> Load_Contact(string filename)
        {
            List<MainContract> result = new List<MainContract>();
            SqlConnection con = Get_Connection();
            string sql = Config["Sql"]["Load_Contract"].Value;
            if (con != null)
            {
                try
                {
                    using (SqlDataReader sr = DB_BLL.SQLHelp.ExecuteReader(con, CommandType.Text, sql, null))
                    {
                        while (sr.Read())
                        {
                            MainContract s = new MainContract(sr["contract_name"].ToString().Trim(), filename);
                            s.CID= Convert.ToInt32(sr["CID"]);
                            s.Cust_Name= sr["cust_name"].ToString().Trim();
                            s.Area_name = sr["area_name"].ToString().Trim();
                            s.Area_id = Convert.ToInt32(sr["area_id"]);
                            foreach (string a in s.SubSys.Keys)
                                //此处获取子系统的合同号，用配置文件中的子系统名称-连接到配置数据库中的字段定义，再填到sql中
                                s.SubSys[a].Add(sr[Config["Generic"][a].Value].ToString().Trim());
                            result.Add(s);
                        }
                    }
                    return result;
                }
                catch(Exception ex)
                {
                    return null;
                }
            }
            return null;
        }
        private SqlConnection Get_Connection()
        {
            DB<SqlConnection> db = new DB<SqlConnection>();
            SqlConnection conn = db.Get_Conn_Instance(Config["Connection"]["Conn"].Value);
            return conn;
        }
    }
}
