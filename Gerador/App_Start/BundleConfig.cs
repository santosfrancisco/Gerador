using System.Collections.Generic;
using System.Web.Optimization;

namespace IdentitySample
{
	public class BundleConfig
	{
		// For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
		public static void RegisterBundles(BundleCollection bundles)
		{
			bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
						"~/Scripts/jquery-{version}.js"));

			//      bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
			//"~/Scripts/jquery.validate.js",
			//"~/Scripts/jquery.validate.unobtrusive.js",
			//"~/Scripts/globalize.js",
			//"~/Scripts/jquery.validate.globalize.min.js",
			//"~/Scripts/jquery-3.1.0.min.js",
			//"~/Scripts/masks.js",
			//"~/Scripts/jquery-confirm.min.js",
			//"~/Scripts/jquery-ui-1.12.1.js",
			//"~/Scripts/jquery.mask.js"));

			var bundle = new ScriptBundle("~/bundles/jqueryval") { Orderer = new AsIsBundleOrderer() };

			bundle
				.Include("~/Scripts/jquery.validate-vsdoc.js")
				.Include("~/Scripts/jquery.validate.js")
				.Include("~/Scripts/jquery.validate.unobstrusive.js")
				.Include("~/Scripts/globalize.js")
				.Include("~/Scripts/jquery.validate.globalize.js")
			.Include("~/Scripts/jquery.mask.js")
			.Include("~/Scripts/masks.js")
			.Include("~/Scripts/jquery-ui-1.12.1.js")
			.Include("~/Scripts/jquery-confirm.min.js");
			bundles.Add(bundle);


			// Use the development version of Modernizr to develop with and learn from. Then, when you're
			// ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
			bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
						"~/Scripts/modernizr-*"));

			bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
					  "~/Scripts/bootstrap.js",
					  "~/Scripts/respond.js"));

			bundles.Add(new StyleBundle("~/Content/css").Include(
						"~/Content/bootstrap.css",
					  //"~/Content/YetiTheme.min.css",
					  "~/Content/jquery-confirm.css",
					  "~/Content/site.css",
					  "~/Content/animate.css"));
		}
	}

	public class AsIsBundleOrderer : IBundleOrderer
	{
		public IEnumerable<BundleFile> OrderFiles(BundleContext context, IEnumerable<BundleFile> files)
		{
			return files;
		}
	}
}
