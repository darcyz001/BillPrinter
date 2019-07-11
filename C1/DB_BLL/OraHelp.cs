using System;
//using System.Configuration;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data.Sql;
using System.Collections;
namespace DB_BLL
{
    public class OraHelp
    {
        
        // public static readonly string ConnectionString_SevenHappy = System.Configuration.ConfigurationManager.AppSettings["Polaris_SevenHappy"].ToString();
        // ConfigurationManager.ConnectionStrings["SevenHappy_DBConnection"].ConnectionString;
        // Hashtable to store cached parameters
        private static Hashtable parmCache = Hashtable.Synchronized(new Hashtable());

        public static int ExecuteNonQuery(string connectionString, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {

            OracleCommand cmd = new OracleCommand();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                int val = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static int ExecuteNonQuery(OracleConnection connection, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {

            OracleCommand cmd = new OracleCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        public static int ExecuteNonQuery(OracleTransaction trans, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            PrepareCommand(cmd, trans.Connection, trans, cmdType, cmdText, commandParameters);
            int val = cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
            return val;
        }

        public static OracleDataReader ExecuteReader(OracleConnection conn, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();
            //OracleConnection conn = new OracleConnection(connectionString);

            // we use a try/catch here because if the method throws an exception we want to 
            // close the connection throw code, because no datareader will exist, hence the 
            // commandBehaviour.CloseConnection will not work
            try
            {
                PrepareCommand(cmd, conn, null, cmdType, cmdText, commandParameters);
                OracleDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return rdr;
            }
            catch (Exception ex)
            {
                conn.Close();
                throw (ex);
            }
        }
        public static object ExecuteScalar(string connectionString, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {
            OracleCommand cmd = new OracleCommand();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
                object val = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return val;
            }
        }

        public static object ExecuteScalar(OracleConnection connection, CommandType cmdType, string cmdText, params OracleParameter[] commandParameters)
        {

            OracleCommand cmd = new OracleCommand();

            PrepareCommand(cmd, connection, null, cmdType, cmdText, commandParameters);
            object val = cmd.ExecuteScalar();
            cmd.Parameters.Clear();
            return val;
        }

        public static void CacheParameters(string cacheKey, params OracleParameter[] commandParameters)
        {
            parmCache[cacheKey] = commandParameters;
        }

        public static OracleParameter[] GetCachedParameters(string cacheKey)
        {
            OracleParameter[] cachedParms = (OracleParameter[])parmCache[cacheKey];

            if (cachedParms == null)
                return null;

            OracleParameter[] clonedParms = new OracleParameter[cachedParms.Length];

            for (int i = 0, j = cachedParms.Length; i < j; i++)
                clonedParms[i] = (OracleParameter)((ICloneable)cachedParms[i]).Clone();

            return clonedParms;
        }

        private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, CommandType cmdType, string cmdText, OracleParameter[] cmdParms)
        {

            if (conn.State != ConnectionState.Open)
                conn.Open();

            cmd.Connection = conn;
            cmd.CommandText = cmdText;

            if (trans != null)
                cmd.Transaction = trans;

            cmd.CommandType = cmdType;

            if (cmdParms != null)
            {
                foreach (OracleParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
    }
}