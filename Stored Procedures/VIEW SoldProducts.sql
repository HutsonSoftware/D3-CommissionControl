CREATE VIEW SoldProducts
AS
SELECT 
	o.owneridname SalesPersonName, 
	i.d3_invoicedate Date, 
	o.d3_companyidname AccountName, 
	o.customeridname ContactName, 
	o.name OpportunityName, 
	id.productidname ProductName, 
	id.priceperunit PricePerUnit, 
	NULL CostPerUnit, 
	id.quantity Quantity, 
	id.manualdiscountamount Discount, 
	id.priceperunit * id.quantity - id.manualdiscountamount ExtendedPrice, 
	NULL ExtendedCost, 
	id.d3_commissionpaid CommissionPaid, 
	'invoicedetail' Type, 
	id.invoicedetailid ID, 
	p.d3_commissioncategoryname Category, 
	CASE 
		WHEN id.d3_commissionpercent IS NOT NULL THEN id.d3_commissionpercent 
		ELSE 
			CASE p.d3_commissioncategoryname 
				WHEN 'Tier 1' THEN su.d3_tier1commissionratename
				WHEN 'Tier 2' THEN su.d3_tier2commissionratename 
				WHEN 'Sub Renewal' THEN su.d3_renewalcommissionratename
			END 
	END CommissionPercent, 
	o.statecodename OpportunityStateCodeName, 
	o.estimatedclosedate
FROM dbo.FilteredInvoice i WITH (NOLOCK)
INNER JOIN dbo.FilteredInvoiceDetail id WITH (NOLOCK) ON i.invoiceid = id.invoiceid 
INNER JOIN dbo.FilteredProduct p WITH (NOLOCK) ON id.productid = p.productid
INNER JOIN dbo.FilteredOpportunity o WITH (NOLOCK) ON i.opportunityid = o.opportunityid 
INNER JOIN dbo.FilteredSystemUser su WITH (NOLOCK) ON o.ownerid = su.systemuserid
WHERE p.d3_commissioncategoryname IN ('Tier 1','Tier 2','Sub Renewal') 

UNION

SELECT 
	o.owneridname SalesPersonName, 
	CASE 
		WHEN so.d3_invoicedate IS NOT NULL THEN so.d3_invoicedate 
		ELSE o.estimatedclosedate 
	END Date, 
	o.d3_companyidname AccountName, 
	o.customeridname ContactName, 
	o.name OpportunityName, 
	sod.productidname ProductName, 
	NULL PricePerUnit, 
	sod.d3_finalcost CostPerUnit, 
	sod.d3_quantity Quantity, 
	sod.d3_autodeskdiscount Discount, 
	NULL ExtnededPrice, 
	(sod.d3_finalcost * sod.quantity) * (1 - sod.d3_autodeskdiscount / 100) ExtendedCost, 
	sod.d3_commissionpaid CommissionPaid, 
	'salesorderdetail' Type, 
	sod.salesorderdetailid ID, 
	p.d3_commissioncategoryname Category, 
	CASE 
		WHEN sod.d3_commissionpercent IS NOT NULL THEN sod.d3_commissionpercent 
		ELSE 
			CASE p.d3_commissioncategoryname 
				WHEN 'Tier 1' THEN su.d3_tier1commissionratename
				WHEN 'Tier 2' THEN su.d3_tier2commissionratename 
				WHEN 'Sub Renewal' THEN su.d3_renewalcommissionratename
			END 
	END CommissionPercent, 
	o.statecodename OpportunityStateCodeName, 
	o.estimatedclosedate
FROM dbo.FilteredSalesOrderDetail sod WITH (NOLOCK) 
INNER JOIN dbo.FilteredSalesOrder so WITH (NOLOCK) ON sod.salesorderid = so.salesorderid 
INNER JOIN dbo.FilteredProduct p WITH (NOLOCK) ON sod.productid = p.productid 
INNER JOIN dbo.FilteredOpportunity o WITH (NOLOCK) ON so.opportunityid = o.opportunityid 
INNER JOIN dbo.FilteredSystemUser su WITH (NOLOCK) ON o.ownerid = su.systemuserid
WHERE p.d3_commissioncategoryname IN ('Tier 1','Tier 2','Sub Renewal') 
