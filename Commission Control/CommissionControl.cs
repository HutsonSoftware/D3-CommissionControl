using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;


namespace D3.Commission
{
    public partial class CommissionControl : Form
    {
        DataAccess dataAccess;

        public CommissionControl()
        {
            InitializeComponent();
            settings = new Settings();
            dataAccess = new DataAccess(Settings);
        }

        private Settings settings;

        public Settings Settings 
        {   
            get
            {
                return settings;
            } 
            set 
            {
                settings.Save(); 
            } 
        }

        private void CommissionControl_Load(object sender, EventArgs e)
        {
            ViewReport.Enabled = false;
            PayCommission.Enabled = false;
            InitializeTrainingGrid();
            InitializeProjectGrid();
            InitializeTier1Grid();
            InitializeTier2Grid();
            InitializeRenewalGrid();
            InitializeIncompleteGrid();
            InitializePriorGrid();
            
            SalesPerson.Items.AddRange(dataAccess.GetSalesPeople());
            SalesPerson.Text = "Select Sales Person";
            SalesPerson.Focus();
        }

        private void SalesPerson_SelectedIndexChanged(object sender, EventArgs e)
        {
            EndDate.Focus();
        }

        private void UpdatePerson_Click(object sender, EventArgs e)
        {
            if (SalesPerson.Text != "Select Sales Person")
            {
                UpdateGrids();
                ExportReport.Enabled = true;
            }
        }

        private void UpdateGrids()
        {
            ClearProgressBar(7);
            ClearGrids();

            ArrayList rates = GetCommissionRates();
            progressBar1.Value++;

            LoadTrainingGrid(dataAccess.GetTrainingData(SalesPerson.Text, (String)rates[0], EndDate.Value.ToShortDateString()));
            progressBar1.Value++;

            LoadProjectGrid(dataAccess.GetProjectData(SalesPerson.Text, (String)rates[1], EndDate.Value.ToShortDateString()));
            progressBar1.Value++;

            LoadTier1Grid(dataAccess.GetTier1Data(SalesPerson.Text, (String)rates[2], EndDate.Value.ToShortDateString()));
            progressBar1.Value++;

            LoadTier2Grid(dataAccess.GetTier2Data(SalesPerson.Text, (String)rates[3], EndDate.Value.ToShortDateString()));
            progressBar1.Value++;

            LoadRenewalGrid(dataAccess.GetRenewalData(SalesPerson.Text, (String)rates[4], EndDate.Value.ToShortDateString()));
            progressBar1.Value++;

            LoadIncompleteGrid(dataAccess.GetIncompleteData(SalesPerson.Text, (String)rates[5], EndDate.Value.ToShortDateString()));
            progressBar1.Value++;

            LoadPriorGrid(dataAccess.GetPriorData(SalesPerson.Text, (String)rates[6], EndDate.Value.ToShortDateString()));
            //ViewReport.Enabled = true;

            UpdatePayCommissionText();

            CalculateBigTotal();
            
            ClearProgressBar();
        }

        private void UpdatePayCommissionText()
        {
            String firstname = SalesPerson.Text.Split(Convert.ToChar(","))[1].Substring(1);
            String lastname = SalesPerson.Text.Split(Convert.ToChar(","))[0];
            PayCommission.Text = string.Format("Pay {0} {1}'s Incentives", firstname, lastname);
            PayCommission.Enabled = true;
        }

        private ArrayList GetCommissionRates()
        {
            ArrayList rates;
            rates = dataAccess.GetCommissionRates(SalesPerson.Text);
            if (rates != null)
            {
                for (int i = 0; i < rates.Count; i++)
                {
                    if (rates[i] != DBNull.Value)
                        rates[i] = Convert.ToString((rates[i]));
                    else
                        rates[i] = "0";
                }
            }
            else
            {
                rates = new ArrayList();
                for (int i = 0; i < 7; i++)
                {
                    rates.Add("0");
                }
            }
            return rates;
        }

        private void ClearGrids()
        {
            TrainingGrid.Rows.Clear();
            ProjectGrid.Rows.Clear();
            Tier1Grid.Rows.Clear();
            Tier2Grid.Rows.Clear();
            RenewalGrid.Rows.Clear();
            IncompleteGrid.Rows.Clear();
            PriorGrid.Rows.Clear();
        }

        private void ClearProgressBar()
        {
            progressBar1.Value = 0;
        }

        private void ClearProgressBar(int maximumValue)
        {
            progressBar1.Maximum = maximumValue;
            progressBar1.Value = 0;
        }

        private void CalculateBigTotal()
        {
            Decimal profit = 0;
            Decimal commission = 0;

            if (sTrainingProfit.Text != "")
            {
                profit += Convert.ToDecimal(sTrainingProfit.Text.Substring(1));
            }
            if (sTrainingCommission.Text != "")
            {
                commission += Convert.ToDecimal(sTrainingCommission.Text.Substring(1));
            }

            if (sProjectProfit.Text != "")
            {
                profit += Convert.ToDecimal(sProjectProfit.Text.Substring(1));
            }
            if (sProjectCommission.Text != "")
            {
                commission += Convert.ToDecimal(sProjectCommission.Text.Substring(1));
            }

            if (sTier1Profit.Text != "")
            {
                profit += Convert.ToDecimal(sTier1Profit.Text.Substring(1));
            }
            if (sTier1Commission.Text != "")
            {
                commission += Convert.ToDecimal(sTier1Commission.Text.Substring(1));
            }

            if (sTier2Profit.Text != "")
            {
                profit += Convert.ToDecimal(sTier2Profit.Text.Substring(1));
            }
            if (sTier2Commission.Text != "")
            {
                commission += Convert.ToDecimal(sTier2Commission.Text.Substring(1));
            }

            if (sRenewalProfit.Text != "")
            {
                profit += Convert.ToDecimal(sRenewalProfit.Text.Substring(1));
            }
            if (sRenewalCommission.Text != "")
            {
                commission += Convert.ToDecimal(sRenewalCommission.Text.Substring(1));
            }

            if (sIncompleteProfit.Text != "")
            {
                profit += Convert.ToDecimal(sIncompleteProfit.Text.Substring(1));
            }
            if (sIncompleteCommission.Text != "")
            {
                commission += Convert.ToDecimal(sIncompleteCommission.Text.Substring(1));
            }

            if (sPriorCommission.Text != "")
            {
                commission += Convert.ToDecimal(sPriorCommission.Text.Substring(1));
            }

            sTotalProfit.Text = String.Format("${0:f}", profit);
            sTotalCommission.Text = String.Format("${0:f}", commission);
        }

        private void ViewReport_Click(object sender, EventArgs e)
        {
            //ArrayList trainList = RetrieveTrainingIdsFromGrid();
            //ArrayList projList = RetrieveProjectIdsFromGrid();
            //ArrayList tier1List = RetrieveTier1IdsFromGrid();
            //ArrayList tier2List = RetrieveTier2IdsFromGrid();
            //ArrayList renewalList = RetrieveRenewalIdsFromGrid();
            //ArrayList incompleteList = RetrieveIncompleteIdsFromGrid();
            //ArrayList priorList = RetrievePriorIdsFromGrid();
            //CommissionNumbers numbersSet = new CommissionNumbers();
            //numbersSet.DataTable1.Rows.Add(new object[]
            //{
            //    Convert.ToDecimal(sTrainingCommission.Text.Substring(1)),
            //    Convert.ToDecimal(sProjectCommission.Text.Substring(1)), 
            //    Convert.ToDecimal(sTier1Commission.Text.Substring(1)),
            //    Convert.ToDecimal(sTier2Commission.Text.Substring(1)), 
            //    Convert.ToDecimal(sRenewalCommission.Text.Substring(1)),
            //    Convert.ToDecimal(sTotalCommission.Text.Substring(1)),
            //    Convert.ToDecimal(sIncompleteCommission.Text.Substring(1)),
            //    Convert.ToDecimal(sPriorCommission.Text.Substring(1))
                
            //});

            //discrepancies discrepancySet = new discrepancies();
            //ArrayList dLines = RetrieveDirtyTraining();
            //foreach (object[] o in dLines)
            //{
            //    discrepancySet.DataTable1.Rows.Add(o);
            //}
            //dLines = RetrieveDirtyProjects();
            //foreach (object[] p in dLines)
            //{
            //    discrepancySet.DataTable1.Rows.Add(p);
            //}
            //dLines = RetrieveDirtyTier1();
            //foreach (object[] q in dLines)
            //{
            //    discrepancySet.DataTable1.Rows.Add(q);
            //}
            //dLines = RetrieveDirtyTier2();
            //foreach (object[] r in dLines)
            //{
            //    discrepancySet.DataTable1.Rows.Add(r);
            //}
            //dLines = RetrieveDirtyRenewal();
            //foreach (object[] s in dLines)
            //{
            //    discrepancySet.DataTable1.Rows.Add(s);
            //}
            //dLines = RetrieveDirtyIncomplete();
            //foreach (object[] t in dLines)
            //{
            //    discrepancySet.DataTable1.Rows.Add(t);
            //}
            //dLines = RetrieveDirtyPrior();
            //foreach (object[] u in dLines)
            //{
            //    discrepancySet.DataTable1.Rows.Add(u);
            //}

            //ShowPaidCommissionReport(trainList, projList, tier1List, tier2List, renewalList, incompleteList, priorList, numbersSet, discrepancySet);
        }

        private void ShowPaidCommissionReport(ArrayList trainList, ArrayList projList, ArrayList tier1List, ArrayList tier2List, ArrayList renewalList, ArrayList incompleteList, ArrayList priorList, CommissionNumbers nums, discrepancies disc)
        {
            //progressBar1.Value = 0;
            //progressBar1.Maximum = 8;

            //TrainingSet tSet;
            //ProjectsSet projSet;
            //Tier1Set tier1Set;
            //Tier2Set tier2Set;
            //RenewalProducts renewalSet;
            //IncompleteSet incompleteSet;
            //PriorSet priorSet;
            //CommissionNumbers numbersSet = nums;
            //discrepancies discrepancySet = disc;

            //progressBar1.Value++;

            //tSet = sqlControl.BuildTrainingSet(trainList);
            //progressBar1.Value++;
            //projSet = sqlControl.BuildProjectsSet(projList);
            //progressBar1.Value++;
            //tier1Set = sqlControl.BuildTier1Set(tier1List);
            //progressBar1.Value++;
            //tier2Set = sqlControl.BuildTier2Set(tier2List);
            //progressBar1.Value++;
            //renewalSet = sqlControl.BuildRenewalSet(renewalList);
            //progressBar1.Value++;
            //incompleteSet = sqlControl.BuildIncompleteSet(incompleteList);
            //progressBar1.Value++;
            //priorSet = sqlControl.BuildPriorSet(priorList);
            //progressBar1.Value++;

            //ReportForm rptform = new ReportForm(tSet, projSet, tier1Set, tier2Set, renewalSet, incompleteSet, priorSet, numbersSet, discrepancySet);

            //progressBar1.Value = 0;
            //rptform.Show();
        }

        private void ExportReport_Click(object sender, EventArgs e)
        {
            progressBar1.Maximum = 9;
            progressBar1.Value = 1;

            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine(SalesPerson.Text);
            sb.AppendLine();

            sb.AppendLine("Totals");
            sb.AppendLine();
            sb.AppendLine(",Cost,Price,Profit,PBP");
            sb.AppendLine(string.Format("Training,{0},{1},{2},{3}", sTrainingCost.Text.Replace("$",""),sTrainingPrice.Text.Replace("$", ""),sTrainingProfit.Text.Replace("$", ""),sTrainingCommission.Text.Replace("$", "")));
            sb.AppendLine(string.Format("Projects,{0},{1},{2},{3}", sProjectCost.Text.Replace("$",""), sProjectPrice.Text.Replace("$",""), sProjectProfit.Text.Replace("$",""),sProjectCommission.Text.Replace("$", "")));
            sb.AppendLine(string.Format("Tier1 Products,{0},{1},{2},{3}", sTier1Cost.Text.Replace("$",""), sTier1Price.Text.Replace("$",""), sTier1Profit.Text.Replace("$",""), sTier1Commission.Text.Replace("$","")));
            sb.AppendLine(string.Format("Tier2 Products,{0},{1},{2},{3}", sTier2Cost.Text.Replace("$",""), sTier2Price.Text.Replace("$",""), sTier2Profit.Text.Replace("$",""), sTier2Commission.Text.Replace("$","")));
            sb.AppendLine(string.Format("Renewal,{0},{1},{2},{3}", sRenewalCost.Text.Replace("$",""), sRenewalPrice.Text.Replace("$",""), sRenewalProfit.Text.Replace("$",""), sRenewalCommission.Text.Replace("$","")));
            sb.AppendLine(",Estimated Cost,Actual Price,EstimatedProfit,PBP");
            sb.AppendLine(string.Format("Training/Service Incomplete,{0},{1},{2},{3}", sIncompleteCost.Text.Replace("$",""), sIncompletePrice.Text.Replace("$",""), sIncompleteProfit.Text.Replace("$",""), sIncompleteCommission.Text.Replace("$","")));
            sb.AppendLine(",Estimated Cost,Actual Cost,Difference,PBP");
            sb.AppendLine(string.Format("Training/Service Sold Prior,{0},{1},{2},{3}", sPriorEstimatedCost.Text.Replace("$",""), sPriorActualCost.Text.Replace("$",""), sPriorDifference.Text.Replace("$",""), sPriorCommission.Text.Replace("$","")));
            sb.AppendLine();
            sb.AppendLine(string.Format("TOTAL PBP,,,,{0}", sTotalCommission.Text.Replace("$","")));
            sb.AppendLine();
            progressBar1.Value++;

            sb.AppendLine("Training");
            AppendDataSet(sb, BuildTrainingSetFromGrid());
            sb.AppendLine();
            progressBar1.Value++;

            sb.AppendLine("Projects");
            AppendDataSet(sb, BuildProjectSetFromGrid());
            sb.AppendLine();
            progressBar1.Value++;

            sb.AppendLine("Tier1 Products");
            AppendDataSet(sb, BuildTier1SetFromGrid());
            sb.AppendLine();
            progressBar1.Value++;

            sb.AppendLine("Tier2 Products");
            AppendDataSet(sb, BuildTier2SetFromGrid());
            sb.AppendLine();
            progressBar1.Value++;

            sb.AppendLine("Renewal Products");
            AppendDataSet(sb, BuildRenewalSetFromGrid());
            sb.AppendLine();
            progressBar1.Value++;

            sb.AppendLine("Training and Service Incomplete");
            AppendDataSet(sb, BuildIncompleteSetFromGrid());
            sb.AppendLine();
            progressBar1.Value++;

            sb.AppendLine("Training and Service Completed Prior");
            AppendDataSet(sb, BuildPriorSetFromGrid());
            sb.AppendLine();
            progressBar1.Value++;

            SaveSalesPersonFile(sb);

            progressBar1.Value = 0;
        }

