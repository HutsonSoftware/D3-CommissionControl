USE D3_TECHNOLOGIES_MSCRM

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = 'usp_GetTier1Data' AND ROUTINE_TYPE = 'PROCEDURE')	
	EXEC ('CREATE PROCEDURE dbo.usp_GetTier1Data AS SELECT 1')
GO

/*
	Author:		Adam Hutson
	Desc:		
	Usage:	
		EXEC dbo.usp_GetTier1Data
			@SalesPersonName = 'Wells, Jamie',
			@EndDate = '6/7/2013'
			
	History:	2013/01/14 - Created.
				2013/06/05 - Added Quantity
			
*/

ALTER PROCEDURE dbo.usp_GetTier1Data
	@SalesPersonName VARCHAR(MAX),
	@EndDate DATETIME
AS
SET NOCOUNT ON

DECLARE @StartDate DATETIME
SET @StartDate = '5/1/2009'

SELECT 
	Type, 
	ID, 
	SalesPersonName, 
	Date, 
	OpportunityName, 
	AccountName, 
	ContactName, 
	ProductName, 
	ExtendedCost, 
	ExtendedPrice,
	Quantity,
	OpportunityID
FROM 
(	SELECT 
		'invoicedetail' Type, 
		id.invoicedetailid ID, 
		o.owneridname SalesPersonName, 
		i.d3_invoicedate Date, 
		o.name OpportunityName, 
		o.d3_companyidname AccountName, 
		o.customeridname ContactName, 
		id.productidname ProductName, 
		NULL ExtendedCost, 
		id.priceperunit * id.quantity - id.manualdiscountamount ExtendedPrice,
		id.quantity,
		o.opportunityid
	FROM dbo.FilteredInvoice i WITH (NOLOCK)
	INNER JOIN dbo.FilteredInvoiceDetail id WITH (NOLOCK) ON i.invoiceid = id.invoiceid 
	INNER JOIN dbo.FilteredProduct p WITH (NOLOCK) ON id.productid = p.productid
	INNER JOIN dbo.FilteredOpportunity o WITH (NOLOCK) ON i.opportunityid = o.opportunityid 
	INNER JOIN dbo.FilteredSystemUser su WITH (NOLOCK) ON o.ownerid = su.systemuserid
	WHERE o.statecodename = 'Won' 
		AND p.d3_commissioncategoryname = 'Tier 1' 
		AND id.d3_commissionpaid IS NULL 
		AND o.owneridname = @SalesPersonName 
		AND i.d3_invoicedate >= @StartDate 
		AND i.d3_invoicedate <= DATEADD(DAY, 5, @EndDate) 
		AND i.statecode <> 3

	UNION

	SELECT 
		'salesorderdetail' Type, 
		sod.salesorderdetailid ID, 
		o.owneridname SalesPersonName, 
		ISNULL(so.d3_invoicedate,o.estimatedclosedate) Date, 
		o.name OpportunityName, 
		o.d3_companyidname AccountName, 
		o.customeridname ContactName, 
		sod.productidname ProductName, 
		(sod.d3_finalcost * sod.quantity) * (1 - sod.d3_autodeskdiscount / 100) ExtendedCost, 
		NULL ExtendedPrice,
		sod.quantity,
		o.opportunityid
	FROM dbo.FilteredSalesOrderDetail sod WITH (NOLOCK) 
	INNER JOIN dbo.FilteredSalesOrder so WITH (NOLOCK) ON sod.salesorderid = so.salesorderid 
	INNER JOIN dbo.FilteredProduct p WITH (NOLOCK) ON sod.productid = p.productid 
	INNER JOIN dbo.FilteredOpportunity o WITH (NOLOCK) ON so.opportunityid = o.opportunityid 
	INNER JOIN dbo.FilteredSystemUser su WITH (NOLOCK) ON o.ownerid = su.systemuserid
	WHERE o.statecodename = 'Won' 
		AND p.d3_commissioncategoryname = 'Tier 1' 
		AND sod.d3_commissionpaid IS NULL 
		AND o.owneridname = @SalesPersonName 
		AND ISNULL(so.d3_invoicedate,o.estimatedclosedate) >= @StartDate 
		AND ISNULL(so.d3_invoicedate,o.estimatedclosedate) <= DATEADD(day, 5, @EndDate) 
) t
ORDER BY Date, OpportunityName, ProductName, ExtendedCost

GO