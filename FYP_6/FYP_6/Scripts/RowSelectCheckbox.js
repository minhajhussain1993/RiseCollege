$(document).ready(function ()
{
   
    BindEventOnclickOnRows();
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
    $("article").mouseenter(function (event)
    {
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
            for (var i = 0; i < result.length; i++)
            {
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
    //$(function () {
    //    $('[data-toggle="tooltip"]').tooltip();
    //})
    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });
    $("#province").change(function () {
        //alert($("#province").val());
        if ($("#province").val() != "Please select") {
            $(".valMessageForProvince").hide();
            var options = {};
            options.url = "/Employees/GetCitiesDomicile";
            options.type = "POST";
            options.data = JSON.stringify({ province: $("#province").val() });
            options.dataType = "json";
            options.contentType = "application/json";
            options.success = function (domi) {
                //alert("ss");
                $("#domicile").empty();
                //$("#domicile").append("<option value='Please select'>Please select</option>");
                for (var i = 0; i < domi.length;) {
                    $("#domicile").append("<option value='" + domi[i].DistrictID + "'>" + domi[i].DistrictName + "</option>");
                    i++;
                }

                $("#domicile").prop("disabled", false);
            };
            options.error = function () { alert("Error retrieving Districts!"); };
            $.ajax(options);
        }
        else
        {
            $(".valMessageForProvince").show();
            $("#domicile").empty();
            $("#domicile").prop("disabled", true);
        }
    });
    $(".totalFeeCalClass").keyup(function ()
    {
        var regex = "^(?:[1-9]\d*|0)?(?:\.\d+)?$";

        //var finefee = document.getElementById("finefee").value;
        var examfee = document.getElementById("examfee").value;
        var tutionfee = document.getElementById("tutionfee").value;
        var admissionfee = document.getElementById("admissionfee").value;
        var regFee = document.getElementById("regFee").value;

         
        var valtotal = Number(examfee) + Number(tutionfee) + Number(admissionfee) + Number(regFee);
         
            $(".tFEE").val(valtotal);
         
    });
    $(".totalFeeCalClass2").keyup(function () {
        //var regex = "^(?:[1-9]\d*|0)?(?:\.\d+)?$";

        //var finefee = document.getElementById("finefee2").value;
        var examfee = document.getElementById("examfee2").value;
        var tutionfee = document.getElementById("tutionfee2").value;
        var admissionfee = document.getElementById("admissionfee2").value;
        var regFee = document.getElementById("regFee2").value;


        var valtotal = Number(examfee) + Number(tutionfee) + Number(admissionfee) + Number(regFee);

        $(".tFEE2").val(valtotal);

    });
    //fillfortFee
    //totaldegfee

    var typingTimer;                //timer identifier
    var doneTypingInterval = 1000;  //time in ms, 5 second for example
    var $input = $(".rollnoForFeeAdd");

    //on keyup, start the countdown
    $input.on('keyup', function () {
        clearTimeout(typingTimer);
        typingTimer = setTimeout(doneTyping2, doneTypingInterval);
    });

    //on keydown, clear the countdown 
    $input.on('keydown', function () {
        clearTimeout(typingTimer);
    });

    //user is "finished typing," do something
    function doneTyping2()
    {
         
        if ($(".rollnoForFeeAdd").val() != "") {
            //alert($("#Addfeescreenblur").val());
            $("#Addfeescreenblur").show();
            var options = {};
            options.url = "/Fee/FeeAddGetSummary";
            options.type = "POST";
            options.data = JSON.stringify({ roll: $(".rollnoForFeeAdd").val() });
            options.dataType = "json";
            options.contentType = "application/json";
            options.success = function (batch) {
                if (batch[6] == "rs") {
                    $(".totaldegfee").prop("readonly", false);
                    $(".totalinstall").prop("readonly", false);
                }
                else {
                    $(".totaldegfee").prop("readonly", true);
                    $(".totalinstall").prop("readonly", true);
                }
                $(".totaldegfee").val(batch[0]);
                $(".totalSubmitfee").val(batch[1]);
                $(".totalremfee").val(batch[2]);
                $(".totalinstall").val(batch[3]);
                $(".paidInst").val(batch[4]);
                $(".stdNameForFeeAdd").val(batch[5]);
                //stdNameForFeeAdd
                //$("input[id='changePic']").click();
                $(".totaldegfee").change();
                $(".totalinstall").change();
                $("#Addfeescreenblur").hide();
            };
            options.error = function () {
                alert("Unable to Retrieve Results!");
                $("#Addfeescreenblur").hide();
            };
            $.ajax(options);
        }
        else {
            $(".totaldegfee").empty();
            $(".totalSubmitfee").empty();
            $(".totalremfee").empty();
            $(".totalinstall").empty();
            $(".paidInst").empty();
            $(".stdNameForFeeAdd").empty();
            $(".totaldegfee").change();
            $(".totalinstall").change();
            $("#Addfeescreenblur").hide();
        }
    }
    $(".collectfeeBtn").on("click", function ()
    {
        //alert("hello");
        var totaldegfee = Number($(".totaldegfee").val());
        var totalinstall = Number($(".totalinstall").val());

        var regex = /[^0-9]/gi;
        //regex.test($(".totaldegfee").val())
        //isNaN(totaldegfee)
        if (isNaN(totaldegfee)) {
            document.getElementById("fillfortFee").innerHTML = "Plz Enter an unsigned whole number between 0 and 100000";
            $("#fillfortFee").show();
            return false;
        }
        else if (totaldegfee > 100000) {
            document.getElementById("fillfortFee").innerHTML = "Plz Enter an unsigned whole number between 0 and 100000";
            $("#fillfortFee").show();
            return false;
        }
        else if (totaldegfee < 0) {
            document.getElementById("fillfortFee").innerHTML = "Plz Enter an unsigned whole number between 0 and 100000";
            $("#fillfortFee").show();
            return false;
        }
        else {

            $("#fillfortFee").hide();
        }
        //else {

        //    $("#fillfortFee").hide();
        //}
        //if (isNaN(totaldegfee) || totaldegfee > 100000 || totaldegfee < 0 || totaldegfee=="")
        //{
        //    document.getElementById("fillfortFee").innerHTML = "Plz Enter an unsigned number between 0 and 100000"; 
        //    $("#fillfortFee").show();
        //    return false;
        //}
        if ($(".totalinstall").val() != "")
        {
        if (regex.test($(".totalinstall").val()) || totalinstall > 10 || totalinstall < 1 || totalinstall == "")
            {
                document.getElementById("fillfortInst").innerHTML = "Plz Enter an unsigned whole number between 1 and 10";
                $("#fillfortInst").show();
                //$(".fillfortInst").val("Plz Enter an unsigned number between 1 and 10");
                //$(".fillfortInst").show();
                return false;
            }
        }
         
    });
    $(".totaldegfee").keyup(function () {
        var totaldegfee =parseInt($(".totaldegfee").val());
         
        var regex = /[^0-9]/gi;
        //regex.test($(".totaldegfee").val());
         
        if (isNaN(totaldegfee))
        {
            //alert("1");
                $("#fillfortFee").show();
                return false;
        }
        else if ( totaldegfee > 100000) 
        {
            //alert("2");
            $("#fillfortFee").show();
            return false;
        }
        else if (totaldegfee < 0) 
        {
            //alert("3");
            $("#fillfortFee").show();
            return false;
        }
    //else if (totaldegfee == "") 
    //{
    //    alert("4");
    //    $("#fillfortFee").show();
    //    return false;
    //    }
        else {
             
            $("#fillfortFee").hide(); 
        }
        //if (isNaN(totaldegfee) || totaldegfee > 100000 || totaldegfee.search(regex) < 0 || totaldegfee == "") {
        //    document.getElementById("fillfortFee").innerHTML = "Plz Enter an unsigned whole number between 0 and 100000";
        //     $("#fillfortFee").show();
        //    //    return false;
        //}
        //else {
        //    $("#fillfortFee").hide();
        //}
    });
    $(".totalinstall").keyup(function () {
        var totalinstall = Number($(".totalinstall").val());
        var regex = /[^0-9]/gi;
        if ($(".totalinstall").val() != "") {
            if (regex.test($(".totalinstall").val()) || totalinstall > 10 || totalinstall < 1 || totalinstall == "") {
                document.getElementById("fillfortInst").innerHTML = "Plz Enter an unsigned whole number between 1 and 10";
                $("#fillfortInst").show();
                //$(".fillfortInst").val("Plz Enter an unsigned number between 1 and 10");
                //$(".fillfortInst").show();
                return false;
            }
            //if (isNaN(totalinstall) || totalinstall > 10 || totalinstall < 1 || totalinstall == "") {
            //    document.getElementById("fillfortInst").innerHTML = "Plz Enter an unsigned number between 1 and 10";
            //    $("#fillfortInst").show();
            //    //$(".fillfortInst").val("Plz Enter an unsigned number between 1 and 10");
            //    //$(".fillfortInst").show();
            //    //return false;
            //}
            else {
                $("#fillfortInst").hide();
            }
        }
    });
    $(".totaldegfee").change(function ()
    {
        var totaldegfee = Number($(".totaldegfee").val());

        if (isNaN(totaldegfee) || totaldegfee > 100000 || totaldegfee < 0 || totaldegfee == "") {
             
            document.getElementById("fillfortFee").innerHTML = "Plz Enter an unsigned number between 0 and 100000";
           // $("#fillfortFee").show();
        //    return false;
        }
        else {
            $("#fillfortFee").hide();
        }
    });
    $(".totalinstall").change(function ()
    {
        var totalinstall = Number($(".totalinstall").val());
        if ($(".totalinstall").val() != "") {
            if (isNaN(totalinstall) || totalinstall > 10 || totalinstall < 1 || totalinstall == "") {
                document.getElementById("fillfortInst").innerHTML = "Plz Enter an unsigned number between 1 and 10";
                //$("#fillfortInst").show();
                //$(".fillfortInst").val("Plz Enter an unsigned number between 1 and 10");
                //$(".fillfortInst").show();
                //return false;
            }
            else
            {
                $("#fillfortInst").hide();
            }
        }
    });
    //totalFeeCalClass
    //$(".rollnoForFeeAdd").keyup(function(){ 
    //    //alert("hello");
    //    if ($(".rollnoForFeeAdd").val() != "") {
             
    //        var options = {};
    //        options.url = "/Fee/FeeAddGetSummary";
    //        options.type = "POST";
    //        options.data = JSON.stringify({ roll: $(".rollnoForFeeAdd").val() });
    //        options.dataType = "json";
    //        options.contentType = "application/json";
    //        options.success = function (batch) {
    //            if (batch[6] == "rs")
    //            {
    //                $(".totaldegfee").prop("readonly", false);
    //                $(".totalinstall").prop("readonly", false);
    //            }
    //            else
    //            {
    //                $(".totaldegfee").prop("readonly", true);
    //                $(".totalinstall").prop("readonly", true);
    //            }
    //            $(".totaldegfee").val(batch[0]);
    //            $(".totalSubmitfee").val(batch[1]);
    //            $(".totalremfee").val(batch[2]);
    //            $(".totalinstall").val(batch[3]);
    //            $(".paidInst").val(batch[4]);
    //            $(".stdNameForFeeAdd").val(batch[5]);
    //            //stdNameForFeeAdd
    //            //$("input[id='changePic']").click();
    //            $(".totaldegfee").change();
    //            $(".totalinstall").change();
    //        };
    //        options.error = function () { alert("Unable to Retrieve Results!"); };
    //        $.ajax(options);
    //    }
    //    else {
    //        $(".totaldegfee").empty();
    //        $(".totalSubmitfee").empty();
    //        $(".totalremfee").empty();
    //        $(".totalinstall").empty();
    //        $(".paidInst").empty();
    //        $(".stdNameForFeeAdd").empty();
    //        $(".totaldegfee").change();
    //        $(".totalinstall").change();
    //    }
    //});

     
});
function BindEventOnclickOnRows() {
    $("tr").on("click", function () {

        //alert($(this).attr("class"));

        //var checked = $(this).attr("class").is(":checked");
        var className = $(this).attr("class");
        var checked = $("." + className).is(":checked");
        if (checked) {
            $("." + className).prop('checked', false);
        }
        else {

            $("." + className).prop('checked', true);
        }
    });
    $("tr").hover(function () {
        $("tr").css("cursor", "pointer");
    });


}
function UNBINDROWS() {
    $("tr").unbind("click"); 
}