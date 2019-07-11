using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using DB_BLL;
namespace Models
{
    public class Sys_Contract
    {
        private string _ContractCode, _Cust_Name, _add_name, _add_id,_sys_name;
        [DisplayName("系统合同号")]
        public string ContractCode
        {
            get
            {
                return _ContractCode;
            }

            set
            {
                _ContractCode = value;
            }
        }
        [DisplayName("客户名称")]
        public string Cust_Name
        {
            get
            {
                return _Cust_Name;
            }

            set
            {
                _Cust_Name = value;
            }
        }
        [DisplayName("系统地址")]
        public string Add_name
        {
            get
            {
                return _add_name;
            }

            set
            {
                _add_name = value;
            }
        }
        [DisplayName("地址编号")]
        public string Add_id
        {
            get
            {
                return _add_id;
            }

            set
            {
                _add_id = value;
            }
        }
        [Browsable(false)]
        public string ContractType
        {
            get
            {
                return _sys_name;
            }
        }

        public Sys_Contract(string SysName  )
        {
            _sys_name = SysName;
            _ContractCode = _Cust_Name = "";
        }
    }
   
    public class MainContract:IEnumerable<string>//装载子系统的合同，装载主系统的合同，
    {
        private int _CID, _area_id;
        protected string _area_name, _Contract_Name, _Cust_Name, _Address_Comment, _sqlConnection;

