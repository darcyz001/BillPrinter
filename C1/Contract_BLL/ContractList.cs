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
namespace ContractList_BLL
{
    public class Contract : ContractList
    {
        private Configuration Config;
        protected override object i_GetList()
        {
            var result = GetList();
            return (object)result;
        }
        public Contract(string filename) : base(filename)
        {
            Configuration.ValidCommentChars = "#".ToArray();
            Config = Configuration.LoadFromFile(filename, Encoding.UTF8);
            base._sysName = Config["Generic"]["Sys_Name"].Value;
        }
        public new List<Sys_Contract> GetList()
        {
            List<Sys_Contract> result = Load_Contact();
            return result;
        }
        private List<Sys_Contract> Load_Contact()
        {
            List<Sys_Contract> result = new List<Sys_Contract>();
            DbConnection con=null;
            DbDataReader sr = null;
            string sql = Config["Sql"]["Load_Contract"].Value;

            if (Config["Connection"]["Type"].Value == "Sqlserver")
            {
                con = DB2<SqlConnection>.GetConnection(Config["Connection"]["Conn"].Value);//Get_SqlConnection();
                sr =  DB_BLL.SQLHelp.ExecuteReader((SqlConnection)con, CommandType.Text, sql, null);
            }
            if (Config["Connection"]["Type"].Value == "Oracle")
            {
                con = DB2<OracleConnection>.GetConnection(Config["Connection"]["Conn"].Value);//Get_OracleConnection();
                sr = DB_BLL.OraHelp.ExecuteReader((OracleConnection)con, CommandType.Text, sql, null);
            }
            if (con != null)
            {
                try
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
     
    }
    public class MainContractor: ContractList
    {
        private Configuration Config;
        List<MainContract> _MainContracts;
        object[] myArray;
        protected override object i_GetList()
        {
            var result = GetList();
            return (object)result;
        }
        public MainContractor(string filename):base(filename)
        {
            Configuration.ValidCommentChars = "#".ToArray();
            Config = Configuration.LoadFromFile(filename, Encoding.UTF8);
            base._sysName = Config["Generic"]["Sys_Name"].Value;
            try
            {
                myArray = Config["Generic"]["Sub_SysType"].GetValue<object[]>();//把子系统的类型定义好
            }
            catch(Exception ex)
            { ex = new Exception("配置文件中子系统类型定义出错"); }
            _MainContracts = Load_Contact(filename);
        }
        public new List<MainContract> GetList()
        {
           // List<MC> result = Load_Contact();
            return _MainContracts;
        }
       
        private List<MainContract> Load_Contact(string filename)
        {
            List<MainContract> result = new List<MainContract>();
            SqlConnection con = DB2<SqlConnection>.GetConnection(Config["Connection"]["Conn"].Value);// Get_Connection();
            string sql = Config["Sql"]["Load_Contract"].Value;
            if (con != null)
            {
                try
                {
                    using (SqlDataReader sr = DB_BLL.SQLHelp.ExecuteReader(con, CommandType.Text, sql, null))
                    {
                        while (sr.Read())
                        {
                            MainContract s = new MainContract(Convert.ToInt32(sr["CID"]), Convert.ToInt32(sr["area_id"]), sr["area_name"].ToString().Trim(),
                               sr["cust_name"].ToString().Trim(), sr["Address_Comment"].ToString().Trim(), sr["contract_name"].ToString().Trim(), myArray, Config["Connection"]["Conn"].Value);
                            string[] keys = s.SubSys.Keys.ToArray<string>();
                            foreach (string a in keys)
                                //此处获取子系统的合同号，用配置文件中的子系统名称-连接到配置数据库中的字段定义，再填到sql中
                                s.SubSys[a]=sr[Config["Generic"][a].Value].ToString().Trim();
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
    }
}
