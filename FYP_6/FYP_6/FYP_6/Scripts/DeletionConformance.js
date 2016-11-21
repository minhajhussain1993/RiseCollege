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
//UploadTeacherAtt
//UploadStudentAtt
//UploadMarksStd