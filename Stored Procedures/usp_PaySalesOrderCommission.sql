USE D3_TECHNOLOGIES_MSCRM

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = 'usp_PaySalesOrderCommission' AND ROUTINE_TYPE = 'PROCEDURE')	
	EXEC ('CREATE PROCEDURE dbo.usp_PaySalesOrderCommission AS SELECT 1')
GO

/*
	Author:		Adam Hutson
	Desc:		Updates the SalesOrderDetailExtensionBase for the supplied SalesOrderDetailId
	Usage:	
		EXEC dbo.usp_PaySalesOrderCommission
			@SalesOrderDetailId = ,
			@CommissionPercent = ,
			@CommissionPaid = 
			
	History:	2013/01/14 - Created.
			
*/

ALTER PROCEDURE dbo.usp_PaySalesOrderCommission
	@SalesOrderDetailId VARCHAR(MAX) = NULL,
	@CommissionPercent VARCHAR(MAX) = NULL,
	@CommissionPaid DATETIME = NULL
AS
SET NOCOUNT ON

UPDATE dbo.SalesOrderDetailExtensionBase
SET D3_CommissionPercent = @CommissionPercent,
	D3_CommissionPaid = @CommissionPaid
WHERE SalesOrderDetailId = @SalesOrderDetailId

GO