using System;
using System.Web.Mvc.Html;
using System.Web.Mvc;
using System.Web.Optimization;

namespace StaticEmbedded
{
    public static class HtmlHelpers
    {
        public static MvcHtmlString NewTextBox(this HtmlHelper html, string name)
        {
            var js = Scripts.Render("~/StaticEmbedded/Embedded/Js").ToString();
            var css = Styles.Render("~/StaticEmbedded/Embedded/Css").ToString();
            var textbox = html.TextBox(name).ToString();
            return MvcHtmlString.Create(textbox + js + css);
        }
        public static MvcHtmlString UseStaticEmbedded(this HtmlHelper html)
        { 
            var js = Scripts.Render("~/StaticEmbedded/Embedded/Js").ToString();
            var css = Styles.Render("~/StaticEmbedded/Embedded/Css").ToString();
            return MvcHtmlString.Create( js + css);
        }
    }
}
