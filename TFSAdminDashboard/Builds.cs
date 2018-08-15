using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json;
using Artifact = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact;
using WebApiRelease = Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release;
using Microsoft.VisualStudio.Services.Common;
using System.Net;
using System.IO;
using Microsoft.TeamFoundation.Build.WebApi;
using System.Configuration;


namespace TFSAdminDashboard
{
    public static class Builds
    {


        public static List<Value> ListAllBuilds()
        {
            JSONHelper _jsonHelper = new JSONHelper();
            string result = _jsonHelper.GetTFSJsonData(TFSAdminDashboard.Properties.Settings.Default.TFSUrl+"/"
                                                        + TFSAdminDashboard.Properties.Settings.Default.TeamProject
                                                        + "/_apis/build/definitions?api-version=2.0");

            var resultList = JsonConvert.DeserializeObject<DefinitionsList>(result);
            return resultList.value.ToList();
            //var resultList = JsonConvert.DeserializeObject<DefinitionsList>(result).value;
            //return resultList.ToList();
        }

        public static List<Value> ListAllReleases()
        {
            JSONHelper _jsonHelper = new JSONHelper();
            string result = _jsonHelper.GetTFSJsonData("http://hqtfsprod:8080/tfs/south%20carolina%20dmv/"
                                                        + TFSAdminDashboard.Properties.Settings.Default.TeamProject
                                                        + "/_apis/release/definitions/?api-version=2.0-preview");

            var resultList = JsonConvert.DeserializeObject<DefinitionsList>(result);
            return resultList.value.ToList();
        }

