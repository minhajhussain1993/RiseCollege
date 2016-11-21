$(document).ready(function () {
    //$("#batch").prop("disabled", true);
    $("#degree").change(function () {
        if ($("#degree").val() != "Please select") {
            var options = {};
            options.url = "/Teacher/GetBatches2";
            options.type = "POST";
            options.data = JSON.stringify({ degree: $("#degree").val() });
            options.dataType = "json";
            options.contentType = "application/json";
            options.success = function (batch) {
                $("#batch").empty();
                $("#batch").append("<option value='Please select'>Please select</option>");
                for (var i = 0; i < batch.length;) {
                    $("#batch").append("<option value='" + batch[i] + "'>" + batch[i] + "</option>");
                    i++;
                }

                $("#batch").prop("disabled", false);
            };
            options.error = function () { alert("Error retrieving batches!"); };
            $.ajax(options);
        }
        else {
            $("#batch").empty();
            $("#batch").prop("disabled", true);
        }
    });

    $("#batch").change(function () {
        if ($("#batch").val() != "Please select" && $("#batch").val() != null) {
            var options = {};
            options.url = "/Teacher/GetSections";
            options.type = "POST";
            options.data = JSON.stringify({ batch: $(this).val() });
            options.dataType = "json";
            options.contentType = "application/json";
            options.success = function (section) {
                //$('#aeRCHRESULT').html(section);
                $("#section").empty();
                //$("#section").append("<option value='Please select'>Please select</option>");
                for (var i = 0; i < section.length;) {
                    $("#section").append("<option value='" + section[i].SectionID + "'>" + section[i].SectionName + "</option>");
                    i++;
                }

                $("#section").prop("disabled", false);
            };
            options.error = function () { alert("Error retrieving sections!"); };
            $.ajax(options);

        }
        else {
            $("#section").empty();
            $("#section").prop("disabled", true);
        }
    });
});