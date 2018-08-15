using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TFSAdminDashboard.Model;
using WindowsCredential = Microsoft.VisualStudio.Services.Common.WindowsCredential;

namespace TFSAdminDashboard
{
    public partial class About : Page
    {
        private String workspaceName = string.Empty;
        TfsTeamProjectCollection tfs;
        private VersionControlServer _vcs;
        string response;
        public VersionControlServer vcs
        {
            get { return _vcs; }
            set { _vcs = value; }
        }        
       
      
        List<string> branches = new List<string>();
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetTFSBranches("$/" + TFSAdminDashboard.Properties.Settings.Default.TeamProject);

                var mainBranches = branches.Where(x => x.Contains("/MAIN")).Select(x => x.Replace("/MAIN", ""));
                mainBranches = mainBranches.Select(x => x.Replace("$/" + TFSAdminDashboard.Properties.Settings.Default.TeamProject + "/", ""));
               
                cblistBranches.DataSource = mainBranches;
                cblistBranches.DataBind();
            }
        }


        public void GetTFSBranches(string scopePath)
        {
            JSONHelper _jsonHelper = new JSONHelper();
            string result = _jsonHelper.GetTFSJsonData(TFSAdminDashboard.Properties.Settings.Default.TFSUrl+
                                                        "/_apis/tfvc/branches?scopePath="+ scopePath 
                                                        + "&includeDeleted=false&api-version=2.0");
            if (!string.IsNullOrEmpty(result))
            {
                var result22 = JsonConvert.DeserializeObject<BranchesList>(result);
                foreach (BranchValue value in result22.value)
                {
                    branches.Add(value.path);
                }
            }
            else
            {
                //To-Do
            }

          


        }

        public class BranchValue
        {
            public string path { get; set; }
        }

        public class BranchesList
        {
            public int count { get; set; }
            public IList<BranchValue> value { get; set; }
        }


        protected void btnCreateBranch_Click(object sender, EventArgs e)
        {
            // Response.Write("Your Message");
            response = string.Empty;

            List<ListItem> selected = cblistBranches.Items.Cast<ListItem>().Where(li => li.Selected).ToList();
            foreach (ListItem branchName in selected)
            {
                string changesetNum = CreateBranch(branchName.Value, txbRelName.Text);
                
            }

           // Response.Write(response);

            lblModalTitle.Text = "Message";
            lblModalBody.Text = response;
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$('#myModal').modal();", true);
            upModal.Update();

            txbRelName.Text = string.Empty;
            cblistBranches.ClearSelection();

        }

        public string CreateBranch(string applicationName, string releaseNum)
        {         

            tfs = new TfsTeamProjectCollection(new Uri(TFSAdminDashboard.Properties.Settings.Default.TFSUrl, UriKind.Absolute),
                                                      new VssCredentials(new WindowsCredential(new NetworkCredential(
                                                      TFSAdminDashboard.Properties.Settings.Default.UserName,
                                                      TFSAdminDashboard.Properties.Settings.Default.Password,
                                                      TFSAdminDashboard.Properties.Settings.Default.Domain)))
                                                      );
            vcs = tfs.GetService<VersionControlServer>();

            string _mainBranchPath = string.Format("$/{0}/{1}/MAIN", TFSAdminDashboard.Properties.Settings.Default.TeamProject, applicationName);
            string _releaseBranchPath = string.Format("$/{0}/_Releases/{2}/{1}", TFSAdminDashboard.Properties.Settings.Default.TeamProject, applicationName, releaseNum);

            if (vcs.ServerItemExists(_releaseBranchPath, ItemType.Any))
            {
                // Response.Write("Branch already exists for -" + applicationName+Environment.NewLine); // return "Branch already exists";
                response = response + applicationName + " - Branch Already Exists <br />";
                return null;
            }
            else
            {
                string changeSet = vcs.CreateBranch(_mainBranchPath, _releaseBranchPath, VersionSpec.Latest).ToString();
                response = response + applicationName + " - " + changeSet + "<br />";
                return changeSet;
            }
        }

    }
}