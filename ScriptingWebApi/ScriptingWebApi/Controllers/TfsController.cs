using System.Collections.Generic;
using System.Web.Http;

using Microsoft.TeamFoundation.Framework.Client;

using ScriptingWebApi.Models.Tfs;

using TFSAccessLayer;

namespace ScriptingWebApi.Controllers
{
    public class TfsController : ApiController
    {
        public ProjectInformation Get([FromUri]TfsProjectDetails projectDetails)
        {
            IProjectInfos projectInfos = new ProjectInfos();
            var teams = projectInfos.GetTeams(projectDetails);

            ProjectInformation projectInformation = new ProjectInformation { Teams = new List<Team>() };

            foreach (var teamFoundationTeam in teams)
            {
                Team team = new Team { Name = teamFoundationTeam.Name, Members = new List<TeamMember>() };
                TeamFoundationIdentity[] teamMembers = projectInfos.GetTeamMembers(teamFoundationTeam);
                foreach (var teamFoundationIdentity in teamMembers)
                {
                    team.Members.Add(new TeamMember { Name = teamFoundationIdentity.DisplayName });
                }

                projectInformation.Teams.Add(team);
            }

            projectInformation.Iterations = new List<Iteration>();
            IList<ScheduleInfo> iterations = projectInfos.GetIterations(projectDetails.ProjectName);
            foreach (var scheduleInfo in iterations)
            {
                Iteration iteration = new Iteration
                                          {
                                              Name = scheduleInfo.Name,
                                              Path = scheduleInfo.Path,
                                              StartDate = scheduleInfo.StartDate,
                                              EndDate = scheduleInfo.EndDate
                                          };
                projectInformation.Iterations.Add(iteration);
            }

            return projectInformation;
        }
    }
}