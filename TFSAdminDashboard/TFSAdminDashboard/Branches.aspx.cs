using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TFSAdminDashboard
{
    public partial class About : Page
    {

        string response;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Get the list of TFS branches for given team project
                SourceCode.Object result = SourceCode.GetTFSBranches("$/" + TFSAdminDashboard.Properties.Settings.Default.TeamProject);

                //seperate only MAIN branches
                var mainBranches = result.value.Where(x => x.path.ToUpper().EndsWith("/MAIN"))
                                                      .Select(x => x.path.Replace("/MAIN", "")
                                                      .Replace("$/" + TFSAdminDashboard.Properties.Settings.Default.TeamProject + "/", "")
                                                     ).OrderBy(s=>s, StringComparer.CurrentCultureIgnoreCase);

                //Bind Main branches with chekbox list for user view
                cblistBranches.DataSource = mainBranches;
                cblistBranches.DataBind();
            }
        }


        protected void btnCreateBranch_Click(object sender, EventArgs e)
        {
            if (cblistBranches.Items.Cast<ListItem>().Where(li => li.Selected).ToList().Count > 0)
            {
                if (!string.IsNullOrEmpty(txbRelName.Text) && !string.IsNullOrWhiteSpace(txbRelName.Text))
                {
                    //Validate entered release number and trim extra/leading spaces
                    string releasenum = SourceCode.ValidateRelTextBox(txbRelName.Text);
                    response = string.Empty;

                    foreach (ListItem branchName in cblistBranches.Items.Cast<ListItem>().Where(li => li.Selected).ToList())
                    {
                        //Create Release Branch
                        string changesetNum = SourceCode.CreateBranch(branchName.Value, releasenum);

                        if (string.IsNullOrEmpty(changesetNum))
                        {
                            response = response + branchName.Value + " - Branch Already Exists <br />";
                        }
                        else
                        {
                            response = response + branchName.Value + " - " + changesetNum + "<br />";
                        }
                    }

                    //Write response to popup message
                    response+= string.Format("<br /><a class=info href={0}/{1}/_versionControl?path=%24%2F{1}%2F_Releases%2F{2} Target=_blank>Show Release Branches</a> <br />"
                                               , TFSAdminDashboard.Properties.Settings.Default.TFSUrl
                                               , TFSAdminDashboard.Properties.Settings.Default.TeamProject.Replace(" ", "%20")
                                               , releasenum);
                    ShowMessage(response);
                    txbRelName.Text = string.Empty;
                    cblistBranches.ClearSelection();
                }
                else
                {
                    ShowMessage("Please enter valid Release Number....!");                    
                }
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