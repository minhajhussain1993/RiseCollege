$(document).ready(function () {
    $("#search").keyup(function () {
        $(".loadingImageWhileTyping").show();

        if ($("#search").val() != null && $("#search").val() != "") {
            var options = {};
            options.url = "/Home/GetTeacherIDIntellisense";
            options.type = "POST";
            options.data = JSON.stringify({search: $("#search").val()});
            options.dataType = "json";
            options.contentType = "application/json";
            options.success = function (value)
            {
                $("#search").autocomplete({
                    source: value
                });
                //$(".loadingImageWhileTyping").hide();
                $(".loadingImageWhileTyping").fadeOut(6000);
            };
            options.error = function () {
                $("#search").val("");
                //$(".loadingImageWhileTyping").hide();
                $(".loadingImageWhileTyping").fadeOut(6000);
            };
            $.ajax(options);
        }
        else {
            $("#search").val("");
            $(".loadingImageWhileTyping").fadeOut(6000);
        }
    });
    var typingTimer;                //timer identifier
    var doneTypingInterval = 1000;  //time in ms, 5 second for example
    var $input = $(".tidSearch");

    //on keyup, start the countdown
    $input.on('keyup', function () {
        clearTimeout(typingTimer);
        typingTimer = setTimeout(doneTyping, doneTypingInterval);
    });

    //on keydown, clear the countdown 
    $input.on('keydown', function () {
        clearTimeout(typingTimer);
    });

    //user is "finished typing," do something
    function doneTyping() {
        
            //$(".loadingImageWhileTyping").show();

        if ($(".tidSearch").val() != null && $(".tidSearch").val() != "") {
            //alert($(".tidSearch").val());
                var options = {};
                options.url = "/Employees/GetTbatches";
                options.type = "POST";
                options.data = JSON.stringify({ search: $(".tidSearch").val() });
                options.dataType = "json";
                options.contentType = "application/json";
                options.success = function (result)
                {
                    var shownBatchesRows = $(".pehlaStepTeacherBatches>tr").length;
                    var k = 1;
                    if (result.length == 0)
                    {
                        for (var i = 0; i < shownBatchesRows; i++)
                        {
                            $(".roo_" + k).prop('checked', false);
                            //alert($(".roo_" + k).is(":checked"));
                            k++;
                        }
                    }
                    else
                    {
                        for (var i = 0; i < shownBatchesRows; i++) {

                            for (var z = 0; z < result.length; z++) {
                                if (document.getElementById("batchname_" + k).innerHTML.trim() == result[z]) {
                                    $(".roo_" + k).prop('checked', true);
                                    //alert($(".roo_" + k).is(":checked"));
                                }
                            }
                            k++;
                        }
                    }
                    
                };
                options.error = function () {
                    alert('Error Retrieving Results!');
                  //  $(".tbodyInsideForBatchesOfTeacher").html('');
                };
                $.ajax(options);
            }
            else {
                //$(".tidSearch").val("");
                //$(".tbodyInsideForBatchesOfTeacher").html('');
            }
        
    }
    $("#search1").keyup(function () {
        //alert('hello');
        $(".loadingImageWhileTyping").show();

        if ($("#search1").val() != null && $("#search1").val() != "") {
            var options = {};
            options.url = "/Home/GetTeacherIDIntellisense";
            options.type = "POST";
            options.data = JSON.stringify({ search: $("#search1").val() });
            options.dataType = "json";
            options.contentType = "application/json";
            options.success = function (value) {
                $("#search1").autocomplete({
                    source: value
                });
                //$(".loadingImageWhileTyping").hide();
                $(".loadingImageWhileTyping").fadeOut(6000);
            };
            options.error = function () {
                $("#search1").val("");
                //$(".loadingImageWhileTyping").hide();
                $(".loadingImageWhileTyping").fadeOut(6000);
            };
            $.ajax(options);
        }
        else {
            $("#search1").val("");
            $(".loadingImageWhileTyping").fadeOut(6000);
        }
    });

    //$(".specialDateEnrollStudent").on("change", function ()
    //{
    //    alert($(".specialDateEnrollStudent").val());
    //    if ($(".specialDateEnrollStudent").val() != null && $(".specialDateEnrollStudent").val() != "")
    //    {
    //        localStorage.setItem(studentDate, $(".specialDateEnrollStudent").val());
    //    }
    //    alert(localStorage.getItem(studentDate));
    //});
     
    //$(window).unload(function ()
    //{
    //    alert($(".specialDateEnrollStudent").val());
    //    var val = localStorage.getItem(studentDate);
    //    $(".specialDateEnrollStudent").val(val);
    //});
});
