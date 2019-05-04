using Microsoft.TeamFoundation.VersionControl.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;

namespace TFSAdminDashboard
{
    public static class SourceCode
    {
        #region Properties

        public class value
        {
            public string path { get; set; }
        }

        public class Object
        {
            public int count { get; set; }
            public IList<value> value { get; set; }
        }

        public class changeset
        {
            public string changesetId { get; set; }
            public string createdDate { get; set; }
            public string comment { get; set; }
            public object checkedInBy { get; set; }

        }        
        public class ChangeSets
        {
            public int count { get; set; }
            public IList<changeset> value { get; set; }
        }

        //public class Changes
        //{
        //    public int count { get; set; }
        //    public IList<change> value { get; set; }
        //}
        //public class change
        //{
        //    public string changeType { get; set; }
        //    public string[] item { get; set; }
        //}
        //public class Items
        //{
        //    public string url { get; set; }
        //    public string path { get; set; }
        //}
            #endregion
            /// <summary>
            /// Gets all available branches for given tfs scope path
            /// </summary>
            /// <param name="scopePath">TFS Server path</param>
            /// <returns></returns>
            public static Object GetTFSBranches(string scopePath)
        {
            JSONHelper _jsonHelper = new JSONHelper();
            string result = _jsonHelper.GetTFSJsonData(TFSAdminDashboard.Properties.Settings.Default.TFSUrl +
                                                        "/_apis/tfvc/branches?scopePath=" + scopePath
                                                        + "&includeDeleted=false&api-version=2.0");


            if (!string.IsNullOrEmpty(result))
            {
                return JsonConvert.DeserializeObject<Object>(result);
            }
            else
            {
                return null;
            }
        }

