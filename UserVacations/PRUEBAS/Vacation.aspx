<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="Vacation.aspx.cs" Inherits="SmartEnterprise.Employee.Vacation" %>
    <form id="form1" runat="server">
        <script type="text/javascript">
            var aFilterList = new Array();
            var dsUserVacations = null;
            sFilterList = new Array();
            var pfficePto = [];
            var datesVacations = [];
            var datesHolidays = [];
            var datesSick = [];
            var datesStudy = [];
            var isUSOffice = false;
            var aHolidaysColList;
            var aHolidayColListFormat;
            var sHeaderHolidayTable;
            var aRequestsColListFormat;
            var aRequestsColList;
            var sHeaderRequestsTable;
            var sRequestsTemplate;
            var dsReferences;
            var typesAndDates = [];
            var ColorHolidays = "#ff9898";
            var weekCalendar = {
                flat: true,
                mode: 'range',
                format: 'Y-m-d',
                select_month: false,
                select_year: false,
                first_day: 0,
                change: function () {
                    var rangeDate = String($('.range').pickmeup('get_date', true));
                    var arrayDate = rangeDate.split(',');
                    if (arrayDate[0] != arrayDate[1]) {
                        $('#txtRequestFrom').val(arrayDate[0]);
                        $('#txtRequestTo').val(arrayDate[1]);
                        var DateFrom = new Date(arrayDate[0].substring(0, 4), arrayDate[0].substring(5, 7) - 1, arrayDate[0].substring(8, 10));//format "aaaa-MM-dd"
                        var DateTo = new Date(arrayDate[1].substring(0, 4), arrayDate[1].substring(5, 7) - 1, arrayDate[1].substring(8, 10));//format "aaaa-MM-dd"
                        var DateDiff = getWeekDays(DateFrom, DateTo);
                        $('#txtRequestTotal').val(DateDiff);
                        SetHalfDay();
                    }
                    else {
                        $('#txtRequestFrom').val(arrayDate[0]);
                        $('#txtRequestTo').val('');
                        $('#txtRequestTotal').val('1');
                        SetHalfDay();
                    }
                    return true;
                },
                render: function (date) {
                    var dow = date.getDay();
                    if (dow == 0 || dow == 6) {//Weekend
                        return { disabled: true };
                    }
                }
            }
            
            var saturdayCalendar = {
                flat: true,
                mode: 'range',
                format: 'Y-m-d',
                select_month: false,
                select_year: false,
                first_day: 0,
                change: function () {
                    var rangeDate = String($('.range').pickmeup('get_date', true));
                    var arrayDate = rangeDate.split(',');
                    if (arrayDate[0] != arrayDate[1]) {
                        $('#txtRequestFrom').val(arrayDate[0]);
                        $('#txtRequestTo').val(arrayDate[1]);
                        var DateFrom = new Date(arrayDate[0].substring(0, 4), arrayDate[0].substring(5, 7) - 1, arrayDate[0].substring(8, 10));//format "aaaa-MM-dd"
                        var DateTo = new Date(arrayDate[1].substring(0, 4), arrayDate[1].substring(5, 7) - 1, arrayDate[1].substring(8, 10));//format "aaaa-MM-dd"
                        var DateDiff = getWeekDays(DateFrom, DateTo);
                        $('#txtRequestTotal').val(DateDiff);
                        SetHalfDay();
                    }
                    else {
                        $('#txtRequestFrom').val(arrayDate[0]);
                        $('#txtRequestTo').val('');
                        $('#txtRequestTotal').val('1');
                        SetHalfDay();
                    }
                    return true;
                },
                render: function (date) {
                    $('.range').find("*").removeClass("pmu-saturday");
                    var dow = date.getDay();
                    if (dow == 0) {//if (dow == 0 || dow == 6) {//Weekend
                        return { disabled: true };
                    }
                }
            }

            function LoadFieds() {
                aHolidaysColList = new Array("Id","HolidayDate", "HolidayDescription", "CommentaryNote", "StatusVacation", "HolidayOnDuty");
                aHolidayColListFormat = new Array("string", "string", "string", "string", "string", "string");

                sHeaderHolidayTable = "<tr class='LH30 H30'>";
                sHeaderHolidayTable += "<th class='W80 TCenter'>Date</th>";
                sHeaderHolidayTable += "<th class='TCenter'>Description</th>";
                sHeaderHolidayTable += "</tr>";

                sHolidayTemplate = "<tr class='whiteBG H25 LH25'>";
                sHolidayTemplate += "<td class='W80'>##HolidayDate##</td>";
                sHolidayTemplate += "<td class=''>##HolidayDescription##</td>";
                sHolidayTemplate += "</tr>";
                sHolidayTemplate += "<tr class='tTemplate'>";
                sHolidayTemplate += "<td colspan = \"2\" style='cursor:default !important' class='LH25 LightGrayText Indent10'>##CommentaryNote##</td>";
                sHolidayTemplate += "</tr>";

                aRequestsColListFormat = new Array("string", "string", "string", "string", "string", "string");
                aRequestsColList = new Array("VacationType", "RangeDate", "DaysNumber", "Note", "Status", "StatusClass");

                sHeaderRequestsTable = "<tr class='H30 LH30'>";
                sHeaderRequestsTable += "<th class=''>Range Date</th>";
                sHeaderRequestsTable += "<th class=''>Type</th>";
                sHeaderRequestsTable += "<th class=''>Days</th>";
                sHeaderRequestsTable += "<th class=''>Status</th>";
                sHeaderRequestsTable += "</tr>";

                sRequestsTemplate = "<tr class='whiteBG H25 LH25'>";
                sRequestsTemplate += "<td class='W160'>##RangeDate##</td>";
                sRequestsTemplate += "<td>##VacationType##</td>";
                sRequestsTemplate += "<td class='W40'>##DaysNumber##</td>";   
                sRequestsTemplate += "<td class='W40'><i title='##Status##' class='fa ##StatusClass##'></i></td>";
                sRequestsTemplate += "</tr>";
                sRequestsTemplate += "<tr class='tTemplate'>";
                sRequestsTemplate += "<td colspan = \"4\" style='cursor:default !important' class='LH25 LightGrayText Indent10'>##Note##</td>";
                sRequestsTemplate += "</tr>";
            }

            
              function smartLoad_CallBack(response) {
                if (response == null)
                    response = responseGeneric;
                sFilterList = new Array();
                if ((response.error == null) && (response.value[0] != null)) {
                    LoadFieds();
                    var strtbl = "";
                    CompleteInfo(response.value[1]);
                    var ArrayUserRequests = response.value[2];
                    var ArrayOfficeHolidays = response.value[3];
                    dsReferences = response.value[4];
                    var RequestDates = ArrayUserRequests[1];
                    var HolidayDates = ArrayOfficeHolidays[1];
                    $("#divHolidaysRequest").html(GetTableTemplateWithFormatForSort(ArrayUserRequests[0], aRequestsColList, sRequestsTemplate, sHeaderRequestsTable, aRequestsColListFormat, 'Listrequest_Table', 'W100P OrderTable'));
                    $("#divHolidayList").html(GetTableTemplateWithFormatForSort(ArrayOfficeHolidays[0], aHolidaysColList, sHolidayTemplate, sHeaderHolidayTable, aHolidayColListFormat, 'ListHolidays_Table', 'W100P OrderTable'));
                    SetColorReferences(dsReferences);
                    SetDays(RequestDates);
                    SetDays(HolidayDates);
                    $(function () {                        
                        $('.12-calendars').pickmeup({
                            flat: true,
                            mode: 'multiple',
                            select_month: false,
                            select_year: false,
                            calendars: 12,
                            first_day: 0,
                            format: 'Y-m-d',
                            render: function (date) {
                                var dow = date.getDay();
                                for (var i = 0; i < typesAndDates.length; i++) {
                                    var sTypeName = typesAndDates[i].sType;
                                    if ($.inArray(date.getTime(), typesAndDates[i].sDate) > -1) {
                                    if (dow == 0) {
                                        return { disabled: true };
                                    }
                                    return { class_name: 'pmu-selected-'+sTypeName+'' }
                                    }
                                }
                            }
                        });
                    });
                    $(function () {
                        $('.range').pickmeup(weekCalendar);
                    });
                    HidePleaseWaitBox();
                    ReplaceClases();
                }
                else {
                    HidePleaseWaitBox();
                    if (response.error != null)
                        alert("Application Error: " + response);
                    else
                        ConfirmSendError(response.value[1]);
                }
            }
            
            reloadPage = function () {
                GetPageAndCallAjaxObjectFunction('Employee/Vacation.aspx', 'Employee.Vacation.smartLoad');
            }
            function GetVacationYearSelected_ClientEvent() {
                var ddlYear = $('#ddlHolidaysYears').val();
                CallAjaxObjectFunction("Employee.Vacation.GetVacationYearSelected", ddlYear);
            }

            function GetVacationYearSelected_CallBack(response) {
                aFilterList = new Array();
                if ((response.error == null) && (response.value[0] != null)) {
                    ClearCalendar('.12-calendars');
                    var ArrayUserRequests = response.value[1];
                    var ArrayOfficeHolidays = response.value[2];
                    var RequestDates = ArrayUserRequests[1];
                    var HolidayDates = ArrayOfficeHolidays[1];
                    $("#divHolidaysRequest").html(GetTableTemplateWithFormatForSort(ArrayUserRequests[0], aRequestsColList, sRequestsTemplate, sHeaderRequestsTable, aRequestsColListFormat, 'Listrequest_Table', 'W100P OrderTable'));
                    $("#divHolidayList").html(GetTableTemplateWithFormatForSort(ArrayOfficeHolidays[0], aHolidaysColList, sHolidayTemplate, sHeaderHolidayTable, aHolidayColListFormat, 'ListHolidays_Table', 'W100P OrderTable'));
                    SetDays(RequestDates);
                    SetDays(HolidayDates);
                    ReplaceClases();
                    HidePleaseWaitBox();
                }
                else {
                    HidePleaseWaitBox();
                    if (response.error != null)
                        alert("Application Error: " + response);
                    else
                        ConfirmSendError(response.value[1]);
                }
            }

            ClearClases = function () {
                $('.12-calendars').find("*").removeClass("pmu-selected-vacation");
                $('.12-calendars').find("*").removeClass("pmu-not-selected-vacation");

                $('.12-calendars').find("*").removeClass("pmu-selected-study");
                $('.12-calendars').find("*").removeClass("pmu-not-selected-study");

                $('.12-calendars').find("*").removeClass("pmu-selected-sickLeave");
                $('.12-calendars').find("*").removeClass("pmu-not-selected-sickLeave");
            }
            UpdateCalendar = function () {
                $('.12-calendars').pickmeup('update');
                //ReplaceClases();
            }

            function ClearRequestCalendar() {
                $('#txtRequestFrom').val('');
                $('#txtRequestTo').val('');
                $('#txtRequestTotal').val('');
                $('#DaysInfoRow').hide();
                if (!isUSOffice)
                    $('#sCategory').val('0');
                $('.range').pickmeup('clear');
                $('#chkHalfDayOff').prop('checked', false);
                $('#divHalfDay').css('display', 'none');
                $('.range').find("*").addClass("pmu-disabled");
            }

            function VacationSetRequest(sId) {
                var fnYes = "VirtualFormValidation('');CallAjaxObjectFunction('Employee.Vacation.SetOnDutyRequest'," + sId + " )";
                Confirm(fnYes, "", "On Duty", "Are you sure to send a request for work on Holiday?");
            }

            function SetOnDutyRequest_CallBack(response) {
                aFilterList = new Array();
                if ((response.error == null) && (response.value[0] != null)) {
                    if (response.value[0] != "Exist.") {
                        if (response.value[1] != "Done") {
                            Notify("Some kind of error occurred sendig an email");
                        }
                        else {
                            Notify("Email Request for work on holiday was sent");
                        }
                        SetTableRequest(response.value[2]);
                    }
                    else {
                        Alert("","On Duty Request", "The Request already exist.");
                    }
                    HidePleaseWaitBox();
                }
                else {
                    HidePleaseWaitBox();
                    if (response.error != null)
                        alert("Application Error: " + response);
                    else
                        ConfirmSendError(response.value[1]);
                }
            }

            function SetTableVacationDays(response) {
                var dsHolidays = response.value[1];
                var dsRequests = response.value[2];
                var dsInfo = response.value[3];
                var dsSetDays = response.value[5];
                $("#divHolidayList").html(GetTableTemplateWithFormatForSort(dsHolidays, aHolidaysColList, sHolidayTemplate, sHeaderHolidayTable, aHolidayColListFormat, 'ListHolidays_Table', 'W100P OrderTable'));
                $("#divHolidaysRequest").html(GetTableTemplateWithFormatForSort(dsRequests, aRequestsColList, sRequestsTemplate, sHeaderRequestsTable, aRequestsColListFormat, 'Listrequest_Table', 'W100P OrderTable'));
                CompleteInfo(dsInfo);
                $(".icon_false").hide();
                SetDays(dsSetDays);
                ReplaceClases();
            }

            function SetTableRequest(ds)
            {
                $("#divHolidaysRequest").html(GetTableTemplateWithFormatForSort(ds, aRequestsColList, sRequestsTemplate, sHeaderRequestsTable, aRequestsColListFormat, 'Listrequest_Table', 'W100P OrderTable'));
                $("#Listrequest_Table_filter").hide();
                $("#Listrequest_Table_info").hide();
            }

            function CompleteInfo(ds) {
                var YearFocus = $('#ddlHolidaysYears').val();
                var idx = 0;
                var dsYear = "";
                var sFlag = true;
                if (ds.Tables[0].Rows.length > 0) {
                    while (sFlag) {
                        dsYear = String(ds.Tables[0].Rows[idx]["VacationYear"]);
                        if (YearFocus == dsYear) {
                            sFlag = false;
                            var Total = ds.Tables[0].Rows[idx]["VacationDays"];
                            var Used = ds.Tables[0].Rows[idx]["VacationDaysTaken"];
                            var Remaining = ds.Tables[0].Rows[idx]["RemainingDays"];
                            $('#TxtTotal').val(Total);
                            $('#TxtUsed').val(Used);
                            $('#TxtRemaining').val(Remaining);
                        }
                        else {
                            idx++;
                        }
                    }
                }
                else {
                    $('#TxtTotal').val('0');
                    $('#TxtUsed').val('0');
                    $('#TxtRemaining').val('0');
                }
            }

            function ChangeRequestDays() {
                var sCategory = $("#sCategory option:selected").text();  //$('#sCategory').text();
                $('.range').find("*").removeClass("pmu-disabled");
                SetHalfDay();
                if (sCategory.toLowerCase() == "on duty") {
                    $('.range').pickmeup('destroy');
                    $('.range').pickmeup(saturdayCalendar);
                }
                else {
                    $('.range').pickmeup('destroy');
                    $('.range').pickmeup(weekCalendar);
                }
            }

            function SendRequest() {
                var CategoryOpt = $('#sCategory').val();
                var sYear = $('#ddlHolidaysYears').val();
                var Remaining = $('#TxtDaysInfo').val();
                var Requested = $('#txtRequestTotal').val();
                var txtFrom = $('#txtRequestFrom').val();
                var txtTo = $('#txtRequestTo').val();
                var chkHalfDay = $('#chkHalfDayOff').prop('checked');
                if (chkHalfDay) {
                    CategoryOpt = "7";
                }
                if (CategoryOpt != "0") {
                    if (txtFrom.trim() != "") {
                        if ((CategoryOpt == "1") || (CategoryOpt == "7")) {
                            if (parseInt(Requested) > parseInt(Remaining)) {
                                Alert("", "Vacations", "The Requested days exceed the amount of Remaining days");
                            }
                            else {
                                CallAjaxObjectFunction("Employee.Vacation.SendRequestForVacations", CategoryOpt, txtFrom, txtTo, Requested, sYear);
                            }
                        }
                        else {
                            CallAjaxObjectFunction("Employee.Vacation.SendRequestForVacations", CategoryOpt, txtFrom, txtTo, Requested, sYear);
                        }
                    }
                    else {
                        Alert("", "Vacations", "Please select a valid date 'From'");
                    }
                }
                else {
                    Alert("", "Vacations", "Please select a category for request"); 
                }
            }

            function SendRequestForVacations_CallBack(response) {
                aFilterList = new Array();
                if ((response.error == null) && (response.value[0] != null)) {
                    $("#ModalRequest").modal("hide");
                    if (response.value[1] != "Done") {
                        Notify("Some kind of error occurred sending an email");
                    }
                    else {
                        switch (response.value[3]) {                            
                            case "1":
                                Notify("Email Request for vacation was sent");
                                break;
                            case "4":
                                Notify("Email Request for study day was sent");
                                break;
                            case "6":
                                Notify("Email Request for day off was sent");
                                break;
                            case "7":
                                Notify("Email Request for Half day off was sent");
                                break;
                        }                        
                    }
                    var dsRequests = response.value[2];
                    SetTableRequest(dsRequests);
                    ClearRequestCalendar();
                    HidePleaseWaitBox();
                }
                else {
                    HidePleaseWaitBox();
                    if (response.error != null)
                        alert("Application Error: " + response);
                    else
                        ConfirmSendError(response.value[1]);
                }
            }

            function HighlightHolidays(ds) {
                if (ds.Tables[0].Rows.length > 0) {
                    for (i = 0; i < ds.Tables[0].Rows.length; i++) {
                        var visible = ds.Tables[0].Rows[i]["ItemUsersView"];
                        if (visible == true) {
                            var HolidayDate = ds.Tables[0].Rows[i]["Date"];
                            var tempDay = new Date(HolidayDate);
                            tempDay.setHours(0, 0, 0, 0);
                            datesHolidays.push(tempDay.getTime());
                        }
                    }
                }
            }

            function SetDays(dsDates) {
                if (dsDates && dsDates.Tables[0]) {
                    for (var i = 0; i < dsDates.Tables[0].Rows.length; i++) {
                        var aDate = [];
                        var sVacationType = dsDates.Tables[0].Rows[i]["TypeName"].toLowerCase();
                        var HolidayDate = dsDates.Tables[0].Rows[i]["sDate"];
                        var tempDay = new Date(HolidayDate);
                        tempDay.setHours(0, 0, 0, 0);
                        var objDateReference = {
                            sDate: [tempDay.getTime()],
                            sType: sVacationType.replace(/ /g,'')
                        }
                        typesAndDates.push(objDateReference);
                   }
                }
            }

            function ReplaceClases() {
                var css = document.createElement('style');
                css.type = 'text/css';
                if (dsReferences && dsReferences.Tables[0]) {
                    for (var i = 0; i < dsReferences.Tables[0].Rows.length; i++) {
                        var NewStyle = dsReferences.Tables[0].Rows[i]["TypeName"].toLowerCase().replace(/ /g, '');
                        var NewColor = dsReferences.Tables[0].Rows[i]["TypeColor"];
                        var darkerColor = shadeColor(NewColor, -0.2);
                        var NewRule = '.pmu-selected-' + NewStyle + ' {color: ' + darkerColor + ' !important; border: 1px ' + darkerColor + ' solid !important; background: ' + NewColor + ' !important }';
                        cssCalendarEngine(NewRule);
                    }
                }
                var darkerColorHoliday = shadeColor(ColorHolidays, -0.2);
                var HolidayRule = '.pmu-selected-holiday {color: ' + darkerColorHoliday + ' !important; border: 1px ' + darkerColorHoliday + ' solid !important; background: ' + ColorHolidays + ' !important }';;
                cssCalendarEngine(HolidayRule);
            }

            function cssCalendarEngine(newRule) {
                var css = document.createElement('style'); 
                css.type = 'text/css'; 
                if (css.styleSheet) css.styleSheet.cssText = newRule; 
                else css.appendChild(document.createTextNode(newRule)); 
                document.getElementsByTagName("head")[0].appendChild(css); 
            }

            function SetHalfDay() {
                var TotalRequested = $('#txtRequestTotal').val();
                var ddlOpt = $('#sCategory').val();
                if ((TotalRequested == "1") && (ddlOpt == "1"))  {
                    $('#divHalfDay').css('display', 'block');
                }
                else {
                    $('#divHalfDay').css('display', 'none');
                    $('#chkHalfDayOff').prop('checked', false);
                }
            }

       function SetColorReferences(ds) {
           var ColorTable = "";
           ColorTable = "<table style='right: 10px; bottom: 20px;' class='PosAbs'><tr class='jqplot-table-legend'>";
           for (var i = 0; i < ds.Tables[0].Rows.length; i++) {
               var color = ds.Tables[0].Rows[i]["TypeColor"];
               var reference = ds.Tables[0].Rows[i]["TypeName"];
               var brigtherColor = shadeColor(color, 0.5);
               ColorTable += "<td class='jqplot-table-legend jqplot-table-legend-swatch'><div class='jqplot-table-legend-swatch-outline'><div style='background:" + brigtherColor + " !important; border-color:" + color + " !important;' class='jqplot-table-legend-swatch'></div></div></td><td style='padding-right: 10px;' class='jqplot-table-legend jqplot-table-legend-label'>" + reference + "</td>";
           }
           ColorTable += "<td class='jqplot-table-legend jqplot-table-legend-swatch'><div class='jqplot-table-legend-swatch-outline'><div style='background:" + shadeColor(ColorHolidays, 0.5) + " !important; border-color:" + ColorHolidays + " !important;' class='jqplot-table-legend-swatch'></div></div></td><td style='padding-right: 10px;' class='jqplot-table-legend jqplot-table-legend-label'>Holidays</td>";
           ColorTable += "</tr></table>";
           $("#divReference").html(ColorTable);
       }

       function ClearCalendar(calendar) {
           $(calendar).pickmeup('clear');
           $('head style').remove();
       }
        </script>
