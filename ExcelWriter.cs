using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace DownloadTop100Apks
{
    class ExcelWriter
    {
        private Application oXL = null;
        private _Workbook oWB = null;
        private Workbooks oWBs = null;
        private Sheets oSheets = null;

        private string excelFileName = null;
        private bool excelFileReady = false;
        public static void cleanLastRun()
        {
            Process[] ps = Process.GetProcessesByName("EXCEL");
            foreach (Process p in ps)
            {
                string window = p.MainWindowTitle;
                if (window == null || window.Length <= 0)
                {
                    Log.info("Unwanted Excel process get killed pid" + p.Id);
                    p.Kill();
                }
            }
        }

        public static void deleteExcelFile(string fname)
        {
            cleanLastRun();
            if (File.Exists(fname))
            {
                File.Delete(fname);
            }
            else if (File.Exists(fname + ".xls"))
            {
                File.Delete(fname + ".xls");
            }
            else if (File.Exists(fname + ".xlsx"))
            {
                File.Delete(fname + ".xlsx");
            }
        }

        public ExcelWriter(string fileName, string[] sheetNames)
        {
            oXL = new Application();
            oXL.DisplayAlerts = true;
            oWBs = oXL.Workbooks;
            oWB = oWBs.Open(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                    + "\\DownloadTop100Apks.xlsx", ReadOnly: true);
            oSheets = oWB.Sheets;

            int sheetIndex = 1;
            foreach (string name in sheetNames)
            {
                Worksheet oSheet = null;
                try
                {
                    oSheet = oSheets[sheetIndex];
                }
                catch (Exception)
                {
                    Worksheet oSheet_indexed = oSheets[sheetIndex - 1];
                    oSheet = oSheets.Add(After: oSheet_indexed);
                    Marshal.FinalReleaseComObject(oSheet_indexed);
                }
                oSheet.Name = sheetNames[sheetIndex - 1];
                sheetIndex++;
                Marshal.FinalReleaseComObject(oSheet);
            }

            excelFileName = fileName;
            if (!excelFileName.EndsWith(".xlsx"))
            {
                excelFileName = excelFileName + ".xlsx";
            }
            if (!excelFileName.Contains('\\'))
            {
                excelFileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                    + "\\" + excelFileName;
            }

            if (File.Exists(excelFileName))
            {
                int i = 1;
                while (File.Exists(excelFileName + i + ".xlsx"))
                {
                    i++;
                }
                excelFileName = excelFileName + i + ".xlsx";
            }
            Log.info("Excel created " + excelFileName);
            excelFileReady = true;
        }

        public void setFormatAsDate(int sheet, int column)
        {
            Worksheet oSheet = oSheets[sheet];
            Range r = oSheet.Columns[column];
            r.NumberFormat = "mm/dd/yyyy";
            Marshal.FinalReleaseComObject(r);
            Marshal.FinalReleaseComObject(oSheet);
        }

        public void writeHeadRow(int sheet, string[] values)
        {
            writeRow(sheet, values, 1);
        }

        public void writeCell(int sheet, string value, int row, int column)
        {
            if (!excelFileReady)
            {
                return;
            }
            Worksheet oSheet = oSheets[sheet];
            Range r = oSheet.Cells[row, column];
            r.Value2 = value;
            Marshal.FinalReleaseComObject(r);
            Marshal.FinalReleaseComObject(oSheet);
        }

        public void writeRow(int sheet, string[] values, int row)
        {
            if (!excelFileReady)
            {
                return;
            }
            Worksheet oSheet = oSheets[sheet];
            Range r1 = oSheet.Cells[row, 1];
            Range r2 = oSheet.Cells[row, values.Length];
            Range r = oSheet.Range[r1, r2];

            r.Value2 = values;

            Marshal.FinalReleaseComObject(r);
            Marshal.FinalReleaseComObject(r1);
            Marshal.FinalReleaseComObject(r2);
            Marshal.FinalReleaseComObject(oSheet);
        }

        public void writeRows(int sheet, List<string[]> values, int startingRow)
        {
            if (!excelFileReady)
            {
                return;
            }

            setFormatAsDate(sheet, 16);

            int rowCount = values.Count;
            int columnCount = values.FirstOrDefault().Length;
            object[,] arr = new object[rowCount, columnCount];
            for (int row = 0; row < rowCount; row++)
            {
                string[] content = values[row];
                for (int c = 0; c < columnCount; c++)
                {
                    arr[row, c] = content[c];
                }
            }

            Worksheet oSheet = oSheets[sheet];
            Range r1 = oSheet.Cells[startingRow, 1];
            Range r2 = oSheet.Cells[startingRow + rowCount - 1, columnCount];

            Range r = oSheet.Range[r1, r2];
            r.Value2 = arr;

            Marshal.FinalReleaseComObject(r);
            Marshal.FinalReleaseComObject(r1);
            Marshal.FinalReleaseComObject(r2);
            Marshal.FinalReleaseComObject(oSheet);
        }

        public void saveAndClose()
        {
            excelFileReady = false;
            oWB.SaveAs(excelFileName);
            Marshal.FinalReleaseComObject(oSheets);
            Marshal.FinalReleaseComObject(oWB);
            oWBs.Close();
            Marshal.FinalReleaseComObject(oWBs);
            oXL.Quit();
            Marshal.FinalReleaseComObject(oXL);
            Log.info("Excel saved " + excelFileName);
        }
    }
}
