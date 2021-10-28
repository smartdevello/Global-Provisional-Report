using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Global_Provisional_Report
{
    public class PrecinctRenderer
    {

        private int width = 0, height = 0;
        private double totHeight = 1000;
        private Bitmap bmp = null;
        private Graphics gfx = null;
        private List<PrecinctData> data = null;
        private List<CodeDescription> ycode = null;
        private List<CodeDescription> ncode = null;

        Image logoImg = Image.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "assets", "logo.png"));
        public PrecinctRenderer(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
        public void setRenderSize(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
        public int getDataCount()
        {
            if (this.data == null) return 0;
            else return this.data.Count;
        }
        public List<PrecinctData> getData()
        {
            return this.data;
        }
        public void setChatData(List<PrecinctData> data, List<CodeDescription> ycode, List<CodeDescription> ncode)
        {
            this.data = data;
            this.ycode = ycode;
            this.ncode = ncode;
        }
        public Point convertCoord(Point a)
        {
            double px = height / totHeight;

            Point res = new Point();
            res.X = (int)((a.X + 20) * px);
            res.Y = (int)((1000 - a.Y) * px);
            return res;
        }
        public PointF convertCoord(PointF p)
        {
            double px = height / totHeight;
            PointF res = new PointF();
            res.X = (int)((p.X + 20) * px);
            res.Y = (int)((1000 - p.Y) * px);
            return res;
        }
        public Bitmap getBmp()
        {
            return this.bmp;
        }
        public void drawCenteredString_withBorder(string content, Rectangle rect, Brush brush, Font font, Color borderColor)
        {

            //using (Font font1 = new Font("Arial", fontSize, FontStyle.Bold, GraphicsUnit.Point))

            // Create a StringFormat object with the each line of text, and the block
            // of text centered on the page.
            double px = height / totHeight;
            rect.Location = convertCoord(rect.Location);
            rect.Width = (int)(px * rect.Width);
            rect.Height = (int)(px * rect.Height);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            // Draw the text and the surrounding rectangle.
            gfx.DrawString(content, font, brush, rect, stringFormat);

            Pen borderPen = new Pen(new SolidBrush(borderColor), 2);
            gfx.DrawRectangle(borderPen, rect);
            borderPen.Dispose();
        }

        public void drawCenteredString(string content, Rectangle rect, Brush brush, Font font)
        {

            //using (Font font1 = new Font("Arial", fontSize, FontStyle.Bold, GraphicsUnit.Point))

            // Create a StringFormat object with the each line of text, and the block
            // of text centered on the page.
            double px = height / totHeight;
            rect.Location = convertCoord(rect.Location);
            rect.Width = (int)(px * rect.Width);
            rect.Height = (int)(px * rect.Height);

            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            // Draw the text and the surrounding rectangle.
            gfx.DrawString(content, font, brush, rect, stringFormat);
            //gfx.DrawRectangle(Pens.Black, rect);

        }
        private void fillPolygon(Brush brush, PointF[] points)
        {
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = convertCoord(points[i]);
            }
            gfx.FillPolygon(brush, points);
        }
        public void drawLine(Point p1, Point p2, Color color, int linethickness = 1)
        {
            if (color == null)
                color = Color.Gray;

            p1 = convertCoord(p1);
            p2 = convertCoord(p2);
            gfx.DrawLine(new Pen(color, linethickness), p1, p2);

        }
        public void drawString(Font font, Color brushColor, string content, Point o)
        {
            o = convertCoord(o);
            SolidBrush drawBrush = new SolidBrush(brushColor);
            gfx.DrawString(content, font, drawBrush, o.X, o.Y);
        }
        public void drawString(Point o, string content, int font = 15)
        {

            o = convertCoord(o);

            // Create font and brush.
            Font drawFont = new Font("Arial", font);
            SolidBrush drawBrush = new SolidBrush(Color.Black);

            gfx.DrawString(content, drawFont, drawBrush, o.X, o.Y);

        }
        public void drawString(Color color, Point o, string content, int font = 15)
        {

            o = convertCoord(o);

            // Create font and brush.
            Font drawFont = new Font("Arial", font);
            SolidBrush drawBrush = new SolidBrush(color);

            gfx.DrawString(content, drawFont, drawBrush, o.X, o.Y);

            drawFont.Dispose();
            drawBrush.Dispose();

        }
        public void fillRectangle(Color color, Rectangle rect)
        {
            rect.Location = convertCoord(rect.Location);
            double px = height / totHeight;
            rect.Width = (int)(rect.Width * px);
            rect.Height = (int)(rect.Height * px);

            Brush brush = new SolidBrush(color);
            gfx.FillRectangle(brush, rect);
            brush.Dispose();

        }
        public void drawRectangle(Pen pen, Rectangle rect)
        {
            rect.Location = convertCoord(rect.Location);
            double px = height / totHeight;
            rect.Width = (int)(rect.Width * px);
            rect.Height = (int)(rect.Height * px);
            gfx.DrawRectangle(pen, rect);
        }
        public void drawImg(Image img, Point o, Size size)
        {
            double px = height / totHeight;
            o = convertCoord(o);
            Rectangle rect = new Rectangle(o, new Size((int)(size.Width * px), (int)(size.Height * px)));
            gfx.DrawImage(img, rect);

        }
        public void drawPie(Color color, Point o, Size size, float startAngle, float sweepAngle, string content = "")
        {
            // Create location and size of ellipse.
            double px = height / totHeight;
            size.Width = (int)(size.Width * px);
            size.Height = (int)(size.Height * px);

            Rectangle rect = new Rectangle(convertCoord(o), size);
            // Draw pie to screen.            
            Brush grayBrush = new SolidBrush(color);
            gfx.FillPie(grayBrush, rect, startAngle, sweepAngle);

            o.X += size.Width / 2;
            o.Y -= size.Height / 2;
            float radius = size.Width * 0.3f;
            o.X += (int)(radius * Math.Cos(Helper.DegreesToRadians(startAngle + sweepAngle / 2)));
            o.Y -= (int)(radius * Math.Sin(Helper.DegreesToRadians(startAngle + sweepAngle / 2)));
            content += "\n" + string.Format("{0:F}%", sweepAngle * 100.0f / 360.0f);
            drawString(o, content, 9);
        }
        public void drawFilledCircle(Brush brush, Point o, Size size)
        {
            double px = height / totHeight;
            size.Width = (int)(size.Width * px);
            size.Height = (int)(size.Height * px);

            Rectangle rect = new Rectangle(convertCoord(o), size);

            gfx.FillEllipse(brush, rect);
        }

        public void draw()
        {
            if (bmp == null)
                bmp = new Bitmap(width, height);
            else
            {
                if (bmp.Width != width || bmp.Height != height)
                {
                    bmp.Dispose();
                    bmp = new Bitmap(width, height);

                    gfx.Dispose();
                    gfx = Graphics.FromImage(bmp);
                    gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                }
            }
            if (gfx == null)
            {
                gfx = Graphics.FromImage(bmp);
                gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            }
            else
            {
                gfx.Clear(Color.Transparent);
            }
            drawImg(logoImg, new Point(20, 60), new Size(150, 50));



            if (data == null) return;

            int tA1 = 0, tA2 = 0, tA3 = 0, tA4 = 0, tA5 = 0, tA6 = 0, tA7 = 0, tA8 = 0, tYes = 0 ;
            int tB10 = 0, tB11 = 0, tB12 = 0, tB13 = 0, tB14 = 0, tB17 = 0 , tNo = 0;
            int tReg_v = 0, tActual = 0, tProv = 0;
            foreach (var item in data)
            {
                tA1 += item.A1;
                tA2 += item.A2;
                tA3 += item.A3;
                tA4 += item.A4;
                tA5 += item.A5;
                tA6 += item.A6;
                tA7 += item.A7;
                tA8 += item.A8;

                tB10 += item.B10;
                tB11 += item.B11;
                tB12 += item.B12;
                tB13 += item.B13;
                tB14 += item.B14;
                tB17 += item.B17;

                tReg_v += item.reg_v;
                tActual += item.actual;
                tProv += item.prov;
            }
            tYes = tA1 + tA2 + tA3 + tA4 + tA5 + tA6 + tA7 + tA8;
            tNo = tB10 + tB11 + tB12 + tB13 + tB14 + tB17;

            List<string> ycodeList = new List<string>();
            List<double> ypercentList = new List<double>();
            List<double> npercentList = new List<double>();
            foreach (var item in ycode)
                if (item.reason_code == "A1")
                { 
                    ycodeList.Add(item.description);
                    ypercentList.Add(tA1 / (double)tYes);
                    break;
                }
            foreach (var item in ycode)
                if (item.reason_code == "A2")
                {
                    ycodeList.Add(item.description); 
                    ypercentList.Add(tA2 / (double)tYes);
                    break;
                }
            foreach (var item in ycode)
                if (item.reason_code == "A3")
                {
                    ycodeList.Add(item.description);
                    ypercentList.Add(tA3 / (double)tYes);
                    break;                
                }
            foreach (var item in ycode)
                if (item.reason_code == "A4")
                { 
                    ycodeList.Add(item.description);
                    ypercentList.Add(tA4 / (double)tYes);
                    break;
                }
            foreach (var item in ycode)
                if (item.reason_code == "A5")
                {
                    ycodeList.Add(item.description);
                    ypercentList.Add(tA5 / (double)tYes);
                    break;
                }
            foreach (var item in ycode)
                if (item.reason_code == "A6")
                {
                    ycodeList.Add(item.description);
                    ypercentList.Add(tA6 / (double)tYes);
                    break;
                }
            foreach (var item in ycode)
                if (item.reason_code == "A7")
                {
                    ycodeList.Add(item.description);
                    ypercentList.Add(tA7 / (double)tYes);
                    break;
                }
            foreach (var item in ycode)
                if (item.reason_code == "A8")
                {
                    ycodeList.Add(item.description);
                    ypercentList.Add(tA8 / (double)tYes);
                    break;
                }

            List<string> ncodeList = new List<string>();
            foreach (var item in ncode)
                if (item.reason_code == "B10")
                { 
                    ncodeList.Add(item.description);
                    npercentList.Add(tB10 / (double)tNo);
                    break;
                }

            foreach (var item in ncode)
                if (item.reason_code == "B11")
                {
                    ncodeList.Add(item.description);
                    npercentList.Add(tB11 / (double)tNo);
                    break;
                }

            foreach (var item in ncode)
                if (item.reason_code == "B12")
                {
                    ncodeList.Add(item.description);
                    npercentList.Add(tB12 / (double)tNo);
                    break;
                }

            foreach (var item in ncode)
                if (item.reason_code == "B13")
                {
                    ncodeList.Add(item.description);
                    npercentList.Add(tB13 / (double)tNo);
                    break;
                }

            foreach (var item in ncode)
                if (item.reason_code == "B14")
                {
                    ncodeList.Add(item.description);
                    npercentList.Add(tB14 / (double)tNo);
                    break;
                }

            foreach (var item in ncode)
                if (item.reason_code == "B17")
                {
                    ncodeList.Add(item.description);
                    npercentList.Add(tB17 / (double)tNo);
                    break;
                }

            ////////////////////////////////////////First Section//////////////////////////////////////////////////
            ///
            Font textFont12 = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Point);
            Font textFont10 = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Point);

            Font titleFont = new Font("Arial", 25, FontStyle.Bold, GraphicsUnit.Point);
            Font h5font = new Font("Arial", 16, FontStyle.Bold, GraphicsUnit.Point);
            Font h6font = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point);


            drawString(titleFont, Color.Black, "GLOBAL PROVISIONAL REPORT", new Point(500, 970));

            drawString(textFont12, Color.Black, "  ACCEPTED\nPROVISIONALS", new Point(0, 900));
            drawString(h5font, Color.Black, tYes.ToString(), new Point(470, 890));
            drawString(textFont12, Color.Black, "  REJECTED\nPROVISIONALS", new Point(0, 850));
            drawString(h5font, Color.Black, tNo.ToString(), new Point(470, 840));

            int maxVal = 0, len = 0;
            double percent = 0;
            maxVal = Math.Max(tYes, tNo);
            len = (int)(300 * tYes / (double)maxVal);
            fillRectangle(Color.LimeGreen, new Rectangle(160, 900, len, 50));

            len = (int)(300 * tNo / (double)maxVal);
            fillRectangle(Color.Red, new Rectangle(160, 850, len, 50));


            drawString(textFont12, Color.Black, "  ACCEPTED\nPROVISIONALS", new Point(600, 900));
            percent = Math.Round(tYes * 100 / (double)(tYes + tNo), 2);
            drawString(h5font, Color.Black, percent.ToString() + "%", new Point(1080, 890));

            percent = Math.Round(tNo * 100 / (double)(tYes + tNo), 2);
            drawString(textFont12, Color.Black, "  REJECTED\nPROVISIONALS", new Point(600, 850));
            drawString(h5font, Color.Black, percent.ToString() + "%", new Point(1080, 840));

            len = (int)(300 * tYes / (double)maxVal);
            fillRectangle(Color.LimeGreen, new Rectangle(760, 900, len, 50));
            len = (int)(300 * tNo / (double)maxVal);
            fillRectangle(Color.Red, new Rectangle(760, 850, len, 50));


            ////////////////////////////////////////////////////////////////////////////////////////////////////
            ///
            Brush redBrush = new SolidBrush(Color.Red);
            Brush greenBrush = new SolidBrush(Color.LimeGreen);
            Brush blackBrush = new SolidBrush(Color.Black);
            double lowest_Rejection_Rate = 1;
            double highest_Rejection_Rate = 0;
            double avg_Rejection_Rate = 0;
            string low_precinct = "", high_precinct = "";
             
            foreach (var item in data)
            {
                if (item.prov == 0 || item.tNo == 0) continue;
                percent = item.tNo / (double)item.prov;
                if (percent < lowest_Rejection_Rate)
                {
                    lowest_Rejection_Rate = percent;
                    low_precinct = item.precinct;
                    
                }
                if (percent > highest_Rejection_Rate)
                {
                    highest_Rejection_Rate = percent;
                    high_precinct = item.precinct;
                }

            }
            lowest_Rejection_Rate = Math.Round(lowest_Rejection_Rate * 100, 2);
            highest_Rejection_Rate = Math.Round(highest_Rejection_Rate * 100, 2);
            fillRectangle(Color.LimeGreen, new Rectangle(20, 750, 250, 40));
            drawFilledCircle(greenBrush, new Point(0, 750), new Size(40, 40));
            drawFilledCircle(greenBrush, new Point(250, 750), new Size(40, 40));

            len = (int)(lowest_Rejection_Rate * 2.5);
            drawFilledCircle(redBrush, new Point(0, 750), new Size(40, 40));
            fillRectangle(Color.Red, new Rectangle(20, 750, len, 40));
            drawFilledCircle(redBrush, new Point(0 + len, 750), new Size(40, 40));

            drawString(h5font, Color.Black, lowest_Rejection_Rate.ToString() + "%" , new Point(300, 750));
            drawString(h5font, Color.Black, "Lowest Rejection Rate", new Point(0, 700));


            /////////////////////////////////////////////////
            fillRectangle(Color.LimeGreen, new Rectangle(420, 750, 250, 40));
            drawFilledCircle(greenBrush, new Point(400, 750), new Size(40, 40));
            drawFilledCircle(greenBrush, new Point(650, 750), new Size(40, 40));

            len = tNo * 250 / (tYes + tNo);
            percent = Math.Round(tNo * 100 / (double)(tYes + tNo), 2);
            drawFilledCircle(redBrush, new Point(400, 750), new Size(40, 40));
            fillRectangle(Color.Red, new Rectangle(420, 750, len, 40));
            drawFilledCircle(redBrush, new Point(400 + len, 750), new Size(40, 40));


            drawString(h5font, Color.Black, percent.ToString() + "%", new Point(700, 750));
            drawString(h5font, Color.Black, "Average Rejection Rate", new Point(400, 700));

            ///////////////////////////////////////////////////////////////////
            fillRectangle(Color.LimeGreen, new Rectangle(820, 750, 250, 40));
            drawFilledCircle(greenBrush, new Point(800, 750), new Size(40, 40));
            drawFilledCircle(greenBrush, new Point(1050, 750), new Size(40, 40));

            len = (int)(highest_Rejection_Rate * 2.5);
            drawFilledCircle(redBrush, new Point(800, 750), new Size(40, 40));
            fillRectangle(Color.Red, new Rectangle(820, 750, len, 40));
            drawFilledCircle(redBrush, new Point(800 + len, 750), new Size(40, 40));

            drawString(h5font, Color.Black, highest_Rejection_Rate.ToString() + "%", new Point(1100, 750));
            drawString(h5font, Color.Black, "Highest Rejection Rate", new Point(800, 700));

            /////////////////////////////////////////////////////////////////////////////////////////////////////

            ////////////////////////Draw D&R Section///////////////////////
            int rep = 0, dem = 0;
            foreach(var item in data)
            {
                if (item.r08 == "REP") rep++;
                else if (item.r08 == "DEM") dem++;
            }

            fillRectangle(Color.Black, new Rectangle(1250, 900, 60, 60));
            drawCenteredString("08", new Rectangle(1250, 900, 60, 60), Brushes.White, titleFont);

            fillRectangle(Color.Black, new Rectangle(1250, 830, 60, 60));
            drawCenteredString("D", new Rectangle(1250, 830, 60, 60), Brushes.Aqua, titleFont);
            int rdpercent = 0;
            rdpercent = (int)Math.Round(dem * 100 / (double)data.Count);
            drawCenteredString(rdpercent.ToString() + "%", new Rectangle(1250, 770, 60, 40), Brushes.Black, textFont12);

            fillRectangle(Color.Black, new Rectangle(1250, 730, 60, 60));
            rdpercent = (int)Math.Round(rep * 100 / (double)data.Count);
            drawCenteredString("R", new Rectangle(1250, 730, 60, 60), Brushes.Red, titleFont);
            drawCenteredString(rdpercent.ToString() + "%", new Rectangle(1250, 670, 60, 40), Brushes.Black, textFont12);


            ////////////////////////////////
            rep = 0; dem = 0;
            foreach (var item in data)
            {
                if (item.r12 == "REP") rep++;
                else if (item.r12 == "DEM") dem++;
            }

            fillRectangle(Color.Black, new Rectangle(1330, 900, 60, 60));
            drawCenteredString("12", new Rectangle(1330, 900, 60, 60), Brushes.White, titleFont);

            fillRectangle(Color.Black, new Rectangle(1330, 830, 60, 60));
            drawCenteredString("D", new Rectangle(1330, 830, 60, 60), Brushes.Aqua, titleFont);
            rdpercent = (int)Math.Round(dem * 100 / (double)data.Count);
            drawCenteredString(rdpercent.ToString() + "%", new Rectangle(1330, 770, 60, 40), Brushes.Black, textFont12);

            fillRectangle(Color.Black, new Rectangle(1330, 730, 60, 60));
            drawCenteredString("R", new Rectangle(1330, 730, 60, 60), Brushes.Red, titleFont);
            rdpercent = (int)Math.Round(rep * 100 / (double)data.Count);
            drawCenteredString(rdpercent.ToString() + "%", new Rectangle(1330, 670, 60, 40), Brushes.Black, textFont12);


            ////////////////////////////////
            ///
            rep = 0; dem = 0;
            foreach (var item in data)
            {
                if (item.r16 == "REP") rep++;
                else if (item.r16 == "DEM") dem++;
            }

            fillRectangle(Color.Black, new Rectangle(1410, 900, 60, 60));
            drawCenteredString("16", new Rectangle(1410, 900, 60, 60), Brushes.White, titleFont);

            fillRectangle(Color.Black, new Rectangle(1410, 830, 60, 60));
            drawCenteredString("D", new Rectangle(1410, 830, 60, 60), Brushes.Aqua, titleFont);
            rdpercent = (int)Math.Round(dem * 100 / (double)data.Count);
            drawCenteredString(rdpercent.ToString() + "%", new Rectangle(1410, 770, 60, 40), Brushes.Black, textFont12);

            fillRectangle(Color.Black, new Rectangle(1410, 730, 60, 60));
            drawCenteredString("R", new Rectangle(1410, 730, 60, 60), Brushes.Red, titleFont);
            rdpercent = (int)Math.Round(rep * 100 / (double)data.Count);
            drawCenteredString(rdpercent.ToString() + "%", new Rectangle(1410, 670, 60, 40), Brushes.Black, textFont12);


            ////////////////////////////////
            ///
            rep = 0; dem = 0;
            foreach (var item in data)
            {
                if (item.r20 == "REP") rep++;
                else if (item.r20 == "DEM") dem++;
            }
            fillRectangle(Color.Black, new Rectangle(1490, 900, 60, 60));
            drawCenteredString("20", new Rectangle(1490, 900, 60, 60), Brushes.White, titleFont);

            fillRectangle(Color.Black, new Rectangle(1490, 830, 60, 60));
            drawCenteredString("D", new Rectangle(1490, 830, 60, 60), Brushes.Aqua, titleFont);
            rdpercent = (int)Math.Round(dem * 100 / (double)data.Count);
            drawCenteredString(rdpercent.ToString() + "%", new Rectangle(1490, 770, 60, 40), Brushes.Black, textFont12);

            fillRectangle(Color.Black, new Rectangle(1490, 730, 60, 60));
            drawCenteredString("R", new Rectangle(1490, 730, 60, 60), Brushes.Red, titleFont);
            rdpercent = (int)Math.Round(rep * 100 / (double)data.Count);
            drawCenteredString(rdpercent.ToString() + "%", new Rectangle(1490, 670, 60, 40), Brushes.Black, textFont12);




            //////////////////////////////////////////Text Section//////////////////////////////////////////////////
            //LightGray Background
            fillRectangle(Color.LightGray, new Rectangle(800, 620, 900, 650));

            //Draw Border Lines
            fillRectangle(Color.Black, new Rectangle(100, 620, 1400, 10));
            fillRectangle(Color.Black, new Rectangle(100, 640, 50, 50));
            fillRectangle(Color.Black, new Rectangle(1450, 640, 50, 50));

            //Draw Half Line
            drawLine(new Point(800, 620), new Point(800, 0), Color.Black, 3);

            drawCenteredString("ACCEPTED", new Rectangle(100, 620, 700, 100), blackBrush, h5font);
            drawCenteredString("REJECTED", new Rectangle(800, 620, 700, 100), blackBrush, h5font);

            /////////////////////////////////////////////////////////////////////////////////////////////////////


            ///////////////////////////////////////  Draw Content /////////////////////////////////////////////////////
            /////
            ///




            int yCood = 500;
            int index = 0;
            foreach (var code in ycodeList)
            {
                ;
                drawString(textFont10, Color.Black, code, new Point(100, yCood));                
                drawString(h6font, Color.Black, Math.Round(ypercentList[index] * 100, 2).ToString() + "%", new Point(600, yCood));
                int lineCnt = code.Count(c => c == '\n') + 1;
                yCood = yCood - 25 * lineCnt;
                index++;
            }

            yCood = 500;
            index = 0;
            foreach (var code in ncodeList)
            {
                drawString(textFont10, Color.Black, code, new Point(850, yCood));
                drawString(h6font, Color.Black, Math.Round(npercentList[index] * 100, 2).ToString() + "%", new Point(1400, yCood));
                int lineCnt = code.Count(c => c == '\n') + 1;
                yCood = yCood - 25 * lineCnt;
                index++;
            }



            //List<CodeDescription> currentNoCode = ncode.FindAll(item => item.precinct == currentPrecinct.precinct);

            //yCood = 450;
            //foreach (var code in currentNoCode)
            //{
            //    drawString(textFont, Color.Black, code.description, new Point(850, yCood));
            //    percent = Math.Round(code.count * 100 / (double)totNo, 2);
            //    drawString(titleFont, Color.Black, percent.ToString() + "%", new Point(1400, yCood));
            //    int lineCnt = code.description.Count(c => c == '\n') + 1;
            //    yCood = yCood - 40 * lineCnt;
            //}


            //textFont = new Font("Arial", 16, FontStyle.Regular, GraphicsUnit.Point);

            //drawString(textFont, Color.Black, totYes.ToString(), new Point(750, 50));
            //drawString(textFont, Color.Black, totNo.ToString(), new Point(815, 50));

            drawString(textFont12, Color.Black, "© 2021 Tesla Laboratories, llc & JHP", new Point(1200, 50));

            textFont12.Dispose();
            textFont10.Dispose();
            h6font.Dispose();

            titleFont.Dispose();
            redBrush.Dispose();
            greenBrush.Dispose();
            blackBrush.Dispose();
            h5font.Dispose();

        }
    }
}
