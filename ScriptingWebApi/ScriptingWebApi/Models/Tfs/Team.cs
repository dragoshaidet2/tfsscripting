using System.Collections.Generic;

namespace ScriptingWebApi.Models.Tfs
{
    public class Team
    {
        public string Name { get; set; }
        public IList<TeamMember> Members { get; set; }
    }
}