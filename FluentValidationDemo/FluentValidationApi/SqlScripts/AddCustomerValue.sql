DECLARE @MyList TABLE (Value int)
INSERT INTO @MyList VALUES (1)
INSERT INTO @MyList VALUES (1)
INSERT INTO @MyList VALUES (3)
INSERT INTO @MyList VALUES (1)
INSERT INTO @MyList VALUES (2)
INSERT INTO @MyList VALUES (3)
INSERT INTO @MyList VALUES (1)
INSERT INTO @MyList VALUES (1)
INSERT INTO @MyList VALUES (1)
DECLARE @Counter INT 
DECLARE @Email NVARCHAR(100)
SET @Counter=1
DECLARE @value INT
DECLARE db_cursor CURSOR FOR  
SELECT Value FROM @MyList
OPEN db_cursor   
FETCH NEXT FROM db_cursor INTO @value   
WHILE @@FETCH_STATUS = 0   
BEGIN   
       PRINT @value
	   SET @Email= STUFF('eslamelnagar4@gmail.com', 13, 0, CONVERT(nvarchar, @Counter))
	   INSERT INTO Customers (Surname,Forename,Discount,AddressId,Email,customerStatus)
       VALUES (CONCAT('Elnagar',@Counter+100), CONCAT('Islam',@Counter+10),@Counter/(0.7*@Counter+3),1,@Email,@value)
       SET @Counter  = @Counter  + 1
       FETCH NEXT FROM db_cursor INTO @value   
END   

CLOSE db_cursor   
DEALLOCATE db_cursor
SELECT * FROM Customers
