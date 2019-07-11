using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using SharpConfig;
namespace InterFace
{
    public abstract class Export
    {
        protected string _sysName;
        public Export(Configuration config) { }
        protected virtual object _doExport(string profile, string Account_date, string MadeBillDate, string payDate, List<Ticket_Group> Tickets, List<MainContract> msc) { return null; }
        public object doExport(string profile, string Account_date, string MadeBillDate, string payDate, List<Ticket_Group> Tickets, List<MainContract> msc)
        { return _doExport(profile, Account_date, MadeBillDate, payDate, Tickets, msc); }
    }
}
