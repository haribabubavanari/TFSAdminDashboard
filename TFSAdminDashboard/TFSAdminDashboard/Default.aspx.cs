using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using WebApiRelease = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release;
namespace TFSAdminDashboard
{
    public partial class ActiveReleases : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Load Top DataGrid with all current active releases Eg: MR-271, HF-32, HTTPS
                DataTable activeReleasesData = new DataTable();
                activeReleasesData.Columns.Add("ActiveReleases");
                             

                List<string> archivedList = TFSAdminDashboard.Properties.Settings.Default.ArchivedReleasesList.Cast<string>().ToList();
                foreach (Builds.Value item in Builds.ListAllBuilds())
                {
                    if (!item.name.Contains("MAIN") && !archivedList.Any(s =>item.name.Contains(s)))
                    {
                        activeReleasesData.Rows.Add(item.name.Split('_')[0]);
                    }
                }

                DataView view = new DataView(activeReleasesData);
                //Remove duplicates from Table
                activeReleasesData = view.ToTable(true, "ActiveReleases");

                gvActiveReleases.DataSource = activeReleasesData;
                gvActiveReleases.DataBind();
            }
        }

        protected void gvActiveReleases_OnRowDataBound(object sender, GridViewRowEventArgs e)
        { }

        protected void gvActiveBuilds_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //Bind all builds for each application specific to a release
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList drpdwnBuilds = e.Row.FindControl("drpdwnBuilds") as DropDownList;
                List<Builds.BuildsValue> allBuilds = Builds.GetBuildsforDef(e.Row.Cells[1].Text);

                foreach (Builds.BuildsValue value in allBuilds)
                {
                    drpdwnBuilds.Items.Add(new ListItem(value.buildNumber + "  ##" + value.result, value.id.ToString()));
                }

                ShowLogs(e.Row);
            }
        }
        /// <summary>
        /// This method will populate the logs hyperlinks for each application release.
        /// </summary>
        /// <param name="gvr">grid view row to populate logs hyperlinks</param>
        private void ShowLogs(GridViewRow gvr)
        {
            DropDownList drpdwnBuilds = gvr.FindControl("drpdwnBuilds") as DropDownList;
            HyperLink linkBuildLog = gvr.FindControl("linkBuildLog") as HyperLink;
            HyperLink linkReleaseLog = gvr.FindControl("linkReleaseLog") as HyperLink;

            if (drpdwnBuilds.SelectedIndex >= 0)
            {
                //Build log hyperlink
                linkBuildLog.Visible = true;
                linkBuildLog.NavigateUrl = string.Format("{0}/{1}/_build?_a=summary&buildId={2}"
                                                        , TFSAdminDashboard.Properties.Settings.Default.TFSUrl
                                                        , TFSAdminDashboard.Properties.Settings.Default.TeamProject
                                                        , drpdwnBuilds.SelectedValue);
                //Release log hyperlink will not show if build failed. Because, there will be no deployment on failed build.
                if (!drpdwnBuilds.SelectedItem.Text.Contains("failed"))
                {
                    //Release log hyperlink
                    linkReleaseLog.Visible = true;
                    linkReleaseLog.NavigateUrl = string.Format("{0}/{1}/_apps/hub/ms.vss-releaseManagement-web.hub-explorer?_a=release-summary&releaseId={2}"
                                                            , TFSAdminDashboard.Properties.Settings.Default.TFSUrl
                                                            , TFSAdminDashboard.Properties.Settings.Default.TeamProject
                                                            , Builds.GetReleaseIDforBuildID(drpdwnBuilds.SelectedValue));
                }
                else
                {
                    linkReleaseLog.Visible = false;
                }
            }
            else
            {
                linkReleaseLog.Visible = false;
                linkBuildLog.Visible = false;
            }
        }

        protected void btnQueueNewBuild_Click(object sender, EventArgs e)
        {
            string response = string.Empty;
            Button btn = (Button)sender;
            //Get the associated button click row details
            GridViewRow gvr = (GridViewRow)btn.NamingContainer;

            //Queue new build
            Build newBuild = Builds.QueueBuild(int.Parse(gvr.Cells[1].Text));
            response = response + "Queue New build complete - " + newBuild.BuildNumber + " <br />   -Build Status:" + newBuild.Result.ToString() + " <br />";
            //Start release defintion if build is succeeded
            if (newBuild.Result.ToString().ToUpper() == "SUCCEEDED")
            {
                Builds.Value releaseDef = Builds.ListAllReleases().Where(x => x.name.Contains(newBuild.Definition.Name)).FirstOrDefault();
                //Queue new Release
                WebApiRelease newRelease = Builds.CreateRelease(releaseDef.id, newBuild);
                response = response + "Queue new Release complete - " + newRelease.Name + " <br />";
            }
            //update builds dropdown with new (current) Build
            DropDownList drpdwnBuilds = gvr.FindControl("drpdwnBuilds") as DropDownList;
            drpdwnBuilds.Items.Insert(0, new ListItem(newBuild.BuildNumber + "  ##" + newBuild.Result, newBuild.Id.ToString()));
            drpdwnBuilds.ClearSelection();

            //Repopulate environments for newly queue build and release
            BindReleaseEnvData(gvr, drpdwnBuilds.SelectedValue);
            //update logs hyperlinks
            ShowLogs(gvr);
            //show popup message
            lblModalTitle.Text = "Message";
            lblModalBody.Text = response;
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$('#myModal').modal();", true);
            upModal.Update();

        }

        protected void btnDeploy_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            GridViewRow gvr = (GridViewRow)btn.NamingContainer;
            //start deployment for selected environment
            string envStatus = Builds.StartDeployment(int.Parse(gvr.Cells[0].Text), int.Parse(gvr.Cells[1].Text));
            gvr.Cells[3].Text = envStatus;
            //Show green for success and red for fail
            SetEnvColor(gvr);
        }

        protected void grdRelease_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            SetEnvColor(e.Row);
        }

        private void SetEnvColor(GridViewRow dr)
        {
            if (dr.RowType == DataControlRowType.DataRow)
            {
                if (dr.Cells[3].Text == "Rejected")
                {
                    dr.Cells[3].BackColor = System.Drawing.Color.Red;
                }
                else if (dr.Cells[3].Text == "Succeeded")
                {
                    dr.Cells[3].BackColor = System.Drawing.Color.Green;
                }
            }
        }
        protected void drpdwnBuilds_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            GridViewRow gvr = (GridViewRow)ddl.NamingContainer;
            BindReleaseEnvData(gvr, ddl.SelectedValue);
        }

        protected void btnCollapse_Click(object sender, EventArgs e)
        {
            //Binding data for Child Grid - Show all associated applications and its builds
            Button btn = (Button)sender;
            if (btn.Text == "-")
            {
                btn.Text = "+";
                GridViewRow gvr = (GridViewRow)btn.NamingContainer;
                Panel pnlRelease = gvr.FindControl("Panel1") as Panel;
                pnlRelease.Visible = false;

            }
            else
            {
                btn.Text = "-";
                GridViewRow gvr = (GridViewRow)btn.NamingContainer;
                Panel pnlRelease = gvr.FindControl("Panel1") as Panel;
                pnlRelease.Visible = true;

                if (gvr.RowType == DataControlRowType.DataRow)
                {
                    string requestedRowText = gvr.Cells[1].Text;
                    DataTable gridData1 = new DataTable();

                    gridData1.Columns.Add("ID", typeof(int));
                    gridData1.Columns.Add("Releases", typeof(string));

                    foreach (Builds.Value item in Builds.ListAllBuilds())
                    {
                        if (item.name.Contains(requestedRowText))
                        {
                            DataRow dr = gridData1.NewRow();
                            dr["ID"] = item.id;
                            dr["Releases"] = item.name.Replace(requestedRowText + "_", "");
                            gridData1.Rows.Add(dr);
                        }
                    }
                    GridView gvActiveBuilds = gvr.FindControl("gvActiveBuilds") as GridView;
                    gvActiveBuilds.DataSource = gridData1;
                    gvActiveBuilds.DataBind();
                }
            }
        }

        protected void btnNestedCollapse_Click(object sender, EventArgs e)
        {
            //Binding Nested child grid data - Show all release environments
            Button btn = (Button)sender;
            if (btn.Text == "-")
            {
                btn.Text = "+";
                GridViewRow gvr = (GridViewRow)btn.NamingContainer;
                Panel pnlRelease = gvr.FindControl("pnlRelease") as Panel;
                pnlRelease.Visible = false;

            }
            else
            {
                btn.Text = "-";
                GridViewRow gvr = (GridViewRow)btn.NamingContainer;
                Panel pnlRelease = gvr.FindControl("pnlRelease") as Panel;
                pnlRelease.Visible = true;

                if (gvr.RowType == DataControlRowType.DataRow)
                {
                    DropDownList drpdwnBuilds = gvr.FindControl("drpdwnBuilds") as DropDownList;

                    if (drpdwnBuilds.SelectedIndex < 0)
                    {
                        pnlRelease.Visible = false;
                    }
                    else
                    {
                        pnlRelease.Visible = true;
                        BindReleaseEnvData(gvr, drpdwnBuilds.SelectedValue);
                    }
                }
            }
        }

        private void BindReleaseEnvData(GridViewRow gvr, string buildID)
        {
            WebApiRelease releaseDef = Builds.GetReleaseforBuildID(buildID);

            if (releaseDef != null)
            {
                // string requestedRowvalue = gvActiveBuilds.DataKeys[e.Row.RowIndex].Value.ToString();
                DataTable gridData3 = new DataTable();
                gridData3.Columns.Add("ReleaseID");
                gridData3.Columns.Add("EnvID");
                gridData3.Columns.Add("Environments");
                gridData3.Columns.Add("EnvStatus");
                foreach (ReleaseEnvironment env in releaseDef.Environments)
                {
                    DataRow dr = gridData3.NewRow();
                    dr["ReleaseID"] = releaseDef.Id;
                    dr["EnvID"] = env.Id;
                    dr["Environments"] = env.Name;
                    dr["EnvStatus"] = env.Status;
                    gridData3.Rows.Add(dr);
                }

                Panel pnlRelease = gvr.FindControl("pnlRelease") as Panel;
                pnlRelease.Visible = true;

                GridView grdRelease = gvr.FindControl("grdRelease") as GridView;
                grdRelease.DataSource = gridData3;
                grdRelease.DataBind();
            }
            else
            {
                Panel pnlRelease = gvr.FindControl("pnlRelease") as Panel;
                pnlRelease.Visible = false;
            }
        }
        
    }
}