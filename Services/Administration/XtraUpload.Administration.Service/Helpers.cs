using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XtraUpload.Administration.Service.Common;

namespace XtraUpload.Administration.Service
{
    public static class AdministrationHelpers
    {
        public static IEnumerable<ItemCountResult> FormatResult(DateRangeModel range, List<ItemCountResult> items)
        {
            // Check if all days are included (even the days with 0 downloads)
            var totalDays = Math.Round((range.End - range.Start).TotalDays);
            if (items.Count < totalDays)
            {
                for (int i = 0; i <= totalDays; i++)
                {
                    if (items.FirstOrDefault(s => s.Date.Date.CompareTo(range.Start.AddDays(i).Date) == 0) == null)
                    {
                        items.Add(new ItemCountResult()
                        {
                            Date = range.Start.AddDays(i),
                            ItemCount = 0
                        });
                    }
                }

                items = items.OrderBy(s => s.Date).ToList();
            }
            return items;
        }
    }
}
