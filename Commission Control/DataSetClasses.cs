using System.Data;
using System.ComponentModel;
using System.Collections;

namespace D3.Commission
{
    public class ProjectsSet : DataSet
    {
        private ProjectsDataTable tableProjects;
        public ProjectsDataTable Projects
        {
            get
            {
                return tableProjects;
            }
        }
        public class ProjectsDataTable : DataTable, IEnumerable
        { }
        public class ProjectsRow : DataRow
        { }
    }
    public class ProjectsTableAdapter : Component
    { 
    }
    public class RenewalProducts : DataSet
    {
    }
    public class Tier1Set : DataSet
    {
    }
    public class Tier2Set : DataSet
    {
    }
    public class TrainingSet : DataSet
    {
    }
}
