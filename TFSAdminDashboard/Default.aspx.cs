using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WindowsCredential = Microsoft.VisualStudio.Services.Common.WindowsCredential;
using Artifact = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
//using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients;

namespace TFSAdminDashboard
{
    public partial class _Default : Page
    {
        List<Builds.Value> builds = new List<Builds.Value>();
      //  List<Builds.Value> releases = new List<Builds.Value>();
        List<ListItem> _buildsList = new List<ListItem>();
        string response = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                builds = Builds.ListAllBuilds();

                foreach (Builds.Value item in builds)
                {
                    if (item.name.Contains("MAIN"))
                        _buildsList.Add(new ListItem(item.name.Replace("MAIN_", ""), item.id.ToString()));
                }
                cblistBuilds.DataSource = _buildsList;
                cblistBuilds.DataTextField = "text";
                cblistBuilds.DataValueField = "value";
                cblistBuilds.DataBind();
            }
        }


        

        protected void btnCloneBuilds_Click(object sender, EventArgs e)
        {
            List<ListItem> selectedItems = cblistBuilds.Items.Cast<ListItem>().Where(li => li.Selected).ToList();

            foreach (ListItem item in selectedItems)
            {
                BuildDefinition ClonedbuildDef = Builds.CloneBuild(int.Parse(item.Value), item.Text, txbRelease.Text);
                response = response + "Clone build complete - " + ClonedbuildDef.Name + " <br />";

                Builds.Value releaseID = Builds.ListAllReleases().Where(x => x.name.Contains("MAIN_SCDMV.Web.MemberServices")).FirstOrDefault();

                ReleaseDefinition clonedRelDef = Builds.CloneRelease(releaseID.id, ClonedbuildDef.Id, ClonedbuildDef.Name, txbRelease.Text);
                response = response + "Clone Release complete - " + clonedRelDef.Name + " <br />";
            }

            txbRelease.Text = string.Empty;
            cblistBuilds.ClearSelection();

            lblModalTitle.Text = "Message";
            lblModalBody.Text = response;
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$('#myModal').modal();", true);
            upModal.Update();
        }
    }
}