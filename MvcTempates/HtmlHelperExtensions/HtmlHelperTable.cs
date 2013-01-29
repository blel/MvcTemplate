using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.ComponentModel.DataAnnotations;


namespace MvcTempates.HtmlHelperExtensions
{
    public static class HtmlHelperTable
    {
        public static MvcHtmlString Table<T>(this HtmlHelper helper, IEnumerable<T> items, string controller)
        {
            if (items != null && items.Count() > 0)
            {
                if (typeof(T).GetProperty("ID") == null)
                    throw new Exception("The type you want to create a table of does not contain a property \"ID\".");
                return new MvcHtmlString(GetSearchableTable<T>(items, GetPropertiesToShow<T>(), controller));
            }
            else
                return new MvcHtmlString( string.Empty);
        }

        private static string GetTable<T>(IEnumerable<T> items, PropertyInfo[] propertiesToShow, string controller)
        {
            TagBuilder table = new TagBuilder("table");
            table.AddCssClass("table-normal");
            StringBuilder sb = new StringBuilder();
            sb.Append(GetTableHeader(propertiesToShow));
            foreach (var listitem in items)
            {
                sb.Append(GetTableRow<T>(listitem, propertiesToShow, controller));
            }
            table.InnerHtml = sb.ToString();

            TagBuilder tbScript = new TagBuilder("script");
            tbScript.MergeAttribute("type", "text/javascript");
            tbScript.InnerHtml = "$(\".table-normal tr\").click(function () { window.location.href = \"/"
                                    +controller +"/Edit/\"+this.id;});";
            return table.ToString(TagRenderMode.Normal) + tbScript.ToString() ; 
        }

        private static string GetTableRow<T>(T listitem, PropertyInfo[] propertiesToShow, string controller)
        {
            string ID = typeof(T).GetProperty("ID").GetValue(listitem).ToString();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("\t<tr id={0}>", ID));
            foreach (var item in propertiesToShow)
            {
                TagBuilder td = new TagBuilder("td");

                td.SetInnerText(item.GetValue(listitem) != null ? item.GetValue(listitem).ToString() : string.Empty);
                sb.AppendLine("\t\t" + td.ToString());
            }
            TagBuilder actionTd = new TagBuilder("td");
            TagBuilder a = new TagBuilder("a");
            a.MergeAttribute("href", string.Format("{0}/Delete/{1}",controller, ID));
            a.SetInnerText("Delete");
            actionTd.InnerHtml = a.ToString();
            sb.AppendLine("\t\t" + actionTd.ToString());
            sb.AppendLine("\t</tr>");
            return sb.ToString();
        }

        private static string GetTableHeader(PropertyInfo[] propertiesToShow)
        {
            StringBuilder headerString = new StringBuilder();
            headerString.AppendLine("\t<tr>");
            foreach (var item in propertiesToShow)
            {

                TagBuilder tb = new TagBuilder("th");
                if (item.GetCustomAttribute<DisplayAttribute>() != null &&
                    item.GetCustomAttribute<DisplayAttribute>().GetName() != string.Empty)
                {
                    tb.SetInnerText(item.GetCustomAttribute<DisplayAttribute>().GetName());
                }
                else
                {
                    tb.InnerHtml = item.Name;
                }
                headerString.AppendLine("\t\t" + tb.ToString());



            }
            TagBuilder  tbAction = new TagBuilder("th");
            tbAction.SetInnerText("Actions");
            headerString.AppendLine("\t\t" + tbAction.ToString());
            headerString.AppendLine("\t</tr>");
            return headerString.ToString();
        }

        private static PropertyInfo[] GetPropertiesToShow<T>()
        {
          return  (from property in typeof(T).GetProperties()
             where (property.GetCustomAttribute<HideOnSearchListAttribute>() == null ||
                   !property.GetCustomAttribute<HideOnSearchListAttribute>().HideOnSearchlist)
             select property).ToArray();
        }

        private static string GetSearchableTable<T>(IEnumerable<T> items, PropertyInfo[] propertiesToShow, string controller)
        {
            StringBuilder html = new StringBuilder();
            html.AppendFormat("<form action=\"/{0}/Search\" data-ajax=\"true\" data-ajax-mode=\"replace\" data-ajax-update=\"#ajaxContent\" id=\"form0\" method=\"post\">",controller);
            html.AppendLine("<table>");
            html.AppendLine("\t<tr>");
            html.AppendLine("\t\t<th style=\"text-align:left;\">Max. Records</th>");
            html.AppendLine("\t\t<th style=\"text-align:left;\"> <input id=\"maxRecords\" value=\"100\" style=\"width:50px;\"/> </th>");
            html.AppendLine("\t\t<th style=\"text-align:right;\"> <input id=\"searchField\" name=\"txt\"/> </th>");
            html.AppendLine("\t\t<th style=\"text-align:right;\"> <input type=\"submit\" value=\"Search\"/> </th>");
            html.AppendLine("\t</tr>");
            html.AppendLine("\t<tr><td colspan=\"4\">");
            html.AppendLine("\t\t<div id = \"ajaxContent\">");
            html.Append(GetTable(items, propertiesToShow,controller));
            html.AppendLine("\t\t</div></td>");
            html.AppendLine("\t</tr>");
            html.AppendLine("</table></form>");
            return html.ToString();
        }

    }


}