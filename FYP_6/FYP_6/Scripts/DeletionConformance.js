function FormSubmitted() {
    //var rollno = document.getElementById("rollno").innerHTML;
    if (confirm('Are You sure You want to Delete Records')) {
        return true;
    }
    else {
        return false;
    }
}
function FormSubmitted2() {
    //var rollno = document.getElementById("rollno").innerHTML;
    if (confirm('Are You sure You want to BackUp Records?')) {
        return true;
    }
    else {
        return false;
    }

}
function UploadTeacherAtt() {
    //alert('hello');
    //var rollno = document.getElementById("rollno").innerHTML;
    if (confirm('Are You sure You want to Upload Teacher Attendance?'))
    {
        //alert('hello');
        return true;
    }
    else
    {
        return false;
    }

}
//AssignSubjectsOnBatchSectionDegreeChange
function AssignSubjectsOnBatchSectionDegreeChange() {
    //var rollno = document.getElementById("rollno").innerHTML;
    if (confirm('Are You sure You want to Assign Subjects?')) {
        return true;
    }
    else {
        return false;
    }

}
function UploadStudentAtt() {
    //var rollno = document.getElementById("rollno").innerHTML;
    if (confirm('Are You sure You want to Upload Student Attendance?')) {
        return true;
    }
    else {
        return false;
    }

}
function UploadMarksStd() {
    //var rollno = document.getElementById("rollno").innerHTML;
    if (confirm('Are You sure You want to Upload Student Marks?')) {
        return true;
    }
    else {
        return false;
    }

}
function NewAddmission1stStepConfirm()
{
    if ($("#province").val() == "Please select")
    {
        //$(".valMessageForProvince").val("");
        //$(".valMessageForProvince").val("*Province is Required");
        $(".valMessageForProvince").show();
        return false;
    }
    else
    {
     //   $(".valMessageForProvince").val("");
        $(".valMessageForProvince").hide();
        return true;
    }
}

    


//UploadTeacherAtt
//UploadStudentAtt
//UploadMarksStd