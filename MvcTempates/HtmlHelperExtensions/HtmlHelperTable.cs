using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using PagedList;
using PagedList.Mvc;


namespace MvcTempates.HtmlHelperExtensions
{
    public static class HtmlHelperTable
    {
        /// <summary>
        /// This is the helper which can be inserted in the view.
        /// The list is based on the paged list which can be obtained via nuget.
        /// Also, it needs some basic css styles- currently found in Site.css
        /// and jquery. 
        /// Unobtrusive javascript is also required as otherwise the MVC ajax functions will not work.
        /// Known bugs and open items:
        /// - pressing the table header will also call a details form, which will not work
        /// - ordering is not supported yet
        /// - resizing is not supported yet
        /// - column ordering is not supported yet
        /// </summary>
        /// <typeparam name="T">The type of which the list shall be created</typeparam>
        /// <param name="helper">Extension</param>
        /// <param name="items">The items of which the list shall be created for</param>
        /// <param name="controller"></param>
        /// <returns>Html String containing the paged list</returns>
        public static MvcHtmlString TableWithSearchField<T>(this System.Web.Mvc.HtmlHelper helper, IEnumerable<T> items, string controller)
        {
            if (items != null && items.Count() > 0)
            {
                if (typeof(T).GetProperty("ID") == null)
                    throw new Exception("The type you want to create a table of does not contain a property \"ID\".");
                return new MvcHtmlString(GetSearchableTable<T>(helper, items, GetPropertiesToShow<T>(), controller));
            }
            else
                return new MvcHtmlString( string.Empty);
        }

        /// <summary>
        /// Html helper table without the search fields.
        /// </summary>
        /// <typeparam name="T">The type of which the list shall be created. The type must
        /// contain a column ID, otherwise an error is thrown.</typeparam>
        /// <param name="helper">Extension</param>
        /// <param name="items">The items</param>
        /// <param name="controller">the controller, needed for the details link</param>
        /// <returns></returns>
        public static MvcHtmlString Table<T>(this System.Web.Mvc.HtmlHelper helper, IEnumerable<T> items, string controller)
        {
            if (items != null && items.Count() > 0)
            {
                if (typeof(T).GetProperty("ID") == null)
                    throw new Exception("The type you want to create a table of does not contain a property \"ID\".");
                return new MvcHtmlString(GetTable<T>(items, GetPropertiesToShow<T>(), controller));
            }
            else
                return new MvcHtmlString(string.Empty);
        }

        /// <summary>
        /// Returns the table 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="propertiesToShow"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
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
            //Script that calls the edit screen if the current tr has a valid id
            TagBuilder tbScript = new TagBuilder("script");
            tbScript.MergeAttribute("type", "text/javascript");
            tbScript.InnerHtml = "$(\".table-normal tr\").click(function () { if (this.id != null && this.id !=\"\") { window.location.href = \"/"
                                    +controller +"/Edit/\"+this.id;}});";
            //Script for sorting order
            TagBuilder sortingOrderScript = new TagBuilder("script");
            sortingOrderScript.MergeAttribute("type", "text/javascript");
            sortingOrderScript.InnerHtml = @"$("".table-normal tr th"").click(function () 
                                                {
                                                    if (this.id != null && this.id !="""")
                                                    {
                                                        
                                                        $(""#ajaxContent"").html(""<p>nothing</p>"").load(""/" + controller + string.Format("/Refresh?TableAction={0}&SortedColumn={1}&SortDirection={2}", TableActions.Sort, "\"+this.id+\"", SortDirection.Ascending)
                                                                                                                 + "\");}});";
            return table.ToString(TagRenderMode.Normal) + tbScript.ToString() + sortingOrderScript.ToString() ; 
        }

        /// <summary>
        ///  Returns a table row
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listitem"></param>
        /// <param name="propertiesToShow"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns the table header 
        /// </summary>
        /// <param name="propertiesToShow"></param>
        /// <returns></returns>
        private static string GetTableHeader(PropertyInfo[] propertiesToShow)
        {
            StringBuilder headerString = new StringBuilder();
            headerString.AppendLine("\t<tr>");
            foreach (var item in propertiesToShow)
            {
                TagBuilder tb = new TagBuilder("th");
                //used for sorting
                tb.Attributes.Add(new KeyValuePair<string,string>("id", item.Name));
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

        /// <summary>
        /// Evaluates the properties which shall be displayed. If a column contains the attribute 
        /// "HideOnSearchList" the column is suppressed in the output.
        /// </summary>
        /// <typeparam name="T">The type whose items shall be displayed</typeparam>
        /// <returns>Array of PropertyInfo</returns>
        private static PropertyInfo[] GetPropertiesToShow<T>()
        {
          return  (from property in typeof(T).GetProperties()
             where (property.GetCustomAttribute<HideOnSearchListAttribute>() == null ||
                   !property.GetCustomAttribute<HideOnSearchListAttribute>().HideOnSearchlist)
             select property).ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="helper"></param>
        /// <param name="items"></param>
        /// <param name="propertiesToShow"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        private static string GetSearchableTable<T>(System.Web.Mvc.HtmlHelper helper, IEnumerable<T> items, PropertyInfo[] propertiesToShow, string controller)
        {
            StringBuilder html = new StringBuilder();
            html.AppendFormat("<form action=\"/{0}/Refresh\" data-ajax=\"true\" data-ajax-mode=\"replace\" data-ajax-update=\"#ajaxContent\" id=\"form0\" method=\"post\">",controller);
            html.AppendLine("<table>");
            html.AppendLine("\t<tr>");
            html.AppendLine("\t\t<th style=\"text-align:left;\">Max. Records</th>");
            html.AppendLine("\t\t<th style=\"text-align:left;\"> <input id=\"maxRecords\" value=\"100\" style=\"width:50px;\"/> </th>");
            html.AppendLine("\t\t<th style=\"text-align:right;\"> <input id=\"searchField\" name=\"SearchText\"/> </th>");
            //add hidden field to set the TableAction = Search when Searchbutton is clicked
            html.AppendLine("\t\t<input type=\"hidden\" name=\"TableAction\" value = \"Search\">");
            html.AppendLine("\t\t<th style=\"text-align:right;\"> <input type=\"submit\" value=\"Search\"/> </th>");
            html.AppendLine("\t</tr>");
            html.AppendLine("\t<tr><td colspan=\"4\">");
            html.AppendLine("\t\t<div id = \"ajaxContent\">");
            html.Append(GetTable(items, propertiesToShow,controller));
            html.Append(helper.PagedListPager((IPagedList)items, selectedPage => (string.Format("{0}/Refresh?RequestedPage={1}&SearchText={2}&TableAction={3}",
                controller, selectedPage, new ViewDataDictionary ().Eval("searchField"), TableActions.Page)), PagedListRenderOptions.EnableUnobtrusiveAjaxReplacing("#ajaxContent"))); 
            html.AppendLine("\t\t</div></td>");
            html.AppendLine("\t</tr>");
            html.AppendLine("</table></form>");
            
            
            return html.ToString();
        }

    }

    public enum TableActions
    {
        Search, Page, Sort
    }

    public enum SortDirection
    {
        Ascending, Descending
    }

}