﻿$(document).ready(function () {
    $("#AttLec").keyup(function () {
        if ($("#AttLec").val() != null && $("#AttTotal").val() != null) {
            var options = {};
            options.url = "/Teacher/ValidateAttendanceGetPercentage";
            options.type = "POST";
            options.data = JSON.stringify({
                attLec: $("#AttLec").val(),
                totalLec: $("#AttTotal").val()
            });
            options.dataType = "json";
            options.contentType = "application/json";
            options.success = function (value) {
                $("#AttPer").empty();
                $("#AttPer").val(value);
            };
            options.error = function () { $("#AttPer").val(0); };
            $.ajax(options);
        }
        else {
            $("#AttPer").val(0);
            alert($("#AttPer").val());
        }
    });
    $(".marksscored").keyup(function ()
    {
        if (Number.isInteger($("#totalMarks").val()) && $("#totalMarks").val()>0)
        {

        }
        var marksscored = $(this).val();
        var totalmarkstoall = document.getElementById("totalmarkstoall").value;
        if (parseInt(marksscored) > parseInt(totalmarkstoall)) {
            alert("greater then  Total Mrks");
            $(this).val("");
        } else {
            // do something
        }
    });
     
});

