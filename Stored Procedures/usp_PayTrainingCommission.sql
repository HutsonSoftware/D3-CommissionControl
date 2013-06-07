USE D3_TECHNOLOGIES_MSCRM

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = 'usp_PayTrainingCommission' AND ROUTINE_TYPE = 'PROCEDURE')	
	EXEC ('CREATE PROCEDURE dbo.usp_PayTrainingCommission AS SELECT 1')
GO

/*
	Author:		Adam Hutson
	Desc:		Updates the D3_attendeeinvoicedetailExtensionBase for the supplied D3_attendeeinvoicedetailId
	Usage:	
		EXEC dbo.usp_PayTrainingCommission
			@AttendeeInvoiceDetailId = ,
			@CommissionPercent = ,
			@CommissionPaid =  
	
	History:	2013/01/14 - Created.
			
*/

ALTER PROCEDURE dbo.usp_PayTrainingCommission
	@AttendeeInvoiceDetailId VARCHAR(MAX) = NULL,
	@CommissionPercent DECIMAL(24,2) = NULL,
	@CommissionPaid DATETIME = NULL
AS
SET NOCOUNT ON

DECLARE @sql VARCHAR(4000)

UPDATE dbo.D3_attendeeinvoicedetailExtensionBase
SET D3_CommissionPercent = @CommissionPercent,
	D3_CommissionPaid = @CommissionPaid
WHERE D3_attendeeinvoicedetailId = @AttendeeInvoiceDetailId

GO