using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterFace;
using ContractList_BLL;
using TicketList_BLL;
using 
namespace testConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            
            ContractList sy = new Contract(@".\syContract.ini");//获取子系统中的合同号
            List<Sys_Contract> sy_contracts=(List<Sys_Contract >)sy.GetList();

            ContractList kc = new Contract(@".\kcContract.ini");//获取子系统中的合同号
            List<Sys_Contract> kc_contracts = (List<Sys_Contract>)kc.GetList();

            ContractList mContractor = new MainContractor(@".\mainContract.ini");
            List<MainContract> msc = (List< MainContract >) mContractor.GetList();
            
            TicketList syor = new Ticket(@".\syContract.ini");
            List<SysTicket> sy_Tickets= (List<SysTicket>)syor.GetList();

            TicketList kcor = new Ticket(@".\kcContract.ini");
            List<SysTicket> kc_Tickets = (List<SysTicket>)kcor.GetList();

            List<SysTicket> t = new List<SysTicket>();
            t.AddRange(kc_Tickets);
            t.AddRange(sy_Tickets);

            //Ticketor ticketor = new Ticketor(t,msc);
            //Ticket_sum tm = ticketor["物业费"].GetByDate(2018, 9);
            //Ticket_sum tm1 = ticketor["固定租金"].GetByDate(2018, 9);
            //double fee = tm1.GetTicketSum(10);
        }
    }
}
