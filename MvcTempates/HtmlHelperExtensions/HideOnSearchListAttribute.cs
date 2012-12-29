using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MvcTempates.HtmlHelperExtensions
{
    [AttributeUsage(AttributeTargets.Property,Inherited = false, AllowMultiple=false)]
    public class HideOnSearchListAttribute:Attribute 
    {
        private bool _hide;

        public bool HideOnSearchlist
        {
            get
            {
                return _hide;
            }
        }

        
        public HideOnSearchListAttribute(bool hide)
        {
            _hide = hide;
        }


    }
}