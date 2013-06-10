USE D3_TECHNOLOGIES_MSCRM

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = 'usp_GetProjectData' AND ROUTINE_TYPE = 'PROCEDURE')	
	EXEC ('CREATE PROCEDURE dbo.usp_GetProjectData AS SELECT 1')
GO

/*
	Author:		Adam Hutson
	Desc:		
	Usage:	
		EXEC dbo.usp_GetProjectData
			@SalesPersonName = 'Corcoran, Trish',
			@EndDate = '12/31/2013'
			
	History:	2013/01/14 - Created.
			
*/

ALTER PROCEDURE dbo.usp_GetProjectData
	@SalesPersonName VARCHAR(MAX),
	@EndDate DATETIME
AS
SET NOCOUNT ON

DECLARE @StartDate DATETIME
SET @StartDate = '5/1/2009'

SELECT 
	Type, 
	ID, 
	SalesPersonName, 
	datefulfilled, 
	ProjectName, 
	OpportunityName,  
	AccountName, 
	ContactName, 
	ProductName, 
	ExtendedCost, 
	ExtendedPrice,
	OpportunityID
FROM dbo.Projects WITH (NOLOCK)
WHERE CommissionDate IS NULL 
	AND datefulfilled IS NOT NULL 
	AND SalesPersonName = @SalesPersonName 
	AND datefulfilled >= @StartDate
	AND datefulfilled <= @EndDate 
	AND ExtendedCost <> 0
	AND d3_projecttype <> 6
ORDER BY ProjectName, ProductName, ExtendedCost

GO