        private DataSet BuildPriorSetFromGrid()
        {
            DataSet ds = new DataSet();

            DataTable dt = new DataTable("ExportSet");
            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("Opportunity Name", typeof(string));
            dt.Columns.Add("Account Name", typeof(string));
            dt.Columns.Add("Estimated Cost", typeof(string));
            dt.Columns.Add("Actual Cost", typeof(string));
            dt.Columns.Add("Difference", typeof(string));
            dt.Columns.Add("Pay Rate", typeof(string));
            dt.Columns.Add("PBP", typeof(string));

            foreach (DataGridViewRow row in PriorGrid.Rows)
            {
                if ((Boolean)row.Cells["PayCommission"].Value)
                {
                    double dbl = 0;
                    string difference = row.Cells["Difference"].Value.ToString().Replace("$", "");
                    string percent = row.Cells["CommissionPercent"].Value.ToString();

                    if (!Double.TryParse(difference, out dbl)) difference = "0";
                    if (!Double.TryParse(percent, out dbl)) percent = "0";

                    double pbp = Convert.ToDouble(difference) * (Convert.ToDouble(percent) / 100);

                    dt.Rows.Add(
                        row.Cells["CloseDate"].Value.ToString(),
                        row.Cells["OpportunityName"].Value.ToString().Replace(",", " "),
                        row.Cells["AccountName"].Value.ToString().Replace(",", " "),
                        row.Cells["EstimatedCost"].Value.ToString().Replace("$", ""),
                        row.Cells["ActualCost"].Value.ToString().Replace("$", ""),
                        row.Cells["Difference"].Value.ToString().Replace("$", ""),
                        row.Cells["CommissionPercent"].Value.ToString(),
                        pbp.ToString());
                }
            }

            ds.Tables.Add(dt);
            return ds;            
        }

        private DataSet BuildIncompleteSetFromGrid()
        {
            DataSet ds = new DataSet();

            DataTable dt = new DataTable("ExportSet");
            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("Opportunity Name", typeof(string));
            dt.Columns.Add("Account Name", typeof(string));
            dt.Columns.Add("Estimated Cost", typeof(string));
            dt.Columns.Add("Actual Price", typeof(string));
            dt.Columns.Add("Profit", typeof(string));
            dt.Columns.Add("Pay Rate", typeof(string));
            dt.Columns.Add("PBP", typeof(string));

            foreach (DataGridViewRow row in IncompleteGrid.Rows)
            {
                if ((Boolean)row.Cells["PayCommission"].Value)
                {
                    double dbl = 0;
                    string percent = row.Cells["CommissionPercent"].Value.ToString();
                    string profit = row.Cells["ExtendedProfit"].Value.ToString().Replace("$", "");

                    if (!Double.TryParse(percent, out dbl)) percent = "0";
                    if (!Double.TryParse(profit, out dbl)) profit = "0";

                    double pbp = Convert.ToDouble(profit) * (Convert.ToDouble(percent) / 100);

                    dt.Rows.Add(
                        row.Cells["CloseDate"].Value.ToString(),
                        row.Cells["OpportunityName"].Value.ToString().Replace(",", " "),
                        row.Cells["AccountName"].Value.ToString().Replace(",", " "),
                        row.Cells["EstimatedCost"].Value.ToString().Replace("$", ""),
                        row.Cells["ExtendedPrice"].Value.ToString().Replace("$", ""),
                        row.Cells["ExtendedProfit"].Value.ToString().Replace("$", ""),
                        row.Cells["CommissionPercent"].Value.ToString(),
                        pbp.ToString());
                }
            }

            ds.Tables.Add(dt);
            return ds;
        }

        private DataSet BuildRenewalSetFromGrid()
        {
            DataSet ds = new DataSet();

            DataTable dt = new DataTable("ExportSet");
            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("Opportunity Name", typeof(string));
            dt.Columns.Add("Account Name", typeof(string));
            dt.Columns.Add("Contact Name", typeof(string));
            dt.Columns.Add("Product Name", typeof(string));
            dt.Columns.Add("Quantity", typeof(string));
            dt.Columns.Add("Cost", typeof(string));
            dt.Columns.Add("Price", typeof(string));
            dt.Columns.Add("Pay Rate", typeof(string));
            dt.Columns.Add("PBP", typeof(string));

            foreach (DataGridViewRow row in RenewalGrid.Rows)
            {
                if ((Boolean)row.Cells["PayCommission"].Value)
                {
                    double dbl = 0;
                    string cost = row.Cells["ExtendedCost"].Value.ToString().Replace("$", "");
                    string price = row.Cells["ExtendedPrice"].Value.ToString().Replace("$", "");
                    string percent = row.Cells["CommissionPercent"].Value.ToString();

                    if (!Double.TryParse(cost, out dbl)) cost = "0";
                    if (!Double.TryParse(price, out dbl)) price = "0";
                    if (!Double.TryParse(percent, out dbl)) percent = "0";

                    double profit = Convert.ToDouble(price) - Convert.ToDouble(cost);
                    double pbp = profit * (Convert.ToDouble(percent) / 100);

                    dt.Rows.Add(
                        row.Cells["Date"].Value.ToString(),
                        row.Cells["OpportunityName"].Value.ToString().Replace(",", " "),
                        row.Cells["AccountName"].Value.ToString().Replace(",", " "),
                        row.Cells["ContactName"].Value.ToString().Replace(",", " "),
                        row.Cells["ProductName"].Value.ToString().Replace(",", " "),
                        row.Cells["Quantity"].Value.ToString(),
                        row.Cells["ExtendedCost"].Value.ToString().Replace("$", ""),
                        row.Cells["ExtendedPrice"].Value.ToString().Replace("$", ""),
                        row.Cells["CommissionPercent"].Value.ToString(),
                        pbp.ToString());
                }
            }

            ds.Tables.Add(dt);
            return ds;            
        }

        private DataSet BuildTier2SetFromGrid()
        {
            DataSet ds = new DataSet();

            DataTable dt = new DataTable("ExportSet");
            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("Opportunity Name", typeof(string));
            dt.Columns.Add("Account Name", typeof(string));
            dt.Columns.Add("Contact Name", typeof(string));
            dt.Columns.Add("Product Name", typeof(string));
            dt.Columns.Add("Quantity", typeof(string));
            dt.Columns.Add("Cost", typeof(string));
            dt.Columns.Add("Price", typeof(string));
            dt.Columns.Add("Pay Rate", typeof(string));
            dt.Columns.Add("PBP", typeof(string));
            

            foreach (DataGridViewRow row in Tier2Grid.Rows)
            {
                if ((Boolean)row.Cells["PayCommission"].Value)
                {
                    double dbl = 0;
                    string cost = row.Cells["ExtendedCost"].Value.ToString().Replace("$", "");
                    string price = row.Cells["ExtendedPrice"].Value.ToString().Replace("$", "");
                    string percent = row.Cells["CommissionPercent"].Value.ToString();

                    if (!Double.TryParse(cost, out dbl)) cost = "0";
                    if (!Double.TryParse(price, out dbl)) price = "0";
                    if (!Double.TryParse(percent, out dbl)) percent = "0";

                    double profit = Convert.ToDouble(price) - Convert.ToDouble(cost);
                    double pbp = profit * (Convert.ToDouble(percent) / 100);

                    dt.Rows.Add(
                        row.Cells["Date"].Value.ToString(),
                        row.Cells["OpportunityName"].Value.ToString().Replace(",", " "),
                        row.Cells["AccountName"].Value.ToString().Replace(",", " "),
                        row.Cells["ContactName"].Value.ToString().Replace(",", " "),
                        row.Cells["ProductName"].Value.ToString().Replace(",", " "),
                        row.Cells["Quantity"].Value.ToString(),
                        row.Cells["ExtendedCost"].Value.ToString().Replace("$", ""),
                        row.Cells["ExtendedPrice"].Value.ToString().Replace("$", ""),
                        row.Cells["CommissionPercent"].Value.ToString(),
                        pbp.ToString());
                }
            }

            ds.Tables.Add(dt);
            return ds;
        }

        private DataSet BuildTier1SetFromGrid()
        {
            DataSet ds = new DataSet();

            DataTable dt = new DataTable("ExportSet");
            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("Opportunity Name", typeof(string));
            dt.Columns.Add("Account Name", typeof(string));
            dt.Columns.Add("Contact Name", typeof(string));
            dt.Columns.Add("Product Name", typeof(string));
            dt.Columns.Add("Quantity", typeof(string));
            dt.Columns.Add("Cost", typeof(string));
            dt.Columns.Add("Price", typeof(string));
            dt.Columns.Add("Pay Rate", typeof(string));
            dt.Columns.Add("PBP", typeof(string));

            foreach (DataGridViewRow row in Tier1Grid.Rows)
            {
                if ((Boolean)row.Cells["PayCommission"].Value)
                {
                    double dbl = 0;
                    string cost = row.Cells["ExtendedCost"].Value.ToString().Replace("$", "");
                    string price = row.Cells["ExtendedPrice"].Value.ToString().Replace("$", "");
                    string percent = row.Cells["CommissionPercent"].Value.ToString();

                    if (!Double.TryParse(cost, out dbl)) cost = "0";
                    if (!Double.TryParse(price, out dbl)) price = "0";
                    if (!Double.TryParse(percent, out dbl)) percent = "0";

                    double profit = Convert.ToDouble(price) - Convert.ToDouble(cost);
                    double pbp = profit * (Convert.ToDouble(percent) / 100);

                    dt.Rows.Add(
                        row.Cells["Date"].Value.ToString(),
                        row.Cells["OpportunityName"].Value.ToString().Replace(",", " "),
                        row.Cells["AccountName"].Value.ToString().Replace(",", " "),
                        row.Cells["ContactName"].Value.ToString().Replace(",", " "),
                        row.Cells["ProductName"].Value.ToString().Replace(",", " "),
                        row.Cells["Quantity"].Value.ToString(),
                        row.Cells["ExtendedCost"].Value.ToString().Replace("$", ""),
                        row.Cells["ExtendedPrice"].Value.ToString().Replace("$", ""),
                        row.Cells["CommissionPercent"].Value.ToString(),
                        pbp.ToString());
                }
            }

            ds.Tables.Add(dt);
            return ds;
        }

        private DataSet BuildProjectSetFromGrid()
        {
            DataSet ds = new DataSet();

            DataTable dt = new DataTable("ExportSet");
            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("Project Name", typeof(string));
            dt.Columns.Add("Opportunity Name", typeof(string));
            dt.Columns.Add("Account Name", typeof(string));
            dt.Columns.Add("Contact Name", typeof(string));
            dt.Columns.Add("Product Name", typeof(string));
            dt.Columns.Add("Cost", typeof(string));
            dt.Columns.Add("Price", typeof(string));
            dt.Columns.Add("Pay Rate", typeof(string));
            dt.Columns.Add("PBP", typeof(string));

            foreach (DataGridViewRow row in ProjectGrid.Rows)
            {
                if ((Boolean)row.Cells["PayCommission"].Value)
                {
                    double dbl = 0;
                    string cost = row.Cells["ExtendedCost"].Value.ToString().Replace("$", "");
                    string price = row.Cells["ExtendedPrice"].Value.ToString().Replace("$", "");
                    string percent = row.Cells["CommissionPercent"].Value.ToString();

                    if (!Double.TryParse(cost, out dbl)) cost = "0";
                    if (!Double.TryParse(price, out dbl)) price = "0";
                    if (!Double.TryParse(percent, out dbl)) percent = "0";

                    double profit = Convert.ToDouble(price) - Convert.ToDouble(cost);
                    double pbp = profit * (Convert.ToDouble(percent) / 100);

                    dt.Rows.Add(
                        row.Cells["Date"].Value.ToString(),
                        row.Cells["ProjectName"].Value.ToString().Replace(",", " "),
                        row.Cells["OpportunityName"].Value.ToString().Replace(",", " "),
                        row.Cells["AccountName"].Value.ToString().Replace(",", " "),
                        row.Cells["ContactName"].Value.ToString().Replace(",", " "),
                        row.Cells["ProductName"].Value.ToString().Replace(",", ""),
                        row.Cells["ExtendedCost"].Value.ToString().Replace("$", ""),
                        row.Cells["ExtendedPrice"].Value.ToString().Replace("$", ""),
                        row.Cells["CommissionPercent"].Value.ToString(),
                        pbp.ToString());
                }
            }

            ds.Tables.Add(dt);
            return ds;
        }

        private DataSet BuildTrainingSetFromGrid()
        {
            DataSet ds = new DataSet();
            
            DataTable dt = new DataTable("ExportSet");
            dt.Columns.Add("Date", typeof(string));
            dt.Columns.Add("Event Name", typeof(string));
            dt.Columns.Add("Opportunity Name", typeof(string));
            dt.Columns.Add("Account Name", typeof(string));
            dt.Columns.Add("Contact Name", typeof(string));
            dt.Columns.Add("Total Cost", typeof(string));
            dt.Columns.Add("Total Price", typeof(string));
            dt.Columns.Add("Profit", typeof(string));
            dt.Columns.Add("Pay Rate", typeof(string));
            dt.Columns.Add("PBP", typeof(string));

            foreach (DataGridViewRow row in TrainingGrid.Rows)
            {
                if ((Boolean)row.Cells["PayCommission"].Value)
                {
                    double dbl = 0;
                    string profit = row.Cells["Profit"].Value.ToString().Replace("$", "");
                    string percent = row.Cells["CommissionPercent"].Value.ToString();

                    if (!Double.TryParse(profit, out dbl)) profit = "0";
                    if (!Double.TryParse(percent, out dbl)) percent = "0";

                    double pbp = Convert.ToDouble(profit) * (Convert.ToDouble(percent) / 100);

                    dt.Rows.Add(
                        row.Cells["Date"].Value.ToString(),
                        row.Cells["EventName"].Value.ToString().Replace(",", " "),
                        row.Cells["OpportunityName"].Value.ToString().Replace(",", " "),
                        row.Cells["AccountName"].Value.ToString().Replace(",", " "),
                        row.Cells["AttendeeName"].Value.ToString().Replace(",", " "),
                        row.Cells["Cost"].Value.ToString().Replace("$",""),
                        row.Cells["Price"].Value.ToString().Replace("$", ""),
                        row.Cells["Profit"].Value.ToString().Replace("$", ""),
                        row.Cells["CommissionPercent"].Value.ToString(),
                        pbp.ToString());
                }
            }
            
            ds.Tables.Add(dt);
            return ds;
        }

        private void AppendDataSet(StringBuilder sb, DataSet ds)
        {
            foreach (DataColumn dc in ds.Tables[0].Columns)
            {
                sb.Append(dc.ToString() + ",");
            }
            sb.Replace(",", Environment.NewLine, sb.Length - 1, 1);

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                foreach (object o in dr.ItemArray)
                {
                    sb.Append(o.ToString() + ",");
                }
                sb.Replace(",", Environment.NewLine, sb.Length - 1, 1);
            }
        }

