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
using WindowsCredential = Microsoft.VisualStudio.Services.Common.WindowsCredential;

namespace TFSAdminDashboard
{
    public partial class Contact : Page
    {
        List<string> branches = new List<string>();
        private Workspace workspace = null;
        private String workspaceName = string.Empty;
        VersionControlServer vcs = null;
        private WorkingFolder sourceFolder = null;
    
        TfsTeamProjectCollection tfs;
        string response=string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            string TeamProject = TFSAdminDashboard.Properties.Settings.Default.TeamProject;

            if (!IsPostBack)
            {
                GetTFSBranches("$/" + TeamProject);
                
                cblistMergeBranch.DataSource = branches.Where(x => x.Contains("/MR-")).Select(x => x.Replace("$/" + TeamProject + "/_Releases/", "")); 
                cblistMergeBranch.DataBind();

                cblistHFMergeBranch.DataSource = branches.Where(x => x.Contains("/HF-")).Select(x => x.Replace("$/" + TeamProject + "/_Releases/", ""));
                cblistHFMergeBranch.DataBind();

                cblistProjMergeBranch.DataSource = branches.Where(x => x.Contains("/MR-")).Select(x => x.Replace("$/" + TeamProject + "/_Releases/", ""));
                cblistProjMergeBranch.DataBind();
            }
        }

        public void GetTFSBranches(string scopePath)
        {

            JSONHelper _jsonHelper = new JSONHelper();
            string result = _jsonHelper.GetTFSJsonData(TFSAdminDashboard.Properties.Settings.Default.TFSUrl
                                                        + "/_apis/tfvc/branches?scopePath=" + scopePath
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

        protected void btnMergeBranch_Click(object sender, EventArgs e)
        {

            List<ListItem> selectedItems = cblistMergeBranch.Items.Cast<ListItem>().Where(li => li.Selected).ToList();

            //tfs = new TfsTeamProjectCollection(new Uri(serverName));
            //vcs = tfs.GetService<VersionControlServer>();
            //workspace = CreateWorkspace();

            foreach (ListItem item in selectedItems)
            {
                string applicationName = item.Text.Split('/').Last();
                response = response + "========== Application: " + applicationName + "============ <br />";
                List <string> activeBranches = new List<string>();
                GetTFSBranches("$/" + TFSAdminDashboard.Properties.Settings.Default.TeamProject + "/_Releases/");
                activeBranches = branches.Where(x => x.Contains(applicationName)).ToList();
                bool isMainConflict = false;
                //  activeBranches.Add(string.Format("$/{0}/{1}/MAIN", TeamProject, applicationName));

                isMainConflict= MergeBranch(string.Format("$/{0}/_Releases/{1}", TFSAdminDashboard.Properties.Settings.Default.TeamProject,item.Text)
                                            ,string.Format("$/{0}/{1}/MAIN", TFSAdminDashboard.Properties.Settings.Default.TeamProject, applicationName));
                //Merge to MAIN
               
             
                if (!isMainConflict)
                {
                    //Merge to other Active Release Branches
                    foreach (string activeBranch in activeBranches)
                    {
                        if (!activeBranch.Contains(item.Value))
                        {
                            MergeBranch("$/" + TFSAdminDashboard.Properties.Settings.Default.TeamProject + "/" + applicationName + "/MAIN", activeBranch);
                        }
                    }
                }
            }

            lblModalTitle.Text = "Message";
            lblModalBody.Text = response;
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$('#myModal').modal();", true);
            upModal.Update();
        }
        

        private bool MergeBranch(string sourcePath, string targetPath)
        {
            workspace = null;

            tfs = new TfsTeamProjectCollection(new Uri(TFSAdminDashboard.Properties.Settings.Default.TFSUrl, UriKind.Absolute),
                                                      new VssCredentials(new WindowsCredential(new NetworkCredential(
                                                      TFSAdminDashboard.Properties.Settings.Default.UserName,
                                                      TFSAdminDashboard.Properties.Settings.Default.Password,
                                                      TFSAdminDashboard.Properties.Settings.Default.Domain)))
                                                      );
            vcs = tfs.GetService<VersionControlServer>();

            var x = targetPath.Split('/');
            var temp = x[x.Length - 2];

            workspace = CreateWorkspace(temp);
            
            GetStatus mergeStatus = workspace.Merge(sourcePath, targetPath, null, VersionSpec.Latest, LockLevel.Unchanged, RecursionType.Full, MergeOptions.None);

            if (mergeStatus.NumConflicts + mergeStatus.NumFailures + mergeStatus.NumUpdated == 0)
            {
                response = response + "**Nothing to Merge** from " + sourcePath + " to " + targetPath  + "<br /><br />";
                return false;
            }
            Conflict[] objConflicts = workspace.QueryConflicts((new string[] { targetPath }), true);
            foreach (Conflict conflict in objConflicts)
            {
                workspace.MergeContent(conflict, false);
                if(conflict.ContentMergeSummary.TotalConflicting == 0)
                {
                    conflict.Resolution = Resolution.AcceptMerge;
                    workspace.ResolveConflict(conflict);
                }
            }
            Conflict[] conflicts = workspace.QueryConflicts((new string[] { targetPath }), true);
            if (conflicts.Length < 1)
            {
                PendingChange[] pendingChanges = workspace.GetPendingChanges(targetPath, RecursionType.Full);
                string CheckInComment = "Merging changes from "+ sourcePath+" to " + targetPath;
                WorkspaceCheckInParameters wcip = new WorkspaceCheckInParameters(pendingChanges, CheckInComment);
                if (pendingChanges.Length > 0)
                {
                    int ci2 = workspace.CheckIn(wcip);
                    response = response + "Merging changes from " + sourcePath + " to " + targetPath + ": " + ci2 + "<br /><br />";
                }
            }
            else
            {
                response = response + "Conflict occurred when merge from" + sourcePath + " to " + targetPath + ". Please merge branches manually <br /><br />";
                return true;
            }

            RemoveWorkSpace();
            return false;
        }

        private Workspace CreateWorkspace(string folderName)
        {
            workspaceName = String.Format("{0}-{1}", Environment.MachineName, folderName);
            Workspace workspace = null;
            string serverFolderName = TFSAdminDashboard.Properties.Settings.Default.TeamProject;
            string localFolderNameToMap = "C:\\Temp22\\ToolWS2\\"+ folderName;
            try
            {
                workspace = vcs.CreateWorkspace(workspaceName);
                sourceFolder = new WorkingFolder(serverFolderName, localFolderNameToMap);
                workspace.CreateMapping(sourceFolder);
            }
            catch (WorkspaceExistsException)
            {
                workspace = vcs.GetWorkspace(workspaceName, vcs.AuthorizedUser);
            }

            return workspace;
        }
        private void RemoveWorkSpace()
        {
            if (workspace != null)
            {
                vcs.DeleteWorkspace(workspaceName, vcs.AuthorizedUser);
            }
        }


    }
}