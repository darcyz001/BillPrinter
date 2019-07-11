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
    //public class syContract : ContractList
    //{
    //    private Configuration Config;
    //    protected override object i_GetList()
    //    {
    //        var result = GetList();
    //        return (object)result;
    //    }
    //    public syContract(string filename):base(filename)
    //    {
    //        Configuration.ValidCommentChars = "#".ToArray();
    //        Config = Configuration.LoadFromFile(filename, Encoding.UTF8);
    //        base._sysName= Config["Generic"]["Sys_Name"].Value;
    //    }

    //    public new List<Sys_Contract> GetList()
    //    {
            
    //        List<Sys_Contract> result = Load_Contact();
    //        return  result;
    //    }
    //    private List<Sys_Contract> Load_Contact()
    //    {
    //        List<Sys_Contract> result = new List<Sys_Contract>();
    //        SqlConnection con = Get_Connection();
    //        if (con != null)
    //        {
    //            try
    //            {
    //                string sql = Config["Sql"]["Load_Contract"].Value;
    //                using (SqlDataReader sr = DB_BLL.SQLHelp.ExecuteReader(con, CommandType.Text, sql, null))
    //                {
    //                    while (sr.Read())
    //                    {
    //                        Sys_Contract s = new Sys_Contract(Config["Generic"]["Sys_Name"].Value);
    //                        s.Cust_Name = sr["客户名称"].ToString().Trim();
    //                        s.ContractCode = sr["合同编号"].ToString().Trim();
    //                        s.Add_id = sr["资源编号"].ToString().Trim();
    //                        s.Add_name = sr["租用单元"].ToString().Trim();
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
    //    private SqlConnection Get_Connection()
    //    {
           
    //        DB<SqlConnection> sydb = new DB<SqlConnection>();
    //        SqlConnection sy_con = sydb.Get_Conn_Instance(Config["Connection"]["Conn"].Value);
    //        return sy_con;
    //    }
    //}
    //public class kcContract : ContractList
    //{
    //    private Configuration Config;
    //    protected override object i_GetList()
    //    {
    //        var result = GetList();
    //        return (object)result;
    //    }
    //    public kcContract(string filename):base(filename)
    //    {
    //        Configuration.ValidCommentChars = "#".ToArray();
    //        Config = Configuration.LoadFromFile(filename, Encoding.UTF8);
    //        base._sysName = Config["Generic"]["Sys_Name"].Value;
    //    }
    //    public new List<Sys_Contract> GetList()
    //    {
    //        List<Sys_Contract> result = Load_Contact();
    //        return result;
    //    }
    //    private List<Sys_Contract> Load_Contact()
    //    {
    //        List<Sys_Contract> result = new List<Sys_Contract>();
    //        OracleConnection con = Get_Connection();
    //        if (con != null)
    //        {
    //            try
    //            {
    //                string sql = Config["Sql"]["Load_Contract"].Value;
    //                using (OracleDataReader sr = DB_BLL.OraHelp.ExecuteReader(con, CommandType.Text, sql, null))
    //                {
    //                    while (sr.Read())
    //                    {
    //                        Sys_Contract s = new Sys_Contract(Config["Generic"]["Sys_Name"].Value);
    //                        s.Cust_Name = sr["客户名称"].ToString().Trim();
    //                        s.ContractCode = sr["合同编号"].ToString().Trim();
    //                        s.Add_id = sr["资源编号"].ToString().Trim();
    //                        s.Add_name = sr["租用单元"].ToString().Trim();
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
    //    private OracleConnection Get_Connection()
    //    {

    //        DB<OracleConnection> kcdb = new DB<OracleConnection>();
    //        OracleConnection kc_con = kcdb.Get_Conn_Instance(Config["Connection"]["Conn"].Value);
    //        return kc_con;
    //    }
    //}
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
        //private OracleConnection Get_OracleConnection()
        //{

        //    DB<OracleConnection> kcdb = new DB<OracleConnection>();
        //    OracleConnection kc_con = kcdb.Get_Conn_Instance(Config["Connection"]["Conn"].Value);
        //    return kc_con;
        //}
        //private SqlConnection Get_SqlConnection()
        //{

        //    DB<SqlConnection> sydb = new DB<SqlConnection>();
        //    SqlConnection sy_con = sydb.Get_Conn_Instance(Config["Connection"]["Conn"].Value);
        //    return sy_con;
        //}
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
            { ex = new Exception("子系统类型定义出错"); }
            _MainContracts = Load_Contact(filename);
        }
        public new List<MainContract> GetList()
        {
           // List<MC> result = Load_Contact();
            return _MainContracts;
        }
        //public override void SetAddressComment(int cid, string Comment)
        //{
        //    _MainContracts.Find(c => c.CID == cid).Address_Comment = Comment;
        //    //string sql = "update main_contract set Address_comment=@comment where CID =@cid";
        //    //SqlParameter[] ps = { new SqlParameter("@cid", CID),new SqlParameter("@comment",Comment)};
        //    //using (SqlCommand cmd = new SqlCommand())
        //    //{
        //    //    SqlConnection conn = DB2<SqlConnection>.GetConnection(Config["Connection"]["Conn"].Value);//Get_Connection();
        //    //    SqlTransaction tr = null;
        //    //    try
        //    //    {
        //    //        conn.Open();
        //    //        tr = conn.BeginTransaction();
        //    //        cmd.Connection = conn;
        //    //        cmd.Transaction = tr;
        //    //        cmd.CommandType = CommandType.Text;
        //    //        cmd.CommandText = sql;
        //    //        cmd.Parameters.AddRange(ps);
        //    //        cmd.ExecuteNonQuery();
        //    //        tr.Commit();
        //    //        return true;
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        if (tr != null)
        //    //            tr.Rollback();
        //    //        return false;
        //    //    }
        //    //    finally
        //    //    {
        //    //        conn.Close();
        //    //    }
        //    //}
        //}
        //public override string GetAddressComment(int cid)
        //{
        //    //SqlConnection con = DB2<SqlConnection>.GetConnection(Config["Connection"]["Conn"].Value);// Get_Connection();
        //    //string result = "";
        //    //string sql = @"select Address_Comment from main_contract where cid=@cid";
        //    //SqlParameter[] ps = { new SqlParameter("@cid", cid) };
        //    //if (con != null)
        //    //{
        //    //    try
        //    //    {
        //    //        using (SqlDataReader sr = DB_BLL.SQLHelp.ExecuteReader(con, CommandType.Text, sql,ps))
        //    //        {
        //    //            while (sr.Read())
        //    //            {
        //    //                result = sr["Address_Comment"].ToString();
        //    //            }
        //    //        }
        //    //        return result;
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        return result;
        //    //    }
        //    //}
        //    //return result;
        //    return _MainContracts.Find(c => c.CID == cid).Address_Comment;
        //}
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
                            //s.CID= Convert.ToInt32(sr["CID"]);
                            //s.Cust_Name= sr["cust_name"].ToString().Trim();
                            //s.Area_name = sr["area_name"].ToString().Trim();
                            //s.Area_id = Convert.ToInt32(sr["area_id"]);
                            //s.Address_Comment = sr["Address_Comment"].ToString().Trim();
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
        //private SqlConnection Get_Connection()
        //{
        //    DB<SqlConnection> db = new DB<SqlConnection>();
        //    SqlConnection conn = db.Get_Conn_Instance(Config["Connection"]["Conn"].Value);
        //    return conn;
        //}
    }
}
