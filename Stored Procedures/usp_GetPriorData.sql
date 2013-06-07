USE D3_TECHNOLOGIES_MSCRM

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = 'usp_GetPriorData' AND ROUTINE_TYPE = 'PROCEDURE')	
	EXEC ('CREATE PROCEDURE dbo.usp_GetPriorData AS SELECT 1')
GO

/*
	Author:		Adam Hutson
	Desc:		
	Usage:	
		EXEC dbo.usp_GetPriorData
			@SalesPersonName = 'Corcoran, Trish',
			@EndDate = '12/31/2013'
			
	History:	2013/01/14 - Created.

*/

ALTER PROCEDURE dbo.usp_GetPriorData
	@SalesPersonName VARCHAR(MAX),
	@EndDate DATETIME
AS
SET NOCOUNT ON

SELECT 
	Opps.Owner SalesPerson, 
	Opps.opportunityid ID, 
	Opps.estcost EstimatedCost,
	Invoices.Price ExtendedPrice,
	Opps.estimatedclosedate CloseDate,
	su.d3_projectcommissionratename CommissionPercent,
	Opps.OppName OpportunityName,
	OppCosts.ActualCosts ActualCost,
	Opps.Company AccountName
FROM
(	SELECT 
		o.opportunityid, 
		id.d3_commissionpaid,
		SUM(id.extendedamount) Price		
	FROM dbo.FilteredOpportunity o WITH (NOLOCK)
	INNER JOIN dbo.FilteredInvoice i WITH (NOLOCK) ON o.opportunityid = i.opportunityid
	INNER JOIN dbo.FilteredInvoiceDetail id WITH (NOLOCK) ON i.invoiceid = id.invoiceid 
	INNER JOIN dbo.FilteredProduct p WITH (NOLOCK) ON id.productid = p.productid 
	WHERE p.producttypecodename IN (N'Training',N'Services')
		AND o.owneridname = @SalesPersonName
		AND o.estimatedclosedate < DATEADD(day, 5, @EndDate)
		AND id.d3_commissionpaid IS NULL
	GROUP BY o.opportunityid, id.d3_commissionpaid
) Invoices 
INNER JOIN
(	SELECT DISTINCT 
		name OppName, 
		owneridname Owner, 
		opportunityid, 
		d3_estimatedcommissionpaidcost estcost, 
		estimatedclosedate, 
		d3_estimatedcommissionpaid estpaiddate, 
		d3_companyidname Company
	FROM dbo.FilteredOpportunity WITH (NOLOCK) 
	WHERE owneridname = @SalesPersonName
		AND estimatedclosedate < DATEADD(day, 5, @EndDate)
		AND statecodename = N'Won'
		AND d3_estimatedcommissionpaid IS NOT NULL
		AND estimatedclosedate > CONVERT(DATETIME, '2009-06-01 00:00:00', 102)
		AND opportunityid IN 
		(	SELECT opportunityid 
			FROM dbo.FilteredOpportunityProduct WITH (NOLOCK) 
			WHERE productid IN 
			(	SELECT productid 
				FROM dbo.FilteredProduct WITH (NOLOCK) 
				WHERE d3_category1name IN ('Training','Services') 
			)
		)
		AND opportunityid NOT IN
		(	SELECT DISTINCT opportunityid
			FROM dbo.FilteredInvoice WITH (NOLOCK) 
			WHERE invoiceid IN 
			(	SELECT d3_invoiceid 
				FROM dbo.FilteredD3_attendeeinvoicedetail WITH (NOLOCK)
				WHERE d3_attendeeinvoicedetailid IN 
				(	SELECT d3_attendeeinvoicedetailid 
					FROM dbo.FilteredD3_Attendee WITH (NOLOCK) 
					WHERE d3_eventid IN 
					(	SELECT d3_eventtrainingid
						FROM dbo.FilteredD3_EventTraining WITH (NOLOCK) 
						WHERE d3_statename <> N'Complete'
					)
				)
			)
			
			UNION
			
			SELECT DISTINCT d3_opportunityid opportunityid
			FROM dbo.FilteredD3_project WITH (NOLOCK)
			WHERE statecodename = N'Active'
				AND d3_projecttypename <> N'DM Support Contract Upgrade'
			
			UNION
			
			SELECT DISTINCT opportunityid
			FROM dbo.FilteredInvoice WITH (NOLOCK) 
			WHERE statecode <> 3
				AND invoiceid IN 
				(	SELECT d3_invoiceid 
					FROM dbo.FilteredD3_attendeeinvoicedetail WITH (NOLOCK)
					WHERE d3_attendeeinvoicedetailid NOT IN
					(	SELECT d3_attendeeinvoicedetailid
						FROM dbo.FilteredD3_Attendee WITH (NOLOCK)
						WHERE d3_attendeeinvoicedetailid IS NOT NULL
					)
				)

			
			UNION
			
			SELECT DISTINCT opportunityid
			FROM dbo.FilteredOpportunity WITH (NOLOCK) 
			WHERE owneridname = @SalesPersonName
				AND estimatedclosedate < DATEADD(day, 5, @EndDate)
				AND estimatedclosedate >= '6/1/2009'
				AND opportunityid IN 
				(	SELECT opportunityid 
					FROM dbo.FilteredInvoice WITH (NOLOCK)
					WHERE statecode <> 3
						AND invoiceid IN 
						(	SELECT invoiceid 
							FROM dbo.FilteredInvoiceDetail WITH (NOLOCK)
							WHERE d3_projectid IS NULL
								AND productid IN 
								(	SELECT productid 
									FROM dbo.FilteredProduct WITH (NOLOCK)
									WHERE producttypecodename IN ('Services','IT Services') 
										AND d3_category1name <> N'Service Contract'
								)
						)
				)
		)
) Opps ON Invoices.opportunityid = Opps.opportunityid 
INNER JOIN dbo.FilteredSystemUser su WITH (NOLOCK) ON Opps.Owner = su.fullname 
INNER JOIN
(	SELECT
		oppid, 
		d3_commissionpaid,
		SUM(Costs) ActualCosts		
	FROM
	(	SELECT
			i.opportunityid oppid, 
			aid.d3_commissionpaid,
			CASE
				WHEN et.d3_attended <> 0 
				THEN SUM((sod.d3_finalcost * sod.quantity) * (1 - sod.d3_autodeskdiscount / 100)) / et.d3_attended 
				ELSE 0 
			END Costs
		FROM dbo.FilteredInvoice i WITH (NOLOCK)
		INNER JOIN dbo.FilteredD3_attendeeinvoicedetail aid WITH (NOLOCK) ON i.invoiceid = aid.d3_invoiceid
		INNER JOIN dbo.FilteredD3_Attendee a WITH (NOLOCK) ON aid.d3_attendeeinvoicedetailid = a.d3_attendeeinvoicedetailid
		INNER JOIN dbo.FilteredD3_EventTraining et WITH (NOLOCK) ON a.d3_eventid = et.d3_eventtrainingid 
		INNER JOIN dbo.FilteredSalesOrder so WITH (NOLOCK) ON et.d3_eventtrainingid = so.d3_eventtrainingid
		INNER JOIN dbo.FilteredSalesOrderDetail sod WITH (NOLOCK) ON so.salesorderid = sod.salesorderid
		WHERE aid.d3_commissionpaid IS NULL
		GROUP BY 
			i.opportunityid, 
			et.d3_attended, 
			aid.d3_commissionpaid
		
		UNION
		
		SELECT
			p.d3_opportunityid oppid, 
			sod.d3_commissionpaid,
			SUM((sod.d3_finalcost * sod.quantity) * (1 - sod.d3_autodeskdiscount / 100)) Costs	
		FROM dbo.FilteredD3_project p WITH (NOLOCK) 
		INNER JOIN dbo.FilteredSalesOrder so WITH (NOLOCK) ON p.d3_projectid = so.d3_projectid 
		INNER JOIN dbo.FilteredSalesOrderDetail sod WITH (NOLOCK) ON so.salesorderid = sod.salesorderid
		WHERE sod.d3_commissionpaid IS NULL
		GROUP BY 
			p.d3_opportunityid, 
			sod.d3_commissionpaid
	) TSCosts
	GROUP BY 
		oppid, 
		d3_commissionpaid
) OppCosts ON OppCosts.oppid = Opps.opportunityid

GO