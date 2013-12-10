
using System.Collections.Generic;
namespace ScriptingWebApi.Models.Tfs
{
    public class ProjectInformation
    {
        public IList<Iteration> Iterations { get; set; }
        public IList<Team> Teams { get; set; }
    }
}