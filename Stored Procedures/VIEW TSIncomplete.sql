CREATE VIEW [dbo].[TSIncomplete]
AS
SELECT 
	Opps.Owner SalesPerson, 
	Opps.opportunityid ID, 
	Opps.estcost EstimatedCost, 
	Invoices.Price ExtendedPrice, 
	Opps.estimatedclosedate CloseDate, 
	su.d3_projectcommissionratename CommissionPercent, 
	(Invoices.Price - Opps.estcost) * su.d3_projectcommissionratename / 100 PBP, 
	Opps.OppName OpportunityName, 
	Opps.Company AccountName, 
	'Opportunity' Type, 
	Opps.d3_estimatedcommissionpaid
FROM 
(	SELECT 
		i.opportunityid,
		SUM(id.extendedamount) Price
	FROM dbo.FilteredInvoice i WITH (NOLOCK)
	INNER JOIN dbo.FilteredInvoiceDetail id WITH (NOLOCK) ON i.invoiceid = id.invoiceid 
	WHERE id.productid IN 
	(	SELECT productid 
		FROM dbo.FilteredProduct WITH (NOLOCK) 
		WHERE producttypecodename IN (N'Training',N'Services')
	)
	GROUP BY i.opportunityid
) Invoices 
INNER JOIN
(	SELECT
		o.name OppName, 
		o.owneridname Owner, 
		o.d3_companyidname Company, 
		o.opportunityid, 
		SUM(p.currentcost * op.quantity) estcost, 
		o.estimatedclosedate, 
		o.d3_estimatedcommissionpaid
	FROM dbo.FilteredProduct p WITH (NOLOCK) 
	INNER JOIN dbo.FilteredOpportunityProduct op WITH (NOLOCK) ON p.productid = op.productid 
	INNER JOIN dbo.FilteredOpportunity o WITH (NOLOCK) ON op.opportunityid = o.opportunityid
	WHERE o.statecodename = N'Won' 
		AND o.estimatedclosedate > CONVERT(DATETIME, '2009-06-01 00:00:00', 102)
		AND p.d3_category1name IN ('Training', 'Services') 
		AND o.opportunityid IN
		(	SELECT DISTINCT o.opportunityid
			FROM dbo.FilteredOpportunity o WITH (NOLOCK) 
			INNER JOIN dbo.FilteredInvoice i  WITH (NOLOCK) ON o.opportunityid = i.opportunityid
			INNER JOIN dbo.FilteredD3_attendeeinvoicedetail aid WITH (NOLOCK) ON i.invoiceid = aid.d3_invoiceid
			INNER JOIN dbo.FilteredD3_Attendee a WITH (NOLOCK) ON aid.d3_attendeeinvoicedetailid = a.d3_attendeeinvoicedetailid
			WHERE a.d3_eventid IN 
			(	SELECT d3_eventtrainingid 
				FROM dbo.FilteredD3_EventTraining WITH (NOLOCK)
				WHERE d3_statename <> N'Complete'
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
			WHERE estimatedclosedate >= '6/1/2009'
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
									WHERE producttypecodename IN ('Services', 'IT Services')
										AND d3_category1name <> N'Service Contract'
								)
						)
				)
		)
		AND o.opportunityid NOT IN
			(	SELECT DISTINCT i.opportunityid
				FROM dbo.FilteredInvoice i WITH (NOLOCK) 
				INNER JOIN dbo.FilteredInvoiceDetail id WITH (NOLOCK) ON i.invoiceid = id.invoiceid 
				WHERE id.d3_commissionpaid IS NOT NULL
			)
	GROUP BY 
		o.opportunityid, 
		o.d3_companyidname, 
		o.d3_estimatedcommissionpaid, 
		o.statecodename, 
		o.owneridname, 
		o.estimatedclosedate, 
		o.name
) Opps ON Invoices.opportunityid = Opps.opportunityid 
INNER JOIN dbo.FilteredSystemUser su WITH (NOLOCK) ON Opps.Owner = su.fullname
