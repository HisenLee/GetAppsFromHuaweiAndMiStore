using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace DownloadTop100Apks
{
    class ExcelReader
    {
        private Application oXL = null;
        private _Workbook oWB = null;
        private Workbooks oWBs = null;
        private _Worksheet oSheet = null;

        private string excelFileName = null;

        private bool excelFileReady = false;

        public ExcelReader(string fileName)
        {
            excelFileReady = false;
            excelFileName = fileName;
            if (!excelFileName.Contains("\\"))
            {
                excelFileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + excelFileName;
            }
            if (!File.Exists(excelFileName))
            {
                Log.info("Excel file not found! " + excelFileName);
                return;
            }

            oXL = new Application();
            oXL.DisplayAlerts = true;
            oWBs = oXL.Workbooks;
            oWB = oWBs.Open(excelFileName, ReadOnly: true);
            oSheet = oWB.Sheets[1];
            oSheet.EnableAutoFilter = false;
            oSheet.EnableCalculation = false;
            oSheet.EnableFormatConditionsCalculation = false;
            oSheet.EnableOutlining = false;
            oSheet.EnablePivotTable = false;

            excelFileReady = true;
        }

        public object[,] readAll()
        {
            if (!excelFileReady)
            {
                Log.info("Excel file not ready! " + excelFileName);
                return null;
            }

            Range allRange = oSheet.UsedRange;
            Range rows = allRange.Rows;
            Range columns = allRange.Columns;
            object[,] arr = new object[rows.Count, columns.Count];
            arr = allRange.Value2;

            Marshal.FinalReleaseComObject(columns);
            Marshal.FinalReleaseComObject(rows);
            Marshal.FinalReleaseComObject(allRange);
            return arr;
        }

        public void close()
        {
            excelFileReady = false;
            Marshal.FinalReleaseComObject(oSheet);
            oWBs.Close();
            Marshal.FinalReleaseComObject(oWBs);
            oXL.Quit();
            Marshal.FinalReleaseComObject(oXL);
            Log.info("Excel saved " + excelFileName);
        }
    }
}
