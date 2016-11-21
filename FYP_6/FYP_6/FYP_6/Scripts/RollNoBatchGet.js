$(document).ready(function () {
    $("#batch1").change(function () {
        if ($("#batch1").val() != "Please select") {
            var options = {};
            options.url = "/Teacher/GetBatches";
            options.type = "POST";
            options.data = JSON.stringify({ batch: $("#batch1").val() });
            options.dataType = "json";
            options.contentType = "application/json";
            options.success = function (values) {
                //$("#degree").empty();
                //$("#batch").empty();
                //$("#section").empty();
                $("#degree1").val(values[0]);
                $("#section1").val(values[1]);
                $("#part1").val(values[2]);
            };
            options.error = function () { alert("Error retrieving results!"); };
            $.ajax(options);
        }
        else {
            $("#degree1").empty();
            $("#section1").empty();
            $("#part1").empty();
        }
    });
    
});
