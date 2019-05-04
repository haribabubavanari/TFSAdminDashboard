using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TFSAdminDashboard
{
    public partial class Merge : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadTreeViewData();
            }
        }
        /// <summary>
        /// This method will load all the branches
        /// </summary>
        private void LoadTreeViewData()
        {
            TreeViewBranches.Nodes.Clear();
            TreeNode node = new TreeNode();
            node.Text = "Releases";

            SourceCode.Object result = SourceCode.GetTFSBranches("$/" + TFSAdminDashboard.Properties.Settings.Default.TeamProject + "/_Releases");
            if (result != null)
            {
                List<string> releases = new List<string>();
                foreach (SourceCode.value value in result.value)
                {
                    releases.Add(value.path.Split('/', '/')[3]);
                }
                releases = releases.Distinct().ToList();

                foreach (string rel in releases)
                {
                    TreeNode relNode = new TreeNode();
                    relNode.Text = rel;
                    relNode.Value = rel;
                    node.ChildNodes.Add(relNode);
                    foreach (SourceCode.value value in result.value)
                    {
                        if (value.path.Contains("$/" + TFSAdminDashboard.Properties.Settings.Default.TeamProject + "/_Releases/" + rel))
                        {
                            TreeNode branchNode = new TreeNode();
                            branchNode.Text = Regex.Match(value.path, "(?<=" + rel + "/).*").Value;
                            branchNode.Value = value.path;
                            relNode.ChildNodes.Add(branchNode);
                        }
                    }
                }
            }

            TreeViewBranches.Nodes.Add(node);
            TreeViewBranches.ExpandAll();
        }

        protected void btnMergeBranch_Click(object sender, EventArgs e)
        {
            if (TreeViewBranches.CheckedNodes.Count > 0)
            {
                //Merge branches for selected nodes
                string response = SourceCode.MergeBranch(TreeViewBranches.CheckedNodes);
                ShowMessage(response);
                LoadTreeViewData();
            }
            else
            {
                ShowMessage("No Branches selected to merge, Please select atleast one branch for merge");
            }
        }

        protected void btnDeleteBranch_Click(object sender, EventArgs e)
        {
            if (TreeViewBranches.CheckedNodes.Count > 0)
            {
                string response = SourceCode.DeleteBranch(TreeViewBranches.Nodes);
                ShowMessage(response);
                LoadTreeViewData();
            }
            else
            {
                ShowMessage("You must select atleast one project....!");
            }
        }
        /// <summary>
        /// This method writes a popup message
        /// </summary>
        /// <param name="response">Message Text</param>
        private void ShowMessage(string response)
        {
            lblModalTitle.Text = "Message";
            lblModalBody.Text = response;
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$('#myModal').modal();", true);
            upModal.Update();
        }
    }
}