        public static ReleaseDefinition CloneRelease(int relDefId, int buildID, string buildDefName, string relNum)
        {
            try
            {
                var sourceProject = TFSAdminDashboard.Properties.Settings.Default.TeamProject;
                var releaseClient = new ReleaseHttpClient(new Uri(TFSAdminDashboard.Properties.Settings.Default.TFSUrl, UriKind.Absolute),
                                                      new VssCredentials(new WindowsCredential(new NetworkCredential(
                                                      TFSAdminDashboard.Properties.Settings.Default.UserName,
                                                      TFSAdminDashboard.Properties.Settings.Default.Password,
                                                      TFSAdminDashboard.Properties.Settings.Default.Domain)))
                                                      );

              //  releaseClient.GetReleaseDefinitionsAsync(sourceProject, releaseDefName);
                ReleaseDefinition release = releaseClient.GetReleaseDefinitionAsync(sourceProject, relDefId).Result;

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
                release.Name = buildDefName;
                //    ReleaseDefinition rel = await client.UpdateReleaseDefinitionAsync(releaseDefinition: release, project: sourceProject);
                return releaseClient.CreateReleaseDefinitionAsync(releaseDefinition: release, project: sourceProject).Result;

                // response = response + "Clone Release Complete - ID- " + rel.Id.ToString() + " Name-" + release.Name + " < br />";
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static string GetReleaseDefID(string relDefName)
        {
            try
            {
                var sourceProject = TFSAdminDashboard.Properties.Settings.Default.TeamProject;
                var releaseClient = new ReleaseHttpClient(new Uri(TFSAdminDashboard.Properties.Settings.Default.TFSUrl, UriKind.Absolute),
                                                      new VssCredentials(new WindowsCredential(new NetworkCredential(
                                                      TFSAdminDashboard.Properties.Settings.Default.UserName,
                                                      TFSAdminDashboard.Properties.Settings.Default.Password,
                                                      TFSAdminDashboard.Properties.Settings.Default.Domain)))
                                                      );

              var _releasesList=  releaseClient.GetReleaseDefinitionsAsync(sourceProject, searchText: relDefName);
                return "";
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static BuildDefinition CloneBuild(int buildDefId, string applicationName, string relNum)
        {
            try
            {
                var sourceProject = TFSAdminDashboard.Properties.Settings.Default.TeamProject;
                var buildClient = new BuildHttpClient(new Uri(TFSAdminDashboard.Properties.Settings.Default.TFSUrl, UriKind.Absolute),
                                                      new VssCredentials(new WindowsCredential(new NetworkCredential(
                                                      TFSAdminDashboard.Properties.Settings.Default.UserName,
                                                      TFSAdminDashboard.Properties.Settings.Default.Password,
                                                      TFSAdminDashboard.Properties.Settings.Default.Domain)))
                                                      );

                BuildDefinition buildDef = buildClient.GetDefinitionAsync(project: sourceProject, definitionId: buildDefId).Result;

                buildDef.Repository.Properties["tfvcmapping"] = "{\"mappings\":[{\"serverPath\":\"$/" + sourceProject + "/_Releases/" + relNum + "/" + applicationName + "\",\"mappingType\":\"map\",\"localPath\":\"\\\\\"}]}";
                buildDef.Repository.DefaultBranch = "$/" + sourceProject + "/_Releases/" + relNum + "/" + applicationName;

                //Remove Current variable & add
                buildDef.Variables.Remove("ReleaseNumber");
                BuildDefinitionVariable temp = new BuildDefinitionVariable();
                temp.AllowOverride = true;
                temp.IsSecret = false;
                temp.Value = relNum;
                buildDef.Variables.Add("ReleaseNumber", temp);

                buildDef.Name = buildDef.Name.Replace("MAIN", relNum);
                return buildClient.CreateDefinitionAsync(buildDef).Result;
                
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static void QueueBuild(int buildID)
        {
            var sourceProject = TFSAdminDashboard.Properties.Settings.Default.TeamProject;
            var buildClient = new BuildHttpClient(new Uri(TFSAdminDashboard.Properties.Settings.Default.TFSUrl, UriKind.Absolute),
                                                  new VssCredentials(new WindowsCredential(new NetworkCredential(
                                                  TFSAdminDashboard.Properties.Settings.Default.UserName,
                                                  TFSAdminDashboard.Properties.Settings.Default.Password,
                                                  TFSAdminDashboard.Properties.Settings.Default.Domain)))
                                                  );


            // First we get project's GUID and buildDefinition's ID.
            // Get the list of build definitions.
            BuildDefinition builddef = buildClient.GetDefinitionAsync(sourceProject,buildID).Result;

            //// Get the specified name of build definition.
            //var target = definitions.First(d => d.Name == buildDefinitionName);

            //// Build class has many properties, hoqever we can set only these properties.
            ////ref: https://www.visualstudio.com/integrate/api/build/builds#queueabuild
            ////In this nuget librari, we should set Project property.
            ////It requires project's GUID, so we're compelled to get GUID by API.
            Build newBuild = buildClient.QueueBuildAsync(new Build
            {
                Definition = new DefinitionReference
                {
                    Id = builddef.Id
                },
                Project = builddef.Project
            }).Result;
            
            
            do
            {
                newBuild = buildClient.GetBuildAsync(sourceProject, newBuild.Id).Result;
            } while (newBuild.Status!= Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Completed);

            //newBuild.Result

            WebApiRelease newRelease = CreateRelease(26, newBuild);
            //int releaseEnvironmentId = release.Environments.FirstOrDefault().Id;
            int releaseEnvironmentId = newRelease.Environments.Where(x => x.Name == "Test2").FirstOrDefault().Id;

            StartDeployment(newRelease, releaseEnvironmentId);
        }

        private static WebApiRelease CreateRelease(int releaseDefinitionId, Build newBuild)
        {
            var sourceProject = TFSAdminDashboard.Properties.Settings.Default.TeamProject;
            var releaseClient = new ReleaseHttpClient(new Uri(TFSAdminDashboard.Properties.Settings.Default.TFSUrl, UriKind.Absolute),
                                                  new VssCredentials(new WindowsCredential(new NetworkCredential(
                                                  TFSAdminDashboard.Properties.Settings.Default.UserName,
                                                  TFSAdminDashboard.Properties.Settings.Default.Password,
                                                  TFSAdminDashboard.Properties.Settings.Default.Domain)))
                                                  );

            // BuildVersion instanceReference = new BuildVersion { Id = "2" };
            BuildVersion instanceReference = new BuildVersion { Id =newBuild.Id.ToString(), Name=newBuild.BuildNumber };
            ArtifactMetadata artifact = new ArtifactMetadata { Alias = newBuild.Definition.Name, InstanceReference = instanceReference };
            ReleaseStartMetadata releaseStartMetaData = new ReleaseStartMetadata();
            releaseStartMetaData.DefinitionId = releaseDefinitionId;
            releaseStartMetaData.Description = "Creating Sample release";
            releaseStartMetaData.Artifacts = new[] { artifact };
            // Create  a release
            WebApiRelease release =
                releaseClient.CreateReleaseAsync(project: sourceProject, releaseStartMetadata: releaseStartMetaData).Result;
            return release;
        }

        public static WebApiRelease StartDeployment(WebApiRelease release, int releaseEnvironmentId)
        {
            string projectName = TFSAdminDashboard.Properties.Settings.Default.TeamProject;
            VssConnection connection = new VssConnection(new Uri(TFSAdminDashboard.Properties.Settings.Default.TFSUrl),
                                       new VssCredentials(new WindowsCredential(new NetworkCredential(
                                           TFSAdminDashboard.Properties.Settings.Default.UserName,
                                           TFSAdminDashboard.Properties.Settings.Default.Password,
                                           TFSAdminDashboard.Properties.Settings.Default.Domain))));


            ReleaseHttpClient releaseClient = connection.GetClient<ReleaseHttpClient>();

           // WebApiRelease release = CreateRelease(releaseClient, newlyCreatedReleaseDefinitionId, projectName);

            ReleaseEnvironmentUpdateMetadata releaseEnvironmentUpdateMetadata = new ReleaseEnvironmentUpdateMetadata()
            {
                Status = EnvironmentStatus.InProgress
            };

            //int releaseEnvironmentId = release.Environments.FirstOrDefault().Id;
           // int releaseEnvironmentId = release.Environments.Where(x => x.Name == "Test2").FirstOrDefault().Id;

            // Start deployment to an environment
            ReleaseEnvironment releaseEnvironment = releaseClient.UpdateReleaseEnvironmentAsync(releaseEnvironmentUpdateMetadata, projectName, release.Id, releaseEnvironmentId).Result;
          //  Console.WriteLine("{0} {1}", releaseEnvironment.Id.ToString().PadLeft(6), releaseEnvironment.Name);

            return release;
        }

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

    }
}