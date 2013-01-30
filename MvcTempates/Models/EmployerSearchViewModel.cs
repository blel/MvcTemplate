using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcTempates.Models
{
    public class EmployerSearchViewModel
    {
        public IEnumerable<Employer> ResultSet { get; set; }
        public string SearchText { get; set; }
        public int MaxSearchResult { get; set; }
        public int RequestedPage { get; set; }
        public string SortedColumn { get; set; }
        public HtmlHelperExtensions.SortDirection SortDirection { get; set; }
        public HtmlHelperExtensions.TableActions TableAction { get; set; }


    }
}