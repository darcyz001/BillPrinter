using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using InterFace;
using SharpConfig;
using System.IO;
using Models;
namespace PdfExport
{
    public class PdfExport : Export
    {
        Document PDFDoc;
        PdfWriter PDFWri;
        PDFHeaderFooter HeadFooter;
        string temp_file_before,_profile;
        Configuration _options;
        WaterStamp_Pic stamp_pic;
        WaterStamp_Txt stamp_txt;
        int file_index; Font contrant_font;
        string account_date;
        public  PdfExport(Configuration Options)
            : base(Options)
         {
             _options=Options;
             file_index = 0;
             temp_file_before = System.Environment.GetEnvironmentVariable("TEMP") + @"\" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-");
             contrant_font = new Font(BaseFont.CreateFont(@"C:\Windows\Fonts\SIMSUN.TTC,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED),
                 12, Font.NORMAL);
         }
        protected override object _doExport(string profile, string Account_date, string MadeBillDate, string payDate, List<Ticket_Group> Tickets, List<MainContract> msc)
        {
            var result = doExport(  profile,   Account_date, MadeBillDate, payDate,Tickets,  msc);
            return (object)result;
        }
        public new string doExport(string profile,string Account_date,string MadeBillDate, string payDate, List<Ticket_Group> Tickets, List<MainContract> msc)
        {
            string result_file = null;
            account_date = Account_date;
            _profile = "pdf";
            for (int i = 0; i < _options.SectionCount;i++ )
            {
                if (_options[i].Name == profile)
                    _profile = profile;
            }
                

            stamp_pic = new WaterStamp_Pic(_options, _profile);
            stamp_txt = new WaterStamp_Txt(_options, _profile);
            HeadFooter = new PDFHeaderFooter(_options, _profile);
            CreatePDF(get_tempname(true), Tickets, msc, Account_date, MadeBillDate, payDate);
            if (_options[_profile]["IsWaterStamp_Txt"].Value == "T")
                setWatermark_Txt(stamp_txt, get_tempname(false), get_tempname(true));
            if (_options[_profile]["IsWaterStamp_Pic"].Value == "T")
                setWatermark_Pic(stamp_pic, get_tempname(false), get_tempname(true));
            System.Diagnostics.Process.Start("IEXPLORE.EXE", get_tempname(false));
            return result_file;
        }
        private void CreatePDF(string fileName, List<Ticket_Group> Tickets, List<MainContract> msc,string Account_date,string MadeBillDate,string payDate)
        {
            int colspan = 3;
            PDFDoc = new Document(PageSize.A4);
            PDFWri = PdfWriter.GetInstance(PDFDoc, new FileStream(fileName, FileMode.Create));
            float[] widths = new float[] { 100f, 70f, 120f };
            //if (HeadFooter.is_head || HeadFooter.is_foot)
            PDFWri.PageEvent = HeadFooter;
            PDFDoc.Open();
            PdfContentByte canvas = PDFWri.DirectContent;
            PDFWri.CompressionLevel = 0;
            foreach(MainContract mc in msc)
            {
                PdfPTable tab = new PdfPTable(3);
                tab.TotalWidth = PDFDoc.PageSize.Width - 50;
                tab.SetWidths(widths); 
                BaseFont bfont = BaseFont.CreateFont(@"C:\Windows\Fonts\SIMSUN.TTC,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                 Font headFont = new Font(bfont, 16, Font.BOLD);
                foreach (PdfPCell p in GetContantHead(mc, Account_date,3,headFont))
                    tab.AddCell(p);
                foreach (PdfPCell p in GetContantBill(mc, Tickets,3,contrant_font))
                    tab.AddCell(p);
                foreach (PdfPCell c in GetContantTail(payDate, colspan))
                    tab.AddCell(c);
                tab.WriteSelectedRows(0, -1, 30, PDFDoc.Top - 20, canvas);

                PdfPTable tab1 = new PdfPTable(3);tab1.TotalWidth = PDFDoc.PageSize.Width - 50;tab1.SetWidths(widths);
                foreach (PdfPCell c in GetContantTail_buttom("", MadeBillDate, colspan))
                    tab1.AddCell(c);
                tab1.WriteSelectedRows(0, -1, 30, PDFDoc.Bottom + 50, canvas);

                PDFDoc.NewPage();
            }
            PDFDoc.Close();
        }
        private List<PdfPCell> GetContantBill(MainContract msc, List<Ticket_Group> Tickets, int colspan, Font fb)
        {
             List<PdfPCell> result = new  List<PdfPCell>();
            double Total_count_zj = 0;
            double Total_count_else = 0;
            ///////////////////////////租金类
            PdfPCell c_zj = new PdfPCell(new Phrase("租金类账单:", contrant_font));c_zj.Colspan = colspan; c_zj.Border = Rectangle.NO_BORDER;
            result.Add(c_zj);
            foreach(Ticket_Group Ticket in Tickets.FindAll(c=>c.Cid==msc.CID))
                if(Ticket.Fee_name.Contains("租金"))
                {
                    PdfPCell count_time = new PdfPCell(new Phrase(@"结算期:" + Ticket.Data_start + "   " + Ticket.Data_end, contrant_font)); count_time.Border = Rectangle.NO_BORDER;
                    result.Add(count_time);//结算期

                    PdfPCell f_name = new PdfPCell(new Phrase(Ticket.Fee_name, contrant_font)); f_name.Border = Rectangle.NO_BORDER;
                    result.Add(f_name);//费用名称

                    PdfPCell fee_count=new PdfPCell(new Phrase(@"人民币:"+Ticket.Fee.ToString(),contrant_font));fee_count.Border = Rectangle.NO_BORDER;
                    result.Add(fee_count);//费用
                    Total_count_zj += Ticket.Fee;
                }
            PdfPCell c_temp = new PdfPCell();//total左边两格
            c_temp.BorderWidth = 0.5f; c_temp.BorderColor = new BaseColor(178, 178, 178);c_temp.Border = Rectangle.NO_BORDER; c_temp.Colspan = colspan-1;
            result.Add(c_temp);

            PdfPCell c_zjtotal = new PdfPCell(new Phrase("合计  :" + Total_count_zj.ToString("f2") + "元\n", contrant_font));//合计
            c_zjtotal.BorderWidth = 0.5f; c_zjtotal.BorderColor = new BaseColor(178, 178, 178); c_zjtotal.Border = Rectangle.BOTTOM_BORDER;
            result.Add(c_zjtotal);

            PdfPCell c_blank = new PdfPCell(new Phrase(_options[_profile]["kc_bank"].Value.ToString().Replace("\\n", Environment.NewLine), contrant_font)); c_blank.Colspan = colspan;
            c_blank.BorderWidth = 0.5f; c_blank.BorderColor = new BaseColor(178, 178, 178); c_blank.Border = Rectangle.BOTTOM_BORDER;
             result.Add(c_blank); 
            ///////////////////////////////物业类
             PdfPCell c_wy = new PdfPCell(new Phrase("\n\n物业类账单:", contrant_font)); c_wy.Colspan = colspan; c_wy.Border = Rectangle.NO_BORDER;
            result.Add(c_wy);
            foreach (Ticket_Group Ticket in Tickets.FindAll(c => c.Cid == msc.CID))
                if (!Ticket.Fee_name.Contains("租金"))
                {
                    PdfPCell count_time = new PdfPCell(new Phrase(@"结算期:" + Ticket.Data_start + "   " + Ticket.Data_end, contrant_font)); count_time.Border = Rectangle.NO_BORDER;
                    count_time.Border = Rectangle.NO_BORDER;
                    result.Add(count_time);
                    PdfPCell f_name = new PdfPCell(new Phrase(Ticket.Fee_name, contrant_font));
                    f_name.Border = Rectangle.NO_BORDER;
                    result.Add(f_name);//费用名称
                    PdfPCell fee_count = new PdfPCell(new Phrase(@"人民币:" + Ticket.Fee.ToString(), contrant_font));
                    fee_count.Border = Rectangle.NO_BORDER;
                    result.Add(fee_count);
                    Total_count_else += Ticket.Fee;
                }
            result.Add(c_temp);

            PdfPCell c_wytotal = new PdfPCell(new Phrase("合计  :" + Total_count_else.ToString("f2") + "元\n", contrant_font));//合计
            c_wytotal.BorderWidth = 0.5f; c_wytotal.BorderColor = new BaseColor(178, 178, 178);c_wytotal.Border = Rectangle.BOTTOM_BORDER;
            result.Add(c_wytotal);

            PdfPCell c_blank1 = new PdfPCell(new Phrase(_options[_profile]["sy_bank"].Value.ToString().Replace("\\n", Environment.NewLine), contrant_font)); c_blank1.Colspan = colspan;
            c_blank1.BorderWidth = 0.5f; c_blank1.BorderColor = new BaseColor(178, 178, 178);c_blank1.Border = Rectangle.BOTTOM_BORDER;
            result.Add(c_blank1);
            
            return result; 
        }
        private List<PdfPCell> GetContantHead(MainContract msc,string AccountDate,int colspan,Font fb)
        {
            List<PdfPCell> result = new List<PdfPCell>();
            string str1 = @"尊敬的租户:" + msc.Cust_Name + "      "+msc.Address_Comment+"\n\n";
            PdfPCell head = new PdfPCell(new Phrase(str1, fb)); head.Colspan = colspan;
            head.Border = Rectangle.NO_BORDER;
            result.Add(head);
            string str2 = _options[_profile]["HeadContent1"].Value.ToString();//@"关于东方渔人码头项目租赁单元，双方已签订租赁合同或意向书。";
            string str3 = _options[_profile]["HeadContent21"].Value.ToString() + AccountDate.Substring(0, 4) + "年" + AccountDate.Substring(4, 2) +
                _options[_profile]["HeadContent22"].Value.ToString();
            PdfPCell head1 = new PdfPCell(new Phrase(str2 + "\n\n" + str3 + "\n\n", contrant_font));
            // PdfPCell result = new PdfPCell(new Phrase(str1, f));
            head1.Border = Rectangle.NO_BORDER; head1.Colspan = colspan;
            result.Add(head1);
            return result;  
        }
        private List<PdfPCell> GetContantTail(string PayDate,int Colspan)
        {

            List<PdfPCell> result = new List<PdfPCell>();
            BaseFont bfont = BaseFont.CreateFont(@"C:\Windows\Fonts\SIMSUN.TTC,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Font fn = new Font(bfont, 12, Font.NORMAL);
            Font fb = new Font(bfont, 12, Font.BOLD);
            string str_add = "";// "账单寄送地址：\n";
            string str_add1 = "";// get_address(bill.Kc_contract_id);
            string str = _options[_profile]["tail_info2"].Value.Replace("\\n", Environment.NewLine);
            str = str.Replace("_year", PayDate.Substring(0,4));
            str = str.Replace("_month",PayDate.Substring(4,2));

            PdfPCell t = new PdfPCell(new Phrase(str_add, fb)); t.Border = Rectangle.NO_BORDER; t.Colspan = Colspan;
            result.Add(t);
            t = new PdfPCell(new Phrase(str_add1, fn)); t.Border = Rectangle.NO_BORDER; t.Colspan = Colspan;
            result.Add(t);

            t = new PdfPCell(new Phrase(_options[_profile]["tail_info1"].Value.Replace("\\n", Environment.NewLine), fb)); t.Border = Rectangle.NO_BORDER; t.Colspan = Colspan;
            result.Add(t);
            t = new PdfPCell(new Phrase(str, fn)); t.Border = Rectangle.NO_BORDER; t.Colspan = Colspan;
            result.Add(t);


            return result;
        }
        private List<PdfPCell> GetContantTail_buttom(string Address_Consum,string MadeBillDate, int Colspan)
        {
            List<PdfPCell> result = new List<PdfPCell>();
            BaseFont bfont = BaseFont.CreateFont(@"C:\Windows\Fonts\SIMSUN.TTC,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
            Font fb = new Font(bfont, 12, Font.BOLD);
            string str_add = Address_Consum;// "账单寄送地址：\n";
            /////////////////////////////////////落款/////////////////////////////////////////////
            PdfPCell t1 = new PdfPCell(new Phrase()); t1.Border = Rectangle.NO_BORDER; t1.Colspan = Colspan - 1;
            result.Add(t1);
            PdfPCell t = new PdfPCell(new Phrase(str_add, fb)); t.Border = Rectangle.NO_BORDER; t.Colspan = Colspan;
            t = new PdfPCell(new Phrase(_options[_profile]["tail_info3"].Value.Replace("\\n", Environment.NewLine)
            .Replace("_date", MadeBillDate), contrant_font));//出单日期
            t.Border = Rectangle.NO_BORDER; t.Colspan = 1;
            result.Add(t);
            return result;
        }
        private string get_tempname(bool is_new)
        {
            string temp_file;
            if (is_new)
            {
                temp_file = temp_file_before + file_index.ToString() + @".pdf";
                file_index++;
            }
            else
                temp_file = temp_file_before + Convert.ToString(file_index - 1) + @".pdf";
            return temp_file;
        }
        private void setWatermark_Txt(WaterStamp_Txt WaterStamp_Txt_Option, string in_file, string out_file)
        {
            PdfReader pdfReader = null;
            PdfStamper pdfStamper = null;
            try
            {
                pdfReader = new PdfReader(in_file);
                FileStream fs = new FileStream(out_file, FileMode.Create);
                pdfStamper = new PdfStamper(pdfReader, fs);
                // 设置密码   
                //pdfStamper.SetEncryption(true,"1", "1", PdfWriter.AllowPrinting); 
                int total = pdfReader.NumberOfPages + 1;
                PdfContentByte content;
                BaseFont font = BaseFont.CreateFont(WaterStamp_Txt_Option.Font, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                PdfGState gs = new PdfGState();
                gs.FillOpacity = WaterStamp_Txt_Option.FillOpacity; //透明度

                int j = WaterStamp_Txt_Option.WaterMarkName.Length;
                char c;
                int rise = 0;
                for (int i = 1; i < total; i++)
                {
                    rise = WaterStamp_Txt_Option.TxtLeft;
                    //content = pdfStamper.GetOverContent(i);//在内容上方加水印
                    content = pdfStamper.GetUnderContent(i);//在内容下方加水印
                    content.SetGState(gs);
                    content.BeginText();
                    content.SetColorFill(new BaseColor(WaterStamp_Txt_Option.FTxtColor.Red,
                                                       WaterStamp_Txt_Option.FTxtColor.Green,
                                                       WaterStamp_Txt_Option.FTxtColor.Blue));//(BaseColor.DARK_GRAY);
                    content.SetFontAndSize(font, WaterStamp_Txt_Option.FontSize);
                    // 设置水印文字字体倾斜 开始 
                    {
                        content.SetTextMatrix(WaterStamp_Txt_Option.TxtLeft, WaterStamp_Txt_Option.TxtHeight);
                        for (int k = 0; k < j; k++)
                        {
                            content.SetTextRise(rise);
                            c = WaterStamp_Txt_Option.WaterMarkName[k];
                            content.ShowText(c + "");
                            rise -= WaterStamp_Txt_Option.TxtRotation;
                        }
                    }
                    content.EndText();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (pdfStamper != null)
                    pdfStamper.Close();
                if (pdfReader != null)
                    pdfReader.Close();
            }
        }
        private static bool setWatermark_Pic(WaterStamp_Pic WaterStamp_Pic_Option, string in_file, string out_file)
        {
            //throw new NotImplementedException();
            PdfReader pdfReader = null;
            PdfStamper pdfStamper = null;
            try
            {
                pdfReader = new PdfReader(in_file);

                int numberOfPages = pdfReader.NumberOfPages;

                iTextSharp.text.Rectangle psize = pdfReader.GetPageSize(1);

                float width = psize.Width;

                float height = psize.Height;

                pdfStamper = new PdfStamper(pdfReader, new FileStream(out_file, FileMode.Create));

                PdfContentByte waterMarkContent;

                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(WaterStamp_Pic_Option.ModelPicName);

                //image.GrayFill = opt.WaterStamp_Pic_Option.GrayFill;//透明度，灰色填充
                image.Rotation = WaterStamp_Pic_Option.Rotation;//旋转
                image.RotationDegrees = WaterStamp_Pic_Option.RotationDegrees;//旋转角度
                //水印的位置 
                if (WaterStamp_Pic_Option.Left < 0)
                {
                    WaterStamp_Pic_Option.Left = width / 2 - image.Width + WaterStamp_Pic_Option.Left;
                }

                //image.SetAbsolutePosition(left, (height - image.Height) - top);
                image.SetAbsolutePosition(WaterStamp_Pic_Option.Left, (height / 2 - image.Height) - WaterStamp_Pic_Option.Top);

                //每一页加水印,也可以设置某一页加水印 

                for (int i = 1; i <= numberOfPages; i++)
                {
                    if (WaterStamp_Pic_Option.IsOverContent)
                        waterMarkContent = pdfStamper.GetOverContent(i);
                    else
                        waterMarkContent = pdfStamper.GetUnderContent(i);
                    waterMarkContent.AddImage(image);
                }
                //strMsg = "success";
                return true;
            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {

                if (pdfStamper != null)
                    pdfStamper.Close();

                if (pdfReader != null)
                    pdfReader.Close();
            }
        }
    }
    public class PDFHeaderFooter : PdfPageEventHelper
    {
        Font fontText;
        Foot_Head Head = null;
        Foot_Head Foot = null;
        BaseFont bfSun;
        string _profile;
        Configuration _options;
        public PDFHeaderFooter(Configuration Options,string profile)//PDF_Option opt)
        {
            // _opt = opt;
            _options = Options;
            _profile = profile;
            Head = LoadFootHead(true);
            Foot = LoadFootHead(false);
            bfSun = BaseFont.CreateFont(@"C:\Windows\Fonts\SIMSUN.TTC,1", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        }
        private PdfPTable onDraw(PdfWriter writer, Document document, Foot_Head options)
        {
            fontText = new Font(bfSun, options.FontSize, 0, new BaseColor(options.FtxtColor.Red, options.FtxtColor.Green, options.FtxtColor.Blue));
            PdfPTable tab;
            PdfPCell cell, cell1;

            options.Txt = options.Txt.Replace(@"\n", "\n");
            cell = new PdfPCell(new Phrase(options.Txt, fontText));
            cell.Border = Rectangle.NO_BORDER;
            if (options.PicName.Length > 1)
            {
                Image img = iTextSharp.text.Image.GetInstance(options.PicName);
                img.ScaleAbsoluteHeight(Convert.ToSingle(options.PicHeight));
                img.ScaleAbsoluteWidth(Convert.ToSingle(options.PicWidth));
                cell1 = new PdfPCell(img);
                cell1.Border = Rectangle.NO_BORDER;
                if (options.IsPicFirst)
                {
                    float[] widths = new float[] {Convert.ToSingle(options.SecondLeftPos+options.PicWidth),
                                                  Convert.ToSingle(options.FontSize *options.Txt.Length + 30)};
                    tab = new PdfPTable(2);
                    tab.TotalWidth = options.FontSize * options.Txt.Length + 30 + options.SecondLeftPos + options.PicWidth;
                    tab.SetWidths(widths);
                    tab.AddCell(cell1); tab.AddCell(cell);
                }
                else
                {
                    float[] widths = new float[] {Convert.ToSingle(options.FontSize *options.Txt.Length + 30),
                                                  Convert.ToSingle(options.SecondLeftPos+options.PicWidth)};
                    tab = new PdfPTable(2);
                    tab.TotalWidth = options.FontSize * options.Txt.Length + 30 + options.SecondLeftPos + options.PicWidth;
                    tab.SetWidths(widths);
                    tab.AddCell(cell1); tab.AddCell(cell);
                    //tab.AddCell(cell); tab.AddCell(cell1);
                }
            }
            else
            {
                tab = new PdfPTable(1);
                // float[] width = new float[] { document.PageSize.Width - 20 };
                tab.TotalWidth = document.PageSize.Width - 40;
                tab.SetWidths(new float[] { tab.TotalWidth });
                tab.AddCell(cell);
            }
            return tab;
        }
        public override void OnOpenDocument(PdfWriter writer, Document document)
        {
            base.OnOpenDocument(writer, document);

        }
        // 页眉
        public override void OnStartPage(PdfWriter writer, Document document)
        {

            base.OnStartPage(writer, document);
            if (Head != null)
                onDraw(writer, document, Head).WriteSelectedRows(0, -1, 30, document.Top + 20, writer.DirectContent);
        }
        // 页脚
        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);
            if (Foot != null)
                onDraw(writer, document, Foot).WriteSelectedRows(0, -1, 30, document.Bottom, writer.DirectContent);
        }
        //write on close of document
        public override void OnCloseDocument(PdfWriter writer, Document document)
        {
            base.OnCloseDocument(writer, document);
        }
        public Foot_Head LoadFootHead(bool isHead)
        {
            Foot_Head opt = new Foot_Head(isHead,_options,_profile);
            return opt;
        }
    }
    public struct FontColor
    {
        int _red; int _green; int _blue;
        public int Red
        {
            get
            {
                return _red;
            }

            set
            {
                _red = value;
            }
        }
        public int Green
        {
            get
            {
                return _green;
            }

            set
            {
                _green = value;
            }
        }
        public int Blue
        {
            get
            {
                return _blue;
            }

            set
            {
                _blue = value;
            }
        }
        public FontColor(int red, int green, int blue)
        {
            _red = red; _green = green; _blue = blue;
        }
    }
    public class Foot_Head
    {
        string _picName, _Txt, _Font, _TxtColor;
        int _FontSize, _SecondLeftPos, _picHeight, _picWidth;
        bool _IsPicFirst;
        FontColor _ftxtColor;
        Configuration _options;
        string _profile;
        bool _ishead;
        public string PicName
        {
            get
            {
                return _picName;
            }
            set
            {
                _picName = value;
            }
        }
        public string Txt
        {
            get
            {
                return _Txt;
            }
            set
            {
                _Txt = value;
            }
        }
        public string FontName
        {
            get
            {
                return _Font;
            }
            set
            {
                _Font = value;
            }
        }
        public string TxtColor
        {
            get
            {
                return _TxtColor;
            }
            set
            {
                _TxtColor = value;
                _ftxtColor = new FontColor(Convert.ToInt16(value.ToString().Split(new char[] { ',' }, 3)[0]),
                                    Convert.ToInt16(value.ToString().Split(new char[] { ',' }, 3)[1]),
                                    Convert.ToInt16(value.ToString().Split(new char[] { ',' }, 3)[2]));
            }
        }
        public int SecondLeftPos
        {
            get
            {
                return _SecondLeftPos;
            }

            set
            {
                _SecondLeftPos = value;
            }
        }
        public bool IsPicFirst
        {
            get
            {
                return _IsPicFirst;
            }
            set
            {
                _IsPicFirst = value;
            }
        }
        public FontColor FtxtColor
        {
            get
            {
                return _ftxtColor;
            }
        }
        public int FontSize
        {
            get
            {
                return _FontSize;
            }
            set
            {
                _FontSize = value;
            }
        }
        public int PicHeight
        {
            get
            {
                return _picHeight;
            }

            set
            {
                _picHeight = value;
            }
        }
        public int PicWidth
        {
            get
            {
                return _picWidth;
            }

            set
            {
                _picWidth = value;
            }
        }
        public Foot_Head(bool isHead,Configuration Options, string profile, string Txt = @"页眉", string FontName = @"C:\WINDOWS\Fonts\SIMFANG.TTF", string TxtColor = @"0,255,188",
                                    int FontSize = 30)
        {
            _ishead = isHead; _Txt = Txt; _Font = FontName; _TxtColor = TxtColor; _FontSize = FontSize; _options = Options; _profile = profile;
            if (isHead)
                LoadHeadOptions();
            else
                LoadFootOptions();
        }
        private void LoadHeadOptions()
        {
            FontName = _options[_profile]["Head_Font"].Value;
            FontSize = Convert.ToInt16(_options[_profile]["Head_FontSize"].Value);
            IsPicFirst = _options[_profile]["Head_IsPicFirst=T"].Value == "T" ? true : false;
            PicHeight = Convert.ToInt16(_options[_profile]["Head_PicHeigth"].Value);
            PicName = _options[_profile]["Head_PicName"].Value;
            PicWidth = Convert.ToInt16(_options[_profile]["Head_PicWidth"].Value);
            SecondLeftPos = Convert.ToInt16(_options[_profile]["Head_SecondLeftPos"].Value);
            Txt = _options[_profile]["Head_Txt"].Value;
            TxtColor = _options[_profile]["Head_TxtColor"].Value;
        }
        private void LoadFootOptions()
        {
            FontName = _options[_profile]["Foot_Font"].Value;
            FontSize = Convert.ToInt16(_options[_profile]["Foot_FontFontSize"].Value);
            IsPicFirst = _options[_profile]["Foot_IsPicFirst=T"].Value == "T" ? true : false;
            PicHeight = Convert.ToInt16(_options[_profile]["Foot_PicHeigth"].Value);
            PicName = _options[_profile]["Foot_PicName"].Value;
            PicWidth = Convert.ToInt16(_options[_profile]["Foot_PicWidth"].Value);
            SecondLeftPos = Convert.ToInt16(_options[_profile]["Foot_SecondLeftPos"].Value);
            Txt = _options[_profile]["Foot_Txt"].Value;
            TxtColor = _options[_profile]["Foot_TxtColor"].Value;
        }
    }
    public class WaterStamp_Txt
    {
        string _waterMarkName;
        BaseFont _Bfont;
        string _font;
        float _fillOpacity;
        FontColor _ftxtColor;
        string _txtColor;
        int _fontSize = 10;
        int _txtRotation;
        int _txtLeft;
        int _txtHeight;
        string _profile;
        Configuration _options;
        /// <summary>
        /// 水印文字内容
        /// </summary>
        public string WaterMarkName
        {
            get
            {
                _waterMarkName = _options[_profile]["W_Txt_WaterMarkName"].Value;
                return _waterMarkName;
            }

            set
            {
                _waterMarkName = value;
            }
        }
        /// <summary>
        /// 字体样式
        /// </summary>
        public string Font
        {
            get
            {
                _font = _options[_profile]["W_Txt_Font"].Value;
                return _font;
            }

            set
            {
                _font = value;
            }
        }
        /// <summary>
        /// 透明度
        /// </summary>
        public float FillOpacity
        {
            get
            {
                _fillOpacity = Convert.ToSingle(_options[_profile]["W_Txt_FillOpacity"].Value);
                return _fillOpacity;
            }

            set
            {
                _fillOpacity = value;
            }
        }
        /// <summary>
        /// 字体颜色
        /// </summary>

        public FontColor FTxtColor
        {
            get
            {
                return _ftxtColor;
            }
        }
        public string TxtColor
        {
            get
            {
                _ftxtColor = new FontColor(Convert.ToInt16(_options[_profile]["W_Txt_TxtColor"].Value.Split(new char[] { ',' }, 3)[0]),
                                     Convert.ToInt16(_options[_profile]["W_Txt_TxtColor"].Value.Split(new char[] { ',' }, 3)[1]),
                                     Convert.ToInt16(_options[_profile]["W_Txt_TxtColor"].Value.Split(new char[] { ',' }, 3)[2]));
                return _txtColor;
            }
            set
            {
                _txtColor = value;

            }
        }
        /// <summary>
        /// 字体大小
        /// </summary>
        public int FontSize
        {
            get
            {
                _fontSize = Convert.ToInt16(_options[_profile]["W_Txt_FontSize"].Value);
                return _fontSize;
            }

            set
            {
                _fontSize = value;
            }
        }
        /// <summary>
        /// 旋转度
        /// </summary>
        public int TxtRotation
        {
            get
            {
                _txtRotation = Convert.ToInt16(_options[_profile]["W_Txt_TxtRotation"].Value);
                return _txtRotation;
            }

            set
            {
                _txtRotation = value;
            }
        }
        /// <summary>
        /// 文字位置
        /// </summary>
        public int TxtLeft
        {
            get
            {
                _txtLeft = Convert.ToInt16(_options[_profile]["W_Txt_TxtLeft"].Value);
                return _txtLeft;
            }

            set
            {
                _txtLeft = value;
            }
        }
        /// <summary>
        /// 文字位置
        /// </summary>
        public int TxtHeight
        {
            get
            {
                _txtHeight = Convert.ToInt16(_options[_profile]["W_Txt_TxtHeight"].Value);
                return _txtHeight;
            }

            set
            {
                _txtHeight = value;
            }
        }
        public WaterStamp_Txt(Configuration Options, string profile, string WaterMarkName = @"机密水印", string FontName = @"C:\WINDOWS\Fonts\SIMFANG.TTF",
            string TxtColorName = @"0,255,188", float FillOpacity = 0.3f,
            int FontSize = 50, int TxtRotation = 30, int TxtLeft = 30, int TxtHeight = 30)
        {
            _options = Options;
            _profile = profile;
            _waterMarkName = WaterMarkName;
            _font = FontName;
            _txtColor = TxtColor;
            _txtRotation = TxtRotation;
            _txtLeft = TxtLeft;
            _txtHeight = TxtHeight;
            _Bfont = BaseFont.CreateFont(FontName, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            _font = FontName;
            _ftxtColor = new FontColor(Convert.ToInt16(TxtColorName.Split(new char[] { ',' }, 3)[0]),
                                    Convert.ToInt16(TxtColorName.Split(new char[] { ',' }, 3)[1]),
                                    Convert.ToInt16(TxtColorName.Split(new char[] { ',' }, 3)[2]));
        }
    }
    public class WaterStamp_Pic
    {
        string _ModelPicName;
        float _top;
        float _left;
        int _GrayFill;
        int _Rotation;
        int _RotationDegrees;
        bool _isOverContent;
        string _profile;
        Configuration _opitons;
        /// <summary>
        /// 图片文件名称
        /// </summary>
        public string ModelPicName
        {
            get
            {
                _ModelPicName = _opitons[_profile]["W_Pic_PicName"].Value;
                return _ModelPicName;
            }

            set
            {
                _ModelPicName = value;
            }
        }
        /// <summary>
        ///  图片高度
        /// </summary>
        public float Top
        {
            get
            {
                _top = Convert.ToSingle(_opitons[_profile]["W_Pic_Top"].Value);
                return _top;
            }

            set
            {
                _top = value;
            }
        }
        /// <summary>
        /// 图片左边位置
        /// </summary>
        public float Left
        {
            get
            {
                _left = Convert.ToSingle(_opitons[_profile]["W_Pic_Left"].Value);
                return _left;
            }

            set
            {
                _left = value;
            }
        }
        /// <summary>
        /// 透明度，灰色填充
        /// </summary>
        public int GrayFill
        {
            get
            {
                _GrayFill = Convert.ToInt16(_opitons[_profile]["W_Pic_GrayFill"].Value);
                return _GrayFill;
            }

            set
            {
                _GrayFill = value;
            }
        }
        /// <summary>
        /// 旋转
        /// </summary>
        public int Rotation
        {
            get
            {
                _Rotation = Convert.ToInt16(_opitons[_profile]["W_Pic_Rotation"].Value);
                return _Rotation;
            }

            set
            {
                _Rotation = value;
            }
        }
        /// <summary>
        /// 旋转角度
        /// </summary>
        public int RotationDegrees
        {
            get
            {
                _Rotation = Convert.ToInt16(_opitons[_profile]["W_Pic_RotationDegrees"].Value);
                return _RotationDegrees;
            }

            set
            {
                _RotationDegrees = value;
            }
        }
        /// <summary>
        /// 是否在内容上加水印
        /// </summary>
        public bool IsOverContent
        {
            get
            {
                _isOverContent = _opitons[_profile]["W_Pic_IsOverContent"].Value == "T" ? true : false;
                return _isOverContent;
            }

            set
            {
                _isOverContent = value;
            }
        }
        public WaterStamp_Pic(Configuration Options, string profile, string PicName = @".\WaterStamp.jpg", float Top = 100, float Left = 100, int GrayFill = 50, int Rotation = 30, int RorationDegrees = 30, bool isOverContent = false)
        {
            _opitons = Options;
            _profile = profile;
            _ModelPicName = PicName;
            _top = Top;
            _left = Left;
            _GrayFill = GrayFill;
            _Rotation = Rotation;
            _RotationDegrees = RorationDegrees;
            _isOverContent = isOverContent;
        }
    }


}