        private Dictionary<string,string> subSys;//第一个参数sys_type,第二个参数sys_contractCode
                                                 //private Contracts _Contracts;
                                                 // private Configuration Config;
        #region funcions
        private string getSysContract(string ContractType)
        {
            if (subSys[ContractType] == "")
                return null;
            return subSys[ContractType];
        }
        private bool UpdateSth(string strsql, SqlParameter[] paras)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                SqlConnection conn = DB2<SqlConnection>.GetConnection(_sqlConnection);//Get_Connection();
                SqlTransaction tr = null;
                try
                {
                    conn.Open();
                    tr = conn.BeginTransaction();
                    cmd.Connection = conn;
                    cmd.Transaction = tr;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = strsql;
                    cmd.Parameters.AddRange(paras);
                    cmd.ExecuteNonQuery();
                    tr.Commit();
                    
                    return true;
                }
                catch (Exception ex)
                {
                    if (tr != null)
                        tr.Rollback();
                    return false;
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        private object FlushSth(string strsql, SqlParameter[] paras,string returnpara)
        {
            SqlConnection conn = DB2<SqlConnection>.GetConnection(_sqlConnection);
            object result=null;
            if (conn != null)
            {
                try
                {
                    using (SqlDataReader sr = DB_BLL.SQLHelp.ExecuteReader(conn, CommandType.Text, strsql, paras))
                    {
                        while (sr.Read())
                        {
                            result = sr[returnpara];  
                        }
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            return null;
        }
        #endregion
        #region values
        [Browsable(false)]
        public string Address_Comment
        {
            get { return _Address_Comment; }
            set
            {
                string sql = "update main_contract set Address_comment=@comment where CID =@cid";
                SqlParameter[] ps = { new SqlParameter("@cid", CID), new SqlParameter("@comment", value) };
                if(UpdateSth(sql,ps))
                    _Address_Comment = value; 
            }
        }
        [DisplayName("合同ID")]
        public int CID
        {
            get
            {
                return _CID;
            }
        }

        [DisplayName("合同名称")]
        public string Contract_Name
        {
            get
            {
                return _Contract_Name;
            }

            set
            {
                string sql = "update main_contract set Contract_Name=@Contract_Name where CID =@cid";
                SqlParameter[] ps = { new SqlParameter("@cid", CID), new SqlParameter("@Contract_Name", value) };
                if (UpdateSth(sql, ps))
                    _Contract_Name = value;
            }
        }

        [DisplayName("客户名称")]
        public string Cust_Name
        {
            get
            {
                return _Cust_Name;
            }

            set
            {
                string sql = "update main_contract set Cust_Name=@custName where CID =@cid";
                SqlParameter[] ps = { new SqlParameter("@cid", CID), new SqlParameter("@custName", value) };
                if (UpdateSth(sql, ps))
                    _Cust_Name = value;
            }
        }

        //[DisplayName("区域")]
        [Browsable(false)]
        public string Area_name
        {
            get
            {
                return _area_name;
            }
        }

        [Browsable(false)]
        public int Area_id
        {
            get
            {
                return _area_id;
            }
            set
            {
                string sql = "update main_contract set area_id=@area_id where CID =@cid";
                SqlParameter[] ps = { new SqlParameter("@cid", CID), new SqlParameter("@area_id", value) };

                string sql1 = "select area_name from main_area where area_id=@area_id ";
                SqlParameter[] ps1 = { new SqlParameter("@area_id", value) };
                if (UpdateSth(sql, ps))
                { 
                    _area_id = value;
                    _area_name=(string)FlushSth(sql1, ps1, "area_name");
                }
            }
        }

        [Browsable(false)]
        public Dictionary<string, string> SubSys
        {
            get
            {
                return subSys;
            }

            set
            {
                subSys = value;
            }
        }
        #endregion

        public MainContract(int cid,int areaid,string areaname,string custname,string addresscomment,string ContractName, object[] subSysTypes,string sqlConnection)
        {
            _Contract_Name = ContractName;
            _sqlConnection = sqlConnection;
            // mContracts = new List<Sys_Contract>();
            subSys = new Dictionary<string, string>();
            _CID = cid;_area_id = areaid;_area_name = areaname;_Cust_Name = custname;_Address_Comment = addresscomment;
                foreach (string systype in subSysTypes)
                {
                    if (!subSys.ContainsKey(systype))
                    {
                        Dictionary<string, string> d = new Dictionary<string,string>();
                        subSys.Add(systype, "");
                    }
                }
        }
        public IEnumerator<string> GetEnumerator()
        {
            return subSys.Keys.GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
       

        public  string this[string ContractType]
        {
            get
            {
               return getSysContract(ContractType);
            }
        }
        //public void AddContract(Sys_Contract Contract)
        //{
        //    ///?
        //    if (Contract == null)
        //        throw new ArgumentNullException("传入的参数为空");
        //    mContracts.Add(Contract);
        //}
        //public void AddContracts(List<Sys_Contract> cons)
        //{
        //    foreach(Sys_Contract con in cons)
        //    {
        //        this.AddContract(con);
        //    }
        //}
        //public void Add(Sys_Contract Contract)
        //{
        //    if (Contract == null)
        //        throw new ArgumentNullException("传入的参数为空");
        //    mContracts.Add(Contract);
        //}
        ////////////////////////////////////////////////////////////
      
    }
    public class Addr_Area
    {
        private int _id;
        private string _Area_Name;
        //[Browsable(false)]
        public int Area_Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
            }
        }
        [DisplayName("区域")]
        public string Area_Name
        {
            get
            {
                return _Area_Name;
            }
            set
            {
                _Area_Name = value;
            }
        }
        public Addr_Area(int id, string area_name)
        {
            _id = id; _Area_Name = area_name;
        }
        public Addr_Area()
        {
            _id = -1;
            _Area_Name = "";
        }
    }
    //public class Contracts : IEnumerable<MainContract>//子系统的合同装载
    //{
    //    private List<MainContract> m_MainContract;

    //    [Browsable(false)]
    //    public List<MainContract> MainContracts
    //    {
    //        get
    //        {
    //            return m_MainContract;
    //        }

    //        set
    //        {
    //            m_MainContract = value;
    //        }
    //    }

    //    protected Contracts()
    //    {
    //        m_MainContract = new List<MainContract>();
    //    }
    //    public IEnumerator<MainContract> GetEnumerator()
    //    {
    //        return m_MainContract.GetEnumerator();
    //    }
    //    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    //    {
    //        return GetEnumerator();
    //    }
    //    public MainContract this[int index]
    //    {
    //        get
    //        {
    //            if (index < 0 || index > m_MainContract.Count)
    //                throw new ArgumentOutOfRangeException("index");

    //            return m_MainContract[index];
    //        }
    //        set
    //        {
    //            if (index < 0 || index > m_MainContract.Count)
    //                throw new ArgumentOutOfRangeException("index");

    //            m_MainContract[index] = value;
    //        }
    //    }
    //    public MainContract this[string MainContract_Name]
    //    {
    //        get
    //        {
    //            var section = getMainContract(MainContract_Name);
    //            return section;
    //        }
    //        set
    //        {
    //            if (value == null)
    //                throw new ArgumentNullException("value");

    //            // Check if there already is a section by that name.
    //            var section = getMainContract(MainContract_Name);

    //            int settingIndex = section != null ? m_MainContract.IndexOf(section) : -1;

    //            if (settingIndex < 0)
    //            {
    //                // A section with that name does not exist yet; add it.
    //                m_MainContract.Add(section);
    //            }
    //            else
    //            {
    //                // A section with that name exists; overwrite.
    //                m_MainContract[settingIndex] = section;
    //            }
    //        }
    //    }
    //    private MainContract getMainContract(string Contract_Name)
    //    {
    //        foreach (var section in m_MainContract)
    //        {
    //            if (string.Equals(section.Contract_Name, Contract_Name, StringComparison.OrdinalIgnoreCase))
    //                return section;
    //        }

    //        return null;
    //    }
    //    public void Add(MainContract Contract)
    //    {
    //        if (Contract == null)
    //            throw new ArgumentNullException("setting");
    //        m_MainContract.Add(Contract);
    //    }
    //}
}
