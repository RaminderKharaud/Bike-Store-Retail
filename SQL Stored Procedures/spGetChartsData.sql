CREATE PROCEDURE spGetChartsData
AS
BEGIN

SELECT ss.store_id, trim(ss.store_name) as store_name, 
year(so.order_date) as order_year, 
count(so.order_id) as total_orders,
sum(oi.quantity * (oi.list_price - oi.discount)) as total_sales
FROM sales.orders so
inner join sales.order_items oi on so.order_id = oi.order_id
inner join sales.stores ss on so.store_id = ss.store_id
GROUP BY year(so.order_date), ss.store_name, ss.store_id
ORDER BY store_id, year(so.order_date)

END;