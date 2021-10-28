using Microsoft.Win32;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Global_Provisional_Report
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PrecinctRenderer precinctRenderer = null;
        public MainWindow()
        {
            precinctRenderer = null;
            InitializeComponent();
        }
        private void myCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            Render();
        }
        private void btnExportCurrentChart_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Image file (*.png)|*.png";
            //saveFileDialog.Filter = "Image file (*.png)|*.png|PDF file (*.pdf)|*.pdf";
            if (saveFileDialog.ShowDialog() == true)
            {
                SaveControlImage(PrecinctChart, saveFileDialog.FileName);
            }
        }
        public BitmapSource CreateBitmapFromControl(FrameworkElement element)
        {
            // Get the size of the Visual and its descendants.
            Rect rect = VisualTreeHelper.GetDescendantBounds(element);
            DrawingVisual dv = new DrawingVisual();

            using (DrawingContext ctx = dv.RenderOpen())
            {
                VisualBrush brush = new VisualBrush(element);
                ctx.DrawRectangle(brush, null, new Rect(rect.Size));
            }

            // Make a bitmap and draw on it.
            int width = (int)element.ActualWidth;
            int height = (int)element.ActualHeight;
            RenderTargetBitmap rtb = new RenderTargetBitmap(
                width, height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(dv);
            return rtb;
        }

        private BitmapImage BmpImageFromBmp(Bitmap bmp)
        {
            using (var memory = new System.IO.MemoryStream())
            {
                bmp.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
        private void SaveControlImage(FrameworkElement control, string filename)
        {
            RenderTargetBitmap rtb = (RenderTargetBitmap)CreateBitmapFromControl(control);
            // Make a PNG encoder.
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            // Save the file.
            using (FileStream fs = new FileStream(filename,
                FileMode.Create, FileAccess.Write, FileShare.None))
            {
                encoder.Save(fs);
            }
        }
        void Render()
        {
            if (precinctRenderer == null)
            {
                precinctRenderer = new PrecinctRenderer((int)myCanvas.ActualWidth, (int)myCanvas.ActualHeight);
            }


            precinctRenderer.setRenderSize((int)myCanvas.ActualWidth, (int)myCanvas.ActualHeight);
            precinctRenderer.draw();
            myImage.Source = BmpImageFromBmp(precinctRenderer.getBmp());

        }
        private string AddLineBreaktoString(string desc)
        {
            int newLineIndex = 1;
            for (int i = 0; i < desc.Length; i++)
            {
                if (desc[i] == ' ' && newLineIndex >= 28)
                {
                    int j = i - 1;
                    while (desc[j] != ' ') j--;
                    StringBuilder sb = new StringBuilder(desc);
                    sb[j] = '\n';
                    desc = sb.ToString();
                    newLineIndex = 1; i = j;
                }
                else newLineIndex++;
            }
            return desc;
        }
        private void btnImportCSV_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";

            List<PrecinctData> data = new List<PrecinctData>();
            List<CodeDescription> ycode = new List<CodeDescription>();
            List<CodeDescription> ncode = new List<CodeDescription>();

            string errMsg = "";
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    IWorkbook workbook = null;
                    string fileName = openFileDialog.FileName;

                    using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        if (fileName.IndexOf(".xlsx") > 0)
                            workbook = new XSSFWorkbook(fs);
                        else if (fileName.IndexOf(".xls") > 0)
                            workbook = new HSSFWorkbook(fs);

                    }

                    ISheet sheet = workbook.GetSheetAt(0);
                    if (sheet != null)
                    {
                        int rowCount = sheet.LastRowNum;
                        for (int i = 1; i < rowCount; i++)
                        {
                            IRow curRow = sheet.GetRow(i);
                            if (curRow == null)
                            {
                                rowCount = i - 1;
                                break;
                            }

                            if (curRow.Cells.Count == 23)
                            {
                                data.Add(new PrecinctData()
                                {
                                    precinct = curRow.GetCell(0).StringCellValue,
                                    name = curRow.GetCell(1).StringCellValue,
                                    reg_v = Convert.ToInt32(curRow.GetCell(2).NumericCellValue),
                                    actual = Convert.ToInt32(curRow.GetCell(3).NumericCellValue),
                                    prov = Convert.ToInt32(curRow.GetCell(4).NumericCellValue),
                                    A1 = Convert.ToInt32(curRow.GetCell(5).NumericCellValue),
                                    A2 = Convert.ToInt32(curRow.GetCell(6).NumericCellValue),
                                    A3 = Convert.ToInt32(curRow.GetCell(7).NumericCellValue),
                                    A4 = Convert.ToInt32(curRow.GetCell(8).NumericCellValue),
                                    A5 = Convert.ToInt32(curRow.GetCell(9).NumericCellValue),
                                    A6 = Convert.ToInt32(curRow.GetCell(10).NumericCellValue),
                                    A7 = Convert.ToInt32(curRow.GetCell(11).NumericCellValue),
                                    A8 = Convert.ToInt32(curRow.GetCell(12).NumericCellValue),
                                    tYes = Convert.ToInt32(curRow.GetCell(5).NumericCellValue) + Convert.ToInt32(curRow.GetCell(6).NumericCellValue) + Convert.ToInt32(curRow.GetCell(7).NumericCellValue) + Convert.ToInt32(curRow.GetCell(8).NumericCellValue) + Convert.ToInt32(curRow.GetCell(9).NumericCellValue) + Convert.ToInt32(curRow.GetCell(10).NumericCellValue) + Convert.ToInt32(curRow.GetCell(11).NumericCellValue) + Convert.ToInt32(curRow.GetCell(12).NumericCellValue),
                                    B10 = Convert.ToInt32(curRow.GetCell(13).NumericCellValue),
                                    B11 = Convert.ToInt32(curRow.GetCell(14).NumericCellValue),
                                    B12 = Convert.ToInt32(curRow.GetCell(15).NumericCellValue),
                                    B13 = Convert.ToInt32(curRow.GetCell(16).NumericCellValue),
                                    B14 = Convert.ToInt32(curRow.GetCell(17).NumericCellValue),
                                    B17 = Convert.ToInt32(curRow.GetCell(18).NumericCellValue),
                                    tNo = Convert.ToInt32(curRow.GetCell(13).NumericCellValue) + Convert.ToInt32(curRow.GetCell(14).NumericCellValue) + Convert.ToInt32(curRow.GetCell(15).NumericCellValue) + Convert.ToInt32(curRow.GetCell(16).NumericCellValue) + Convert.ToInt32(curRow.GetCell(17).NumericCellValue) + Convert.ToInt32(curRow.GetCell(18).NumericCellValue),
                                    r20 = curRow.GetCell(19).StringCellValue,
                                    r16 = curRow.GetCell(20).StringCellValue,
                                    r12 = curRow.GetCell(21).StringCellValue,
                                    r08 = curRow.GetCell(22).StringCellValue
                                });
                            }
                            else
                            {
                                errMsg += string.Format("The Row {0} has a problem\n", i + 1);
                            }


                        }

                    }

                    sheet = workbook.GetSheetAt(1);
                    if (sheet != null)
                    {
                        int rowCount = sheet.LastRowNum;
                        string last_precint = "";
                        string description = "";
                        for (int i = 1; i <= rowCount; i++)
                        {
                            IRow curRow = sheet.GetRow(i);
                            if (curRow == null)
                            {
                                rowCount = i - 1;
                                break;
                            }

                            if (curRow.Cells.Count == 4)
                            {
                                last_precint = curRow.GetCell(0).StringCellValue;
                                if (last_precint.Contains("#")) last_precint = last_precint.Substring(1);

                                description = AddLineBreaktoString(curRow.GetCell(2).StringCellValue);
                                ycode.Add(new CodeDescription()
                                {
                                    precinct = last_precint,
                                    reason_code = curRow.GetCell(1).StringCellValue,
                                    description = description,
                                    count = Convert.ToInt32(curRow.GetCell(3).NumericCellValue)
                                });
                            }
                            //else if (curRow.Cells.Count == 3)
                            //{
                            //    description = AddLineBreaktoString(curRow.GetCell(2).StringCellValue);
                            //    ycode.Add(new CodeDescription()
                            //    {
                            //        precinct = last_precint,
                            //        reason_code = curRow.GetCell(1).StringCellValue,
                            //        description = description,
                            //        count = Convert.ToInt32(curRow.GetCell(3).NumericCellValue)
                            //    });
                            //}
                            //else
                            //{
                            //    errMsg += string.Format("The Row {0} has a problem\n", i + 1);
                            //}
                        }
                    }

                    sheet = workbook.GetSheetAt(2);
                    if (sheet != null)
                    {
                        int rowCount = sheet.LastRowNum;
                        string last_precint = "";
                        string description = "";
                        for (int i = 1; i <= rowCount; i++)
                        {
                            IRow curRow = sheet.GetRow(i);
                            if (curRow == null)
                            {
                                rowCount = i - 1;
                                break;
                            }

                            if (curRow.Cells.Count == 4)
                            {
                                last_precint = curRow.GetCell(0).StringCellValue;
                                if (last_precint.Contains("#")) last_precint = last_precint.Substring(1);

                                description = AddLineBreaktoString(curRow.GetCell(2).StringCellValue);
                                ncode.Add(new CodeDescription()
                                {
                                    precinct = last_precint,
                                    reason_code = curRow.GetCell(1).StringCellValue,
                                    description = description,
                                    count = Convert.ToInt32(curRow.GetCell(3).NumericCellValue)
                                });
                            }
                            //else if (curRow.Cells.Count == 3)
                            //{
                            //    description = AddLineBreaktoString(curRow.GetCell(2).StringCellValue);
                            //    ncode.Add(new CodeDescription()
                            //    {
                            //        precinct = last_precint,
                            //        reason_code = curRow.GetCell(1).StringCellValue,
                            //        description = description,
                            //        count = Convert.ToInt32(curRow.GetCell(3).NumericCellValue)
                            //    });
                            //}
                            //else
                            //{
                            //    errMsg += string.Format("The Row {0} has a problem\n", i + 1);
                            //}
                        }
                    }

                }
                catch (Exception ex)
                {
                    string msg = ex.GetType().FullName;
                    if (msg == "System.IO.IOException")
                        MessageBox.Show("The file is open by another process", "Error");
                    else if (msg == "CsvHelper.TypeConversion.TypeConverterException")
                    {
                        MessageBox.Show("The file format is invalid, Please check your csv file again.", "Error");
                    }
                    else if (msg == "CsvHelper.HeaderValidationException")
                    {
                        MessageBox.Show("CSV Header is not correct, please make sure you are using the correct CSV file", "Error");
                    }
                    else
                    {

                    }
                }

                if (!string.IsNullOrEmpty(errMsg))
                {
                    //MessageBox.Show(errMsg, "Error");
                }

                if (data.Count > 0)
                {
                    precinctRenderer.setChatData(data, ycode, ncode);
                    Render();
                }
                else
                {

                }

            }
        }


    }
}
