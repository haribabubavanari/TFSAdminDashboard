using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TFSAdminDashboard
{
    public partial class _Default : Page
    {
        string response = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                List<ListItem> _buildsList = new List<ListItem>();
                // Get all builds and find MAIN builds which are ready to clone for new releases
                foreach (Builds.Value item in Builds.ListAllBuilds())
                {
                    if (item.name.StartsWith("MAIN"))
                        _buildsList.Add(new ListItem(item.name.Replace("MAIN_", ""), item.id.ToString()));
                }
             
                //Bind all the MAIN builds list to Checkbox List for user view
                cblistBuilds.DataSource = _buildsList.OrderBy(o=>o.Text,StringComparer.OrdinalIgnoreCase);
                cblistBuilds.DataTextField = "text";
                cblistBuilds.DataValueField = "value";
                cblistBuilds.DataBind();
            }
        }

        protected void btnCloneBuilds_Click(object sender, EventArgs e)
        {
            if (cblistBuilds.Items.Cast<ListItem>().Where(li => li.Selected).ToList().Count > 0)
            {
                if (!string.IsNullOrEmpty(txbRelease.Text) && !string.IsNullOrWhiteSpace(txbRelease.Text))
                {
                    //Validate user entry of Release name, make sure no leading or extra whitespaces
                    string relesenum = TFSAdminDashboard.SourceCode.ValidateRelTextBox(txbRelease.Text);
                    //Get list of all selected applications to clone build and releases
                    List<ListItem> selectedItems = cblistBuilds.Items.Cast<ListItem>().Where(li => li.Selected).ToList();
                    foreach (ListItem item in selectedItems)
                    {
                        //Clone Build Defintion
                        BuildDefinition ClonedbuildDef = Builds.CloneBuild(int.Parse(item.Value), item.Text, relesenum);
                        if (ClonedbuildDef != null)
                        {
                            response += string.Format("Clone build complete - <a class=info href={0}/{1}/_build?definitionId={2} Target=_blank>{3}</a> <br />"
                                                             , TFSAdminDashboard.Properties.Settings.Default.TFSUrl
                                                             , TFSAdminDashboard.Properties.Settings.Default.TeamProject.Replace(" ", "%20")
                                                             , ClonedbuildDef.Id
                                                             , ClonedbuildDef.Name);

                            //clone ReleaseDefintion
                            Builds.Value releaseID = Builds.ListAllReleases().Where(x => x.name.Contains(item.Text)).FirstOrDefault();
                            if (releaseID != null)
                            {
                                ReleaseDefinition clonedRelDef = Builds.CloneRelease(releaseID.id, ClonedbuildDef.Id, ClonedbuildDef.Name, relesenum);
                                response += string.Format("Clone Release complete - <a class=info href={0}/{1}/_apps/hub/ms.vss-releaseManagement-web.hub-explorer?definitionId={2} Target=_blank>{3}</a> <br />"
                                                                         , TFSAdminDashboard.Properties.Settings.Default.TFSUrl
                                                                         , TFSAdminDashboard.Properties.Settings.Default.TeamProject.Replace(" ", "%20")
                                                                         , clonedRelDef.Id
                                                                         , clonedRelDef.Name);
                            }
                            else
                            {
                                response += string.Format(" <mark> ***No Release defintion found to CLONE for - {0}</mark><br />", item.Text);
                            }
                        }
                        else
                        {
                            response += string.Format(" <mark> ***Cloning Build for - {0} is Failed. This application may have already cloned for this release</mark><br />", item.Text);
                        }
                    }
                    //Reset page
                    txbRelease.Text = string.Empty;
                    cblistBuilds.ClearSelection();
                    //Show popup message to user for update on user action 
                    ShowMessage(response);
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