        public static Object GetTFSitems(string scopePath)
        {
            JSONHelper _jsonHelper = new JSONHelper();
            string result = _jsonHelper.GetTFSJsonData(TFSAdminDashboard.Properties.Settings.Default.TFSUrl +
                                                        "/_apis/tfvc/items?scopePath=" + scopePath
                                                        + "&includeDeleted=false&api-version=2.0");


            if (!string.IsNullOrEmpty(result))
            {
                return JsonConvert.DeserializeObject<Object>(result);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Validate user entry of Release number. Make sure to trip and eliminate extra white spaces.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>returns fromatted release number</returns>
        public static string ValidateRelTextBox(string text)
        {
            if (Regex.Replace(text, @"[^A-Za-z]+", String.Empty).ToUpper() == "MR")
            {
                return string.Format("MR-{0}", Regex.Replace(text, @"[^\d]", ""));
            }
            else if (Regex.Replace(text, @"[^A-Za-z]+", String.Empty).ToUpper() == "HF")
            {
                return string.Format("HF-{0}", Regex.Replace(text, @"[^\d]", ""));
            }
            else
            {
                return text.Trim();
            }
        }
        /// <summary>
        /// Create new Release branch for given application
        /// </summary>
        /// <param name="applicationName"> Application Name</param>
        /// <param name="releaseNum"> Release Number</param>
        /// <returns>Returns changeset number or null if the branch already exists</returns>
        public static string CreateBranch(string applicationName, string releaseNum)
        {
            VersionControlServer vcs = TFSConnect.GetVersionControlServer();

            string _mainBranchPath = string.Format("$/{0}/{1}/MAIN", TFSAdminDashboard.Properties.Settings.Default.TeamProject, applicationName);
            string _releaseBranchPath = string.Format("$/{0}/_Releases/{2}/{1}", TFSAdminDashboard.Properties.Settings.Default.TeamProject, applicationName, releaseNum);

            if (vcs.ServerItemExists(_releaseBranchPath, ItemType.Any))
            {
                return null;
            }
            else
            {
                return vcs.CreateBranch(_mainBranchPath, _releaseBranchPath, VersionSpec.Latest).ToString();
            }
        }
        /// <summary>
        /// Merge branches for all given list of applications
        /// </summary>
        /// <param name="branchesCheckedNodes">List of user selected release branches</param>
        /// <returns>Returns message with merge status</returns>
        public static string MergeBranch(TreeNodeCollection branchesCheckedNodes)
        {
            string responseMessage = string.Empty;
            Object result = SourceCode.GetTFSBranches("$/" + TFSAdminDashboard.Properties.Settings.Default.TeamProject);

            VersionControlServer vcs = TFSConnect.GetVersionControlServer();
            Workspace workspace = CreateWorkspace(vcs); //Create Workspace


            foreach (TreeNode item in branchesCheckedNodes)
            {
                responseMessage = responseMessage + "========== Application: " + item.Parent.Text + "-" + item.Text + "============ <br />";

                string mergeStatus = string.Empty;

                if (!item.Text.ToUpper().Replace(" ", "").Contains("VISUALCOBOL"))
                {
                    List<value> activeRelBranches = result.value.Where(z => z.path.EndsWith("/" + item.Text) & !z.path.Replace(" ","").ToUpper().Contains("VISUALCOBOL")).ToList();
                    activeRelBranches.RemoveAll(x => x.path.ToUpper().Replace(" ", "").Contains("VISUALCOBOL/"+item.Text.ToUpper()));
                    string mainBranchPath = result.value.Where(y => y.path.EndsWith("/" + item.Text + "/MAIN")).FirstOrDefault().path;
                    mergeStatus = DoMerge(item.Value, mainBranchPath, workspace);

                    if (mergeStatus.Contains("Conflict occurred")) return mergeStatus;

                    foreach (value relBranchPath in activeRelBranches)
                    {
                        if (!relBranchPath.path.Contains(item.Value))
                            mergeStatus += DoMerge(mainBranchPath, relBranchPath.path, workspace);
                    }

                    responseMessage += mergeStatus;
                }
                else
                {
                    responseMessage += item.Text+" -<mark>**Can't Merge VISUAL COBOL with Tool, Try manual Merge**</mark><br /><br />";
                }
            }

            RemoveWorkSpace(vcs, workspace);
            return responseMessage;
        }
        /// <summary>
        /// Merge the changes with latest version from given target path to source path
        /// </summary>
        /// <param name="sourcePath">SOurce path</param>
        /// <param name="targetPath">Target Path</param>
        /// <param name="workspace">Workspace to use for merge</param>
        /// <returns>Merge status message</returns>
        private static string DoMerge(string sourcePath, string targetPath, Workspace workspace)
        {
            GetStatus mainMergeStatus = workspace.Merge(sourcePath, targetPath, null, VersionSpec.Latest, LockLevel.Unchanged, RecursionType.Full, MergeOptions.None);

            if (mainMergeStatus.NumFailures > 0)
            {
                string _failureMessages = string.Empty;
                foreach (Failure message in mainMergeStatus.GetFailures())
                {
                    _failureMessages += "\t\u2022"+message.Message;
                }
                return string.Format("<mark>Merging changes from {0} to {1} Failed:</mark> <br />{2}", sourcePath, targetPath, _failureMessages);
            }
            else
            {
                if (mainMergeStatus.NumConflicts + mainMergeStatus.NumFailures + mainMergeStatus.NumUpdated < 1)
                {
                    return string.Format("<mark>**Nothing to Merge** from {0} to {1}</mark><br /><br />", sourcePath, targetPath);
                }

                Conflict[] objConflicts = workspace.QueryConflicts((new string[] { targetPath }), true);
                foreach (Conflict conflict in objConflicts)
                {
                    workspace.MergeContent(conflict, false);
                    if (conflict.ContentMergeSummary.TotalConflicting == 0)
                    {
                        conflict.Resolution = Resolution.AcceptMerge;
                        workspace.ResolveConflict(conflict);
                    }
                }
                Conflict[] conflicts = workspace.QueryConflicts((new string[] { targetPath }), true);
                if (conflicts.Length > 1)
                {
                    return string.Format("<mark>Conflict occurred when merge from {0} to {1}. Please merge branches manually</mark> <br /><br />", sourcePath, targetPath);
                }

                PendingChange[] pendingChanges = workspace.GetPendingChanges(targetPath, RecursionType.Full);
                string CheckInComment = "Merging changes from " + sourcePath + " to " + targetPath;
                WorkspaceCheckInParameters wcip = new WorkspaceCheckInParameters(pendingChanges, CheckInComment);
                int newChangesetID = workspace.CheckIn(wcip);

                string response = string.Format("Merging changes from {0} to {1} Completed: #<a class=info href={2}/{3}/_versionControl/changeset/{4} Target=_blank>{4}</a><br /><br />"
                                                , sourcePath, targetPath, TFSAdminDashboard.Properties.Settings.Default.TFSUrl
                                                , TFSAdminDashboard.Properties.Settings.Default.TeamProject.Replace(" ", "%20"), newChangesetID
                                               );
                return response;

            }
        }
        /// <summary>
        /// Create TFS workspace
        /// </summary>
        /// <returns>Workspace</returns>
        public static Workspace CreateWorkspace(VersionControlServer vcs)
        {
            WorkingFolder sourceFolder = null;
            string workspaceName = string.Empty;
            workspaceName = String.Format("{0}-{1}", Environment.MachineName, DateTime.Now.ToString("yyyyMMddHHmmss"));
            Workspace workspace = null;
            string serverFolderName = "$/" + TFSAdminDashboard.Properties.Settings.Default.TeamProject;
            string localFolderNameToMap = TFSAdminDashboard.Properties.Settings.Default.LocalFolderToMap + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss");
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
        public static void RemoveWorkSpace(VersionControlServer vcs, Workspace workspace)
        {
            if (workspace != null)
            {
                vcs.DeleteWorkspace(workspace.Name, workspace.OwnerName);
            }
        }

        public static ChangeSets GetChangesets(string scopePath)
        {   
            JSONHelper _jsonHelper = new JSONHelper();
            string result = _jsonHelper.GetTFSJsonData(string.Format("{0}/{1}/_apis/tfvc/changesets?api-version=1.0&maxCommentLength=5000&$top=5000&searchCriteria.itemPath={2}", 
                                                        TFSAdminDashboard.Properties.Settings.Default.TFSUrl, 
                                                        TFSAdminDashboard.Properties.Settings.Default.TeamProject, 
                                                        scopePath));

            if (!string.IsNullOrEmpty(result))
            {
                return JsonConvert.DeserializeObject<ChangeSets>(result);
            }
            else
            {
                return null;
            }
        }

        //public static Changes GetChanges(string changesetID)
        //{
        //    JSONHelper _jsonHelper = new JSONHelper();
        //    string result = _jsonHelper.GetTFSJsonData(string.Format("{0}/_apis/tfvc/changesets/{1}/changes?api-version=1.0",
        //                                                TFSAdminDashboard.Properties.Settings.Default.TFSUrl, changesetID));

        //    if (!string.IsNullOrEmpty(result))
        //    {
        //        return JsonConvert.DeserializeObject<Changes>(result);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        public static string DeleteBranch(TreeNodeCollection treeviewNodes)
        {
            VersionControlServer vcs = TFSConnect.GetVersionControlServer();

            Workspace workspace = CreateWorkspace(vcs);
            List<string> pathsToDelete = new List<string>();
            List<string> rootPathsToDelete = new List<string>();
            string response = null;

            foreach (TreeNode item in treeviewNodes[0].ChildNodes)
            {
                if (IsAllChildChecked(item.ChildNodes))
                {
                    rootPathsToDelete.Add(string.Format("$/{0}/_Releases/{1}", TFSAdminDashboard.Properties.Settings.Default.TeamProject, item.Text));
                }
                //else
                //{
                    foreach (TreeNode childItem in item.ChildNodes)
                    {
                        if (childItem.Checked)
                            pathsToDelete.Add(childItem.Value);
                    }
                //}
            }
            pathsToDelete = pathsToDelete.Distinct().ToList();
            foreach (string path in pathsToDelete)
            {
                workspace.PendDelete(path);

                if (workspace.GetPendingChanges().Length > 0)
                {
                    int changesetID = workspace.CheckIn(workspace.GetPendingChanges(), "Archive the old release Branch by TFS Admin Tool");

                    response += string.Format("Deleted - {3}: #<a class=info href={0}/{1}/_versionControl/changeset/{2} Target=_blank>{2}</a><br /><br />"
                                                    , TFSAdminDashboard.Properties.Settings.Default.TFSUrl
                                                    , TFSAdminDashboard.Properties.Settings.Default.TeamProject.Replace(" ", "%20")
                                                    , changesetID, path.Substring(path.LastIndexOf("/")+1));
                }
                else
                {
                    response += string.Format("<mark>*** Delete Failed - {0}</mark><br />", path);
                }
            }

            foreach (string rootPath in rootPathsToDelete)
            {
                if (GetTFSBranches(rootPath).count < 1)
                {
                    workspace.PendDelete(rootPath);
                    if (workspace.GetPendingChanges().Length > 0)
                    {
                        workspace.CheckIn(workspace.GetPendingChanges(), "Archiving root folder of the old release Branch by TFS Admin Tool");
                    }
                }
            }
                return response;
        }

        private static bool IsAllChildChecked(TreeNodeCollection nodeCollection)
        {
            bool allChildChecked = true;
            foreach (TreeNode childNode in nodeCollection)
            {
                if (!childNode.Checked)
                {
                    allChildChecked = false;
                }
            }
            return allChildChecked;
        }

    }
}