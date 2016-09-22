using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Configuration;
using SmartEnterprise.Classes;
using System.Text;
using System.Data.SqlClient;

namespace SmartEnterprise.Employee
{

    public partial class Vacation : System.Web.UI.Page
    {
        #region Attributes
        public string sStatus
        {
            get { return ViewState["sStatus"].ToString(); }
            set { ViewState["sStatus"] = value; }
        }
        #endregion

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            this.ID = "Form1";
            this.Load += new System.EventHandler(this.Page_Load);
        }
        #endregion

        protected System.Web.UI.WebControls.DropDownList ddlHolidaysYears;
        protected System.Web.UI.WebControls.TextBox TxtTotal;
        protected System.Web.UI.WebControls.TextBox TxtUsed;
        protected System.Web.UI.WebControls.TextBox TxtRemaining;
        protected System.Web.UI.WebControls.TextBox TextEarned;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                LoadDropDownYears();
            }
            catch (Exception ex)
            {
                CUtil.sErrorMessage = "Error loading page.\n\n" + ex.Message + "\n\nStack Trace:\n" + ex.StackTrace;
            }
        }

        public object[] smartLoad_ServerEvent()
        {
            object[] oReturn = new object[6];
            try
            {
                oReturn[0] = "Done.";
                oReturn[1] = CUserVacations.GetUserVacationInfo(CUserInfo.UserId, DateTime.Now.Year);
                oReturn[2] = GetUserYearRequests(CUserInfo.UserId, DateTime.Now.Year);
                oReturn[3] = GetHolidaysCalendar(CUserInfo.UserId);
                oReturn[4] = CUserVacations.SetVacationDaysReferences(CUserInfo.ActualSystemId, CUserInfo.UserId); //dtVacationTypes and dtHolidays
            }
            catch (Exception ex)
            {
                oReturn[0] = null;
                oReturn[1] = "Error loading vacations.\n\n" + ex.Message + "\n\nStack Trace:\n\n" + ex.StackTrace;
            }
            return oReturn;
        }

        private void LoadDropDownYears()
        {
            ListItem li0 = new ListItem("Select Year", "0");
            int CurrentYear = DateTime.Now.Year;
            ListItem li1 = new ListItem((CurrentYear - 1).ToString(), (CurrentYear - 1).ToString());
            ListItem li2 = new ListItem((CurrentYear).ToString(), (CurrentYear).ToString());
            ListItem li3 = new ListItem((CurrentYear + 1).ToString(), (CurrentYear + 1).ToString());
            ddlHolidaysYears.Items.Insert(0, li3);
            ddlHolidaysYears.Items.Insert(0, li2);
            ddlHolidaysYears.Items.Insert(0, li1);
            ddlHolidaysYears.SelectedIndex = 1;
        }

        public object[] GetVacationYearSelected_ServerEvent(string SelectedYear)
        {
            object[] oReturn = new object[5];
            try
            {
                oReturn[0] = "Done.";
                oReturn[1] = GetUserYearRequests(CUserInfo.UserId, Convert.ToInt32(SelectedYear));
                oReturn[2] = GetHolidaysCalendar(CUserInfo.UserId);
            }
            catch (Exception e)
            {
                oReturn[0] = null;
                oReturn[1] = "Error getting Company List.\n\n" + e.Message + "\n\nStack Trace:\n" + e.StackTrace;
            }
            return oReturn;
        }

        public string GetParentsLeaders(int SystemId, int UserId)
        {
            string parentsList = "";
            DataSet ds = new DataSet();
            ds = CPerson.GetUserSupervisor(SystemId, UserId);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    parentsList = dr["WorkEmail"].ToString();
                }
            }
            return parentsList;
        }

        public object[] SetOnDutyRequest_ServerEvent(string HolidayId)
        {
            object[] oReturn = new object[3];
            try
            {
                string mailList = "";
                string mailResult = "";
                DataSet ds = null;
                ds = CUserVacations.SeUserWorkOnHolidays(CUserInfo.UserId, Convert.ToInt32(HolidayId));
                if (ds.Tables[0].Rows[0]["Result"].ToString() == "Done")
                {
                    string UserName = CUserInfo.FirstName + " " + CUserInfo.LastName;
                    mailList = GetParentsLeaders(CUserInfo.SystemId, CUserInfo.UserId);
                    string OnDutyDate = ds.Tables[0].Rows[0]["OnDutyDate"].ToString();
                    string DutyDateYear = ds.Tables[0].Rows[0]["Year"].ToString();
                    mailResult = CUserVacations.SendRequestVacationEmail(mailList, OnDutyDate, OnDutyDate, DutyDateYear, "0", UserName, "2");
                    oReturn[0] = "Done.";
                    oReturn[1] = mailResult;
                    oReturn[2] = CUser.GetUserVacationsAndDaysOff(CUserInfo.UserId, DateTime.Now.Year);
                }
                else
                {
                    oReturn[0] = "Exist.";
                    oReturn[1] = "";
                }
            }
            catch (Exception e)
            {
                oReturn[0] = null;
                oReturn[1] = "Error getting Company List.\n\n" + e.Message + "\n\nStack Trace:\n" + e.StackTrace;
            }
            return oReturn;
        }

        public object[] SendRequestForVacations_ServerEvent(string sCategory, string sFrom, string sTo, string sRequested, string sYear)
        {
            object[] oReturn = new object[4];
            string mailList = "";
            try
            {
                DataSet ds = new DataSet();
                if (sTo == "")
                {
                    sTo = sFrom;
                }
                if ((sCategory == "1") || (sCategory == "7"))
                {
                    ds = CUserVacations.SetVacationRequest(CUserInfo.UserId, sFrom, sTo, Convert.ToInt32(sYear), Convert.ToInt32(sCategory));
                }
                else if (sCategory == "4")
                {
                    ds = CUserVacations.SetVacationRequest(CUserInfo.UserId, sFrom, sTo, DateTime.Now.Year, Convert.ToInt32(sCategory));
                }
                else if (sCategory == "6")
                {
                    ds = CUserVacations.SeUserCompensatoryDay(CUserInfo.UserId, sFrom, sTo, Convert.ToInt32(sYear));
                }
                string UserName = CUserInfo.FirstName + " " + CUserInfo.LastName;
                mailList = GetParentsLeaders(CUserInfo.SystemId, CUserInfo.UserId);
                string mailResult = CUserVacations.SendRequestVacationEmail(mailList, sFrom, sTo, sYear, sRequested, UserName, sCategory);
                oReturn[0] = "Done.";
                oReturn[1] = mailResult;
                oReturn[2] = CUser.GetUserVacationsAndDaysOff(CUserInfo.UserId, Convert.ToInt32(sYear));
                oReturn[3] = sCategory;
            }
            catch (Exception e)
            {
                oReturn[0] = null;
                oReturn[1] = "Error getting Company List.\n\n" + e.Message + "\n\nStack Trace:\n" + e.StackTrace;
            }
            return oReturn;
        }

        public DataSet CalendarDates(DataSet ds)
        {
            DataSet dsCalendar = new DataSet();
            DataTable dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("TypeId");
            dt.Columns.Add("sDate");
            dt.Columns.Add("TypeName");

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                string sId = dr["Id"].ToString();
                string sType = dr["VacationTypeId"].ToString();
                string sFrom = dr["VacationFrom"].ToString();
                string sTo = dr["VacationTo"].ToString();
                string sTypeName = dr["VacationType"].ToString();
                DateTime sDateFrom = Convert.ToDateTime(sFrom);
                DateTime sDateTo = Convert.ToDateTime(sTo);
                if ((sTypeName == "Holiday") || (dr["Status"].ToString() == "Approved"))
                {
                    for (int i = 0; sDateFrom.AddDays(i) <= sDateTo; i++)
                    {
                        dt.Rows.Add(sId, sType, String.Format("{0:yyyy-MM-dd}", sDateFrom.AddDays(i + 1)), sTypeName);
                    }
                }
            }
            dsCalendar.Tables.Add(dt);
            return dsCalendar;
        }

        public DataSet[] GetUserYearRequests(int iUserId, int iYearId)
        {
            DataSet[] dsArrayReturn = new DataSet[2];
            DataSet ds = CUser.GetUserVacationsAndDaysOff(iUserId, iYearId);
            dsArrayReturn[0] = ds;
            dsArrayReturn[1] = CalendarDates(ds);
            return dsArrayReturn;
        }

        public DataSet[] GetHolidaysCalendar(int iUserId)
        {
            DataSet[] dsArrayReturn = new DataSet[2];
            DataSet ds = CUserVacations.GetListForWorkOnHoliday(CUserInfo.ActualSystemId, CUserInfo.UserId);
            dsArrayReturn[0] = ds;
            dsArrayReturn[1] = CalendarDates(ds);
            return dsArrayReturn;
        }
    }
}
