using System;
using System.Windows.Forms;


namespace D3.Commission
{
    public partial class ReportForm : Form
    {
        TrainingSet trainSet;
        ProjectsSet projSet;
        Tier1Set tier1Set;
        Tier2Set tier2Set;
        RenewalProducts renewalSet;
        IncompleteSet incompleteSet;
        PriorSet priorSet;
        CommissionNumbers commissionSet;
        discrepancies discrepSet;

        public ReportForm(TrainingSet tSet, ProjectsSet pjSet, Tier1Set t1Set, Tier2Set t2Set, RenewalProducts rSet, IncompleteSet iSet, PriorSet pSet, CommissionNumbers cSet, discrepancies dSet)
        {
            trainSet = tSet;
            projSet = pjSet;
            tier1Set = t1Set;
            tier2Set = t2Set;
            renewalSet = rSet;
            incompleteSet = iSet;
            priorSet = pSet;
            commissionSet = cSet;
            discrepSet = dSet;            
            InitializeComponent();
        }

        private void ReportForm_Load(object sender, EventArgs e)
        {
            TrainingBindingSource.DataSource = trainSet;
            ProjectsBindingSource.DataSource = projSet;
            Tier1BindingSource.DataSource = tier1Set;
            Tier2BindingSource.DataSource = tier2Set;
            RenewalProductsBindingSource.DataSource = renewalSet;
            IncompleteBindingSource.DataSource = incompleteSet;
            PriorBindingSource.DataSource = priorSet;
            CommissionNumbersBindingSource.DataSource = commissionSet;
            DiscrepanciesBindingSource.DataSource = discrepSet;

            reportViewer1.RefreshReport();
        }
    }
}