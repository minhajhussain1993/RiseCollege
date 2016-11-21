
function ShowBatches()
{        //alert($('.teacherIDgetBatch').val());
        var teachID = $('.teacherIDgetBatch').val();
        $.ajax({
            url: '/Employees/GetTeacherBatches',
            type: 'POST',
            dataType: "JSON",
            data: { teachID1: teachID },
            success: function (cities) {
                var getRows = $('table>tbody>tr').length;
                if (cities.length > 0) {
                    //alert(cities);
                    for (var i = 1; i <= getRows; i++) {
                        //alert(document.getElementById("batchname_" + i).innerHTML.trim());
                        //alert("row length:"+getRows);
                        for (var j = 0; j < cities.length; j++) {
                            //alert("batch length retrieved"+cities.length);
                            //alert(cities[j].toString());
                            //alert(cities[j].toString());
                            if (document.getElementById("batchname_" + i).innerHTML.trim() == cities[j].toString()) {
                                //alert("checked");
                                $(".roo_" + i).prop('checked', true);
                            }
                        }
                    }
                }
                else {
                    for (var i = 1; i <= getRows; i++) {
                        $(".roo_" + i).prop('checked', false);
                    }
                }

            }
        });

}

function pehlaFun()
{
    var checkerMine = false;
    var getRows = $('.pehlaStepTeacherBatches>tr').length;
    for (var i = 1; i <= getRows ; i++) {
        if ($(".roo_" + i).is(":checked")) {
            checkerMine = true;
        }
    }
    if (checkerMine == false)
    {
        document.getElementById('BatchSelecterValidator').innerHTML = '*Plz Select At least One Batch to Continue!';
        //BatchSelecterValidator
        return false;
    }
    else if ($("#search1").val() == null || $("#search1").val() == "")
    {
        document.getElementById('teachIDvalidator').innerHTML='*Plz Enter Teacher ID to Continue!';
        
        return false;
    }
    else
    {
        

        if (CheckTeacherID($("#search1").val())==true)
        {
            document.getElementById('teachIDvalidator').innerHTML = '*';
            document.getElementById('BatchSelecterValidator').innerHTML = '';
            //alert('hell0');
            var Arr = [];
            var j = 0;
            for (var i = 1; i <= getRows ; i++) {
                //alert(getRows);
                if ($(".roo_" + i).is(":checked")) {
                    //alert('hell0');
                    //alert(document.getElementById("batchname_" + i).innerHTML.trim());
                    Arr[j] = document.getElementById("batchname_" + i).innerHTML.trim();
                    j++;
                    //alert('hell0');
                }
            }
            //alert('hell0 Again!');
            //loadData(Arr);
            //$this.goForward();
            $(".pehliwaliTeacherBatchRegMe").hide();
            $("#screenblur2InTeacherBatchAssigning").show();
            $(".DusriwaliTeacherBatchRegMe").show();

            MYloadData(Arr);

            return false;
        }
        else
        {
            document.getElementById('teachIDvalidator').innerHTML = '*Plz Enter Correct Teacher ID to Continue!';
            return false;
        }
        
    }
}
function CheckTeacherID(teachID1)
{
    var options = {};
    options.url = "/Employees/TeacherIDVal";
    options.type = "POST";
    options.data = JSON.stringify({ teachID: teachID1 });
    options.dataType = "json";
    options.contentType = "application/json";
    options.success = function (result)
    {
        //alert(result);
        if (result != null)
        {
            //alert(result);
            document.getElementById('teachIDvalidator').innerHTML = '*';
            return true;
        }
        else
        {
            //alert('false');
            document.getElementById('teachIDvalidator').innerHTML = '*Plz Enter Correct Teacher ID to Continue!';
            return false;
        }
    };
    options.error = function ()
    {
        
    };
    $.ajax(options);
    if ($.ajax(options).success)
    {
        return true;
    }
    else
    {
        return false;
    }
}

