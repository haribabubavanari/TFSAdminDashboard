using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TFSAdminDashboard
{
    public partial class BuildReleases : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Load all Releases - Builds
                LoadReleasesData();
            }

        }
        /// <summary>
        /// This method will refresh/load all releases and bind to Tree view
        /// </summary>
        private void LoadReleasesData()
        {
            //Clear treee nodes
            TreeViewReleases.Nodes.Clear();
            TreeNode node = new TreeNode();
            node.Text = "Releases";

            //Get all active builds from Team Project
            var result = Builds.ListAllBuilds();
            if (result != null)
            {
                List<string> releases = new List<string>();
                foreach (Builds.Value item in result)
                {
                    if (!item.name.Contains("MAIN"))
                    {
                        //Extract the current releases - eg: MR-270, HF-32, HTTPS
                        releases.Add(item.name.Split('_')[0]);
                    }
                }
                //remove duplicates
                releases = releases.Distinct().ToList();

                foreach (string rel in releases)
                {
                    TreeNode relNode = new TreeNode();
                    relNode.Text = rel;
                    relNode.Value = rel;
                    node.ChildNodes.Add(relNode);
                    //Get nested child nodes for each child node (child node will be release name)
                    foreach (Builds.Value value in result)
                    {
                        if (value.name.Contains(rel + "_"))
                        {
                            TreeNode childNode = new TreeNode();
                            childNode.Text = value.name;
                            childNode.Value = value.id.ToString();
                            relNode.ChildNodes.Add(childNode);
                        }
                    }
                }
            }

            TreeViewReleases.Nodes.Add(node);
            TreeViewReleases.ExpandAll();
        }

        protected void btnDeleteReleases_Click(object sender, EventArgs e)
        {
            if (TreeViewReleases.CheckedNodes.Count > 0)
            {
                //Delete selected build and release definitions
                string response = Builds.DeleteBuildReleases(TreeViewReleases.CheckedNodes);
                //Show message to user
                ShowMessage(response);
                //refresh page for user view
                LoadReleasesData();
            }
            else
            {
                ShowMessage("No releases selected to delete, Please select atleast one ......!");
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