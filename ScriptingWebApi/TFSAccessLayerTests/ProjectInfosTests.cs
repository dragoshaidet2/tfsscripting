using Microsoft.VisualStudio.TestTools.UnitTesting;

using TFSAccessLayer;

namespace TFSAccessLayerTests
{
    [TestClass]
    public class ProjectInfosTests
    {
        [TestMethod]
        [Ignore]
        public void TestGetAllTeams()
        {
            IProjectInfos projectInfos = new ProjectInfos();
            var teams = projectInfos.GetTeams(new TfsProjectDetails());
            Assert.IsTrue(teams != null);
        }
    }
}
