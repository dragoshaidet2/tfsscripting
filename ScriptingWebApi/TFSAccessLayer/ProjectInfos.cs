using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml;

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Server;

namespace TFSAccessLayer
{
    public class ProjectInfos : IProjectInfos
    {
        private TfsTeamProjectCollection tfs;

        public IEnumerable<TeamFoundationTeam> GetTeams(TfsProjectDetails projectDetails)
        {
            NetworkCredential networkCredential = new NetworkCredential(projectDetails.Username, projectDetails.Password);
            BasicAuthCredential basicAuthCredential = new BasicAuthCredential(networkCredential);
            TfsClientCredentials tfsClientCredentials = new TfsClientCredentials(basicAuthCredential)
                                                            {
                                                                AllowInteractive
                                                                    = false
                                                            };

            this.tfs =
                new TfsTeamProjectCollection(
                    new Uri(projectDetails.CollectionUri), tfsClientCredentials);

            this.tfs.Authenticate();

            TfsTeamService teamService = this.tfs.GetService<TfsTeamService>();
            ICommonStructureService4 css4 = this.tfs.GetService<ICommonStructureService4>();

            ProjectInfo projectInfo1 = css4.GetProjectFromName(projectDetails.ProjectName);
            var allTeams = teamService.QueryTeams(projectInfo1.Uri);

            return allTeams;
        }

        public TeamFoundationIdentity[] GetTeamMembers(TeamFoundationTeam team)
        {
            return team.GetMembers(this.tfs, MembershipQuery.Direct);
        }

        public IList<ScheduleInfo> GetIterations(string teamProject)
        {
            ICommonStructureService4 css = this.tfs.GetService<ICommonStructureService4>();
            ProjectInfo projectInfo = css.GetProjectFromName(teamProject);

            return (IList<ScheduleInfo>)this.GetIterationsDetails(css, projectInfo.Uri);
        }

        private IEnumerable<ScheduleInfo> GetIterationsDetails(ICommonStructureService4 css, string projectUri)
        {
            NodeInfo[] structures = css.ListStructures(projectUri);
            NodeInfo iterations = structures.FirstOrDefault(n => n.StructureType.Equals("ProjectLifecycle"));
            List<ScheduleInfo> schedule = null;

            if (iterations != null)
            {
                string projectName = css.GetProject(projectUri).Name;

                XmlElement iterationsTree = css.GetNodesXml(new[] { iterations.Uri }, true);
                this.GetIterationDates(iterationsTree.ChildNodes[0], projectName, ref schedule);
            }

            return schedule;
        }

        private void GetIterationDates(XmlNode node, string projectName, ref List<ScheduleInfo> schedule)
        {
            if (schedule == null)
            {
                schedule = new List<ScheduleInfo>();
            }

            if (node == null)
            {
                return;
            }

            if (node.Attributes != null)
            {
                string iterationPath = node.Attributes["Path"].Value;
                string iterationName = node.Attributes["Name"].Value;
                if (!string.IsNullOrEmpty(iterationPath))
                {
                    string strStartDate = (node.Attributes["StartDate"] != null)
                                              ? node.Attributes["StartDate"].Value
                                              : null;
                    string strEndDate = (node.Attributes["FinishDate"] != null)
                                            ? node.Attributes["FinishDate"].Value
                                            : null;

                    DateTime startDate = DateTime.MinValue, endDate = DateTime.MinValue;

                    if (!string.IsNullOrEmpty(strStartDate) && !string.IsNullOrEmpty(strEndDate))
                    {
                        DateTime.TryParse(strStartDate, out startDate);
                        DateTime.TryParse(strEndDate, out endDate);
                    }

                    schedule.Add(
                        new ScheduleInfo
                            {
                                Path =
                                    iterationPath.Replace(
                                        string.Concat("\\", projectName, "\\Iteration"), projectName),
                                StartDate = startDate,
                                EndDate = endDate,
                                Name = iterationName
                            });
                }
            }

            if (node.FirstChild == null)
            {
                return;
            }

            for (int child = 0; child < node.ChildNodes[0].ChildNodes.Count; child++)
            {
                this.GetIterationDates(node.ChildNodes[0].ChildNodes[child], projectName, ref schedule);
            }
        }
    }
}