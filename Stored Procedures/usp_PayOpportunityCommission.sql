USE D3_TECHNOLOGIES_MSCRM

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = 'usp_PayOpportunityCommission' AND ROUTINE_TYPE = 'PROCEDURE')	
	EXEC ('CREATE PROCEDURE dbo.usp_PayOpportunityCommission AS SELECT 1')
GO

/*
	Author:		Adam Hutson
	Desc:		Updates the OpportunityExtensionBase for the supplied OpportunityId
	Usage:	
		EXEC dbo.usp_PayOpportunityCommission
			@OpportunityId = ,
			@CommissionCost = ,
			@CommissionPercent =  ,
			@ExecutionDate
	
	History:	2013/01/14 - Created.
			
*/

ALTER PROCEDURE dbo.usp_PayOpportunityCommission
	@OpportunityId VARCHAR(MAX) = NULL,
	@CommissionCost MONEY = NULL,
	@CommissionPercent DECIMAL(24,2) = NULL,
	@CommissionDate DATETIME = NULL
AS
SET NOCOUNT ON

UPDATE dbo.OpportunityExtensionBase
SET D3_EstimatedCommissionPaidCost = @CommissionCost,
	D3_estimatedcommissionpercent = @CommissionPercent,
	D3_EstimatedCommissionPaid = @CommissionDate
WHERE OpportunityId = @OpportunityId

GO