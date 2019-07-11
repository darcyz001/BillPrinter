using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
namespace InterFace
{
        public abstract class ContractList
    {
        protected string _sysName;
        public ContractList(string filename) { }
        protected virtual object i_GetList() { return null; }
        public object GetList() { return i_GetList(); }
        //public virtual void SetAddressComment(int cid,string comment) { ;}
        //public virtual string GetAddressComment(int cid) { return null; }
        public string SysName
        {
            get { return _sysName; }
        }
    }
    public abstract class TicketList
    {
        protected string _sysName;
        public TicketList(string filename) { }
        protected virtual object i_GetList() { return null; }
        public object GetList() { return i_GetList(); }
        public string SysName
        {
            get { return _sysName; }
        }
    }

}
