using System.Web;
using System.Web.Optimization;

namespace RemoteSensingProject
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/vendorScripts").Include(
                "~/assets/vendors/chartjs/Chart.min.js",
                "~/assets/vendors/jquery.flot/jquery.flot.js",
                "~/assets/vendors/jquery.flot/jquery.flot.resize.js",
                "~/assets/vendors/bootstrap-datepicker/bootstrap-datepicker.min.js",
                "~/assets/vendors/apexcharts/apexcharts.min.js",
                "~/assets/js/sweetalert.min.js",
                "~/assets/vendors/feather-icons/feather.min.js",
                "~/assets/js/template.js",
                "~/assets/Summernotes/summernote.min.js",
                "~/assets/js/datepicker.js",
                "~/assets/datatable/datatables.min.js",
                "~/assets/vendors/select2/select2.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
            "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));


            bundles.Add(new StyleBundle("~/Content/vendorCss").Include(
                "~/assets/vendors/core/core.css",
                "~/assets/css/style.css",
                "~/assets/css/SweatAlert.min.css",
                "~/assets/vendors/bootstrap-datepicker/bootstrap-datepicker.min.css",
                "~/assets/vendors/select2/select2.min.css",
                "~/assets/datatable/datatables.min.css",
                "~/assets/Summernotes/summernote.min.css",
                "~/assets/fonts/feather-font/css/iconfont.css"
            ));

        }
    }
}
