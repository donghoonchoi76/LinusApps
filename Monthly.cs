using Microsoft.AspNetCore.Components.Forms;
using System.Text;
using ClosedXML.Excel;

public class MonthlyFile
{
    List<OutputRow> rows = new List<OutputRow>();

    public IBrowserFile? File { get; set; }
    public bool Selected { get; set; } = false;
    public List<OutputRow> Rows { get => rows; }

    private string GetText(string id, string text)
    {
        int start = text.IndexOf(string.Format("<{0}>", id));
        if (start >= 0)
        {
            start += (id.Length + 2);
            int end = text.IndexOf(string.Format("</{0}>", id), start);
            if (end < start)
                end = text.IndexOf("\n", start);

            return text.Substring(start, end - start).Trim();
        }

        return string.Empty;
    }
    public async Task<bool> ReadOfxFile(DateTime targetMonth)
    {
        try{
            Stream? s = File?.OpenReadStream();
            if (s is not null)
            {
                var reader = new StreamReader(s);
                var text = await reader.ReadToEndAsync();
                int start = text.IndexOf("<STMTTRN>");
                while (start >= 0)
                {
                    int end = text.IndexOf("</STMTTRN>", start);
                    string chunk = text.Substring(start, end - start);

                    string tmTxt = GetText("DTUSER", chunk);
                    if (string.IsNullOrWhiteSpace(tmTxt))
                        tmTxt = GetText("DTPOSTED", chunk);

                    if (!string.IsNullOrWhiteSpace(tmTxt))
                    {
                        int.TryParse(tmTxt.Substring(0, 4), out int year);
                        int.TryParse(tmTxt.Substring(4, 2), out int month);
                        int.TryParse(tmTxt.Substring(6, 2), out int day);
                        int.TryParse(tmTxt.Substring(8, 2), out int hour);
                        int.TryParse(tmTxt.Substring(10, 2), out int minute);
                        int.TryParse(tmTxt.Substring(12, 2), out int second);

                        DateTime tm = new DateTime(year, month, day, hour, minute, second);

                        if (tm.Month == targetMonth.Month && tm.Year == targetMonth.Year)
                        {
                            OutputRow row = new OutputRow();
                            row.Date = tm;

                            double.TryParse(GetText("TRNAMT", chunk), out double amount);
                            if (amount < 0)
                            {
                                amount *= -1.0;
                                row.Spend = amount.ToString("0.00");
                            }
                            else
                                row.Income = amount.ToString("0.00");

                            row.Desc = GetText("NAME", chunk);
                            rows.Add(row);
                        }
                    }
                    start = text.IndexOf("<STMTTRN>", end);
                }
            }
            return true;
        }
        catch
        {
            return false;
        }
    }
}

public class OutputRow
{
    public DateTime Date { get; set; }
    public string Desc { get; set; }
    public string Spend { get; set; }
    public string Income { get; set; }
    public string Category { get; set; }

    public OutputRow(string desc = "", string spend = "", string income = "")
    {
        Desc = desc;
        Spend = spend;
        Income = income;
        Category = string.Empty;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder(Date.ToShortDateString());
        sb.AppendFormat(",\"{0}\",{1},{2},\"{3}\"\r\n", Desc, Spend, Income, Category);
        return sb.ToString();
    }
}

class OutputRowComp : IComparer<OutputRow>
{
    public int Compare(OutputRow? x, OutputRow? y)
    {
        if( x is null || y is null)
            return 0;

        if (x.Date == y.Date)
            return 0;

        if (x.Date < y.Date)
            return -1;

        return 1;
    }
}

public static class ExcelService
{
    public static byte[] GenerateExcelWorkbook(List<OutputRow> rows)
    {
        using (var workbook = new XLWorkbook())
        {
            IXLWorksheet worksheet = workbook.Worksheets.Add("Monthly");
    
            int index = 1;
            foreach (var r in rows)
            {
                worksheet.Cell(index, 1).Value = r.Date.ToShortDateString();
                worksheet.Cell(index, 2).Value = r.Desc;
                worksheet.Cell(index, 3).Value = r.Spend;
                worksheet.Cell(index, 4).Value = r.Income;    
                index++;
            }
    
            using var stream = new MemoryStream();
            stream.Position = 0;
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}