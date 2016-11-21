$(document).ready(function () {
    $("article").mouseenter(function (event) {
        //$("tr").off('hover');
        //alert(event.clientY);
        //alert(event.clientX);
        var idName = $(this).attr("id");
        $("#loaderimg").offset({ top: event.pageY, left: event.pageX }).show();
        //$("#loaderimg").show();
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
            for (var i = 0; i < result.length; i++) {
                htmlBnaRhahu += result[i] + ",";
            }
            //$('[data-toggle="tooltip"]').tooltip({ 'placement': 'top' });
            $("#" + idName).attr('title', htmlBnaRhahu);
            $("#loaderimg").fadeOut(6000);
            //$(".selector").tooltip("option", "content", "Awesome title!");

        };
        options.error = function () {
            $("#" + idName).attr('title', 'No Subject Found!');
            $("#loaderimg").fadeOut(6000);
            //var htmlBnaRhahu = 'No Subject Found!';
            //return htmlBnaRhahu;
        };
        $.ajax(options);
        //$('[data-toggle="tooltip"]').tooltip();
        //$(this).css("cursor", "default");
        //$(".loaderimg").hide();
    });
    $(".teachersubjectClassForView").hover( function ()
    {
        $(".teachersubjectClassForView").css("cursor", "pointer");
    });
    $(".teachersubjectClassForView").on("click", function ()
    {
        $("#screenblurLoadingSubjectsTeacher").show();
        //SubjectShowKrwa
        var options = {};
        options.url = "/Teacher/GetTeacherSubjectsNamesDistinct";
        options.type = "POST";
        options.data = JSON.stringify({ id: "1" });
        options.dataType = "json";
        options.contentType = "application/json";
        options.success = function (result) {
            $(".SubjectShowKrwa").show();
            $(".SubjectDiBody").html('');
            var z = 1;
            //alert(result.length);
            for (var i = 0; i < result.length;) {
                //var j = 0;

                //id='MySubjID_" + z + "
                $(".SubjectDiBody").append("<tr><td> "
                    + z + "</td>" +"<td>"
                    + result[i] + "</td>" +
                    + "</tr>");
                i++;
                z++;
            }
            //unBindWala();
            $(".SubjectShowKrwa").fadeOut(10000);
            $("#screenblurLoadingSubjectsTeacher").hide();
            //$("#batch").prop("disabled", false);
        };
        options.error = function () { 
            alert("Error retrieving results!");
            $(".SubjectShowKrwa").fadeOut(10000);
            $("#screenblurLoadingSubjectsTeacher").hide();
        };
        $.ajax(options);
    });
    $("#totalMarks").on("keyup", function () {
        //alert("hello");
        var textfield = document.getElementById("totalMarks").value;
        //alert(textfield);
        var regex = /[^0-9]/gi;
        if (textfield.search(regex) > -1 || textfield > 100 || textfield <= 0)
        {
            alert("Total Marks Must be a Positive whole number and Less than 100");
            $("#totalMarks").val("");
            //textfield = textfield.replace(regex, "");
        }
    });
    $(".obtMarksAddAll").on("keyup", function () {
        var obtMarks = $(this).val();
        var textfield = document.getElementById("totalMarks").value;
        //alert(obtMarks);

        //alert(textfield);
        var regex = /[^0-9]/gi;

        if (textfield.search(regex) > -1 || textfield > 100 || textfield == "" || textfield == null) {
            alert('Plz Enter Valid Total Marks First!');
            $(this).val("");
            //textfield = textfield.replace(regex, "");
        }
        else if (parseFloat(obtMarks) > parseInt(textfield))
        {
            alert('Entered Marks must be a positive number and Less Than Total Marks!');
            $(this).val("");
        }
    });
    $("#totalLecAddAll").on("keyup", function () {
        //alert("hello");
        var textfield = document.getElementById("totalLecAddAll").value;
        //alert(textfield);
        var regex = /[^0-9]/gi;
        if (textfield.search(regex) > -1 || textfield > 60 || textfield <= 0) {
            alert("Total Lectures Must be a Positive whole number and between 1 and 60");
            $("#totalLecAddAll").val("");
            //textfield = textfield.replace(regex, "");
        }
    });
    $(".attLecAddAll").on("keyup", function () {
        var attLecAddAll = $(this).val();
        var textfield = document.getElementById("totalLecAddAll").value;
        //alert(obtMarks);

        //alert(textfield);
        var regex = /[^0-9]/gi;

        if (textfield.search(regex) > -1 || textfield > 60 || textfield == "" || textfield == null) {
            alert('Plz Enter Valid Total Lectures First!');
            $(this).val("");
            //textfield = textfield.replace(regex, "");
        }
        else if (parseInt(attLecAddAll) > parseInt(textfield))
        {
            alert('Attended Lectures must be a Positive whole number and Less Than Total Lectures!');
            $(this).val("");
        }
    });
     
});