$(document).ready(function ()
{
    $("tr").on("click", function () {
        
        //alert($(this).attr("class"));
        
        //var checked = $(this).attr("class").is(":checked");
        var className = $(this).attr("class");
        var checked = $("."+className).is(":checked");
        if (checked)
        {
            $("." + className).prop('checked', false);
        }
        else
        {
            
            $("." + className).prop('checked', true);
        }
    });
    $("tr").hover(function () {
        $("tr").css("cursor", "pointer");
    });
    

    $(".checkUncheckAll").change(function() {
        var checked = $(".checkUncheckAll").is(":checked");
        //alert(checked);
        var getRows=$('table>tbody>tr').length;
        if (checked)
        {
            for (var i = 1; i <= getRows ; i++)
            {
                $(".roo_" + i).prop('checked', true);   
            }
        }
        else
        {
            for (var i = 1; i <= getRows; i++)
            {
                $(".roo_" + i).prop('checked', false);
            }
        }
    });
    $(".checkUncheckAll2").change(function ()
    {
        var checked = $(".checkUncheckAll2").is(":checked");
        //alert(checked);
        var getRows = $('.tbodyInsideForBatchesOfTeacher>tr').length;
        //alert(getRows);
        if (checked) {
            for (var i = 1; i <= getRows ; i++) {
                $(".roowa_" + i).prop('checked', true);
                
                //alert($(".roowa_" + i).is(":checked"));
            }
        }
        else {
            for (var i = 1; i <= getRows; i++) {
                $(".roowa_" + i).prop('checked', false);
                //alert($(".roowa_" + i).is(":checked"));
                
            }
        }
    });
    $('tr input[type=checkbox]').click(function (e) {
        e.stopPropagation();
    });
    $("article").mouseenter(function ()
    {
        var idName = $(this).attr("id");
        //$(function () { $("[data-toggle = 'tooltip']").tooltip(); });
        //$("#" + idName).attr('title', 'This is the hover-over text');
        var options = {};
        options.url = "/Home/GetTeacherSubjectsRelatedToTBID";
        options.type = "POST";
        options.data = JSON.stringify({ id: idName });
        options.dataType = "json";
        options.contentType = "application/json";
        options.success = function (result) {
            var htmlBnaRhahu = "";
            //$('[data-toggle="tooltip"]').tooltip({ 'placement': 'top', 'title': '' });
            //alert(result);
            for (var i = 0; i < result.length; i++)
            {
                htmlBnaRhahu += result[i] + ",";
            }
            //$('[data-toggle="tooltip"]').tooltip({ 'placement': 'top' });
            $("#" + idName).attr('title', htmlBnaRhahu);
            //$(".selector").tooltip("option", "content", "Awesome title!");
            
        };
        options.error = function () {
            $("#" + idName).attr('title', 'No Subject Found!');
            //var htmlBnaRhahu = 'No Subject Found!';
            //return htmlBnaRhahu;
        };
        $.ajax(options);        
        //$('[data-toggle="tooltip"]').tooltip();

    });
    //$(function () {
    //    $('[data-toggle="tooltip"]').tooltip();
    //})
    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });
});