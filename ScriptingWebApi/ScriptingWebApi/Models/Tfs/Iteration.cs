
using System;
namespace ScriptingWebApi.Models.Tfs
{
    public class Iteration
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public string Path { get; set; }
    }
}