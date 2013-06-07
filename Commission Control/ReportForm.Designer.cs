namespace D3.Commission
{
    partial class ReportForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource2 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource3 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource4 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource5 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource6 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource7 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource8 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource9 = new Microsoft.Reporting.WinForms.ReportDataSource();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportForm));
            this.ProjectsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.TrainingBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.Tier1BindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.Tier2BindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.RenewalProductsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.IncompleteBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.PriorBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.CommissionNumbersBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.DiscrepanciesBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            ((System.ComponentModel.ISupportInitialize)(this.ProjectsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrainingBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Tier1BindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Tier2BindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RenewalProductsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.IncompleteBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PriorBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CommissionNumbersBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiscrepanciesBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // ProjectsBindingSource
            // 
            this.ProjectsBindingSource.DataMember = "Projects";
            this.ProjectsBindingSource.DataSource = typeof(D3.Commission.ProjectsSet);
            // 
            // TrainingBindingSource
            // 
            this.TrainingBindingSource.DataMember = "Training";
            this.TrainingBindingSource.DataSource = typeof(D3.Commission.TrainingSet);
            // 
            // Tier1BindingSource
            // 
            this.Tier1BindingSource.DataMember = "SoldProducts";
            this.Tier1BindingSource.DataSource = typeof(D3.Commission.Tier1Set);
            // 
            // Tier2BindingSource
            // 
            this.Tier2BindingSource.DataMember = "SoldProducts";
            this.Tier2BindingSource.DataSource = typeof(D3.Commission.Tier2Set);
            // 
            // RenewalProductsBindingSource
            // 
            this.RenewalProductsBindingSource.DataMember = "SoldProducts";
            this.RenewalProductsBindingSource.DataSource = typeof(D3.Commission.RenewalProducts);
            // 
            // IncompleteBindingSource
            // 
            this.IncompleteBindingSource.DataMember = "TSIncomplete";
            this.IncompleteBindingSource.DataSource = typeof(D3.Commission.IncompleteSet);
            // 
            // PriorBindingSource
            // 
            this.PriorBindingSource.DataMember = "TSPrior";
            this.PriorBindingSource.DataSource = typeof(D3.Commission.PriorSet);
            // 
            // CommissionNumbersBindingSource
            // 
            this.CommissionNumbersBindingSource.DataMember = "DataTable1";
            this.CommissionNumbersBindingSource.DataSource = typeof(D3.Commission.CommissionNumbers);
            // 
            // DiscrepanciesBindingSource
            // 
            this.DiscrepanciesBindingSource.DataMember = "DataTable1";
            this.DiscrepanciesBindingSource.DataSource = typeof(D3.Commission.discrepancies);
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "ProjectsSet_Projects";
            reportDataSource1.Value = this.ProjectsBindingSource;
            reportDataSource2.Name = "TrainingSet_Training";
            reportDataSource2.Value = this.TrainingBindingSource;
            reportDataSource3.Name = "Tier1Set_SoldProducts";
            reportDataSource3.Value = this.Tier1BindingSource;
            reportDataSource4.Name = "Tier2Set_SoldProducts";
            reportDataSource4.Value = this.Tier2BindingSource;
            reportDataSource5.Name = "RenewalProducts_SoldProducts";
            reportDataSource5.Value = this.RenewalProductsBindingSource;
            reportDataSource6.Name = "Incomplete_TSIncomplete";
            reportDataSource6.Value = this.IncompleteBindingSource;
            reportDataSource7.Name = "Prior_TSPrior";
            reportDataSource7.Value = this.PriorBindingSource;
            reportDataSource8.Name = "CommissionNumbers_DataTable1";
            reportDataSource8.Value = this.CommissionNumbersBindingSource;
            reportDataSource9.Name = "discrepancies_DataTable1";
            reportDataSource9.Value = this.DiscrepanciesBindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource2);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource3);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource4);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource5);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource6);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource7);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource8);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource9);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "Commission_Control.CommissionReport.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(967, 409);
            this.reportViewer1.TabIndex = 0;
            // 
            // ReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(967, 409);
            this.Controls.Add(this.reportViewer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ReportForm";
            this.Text = "ReportForm";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.ReportForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ProjectsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TrainingBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Tier1BindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Tier2BindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RenewalProductsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.IncompleteBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PriorBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CommissionNumbersBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DiscrepanciesBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private System.Windows.Forms.BindingSource TrainingBindingSource;
        private System.Windows.Forms.BindingSource ProjectsBindingSource;
        private System.Windows.Forms.BindingSource Tier1BindingSource;
        private System.Windows.Forms.BindingSource Tier2BindingSource;
        private System.Windows.Forms.BindingSource RenewalProductsBindingSource;
        private System.Windows.Forms.BindingSource IncompleteBindingSource;
        private System.Windows.Forms.BindingSource PriorBindingSource;
        private System.Windows.Forms.BindingSource CommissionNumbersBindingSource;
        private System.Windows.Forms.BindingSource DiscrepanciesBindingSource;

    }
}