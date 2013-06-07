USE D3_TECHNOLOGIES_MSCRM

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_NAME = 'usp_GetSalesPeople' AND ROUTINE_TYPE = 'PROCEDURE')	
	EXEC ('CREATE PROCEDURE dbo.usp_GetSalesPeople AS SELECT 1')
GO

/*
	Author:		Adam Hutson
	Desc:	retrieves all the available Sales People	
	Usage:	
		EXEC dbo.usp_GetSalesPeople
			
	History:	2013/01/14 - Created.
			
*/

ALTER PROCEDURE dbo.usp_GetSalesPeople
AS
SET NOCOUNT ON

SELECT fullname 
FROM dbo.FilteredSystemUser WITH (NOLOCK)
WHERE isdisabled = 0 
ORDER BY fullname

GO