function MYloadData(Arr) {
    var options = {};
    options.url = "/Employees/GetSubjects";
    options.type = "POST";
    options.data = JSON.stringify({ batchesNames: Arr });
    options.dataType = "json";
    options.contentType = "application/json";
    options.success = function (result) {
        //alert(result);
        //$("tr").off('click');
        UNBINDROWS();
        $(".DusraStepTeacherBatches").html('');
        var z = 1;
        //alert(result.length);
        for (var i = 0; i < result.length;)
        {
            var j = 0;

            //id='MySubjID_" + z + "
            $(".DusraStepTeacherBatches").append("<tr class='rowe_" + z + "'><td> <input type='checkbox' name='subjIDs' value='" + result[i][j++] + "'class='rowe_" + z + "' id='MySubjID_"+z+"'/>" + "</td>" +
                  "<td>" + result[i][j++] + "</td>"

                + "</tr>");
            i++;
            z++;
        }
        BindEventOnclickOnRows();
        $("#screenblur2InTeacherBatchAssigning").hide();
        //$("#batch").prop("disabled", false);
    };
    options.error = function () {
        alert("Error retrieving results!");
        $("#screenblur2InTeacherBatchAssigning").hide();
    };
    $.ajax(options);
}
function DusraFunEndWala() 
{
    var checkerMine = false;
    var getRows = $('.DusraStepTeacherBatches>tr').length;
    for (var i = 1; i <= getRows ; i++) {
        if ($(".rowe_" + i).is(":checked")) {
            checkerMine = true;
        }
    }
    if (checkerMine == false) {
        document.getElementById('SubjectSelecterValidator').innerHTML = '*Plz Select At least One Subject to Continue!';
        //BatchSelecterValidator
        return false;
    }
    
    else
    {
        document.getElementById('BatchSelecterValidator').innerHTML = '';
        var getRows = $('.pehlaStepTeacherBatches>tr').length;
        var getRows2 = $('.DusraStepTeacherBatches>tr').length;
        var Arr = [];
        //var Arr2 = [];
        var j = 0;
        for (var i = 1; i <= getRows ; i++)
        {
            if ($(".roo_" + i).is(":checked"))
            {
                Arr[j] = document.getElementById("batchname_" + i).innerHTML.trim();
                j++;
            }
        }
        var Arr2 = [];
        var k = 0;
        for (var i = 1; i <= getRows2 ; i++)
        {    
            if ($(".rowe_" + i).is(":checked"))
            {
                //alert();
                Arr2[k] = $("#MySubjID_" + i).val();
                k++;
            }
        }
        $("#screenblur2InTeacherBatchAssigning").show();
        EndFinalBas(Arr, Arr2, $("#search1").val());
    }
}
function EndFinalBas(Arr,Arr2,teachID1)
{
    var options = {};
    options.url = "/Employees/FinalResultGet";
    options.type = "POST";
    options.data = JSON.stringify({ batchesNames: Arr, SubjIDs: Arr2,teachID: teachID1 });
    options.dataType = "json";
    options.contentType = "application/json";
    options.success = function (result)
    {
        if (result == "S")
        {
            //alert(result);
            $("#FinalResultSuccessTBAdd").show();
            document.getElementById("FinalResultSuccessTBAdd").innerHTML = "Successfully Assigned Batches And Subjects to Teacher ID: " + teachID1;
            $("#FinalResultSuccessTBAdd").fadeOut(15000);
        }
        else
        {
            //alert(result);
            $("#FinalResultFailTBAdd").show();
            document.getElementById("FinalResultFailTBAdd").innerHTML = result;
            $("#FinalResultFailTBAdd").fadeOut(15000);
        }
        
        $("#screenblur2InTeacherBatchAssigning").hide();
        //$("#FinalMessageteacherBatchesAssign").show();
        //document.getElementById("FinalMessageteacherBatchesAssign").innerHTML = result;

      //$("#FinalMessageteacherBatchesAssign")      
    };
    options.error = function () { alert("Error Unable To Assign Batches!"); $("#screenblur2InTeacherBatchAssigning").hide(); };
    $.ajax(options);
}
function TeesraFunctionBacktoBatches()
{
    $(".pehliwaliTeacherBatchRegMe").show();
    $(".DusriwaliTeacherBatchRegMe").hide();
}
 