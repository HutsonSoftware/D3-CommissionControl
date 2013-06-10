using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace D3.Commission
{
    class DataAccess
    {
        internal DataAccess(Settings settings)
        {
            Settings = settings;
        }

        public Settings Settings { get; set; }

        private LogUtility log;

        public void Log(string logInfo)
        {
            if (log == null)
                log = new LogUtility();

            log.Log(logInfo);
        }

        public String[] GetSalesPeople()
        {
            String[] salesPeople = new String[0];
            try
            {
                ArrayList temp = new ArrayList();
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    SqlDataReader dr = null;
                    SqlCommand cmd = new SqlCommand("usp_GetSalesPeople", conn);
                    cmd.CommandTimeout = 36000; 
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    dr = cmd.ExecuteReader();
                    cmd.Dispose();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            temp.Add(dr[0]);
                        }
                    }
                    salesPeople = new String[temp.Count];
                    for (int i = 0; i < temp.Count; i++)
                    {
                        salesPeople[i] = (String)temp[i];
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            return salesPeople;
        }

        public ArrayList GetCommissionRates(String salesPersonName)
        {
            ArrayList rates = new ArrayList();
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    SqlDataReader dr = null;
                    SqlCommand cmd = new SqlCommand("usp_GetCommissionRates", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 36000;
                    AddSqlParameter(cmd, "@SalesPersonName", SqlDbType.VarChar, 8000, salesPersonName);
                    conn.Open();
                    dr = cmd.ExecuteReader();
                    cmd.Dispose();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            rates.Add(dr["Training"]);
                            rates.Add(dr["Project"]);
                            rates.Add(dr["Tier1"]);
                            rates.Add(dr["Tier2"]);
                            rates.Add(dr["Renewal"]);
                            rates.Add(dr["Incomplete"]);
                            rates.Add(dr["Prior"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            return rates;
        }

        public Object[] GetTrainingData(String salesPersonName, String rate, String endDate)
        {
            Object[] trainingData = null;
            try
            {
                Object[] dataLine;
                ArrayList temp = new ArrayList();
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    SqlDataReader dr = null;
                    SqlCommand cmd = new SqlCommand("usp_GetTrainingData", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 36000;
                    AddSqlParameter(cmd, "@SalesPersonName", SqlDbType.VarChar, 8000, salesPersonName);
                    AddSqlParameter(cmd, "@EndDate", SqlDbType.DateTime, endDate);
                    conn.Open();
                    dr = cmd.ExecuteReader();
                    cmd.Dispose();

                    Decimal price = 0;
                    Decimal cost = 0;
                    Decimal profit = 0;

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            dataLine = new Object[15];
                            dataLine[0] = true;

                            for (int i = 1; i < 7; i++)
                            {
                                dataLine[i] = "";
                            }

                            if (dr["d3_attendeeinvoicedetailid"] != DBNull.Value)
                                dataLine[1] = Convert.ToString(dr["d3_attendeeinvoicedetailid"]);

                            if (dr["SalesPerson"] != DBNull.Value)
                                dataLine[2] = Convert.ToString(dr["SalesPerson"]);

                            if (dr["Date"] != DBNull.Value)
                                dataLine[3] = Convert.ToDateTime(dr["Date"]).ToShortDateString();

                            if (dr["EventName"] != DBNull.Value)
                                dataLine[4] = Convert.ToString(dr["EventName"]);

                            if (dr["OpportunityName"] != DBNull.Value)
                                dataLine[5] = Convert.ToString(dr["OpportunityName"]);

                            if (dr["AccountName"] != DBNull.Value)
                                dataLine[6] = Convert.ToString(dr["AccountName"]);

                            if (dr["ContactName"] != DBNull.Value)
                                dataLine[7] = Convert.ToString(dr["ContactName"]);

                            if (dr["NumberAttended"] == DBNull.Value || Convert.ToDecimal(dr["NumberAttended"]) == 0)
                            {
                                dataLine[4] = "ERROR IN NUMBER ATTENDED Call Admin";
                                dataLine[8] = String.Format("${0:f}", 0);
                                dataLine[9] = String.Format("${0:f}", 0);
                                dataLine[10] = String.Format("${0:f}", 0);
                            }
                            else
                            {
                                cost = Convert.ToDecimal(dr["TotalCost"]) / Convert.ToDecimal(dr["NumberAttended"]);
                                price = Convert.ToDecimal(dr["AttendeePrice"]);
                                profit = price - cost;
                                dataLine[8] = String.Format("${0:f}", cost);
                                dataLine[9] = String.Format("${0:f}", price);
                                dataLine[10] = String.Format("${0:f}", profit);
                            }
                            if (dr["OpportunityID"] != DBNull.Value)
                            {
                                dataLine[11] = Convert.ToString(dr["OpportunityID"]);
                            }

                            dataLine[12] = rate;
                            dataLine[13] = String.Format("%{0:f}", profit / price * 100);
                            dataLine[14] = "false";

                            temp.Add(dataLine);
                        }

                        trainingData = new Object[temp.Count];
                        for (int i = 0; i < temp.Count; i++)
                        {
                            trainingData[i] = temp[i];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            return trainingData;
        }

        public Object[] GetProjectData(String salesPersonName, String rate, String endDate)
        {
            Object[] projectData = null;
            try
            {
                Object[] dataLine;
                ArrayList temp = new ArrayList();
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    SqlDataReader dr = null;
                    SqlCommand cmd = new SqlCommand("usp_GetProjectData", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 36000;
                    AddSqlParameter(cmd, "@SalesPersonName", SqlDbType.VarChar, 8000, salesPersonName);
                    AddSqlParameter(cmd, "@EndDate", SqlDbType.DateTime, endDate);
                    conn.Open();
                    dr = cmd.ExecuteReader();
                    cmd.Dispose();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            dataLine = new Object[15];
                            dataLine[0] = true;
                            if (dr["Type"] != DBNull.Value)
                            {
                                dataLine[1] = Convert.ToString(dr["Type"]);
                            }
                            else
                            {
                                dataLine[1] = "";
                            }
                            if (dr["ID"] != DBNull.Value)
                            {
                                dataLine[2] = Convert.ToString(dr["ID"]);
                            }
                            else
                            {
                                dataLine[2] = "";
                            }
                            if (dr["SalesPersonName"] != DBNull.Value)
                            {
                                dataLine[3] = Convert.ToString(dr["SalesPersonName"]);
                            }
                            else
                            {
                                dataLine[3] = "";
                            }
                            if (dr["datefulfilled"] != DBNull.Value)
                            {
                                dataLine[4] = Convert.ToDateTime(dr["datefulfilled"]).ToShortDateString();
                            }
                            else
                            {
                                dataLine[4] = "";
                            }
                            if (dr["ProjectName"] != DBNull.Value)
                            {
                                dataLine[5] = Convert.ToString(dr["ProjectName"]);
                            }
                            else
                            {
                                dataLine[5] = "";
                            }
                            if (dr["OpportunityName"] != DBNull.Value)
                            {
                                dataLine[6] = Convert.ToString(dr["OpportunityName"]);
                            }
                            else
                            {
                                dataLine[6] = "";
                            }
                            if (dr["AccountName"] != DBNull.Value)
                            {
                                dataLine[7] = Convert.ToString(dr["AccountName"]);
                            }
                            else
                            {
                                dataLine[7] = "";
                            }
                            if (dr["ContactName"] != DBNull.Value)
                            {
                                dataLine[8] = Convert.ToString(dr["ContactName"]);
                            }
                            else
                            {
                                dataLine[8] = "";
                            }
                            if (dr["ProductName"] != DBNull.Value)
                            {
                                dataLine[9] = Convert.ToString(dr["ProductName"]);
                            }
                            else
                            {
                                dataLine[9] = "";
                            }
                            if (dr["ExtendedCost"] != DBNull.Value)
                            {
                                dataLine[10] = String.Format("${0:f}", Convert.ToDecimal(dr["ExtendedCost"]));
                            }
                            else
                            {
                                dataLine[10] = "";
                            }
                            if (dr["ExtendedPrice"] != DBNull.Value)
                            {
                                dataLine[11] = String.Format("${0:f}", Convert.ToDecimal(dr["ExtendedPrice"]));
                            }
                            else
                            {
                                dataLine[11] = "";
                            }
                            if (dr["OpportunityID"] != DBNull.Value)
                            {
                                dataLine[12] = Convert.ToString(dr["OpportunityID"]);
                            }
                            dataLine[13] = rate;
                            dataLine[14] = "false";
                            temp.Add(dataLine);
                        }
                        projectData = new Object[temp.Count];
                        for (int i = 0; i < temp.Count; i++)
                        {
                            projectData[i] = temp[i];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            return projectData;
        }

        public Object[] GetTier1Data(String salesPersonName, String rate, String endDate)
        {
            Object[] tier1Data = null;
            try
            {
                Object[] dataLine;
                ArrayList temp = new ArrayList();
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    SqlDataReader dr = null;
                    SqlCommand cmd = new SqlCommand("usp_GetTier1Data", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 36000;
                    AddSqlParameter(cmd, "@SalesPersonName", SqlDbType.VarChar, 8000, salesPersonName);
                    AddSqlParameter(cmd, "@EndDate", SqlDbType.DateTime, endDate);
                    conn.Open();
                    dr = cmd.ExecuteReader();
                    cmd.Dispose();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            dataLine = new Object[15];
                            dataLine[0] = true;

                            if (dr["Type"] != DBNull.Value)
                            {
                                dataLine[1] = Convert.ToString(dr["Type"]);
                            }
                            else
                            {
                                dataLine[1] = "";
                            }
                            if (dr["ID"] != DBNull.Value)
                            {
                                dataLine[2] = Convert.ToString(dr["ID"]);
                            }
                            else
                            {
                                dataLine[2] = "";
                            }
                            if (dr["SalesPersonName"] != DBNull.Value)
                            {
                                dataLine[3] = Convert.ToString(dr["SalesPersonName"]);
                            }
                            else
                            {
                                dataLine[3] = "";
                            }
                            if (dr["Date"] != DBNull.Value)
                            {
                                dataLine[4] = Convert.ToDateTime(dr["Date"]).ToShortDateString();
                            }
                            else
                            {
                                dataLine[4] = "";
                            }
                            if (dr["OpportunityName"] != DBNull.Value)
                            {
                                dataLine[5] = Convert.ToString(dr["OpportunityName"]);
                            }
                            else
                            {
                                dataLine[5] = "";
                            }
                            if (dr["AccountName"] != DBNull.Value)
                            {
                                dataLine[6] = Convert.ToString(dr["AccountName"]);
                            }
                            else
                            {
                                dataLine[6] = "";
                            }
                            if (dr["ContactName"] != DBNull.Value)
                            {
                                dataLine[7] = Convert.ToString(dr["ContactName"]);
                            }
                            else
                            {
                                dataLine[7] = "";
                            }
                            if (dr["ProductName"] != DBNull.Value)
                            {
                                dataLine[8] = Convert.ToString(dr["ProductName"]);
                            }
                            else
                            {
                                dataLine[8] = "";
                            }
                            if (dr["Quantity"] != DBNull.Value)
                            {
                                dataLine[9] = String.Format("{0:f}", Convert.ToDecimal(dr["Quantity"]));
                            }
                            else
                            {
                                dataLine[9] = "";
                            }
                            if (dr["ExtendedCost"] != DBNull.Value)
                            {
                                dataLine[10] = String.Format("${0:f}", Convert.ToDecimal(dr["ExtendedCost"]));
                            }
                            else
                            {
                                dataLine[10] = "";
                            }
                            if (dr["ExtendedPrice"] != DBNull.Value)
                            {
                                dataLine[11] = String.Format("${0:f}", Convert.ToDecimal(dr["ExtendedPrice"]));
                            }
                            else
                            {
                                dataLine[11] = "";
                            }
                            if (dr["OpportunityID"] != DBNull.Value)
                            {
                                dataLine[12] = Convert.ToString(dr["OpportunityID"]);
                            }

                            dataLine[13] = rate;
                            dataLine[14] = "false";
                            temp.Add(dataLine);
                        }
                        tier1Data = new Object[temp.Count];
                        for (int i = 0; i < temp.Count; i++)
                        {
                            tier1Data[i] = temp[i];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            return tier1Data;
        }

        public Object[] GetTier2Data(String salesPersonName, String rate, String endDate)
        {
            Object[] tier2Data = null;
            try
            {
                Object[] dataLine;
                ArrayList temp = new ArrayList();
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    SqlDataReader dr = null;
                    SqlCommand cmd = new SqlCommand("usp_GetTier2Data", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 36000;
                    AddSqlParameter(cmd, "@SalesPersonName", SqlDbType.VarChar, 8000, salesPersonName);
                    AddSqlParameter(cmd, "@EndDate", SqlDbType.DateTime, endDate);
                    conn.Open();
                    dr = cmd.ExecuteReader();
                    cmd.Dispose();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            dataLine = new Object[15];
                            dataLine[0] = true;

                            if (dr["Type"] != DBNull.Value)
                            {
                                dataLine[1] = Convert.ToString(dr["Type"]);
                            }
                            else
                            {
                                dataLine[1] = "";
                            }
                            if (dr["ID"] != DBNull.Value)
                            {
                                dataLine[2] = Convert.ToString(dr["ID"]);
                            }
                            else
                            {
                                dataLine[2] = "";
                            }
                            if (dr["SalesPersonName"] != DBNull.Value)
                            {
                                dataLine[3] = Convert.ToString(dr["SalesPersonName"]);
                            }
                            else
                            {
                                dataLine[3] = "";
                            }
                            if (dr["Date"] != DBNull.Value)
                            {
                                dataLine[4] = Convert.ToDateTime(dr["Date"]).ToShortDateString();
                            }
                            else
                            {
                                dataLine[4] = "";
                            }
                            if (dr["OpportunityName"] != DBNull.Value)
                            {
                                dataLine[5] = Convert.ToString(dr["OpportunityName"]);
                            }
                            else
                            {
                                dataLine[5] = "";
                            }
                            if (dr["AccountName"] != DBNull.Value)
                            {
                                dataLine[6] = Convert.ToString(dr["AccountName"]);
                            }
                            else
                            {
                                dataLine[6] = "";
                            }
                            if (dr["ContactName"] != DBNull.Value)
                            {
                                dataLine[7] = Convert.ToString(dr["ContactName"]);
                            }
                            else
                            {
                                dataLine[7] = "";
                            }
                            if (dr["ProductName"] != DBNull.Value)
                            {
                                dataLine[8] = Convert.ToString(dr["ProductName"]);
                            }
                            else
                            {
                                dataLine[8] = "";
                            }
                            if (dr["Quantity"] != DBNull.Value)
                            {
                                dataLine[9] = String.Format("{0:f}", Convert.ToDecimal(dr["Quantity"]));
                            }
                            else
                            {
                                dataLine[9] = "";
                            }
                            if (dr["ExtendedCost"] != DBNull.Value)
                            {
                                dataLine[10] = String.Format("${0:f}", Convert.ToDecimal(dr["ExtendedCost"]));
                            }
                            else
                            {
                                dataLine[10] = "";
                            }
                            if (dr["ExtendedPrice"] != DBNull.Value)
                            {
                                dataLine[11] = String.Format("${0:f}", Convert.ToDecimal(dr["ExtendedPrice"]));
                            }
                            else
                            {
                                dataLine[11] = "";
                            }
                            if (dr["OpportunityID"] != DBNull.Value)
                            {
                                dataLine[12] = Convert.ToString(dr["OpportunityID"]);
                            }

                            dataLine[13] = rate;
                            dataLine[14] = "false";
                            temp.Add(dataLine);
                        }
                        tier2Data = new Object[temp.Count];
                        for (int i = 0; i < temp.Count; i++)
                        {
                            tier2Data[i] = temp[i];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            return tier2Data;
        }

        public Object[] GetRenewalData(String salesPersonName, String rate, String endDate)
        {
            Object[] renewalData = null;
            try
            {
                Object[] dataLine;
                ArrayList temp = new ArrayList();
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    SqlDataReader dr = null;
                    SqlCommand cmd = new SqlCommand("usp_GetRenewalData", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 36000;
                    AddSqlParameter(cmd, "@SalesPersonName", SqlDbType.VarChar, 8000, salesPersonName);
                    AddSqlParameter(cmd, "@EndDate", SqlDbType.DateTime, endDate);
                    conn.Open();
                    dr = cmd.ExecuteReader();
                    cmd.Dispose();
                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            dataLine = new Object[15];
                            dataLine[0] = true;

                            if (dr["Type"] != DBNull.Value)
                            {
                                dataLine[1] = Convert.ToString(dr["Type"]);
                            }
                            else
                            {
                                dataLine[1] = "";
                            }
                            if (dr["ID"] != DBNull.Value)
                            {
                                dataLine[2] = Convert.ToString(dr["ID"]);
                            }
                            else
                            {
                                dataLine[2] = "";
                            }
                            if (dr["SalesPersonName"] != DBNull.Value)
                            {
                                dataLine[3] = Convert.ToString(dr["SalesPersonName"]);
                            }
                            else
                            {
                                dataLine[3] = "";
                            }
                            if (dr["Date"] != DBNull.Value)
                            {
                                dataLine[4] = Convert.ToDateTime(dr["Date"]).ToShortDateString();
                            }
                            else
                            {
                                dataLine[4] = "";
                            }
                            if (dr["OpportunityName"] != DBNull.Value)
                            {
                                dataLine[5] = Convert.ToString(dr["OpportunityName"]);
                            }
                            else
                            {
                                dataLine[5] = "";
                            }
                            if (dr["AccountName"] != DBNull.Value)
                            {
                                dataLine[6] = Convert.ToString(dr["AccountName"]);
                            }
                            else
                            {
                                dataLine[6] = "";
                            }
                            if (dr["ContactName"] != DBNull.Value)
                            {
                                dataLine[7] = Convert.ToString(dr["ContactName"]);
                            }
                            else
                            {
                                dataLine[7] = "";
                            }
                            if (dr["ProductName"] != DBNull.Value)
                            {
                                dataLine[8] = Convert.ToString(dr["ProductName"]);
                            }
                            else
                            {
                                dataLine[8] = "";
                            }
                            if (dr["Quantity"] != DBNull.Value)
                            {
                                dataLine[9] = String.Format("{0:f}", Convert.ToDecimal(dr["Quantity"]));
                            }
                            else
                            {
                                dataLine[9] = "";
                            }
                            if (dr["ExtendedCost"] != DBNull.Value)
                            {
                                dataLine[10] = String.Format("${0:f}", Convert.ToDecimal(dr["ExtendedCost"]));
                            }
                            else
                            {
                                dataLine[10] = "";
                            }
                            if (dr["ExtendedPrice"] != DBNull.Value)
                            {
                                dataLine[11] = String.Format("${0:f}", Convert.ToDecimal(dr["ExtendedPrice"]));
                            }
                            else
                            {
                                dataLine[11] = "";
                            }
                            if (dr["OpportunityID"] != DBNull.Value)
                            {
                                dataLine[12] = Convert.ToString(dr["OpportunityID"]);
                            }

                            dataLine[13] = rate;
                            dataLine[14] = "false";
                            temp.Add(dataLine);
                        }
                        renewalData = new Object[temp.Count];
                        for (int i = 0; i < temp.Count; i++)
                        {
                            renewalData[i] = temp[i];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            return renewalData;
        }

        public Object[] GetIncompleteData(String salesPersonName, String rate, String endDate)
        {
            Object[] trainingData = null;
            try
            {
                Object[] dataLine;
                ArrayList temp = new ArrayList();
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    SqlDataReader dr = null;
                    SqlCommand cmd = new SqlCommand("usp_GetIncompleteData", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 36000;
                    AddSqlParameter(cmd, "@SalesPersonName", SqlDbType.VarChar, 8000, salesPersonName);
                    AddSqlParameter(cmd, "@EndDate", SqlDbType.DateTime, endDate);
                    conn.Open();
                    dr = cmd.ExecuteReader();
                    cmd.Dispose();

                    Decimal price = 0;
                    Decimal cost = 0;
                    Decimal profit = 0;

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            dataLine = new Object[12];
                            dataLine[0] = true;

                            for (int i = 1; i < 7; i++)
                            {
                                dataLine[i] = "";
                            }

                            if (dr["ID"] != DBNull.Value)
                                dataLine[1] = Convert.ToString(dr["ID"]);

                            if (dr["SalesPerson"] != DBNull.Value)
                                dataLine[2] = Convert.ToString(dr["SalesPerson"]);

                            if (dr["CloseDate"] != DBNull.Value)
                                dataLine[3] = Convert.ToDateTime(dr["CloseDate"]).ToShortDateString();

                            if (dr["OpportunityName"] != DBNull.Value)
                                dataLine[4] = Convert.ToString(dr["OpportunityName"]);

                            if (dr["AccountName"] != DBNull.Value)
                                dataLine[5] = Convert.ToString(dr["AccountName"]);


                            cost = Convert.ToDecimal(dr["EstimatedCost"]);
                            price = Convert.ToDecimal(dr["ExtendedPrice"]);
                            profit = price - cost;
                            dataLine[6] = String.Format("${0:f}", cost);
                            dataLine[7] = String.Format("${0:f}", price);
                            dataLine[8] = String.Format("${0:f}", profit);

                            dataLine[9] = rate;
                            dataLine[10] = String.Format("%{0:f}", profit / price * 100);
                            dataLine[11] = "false";

                            temp.Add(dataLine);
                        }

                        trainingData = new Object[temp.Count];
                        for (int i = 0; i < temp.Count; i++)
                        {
                            trainingData[i] = temp[i];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            return trainingData;
        }
        
        public Object[] GetPriorData(String salesPersonName, String rate, String endDate)
        {
            Object[] priorData = null;
            try
            {
                Object[] dataLine;
                ArrayList temp = new ArrayList();
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    SqlDataReader dr = null;
                    SqlCommand cmd = new SqlCommand("usp_GetPriorData", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 36000;
                    AddSqlParameter(cmd, "@SalesPersonName", SqlDbType.VarChar, 8000, salesPersonName);
                    AddSqlParameter(cmd, "@EndDate", SqlDbType.DateTime, endDate);
                    conn.Open();
                    dr = cmd.ExecuteReader();
                    cmd.Dispose();

                    Decimal price = 0;
                    Decimal cost = 0;
                    Decimal profit = 0;

                    if (dr.HasRows)
                    {
                        while (dr.Read())
                        {
                            dataLine = new Object[11];
                            dataLine[0] = true;

                            for (int i = 1; i < 7; i++)
                            {
                                dataLine[i] = "";
                            }

                            if (dr["ID"] != DBNull.Value)
                                dataLine[1] = Convert.ToString(dr["ID"]);

                            if (dr["SalesPerson"] != DBNull.Value)
                                dataLine[2] = Convert.ToString(dr["SalesPerson"]);

                            if (dr["CloseDate"] != DBNull.Value)
                                dataLine[3] = Convert.ToDateTime(dr["CloseDate"]).ToShortDateString();

                            if (dr["OpportunityName"] != DBNull.Value)
                                dataLine[4] = Convert.ToString(dr["OpportunityName"]);

                            if (dr["AccountName"] != DBNull.Value)
                                dataLine[5] = Convert.ToString(dr["AccountName"]);


                            cost = Convert.ToDecimal(dr["EstimatedCost"]);
                            price = Convert.ToDecimal(dr["ActualCost"]);
                            profit = cost - price;
                            dataLine[6] = String.Format("${0:f}", cost);
                            dataLine[7] = String.Format("${0:f}", price);
                            dataLine[8] = String.Format("${0:f}", profit);

                            dataLine[9] = rate;
                            dataLine[10] = "false";

                            temp.Add(dataLine);
                        }

                        priorData = new Object[temp.Count];
                        for (int i = 0; i < temp.Count; i++)
                        {
                            priorData[i] = temp[i];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            return priorData;
        }

        //public String PayCommissionManual(String entityid, String type, String percent, String pd)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
        //        {
        //            SqlCommand cmd = new SqlCommand("usp_PayCommissionManual", conn);
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.CommandTimeout = 36000;
        //            AddSqlParameter(cmd, "@EntityID", SqlDbType.VarChar, 8000, entityid);
        //            AddSqlParameter(cmd, "@Type", SqlDbType.VarChar, 8000, type);
        //            AddSqlParameter(cmd, "@Percent", SqlDbType.VarChar, 8000, percent);
        //            AddSqlParameter(cmd, "@Pd", SqlDbType.VarChar, 8000, pd);
        //            conn.Open();
        //            cmd.ExecuteNonQuery();
        //            cmd.Dispose();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log(ex.ToString());
        //    }
        //    return "";
        //}

        private void AddSqlParameter(SqlCommand cmd, string parameterName, SqlDbType sqlDbType, object parameterValue)
        {
            SqlParameter param = cmd.Parameters.Add(parameterName, sqlDbType);
            param.Value = parameterValue;
        }

        private void AddSqlParameter(SqlCommand cmd, string parameterName, SqlDbType sqlDbType, int size, object parameterValue)
        {
            SqlParameter param = cmd.Parameters.Add(parameterName, sqlDbType, size);
            param.Value = parameterValue;
        }

        private void AddSqlParameter(SqlCommand cmd, string parameterName, SqlDbType sqlDbType, byte precision, byte scale, object parameterValue)
        {
            SqlParameter param = cmd.Parameters.Add(parameterName, sqlDbType);
            param.Precision = precision; 
            param.Scale = scale;            
            param.Value = parameterValue;
        }

        internal string PayTrainingCommission(string attendeeInvoiceDetailId, string commissionPercent, string commissionPaid)
        {
            string error = string.Empty;
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("usp_PayTrainingCommission", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 36000;
                    AddSqlParameter(cmd, "@AttendeeInvoiceDetailId", SqlDbType.VarChar, 8000, attendeeInvoiceDetailId);
                    AddSqlParameter(cmd, "@CommissionPercent", SqlDbType.VarChar, 8000, commissionPercent);
                    AddSqlParameter(cmd, "@CommissionPaid", SqlDbType.VarChar, 8000, commissionPaid);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                error = ex.ToString();
            }
            return error;
        }

        internal string PaySalesOrderCommission(string entityId, string commissionPercent, string commissionPaid)
        {
            string error = string.Empty;
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("usp_PaySalesOrderCommission", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 36000;
                    AddSqlParameter(cmd, "@SalesOrderDetailId", SqlDbType.VarChar, 8000, entityId);
                    AddSqlParameter(cmd, "@CommissionPercent", SqlDbType.VarChar, 8000, commissionPercent);
                    AddSqlParameter(cmd, "@CommissionPaid", SqlDbType.VarChar, 8000, commissionPaid);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                error = ex.ToString();
            }
            return error;
        }

        internal string PayInvoiceCommission(string entityId, string commissionPercent, string commissionPaid)
        {
            string error = string.Empty;
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("usp_PayInvoiceCommission", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 36000;
                    AddSqlParameter(cmd, "@InvoiceDetailId", SqlDbType.VarChar, 8000, entityId);
                    AddSqlParameter(cmd, "@CommissionPercent", SqlDbType.VarChar, 8000, commissionPercent);
                    AddSqlParameter(cmd, "@CommissionPaid", SqlDbType.VarChar, 8000, commissionPaid);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                error = ex.ToString();
            }
            return error;
        }

        internal string PayOpportunityCommission(string opportunityId, string commissionCost, string commissionPercent, string commissionDate)
        {
            string error = string.Empty;
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("usp_PayOpportunityCommission", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 36000;
                    AddSqlParameter(cmd, "@OpportunityId", SqlDbType.VarChar, 8000, opportunityId);
                    AddSqlParameter(cmd, "@CommissionCost", SqlDbType.Money, Convert.ToDecimal(commissionCost.Replace("$","")));
                    AddSqlParameter(cmd, "@CommissionPercent", SqlDbType.Decimal, 24, 2, Convert.ToDecimal(commissionPercent));
                    AddSqlParameter(cmd, "@CommissionDate", SqlDbType.VarChar, 8000, commissionDate);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                error = ex.ToString();
            }
            return error;
        }

        internal string PayPriorCommission(string opportunityId, string commissionPercent, string commissionDate)
        {
            string error = string.Empty;
            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.GetConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("usp_PayPriorCommission", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 36000;
                    AddSqlParameter(cmd, "@OpportunityId", SqlDbType.VarChar, 8000, opportunityId);
                    AddSqlParameter(cmd, "@CommissionPercent", SqlDbType.VarChar, 8000, commissionPercent);
                    AddSqlParameter(cmd, "@CommissionPaid", SqlDbType.VarChar, 8000, commissionDate);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                error = ex.ToString();
            }
            return error;
        }
    }
}