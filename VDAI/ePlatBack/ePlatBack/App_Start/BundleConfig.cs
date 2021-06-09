using System.Web;
using System.Web.Optimization;

namespace ePlatBack
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        //"~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery-ui-timepicker-addon.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/eplat").Include(
                        "~/Scripts/jquery.easing.1.3.js",
                        "~/Scripts/eplatback-utils.js",
                        "~/Scripts/layout/ui.js",
                        "~/Scripts/layout/jquery-multi-nav.js",
                        //"~/Scripts/jquery.blockUI.js",
                        "~/Scripts/Settings.js",
                        "~/Scripts/Utils.js",
                        //"~/Scripts/jquery.dataTables.js",
                        "~/Scripts/jquery.numeric.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/logon").Include(
                "~/Scripts/account/logon.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            

            bundles.Add(new StyleBundle("~/Content/css").Include(
                //"~/Content/site.css",
                "~/Content/multi-nav.css",
                "~/Content/jquery.dataTables.css"
                ));

            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css"
                        ));

            bundles.Add(new StyleBundle("~/Content/ui-custom").Include(
                        "~/Content/jquery-ui-1.8.14.custom.css",
                        "~/Content/jquery-ui-timepicker-addon.css"
                        ));
            //BundleTable.EnableOptimizations = false;
        }
    }
}