<input type="hidden" id="hdnRangeTotal" />
<div style="height:300px;" id="ModalRequest" class="modal fade W600">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                    <div class="btnClose" data-dismiss="modal"><i class="fa fa-times-circle XLarge"></i></div>
                <h4 class="modal-title">Make a Request</h4>
            </div>                                   
            <div id="modalContent" style="padding:10px 10px;">
                <div style="padding-left:20px;padding-top:20px;float:left;" id='Holycalendar' class='range W100P'>
                    <div id="DivRequest" style="float:right;margin-left:30px;width:300px">
                        <table  class="W100P H35">
                            <tr class="H35"><td>Category</td><td><select id="sCategory" onchange="ChangeRequestDays()" class="form-control"><option  Selected value="0">Select Category</option><option value="1">Vacations</option><option value="4">Study Day</option><option value="6">Compensatory Days</option></select></td><td></td></tr>
                            <tr id="DaysInfoRow" style="display:none" class="H35 "><td>Remaining Days</td><td><input id="TxtDaysInfo" class="W60 form-control" readonly type="text" /></td><td></td></tr>
                            <tr class="H35"><td class="W100"><span class="row">From</span><input class="W90 form-control" readonly id="txtRequestFrom" readonly type="text" /></td><td colspan="2"><span class="row">To</span><input style="display:inline-block" readonly id="txtRequestTo" class="W90 form-control" type="text" /><div id="divHalfDay" style="float:right !important; display:none">Half Day Off<input type="checkbox"  id="chkHalfDayOff"></div></td><td></td></tr>
                            <tr class="H35"><td>Total Request Days</td><td><input readonly id="txtRequestTotal" class="W90 form-control" type="text" /></td><td></td></tr>
                            <tr class="H35"><td></td><td><div class="btn btnNotAdd" onclick="SendRequest();" style="margin-left:15px;">Send Request</div></td><td></td></tr>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
