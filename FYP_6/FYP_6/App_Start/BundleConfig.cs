using System.Web;
using System.Web.Optimization;

namespace FYP_6
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            //Home Bundle
            bundles.Add(new ScriptBundle("~/bundles/homeScript").Include(
                      "~/B-School-Free-Education-HTML5-Website-Template/js/modernizr-latest.js",
                      "~/B-School-Free-Education-HTML5-Website-Template/js/jquery-1.8.2.min.js",
                      "~/B-School-Free-Education-HTML5-Website-Template/js/bootstrap.min.js",
                      "~/B-School-Free-Education-HTML5-Website-Template/js/jquery.isotope.min.js"
                      , "~/B-School-Free-Education-HTML5-Website-Template/js/fancybox/jquery.fancybox.pack.js"
                      , "~/B-School-Free-Education-HTML5-Website-Template/js/jquery.nav.js"
                      , "~/B-School-Free-Education-HTML5-Website-Template/js/jquery.fittext.js"
                      , "~/B-School-Free-Education-HTML5-Website-Template/js/waypoints.js"
                      , "~/B-School-Free-Education-HTML5-Website-Template/js/waypoints.js"
                      , "~/B-School-Free-Education-HTML5-Website-Template/js/custom.js"
                      , "~/B-School-Free-Education-HTML5-Website-Template/js/owl-carousel/owl.carousel.js"));

            bundles.Add(new StyleBundle("~/Content/homeCSS").Include(
                      "~/B-School-Free-Education-HTML5-Website-Template/css/bootstrap.min.css",
                      "~/B-School-Free-Education-HTML5-Website-Template/css/isotope.css",
                      "~/B-School-Free-Education-HTML5-Website-Template/js/fancybox/jquery.fancybox.css",
                      "~/B-School-Free-Education-HTML5-Website-Template/js/owl-carousel/owl.carousel.css",
                      "~/B-School-Free-Education-HTML5-Website-Template/font/css/font-awesome.min.css"));

            //Login Bundles
            bundles.Add(new ScriptBundle("~/Scripts/login").Include(
          "~/js/jquery-1.11.1.min.js",
          "~/js/bootstrap.min.js",
          "~/js/chart.min.js",
          //"~/js/chart-data.js",
          //"~/js/easypiechart.js",
          //"~/js/easypiechart-data.js",
          "~/js/bootstrap-datepicker.js",
          "~/Scripts/SideBarScript.js",
          "~/Scripts/dropdownValueSave.js",
          "~/js/lumino.glyphs.js"));

            bundles.Add(new StyleBundle("~/Scripts/login1").Include(
                      "~/css/PictureResponsiveStyles.css",
                      "~/css/bootstrap.min.css",
                      "~/css/datepicker3.css",
                      "~/css/styles.css"));

            //Employees Bundle:

            bundles.Add(new ScriptBundle("~/Scripts/EmpScript").Include(
          "~/Scripts/SideBarScript.js",
          "~/Scripts/SHowPass.js",
          "~/MainStudentInfoTheme/js/jquery-1.11.1.min.js",
          "~/Scripts/jquery.validate.min.js",
          "~/Scripts/jquery.validate.unobtrusive.min.js",
          "~/MainStudentInfoTheme/js/bootstrap.min.js",
          "~/MainStudentInfoTheme/js/bootstrap-datepicker.js",
          "~/MainStudentInfoTheme/js/RunDatePicker.js",
          "~/Scripts/DegreeBatchGetterFile.js",
          "~/Scripts/CameraImageUpload.js",
          "~/Scripts/RowSelectCheckbox.js",
          "~/Scripts/jquery-ui.js",
          "~/Scripts/IntellisenseForTeacherID.js",
          "~/MainStudentInfoTheme/js/lumino.glyphs.js",
          "~/Scripts/TableScript.js",
          "~/Scripts/OnJSDisableDeleteDisable.js",
          "~/Scripts/DeletionConformance.js",
          "~/Scripts/TeacherGetBatchesWhenAssigningBatch.js"
          , "~/Scripts/TeacherSubjectsAssignSubjects.js", "~/Scripts/DomicileCities.js"
          , "~/Scripts/CollectFeeScript.js"
          ));

            bundles.Add(new StyleBundle("~/css/EmpStyles").Include(
                      "~/MainStudentInfoTheme/css/bootstrap.css",
                      "~/MainStudentInfoTheme/css/bootstrap.min.css",
                      "~/MainStudentInfoTheme/css/datepicker3.css",
                      "~/MainStudentInfoTheme/css/styles.css",
                      "~/css/PictureResponsiveStyles.css",
                      "~/css/jquery-ui.css"));

            //Student Bundles:
            bundles.Add(new ScriptBundle("~/Scripts/StdScripts").Include(
                      "~/Scripts/TableScript.js",
                      "~/Scripts/SideBarScript.js",
                      "~/Scripts/SHowPass.js",
                      "~/MainStudentInfoTheme/js/jquery-1.11.1.min.js",
                      "~/Scripts/jquery.validate.min.js",
                      "~/Scripts/jquery.validate.unobtrusive.min.js",
                      "~/MainStudentInfoTheme/js/bootstrap.min.js",
                      "~/MainStudentInfoTheme/js/chart.min.js",
                      "~/MainStudentInfoTheme/js/chart-data.js"
                      , "~/MainStudentInfoTheme/js/easypiechart-data.js",
                      "~/MainStudentInfoTheme/js/bootstrap-datepicker.js",
                      "~/MainStudentInfoTheme/js/lumino.glyphs.js"));

            bundles.Add(new StyleBundle("~/MainStudentInfoTheme/css/StdStyles").Include(
          "~/MainStudentInfoTheme/css/bootstrap.min.css",
          "~/MainStudentInfoTheme/css/datepicker3.css",
          "~/MainStudentInfoTheme/css/styles.css",
          "~/css/PictureResponsiveStyles.css"));

            //Admin Bundles

            bundles.Add(new ScriptBundle("~/Scripts/AdminViewScript").Include(
                "~/Scripts/SideBarScript.js",
          "~/Scripts/SHowPass.js",
          "~/MainStudentInfoTheme/js/jquery-1.11.1.min.js",
          "~/Scripts/jquery.validate.min.js",
          "~/Scripts/jquery.validate.unobtrusive.min.js",
          "~/MainStudentInfoTheme/js/bootstrap.min.js",
          "~/MainStudentInfoTheme/js/bootstrap-datepicker.js",
          "~/MainStudentInfoTheme/js/RunDatePicker.js",
          "~/Scripts/DegreeBatchAdmin.js",
          "~/Scripts/CameraImageUploadAdmin.js",
          "~/Scripts/RowSelectCheckbox.js",
          "~/Scripts/jquery-ui.js",
          "~/Scripts/IntellisenseForTeacherID.js",
          "~/MainStudentInfoTheme/js/lumino.glyphs.js",
          "~/Scripts/TableScript.js",
          "~/Scripts/OnJSDisableDeleteDisable.js",
          "~/Scripts/DeletionConformance.js"
                ));



            bundles.Add(new StyleBundle("~/MainStudentInfoTheme/css/AdminViewStyle1").Include(
                      "~/MainStudentInfoTheme/css/bootstrap.css",
                      "~/MainStudentInfoTheme/css/bootstrap.min.css",
                      "~/MainStudentInfoTheme/css/datepicker3.css",
                      "~/MainStudentInfoTheme/css/styles.css",
                      "~/css/PictureResponsiveStyles.css",
                      "~/css/jquery-ui.css"));

            //Teacher Bundles
            bundles.Add(new ScriptBundle("~/Scripts/TeacherScript").Include(
                          "~/Scripts/DegreeBatchTeacherJS.js",
          "~/Scripts/SideBarScript.js",
          "~/Scripts/SHowPass.js",
          "~/Scripts/TableScript.js",
          "~/MainStudentInfoTheme/js/jquery-1.11.1.min.js",
          "~/Scripts/jquery.validate.min.js",
          "~/Scripts/jquery.validate.unobtrusive.min.js",
          "~/MainStudentInfoTheme/js/bootstrap.min.js",
          "~/MainStudentInfoTheme/js/bootstrap-datepicker.js",
          "~/MainStudentInfoTheme/js/RunDatePicker.js",
          "~/MainStudentInfoTheme/js/lumino.glyphs.js",
           "~/Scripts/EditAtt_ChangePercentage.js",
          "~/Scripts/EditMarks_ChangePercentage.js"
          , "~/Scripts/RollNoBatchGet.js"
          , "~/Scripts/TeacherAttendanceUpload.js"
          ));

            bundles.Add(new StyleBundle("~/css/TeacherStyles").Include(
                      "~/MainStudentInfoTheme/css/bootstrap.css",
                      "~/MainStudentInfoTheme/css/bootstrap.min.css",
                      "~/MainStudentInfoTheme/css/datepicker3.css",
                      "~/MainStudentInfoTheme/css/styles.css",
                      "~/css/PictureResponsiveStyles.css"));

        }
    }
}
