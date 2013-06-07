USE D3_TECHNOLOGIES_MSCRM

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = 'usp_GetTrainingData' AND ROUTINE_TYPE = 'PROCEDURE')	
	EXEC ('CREATE PROCEDURE dbo.usp_GetTrainingData AS SELECT 1')
GO

/*
	Author:		Adam Hutson
	Desc:		
	Usage:	
		EXEC dbo.usp_GetTrainingData
			@SalesPersonName = 'Corcoran, Trish',
			@EndDate = '12/31/2013'
			
	History:	2013/01/14 - Created.
			
*/

ALTER PROCEDURE dbo.usp_GetTrainingData
	@SalesPersonName VARCHAR(MAX),
	@EndDate DATETIME
AS
SET NOCOUNT ON

DECLARE @StartDate DATETIME
SET @StartDate = '5/1/2009'

SELECT 
	aid.d3_attendeeinvoicedetailid, 
	o.owneridname SalesPerson, 
	i.d3_invoicedate Date, 
	so.d3_eventtrainingidname EventName, 
	o.name OpportunityName, 
	a.d3_attendeescompanyidname AccountName, 
	a.d3_contactidname ContactName, 
	so.d3_totalcost TotalCost, 
	aid.d3_price AttendeePrice, 
	et.d3_attended NumberAttended 
FROM dbo.FilteredSalesOrder so WITH (NOLOCK) 
INNER JOIN dbo.FilteredD3_EventTraining et WITH (NOLOCK) ON so.d3_eventtrainingid = et.d3_eventtrainingid 
INNER JOIN dbo.FilteredD3_Attendee a WITH (NOLOCK) ON et.d3_eventtrainingid = a.d3_eventid 
INNER JOIN dbo.FilteredD3_attendeeinvoicedetail aid WITH (NOLOCK) ON a.d3_attendeeinvoicedetailid = aid.d3_attendeeinvoicedetailid 
INNER JOIN dbo.FilteredInvoice i WITH (NOLOCK) ON aid.d3_invoiceid = i.invoiceid 
INNER JOIN dbo.FilteredOpportunity o WITH (NOLOCK) ON i.opportunityid = o.opportunityid 
INNER JOIN dbo.FilteredSystemUser su WITH (NOLOCK) ON o.ownerid = su.systemuserid
WHERE so.statecodename = 'Fulfilled' 
	AND a.d3_statename = 'Attended' 
	AND aid.d3_commissionpaid IS NULL 
	AND o.owneridname = @SalesPersonName 
	AND i.d3_invoicedate >= DATEADD(day, 4, @StartDate) AND i.d3_invoicedate < DATEADD(day, 5, @EndDate) 
ORDER BY o.name, so.d3_eventtrainingidname

GO