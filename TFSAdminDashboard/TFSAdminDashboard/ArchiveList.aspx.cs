using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace TFSAdminDashboard
{
    public partial class ArchiveList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ArchiveListBox.DataSource = Properties.Settings.Default.ArchivedReleasesList.Cast<string>().ToList();
            ArchiveListBox.DataBind();
            //var listivew = new ListView();
            //foreach(string item in Properties.Settings.Default.ArchivedReleasesList.Cast<string>().ToList())
            //{
            //    listivew.Items.Add(new ListItem(item);
            //}
            //ListViewDataItem x = new ListViewDataItem(;

            //ListView1.Items.Add()

            //     ArrayList authors = new ArrayList();
            //authors.Add("Mahesh Chand");
            //authors.Add("Mike Gold");
            //authors.Add("Raj Kumar");
            //authors.Add("Praveen Kumar");
            //ListView1.Items = authors;

            //ArchiveListView.DataBind();
        }

        private void AddArchiveList()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            XmlElement elem = xmlDoc.CreateElement("string");
            elem.InnerText = "Test1";
            XmlElement elx = (XmlElement)xmlDoc.SelectSingleNode("configuration/applicationSettings/WebApplication2.Properties.Settings/setting[@name='Setting4']/value/ArrayOfString");
            elx.AppendChild(elem);

            xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            ConfigurationManager.RefreshSection("applicationSettings");
        }


        private void RemoveArchiveList()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            XmlNodeList nodes = xmlDoc.SelectNodes("configuration/applicationSettings/WebApplication2.Properties.Settings/setting[@name='Setting4']/value");
            XmlNodeList childNodes = nodes[0].ChildNodes[0].ChildNodes;

            for (int i = childNodes.Count - 1; i >= 0; i--)
            {
                if (childNodes[i].InnerText == "Test1")
                    childNodes[i].ParentNode.RemoveChild(childNodes[i]);
            }

            xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            ConfigurationManager.RefreshSection("applicationSettings");
        }

        protected void btnAddRelease_Click(object sender, EventArgs e)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            XmlElement elem = xmlDoc.CreateElement("string");
            //elem.InnerText = "Test1";
            elem.InnerText = txbRelease.Text;
            XmlElement elx = (XmlElement)xmlDoc.SelectSingleNode("configuration/applicationSettings/TFSAdminDashboard.Properties.Settings/setting[@name='ArchivedReleasesList']/value/ArrayOfString");
            elx.AppendChild(elem);

            xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            ConfigurationManager.RefreshSection("applicationSettings");
        }

        protected void btnDeleteRelease_Click(object sender, EventArgs e)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

            XmlNodeList nodes = xmlDoc.SelectNodes("configuration/applicationSettings/TFSAdminDashboard.Properties.Settings/setting[@name='ArchivedReleasesList']/value/ArrayOfString");
            XmlNodeList childNodes = nodes[0].ChildNodes[0].ChildNodes;

            for (int i = childNodes.Count - 1; i >= 0; i--)
            {
                if (childNodes[i].InnerText == txbRelease.Text)
                    childNodes[i].ParentNode.RemoveChild(childNodes[i]);
            }

            xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            ConfigurationManager.RefreshSection("applicationSettings");
        }

        protected void ArchiveListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var obj = ArchiveListBox.SelectedValue;
        }
    }
}