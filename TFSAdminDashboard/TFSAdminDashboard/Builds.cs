using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.UI.WebControls;
using WebApiRelease = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release;

namespace TFSAdminDashboard
{
    public static class Builds
    {

        /// <summary>
        /// This method will list all current active build defintions for team project mentioned in config file
        /// </summary>
        /// <returns> returns a list with all builds</returns>
        public static List<Value> ListAllBuilds()
        {
            JSONHelper _jsonHelper = new JSONHelper();
            string result = _jsonHelper.GetTFSJsonData(TFSAdminDashboard.Properties.Settings.Default.TFSUrl + "/"
                                                        + TFSAdminDashboard.Properties.Settings.Default.TeamProject
                                                        + "/_apis/build/definitions?api-version=2.0");

            var resultList = JsonConvert.DeserializeObject<DefinitionsList>(result);
            return resultList.value.ToList();
        }
        /// <summary>
        /// This method will get all the builds for 
        /// </summary>
        /// <param name="buildID">Build Definition ID Number</param>
        /// <returns>List of all history of builds for specific build</returns>
        public static List<BuildsValue> GetBuildsforDef(string buildID)
        {
            JSONHelper _jsonHelper = new JSONHelper();
            string result = _jsonHelper.GetTFSJsonData(TFSAdminDashboard.Properties.Settings.Default.TFSUrl + "/"
                                                        + TFSAdminDashboard.Properties.Settings.Default.TeamProject
                                                        + "/_apis/build/builds/?api-version=2.0&definitions=" + buildID);

            var resultList = JsonConvert.DeserializeObject<BuildsList>(result);
            return resultList.value.ToList();
        }
        /// <summary>
        /// This method will list all current active releases defintions for team project mentioned in config file
        /// </summary>
        /// <returns> returns a list with all Release Defintions </returns>
        public static List<Value> ListAllReleases()
        {
            JSONHelper _jsonHelper = new JSONHelper();
            string result = _jsonHelper.GetTFSJsonData("http://hqtfsprod:8080/tfs/south%20carolina%20dmv/"
                                                        + TFSAdminDashboard.Properties.Settings.Default.TeamProject
                                                        + "/_apis/release/definitions/?api-version=2.0-preview");

            var resultList = JsonConvert.DeserializeObject<DefinitionsList>(result);
            return resultList.value.ToList();
        }
        /// <summary>
        /// Clone Default Release defintions for new MR/HF/Project release
        /// </summary>
        /// <param name="relDefId">Enter release defintion ID(this is where it clone from)</param>
        /// <param name="buildID">Build Definition ID to attach as artifact for release</param>
        /// <param name="buildDefName">Build Defintion Name to attach as artifact for release</param>
        /// <param name="relNum">Release number</param>
        /// <returns>Returns the object of Release defintion, this will have full details on newly cloned release definitions</returns>
        public static ReleaseDefinition CloneRelease(int relDefId, int buildID, string buildDefName, string relNum)
        {
            try
            {
                var sourceProject = TFSAdminDashboard.Properties.Settings.Default.TeamProject;
                var releaseClient = TFSConnect.GetReleaseClient();
                //Get Release Defintion for given release defintion ID. This is where it clone from
                ReleaseDefinition release = releaseClient.GetReleaseDefinitionAsync(sourceProject, relDefId).Result;

                //Update Release Artifacts for new cloned Release
                release.Artifacts[0].Alias = buildDefName;
                release.Artifacts[0].DefinitionReference["definition"].Id = buildID.ToString();
                release.Artifacts[0].DefinitionReference["definition"].Name = buildDefName;
                for (int i = 0; i < release.Environments.Count; i++)
                {
                    for (int j = 0; j < release.Environments[i].DeployStep.Tasks.Count; j++)
                    {
                        if (release.Environments[i].DeployStep.Tasks[j].Name.Contains("Copy files $(System.DefaultWorkingDirectory)/MAIN_"))
                        {
                            release.Environments[i].DeployStep.Tasks[j].Inputs.Remove("SourcePath");
                            release.Environments[i].DeployStep.Tasks[j].Inputs.Add("SourcePath", string.Format("$(System.DefaultWorkingDirectory)/{0}/Drop", buildDefName));
                        }
                    }
                }
                //A new release def name for new cloned release
                release.Name = buildDefName;
                //clone and return new cloned release object
                return releaseClient.CreateReleaseDefinitionAsync(releaseDefinition: release, project: sourceProject).Result;

            }
            catch (Exception ex)
            {
                return null;
            }

        }
        /// <summary>
        /// Get associated release defintion for given Build ID
        /// </summary>
        /// <param name="buildID">Enter BUild Def ID</param>
        /// <returns>Release defintion</returns>
        public static WebApiRelease GetReleaseforBuildID(string buildID)
        {
            try
            {
                var sourceProject = TFSAdminDashboard.Properties.Settings.Default.TeamProject;
                var releaseClient = TFSConnect.GetReleaseClient();

                List<WebApiRelease> releases = releaseClient.GetReleasesAsync(project: sourceProject, artifactVersionId: buildID, expand: ReleaseExpands.Environments).Result;

                return releases.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        /// <summary>
        /// Gets associated Release ID for given Build def ID
        /// </summary>
        /// <param name="buildID">Enter Build Def ID</param>
        /// <returns>Release Defintion</returns>
        public static string GetReleaseIDforBuildID(string buildID)
        {
            try
            {
                var sourceProject = TFSAdminDashboard.Properties.Settings.Default.TeamProject;
                var releaseClient = TFSConnect.GetReleaseClient();

                List<WebApiRelease> releases = releaseClient.GetReleasesAsync(project: sourceProject, artifactVersionId: buildID, expand: ReleaseExpands.Environments).Result;

                return releases.FirstOrDefault().Id.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        /// <summary>
        /// Clone build for new Release
        /// </summary>
        /// <param name="buildDefId">Build Defintion ID to clone from</param>
        /// <param name="applicationName">Application Name</param>
        /// <param name="relNum">New Release/Project Name (eg: MR-272, HTTPS, HF-32)</param>
        /// <returns> New cloned BuildDefintion</returns>
        public static BuildDefinition CloneBuild(int buildDefId, string applicationName, string relNum)
        {
            try
            {
                var sourceProject = TFSAdminDashboard.Properties.Settings.Default.TeamProject;
                var buildClient = TFSConnect.GetBuildClient();
                //Get the build Def for clone
                BuildDefinition buildDef = buildClient.GetDefinitionAsync(project: sourceProject, definitionId: buildDefId).Result;

                //Update code repository path for new cloned build
                string relbranchPath = buildDef.Repository.DefaultBranch;
                relbranchPath = relbranchPath.Replace("/MAIN", "");
                relbranchPath = relbranchPath.Replace("$/" + sourceProject, "$/" + sourceProject + "/_Releases/" + relNum);
                buildDef.Repository.Properties["tfvcmapping"] = "{\"mappings\":[{\"serverPath\":\"" + relbranchPath + "\",\"mappingType\":\"map\",\"localPath\":\"\\\\\"}]}";
                buildDef.Repository.DefaultBranch = relbranchPath;

                //Remove Current variable & add (Variables can't update)
                buildDef.Variables.Remove("ReleaseNumber");
                BuildDefinitionVariable temp = new BuildDefinitionVariable();
                temp.AllowOverride = true;
                temp.IsSecret = false;
                temp.Value = relNum;
                buildDef.Variables.Add("ReleaseNumber", temp);

                //Change build def name for new clone build
                buildDef.Name = buildDef.Name.Replace("MAIN", relNum);
                //create new cloned build and return 
                return buildClient.CreateDefinitionAsync(buildDef).Result;

            }
            catch (Exception ex)
            {
                return null;
            }

        }
        /// <summary>
        /// Queue new build for given build Def ID (Application)
        /// </summary>
        /// <param name="buildID"></param>
        /// <returns>Build</returns>
        public static Build QueueBuild(int buildID)
        {
            var sourceProject = TFSAdminDashboard.Properties.Settings.Default.TeamProject;
            var buildClient = TFSConnect.GetBuildClient();


            //Get Build Defintion to queue new build
            BuildDefinition builddef = buildClient.GetDefinitionAsync(sourceProject, buildID).Result;
            //Queue new build
            Build newBuild = buildClient.QueueBuildAsync(new Build
            {
                Definition = new DefinitionReference
                {
                    Id = builddef.Id
                },
                Project = builddef.Project
            }).Result;

            //Wait for Build to get complete          
            do
            {
                newBuild = buildClient.GetBuildAsync(sourceProject, newBuild.Id).Result;
                Thread.Sleep(200);

            } while (newBuild.Status != Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed);
            //return completed build to read its status
            return newBuild;

        }
        /// <summary>
        /// Get Build result (Succeeded, failed) for given build ID
        /// </summary>
        /// <param name="buildID">build ID to find its result</param>
        /// <returns>Succeeded, Failed or Partially Succeeded</returns>
        public static string GetBuildResult(int buildID)
        {
            var sourceProject = TFSAdminDashboard.Properties.Settings.Default.TeamProject;
            var buildClient = TFSConnect.GetBuildClient();
            Build builddef = null;
            do
            {
                Thread.Sleep(200);
                //Get Build Def
                builddef = buildClient.GetBuildAsync(sourceProject, buildID).Result;

            } while (builddef.Status != Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed);

            return builddef.Result.ToString();
        }
        /// <summary>
        /// Queue/create/start new release
        /// </summary>
        /// <param name="releaseDefinitionId"> release def id</param>
        /// <param name="newBuild"> New build for artifact</param>
        /// <returns>release</returns>
        public static WebApiRelease CreateRelease(int releaseDefinitionId, Build newBuild)
        {
            var sourceProject = TFSAdminDashboard.Properties.Settings.Default.TeamProject;
            var releaseClient = TFSConnect.GetReleaseClient();

            BuildVersion instanceReference = new BuildVersion { Id = newBuild.Id.ToString(), Name = newBuild.BuildNumber };
            ArtifactMetadata artifact = new ArtifactMetadata { Alias = newBuild.Definition.Name, InstanceReference = instanceReference };
            ReleaseStartMetadata releaseStartMetaData = new ReleaseStartMetadata();
            releaseStartMetaData.DefinitionId = releaseDefinitionId;
            //releaseStartMetaData.Description = "Creating Sample release";
            releaseStartMetaData.Artifacts = new[] { artifact };
            // Create  a release
            WebApiRelease release =
                releaseClient.CreateReleaseAsync(project: sourceProject, releaseStartMetadata: releaseStartMetaData).Result;
            return release;
        }
        /// <summary>
        /// Start deployment for given release and Environment
        /// </summary>
        /// <param name="releaseID"> release id</param>
        /// <param name="releaseEnvironmentId">environment ID</param>
        /// <returns>Deployment status (Success/Failed)</returns>
        public static string StartDeployment(int releaseID, int releaseEnvironmentId)
        {
            string projectName = TFSAdminDashboard.Properties.Settings.Default.TeamProject;
            ReleaseHttpClient releaseClient = TFSConnect.GetReleaseClient();

            ReleaseEnvironmentUpdateMetadata releaseEnvironmentUpdateMetadata = new ReleaseEnvironmentUpdateMetadata()
            {
                Status = EnvironmentStatus.InProgress
            };
            // Start deployment to an environment
            ReleaseEnvironment releaseEnvironment = releaseClient.UpdateReleaseEnvironmentAsync(releaseEnvironmentUpdateMetadata, projectName, releaseID, releaseEnvironmentId).Result;

            while (releaseEnvironment.Status == EnvironmentStatus.InProgress || releaseEnvironment.Status == EnvironmentStatus.Queued)
            {
                Thread.Sleep(200);
                releaseEnvironment = releaseClient.GetReleaseEnvironmentAsync(projectName, releaseID, releaseEnvironmentId).Result;
            }

            return releaseEnvironment.Status.ToString();
        }
        /// <summary>
        /// Delete all given Build and Release defintions. 
        /// </summary>
        /// <param name="releasesCheckedNodes">Tree node collection of ids</param>
        /// <returns>Message</returns>
        public static string DeleteBuildReleases(TreeNodeCollection releasesCheckedNodes)
        {
            try
            {
                var sourceProject = TFSAdminDashboard.Properties.Settings.Default.TeamProject;
                var buildClient = TFSConnect.GetBuildClient();
                var releaseClient = TFSConnect.GetReleaseClient();

                foreach (TreeNode item in releasesCheckedNodes)
                {
                    //Delete Build Definition
                    var re = buildClient.DeleteDefinitionAsync(sourceProject, int.Parse(item.Value));

                    //Get Release ID
                    var obj = releaseClient.GetReleaseDefinitionsAsync(sourceProject, searchText: item.Text).Result.FirstOrDefault();
                    if (obj != null)
                    {
                        int releaseDefID = releaseClient.GetReleaseDefinitionsAsync(sourceProject, searchText: item.Text).Result.FirstOrDefault().Id;
                        //Delete Release Defintiion
                        releaseClient.DeleteReleaseDefinitionAsync(sourceProject, releaseDefID);
                    }

                }
                return "All selectd Build and Release defintions are deleted";
            }
            catch (Exception ex)
            {
                return "Exception Occurred:" + ex.Message.ToString();
            }
        }

        #region Properties
        public class Value
        {
            public int id { get; set; }
            public string name { get; set; }
        }
        public class DefinitionsList
        {
            public int count { get; set; }
            public IList<Value> value { get; set; }
        }

        public class BuildsValue
        {
            public int id { get; set; }
            public string buildNumber { get; set; }
            public string result { get; set; }
        }
        public class BuildsList
        {
            public int count { get; set; }
            public IList<BuildsValue> value { get; set; }
        }
        #endregion
    }
}