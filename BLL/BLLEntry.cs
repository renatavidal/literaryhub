using System;
using System.Collections.Generic;
using System.Data;
using MPP;

public class BLLEntry
{
    public DateTime PeriodStart { get; set; }
    public string Label { get; set; }
    public decimal Amount { get; set; }
    public int Count { get; set; }
}

public class BLLRevenue
{
    private readonly MPPRevenue _mpp = new MPPRevenue();

    public List<BLLEntry> List(string groupBy, DateTime? fromUtc, DateTime? toUtc, string currency)
    {
        var dt = _mpp.SumBy(groupBy, fromUtc, toUtc, currency);
        var list = new List<BLLEntry>();
        foreach (DataRow r in dt.Rows)
        {
            list.Add(new BLLEntry
            {
                PeriodStart = (DateTime)r["PeriodStart"],
                Label = Convert.ToString(r["Label"]),
                Amount = Convert.ToDecimal(r["Amount"]),
                Count = Convert.ToInt32(r["Count"])
            });
        }
        return list;
    }
}
