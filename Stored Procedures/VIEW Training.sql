CREATE VIEW dbo.Training
AS
SELECT 
	 so.d3_totalcost TotalCost, 
	 aid.d3_attendeeinvoicedetailid, 
	CASE 
		WHEN et.d3_attended != 0 
		THEN so.d3_totalcost / et.d3_attended 
		ELSE 0 
	END CostPerAttendee,
	 so.d3_d3po D3PoNumber, 
	 i.d3_invoicedate Date, 
	 et.d3_courseidname EventCourse, 
	 so.d3_eventtrainingidname EventName, 
	 et.d3_localinstructorname Instructor, 
	 et.d3_locationidname Location, 
	 et.d3_primarypresenteridname Presenter, 
	 et.d3_typename EventType, 
	 et.d3_statename EventState, 
	 et.statecodename EventStateCodeName, 
	 so.statecode OrderStateCode, 
	 so.statecodename OrderStateCodeName, 
	 a.d3_name AttendeeName, 
	 a.d3_attendeeinvoicedetailidname AttendeeInvoiceDetailName, 
	 aid.d3_invoiceidname InvoiceName, 
	 a.d3_attendeescompanyid AccountID, 
	 a.d3_attendeescompanyidname AccountName, 
	 a.d3_contactid ContactID, 
	 a.d3_contactidname ContactName, 
	 aid.d3_price AttendeePrice, 
	 o.name OpportunityName, 
	 et.d3_date EventDate, 
	 a.statecode AttendeeStateCode, 
	 a.statecodename AttendeeStateCodeName, 
	 so.ordernumber, 
	 o.statecode OpportunityStatecode, 
	 o.statecodename OpportunityStateCodeName, 
	 aid.statecodename attendeeinvoicedetailstatecodename, 
	 i.statecode invoicestatecode, 
	 i.statecodename invoicestatecodename, 
	 o.owneridname SalesPerson, 
	 et.d3_attended NumberAttended, 
	 a.d3_state AttendeeState, 
	 a.d3_statename AttendeeStateName, 
	 aid.d3_commissionpaid CommissionPaid, 
	CASE 
		WHEN  aid.d3_commissionpercent IS NOT NULL 
		THEN  aid.d3_commissionpercent 
		ELSE  su.d3_trainingcommissionratename 
	END CommissionPercent,
	 o.opportunityid
FROM dbo.FilteredSalesOrder so WITH (NOLOCK) 
INNER JOIN dbo.FilteredD3_EventTraining et WITH (NOLOCK) ON so.d3_eventtrainingid = et.d3_eventtrainingid 
INNER JOIN dbo.FilteredD3_Attendee a WITH (NOLOCK) ON et.d3_eventtrainingid = a.d3_eventid 
INNER JOIN dbo.FilteredD3_attendeeinvoicedetail aid WITH (NOLOCK) ON a.d3_attendeeinvoicedetailid = aid.d3_attendeeinvoicedetailid 
INNER JOIN dbo.FilteredInvoice i WITH (NOLOCK) ON aid.d3_invoiceid = i.invoiceid 
INNER JOIN dbo.FilteredOpportunity o WITH (NOLOCK) ON i.opportunityid = o.opportunityid 
INNER JOIN dbo.FilteredSystemUser su WITH (NOLOCK) ON o.ownerid = su.systemuserid

GO