        private void SaveSalesPersonFile(StringBuilder sb)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "SalesPersonFile|*.csv";
            sfd.Title = "Export Current SalesPerson";
            sfd.ShowDialog();
            if (sfd.FileName != "")
            {
                using (StreamWriter sw = new StreamWriter(sfd.FileName))
                {
                    sw.Write(sb.ToString());
                }
            }
        }

        private void PayCommission_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(string.Format("You are about to pay {0} PBP. Are you sure?", SalesPerson.Text), "Are you sure?", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
            {
                return;
            }
            ClearProgressBar(7);

            PayTrainingCommission();
            progressBar1.Value++;
            PayProjectCommission();
            progressBar1.Value++;
            PayTier1Commission();
            progressBar1.Value++;
            PayTier2Commission();
            progressBar1.Value++;
            PayRenewalCommission();
            progressBar1.Value++;
            PayIncompleteCommission();
            progressBar1.Value++;
            PayPriorCommission();
            progressBar1.Value++;

            //ArrayList trainList = RetrieveTrainingIdsFromGrid();
            //ArrayList projList = RetrieveProjectIdsFromGrid();
            //ArrayList tier1List = RetrieveTier1IdsFromGrid();
            //ArrayList tier2List = RetrieveTier2IdsFromGrid();
            //ArrayList renewalList = RetrieveRenewalIdsFromGrid();
            //ArrayList incompleteList = RetrieveIncompleteIdsFromGrid();
            //ArrayList priorList = RetrievePriorIdsFromGrid();

            //ArrayList trainType = new ArrayList();
            ////Add Training Type
            //for (int j = 0; j < trainList.Count; j++)
            //{
            //    trainType.Add("d3_attendeeinvoicedetail");
            //}
            //ArrayList projType = RetrieveProjectTypeFromGrid();
            //ArrayList tier1Type = RetrieveTier1TypeFromGrid();
            //ArrayList tier2Type = RetrieveTier2TypeFromGrid();
            //ArrayList renewalType = RetrieveRenewalTypeFromGrid();
            //ArrayList incompleteType = new ArrayList();
            //for (int j = 0; j < incompleteList.Count; j++)
            //{
            //    trainType.Add("Opportunity");
            //}
            //ArrayList priorType = RetrievePriorTypeFromGrid();

            //ArrayList trainCommissionPercent = RetrieveTrainingCommissionPercentFromGrid();
            //ArrayList projCommissionPercent = RetrieveProjectCommissionPercentFromGrid();
            //ArrayList tier1CommissionPercent = RetrieveTier1CommissionPercentFromGrid();
            //ArrayList tier2CommissionPercent = RetrieveTier2CommissionPercentFromGrid();
            //ArrayList renewalCommissionPercent = RetrieveRenewalCommissionPercentFromGrid();
            //ArrayList incompleteCommissionPercent = RetrieveIncompleteCommissionPercentFromGrid();
            //ArrayList priorCommissionPercent = RetrievePriorCommissionPercentFromGrid();
            
            //progressBar1.Value++;

            //int i;

            //ArrayList errors = new ArrayList();          
           
            //String error = "";
            //ArrayList trainErrors = new ArrayList();
            //ArrayList projErrors = new ArrayList();
            //ArrayList tier1Errors = new ArrayList();
            //ArrayList tier2Errors = new ArrayList();
            //ArrayList renewalErrors = new ArrayList();
            //ArrayList incompleteErrors = new ArrayList();
            //ArrayList priorErrors = new ArrayList();

            ////******************** Pay Commissions ****************************************************

            //for (i = 0; i < trainList.Count; i++)
            //{
            //    error = dataAccess.PayCommissionManual((String)trainList[i], (String)trainType[i], (String)trainCommissionPercent[i], PayDate.Value.ToShortDateString());
            //    if (error != "")
            //    {
            //        trainErrors.Add(trainList[i]);
            //        errors.Add(error);
            //    }
            //}
            //progressBar1.Value++;
            //for (i = 0; i < projList.Count; i++)
            //{
            //    error = dataAccess.PayCommissionManual((String)projList[i], (String)projType[i], (String)projCommissionPercent[i], PayDate.Value.ToShortDateString());
            //    if (error != "")
            //    {
            //        projErrors.Add(projList[i]);
            //        errors.Add(error);
            //    }
            //}
            //progressBar1.Value++;
            //for (i = 0; i < tier1List.Count; i++)
            //{
            //    error = dataAccess.PayCommissionManual((String)tier1List[i], (String)tier1Type[i], (String)tier1CommissionPercent[i], PayDate.Value.ToShortDateString());
            //    if (error != "")
            //    {
            //        tier1Errors.Add(tier1List[i]);
            //        errors.Add(error);
            //    }
            //}
            //progressBar1.Value++;
            //for (i = 0; i < tier2List.Count; i++)
            //{
            //    error = dataAccess.PayCommissionManual((String)tier2List[i], (String)tier2Type[i], (String)tier2CommissionPercent[i], PayDate.Value.ToShortDateString());
            //    if (error != "")
            //    {
            //        tier2Errors.Add(tier2List[i]);
            //        errors.Add(error);
            //    }
            //}
            //progressBar1.Value++;
            //for (i = 0; i < renewalList.Count; i++)
            //{
            //    error = dataAccess.PayCommissionManual((String)renewalList[i], (String)renewalType[i], (String)renewalCommissionPercent[i], PayDate.Value.ToShortDateString());
            //    if (error != "")
            //    {
            //        renewalErrors.Add(renewalList[i]);
            //        errors.Add(error);
            //    }
            //}
            //progressBar1.Value++;
            //for (i = 0; i < incompleteList.Count; i++)
            //{
            //    error = dataAccess.PayCommissionManual((String)incompleteList[i], (String)incompleteType[i], (String)incompleteCommissionPercent[i], PayDate.Value.ToShortDateString());
            //    if (error != "")
            //    {
            //        incompleteErrors.Add(incompleteList[i]);
            //        errors.Add(error);
            //    }
            //}
            //progressBar1.Value++;
            //for (i = 0; i < priorList.Count; i++)
            //{
            //    error = dataAccess.PayCommissionManual((String)priorList[i], (String)priorType[i], (String)priorCommissionPercent[i], PayDate.Value.ToShortDateString());
            //    if (error != "")
            //    {
            //        priorErrors.Add(priorList[i]);
            //        errors.Add(error);
            //    }
            //}
            
            ////Remove Error IDS From arraylists for report
            //if (errors.Count > 0)
            //{
            //    if (trainErrors.Count > 0)
            //    {
            //        foreach (String s in trainErrors)
            //        {
            //            trainList.Remove(s);
            //        }
            //    }
            //    if (projErrors.Count > 0)
            //    {
            //        foreach (String s in projErrors)
            //        {
            //            projList.Remove(s);
            //        }
            //    }
            //    if (tier1Errors.Count > 0)
            //    {
            //        foreach (String s in tier1Errors)
            //        {
            //            tier1List.Remove(s);
            //        }
            //    }
            //    if (tier2Errors.Count > 0)
            //    {
            //        foreach (String s in tier2Errors)
            //        {
            //            tier2List.Remove(s);
            //        }
            //    }
            //    if (renewalErrors.Count > 0)
            //    {
            //        foreach (String s in renewalErrors)
            //        {
            //            renewalList.Remove(s);
            //        }
            //    }
            //    if (incompleteErrors.Count > 0)
            //    {
            //        foreach (String s in incompleteErrors)
            //        {
            //            incompleteList.Remove(s);
            //        }
            //    }
            //    if (priorErrors.Count > 0)
            //    {
            //        foreach (String s in priorErrors)
            //        {
            //            priorList.Remove(s);
            //        }
            //    }
            //}

            //CommissionNumbers numbersSet = new CommissionNumbers();
            //numbersSet.DataTable1.Rows.Add(new object[]
            //{
            //    Convert.ToDecimal(sTrainingCommission.Text.Substring(1)),
            //    Convert.ToDecimal(sProjectCommission.Text.Substring(1)), 
            //    Convert.ToDecimal(sTier1Commission.Text.Substring(1)),
            //    Convert.ToDecimal(sTier2Commission.Text.Substring(1)), 
            //    Convert.ToDecimal(sRenewalCommission.Text.Substring(1)),
            //    Convert.ToDecimal(sIncompleteCommission.Text.Substring(1)),
            //    Convert.ToDecimal(sPriorCommission.Text.Substring(1)),
            //    Convert.ToDecimal(sTotalCommission.Text.Substring(1))
            //});

            //discrepancies discrepancySet = new discrepancies();
            //ArrayList dLines = RetrieveDirtyTraining();
            //foreach (object[] o in dLines)
            //{
            //    discrepancySet.DataTable1.Rows.Add(o);
            //}
            //dLines = RetrieveDirtyProjects();
            //foreach (object[] p in dLines)
            //{
            //    discrepancySet.DataTable1.Rows.Add(p);
            //}
            //dLines = RetrieveDirtyTier1();
            //foreach (object[] q in dLines)
            //{
            //    discrepancySet.DataTable1.Rows.Add(q);
            //}
            //dLines = RetrieveDirtyTier2();
            //foreach (object[] r in dLines)
            //{
            //    discrepancySet.DataTable1.Rows.Add(r);
            //}
            //dLines = RetrieveDirtyRenewal();
            //foreach (object[] s in dLines)
            //{
            //    discrepancySet.DataTable1.Rows.Add(s);
            //}
            //dLines = RetrieveDirtyIncomplete();
            //foreach (object[] t in dLines)
            //{
            //    discrepancySet.DataTable1.Rows.Add(t);
            //}
            //dLines = RetrieveDirtyPrior();
            //foreach (object[] u in dLines)
            //{
            //    discrepancySet.DataTable1.Rows.Add(u);
            //}

            progressBar1.Value = 0;
            //Update Grids with new values of unpaid commission
            UpdateGrids();

            ////if there were errors, save an error log
            //if (errors.Count < 1)
            //{
            //    MessageBox.Show("Commissions have been marked as paid, Click OK to view Report");
            //    ShowPaidCommissionReport(trainList, projList, tier1List, tier2List, renewalList, incompleteList, priorList, numbersSet, discrepancySet);
            //}
            //else
            //{
            //    MessageBox.Show("There were errors, Click OK so save an error report");
            //    SaveFileDialog save = new SaveFileDialog();
            //    save.Filter = "CSV file (*.csv)|*.csv";
            //    save.Title = "Error Report";
            //    if (save.ShowDialog() == DialogResult.OK)
            //    {
            //        FileStream stream = new FileStream(save.FileName, FileMode.Create);
            //        StreamWriter writer = new StreamWriter(stream);
            //        foreach (String s in errors)
            //        {
            //            writer.WriteLine(s);
            //        }
            //        writer.Close();
            //        stream.Close();
            //    }
            //    MessageBox.Show("Even though there were some errors some commission may have been paid");
            //    ShowPaidCommissionReport(trainList, projList, tier1List, tier2List, renewalList, incompleteList, priorList, numbersSet, discrepancySet);
            //}
        }

        private void PayTrainingCommission()
        {
            DataSet ds = new DataSet();

            DataTable dt = new DataTable();
            dt.Columns.Add("d3_attendeeinvoicedetailid", typeof(string));
            dt.Columns.Add("CommissionPercent", typeof(string));
            dt.Columns.Add("CommissionPaid", typeof(string));

            foreach (DataGridViewRow row in TrainingGrid.Rows)
            {
                if ((Boolean)row.Cells["PayCommission"].Value)
                {
                    dt.Rows.Add(
                        row.Cells["AttendeeInvoiceDetailId"].Value.ToString(),
                        row.Cells["CommissionPercent"].Value.ToString(),
                        PayDate.Value.ToShortDateString());
                }
            }

            ds.Tables.Add(dt);

            String error = string.Empty;
            ArrayList errors = new ArrayList();          
            ArrayList trainErrors = new ArrayList();

            foreach (DataRow dr in dt.Rows)
            {
                error = dataAccess.PayTrainingCommission(dr["d3_attendeeinvoicedetailid"].ToString(), dr["CommissionPercent"].ToString(), dr["CommissionPaid"].ToString());
                if (error != string.Empty)
                {
                    trainErrors.Add(dr["d3_attendeeinvoicedetailid"].ToString());
                    errors.Add(error);
                }
            }
        }

        private void PayProjectCommission()
        {
            DataSet ds = new DataSet();

            DataTable dt = new DataTable();
            dt.Columns.Add("EntityId", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("CommissionPercent", typeof(string));
            dt.Columns.Add("CommissionPaid", typeof(string));

            foreach (DataGridViewRow row in ProjectGrid.Rows)
            {
                if ((Boolean)row.Cells["PayCommission"].Value)
                {
                    dt.Rows.Add(
                        row.Cells["EntityID"].Value.ToString(),
                        row.Cells["Type"].Value.ToString(),
                        row.Cells["CommissionPercent"].Value.ToString(),
                        PayDate.Value.ToShortDateString());
                }
            }

            ds.Tables.Add(dt);

            String error = string.Empty;
            ArrayList errors = new ArrayList();          
            ArrayList trainErrors = new ArrayList();

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["Type"].ToString() == "salesorderdetail")
                    error = dataAccess.PaySalesOrderCommission(dr["EntityID"].ToString(), dr["CommissionPercent"].ToString(), dr["CommissionPaid"].ToString());
                else if (dr["Type"].ToString() == "invoicedetail")
                    error = dataAccess.PayInvoiceCommission(dr["EntityID"].ToString(), dr["CommissionPercent"].ToString(), dr["CommissionPaid"].ToString());
                else
                    error = "Unknown Type";

                if (error != string.Empty)
                {
                    trainErrors.Add(dr["EntityID"].ToString());
                    errors.Add(error);
                }
            }
        }

        private void PayTier1Commission()
        {
            DataSet ds = new DataSet();

            DataTable dt = new DataTable();
            dt.Columns.Add("EntityId", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("CommissionPercent", typeof(string));
            dt.Columns.Add("CommissionPaid", typeof(string));

            foreach (DataGridViewRow row in Tier1Grid.Rows)
            {
                if ((Boolean)row.Cells["PayCommission"].Value)
              {
                    dt.Rows.Add(
                        row.Cells["EntityID"].Value.ToString(),
                        row.Cells["Type"].Value.ToString(),
                        row.Cells["CommissionPercent"].Value.ToString(),
                        PayDate.Value.ToShortDateString());
                }
            }

            ds.Tables.Add(dt);

            String error = string.Empty;
            ArrayList errors = new ArrayList();
            ArrayList trainErrors = new ArrayList();

            foreach (DataRow dr in dt.Rows)
            {
                DateTime date = Convert.ToDateTime(dr["CommissionPaid"].ToString());
                
                if (dr["Type"].ToString() == "salesorderdetail")
                    error = dataAccess.PaySalesOrderCommission(dr["EntityID"].ToString(), dr["CommissionPercent"].ToString(), date.ToShortDateString());
                else if (dr["Type"].ToString() == "invoicedetail")
                    error = dataAccess.PayInvoiceCommission(dr["EntityID"].ToString(), dr["CommissionPercent"].ToString(), date.ToShortDateString());
                else
                    error = "Unknown Type";

                if (error != string.Empty)
                {
                    trainErrors.Add(dr["EntityID"].ToString());
                    errors.Add(error);
                }
            }
        }

        private void PayTier2Commission()
        {
            DataSet ds = new DataSet();

            DataTable dt = new DataTable();
            dt.Columns.Add("EntityId", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("CommissionPercent", typeof(string));
            dt.Columns.Add("CommissionPaid", typeof(string));

            foreach (DataGridViewRow row in Tier2Grid.Rows)
            {
                if ((Boolean)row.Cells["PayCommission"].Value)
                {
                    dt.Rows.Add(
                        row.Cells["EntityID"].Value.ToString(),
                        row.Cells["Type"].Value.ToString(),
                        row.Cells["CommissionPercent"].Value.ToString(),
                        PayDate.Value.ToShortDateString());
                }
            }

            ds.Tables.Add(dt);

            String error = string.Empty;
            ArrayList errors = new ArrayList();
            ArrayList trainErrors = new ArrayList();

            foreach (DataRow dr in dt.Rows)
            {
                DateTime date = Convert.ToDateTime(dr["CommissionPaid"].ToString());

                if (dr["Type"].ToString() == "salesorderdetail")
                    error = dataAccess.PaySalesOrderCommission(dr["EntityID"].ToString(), dr["CommissionPercent"].ToString(), date.ToShortDateString());
                else if (dr["Type"].ToString() == "invoicedetail")
                    error = dataAccess.PayInvoiceCommission(dr["EntityID"].ToString(), dr["CommissionPercent"].ToString(), date.ToShortDateString());
                else
                    error = "Unknown Type";

                if (error != string.Empty)
                {
                    trainErrors.Add(dr["EntityID"].ToString());
                    errors.Add(error);
                }
            }
        }

        private void PayRenewalCommission()
        {
            DataSet ds = new DataSet();

            DataTable dt = new DataTable();
            dt.Columns.Add("EntityId", typeof(string));
            dt.Columns.Add("Type", typeof(string));
            dt.Columns.Add("CommissionPercent", typeof(string));
            dt.Columns.Add("CommissionPaid", typeof(string));

            foreach (DataGridViewRow row in RenewalGrid.Rows)
            {
                if ((Boolean)row.Cells["PayCommission"].Value)
                {
                    dt.Rows.Add(
                        row.Cells["EntityID"].Value.ToString(),
                        row.Cells["Type"].Value.ToString(),
                        row.Cells["CommissionPercent"].Value.ToString(),
                        PayDate.Value.ToShortDateString());
                }
            }

            ds.Tables.Add(dt);

            String error = string.Empty;
            ArrayList errors = new ArrayList();
            ArrayList trainErrors = new ArrayList();

            foreach (DataRow dr in dt.Rows)
            {
                if (dr["Type"].ToString() == "salesorderdetail")
                    error = dataAccess.PaySalesOrderCommission(dr["EntityID"].ToString(), dr["CommissionPercent"].ToString(), dr["CommissionPaid"].ToString());
                else if (dr["Type"].ToString() == "invoicedetail")
                    error = dataAccess.PayInvoiceCommission(dr["EntityID"].ToString(), dr["CommissionPercent"].ToString(), dr["CommissionPaid"].ToString());
                else
                    error = "Unknown Type";

                if (error != string.Empty)
                {
                    trainErrors.Add(dr["EntityID"].ToString());
                    errors.Add(error);
                }
            }
        }

        private void PayIncompleteCommission()
        {
            DataSet ds = new DataSet();

            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(string));
            dt.Columns.Add("EstimatedCost", typeof(string));
            dt.Columns.Add("CommissionPercent", typeof(string));
            dt.Columns.Add("CommissionPaid", typeof(string));

            foreach (DataGridViewRow row in IncompleteGrid.Rows)
            {
                if ((Boolean)row.Cells["PayCommission"].Value)
                {
                    dt.Rows.Add(
                        row.Cells["ID"].Value.ToString(),
                        row.Cells["EstimatedCost"].Value.ToString(),
                        row.Cells["CommissionPercent"].Value.ToString(),
                        PayDate.Value.ToShortDateString());
                }
            }

            ds.Tables.Add(dt);

            String error = string.Empty;
            ArrayList errors = new ArrayList();
            ArrayList trainErrors = new ArrayList();

            foreach (DataRow dr in dt.Rows)
            {
                error = dataAccess.PayOpportunityCommission(dr["ID"].ToString(), dr["EstimatedCost"].ToString(), dr["CommissionPercent"].ToString(), dr["CommissionPaid"].ToString());
                
                if (error != string.Empty)
                {
                    trainErrors.Add(dr["ID"].ToString());
                    errors.Add(error);
                }
            }
        }

        private void PayPriorCommission()
        {
            DataSet ds = new DataSet();

            DataTable dt = new DataTable();
            dt.Columns.Add("OpportunityId", typeof(string));
            dt.Columns.Add("CommissionPercent", typeof(string));
            dt.Columns.Add("CommissionPaid", typeof(string));

            foreach (DataGridViewRow row in PriorGrid.Rows)
            {
                if ((Boolean)row.Cells["PayCommission"].Value)
                {
                    dt.Rows.Add(
                        row.Cells["ID"].Value.ToString(),
                        row.Cells["CommissionPercent"].Value.ToString(),
                        PayDate.Value.ToShortDateString());
                }
            }

            ds.Tables.Add(dt);

            String error = string.Empty;
            ArrayList errors = new ArrayList();
            ArrayList trainErrors = new ArrayList();

            foreach (DataRow dr in dt.Rows)
            {
                error = dataAccess.PayPriorCommission(dr["OpportunityId"].ToString(), dr["CommissionPercent"].ToString(), dr["CommissionPaid"].ToString());
                if (error != string.Empty)
                {
                    trainErrors.Add(dr["OpportunityId"].ToString());
                    errors.Add(error);
                }
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Title = "Save Current Salesperson";
            save.Filter = "SalesPersonFile (*.spf) | *.spf";
            ArrayList trainingRows = new ArrayList();
            ArrayList trainingRow = new ArrayList();
            ArrayList projectRows = new ArrayList();
            ArrayList projectRow = new ArrayList();
            ArrayList tier1Rows = new ArrayList();
            ArrayList tier1Row = new ArrayList();
            ArrayList tier2Rows = new ArrayList();
            ArrayList tier2Row = new ArrayList();
            ArrayList renewalRows = new ArrayList();
            ArrayList renewalRow = new ArrayList();
            ArrayList incompleteRows = new ArrayList();
            ArrayList incompleteRow = new ArrayList();
            ArrayList priorRows = new ArrayList();
            ArrayList priorRow = new ArrayList();
            if (save.ShowDialog() == DialogResult.OK)
            {
                FileStream stream = new FileStream(save.FileName, FileMode.Create);
                StreamWriter writer = new StreamWriter(stream);
                writer.WriteLine(SalesPerson.Text);

                foreach (DataGridViewRow row in TrainingGrid.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.ColumnIndex == 0)
                        {
                            if ((Boolean)cell.Value)
                            {
                                trainingRow.Add("true");
                            }
                            else
                            {
                                trainingRow.Add("false");
                            }
                        }
                        else
                        {
                            trainingRow.Add(cell.Value.ToString());
                        }
                    }
                    trainingRows.Add(trainingRow);
                    trainingRow = new ArrayList();
                }

                SaveToFile(trainingRows, "TrainingGrid", writer);

                foreach (DataGridViewRow row in ProjectGrid.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.ColumnIndex == 0)
                        {
                            if ((Boolean)cell.Value)
                            {
                                projectRow.Add("true");
                            }
                            else
                            {
                                projectRow.Add("false");
                            }
                        }
                        else
                        {
                            projectRow.Add(cell.Value.ToString());
                        }
                    }
                    projectRows.Add(projectRow);
                    projectRow = new ArrayList();
                }

                SaveToFile(projectRows, "ProjectGrid", writer);

                foreach (DataGridViewRow row in Tier1Grid.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.ColumnIndex == 0)
                        {
                            if ((Boolean)cell.Value)
                            {
                                tier1Row.Add("true");
                            }
                            else
                            {
                                tier1Row.Add("false");
                            }
                        }
                        else
                        {
                            tier1Row.Add(cell.Value.ToString());
                        }
                    }
                    tier1Rows.Add(tier1Row);
                    tier1Row = new ArrayList();
                }

                SaveToFile(tier1Rows, "Tier1Grid", writer);

                foreach (DataGridViewRow row in Tier2Grid.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.ColumnIndex == 0)
                        {
                            if ((Boolean)cell.Value)
                            {
                                tier2Row.Add("true");
                            }
                            else
                            {
                                tier2Row.Add("false");
                            }
                        }
                        else
                        {
                            tier2Row.Add(cell.Value.ToString());
                        }
                    }
                    tier2Rows.Add(tier2Row);
                    tier2Row = new ArrayList();
                }

                SaveToFile(tier2Rows, "Tier2Grid", writer);

                foreach (DataGridViewRow row in RenewalGrid.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.ColumnIndex == 0)
                        {
                            if ((Boolean)cell.Value)
                            {
                                renewalRow.Add("true");
                            }
                            else
                            {
                                renewalRow.Add("false");
                            }
                        }
                        else
                        {
                            renewalRow.Add(cell.Value.ToString());
                        }
                    }
                    renewalRows.Add(renewalRow);
                    renewalRow = new ArrayList();
                }

                SaveToFile(renewalRows, "RenewalGrid", writer);

                foreach (DataGridViewRow row in IncompleteGrid.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.ColumnIndex == 0)
                        {
                            if ((Boolean)cell.Value)
                            {
                                incompleteRow.Add("true");
                            }
                            else
                            {
                                incompleteRow.Add("false");
                            }
                        }
                        else
                        {
                            incompleteRow.Add(cell.Value.ToString());
                        }
                    }
                    incompleteRows.Add(incompleteRow);
                    incompleteRow = new ArrayList();
                }

                SaveToFile(incompleteRows, "IncompleteGrid", writer);

                foreach (DataGridViewRow row in PriorGrid.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell.ColumnIndex == 0)
                        {
                            if ((Boolean)cell.Value)
                            {
                                priorRow.Add("true");
                            }
                            else
                            {
                                priorRow.Add("false");
                            }
                        }
                        else
                        {
                            priorRow.Add(cell.Value.ToString());
                        }
                    }
                    priorRows.Add(priorRow);
                    priorRow = new ArrayList();
                }

                SaveToFile(priorRows, "PriorGrid", writer);

                writer.WriteLine("#");
                writer.Close();
                stream.Close();
            }
        }

        private void SaveToFile(ArrayList rows, String gridName, StreamWriter writer)
        {
            String lineToWrite = "";
            writer.WriteLine("#");
            writer.WriteLine(gridName);
            foreach (ArrayList row in rows)
            {
                foreach (String s in row)
                {
                    lineToWrite += string.Format("{0}\t", s);
                }
                lineToWrite = lineToWrite.TrimEnd(Convert.ToChar("\t"));
                writer.WriteLine(lineToWrite);
                lineToWrite = "";
            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Title = "Open Saved Salesperson";
            open.Filter = "SalesPersonFile (*.spf)|*.spf";
            if (open.ShowDialog() == DialogResult.OK)
            {
                TrainingGrid.Rows.Clear();
                ProjectGrid.Rows.Clear();
                Tier1Grid.Rows.Clear();
                Tier2Grid.Rows.Clear();
                RenewalGrid.Rows.Clear();
                IncompleteGrid.Rows.Clear();
                PriorGrid.Rows.Clear();
                FileStream stream = new FileStream(open.FileName, FileMode.Open);
                StreamReader reader = new StreamReader(stream);
                OpenSavedFile(reader);
                String firstname = SalesPerson.Text.Split(Convert.ToChar(","))[1].Substring(1);
                String lastname = SalesPerson.Text.Split(Convert.ToChar(","))[0];
                PayCommission.Text = string.Format("Pay {0} {1}'s Incentives", firstname, lastname);
                PayCommission.Enabled = true;
                ExportReport.Enabled = true;
                //ViewReport.Enabled = true;
                CalculateBigTotal();
                reader.Close();
                stream.Close();
            }
        }

        private void OpenSavedFile(StreamReader reader)
        {
            String gridName;
            String line;
            String[] lineArray;

            ArrayList rowArray = new ArrayList();
            ArrayList rowsArray = new ArrayList();

            Object[] gridViewRow;
            Object[] gridViewRows;

            int i = 0;
            int j = 0;
            int k;

            SalesPerson.Text = reader.ReadLine();
            reader.ReadLine();
            gridName = reader.ReadLine();
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                if (line == "#")
                {
                    gridViewRows = new Object[rowsArray.Count];
                    foreach (ArrayList row in rowsArray)
                    {
                        gridViewRow = new Object[row.Count];
                        foreach (String s in row)
                        {
                            if (i == 0)
                            {
                                gridViewRow[i] = Convert.ToBoolean(s);
                            }
                            else
                            {
                                gridViewRow[i] = s;
                            }
                            i++;
                        }
                        gridViewRows[j] = gridViewRow;
                        i = 0;
                        j++;
                    }
                    j = 0;
                    switch (gridName)
                    {
                        case "TrainingGrid":
                            LoadTrainingGrid(gridViewRows);
                            break;
                        case "ProjectGrid":
                            LoadProjectGrid(gridViewRows);
                            break;
                        case "Tier1Grid":
                            LoadTier1Grid(gridViewRows);
                            break;
                        case "Tier2Grid":
                            LoadTier2Grid(gridViewRows);
                            break;
                        case "RenewalGrid":
                            LoadRenewalGrid(gridViewRows);
                            break;
                        case "IncompleteGrid":
                            LoadIncompleteGrid(gridViewRows);
                            break;
                        case "PriorGrid":
                            LoadPriorGrid(gridViewRows);
                            break;
                    }
                    gridName = reader.ReadLine();
                    rowArray = new ArrayList();
                    rowsArray = new ArrayList();
                    continue;
                }
                lineArray = line.Split(Convert.ToChar("\t"));
                for (k = 0; k < lineArray.Length; k++)
                {
                    rowArray.Add(lineArray[k]);
                }
                rowsArray.Add(rowArray);
                rowArray = new ArrayList();
            }
        }

        private void ConfigToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewConfig();
        }

        private void ViewConfig()
        {
            SettingsEditor editor = new SettingsEditor(settings);
            editor.ShowDialog();

            if (editor.IsDirty)
                settings = editor.Settings;

            editor.Dispose();
        }
        
        #region Training
        private void InitializeTrainingGrid()
        {
            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn(false);
            checkColumn.Name = "PayCommission";
            checkColumn.HeaderText = "";
            checkColumn.ReadOnly = false;
            TrainingGrid.Columns.Add(checkColumn);
            TrainingGrid.Columns["PayCommission"].Width = 25;
            
            TrainingGrid.Columns.Add("AttendeeInvoiceDetailId", "AttendeeInvoiceDetailId");
            TrainingGrid.Columns["AttendeeInvoiceDetailId"].Visible = false;
            
            TrainingGrid.Columns.Add("SalesPerson", "Sales Person");
            TrainingGrid.Columns["SalesPerson"].Visible = false;
            
            TrainingGrid.Columns.Add("Date", "Date");
            TrainingGrid.Columns["Date"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            
            TrainingGrid.Columns.Add("EventName", "Event Name");
            TrainingGrid.Columns["EventName"].Width = 140;

            DataGridViewLinkColumn linkColumn = new DataGridViewLinkColumn();
            linkColumn.Name = "OpportunityName";
            linkColumn.HeaderText = "Opportunity Name";
            linkColumn.UseColumnTextForLinkValue = false;
            linkColumn.LinkBehavior = LinkBehavior.HoverUnderline;
            linkColumn.TrackVisitedState = false;
            TrainingGrid.Columns.Add(linkColumn);
            TrainingGrid.Columns["OpportunityName"].Width = 140;
            
            TrainingGrid.Columns.Add("AccountName", "Account Name");
            TrainingGrid.Columns["AccountName"].Width = 125;
            
            TrainingGrid.Columns.Add("AttendeeName", "Attendee Name");
            TrainingGrid.Columns["AttendeeName"].Width = 100;
            
            TrainingGrid.Columns.Add("Cost", "Cost");
            TrainingGrid.Columns["Cost"].Width = 65;
            
            TrainingGrid.Columns.Add("Price", "Price");
            TrainingGrid.Columns["Price"].Width = 65;
            
            TrainingGrid.Columns.Add("Profit", "Profit");
            TrainingGrid.Columns["Profit"].Width = 65;

            TrainingGrid.Columns.Add("OpportunityID", "OpportunityID");
            TrainingGrid.Columns["OpportunityID"].ReadOnly = true;
            TrainingGrid.Columns["OpportunityID"].Visible = false;

            DataGridViewComboBoxColumn percentColumn = new DataGridViewComboBoxColumn();
            percentColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            percentColumn.Name = "CommissionPercent";
            percentColumn.HeaderText = "";
            percentColumn.ReadOnly = false;
            List<String> items = new List<string>(21);
            items.AddRange(new String[] { "0", "5", "10", "15", "20", "25", "30", "35", "40", "45", "50", "55", "60", "65", "70", "75", "80", "85", "90", "95", "100" });
            percentColumn.DataSource = items;
            TrainingGrid.Columns.Add(percentColumn);
            TrainingGrid.Columns["CommissionPercent"].Width = 40;
            
            TrainingGrid.Columns.Add("Margin", "Margin");
            TrainingGrid.Columns["Margin"].Width = 65;
            
            TrainingGrid.Columns.Add("isDirty", "isDirty");
            TrainingGrid.Columns["isDirty"].Visible = false;

            TrainingGrid.CellContentClick += new DataGridViewCellEventHandler(TrainingGrid_CellContentClick);
            TrainingGrid.CellValueChanged += new DataGridViewCellEventHandler(TrainingGrid_CellValueChanged);
        }

        private void TrainingAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dr in TrainingGrid.Rows)
            {
                dr.Cells[0].Value = TrainingAll.Checked;
            }
        }

        private void TrainingGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex != -1)
            {
                if ((Boolean)TrainingGrid.CurrentCell.Value)
                {
                    TrainingGrid.CurrentCell.Value = false;
                }
                else
                {
                    TrainingGrid.CurrentCell.Value = true;
                }
            }
            else if (e.ColumnIndex == 5 && e.RowIndex != -1)
            {
                string url = "http://d3crm:5555/D3TECHNOLOGIES/sfa/opps/edit.aspx?id={" + TrainingGrid.CurrentRow.Cells["OpportunityID"].Value + "}#";
                Process.Start(url);
            }
            else if (e.ColumnIndex != 11 && e.RowIndex != -1)
            {
                TrainingGrid.CurrentCell.ReadOnly = true;
            }
        }

        private void TrainingGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 || e.ColumnIndex == 13)
            {
                CalculateTrainingTotals();
                CalculateBigTotal();
                TrainingGrid["isDirty", e.RowIndex].Value = "true";
            }
            else if (e.ColumnIndex != 12 && e.ColumnIndex != 14)
            {
                MessageBox.Show("DO NOT CHANGE VALUES WITHIN THE GRID");
            }
        }

        private void LoadTrainingGrid(Object[] trainingData)
        {
            if (trainingData != null)
            {
                foreach (Object[] o in trainingData)
                {
                    TrainingGrid.Rows.Add(o);
                }
            }
            TrainingAll.Checked = true;
            CalculateTrainingTotals();
        }

        private void CalculateTrainingTotals()
        {
            Decimal price = 0;
            Decimal cost = 0;
            Decimal profit = 0;
            Decimal commission = 0;

            foreach (DataGridViewRow row in TrainingGrid.Rows)
            {
                if ((Boolean)row.Cells[0].Value)
                {
                    if ((String)row.Cells["Price"].Value != "")
                    {
                        price += Convert.ToDecimal(row.Cells["Price"].Value.ToString().Substring(1));
                    }
                    if ((String)row.Cells["Cost"].Value != "")
                    {
                        cost += Convert.ToDecimal(row.Cells["Cost"].Value.ToString().Substring(1));
                    }
                    if ((String)row.Cells["Profit"].Value != "")
                    {
                        profit += Convert.ToDecimal(row.Cells["Profit"].Value.ToString().Substring(1));
                        commission += Convert.ToDecimal(row.Cells["Profit"].Value.ToString().Substring(1)) * Convert.ToDecimal(row.Cells["CommissionPercent"].Value.ToString()) / 100;
                    }
                }
            }
            TrainingPrice.Text = String.Format("${0:f}", price);
            TrainingCost.Text = String.Format("${0:f}", cost);
            TrainingProfit.Text = String.Format("${0:f}", profit);
            TrainingCommission.Text = String.Format("${0:f}", commission);

            sTrainingPrice.Text = String.Format("${0:f}", price);
            sTrainingCost.Text = String.Format("${0:f}", cost);
            sTrainingProfit.Text = String.Format("${0:f}", profit);
            sTrainingCommission.Text = String.Format("${0:f}", commission);
        }

        //private ArrayList RetrieveTrainingIdsFromGrid()
        //{
        //    ArrayList trainList = new ArrayList();
        //    foreach (DataGridViewRow row in TrainingGrid.Rows)
        //    {
        //        if ((Boolean)row.Cells[0].Value)
        //        {
        //            trainList.Add(row.Cells["AttendeeInvoiceDetailId"].Value.ToString());
        //        }
        //    }
        //    return trainList;

        //}

        //private ArrayList RetrieveTrainingCommissionPercentFromGrid()
        //{
        //    ArrayList trainCommissionPercent = new ArrayList();
        //    foreach (DataGridViewRow row in TrainingGrid.Rows)
        //    {
        //        if ((Boolean)row.Cells[0].Value)
        //        {
        //            trainCommissionPercent.Add(row.Cells["CommissionPercent"].Value.ToString());
        //        }
        //    }
        //    return trainCommissionPercent;
        //}

        //private ArrayList RetrieveDirtyTraining()
        //{
        //    ArrayList dirtyLines = new ArrayList();
        //    Object[] dirtyLine;
        //    foreach (DataGridViewRow row in TrainingGrid.Rows)
        //    {
        //        if (!(Boolean)row.Cells[0].Value || Convert.ToBoolean(row.Cells["isDirty"].Value))
        //        {
        //            dirtyLine = new Object[10];
        //            dirtyLine[0] = "Training";
        //            dirtyLine[1] = row.Cells["AttendeeInvoiceDetailId"].Value.ToString();
        //            dirtyLine[2] = row.Cells["OpportunityName"].Value.ToString();
        //            dirtyLine[3] = row.Cells["EventName"].Value.ToString();
        //            dirtyLine[4] = row.Cells["Cost"].Value.ToString();
        //            dirtyLine[5] = row.Cells["Price"].Value.ToString();
        //            dirtyLine[6] = row.Cells["CommissionPercent"].Value.ToString();
        //            dirtyLine[7] = row.Cells["AttendeeName"].Value.ToString();
        //            dirtyLine[8] = row.Cells["AccountName"].Value.ToString();
        //            dirtyLine[9] = row.Cells["Date"].Value.ToString();
        //            dirtyLines.Add(dirtyLine);
        //        }
        //    }
        //    return dirtyLines;
        //}
        #endregion

        #region Projects
        private void InitializeProjectGrid()
        {
            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn(false);
            checkColumn.Name = "PayCommission";
            checkColumn.HeaderText = "";
            checkColumn.ReadOnly = false;
            ProjectGrid.Columns.Add(checkColumn);
            ProjectGrid.Columns["PayCommission"].Width = 25;

            ProjectGrid.Columns.Add("Type", "Type");
            ProjectGrid.Columns["Type"].Visible = false;
            
            ProjectGrid.Columns.Add("EntityID", "EntityID");
            ProjectGrid.Columns["EntityID"].Visible = false;
            
            ProjectGrid.Columns.Add("SalesPerson", "Sales Person");
            ProjectGrid.Columns["SalesPerson"].Visible = false;
            
            ProjectGrid.Columns.Add("Date", "Date");
            ProjectGrid.Columns["Date"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            
            ProjectGrid.Columns.Add("ProjectName", "Project Name");
            ProjectGrid.Columns["ProjectName"].Width = 140;
            
            DataGridViewLinkColumn linkColumn = new DataGridViewLinkColumn();
            linkColumn.Name = "OpportunityName";
            linkColumn.HeaderText = "Opportunity Name";
            linkColumn.UseColumnTextForLinkValue = false;
            linkColumn.LinkBehavior = LinkBehavior.HoverUnderline;
            linkColumn.TrackVisitedState = false;
            ProjectGrid.Columns.Add(linkColumn);
            ProjectGrid.Columns["OpportunityName"].Width = 145;
            
            ProjectGrid.Columns.Add("AccountName", "Account Name");
            ProjectGrid.Columns["AccountName"].Width = 125;
            
            ProjectGrid.Columns.Add("ContactName", "Contact Name");
            ProjectGrid.Columns["ContactName"].Width = 100;
            
            ProjectGrid.Columns.Add("ProductName", "Product Name");
            ProjectGrid.Columns["ProductName"].Width = 145;
            
            ProjectGrid.Columns.Add("ExtendedCost", "Extended Cost");
            ProjectGrid.Columns["ExtendedCost"].Width = 65;
            
            ProjectGrid.Columns.Add("ExtendedPrice", "Extended Price");
            ProjectGrid.Columns["ExtendedPrice"].Width = 65;

            ProjectGrid.Columns.Add("OpportunityID", "OpportunityID");
            ProjectGrid.Columns["OpportunityID"].ReadOnly = true;
            ProjectGrid.Columns["OpportunityID"].Visible = false;

            DataGridViewComboBoxColumn percentColumn = new DataGridViewComboBoxColumn();
            percentColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            percentColumn.Name = "CommissionPercent";
            percentColumn.HeaderText = "";
            percentColumn.ReadOnly = false;
            List<String> items = new List<string>(21);
            items.AddRange(new String[] { "0", "5", "10", "15", "20", "25", "30", "35", "40", "45", "50", "55", "60", "65", "70", "75", "80", "85", "90", "95", "100" });
            percentColumn.DataSource = items; 
            ProjectGrid.Columns.Add(percentColumn);
            ProjectGrid.Columns["CommissionPercent"].Width = 40;

            ProjectGrid.Columns.Add("isDirty", "isDirty");
            ProjectGrid.Columns["isDirty"].Visible = false;

            ProjectGrid.CellContentClick += new DataGridViewCellEventHandler(ProjectGrid_CellContentClick);
            ProjectGrid.CellValueChanged += new DataGridViewCellEventHandler(ProjectGrid_CellValueChanged);
        }

        private void ProjectAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dr in ProjectGrid.Rows)
            {
                dr.Cells[0].Value = ProjectAll.Checked;
            }
        }

        private void ProjectGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex != -1)
            {
                if ((Boolean)ProjectGrid.CurrentCell.Value)
                {
                    ProjectGrid.CurrentCell.Value = false;
                }
                else
                {
                    ProjectGrid.CurrentCell.Value = true;
                }
            }
            else if (e.ColumnIndex == 6 && e.RowIndex != -1)
            {
                string url = "http://d3crm:5555/D3TECHNOLOGIES/sfa/opps/edit.aspx?id={" + ProjectGrid.CurrentRow.Cells["OpportunityID"].Value + "}#";
                Process.Start(url);
            }
            else if (e.ColumnIndex != 12 && e.RowIndex != -1)
            {
                ProjectGrid.CurrentCell.ReadOnly = true;
            }
        }

        private void ProjectGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 || e.ColumnIndex == 12)
            {
                CalculateProjectTotals();
                CalculateBigTotal();
                ProjectGrid["isDirty", e.RowIndex].Value = "true";
            }
            else if (e.ColumnIndex != 13 && e.ColumnIndex != 14)
            {
                MessageBox.Show("DO NOT CHANGE VALUES WITHIN THE GRID");
            }
        }

        private void LoadProjectGrid(Object[] projectData)
        {
            if (projectData != null)
            {
                foreach (Object[] o in projectData)
                {
                    ProjectGrid.Rows.Add(o);
                }
            }
            ProjectAll.Checked = true;
            CalculateProjectTotals();
        }

        private void CalculateProjectTotals()
        {
            Decimal price = 0;
            Decimal cost = 0;
            Decimal profit = 0;
            Decimal commissionPrice = 0;
            Decimal commissionCost = 0;
            Decimal commission = 0;

            foreach (DataGridViewRow row in ProjectGrid.Rows)
            {
                if ((Boolean)row.Cells[0].Value)
                {
                    if ((String)row.Cells["ExtendedPrice"].Value != "")
                    {
                        price += Convert.ToDecimal(row.Cells["ExtendedPrice"].Value.ToString().Substring(1));
                        commissionPrice += Convert.ToDecimal(row.Cells["ExtendedPrice"].Value.ToString().Substring(1)) * Convert.ToDecimal(row.Cells["CommissionPercent"].Value.ToString()) / 100;
                    }
                    if ((String)row.Cells["ExtendedCost"].Value != "")
                    {
                        cost += Convert.ToDecimal(row.Cells["ExtendedCost"].Value.ToString().Substring(1));
                        commissionCost += Convert.ToDecimal(row.Cells["ExtendedCost"].Value.ToString().Substring(1)) * Convert.ToDecimal(row.Cells["CommissionPercent"].Value.ToString()) / 100;
                    }
                }
            }
            profit = price - cost;
            commission = commissionPrice - commissionCost;
            ProjectPrice.Text = String.Format("${0:f}", price);
            ProjectCost.Text = String.Format("${0:f}", cost);
            ProjectProfit.Text = String.Format("${0:f}", profit);
            ProjectCommission.Text = String.Format("${0:f}", commission);

            sProjectPrice.Text = String.Format("${0:f}", price);
            sProjectCost.Text = String.Format("${0:f}", cost);
            sProjectProfit.Text = String.Format("${0:f}", profit);
            sProjectCommission.Text = String.Format("${0:f}", commission);
        }

        //private ArrayList RetrieveProjectIdsFromGrid()
        //{
        //    ArrayList projList = new ArrayList();
        //    foreach (DataGridViewRow row in ProjectGrid.Rows)
        //    {
        //        if ((Boolean)row.Cells[0].Value)
        //        {
        //            projList.Add(row.Cells["EntityID"].Value.ToString());
        //        }
        //    }
        //    return projList;
        //}

        //private ArrayList RetrieveProjectTypeFromGrid()
        //{
        //    ArrayList projType = new ArrayList();
        //    foreach (DataGridViewRow row in ProjectGrid.Rows)
        //    {
        //        if ((Boolean)row.Cells[0].Value)
        //        {
        //            projType.Add(row.Cells["Type"].Value.ToString());
        //        }
        //    }
        //    return projType;
        //}

        //private ArrayList RetrieveProjectCommissionPercentFromGrid()
        //{
        //    ArrayList projCommissionPercent = new ArrayList();
        //    foreach (DataGridViewRow row in ProjectGrid.Rows)
        //    {
        //        if ((Boolean)row.Cells[0].Value)
        //        {
        //            projCommissionPercent.Add(row.Cells["CommissionPercent"].Value.ToString());
        //        }
        //    }
        //    return projCommissionPercent;
        //}

        //private ArrayList RetrieveDirtyProjects()
        //{
        //    ArrayList dirtyLines = new ArrayList();
        //    Object[] dirtyLine;
        //    foreach (DataGridViewRow row in ProjectGrid.Rows)
        //    {
        //        if (!(Boolean)row.Cells[0].Value || Convert.ToBoolean(row.Cells["isDirty"].Value))
        //        {
        //            dirtyLine = new Object[10];
        //            dirtyLine[0] = row.Cells["Type"].Value.ToString();
        //            dirtyLine[1] = row.Cells["EntityId"].Value.ToString();
        //            dirtyLine[2] = row.Cells["OpportunityName"].Value.ToString();
        //            dirtyLine[3] = row.Cells["ProductName"].Value.ToString();
        //            dirtyLine[4] = row.Cells["ExtendedCost"].Value.ToString();
        //            dirtyLine[5] = row.Cells["ExtendedPrice"].Value.ToString();
        //            dirtyLine[6] = row.Cells["CommissionPercent"].Value.ToString();
        //            dirtyLine[7] = row.Cells["ContactName"].Value.ToString();
        //            dirtyLine[8] = row.Cells["AccountName"].Value.ToString();
        //            dirtyLine[9] = row.Cells["Date"].Value.ToString();
        //            dirtyLines.Add(dirtyLine);
        //        }
        //    }
        //    return dirtyLines;
        //}
        #endregion

        #region Tier1
        private void InitializeTier1Grid()
        {
            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn(false);
            checkColumn.Name = "PayCommission";
            checkColumn.HeaderText = "";
            checkColumn.ReadOnly = false;
            Tier1Grid.Columns.Add(checkColumn);
            Tier1Grid.Columns["PayCommission"].Width = 25;
            
            Tier1Grid.Columns.Add("Type", "Type");
            Tier1Grid.Columns["Type"].Visible = false;
            
            Tier1Grid.Columns.Add("EntityID", "EntityID");
            Tier1Grid.Columns["EntityID"].Visible = false;
            
            Tier1Grid.Columns.Add("SalesPerson", "Sales Person");
            Tier1Grid.Columns["SalesPerson"].Visible = false;
            
            Tier1Grid.Columns.Add("Date", "Date");
            Tier1Grid.Columns["Date"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;

            DataGridViewLinkColumn linkColumn = new DataGridViewLinkColumn();
            linkColumn.Name = "OpportunityName";
            linkColumn.HeaderText = "Opportunity Name";
            linkColumn.UseColumnTextForLinkValue = false;
            linkColumn.LinkBehavior = LinkBehavior.HoverUnderline;
            linkColumn.TrackVisitedState = false;
            Tier1Grid.Columns.Add(linkColumn);
            Tier1Grid.Columns["OpportunityName"].Width = 145;
            
            Tier1Grid.Columns.Add("AccountName", "Account Name");
            Tier1Grid.Columns["AccountName"].Width = 125;
            
            Tier1Grid.Columns.Add("ContactName", "Contact Name");
            Tier1Grid.Columns["ContactName"].Width = 100;
            
            Tier1Grid.Columns.Add("ProductName", "Product Name");
            Tier1Grid.Columns["ProductName"].Width = 145;
            
            Tier1Grid.Columns.Add("Quantity", "Quantity");
            Tier1Grid.Columns["Quantity"].Width = 65;
            
            Tier1Grid.Columns.Add("ExtendedCost", "Extended Cost");
            Tier1Grid.Columns["ExtendedCost"].Width = 65;
            
            Tier1Grid.Columns.Add("ExtendedPrice", "Extended Price");
            Tier1Grid.Columns["ExtendedPrice"].Width = 65;

            Tier1Grid.Columns.Add("OpportunityID", "OpportunityID");
            Tier1Grid.Columns["OpportunityID"].ReadOnly = true;
            Tier1Grid.Columns["OpportunityID"].Visible = false;

            DataGridViewComboBoxColumn percentColumn = new DataGridViewComboBoxColumn();
            percentColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            percentColumn.Name = "CommissionPercent";
            percentColumn.HeaderText = "";
            percentColumn.ReadOnly = false;
            List<String> items = new List<string>(21);
            items.AddRange(new String[] { "0", "5", "10", "15", "20", "25", "30", "35", "40", "45", "50", "55", "60", "65", "70", "75", "80", "85", "90", "95", "100" });
            percentColumn.DataSource = items;
            Tier1Grid.Columns.Add(percentColumn);
            Tier1Grid.Columns["CommissionPercent"].Width = 40;

            Tier1Grid.Columns.Add("isDirty", "isDirty");
            Tier1Grid.Columns["isDirty"].Visible = false;    

            Tier1Grid.CellContentClick += new DataGridViewCellEventHandler(Tier1Grid_CellContentClick);
            Tier1Grid.CellValueChanged += new DataGridViewCellEventHandler(Tier1Grid_CellValueChanged);
        }

        private void Tier1All_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dr in Tier1Grid.Rows)
            {
                dr.Cells[0].Value = Tier1All.Checked;
            }
        }

        private void Tier1Grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex != -1)
            {
                if ((Boolean)Tier1Grid.CurrentCell.Value)
                {
                    Tier1Grid.CurrentCell.Value = false;
                }
                else
                {
                    Tier1Grid.CurrentCell.Value = true;
                }
            }
            else if (e.ColumnIndex == 5 && e.RowIndex != -1)
            {
                string url = "http://d3crm:5555/D3TECHNOLOGIES/sfa/opps/edit.aspx?id={" + Tier1Grid.CurrentRow.Cells["OpportunityID"].Value + "}#";
                Process.Start(url);
            }
            else if (e.ColumnIndex != 11 && e.RowIndex != -1)
            {
                Tier1Grid.CurrentCell.ReadOnly = true;
            }
        }

        private void Tier1Grid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 || e.ColumnIndex == 13)
            {
                CalculateTier1Totals();
                CalculateBigTotal();
                Tier1Grid["isDirty", e.RowIndex].Value = "true";
            }
            else if (e.ColumnIndex != 13 && e.ColumnIndex != 14)
            {
                MessageBox.Show("DO NOT CHANGE VALUES WITHIN THE GRID");
            }
        }

        private void LoadTier1Grid(Object[] tier1Data)
        {
            if (tier1Data != null)
            {
                foreach (Object[] o in tier1Data)
                {
                    Tier1Grid.Rows.Add(o);
                }
            }
            Tier1All.Checked = true;
            CalculateTier1Totals();
        }

        private void CalculateTier1Totals()
        {
            Decimal price = 0;
            Decimal cost = 0;
            Decimal profit = 0;
            Decimal commissionPrice = 0;
            Decimal commissionCost = 0;
            Decimal commission = 0;

            foreach (DataGridViewRow row in Tier1Grid.Rows)
            {
                if ((Boolean)row.Cells[0].Value)
                {
                    if ((String)row.Cells["ExtendedPrice"].Value != "")
                    {
                        price += Convert.ToDecimal(row.Cells["ExtendedPrice"].Value.ToString().Substring(1));
                        commissionPrice += Convert.ToDecimal(row.Cells["ExtendedPrice"].Value.ToString().Substring(1)) * Convert.ToDecimal(row.Cells["CommissionPercent"].Value.ToString()) / 100;
                    }
                    if ((String)row.Cells["ExtendedCost"].Value != "")
                    {
                        cost += Convert.ToDecimal(row.Cells["ExtendedCost"].Value.ToString().Substring(1));
                        commissionCost += Convert.ToDecimal(row.Cells["ExtendedCost"].Value.ToString().Substring(1)) * Convert.ToDecimal(row.Cells["CommissionPercent"].Value.ToString()) / 100;
                    }
                }
            }

            profit = price - cost;
            commission = commissionPrice - commissionCost;

            Tier1Price.Text = String.Format("${0:f}", price);
            Tier1Cost.Text = String.Format("${0:f}", cost);
            Tier1Profit.Text = String.Format("${0:f}", profit);
            Tier1Commission.Text = String.Format("${0:f}", commission);

            sTier1Price.Text = String.Format("${0:f}", price);
            sTier1Cost.Text = String.Format("${0:f}", cost);
            sTier1Profit.Text = String.Format("${0:f}", profit);
            sTier1Commission.Text = String.Format("${0:f}", commission);
        }

        //private ArrayList RetrieveTier1IdsFromGrid()
        //{
        //    ArrayList prodList = new ArrayList();
        //    foreach (DataGridViewRow row in Tier1Grid.Rows)
        //    {
        //        if ((Boolean)row.Cells[0].Value)
        //        {
        //            prodList.Add(row.Cells["EntityID"].Value.ToString());
        //        }
        //    }
        //    return prodList;
        //}

        //private ArrayList RetrieveTier1TypeFromGrid()
        //{
        //    ArrayList prodType = new ArrayList();
        //    foreach (DataGridViewRow row in Tier1Grid.Rows)
        //    {
        //        if ((Boolean)row.Cells[0].Value)
        //        {
        //            prodType.Add(row.Cells["Type"].Value.ToString());
        //        }
        //    }
        //    return prodType;
        //}

        //private ArrayList RetrieveTier1CommissionPercentFromGrid()
        //{
        //    ArrayList prodCommissionPercent = new ArrayList();
        //    foreach (DataGridViewRow row in Tier1Grid.Rows)
        //    {
        //        if ((Boolean)row.Cells[0].Value)
        //        {
        //            prodCommissionPercent.Add(row.Cells["CommissionPercent"].Value.ToString());
        //        }
        //    }
        //    return prodCommissionPercent;
        //}

        //private ArrayList RetrieveDirtyTier1()
        //{
        //    ArrayList dirtyLines = new ArrayList();
        //    Object[] dirtyLine;
        //    foreach (DataGridViewRow row in Tier1Grid.Rows)
        //    {
        //        if (!(Boolean)row.Cells[0].Value || Convert.ToBoolean(row.Cells["isDirty"].Value))
        //        {
        //            dirtyLine = new Object[10];
        //            dirtyLine[0] = row.Cells["Type"].Value.ToString();
        //            dirtyLine[1] = row.Cells["EntityId"].Value.ToString();
        //            dirtyLine[2] = row.Cells["OpportunityName"].Value.ToString();
        //            dirtyLine[3] = row.Cells["ProductName"].Value.ToString();
        //            dirtyLine[4] = row.Cells["ExtendedCost"].Value.ToString();
        //            dirtyLine[5] = row.Cells["ExtendedPrice"].Value.ToString();
        //            dirtyLine[6] = row.Cells["CommissionPercent"].Value.ToString();
        //            dirtyLine[7] = row.Cells["ContactName"].Value.ToString();
        //            dirtyLine[8] = row.Cells["AccountName"].Value.ToString();
        //            dirtyLine[9] = row.Cells["Date"].Value.ToString();
        //            dirtyLines.Add(dirtyLine);
        //        }
        //    }
        //    return dirtyLines;
        //}
        #endregion

        #region Tier2
        private void InitializeTier2Grid()
        {
            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn(false);
            checkColumn.Name = "PayCommission";
            checkColumn.HeaderText = "";
            checkColumn.ReadOnly = false;
            Tier2Grid.Columns.Add(checkColumn);
            Tier2Grid.Columns["PayCommission"].Width = 25;

            Tier2Grid.Columns.Add("Type", "Type");
            Tier2Grid.Columns["Type"].Visible = false;
            
            Tier2Grid.Columns.Add("EntityID", "EntityID");
            Tier2Grid.Columns["EntityID"].Visible = false;
            
            Tier2Grid.Columns.Add("SalesPerson", "Sales Person");
            Tier2Grid.Columns["SalesPerson"].Visible = false;
            
            Tier2Grid.Columns.Add("Date", "Date");
            Tier2Grid.Columns["Date"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;

            DataGridViewLinkColumn linkColumn = new DataGridViewLinkColumn();
            linkColumn.Name = "OpportunityName";
            linkColumn.HeaderText = "Opportunity Name";
            linkColumn.UseColumnTextForLinkValue = false;
            linkColumn.LinkBehavior = LinkBehavior.HoverUnderline;
            linkColumn.TrackVisitedState = false;
            Tier2Grid.Columns.Add(linkColumn);
            Tier2Grid.Columns["OpportunityName"].Width = 145;
            
            Tier2Grid.Columns.Add("AccountName", "Account Name");
            Tier2Grid.Columns["AccountName"].Width = 125;
            
            Tier2Grid.Columns.Add("ContactName", "Contact Name");
            Tier2Grid.Columns["ContactName"].Width = 100;
            
            Tier2Grid.Columns.Add("ProductName", "Product Name");
            Tier2Grid.Columns["ProductName"].Width = 145;
            
            Tier2Grid.Columns.Add("Quantity", "Quantity");
            Tier2Grid.Columns["Quantity"].Width = 65;
            
            Tier2Grid.Columns.Add("ExtendedCost", "Extended Cost");
            Tier2Grid.Columns["ExtendedCost"].Width = 65;
            
            Tier2Grid.Columns.Add("ExtendedPrice", "Extended Price");
            Tier2Grid.Columns["ExtendedPrice"].Width = 65;

            Tier2Grid.Columns.Add("OpportunityID", "OpportunityID");
            Tier2Grid.Columns["OpportunityID"].ReadOnly = true;
            Tier2Grid.Columns["OpportunityID"].Visible = false;

            DataGridViewComboBoxColumn percentColumn = new DataGridViewComboBoxColumn();
            percentColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            percentColumn.Name = "CommissionPercent";
            percentColumn.HeaderText = "";
            percentColumn.ReadOnly = false;
            List<String> items = new List<string>(21);
            items.AddRange(new String[] { "0", "5", "10", "15", "20", "25", "30", "35", "40", "45", "50", "55", "60", "65", "70", "75", "80", "85", "90", "95", "100" });
            percentColumn.DataSource = items; 
            Tier2Grid.Columns.Add(percentColumn);
            Tier2Grid.Columns["CommissionPercent"].Width = 40;
            
            Tier2Grid.Columns.Add("isDirty", "idDirty");
            Tier2Grid.Columns["isDirty"].Visible = false;

            Tier2Grid.CellContentClick += new DataGridViewCellEventHandler(Tier2Grid_CellContentClick);
            Tier2Grid.CellValueChanged += new DataGridViewCellEventHandler(Tier2Grid_CellValueChanged);
        }

        private void Tier2All_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dr in Tier2Grid.Rows)
            {
                dr.Cells[0].Value = Tier2All.Checked;
            }
        }

        void Tier2Grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex != -1)
            {
                if ((Boolean)Tier2Grid.CurrentCell.Value)
                {
                    Tier2Grid.CurrentCell.Value = false;
                }
                else
                {
                    Tier2Grid.CurrentCell.Value = true;
                }
            }
            else if (e.ColumnIndex == 5 && e.RowIndex != -1)
            {
                string url = "http://d3crm:5555/D3TECHNOLOGIES/sfa/opps/edit.aspx?id={" + Tier2Grid.CurrentRow.Cells["OpportunityID"].Value + "}#";
                Process.Start(url);
            }
            else if (e.ColumnIndex != 11 && e.RowIndex != -1)
            {
                Tier2Grid.CurrentCell.ReadOnly = true;
            }
        }

        void Tier2Grid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 || e.ColumnIndex == 13)
            {
                CalculateTier2Totals();
                CalculateBigTotal();
                Tier2Grid["isDirty", e.RowIndex].Value = "true";
            }
            else if (e.ColumnIndex != 13 && e.ColumnIndex != 14)
            {
                MessageBox.Show("DO NOT CHANGE VALUES WITHIN THE GRID");
            }
        }

        private void LoadTier2Grid(Object[] tier2Data)
        {
            if (tier2Data != null)
            {
                foreach (Object[] o in tier2Data)
                {
                    Tier2Grid.Rows.Add(o);
                }
            }
            Tier2All.Checked = true;
            CalculateTier2Totals();
        }

        private void CalculateTier2Totals()
        {
            Decimal price = 0;
            Decimal cost = 0;
            Decimal profit = 0;
            Decimal commissionPrice = 0;
            Decimal commissionCost = 0;
            Decimal commission = 0;

            foreach (DataGridViewRow row in Tier2Grid.Rows)
            {
                if ((Boolean)row.Cells[0].Value)
                {
                    if ((String)row.Cells["ExtendedPrice"].Value != "")
                    {
                        price += Convert.ToDecimal(row.Cells["ExtendedPrice"].Value.ToString().Substring(1));
                        commissionPrice += Convert.ToDecimal(row.Cells["ExtendedPrice"].Value.ToString().Substring(1)) * Convert.ToDecimal(row.Cells["CommissionPercent"].Value.ToString()) / 100;
                    }
                    if ((String)row.Cells["ExtendedCost"].Value != "")
                    {
                        cost += Convert.ToDecimal(row.Cells["ExtendedCost"].Value.ToString().Substring(1));
                        commissionCost += Convert.ToDecimal(row.Cells["ExtendedCost"].Value.ToString().Substring(1)) * Convert.ToDecimal(row.Cells["CommissionPercent"].Value.ToString()) / 100;
                    }
                }
            }

            profit = price - cost;
            commission = commissionPrice - commissionCost;
            Tier2Price.Text = String.Format("${0:f}", price);
            Tier2Cost.Text = String.Format("${0:f}", cost);
            Tier2Profit.Text = String.Format("${0:f}", profit);
            Tier2Commission.Text = String.Format("${0:f}", commission);

            sTier2Price.Text = String.Format("${0:f}", price);
            sTier2Cost.Text = String.Format("${0:f}", cost);
            sTier2Profit.Text = String.Format("${0:f}", profit);
            sTier2Commission.Text = String.Format("${0:f}", commission);
        }

        //private ArrayList RetrieveTier2IdsFromGrid()
        //{
        //    ArrayList prodList = new ArrayList();
        //    foreach (DataGridViewRow row in Tier2Grid.Rows)
        //    {
        //        if ((Boolean)row.Cells[0].Value)
        //        {
        //            prodList.Add(row.Cells["EntityID"].Value.ToString());
        //        }
        //    }
        //    return prodList;
        //}

        //private ArrayList RetrieveTier2TypeFromGrid()
        //{
        //    ArrayList prodType = new ArrayList();
        //    foreach (DataGridViewRow row in Tier2Grid.Rows)
        //    {
        //        if ((Boolean)row.Cells[0].Value)
        //        {
        //            prodType.Add(row.Cells["Type"].Value.ToString());
        //        }
        //    }
        //    return prodType;
        //}

        //private ArrayList RetrieveTier2CommissionPercentFromGrid()
        //{
        //    ArrayList prodCommissionPercent = new ArrayList();
        //    foreach (DataGridViewRow row in Tier2Grid.Rows)
        //    {
        //        if ((Boolean)row.Cells[0].Value)
        //        {
        //            prodCommissionPercent.Add(row.Cells["CommissionPercent"].Value.ToString());
        //        }
        //    }
        //    return prodCommissionPercent;
        //}
        #endregion

        #region Renewal
        private void InitializeRenewalGrid()
        {
            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn(false);
            checkColumn.Name = "PayCommission";
            checkColumn.HeaderText = "";
            checkColumn.ReadOnly = false;
            RenewalGrid.Columns.Add(checkColumn);
            RenewalGrid.Columns["PayCommission"].Width = 25;
            
            RenewalGrid.Columns.Add("Type", "Type");
            RenewalGrid.Columns["Type"].Visible = false;
            
            RenewalGrid.Columns.Add("EntityID", "EntityID");
            RenewalGrid.Columns["EntityID"].Visible = false;
            
            RenewalGrid.Columns.Add("SalesPerson", "Sales Person");
            RenewalGrid.Columns["SalesPerson"].Visible = false;
            
            RenewalGrid.Columns.Add("Date", "Date");
            RenewalGrid.Columns["Date"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;

            DataGridViewLinkColumn linkColumn = new DataGridViewLinkColumn();
            linkColumn.Name = "OpportunityName";
            linkColumn.HeaderText = "Opportunity Name";
            linkColumn.UseColumnTextForLinkValue = false;
            linkColumn.LinkBehavior = LinkBehavior.HoverUnderline;
            linkColumn.TrackVisitedState = false;
            RenewalGrid.Columns.Add(linkColumn);
            RenewalGrid.Columns["OpportunityName"].Width = 145;
            
            RenewalGrid.Columns.Add("AccountName", "Account Name");
            RenewalGrid.Columns["AccountName"].Width = 125;
            
            RenewalGrid.Columns.Add("ContactName", "Contact Name");
            RenewalGrid.Columns["Contactname"].Width = 100;
            
            RenewalGrid.Columns.Add("ProductName", "Product Name");
            RenewalGrid.Columns["Productname"].Width = 145;
            
            RenewalGrid.Columns.Add("Quantity", "Quantity");
            RenewalGrid.Columns["Quantity"].Width = 65;
            
            RenewalGrid.Columns.Add("ExtendedCost", "Extended Cost");
            RenewalGrid.Columns["ExtendedCost"].Width = 65;
            
            RenewalGrid.Columns.Add("ExtendedPrice", "Extended Price");
            RenewalGrid.Columns["ExtendedPrice"].Width = 65;

            RenewalGrid.Columns.Add("OpportunityID", "OpportunityID");
            RenewalGrid.Columns["OpportunityID"].ReadOnly = true;
            RenewalGrid.Columns["OpportunityID"].Visible = false;
            
            DataGridViewComboBoxColumn percentColumn = new DataGridViewComboBoxColumn();
            percentColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            percentColumn.Name = "CommissionPercent";
            percentColumn.HeaderText = "";
            percentColumn.ReadOnly = false;
            List<String> items = new List<string>(21);
            items.AddRange(new String[] { "0", "5", "10", "15", "20", "25", "30", "35", "40", "45", "50", "55", "60", "65", "70", "75", "80", "85", "90", "95", "100" });
            percentColumn.DataSource = items; 
            RenewalGrid.Columns.Add(percentColumn);
            RenewalGrid.Columns["CommissionPercent"].Width = 40;
            
            RenewalGrid.Columns.Add("isDirty", "isDirty");
            RenewalGrid.Columns["isDirty"].Visible = false;

            RenewalGrid.CellContentClick += new DataGridViewCellEventHandler(RenewalGrid_CellContentClick);
            RenewalGrid.CellValueChanged += new DataGridViewCellEventHandler(RenewalGrid_CellValueChanged);
        }

        private void RenewalAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dr in RenewalGrid.Rows)
            {
                dr.Cells[0].Value = RenewalAll.Checked;
            }
        }

        void RenewalGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex != -1)
            {
                if ((Boolean)RenewalGrid.CurrentCell.Value)
                {
                    RenewalGrid.CurrentCell.Value = false;
                }
                else
                {
                    RenewalGrid.CurrentCell.Value = true;
                }
            }
            else if (e.ColumnIndex == 5 && e.RowIndex != -1)
            {
                string url = "http://d3crm:5555/D3TECHNOLOGIES/sfa/opps/edit.aspx?id={" + RenewalGrid.CurrentRow.Cells["OpportunityID"].Value +"}#";
                Process.Start(url);
            }
            else if (e.ColumnIndex != 11 && e.RowIndex != -1)
            {
                RenewalGrid.CurrentCell.ReadOnly = true;
            }
        }

        void RenewalGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 || e.ColumnIndex == 13)
            {
                CalculateRenewalTotals();
                CalculateBigTotal();
                RenewalGrid["isDirty", e.RowIndex].Value = "true";
            }
            else if (e.ColumnIndex != 13 && e.ColumnIndex != 14)
            {
                MessageBox.Show("DO NOT CHANGE VALUES WITHIN THE GRID");
            }
        }

        private void LoadRenewalGrid(Object[] renewalData)
        {
            if (renewalData != null)
            {
                foreach (Object[] o in renewalData)
                {
                    RenewalGrid.Rows.Add(o);
                }
            }
            RenewalAll.Checked = true;
            CalculateRenewalTotals();
        }

        private void CalculateRenewalTotals()
        {
            Decimal price = 0;
            Decimal cost = 0;
            Decimal profit = 0;
            Decimal commissionPrice = 0;
            Decimal commissionCost = 0;
            Decimal commission = 0;

            foreach (DataGridViewRow row in RenewalGrid.Rows)
            {
                if ((Boolean)row.Cells[0].Value)
                {
                    if ((String)row.Cells["ExtendedPrice"].Value != "")
                    {
                        price += Convert.ToDecimal(row.Cells["ExtendedPrice"].Value.ToString().Substring(1));
                        commissionPrice += Convert.ToDecimal(row.Cells["ExtendedPrice"].Value.ToString().Substring(1)) * Convert.ToDecimal(row.Cells["CommissionPercent"].Value.ToString()) / 100;
                    }
                    if ((String)row.Cells["ExtendedCost"].Value != "")
                    {
                        cost += Convert.ToDecimal(row.Cells["ExtendedCost"].Value.ToString().Substring(1));
                        commissionCost += Convert.ToDecimal(row.Cells["ExtendedCost"].Value.ToString().Substring(1)) * Convert.ToDecimal(row.Cells["CommissionPercent"].Value.ToString()) / 100;
                    }
                }
            }

            profit = price - cost;
            commission = commissionPrice - commissionCost;
            RenewalPrice.Text = String.Format("${0:f}", price);
            RenewalCost.Text = String.Format("${0:f}", cost);
            RenewalProfit.Text = String.Format("${0:f}", profit);
            RenewalCommission.Text = String.Format("${0:f}", commission);

            sRenewalPrice.Text = String.Format("${0:f}", price);
            sRenewalCost.Text = String.Format("${0:f}", cost);
            sRenewalProfit.Text = String.Format("${0:f}", profit);
            sRenewalCommission.Text = String.Format("${0:f}", commission);
        }

        //private ArrayList RetrieveRenewalIdsFromGrid()
        //{
        //    ArrayList prodList = new ArrayList();
        //    foreach (DataGridViewRow row in RenewalGrid.Rows)
        //    {
        //        if ((Boolean)row.Cells[0].Value)
        //        {
        //            prodList.Add(row.Cells["EntityID"].Value.ToString());
        //        }
        //    }
        //    return prodList;
        //}

        //private ArrayList RetrieveRenewalTypeFromGrid()
        //{
        //    ArrayList prodType = new ArrayList();
        //    foreach (DataGridViewRow row in RenewalGrid.Rows)
        //    {
        //        if ((Boolean)row.Cells[0].Value)
        //        {
        //            prodType.Add(row.Cells["Type"].Value.ToString());
        //        }
        //    }
        //    return prodType;
        //}

        //private ArrayList RetrieveRenewalCommissionPercentFromGrid()
        //{
        //    ArrayList prodCommissionPercent = new ArrayList();
        //    foreach (DataGridViewRow row in RenewalGrid.Rows)
        //    {
        //        if ((Boolean)row.Cells[0].Value)
        //        {
        //            prodCommissionPercent.Add(row.Cells["CommissionPercent"].Value.ToString());
        //        }
        //    }
        //    return prodCommissionPercent;
        //}

        //private ArrayList RetrieveDirtyTier2()
        //{
        //    ArrayList dirtyLines = new ArrayList();
        //    Object[] dirtyLine;
        //    foreach (DataGridViewRow row in Tier2Grid.Rows)
        //    {
        //        if (!(Boolean)row.Cells[0].Value || Convert.ToBoolean(row.Cells["isDirty"].Value))
        //        {
        //            dirtyLine = new Object[10];
        //            dirtyLine[0] = row.Cells["Type"].Value.ToString();
        //            dirtyLine[1] = row.Cells["EntityId"].Value.ToString();
        //            dirtyLine[2] = row.Cells["OpportunityName"].Value.ToString();
        //            dirtyLine[3] = row.Cells["ProductName"].Value.ToString();
        //            dirtyLine[4] = row.Cells["ExtendedCost"].Value.ToString();
        //            dirtyLine[5] = row.Cells["ExtendedPrice"].Value.ToString();
        //            dirtyLine[6] = row.Cells["CommissionPercent"].Value.ToString();
        //            dirtyLine[7] = row.Cells["ContactName"].Value.ToString();
        //            dirtyLine[8] = row.Cells["AccountName"].Value.ToString();
        //            dirtyLine[9] = row.Cells["Date"].Value.ToString();
        //            dirtyLines.Add(dirtyLine);
        //        }
        //    }
        //    return dirtyLines;
        //}

        //private ArrayList RetrieveDirtyRenewal()
        //{
        //    ArrayList dirtyLines = new ArrayList();
        //    Object[] dirtyLine;
        //    foreach (DataGridViewRow row in RenewalGrid.Rows)
        //    {
        //        if (!(Boolean)row.Cells[0].Value || Convert.ToBoolean(row.Cells["isDirty"].Value))
        //        {
        //            dirtyLine = new Object[10];
        //            dirtyLine[0] = row.Cells["Type"].Value.ToString();
        //            dirtyLine[1] = row.Cells["EntityId"].Value.ToString();
        //            dirtyLine[2] = row.Cells["OpportunityName"].Value.ToString();
        //            dirtyLine[3] = row.Cells["ProductName"].Value.ToString();
        //            dirtyLine[4] = row.Cells["ExtendedCost"].Value.ToString();
        //            dirtyLine[5] = row.Cells["ExtendedPrice"].Value.ToString();
        //            dirtyLine[6] = row.Cells["CommissionPercent"].Value.ToString();
        //            dirtyLine[7] = row.Cells["ContactName"].Value.ToString();
        //            dirtyLine[8] = row.Cells["AccountName"].Value.ToString();
        //            dirtyLine[9] = row.Cells["Date"].Value.ToString();
        //            dirtyLines.Add(dirtyLine);
        //        }
        //    }
        //    return dirtyLines;
        //}
        #endregion

        #region Incomplete
        private void InitializeIncompleteGrid()
        {
            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn(false);
            checkColumn.Name = "PayCommission";
            checkColumn.HeaderText = "";
            checkColumn.ReadOnly = false; 
            IncompleteGrid.Columns.Add(checkColumn);
            IncompleteGrid.Columns["PayCommission"].Width = 25;
            
            IncompleteGrid.Columns.Add("ID", "ID");
            IncompleteGrid.Columns["ID"].ReadOnly = true;
            IncompleteGrid.Columns["ID"].Visible = false;
            
            IncompleteGrid.Columns.Add("SalesPerson", "Sales Person");
            IncompleteGrid.Columns["SalesPerson"].ReadOnly = true;
            IncompleteGrid.Columns["SalesPerson"].Visible = false;
            
            IncompleteGrid.Columns.Add("CloseDate", "Close Date");
            IncompleteGrid.Columns["CloseDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            IncompleteGrid.Columns["CloseDate"].ReadOnly = true;

            DataGridViewLinkColumn linkColumn = new DataGridViewLinkColumn();
            linkColumn.Name = "OpportunityName";
            linkColumn.HeaderText = "Opportunity Name";
            linkColumn.UseColumnTextForLinkValue = false;
            linkColumn.LinkBehavior = LinkBehavior.HoverUnderline;
            linkColumn.TrackVisitedState = false;
            IncompleteGrid.Columns.Add(linkColumn);
            IncompleteGrid.Columns["OpportunityName"].Width = 140;
            IncompleteGrid.Columns["OpportunityName"].ReadOnly = true;
            
            IncompleteGrid.Columns.Add("AccountName", "Account Name");
            IncompleteGrid.Columns["AccountName"].Width = 125;
            IncompleteGrid.Columns["AccountName"].ReadOnly = true;
            
            IncompleteGrid.Columns.Add("EstimatedCost", "Estimated Cost");
            IncompleteGrid.Columns["EstimatedCost"].Width = 65;
            IncompleteGrid.Columns["EstimatedCost"].ReadOnly = true;
            
            IncompleteGrid.Columns.Add("ExtendedPrice", "Price");
            IncompleteGrid.Columns["ExtendedPrice"].Width = 65;
            IncompleteGrid.Columns["ExtendedPrice"].ReadOnly = true;
            
            IncompleteGrid.Columns.Add("ExtendedProfit", "Profit");
            IncompleteGrid.Columns["ExtendedProfit"].Width = 65;
            IncompleteGrid.Columns["ExtendedProfit"].ReadOnly = true;

            DataGridViewComboBoxColumn percentColumn = new DataGridViewComboBoxColumn();
            percentColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            percentColumn.Name = "CommissionPercent";
            percentColumn.HeaderText = "";
            percentColumn.ReadOnly = false;
            List<String> percents = new List<string>(21);
            percents.AddRange(new String[] { "0", "5", "10", "15", "20", "25", "30", "35", "40", "45", "50", "55", "60", "65", "70", "75", "80", "85", "90", "95", "100" });
            percentColumn.DataSource = percents;
            IncompleteGrid.Columns.Add(percentColumn);
            IncompleteGrid.Columns["CommissionPercent"].Width = 40;
            
            IncompleteGrid.Columns.Add("Margin", "Margin");
            IncompleteGrid.Columns["Margin"].Width = 65;
            IncompleteGrid.Columns["Margin"].ReadOnly = true;
            
            IncompleteGrid.Columns.Add("isDirty", "isDirty");
            IncompleteGrid.Columns["isDirty"].Visible = false;
            
            IncompleteGrid.CellContentClick += new DataGridViewCellEventHandler(IncompleteGrid_CellContentClick);
            IncompleteGrid.CellValueChanged += new DataGridViewCellEventHandler(IncompleteGrid_CellValueChanged);
        }

        private void IncompleteAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dr in IncompleteGrid.Rows)
            {
                dr.Cells[0].Value = IncompleteAll.Checked;
            }
        }

        void IncompleteGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex != -1)
            {
                if ((Boolean)IncompleteGrid.CurrentCell.Value)
                {
                    IncompleteGrid.CurrentCell.Value = false;
                }
                else
                {
                    IncompleteGrid.CurrentCell.Value = true;
                }
            }
            else if (e.ColumnIndex == 4 && e.RowIndex != -1)
            {
                string url = "http://d3crm:5555/D3TECHNOLOGIES/sfa/opps/edit.aspx?id={" + IncompleteGrid.CurrentRow.Cells["ID"].Value + "}#";
                Process.Start(url);
            }
            else if (e.ColumnIndex != 9 && e.RowIndex != -1)
            {
                IncompleteGrid.CurrentCell.ReadOnly = true;
            }
        }

        void IncompleteGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 || e.ColumnIndex == 9)
            {
                CalculateIncompleteTotals();
                CalculateBigTotal();
                IncompleteGrid["isDirty", e.RowIndex].Value = "true";
            }
            else if (e.ColumnIndex != 9 && e.ColumnIndex != 11)
            {
                MessageBox.Show("DO NOT CHANGE VALUES WITHIN THE GRID");
            }
        }

        private void LoadIncompleteGrid(Object[] incompleteData)
        {
            if (incompleteData != null)
            {
                foreach (Object[] o in incompleteData)
                {
                    IncompleteGrid.Rows.Add(o);
                }
            }
            IncompleteAll.Checked = true;
            CalculateIncompleteTotals();
        }

        private void CalculateIncompleteTotals()
        {
            Decimal price = 0;
            Decimal cost = 0;
            Decimal profit = 0;
            Decimal commission = 0;

            foreach (DataGridViewRow row in IncompleteGrid.Rows)
            {
                if ((Boolean)row.Cells[0].Value)
                {
                    if ((String)row.Cells["ExtendedPrice"].Value != "")
                    {
                        price += Convert.ToDecimal(row.Cells["ExtendedPrice"].Value.ToString().Substring(1));
                    }
                    if ((String)row.Cells["EstimatedCost"].Value != "")
                    {
                        cost += Convert.ToDecimal(row.Cells["EstimatedCost"].Value.ToString().Substring(1));
                    }
                    if ((String)row.Cells["ExtendedProfit"].Value != "")
                    {
                        profit += Convert.ToDecimal(row.Cells["ExtendedProfit"].Value.ToString().Substring(1));
                        commission += Convert.ToDecimal(row.Cells["ExtendedProfit"].Value.ToString().Substring(1)) * Convert.ToDecimal(row.Cells["CommissionPercent"].Value.ToString()) / 100;
                    }
                }
            }
            IncompletePrice.Text = String.Format("${0:f}", price);
            IncompleteCost.Text = String.Format("${0:f}", cost);
            IncompleteProfit.Text = String.Format("${0:f}", profit);
            IncompleteCommission.Text = String.Format("${0:f}", commission);

            sIncompletePrice.Text = String.Format("${0:f}", price);
            sIncompleteCost.Text = String.Format("${0:f}", cost);
            sIncompleteProfit.Text = String.Format("${0:f}", profit);
            sIncompleteCommission.Text = String.Format("${0:f}", commission);
        }

        //private ArrayList RetrieveIncompleteIdsFromGrid()
        //{
        //    ArrayList incompleteList = new ArrayList();
        //    foreach (DataGridViewRow row in IncompleteGrid.Rows)
        //    {
        //        if ((Boolean)row.Cells[0].Value)
        //        {
        //            incompleteList.Add(row.Cells["ID"].Value.ToString());
        //        }
        //    }
        //    return incompleteList;
        //}

        //private ArrayList RetrieveIncompleteTypeFromGrid()
        //{
        //    ArrayList prodType = new ArrayList();
        //    foreach (DataGridViewRow row in IncompleteGrid.Rows)
        //    {
        //        if ((Boolean)row.Cells[0].Value)
        //        {
        //            prodType.Add(row.Cells["Type"].Value.ToString());
        //        }
        //    }
        //    return prodType;
        //}

        //private ArrayList RetrieveIncompleteCommissionPercentFromGrid()
        //{
        //    ArrayList incompleteCommissionPercent = new ArrayList();
        //    foreach (DataGridViewRow row in IncompleteGrid.Rows)
        //    {
        //        if ((Boolean)row.Cells[0].Value)
        //        {
        //            incompleteCommissionPercent.Add(row.Cells["CommissionPercent"].Value.ToString());
        //        }
        //    }
        //    return incompleteCommissionPercent;
        //}

        //private ArrayList RetrieveDirtyIncomplete()
        //{
        //    ArrayList dirtyLines = new ArrayList();
        //    Object[] dirtyLine;
        //    foreach (DataGridViewRow row in IncompleteGrid.Rows)
        //    {
        //        if (!(Boolean)row.Cells[0].Value || Convert.ToBoolean(row.Cells["isDirty"].Value))
        //        {
        //            dirtyLine = new Object[10];
        //            dirtyLine[0] = "Incomplete";
        //            dirtyLine[1] = row.Cells["ID"].Value.ToString();
        //            dirtyLine[2] = row.Cells["CloseDate"].Value.ToString();
        //            dirtyLine[3] = row.Cells["OpportunityName"].Value.ToString();
        //            dirtyLine[4] = row.Cells["AccountName"].Value.ToString();
        //            dirtyLine[5] = row.Cells["EstimatedCost"].Value.ToString();
        //            dirtyLine[6] = row.Cells["Price"].Value.ToString();
        //            dirtyLine[7] = row.Cells["EstimatedProfit"].Value.ToString();
        //            dirtyLine[8] = row.Cells["CommissionPercent"].Value.ToString();
        //            dirtyLines.Add(dirtyLine);
        //        }
        //    }
        //    return dirtyLines;
        //}
        #endregion

        #region Prior
        private void InitializePriorGrid()
        {
            DataGridViewCheckBoxColumn checkColumn = new DataGridViewCheckBoxColumn(false);
            checkColumn.Name = "PayCommission";
            checkColumn.HeaderText = "";
            checkColumn.ReadOnly = false;
            PriorGrid.Columns.Add(checkColumn);
            PriorGrid.Columns["PayCommission"].Width = 25;

            PriorGrid.Columns.Add("ID", "ID");
            PriorGrid.Columns["ID"].Visible = false;
            
            PriorGrid.Columns.Add("SalesPerson", "Sales Person");
            PriorGrid.Columns["SalesPerson"].Visible = false;
            
            PriorGrid.Columns.Add("CloseDate", "Close Date");
            PriorGrid.Columns["CloseDate"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCellsExceptHeader;
            PriorGrid.Columns["CloseDate"].ReadOnly = true;

            DataGridViewLinkColumn linkColumn = new DataGridViewLinkColumn();
            linkColumn.Name = "OpportunityName";
            linkColumn.HeaderText = "Opportunity Name";
            linkColumn.UseColumnTextForLinkValue = false;
            linkColumn.LinkBehavior = LinkBehavior.HoverUnderline;
            linkColumn.TrackVisitedState = false;
            PriorGrid.Columns.Add(linkColumn);
            PriorGrid.Columns["OpportunityName"].Width = 140;
            PriorGrid.Columns["OpportunityName"].ReadOnly = true;
            
            PriorGrid.Columns.Add("AccountName", "Account Name");
            PriorGrid.Columns["AccountName"].Width = 125;
            PriorGrid.Columns["AccountName"].ReadOnly = true;
            
            PriorGrid.Columns.Add("EstimatedCost", "Estimated Cost");
            PriorGrid.Columns["EstimatedCost"].Width = 65;
            PriorGrid.Columns["EstimatedCost"].ReadOnly = true;
            
            PriorGrid.Columns.Add("ActualCost", "Actual Cost");
            PriorGrid.Columns["ActualCost"].Width = 65;
            PriorGrid.Columns["ActualCost"].ReadOnly = true;
            
            PriorGrid.Columns.Add("Difference", "Difference");
            PriorGrid.Columns["Difference"].Width = 65;
            PriorGrid.Columns["Difference"].ReadOnly = true;
            
            DataGridViewComboBoxColumn percentColumn = new DataGridViewComboBoxColumn();
            percentColumn.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            percentColumn.Name = "CommissionPercent";
            percentColumn.HeaderText = "";
            percentColumn.ReadOnly = false;
            List<String> items = new List<string>(21);
            items.AddRange(new String[] { "0", "5", "10", "15", "20", "25", "30", "35", "40", "45", "50", "55", "60", "65", "70", "75", "80", "85", "90", "95", "100" });
            percentColumn.DataSource = items; 
            PriorGrid.Columns.Add(percentColumn);
            PriorGrid.Columns["CommissionPercent"].Width = 40;
            
            PriorGrid.Columns.Add("isDirty", "isDirty");
            PriorGrid.Columns["isDirty"].Visible = false;

            PriorGrid.CellContentClick += new DataGridViewCellEventHandler(PriorGrid_CellContentClick);
            PriorGrid.CellValueChanged += new DataGridViewCellEventHandler(PriorGrid_CellValueChanged);
        }

        private void PriorAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dr in PriorGrid.Rows)
            {
                dr.Cells[0].Value = PriorAll.Checked;
            }
        }

        void PriorGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex != -1)
            {
                if ((Boolean)PriorGrid.CurrentCell.Value)
                {
                    PriorGrid.CurrentCell.Value = false;
                }
                else
                {
                    PriorGrid.CurrentCell.Value = true;
                }
            }
            else if (e.ColumnIndex == 4 && e.RowIndex != -1)
            {
                string url = "http://d3crm:5555/D3TECHNOLOGIES/sfa/opps/edit.aspx?id={" + PriorGrid.CurrentRow.Cells["ID"].Value + "}#";
                Process.Start(url);
            }
            else if (e.ColumnIndex != 9 && e.RowIndex != -1)
            {
                PriorGrid.CurrentCell.ReadOnly = true;
            }
        }

        void PriorGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0 || e.ColumnIndex == 9)
            {
                CalculatePriorTotals();
                CalculateBigTotal();
                PriorGrid["isDirty", e.RowIndex].Value = "true";
            }
            else if (e.ColumnIndex != 9 && e.ColumnIndex != 10)
            {
                MessageBox.Show("DO NOT CHANGE VALUES WITHIN THE GRID");
            }
        }

        private void LoadPriorGrid(Object[] priorData)
        {
            if (priorData != null)
            {
                foreach (Object[] o in priorData)
                {
                    PriorGrid.Rows.Add(o);
                }
            }
            PriorAll.Checked = true;
            CalculatePriorTotals();
        }

        private void CalculatePriorTotals()
        {
            Decimal estimatedCost = 0;
            Decimal actualCost = 0;
            Decimal difference = 0;
            Decimal commission = 0;

            foreach (DataGridViewRow row in PriorGrid.Rows)
            {
                if ((Boolean)row.Cells[0].Value)
                {
                    if ((String)row.Cells["EstimatedCost"].Value != "")
                    {
                        estimatedCost += Convert.ToDecimal(row.Cells["EstimatedCost"].Value.ToString().Substring(1));
                    }
                    if ((String)row.Cells["ActualCost"].Value != "")
                    {
                        actualCost += Convert.ToDecimal(row.Cells["ActualCost"].Value.ToString().Substring(1));
                    }
                    if ((String)row.Cells["Difference"].Value != "")
                    {
                        difference += Convert.ToDecimal(row.Cells["Difference"].Value.ToString().Substring(1));
                        commission += Convert.ToDecimal(row.Cells["Difference"].Value.ToString().Substring(1)) * Convert.ToDecimal(row.Cells["CommissionPercent"].Value.ToString()) / 100;
                    }
                }
            }
            PriorEstimatedCost.Text = String.Format("${0:f}", estimatedCost);
            PriorActualCost.Text = String.Format("${0:f}", actualCost);
            PriorDifference.Text = String.Format("${0:f}", difference);
            PriorCommission.Text = String.Format("${0:f}", commission);

            sPriorEstimatedCost.Text = String.Format("${0:f}", estimatedCost);
            sPriorActualCost.Text = String.Format("${0:f}", actualCost);
            sPriorDifference.Text = String.Format("${0:f}", difference);
            sPriorCommission.Text = String.Format("${0:f}", commission);
        }

        //private ArrayList RetrievePriorIdsFromGrid()
        //{
        //    ArrayList priorList = new ArrayList();
        //    foreach (DataGridViewRow row in PriorGrid.Rows)
        //    {
        //        if ((Boolean)row.Cells[0].Value)
        //        {
        //            priorList.Add(row.Cells["ID"].Value.ToString());
        //        }
        //    }
        //    return priorList;
        //}

        //private ArrayList RetrievePriorTypeFromGrid()
        //{
        //    ArrayList prodType = new ArrayList();
        //    foreach (DataGridViewRow row in PriorGrid.Rows)
        //    {
        //        if ((Boolean)row.Cells[0].Value)
        //        {
        //            prodType.Add(row.Cells["Type"].Value.ToString());
        //        }
        //    }
        //    return prodType;
        //}

        //private ArrayList RetrievePriorCommissionPercentFromGrid()
        //{
        //    ArrayList priorCommissionPercent = new ArrayList();
        //    foreach (DataGridViewRow row in PriorGrid.Rows)
        //    {
        //        if ((Boolean)row.Cells[0].Value)
        //        {
        //            priorCommissionPercent.Add(row.Cells["CommissionPercent"].Value.ToString());
        //        }
        //    }
        //    return priorCommissionPercent;
        //}

        //private ArrayList RetrieveDirtyPrior()
        //{
        //    ArrayList dirtyLines = new ArrayList();
        //    Object[] dirtyLine;
        //    foreach (DataGridViewRow row in PriorGrid.Rows)
        //    {
        //        if (!(Boolean)row.Cells[0].Value || Convert.ToBoolean(row.Cells["isDirty"].Value))
        //        {
        //            dirtyLine = new Object[10];
        //            dirtyLine[0] = "Prior";
        //            dirtyLine[1] = row.Cells["ID"].Value.ToString();
        //            dirtyLine[2] = row.Cells["CloseDate"].Value.ToString();
        //            dirtyLine[3] = row.Cells["OpportunityName"].Value.ToString();
        //            dirtyLine[4] = row.Cells["AccountName"].Value.ToString();
        //            dirtyLine[5] = row.Cells["EstimatedCost"].Value.ToString();
        //            dirtyLine[6] = row.Cells["ActualCost"].Value.ToString();
        //            dirtyLine[7] = row.Cells["Difference"].Value.ToString();
        //            dirtyLine[8] = row.Cells["CommissionPercent"].Value.ToString();
        //            dirtyLines.Add(dirtyLine);
        //        }
        //    }
        //    return dirtyLines;
        //}
        #endregion
        
    }
}