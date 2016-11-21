    $(document).ready(function () {
        $("#ObtMarks").keyup(function ()
        {
            if ($("#ObtMarks").val() != null && $("#TotalMarks").val() != null) {
                var options = {};
                options.url = "/Teacher/ValidateObtMarksGetPercentage";
                options.type = "POST";
                options.data = JSON.stringify({ obtMarks: $("#ObtMarks").val(), totalMarks: $("#TotalMarks").val() });
                options.dataType = "json";
                options.contentType = "application/json";
                options.success = function (value) {
                    $("#PerMarks").empty();
                    $("#PerMarks").val(value);
                };
                options.error = function () { $("#PerMarks").val(0); };
                $.ajax(options);
            }
            else {
                $("#PerMarks").val(0);
                alert($("#PerMarks").val());
            }
        });
    });
