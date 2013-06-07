USE D3_TECHNOLOGIES_MSCRM

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = 'usp_PayInvoiceCommission' AND ROUTINE_TYPE = 'PROCEDURE')	
	EXEC ('CREATE PROCEDURE dbo.usp_PayInvoiceCommission AS SELECT 1')
GO

/*
	Author:		Adam Hutson
	Desc:		Updates the InvoiceDetailExtensionBase for the supplied InvoiceDetailId
	Usage:	
		EXEC dbo.usp_PayInvoiceCommission
			@InvoiceDetailId = ,
			@CommissionPercent = ,
			@CommissionPaid = 
	
	History:	2013/01/14 - Created.
			
*/

ALTER PROCEDURE dbo.usp_PayInvoiceCommission
	@InvoiceDetailId VARCHAR(MAX) = NULL,
	@CommissionPercent VARCHAR(MAX) = NULL,
	@CommissionPaid DATETIME = NULL
AS
SET NOCOUNT ON

UPDATE dbo.InvoiceDetailExtensionBase
SET D3_CommissionPercent = @CommissionPercent,
	D3_CommissionPaid = @CommissionPaid
WHERE InvoiceDetailId = @InvoiceDetailId

GO