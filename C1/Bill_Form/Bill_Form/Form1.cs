using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Models;
using PdfExport;
namespace Bill_Form
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.comboBox1.DataSource = formTool.AreaList;
            this.comboBox1.DisplayMember = "区域";
            this.comboBox1.ValueMember = "Area_Name";
            formTool.SetByArea("全部区域");
            this.dataGridView1.DataSource = new BindingCollection<MainContract>(formTool.CurrentContracts);
            this.toolStripStatusLabel1.Text = this.dataGridView1.Rows.Count.ToString();
            dataGridView1.ClearSelection();
            initDateControl();
            FlushOptionControl();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            formTool.SetByArea(comboBox1.SelectedValue.ToString());
            this.dataGridView1.DataSource = formTool.CurrentContracts;
            this.toolStripStatusLabel1.Text = "合计:"+this.dataGridView1.Rows.Count.ToString();
            dataGridView1.ClearSelection();
            this.toolStripStatusLabel2.Text = "当前选中合同:"+this.dataGridView1.SelectedRows.Count.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
                BindingClocks();
            else
                BindingTickets();
        }
        private List<int> GetViewCurrentCid()
        {
            List<int> Result = null;
             if (this.dataGridView1.SelectedRows.Count > 0)
            {
                Result = new List<int>();
                if (this.dataGridView1.SelectedRows.Count > 0)
                    foreach (DataGridViewRow row in this.dataGridView1.SelectedRows)
                        Result.Add(Convert.ToInt32(row.Cells[0].Value));
             }
             return Result;
        }
        private List<Ticket_Group> GetTickets(List<int> cid)
        {
            List<Ticket_Group> temp = null;
            if (checkBox1.Checked == false)//如果不考虑账期
                temp =  formTool.GetTickets(cid, GetCheckedFeeName());
            ///////////////////////////////////////////////////////////////
            if (checkBox1.Checked == true)//如果考虑账期还有指定项的话
            {
                LoadOption();
                 temp = new List<Ticket_Group>();
                foreach (formOption fo in formTool.CurrentOption)
                    temp.AddRange(formTool.GetTickets(cid, fo.FeeName, fo.AccountDate));
            }
            return temp;
        }
        private List<Clock> GetClocks(List<int> cid)
        {
            List<Clock> temp = null;
            if (checkBox1.Checked == false)//如果不考虑账期
                temp = formTool.GetClocks(cid);
            ///////////////////////////////////////////////////////////////
            if (checkBox1.Checked == true)//如果考虑账期还有指定项的话
            {
                LoadOption();
                temp = new List<Clock>();
                foreach (formOption fo in formTool.CurrentOption)
                {
                    if (fo.FeeName.Contains("电") || fo.FeeName.Contains("水"))
                    {
                        temp.AddRange(formTool.GetClocks(cid, fo.AccountDate));
                        break;
                    }
                }
            }
            return temp;
        }
        private void BindingTickets()
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                this.dataGridViewSummary1.DataSource = null;
                this.dataGridViewSummary1.DataSource = new BindingCollection<Ticket_Group>(GetTickets(GetViewCurrentCid()));
                this.dataGridViewSummary1.Refresh();
            }
        }
        private void BindingClocks()
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                this.dataGridViewSummary1.DataSource = null;
                this.dataGridViewSummary1.DataSource = new BindingCollection<Clock>(GetClocks(GetViewCurrentCid()));
                this.dataGridViewSummary1.Refresh();
            }
        }
        private void ExportPDF()
        {
            List<int> cids = GetViewCurrentCid();
            
            if(cids!=null)
            {
                List<MainContract> temp = new List<MainContract>();
                foreach (int cid in cids)
                    temp.Add(formTool.CurrentContracts.Find(c => c.CID == cid));
                LoadOption();
                PdfExport.PdfExport export = new PdfExport.PdfExport(formTool.GlobleConfig);
                export.doExport(this.comboBox1.SelectedItem.ToString(), this.comboBox2.SelectedItem.ToString() + this.comboBox3.SelectedItem.ToString(),
                    this.dateTimePicker1.Value.ToString("yyyy-MM-dd"), this.comboBox7.SelectedItem.ToString() + this.comboBox8.SelectedItem.ToString(), GetTickets(GetViewCurrentCid()), temp);
            }
        }
        private List<string> GetCheckedFeeName()
        {
            List<string> result = new List<string>();
            for (int i = 0; i < this.checkedListBox1.CheckedItems.Count; i++)
                result.Add(checkedListBox1.CheckedItems[i].ToString());
            return result;
        }
        private void LoadOption()//费用项装载入选项参数
        {
            formTool.CurrentOption.Clear();
            List<string> feeNames=null;
            if (this.checkedListBox1.CheckedItems.Count > 0)
            {
                feeNames = GetCheckedFeeName();
                foreach(string f in feeNames)
                {   formOption o= new formOption();
                    o.FeeName=f;
                    o.AccountDate=comboBox2.SelectedItem.ToString() + "-" + comboBox3.SelectedItem.ToString();
                    formTool.CurrentOption.Add(o);
                }
                if (listBox1.Items.Count > 0)
                {
                    foreach (var item in listBox1.Items)//把指定项改成指定的账期值
                        formTool.CurrentOption.Find(c => c.FeeName == (item.ToString().Split(new char[] { '>' })[0])).AccountDate = item.ToString().Split(new char[] { '>' })[1];
                        //op.FeeName = item.ToString().Split(new char[] { '>' })[0];
                        //op.AccountDate = item.ToString().Split(new char[] { '>' })[1];
                        //formTool.CurrentOption.Add(op);
                }
            }
        }
        private void FlushOptionControl()//费用项加入combo
        {
            checkedListBox1.Items.Clear();
            comboBox6.Items.Clear();
            foreach (string s in formTool.CurrentFeeNames)
            { 
                this.checkedListBox1.Items.Add(s);
                comboBox6.Items.Add(s);
                comboBox6.SelectedIndex = 0;
            }
            for (int i = 0; i < this.checkedListBox1.Items.Count; i++)
                this.checkedListBox1.SetItemChecked(i, true);
        }
        private void initDateControl()
        {
            this.comboBox2.Items.Insert(0,(DateTime.Now.Year - 1).ToString());
            this.comboBox4.Items.Insert(0, (DateTime.Now.Year - 1).ToString());
            this.comboBox7.Items.Insert(0, (DateTime.Now.Year - 1).ToString());
            this.comboBox2.Items.Add((DateTime.Now.Year  ).ToString());
            this.comboBox4.Items.Add((DateTime.Now.Year).ToString());
            this.comboBox7.Items.Add((DateTime.Now.Year).ToString());
            this.comboBox2.Items.Add((DateTime.Now.Year + 1).ToString());
            this.comboBox4.Items.Add((DateTime.Now.Year + 1).ToString());
            this.comboBox7.Items.Add((DateTime.Now.Year + 1).ToString());

            this.comboBox2.SelectedIndex = 1;
            this.comboBox4.SelectedIndex = 1;
            this.comboBox7.SelectedIndex = 1;
            this.comboBox3.Items.Insert(0, "01");
            this.comboBox5.Items.Insert(0, "01");
            this.comboBox8.Items.Insert(0, "01");
            for (int i = 2; i < 13; i++)
            { 
                this.comboBox3.Items.Add(i < 10 ? "0" + i.ToString() : i.ToString());
                this.comboBox5.Items.Add(i < 10 ? "0" + i.ToString() : i.ToString());
                this.comboBox8.Items.Add(i < 10 ? "0" + i.ToString() : i.ToString());
            }
            this.comboBox3.SelectedIndex = DateTime.Now.Month-1;
            this.comboBox5.SelectedIndex = DateTime.Now.Month - 1;
            this.comboBox8.SelectedIndex = DateTime.Now.Month - 1;
            this.dateTimePicker1.Value = DateTime.Now;
            button4.Enabled = false;
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            formTool.CurrentFeeNames.Clear();
            for (int i = 0; i < this.checkedListBox1.SelectedItems.Count;i++ )
            {
                formTool.CurrentFeeNames.Add(this.checkedListBox1.GetItemText(this.checkedListBox1.SelectedItems[i]));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.listBox1.Items.Add(this.comboBox6.SelectedItem.ToString() + ">" + this.comboBox4.SelectedItem.ToString() + "-" + this.comboBox5.SelectedItem.ToString());
        }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (listBox1.Items.Count > 0)
                listBox1.Items.Remove(listBox1.SelectedItem);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ExportPDF();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) 
                button3.Enabled = true;
            else button3.Enabled = false;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button4.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            formTool.Set_AddressComment(Convert.ToInt32(this.dataGridView1.SelectedRows[0].Cells[0].Value.ToString()),
                    textBox1.Text);
                this.button4.Enabled = false;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dataGridView1.SelectedRows.Count > 0)
            {
                if (checkBox2.Checked)
                    BindingClocks();
                else
                    BindingTickets();
                this.textBox1.Text = formTool.Get_AddressComment(Convert.ToInt32(this.dataGridView1.SelectedRows[0].Cells[0].Value.ToString()));

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
