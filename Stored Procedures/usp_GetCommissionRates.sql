USE D3_TECHNOLOGIES_MSCRM

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = 'usp_GetCommissionRates' AND ROUTINE_TYPE = 'PROCEDURE')	
	EXEC ('CREATE PROCEDURE dbo.usp_GetCommissionRates AS SELECT 1')
GO

/*
	Author:		Adam Hutson
	Desc:		
	Usage:	
		EXEC dbo.usp_GetCommissionRates
			@SalesPersonName = 'Nanneman, Keith'
			
	History:	2013/01/14 - Created.
			
*/

ALTER PROCEDURE dbo.usp_GetCommissionRates
	@SalesPersonName VARCHAR(MAX)
AS
SET NOCOUNT ON

SELECT 
	d3_trainingcommissionratename AS Training, 
	d3_projectcommissionratename AS Project, 
	d3_tier1commissionratename AS Tier1, 
	d3_tier2commissionratename AS Tier2, 
	d3_renewalcommissionratename AS Renewal,
	N'20' AS Incomplete,
	N'20' AS [Prior]
FROM dbo.FilteredSystemUser WITH (NOLOCK)
WHERE fullname = @SalesPersonName

GO