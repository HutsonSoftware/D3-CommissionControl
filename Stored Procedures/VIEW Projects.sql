USE D3_TECHNOLOGIES_MSCRM
SET NOCOUNT ON
GO

IF NOT EXISTS (SELECT TOP 1 1 FROM sys.views where name = 'Projects')
	EXEC ('CREATE VIEW dbo.Projects AS SELECT 1')
GO

/*
	06/05/2012 Adam Hutson, Added FilteredD3_project.d3_projecttype
*/	
ALTER VIEW [dbo].[Projects]
AS
SELECT 
	FilteredOpportunity.owneridname AS SalesPersonName, 
	FilteredSalesOrder.datefulfilled AS datefulfilled, 
	FilteredD3_project.d3_customeridname AS AccountName, 
	FilteredD3_project.d3_contactidname AS ContactName, 
	FilteredOpportunity.name AS OpportunityName, 
	FilteredD3_project.d3_name AS ProjectName, 
	FilteredInvoiceDetail.productidname AS ProductName, 
	FilteredInvoiceDetail.priceperunit AS PricePerUnit, 
	NULL AS CostPerUnit, 
	FilteredInvoiceDetail.quantity AS Quantity, 
	FilteredInvoiceDetail.manualdiscountamount AS Discount, 
	FilteredInvoiceDetail.priceperunit * FilteredInvoiceDetail.quantity - FilteredInvoiceDetail.manualdiscountamount AS ExtendedPrice, 
	NULL AS ExtendedCost, 
	FilteredInvoiceDetail.d3_commissionpaid AS CommissionDate, 
	CASE 
		WHEN FilteredInvoiceDetail.d3_commissionpercent IS NOT NULL 
		THEN FilteredInvoiceDetail.d3_commissionpercent 
		ELSE FilteredSystemUser.d3_projectcommissionratename 
	END AS CommissionPercent, 
	FilteredInvoiceDetail.invoicedetailid AS ID, 
	'invoicedetail' AS Type, 
	FilteredD3_project.d3_estcompletiondate AS EstimatedDate, 
	FilteredOpportunity.statecodename AS OpportunityStateCodeName, 
	FilteredOpportunity.opportunityid AS OpportunityId,
	FilteredD3_project.d3_projecttype
FROM dbo.FilteredD3_project 
INNER JOIN dbo.FilteredSalesOrder ON dbo.FilteredD3_project.d3_projectid = FilteredSalesOrder.d3_projectid 
INNER JOIN dbo.FilteredOpportunity ON dbo.FilteredD3_project.d3_opportunityid = dbo.FilteredOpportunity.opportunityid 
INNER JOIN dbo.FilteredInvoiceDetail ON dbo.FilteredD3_project.d3_projectid = dbo.FilteredInvoiceDetail.d3_projectid 
INNER JOIN dbo.FilteredSystemUser ON dbo.FilteredOpportunity.ownerid = dbo.FilteredSystemUser.systemuserid
WHERE dbo.FilteredOpportunity.d3_estimatedcommissionpaid is null
	AND dbo.FilteredInvoiceDetail.InvoiceID NOT IN (SELECT InvoiceID FROM dbo.FilteredInvoice WHERE statecode = 3)
UNION
SELECT 
	FilteredOpportunity.owneridname AS SalesPersonName, 
	FilteredSalesOrder.datefulfilled AS datefulfilled, 
	FilteredD3_project.d3_customeridname AS AccountName, 
	FilteredD3_project.d3_contactidname AS ContactName, 
	FilteredOpportunity.name AS OpportunityName, 
	FilteredD3_project.d3_name AS ProjectName, 
	dbo.FilteredSalesOrderDetail.productidname AS ProductName, 
	NULL AS PricePerUnit, 
	dbo.FilteredSalesOrderDetail.d3_finalcost AS CostPerUnit, 
	dbo.FilteredSalesOrderDetail.d3_quantity AS Quantity, 
	dbo.FilteredSalesOrderDetail.d3_autodeskdiscount AS Discount, 
	NULL AS ExtendedPrice, 
	(dbo.FilteredSalesOrderDetail.d3_finalcost * dbo.FilteredSalesOrderDetail.quantity) * (1 - dbo.FilteredSalesOrderDetail.d3_autodeskdiscount / 100) AS ExtendedCost, 
	dbo.FilteredSalesOrderDetail.d3_commissionpaid AS CommissionDate, 
	CASE 
		WHEN dbo.FilteredSalesOrderDetail.d3_commissionpercent IS NOT NULL 
		THEN dbo.FilteredSalesOrderDetail.d3_commissionpercent 
		ELSE su.d3_projectcommissionratename 
	END AS CommissionPercent, 
	dbo.FilteredSalesOrderDetail.salesorderdetailid AS ID, 
	'salesorderdetail' AS Type, 
	FilteredD3_project.d3_estcompletiondate AS EstimatedDate, 
	FilteredOpportunity.statecodename AS OpportunityStateCodeName, 
	FilteredOpportunity.opportunityid AS OpportunityId,
	FilteredD3_project.d3_projecttype
FROM dbo.FilteredD3_project AS FilteredD3_project 
INNER JOIN dbo.FilteredOpportunity AS FilteredOpportunity ON FilteredD3_project.d3_opportunityid = FilteredOpportunity.opportunityid 
INNER JOIN dbo.FilteredSalesOrder AS FilteredSalesOrder ON FilteredD3_project.d3_projectid = FilteredSalesOrder.d3_projectid 
INNER JOIN dbo.FilteredSalesOrderDetail ON FilteredSalesOrder.salesorderid = dbo.FilteredSalesOrderDetail.salesorderid 
INNER JOIN dbo.FilteredSystemUser AS su ON FilteredOpportunity.ownerid = su.systemuserid
WHERE FilteredOpportunity.d3_estimatedcommissionpaid is null