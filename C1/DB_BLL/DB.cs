//using InterFace;
using System;
namespace DB_BLL
{
    //public class DB<T> : iDB
    //{
    //    protected override object Inner_Get_Conn_Instance(string conn) { return Get_Conn_Instance(conn); }
    //    public new virtual T Get_Conn_Instance(string conn)
    //    {
    //        var db = MadeConntion(conn);
    //        if (CheckDB(db))
    //            return db;
    //        else
    //            return default(T);
    //    }
    //    protected new virtual T MadeConntion(string conn)
    //    {

    //        base.MadeConntion(conn);
    //        object[] args = new object[1];
    //        args[0] = conn;
    //        object c = Activator.CreateInstance(typeof(T), new object[] { conn });
    //        return (T)c;
    //    }
    //    private bool CheckDB(object obj)
    //    {
    //        System.Data.Common.DbConnection conn_inst = (System.Data.Common.DbConnection)obj;
    //        try
    //        {
    //            conn_inst.Open();
    //            conn_inst.Close();
    //            return true;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //        finally
    //        {
    //            conn_inst.Close();
    //        }
    //    }

    //}
    public  class DB2<T>
    {
        public static T GetConnection(string constr)
        {
            object[] args = new object[1];
            args[0] = constr;
            object c = Activator.CreateInstance(typeof(T), new object[] { constr });
            if (CheckDB(c))
                return (T)c;
            else
                return default(T);
        }
        private static bool CheckDB(object obj)
        {
            System.Data.Common.DbConnection conn_inst = (System.Data.Common.DbConnection)obj;
            try
            {
                conn_inst.Open();
                conn_inst.Close();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                conn_inst.Close();
            }
        }
    }
}
