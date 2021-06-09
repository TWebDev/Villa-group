using System.Web;
using System.Web.Optimization;

namespace ePlatFront
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive-ajax.js",
                        "~/Scripts/jquery.validate.js",
                        "~/Scripts/jquery.validate.unobtrusive.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

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
                        "~/Content/themes/base/jquery.ui.theme.css"));

            bundles.Add(new StyleBundle("~/Content/themes/base/datepicker").Include(
                "~/Content/themes/base/jquery.ui.core.css",
                "~/Content/themes/base/jquery.ui.datepicker.css",
                "~/Content/themes/base/jquery-ui-1.8.14.custom.css",
                "~/Content/themes/base/jquery.ui.theme.css"
                ));

            /*Bundles by project*/
            /*DVH*/
            bundles.Add(new StyleBundle("~/themes/dvh").Include(
                "~/Content/themes/dvh/css/main.css",
                "~/Content/plugins/fancybox/jquery.fancybox.css"
                ));

            bundles.Add(new ScriptBundle("~/bundles/dvh").Include(
                        "~/Scripts/jquery.easing.1.3.min.js",
                        "~/Scripts/jquery.ui.touch-punch.min.js",
                        "~/Scripts/jquery.ba-throttle-debounce.min.js",
                        "~/Content/plugins/fancybox/jquery.fancybox.pack.js",
                        "~/Scripts/controls.js",
                        "~/Content/themes/dvh/js/dvh-ui.js",
                        "~/Content/plugins/specialpromo/specialpromo.js"));

            bundles.Add(new ScriptBundle("~/bundles/dvh/slider").Include(
                "~/Content/themes/dvh/js/dvh-slider.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/dvh/detail").Include(
                "~/Content/themes/dvh/js/package-details.js"
                ));

            /*PVO*/
            bundles.Add(new StyleBundle("~/themes/pvo").Include(
                "~/Content/themes/pvo/css/main.css",
                "~/Content/plugins/fancybox/jquery.fancybox.css"
                ));

            bundles.Add(new ScriptBundle("~/bundles/pvo").Include(
                        "~/Scripts/jquery.easing.1.3.min.js",
                        "~/Scripts/jquery.ui.touch-punch.min.js",
                        "~/Scripts/jquery.ba-throttle-debounce.min.js",
                        "~/Content/plugins/fancybox/jquery.fancybox.pack.js",
                        "~/Scripts/controls.js",
                        "~/Content/themes/pvo/js/pvo-ui.js",
                        "~/Content/plugins/specialpromo/specialpromo.js"));

            bundles.Add(new ScriptBundle("~/bundles/pvo/slider").Include(
                "~/Content/themes/pvo/js/pvo-slider.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/pvo/detail").Include(
                "~/Content/themes/pvo/js/package-details.js"
                ));

            /*TGA*/
            bundles.Add(new StyleBundle("~/themes/tga").Include(
                "~/Content/themes/tga/css/main.css",
                "~/Content/plugins/fancybox/jquery.fancybox.css"
                ));

            /*VMS*/
            bundles.Add(new StyleBundle("~/themes/vms").Include(
                "~/Content/themes/vms/css/main.css",
                "~/Content/plugins/fancybox/jquery.fancybox.css"
                ));

            /*MEX*/
            bundles.Add(new ScriptBundle("~/bundles/mex").Include(
                        "~/Scripts/jquery.easing.1.3.min.js",
                        "~/Scripts/jquery.ui.touch-punch.min.js",
                        "~/Scripts/jquery-ui-timepicker-addon.js",
                        "~/Scripts/jquery.ba-throttle-debounce.min.js",
                        "~/Content/plugins/hashchange/jquery.hashchange.min.js",
                        "~/Content/plugins/fancybox/jquery.fancybox.pack.js",
                        "~/Scripts/jquery.json-2.3.min.js",
                        "~/Scripts/controls.js",
                        "~/Scripts/bookingengine-1.0.js",
                        "~/Content/themes/mex/js/ui.js"
                        ));

            bundles.Add(new StyleBundle("~/themes/mex").Include(
                "~/Content/themes/mex/css/main.css",
                "~/Content/plugins/fancybox/jquery.fancybox.css",
                "~/Content/themes/base/jquery-ui-timepicker-addon.css"
                ));
        }
    }
}