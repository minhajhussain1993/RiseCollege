$(function () {

    $("#showPass").change(function () {
        var checked = $(this).is(":checked");
        if (checked) {
            $("#newpass").attr("type", "text");
            $("#oldpass").attr("type", "text");
        } else {
            $("#newpass").attr("type", "password");
            $("#oldpass").attr("type", "password");
        }
    });

});