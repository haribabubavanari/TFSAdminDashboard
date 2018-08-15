using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TFSAdminDashboard
{
    public partial class ActiveReleases : System.Web.UI.Page
    {

        List<Builds.Value> releases = new List<Builds.Value>();
       // List<Value> activeBuilds = new List<Value>();
        List<ListItem> _ActivebuildsList = new List<ListItem>();
        List<string> _currentReleases = new List<string>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                foreach (Builds.Value item in Builds.ListAllBuilds())
                {
                    if (!item.name.Contains("MAIN"))
                    {
                        _ActivebuildsList.Add(new ListItem(item.name, item.id.ToString()));
                        _currentReleases.Add(item.name.Split('_')[0]);
                    }
                }

                _currentReleases = _currentReleases.Distinct().ToList();
                DataTable gridData = new DataTable();
                gridData.Columns.Add("ActiveReleases");
                foreach (string rel in _currentReleases)
                {
                    gridData.Rows.Add(rel);
                }
                
               
                gvActiveReleases.DataSource = gridData;
                gvActiveReleases.DataBind();
            }
        }
                
        protected void gvActiveReleases_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string requestedRowText = gvActiveReleases.DataKeys[e.Row.RowIndex].Value.ToString();
                DataTable gridData1 = new DataTable();
                gridData1.Columns.Add("Releases");

                foreach (Builds.Value item in Builds.ListAllBuilds())
                {                    
                    if (item.name.Contains(requestedRowText))
                    {
                        gridData1.Rows.Add(item.name.Replace(requestedRowText+"_",""));
                    }
                }

                //gridData1.Rows.Add("Phoenix-UI");
                //gridData1.Rows.Add("AccidentEntry");

                GridView gvActiveBuilds = e.Row.FindControl("gvActiveBuilds") as GridView;
                //GridView gvOrders = e.Row.FindControl("gvOrders") as GridView;
                gvActiveBuilds.DataSource = gridData1;
                gvActiveBuilds.DataBind();


                //string customerId = gvActiveBuilds.DataKeys[e.Row.RowIndex].Value.ToString();


            }
        }

        protected void gvActiveBuilds_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // string requestedRowvalue = gvActiveBuilds.DataKeys[e.Row.RowIndex].Value.ToString();
                DataTable gridData3 = new DataTable();
                gridData3.Columns.Add("Environments");
                gridData3.Rows.Add("Test-1");
                gridData3.Rows.Add("Test-2");
                gridData3.Rows.Add("PREP");
                gridData3.Rows.Add("PROD-1");
                gridData3.Rows.Add("PROD-2");
                gridData3.Rows.Add("PROD-3");

                gridData3.Columns.Add("EnvStatus");
                //gridData.Rows.Add("Succeeded");
                //gridData.Rows.Add("FAILED");
                //gridData.Rows.Add("Not yet Started");
                //gridData.Rows.Add("Not yet Started");
                //gridData.Rows.Add("Not yet Started");
                //gridData.Rows.Add("Not yet Started");

                GridView gvOrders = e.Row.FindControl("gvOrders") as GridView;
                gvOrders.DataSource = gridData3;
                gvOrders.DataBind();
            }
        }
    }
}