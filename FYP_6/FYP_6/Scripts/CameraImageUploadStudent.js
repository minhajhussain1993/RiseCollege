$(document).ready(function () {

    $('#changePic').on('change', function (e) {
        $('#screenblur').show();

        var files = e.target.files;
        var myID = 3; //uncomment this to make sure the ajax URL works
        if (files.length > 0) {
            if (window.FormData !== undefined) {
                var data = new FormData();
                for (var x = 0; x < files.length; x++) {
                    data.append("file" + x, files[x]);
                }
                $.ajax({
                    type: "POST",
                    url: '/Home/UploadImageStudent?id=' + myID,
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (result) {
                        $('#screenblur').hide();
                        window.location.reload();

                    },
                    error: function (xhr, status, p3, p4) {
                        var err = "Error " + " " + status + " " + p3 + " " + p4;
                        if (xhr.responseText && xhr.responseText[0] == "{")
                            err = JSON.parse(xhr.responseText).Message;
                        $('#screenblur').hide();

                        alert("Plz Select Image File less than or Equal To 3 MB");
                    }
                });
            } else {

                $('#screenblur').hide();

                alert("This browser doesn't support HTML5 file uploads!");
            }
        }
    });
    $(".questionMarkId").mouseenter(function (event) {
        //$(".myimage").hide();
        $(".questionMarkId").css("cursor", "pointer");
    });
    //$(".myimage").mouseenter(function (event)
    //{
    //    //$(".myimage").hide();
    //    $(".questionMarkId").show();
    //});
    //$(".myimage").mouseleave(function (event) {
    //    $(".questionMarkId").hide();
    //});


    //$(".myimage").mouseout(function (event) {
    //    $(".myimage").show();
    //    $(".questionMarkId").hide();
    //});
    $(".clickgenerateEvent").click(function (event) {
        $("input[id='changePic']").click();
    });
    //$("input[id='my_file']").click();
});