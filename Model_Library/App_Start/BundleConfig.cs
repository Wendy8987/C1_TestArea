using System.Web;
using System.Web.Optimization;

namespace Model_Library
{
    public class BundleConfig
    {
        // 有关捆绑的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备就绪，请使用 https://modernizr.com 上的生成工具仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));


            // Custom 
            bundles.Add(new ScriptBundle("~/bundles/nicepage").Include(
                        "~/Scripts/nicepage.js",
                        "~/Scripts/header.js"));

            bundles.Add(new ScriptBundle("~/bundles/Download").Include(
                        "~/Scripts/DownloadBtn.js"));

            bundles.Add(new StyleBundle("~/Content/Login").Include(
                      "~/Content/Login.css"));

            bundles.Add(new StyleBundle("~/Content/SignUp").Include(
                      "~/Content/SignUp.css"));

            bundles.Add(new StyleBundle("~/Content/Home").Include(
                      "~/Content/Home.css"));

            bundles.Add(new StyleBundle("~/Content/Category").Include(
                      "~/Content/category.css"));

            bundles.Add(new StyleBundle("~/Content/ObjectDetail").Include(
                     "~/Content/ObjectDetail.css"));

            bundles.Add(new StyleBundle("~/Content/Profile").Include(
                     "~/Content/Profile.css"));

            bundles.Add(new StyleBundle("~/Content/nicepage").Include(
                     "~/Content/nicepage.css"));

            bundles.Add(new StyleBundle("~/Content/UploadObject").Include(
                     "~/Content/UploadObject.css"));
        }
    }
}
