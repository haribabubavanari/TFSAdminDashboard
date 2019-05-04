using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TFSAdminDashboard
{
    public partial class CheckInAudit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            //if (!IsPostBack)
            //{
            //    SourceCode.Object result = SourceCode.GetTFSBranches("$/" + TFSAdminDashboard.Properties.Settings.Default.TeamProject + "/_Releases");
            //    if (result != null)
            //    {
            //        foreach (SourceCode.value value in result.value)
            //        {
            //            if (value.path.Contains("$/" + TFSAdminDashboard.Properties.Settings.Default.TeamProject + "/_Releases/"))
            //            {
            //                drpdwnBranches.Items.Add(new ListItem(value.path.Replace("$/" + TFSAdminDashboard.Properties.Settings.Default.TeamProject + "/_Releases/", ""), value.path));
            //            }
            //        }
            //    }
            //}
        }

        protected void btnShowReport_Click(object sender, EventArgs e)
        {
            var changesReport = SourceCode.GetChangesets(txbPath.Text);
            DataTable gridReportData = new DataTable();

            gridReportData.Columns.Add("DateTime");
            gridReportData.Columns.Add("Changeset ID");
            gridReportData.Columns.Add("User");
            gridReportData.Columns.Add("RR Number");
            gridReportData.Columns.Add("Comment");

            foreach (SourceCode.changeset item in changesReport.value)
            {

                DataRow dr = gridReportData.NewRow();
                dr["DateTime"] = item.createdDate;
                dr["Changeset ID"] = item.changesetId;
                dr["User"] = item.checkedInBy.ToString().Split(':')[2].Replace("\"", "").Replace("uniqueName", "");
                if (!string.IsNullOrEmpty(item.comment))
                {
                    var rrNumbers = Regex.Matches(item.comment, @"(?<=(.*?)(?i)RR(?-i)[^\w]*?[_]*?[^\w]*?)\d+");
                    string rrlist = string.Empty;
                    foreach (var rrnum in rrNumbers)
                    {
                        if (string.IsNullOrEmpty(rrlist))
                        {
                            rrlist = rrnum.ToString();
                        }
                        else if (!rrlist.Contains(rrnum.ToString()))
                        {
                            rrlist = string.Format("RR{1}, {0}", rrlist, rrnum.ToString());
                        }
                    }
                    dr["RR Number"] = rrlist;
                    dr["Comment"] = item.comment;
                }
                gridReportData.Rows.Add(dr);
            }


            grdShowReport.DataSource = gridReportData;
            grdShowReport.DataBind();

        }

        protected void btnBrowse_Click(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterStartupScript(
            this.GetType(), "OpenWindow", "window.open('http://hqtfsprod:8080/tfs/south%20carolina%20dmv/South%20Carolina%20DMV/_versionControl?path=%24%2FSouth%20Carolina%20DMV%2F_Releases&_a=contents','_newtab');", true);


            var result = SourceCode.GetTFSitems("$/" + TFSAdminDashboard.Properties.Settings.Default.TeamProject + "/_Releases");
            if (result != null)
            {
                foreach (SourceCode.value value in result.value)
                {

                }
            }
        }
    }
}