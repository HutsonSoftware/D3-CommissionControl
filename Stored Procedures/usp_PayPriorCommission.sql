USE D3_TECHNOLOGIES_MSCRM

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = 'usp_PayPriorCommission' AND ROUTINE_TYPE = 'PROCEDURE')	
	EXEC ('CREATE PROCEDURE dbo.usp_PayPriorCommission AS SELECT 1')
GO

/*
	Author:		Adam Hutson
	Desc:		Accepts a OpportunityID, figures the appropriate InvoiceDetailId & SalesOrderDetailId that are associated, then calls the Pay procedures for each
	Usage:	
		EXEC dbo.usp_PayPriorCommission
			@OpportunityId = '<SomeOpportunityID>',
			@CommissionPercent = '20',
			@CommissionPaid = '2012-12-31'
	
	History:	2013/01/14 - Created.
			
*/

ALTER PROCEDURE dbo.usp_PayPriorCommission
	@OpportunityId VARCHAR(MAX) = NULL,
	@CommissionPercent VARCHAR(MAX) = NULL,
	@CommissionPaid DATETIME = NULL
AS
SET NOCOUNT ON

DECLARE @InvoiceDetails TABLE (InvoiceDetailId UNIQUEIDENTIFIER)
DECLARE @AttendeeInvoiceDetails TABLE (AttendeeInvoiceDetailId UNIQUEIDENTIFIER)
DECLARE @InvoiceDetailId UNIQUEIDENTIFIER, @AttendeeInvoiceDetailId UNIQUEIDENTIFIER

INSERT @InvoiceDetails (InvoiceDetailId)
SELECT DISTINCT id.InvoiceDetailId
FROM dbo.Invoice i WITH (NOLOCK)
INNER JOIN dbo.InvoiceDetail id WITH (NOLOCK) ON i.InvoiceId = id.InvoiceId
WHERE i.OpportunityId = @OpportunityID

WHILE 0 < (SELECT COUNT(*) FROM @InvoiceDetails) BEGIN
	SELECT TOP 1 @InvoiceDetailId = InvoiceDetailId FROM @InvoiceDetails
	
	EXEC dbo.usp_PayInvoiceCommission @InvoiceDetailId, @CommissionPercent, @CommissionPaid
	
	DELETE FROM @InvoiceDetails WHERE InvoiceDetailId = @InvoiceDetailId
END

INSERT @AttendeeInvoiceDetails (AttendeeInvoiceDetailId)
SELECT DISTINCT aid.d3_attendeeinvoicedetailid
FROM dbo.Invoice i WITH (NOLOCK)
INNER JOIN dbo.D3_AttendeeInvoiceDetail aid WITH (NOLOCK) ON i.InvoiceId = aid.D3_InvoiceId
WHERE i.OpportunityId = @OpportunityID

WHILE 0 < (SELECT COUNT(*) FROM @AttendeeInvoiceDetails) BEGIN
	SELECT TOP 1 @AttendeeInvoiceDetailId = AttendeeInvoiceDetailId FROM @AttendeeInvoiceDetails
	
	EXEC dbo.usp_PayTrainingCommission @AttendeeInvoiceDetailId, @CommissionPercent, @CommissionPaid
	
	DELETE FROM @AttendeeInvoiceDetails WHERE AttendeeInvoiceDetailId = @AttendeeInvoiceDetailId
END
GO