<div class="wide round">
    <div class="row LH30">
        <div class="floatLeft subTitle" onclick="reloadPage();">Year</div> 
        <asp:dropdownlist Cssclass="form-control floatLeft" id="ddlHolidaysYears" runat="server" onchange="GetVacationYearSelected_ClientEvent()"></asp:dropdownlist>
        <div class="floatLeft subTitle">Total Days</div>
        <asp:textbox id="TxtTotal" Cssclass="form-control floatLeft" style="float:left" ReadOnly="true" Width="40" runat="server" Text=""></asp:textbox>
        <div class="floatLeft subTitle">Days Used</div> 
        <asp:textbox id="TxtUsed" Cssclass="form-control floatLeft" style="float:left" ReadOnly="true" Width="40" runat="server" Text=""></asp:textbox>
        <div class="floatLeft subTitle">Days Remaining</div> 
        <asp:textbox id="TxtRemaining" Cssclass="form-control floatLeft" style="float:left" ReadOnly="true" Width="40" runat="server" Text=""></asp:textbox>
        <!--<div class="floatLeft subTitle">Earned Days</div>         
        <asp:textbox id="TextEarned" Cssclass="form-control floatLeft" style="float:left" ReadOnly="true" Width="40" runat="server" Text=""></asp:textbox>-->
        <div class="btn btnNotAdd floatLeft" onclick="ClearRequestCalendar(); $('#ModalRequest').modal('show');" style="margin-left:15px;margin-bottom: 8px;">Make a Request</div>         
    </div>
    <div class="row TableDisplay">
        <div id="divNewAdd" class="floatLeft" style="width:calc(100vw - 1000px);max-width:800px;">
            <div id="divTablesRequest">        
                <div class="roundthin">
                    <div id="divHolidaysRequest" style="height:calc(50vh - 70px);" class="list"></div>
                </div>
                <div class="roundthin">
                    <div id="divHolidayList" style="height:calc(50vh - 100px);" class="list"></div>                     
                </div>
            </div>
        </div>
        <div class="Mar10L floatLeft" style="padding:0px;width:790px;">
            <div id="divCalendarContent" style="float:right;width:100%;">
                <hr class="W90P" />  
                <div id="DivYearCalendar" style="width:100%;float:right; font-size:13px;" class="12-calendars"></div>
                <hr class="W90P" />
                <div id="divReference"></div>  
            </div>
        </div>
    </div>
</div>

 </form>