CREATE PROCEDURE spGetProductStock(
 @product_id AS VARCHAR(max) = NULL
 ,@store_id AS VARCHAR(max) = NULL
 ,@brand_id AS VARCHAR(max) = NULL
 ,@category_id AS VARCHAR(max) = NULL
 ,@orderby AS VARCHAR(max) = NULL
 ,@desc AS BIT = 0
 ,@rowOffset INT = 0
 ,@fetchNextRows INT = 50
 )
AS
BEGIN

SELECT
SS.store_id AS 'Store Id',
PP.product_id AS 'Product Id',
SS.store_name AS 'Store Name',
PP.product_name AS 'Product Name',
PB.brand_name AS 'Brand Name',
PC.category_name AS Category,
PS.quantity AS Quantity,
total_count = COUNT(*) OVER()

FROM 
production.stocks PS INNER JOIN production.products PP ON PS.product_id = PP.product_id
INNER JOIN sales.stores SS ON PS.store_id = SS.store_id
INNER JOIN production.brands PB ON PP.brand_id = PB.brand_id
INNER JOIN production.categories PC ON PP.category_id = PC.category_id

WHERE
 (@store_id IS NULL OR SS.store_id = @store_id) AND
 (@product_id IS NULL OR PP.product_id = @product_id) AND
 (@brand_id IS NULL OR PB.brand_id = @brand_id) AND
 (@category_id IS NULL OR PC.category_name = @category_id)

 ORDER BY

	CASE WHEN ((@orderby IS NULL OR LOWER(@orderby) = 'store name') AND @desc = 0) THEN SS.store_name END,
	CASE WHEN ((@orderby IS NULL OR LOWER(@orderby) = 'store name') AND @desc = 1)  THEN SS.store_name END DESC,

	CASE WHEN (LOWER(@orderby) = 'product name' AND @desc = 0) THEN PP.product_name END,
	CASE WHEN (LOWER(@orderby) = 'product name' AND @desc = 1)  THEN PP.product_name END DESC,

	CASE WHEN (LOWER(@orderby) = 'brand name' AND @desc = 0) THEN PB.brand_name END,
	CASE WHEN (LOWER(@orderby) = 'brand name' AND @desc = 1)  THEN PB.brand_name END DESC,

	CASE WHEN (LOWER(@orderby) = 'category name' AND @desc = 0) THEN PC.category_name END,
	CASE WHEN (LOWER(@orderby) = 'category name' AND @desc = 1)  THEN PC.category_name END DESC

OFFSET @rowOffset ROWS FETCH NEXT @fetchNextRows ROWS ONLY;
	
END;
