﻿using System.Web;
using System.Web.Optimization;

namespace CampanasDelDesierto_v1
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            /*SCRIPTS BUNDLES*/
            bundles.Add(new ScriptBundle("~/bundles/template").Include(
                        "~/Scripts/skel.min.js",
                        "~/Scripts/util.js",
                        "~/Scripts/main.js",
                        "~/Scripts/customFunctions.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery.mask.js",
                        "~/Scripts/numeral.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/dataTables").Include(
                        "~/Scripts/jquery.dataTables.min.js",
                        "~/Scripts/dataTables.buttons.min.js",
                        "~/Scripts/dataTables.select.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            /*STYLE BUNDLES*/
            bundles.Add(new StyleBundle("~/Content/template").Include(
                      "~/Content/main.css",
                      "~/Content/bootstrap.css",
                      "~/Content/Site.css",
                      "~/Content/jquery-ui.css"));

            bundles.Add(new StyleBundle("~/Content/icons").Include(
                      "~/Content/font-awesome.min.css",
                      "~/Content/icomoon.css"));
            
            bundles.Add(new StyleBundle("~/Content/dataTables").Include(
                      "~/Content/jquery.dataTables.min.css",
                      "~/Content/buttons.dataTables.min.css",
                      "~/Content/select.dataTables.min.css"));
        }
    }
}
