using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace MvcTempates.HtmlHelperExtensions
{
    public static class HtmlHelperTable
    {
        public static MvcHtmlString Table<T>(this HtmlHelper helper, IEnumerable<T> items)
        {

            


            StringBuilder sb = new StringBuilder();
            sb.Append("<table><tr>");
            foreach (var item in typeof(T).GetProperties())
            {
                TagBuilder tb = new TagBuilder("TH");
                tb.InnerHtml = item.Name;
                sb.Append(tb.ToString());
            }
            sb.Append("</tr></table>");
            return new MvcHtmlString(sb.ToString()); 
        }

  

    }


}