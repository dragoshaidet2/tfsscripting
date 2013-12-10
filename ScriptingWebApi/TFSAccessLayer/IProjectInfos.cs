using System.Collections.Generic;

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;

namespace TFSAccessLayer
{
    public interface IProjectInfos
    {
        IEnumerable<TeamFoundationTeam> GetTeams(TfsProjectDetails projectDetails);

        TeamFoundationIdentity[] GetTeamMembers(TeamFoundationTeam team);

        IList<ScheduleInfo> GetIterations(string teamProject);
    }
}
