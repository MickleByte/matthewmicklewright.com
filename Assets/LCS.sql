/*DDL*/
	/*clean slate*/
	DROP DATABASE IF EXISTS lcs;

	/* Create Database */
	CREATE DATABASE LCS;

	/*Create Tables*/
		CREATE TABLE IF NOT EXISTS lcs.customer (
			Email VARCHAR(50) NOT NULL UNIQUE,
			FirstName VARCHAR(35) NOT NULL,
			LastName VARCHAR(35) NOT NULL,
			City VARCHAR(40) NOT NULL,
			Street VARCHAR(35) NOT NULL,
			Postcode VARCHAR(8) NOT NULL,
			HouseNum INT,
			DOB date NOT NULL,
			
			PRIMARY KEY (Email)
		);

		CREATE TABLE IF NOT EXISTS lcs.staff (
			Email VARCHAR(50),
			FirstName VARCHAR(35) NOT NULL,
			LastName VARCHAR(35) NOT NULL,
			City VARCHAR(40) NOT NULL,
			Street VARCHAR(35) NOT NULL,
			Postcode VARCHAR(8) NOT NULL,
			HouseNum INT(3) NOT NULL,
			DOB date NOT NULL,
			
			PRIMARY KEY (Email)
		);

		CREATE TABLE IF NOT EXISTS lcs.supplier (
			SupplierName VARCHAR(50) NOT NULL UNIQUE,
			City VARCHAR(40) NOT NULL,
			Street VARCHAR(35) NOT NULL,
			Postcode VARCHAR(8) NOT NULL,
			HouseNum INT(3) NOT NULL,
			
			PRIMARY KEY (SupplierName)
		);

		CREATE TABLE IF NOT EXISTS lcs.suppliercontacts(
			Email VARCHAR(50) NOT NULL UNIQUE,
			SupplierName VARCHAR(50) NOT NULL,
			
			PRIMARY KEY (Email),
			FOREIGN KEY (SupplierName) REFERENCES Supplier(SupplierName)
		);

		CREATE TABLE IF NOT EXISTS lcs.stock(
			StockItemName VARCHAR(50) NOT NULL UNIQUE,
			Description VARCHAR(255),
			Quantity INT(3) DEFAULT 0,
			
			PRIMARY KEY (StockItemName)
		);

		CREATE TABLE IF NOT EXISTS lcs.service(
			ServiceName VARCHAR(50) NOT NULL UNIQUE,
			ServicePrice decimal(6,2),
			ServiceDescription VARCHAR(255),
			
			PRIMARY KEY (ServiceName)
		);

		CREATE TABLE IF NOT EXISTS lcs.StockForService(
			StockItemName VARCHAR(50) NOT NULL,
			ServiceName VARCHAR(50) NOT NULL,
			Quantity int(3) DEFAULT 1,
			
			
			FOREIGN KEY (ServiceName) REFERENCES service(ServiceName),
			PRIMARY KEY (ServiceName, StockItemName)
		);

		CREATE TABLE IF NOT EXISTS lcs.Orders(
			Discount INT(3) DEFAULT 0,
			OrderID INT NOT NULL AUTO_INCREMENT,
			Location ENUM('Offices', 'Customer Location') NOT NULL DEFAULT 'Offices',
			CustomerEmail varchar(50) NOT NULL,
			StaffEmail varchar (30),
			orderDateTime INT NOT NULL,
			OrderStatus VARCHAR(15),
			FOREIGN KEY (CustomerEmail) REFERENCES customer(Email),
			FOREIGN KEY (StaffEmail) REFERENCES staff(Email),
			PRIMARY KEY (OrderID)
		);

		CREATE TABLE IF NOT EXISTS lcs.ServicesForOrder(
			ServiceID VARCHAR(50) NOT NULL,
			orderDateTime INT NOT NULL,
			CustomerEmail VARCHAR(50) NOT NULL,
			
			PRIMARY KEY(ServiceID, OrderDateTime, CustomerEmail),	
			FOREIGN KEY (ServiceID) REFERENCES service(ServiceName)
		);

		CREATE TABLE IF NOT EXISTS lcs.Invoice(
			DateTimeSent INT NOT NULL,
			CustomerEmail varchar(50) NOT NULL,
			TotalPrice DECIMAL(6,2) NOT NULL,
			
			PRIMARY KEY (DateTimeSent, CustomerEmail),
			FOREIGN KEY (CustomerEmail) REFERENCES customer(Email)
		);

		CREATE TABLE IF NOT EXISTS lcs.StockOrder(
			DateTimeSent INT NOT NULL,
			DateTimeRecieved INT DEFAULT 0,
			SupplierName varchar(50) NOT NULL,
			StockItem varchar(35) NOT NULL,
			
			FOREIGN KEY (SupplierName) REFERENCES Supplier(SupplierName),
			FOREIGN KEY (StockItem) REFERENCES Stock(StockItemName),
			PRIMARY KEY (DateTimeSent, SupplierName, DateTimeRecieved)
		);





	/*Alter Tables */ 
		/*changes primary key to composite orderDateTime and CustomerEmail + drops OrderID */
		ALTER TABLE lcs.Orders 
			ADD PRIMARY KEY (orderDateTime, CustomerEmail),
			DROP COLUMN OrderID;

		ALTER TABLE lcs.ServicesForOrder  
			ADD FOREIGN KEY (orderDateTime) REFERENCES orders(orderDateTime),
			ADD FOREIGN KEY (CustomerEmail) REFERENCES orders(CustomerEmail);

		/*Changes the size of the house num to a 3 digit int instead of default 11*/
		ALTER TABLE lcs.Customer MODIFY HouseNum INT(3) NOT NULL;

		/*Changes order status from varchar to ENUM and sets default as pending*/
		ALTER TABLE lcs.Orders MODIFY OrderStatus ENUM('Pending', 'Confirmed', 'Dispatched', 'Recieved') NOT NULL DEFAULT 'Pending';

		/*make StockItemName foreign key in StockForService*/
		ALTER TABLE lcs.StockForService ADD FOREIGN KEY (StockItemName) REFERENCES stock(StockItemName);	

		/*Adds a new column StockItemQuantity to stockOrder w/ default of 1*/
		ALTER TABLE lcs.StockOrder ADD StockItemQunatity INT(3) DEFAULT 1;
			
		/*Add Not Null and Unique constraints to Email in staff table*/
		ALTER TABLE lcs.staff MODIFY Email VARCHAR(50) NOT NULL UNIQUE;	
	
		ALTER TABLE lcs.StockOrder ADD OrderCost DECIMAL(6,2);

		
	/*Stored Procedures to validate some inputs*/ 	
		
		/*Validation for Discount in order*/
		DELIMITER $$
		CREATE PROCEDURE lcs.check_order(IN Discount INT(3))
		BEGIN
			IF Discount < 0 THEN
				SIGNAL SQLSTATE '45000'
				SET MESSAGE_TEXT = 'check constraint on order.discount failed - value must be integer % between 0 and 100';
			END IF;   
			IF Discount > 100 THEN
				SIGNAL SQLSTATE '45000'
				SET MESSAGE_TEXT = 'check constraint on order.discount failed - value must be integer % between 0 and 100';
			END IF;
		END$$ 
		DELIMITER ;
		
		/*Calls above procedure on:*/
		/*Insert*/
		DELIMITER $$
		CREATE TRIGGER lcs.check_order_insert BEFORE INSERT ON lcs.orders
		FOR EACH ROW
		BEGIN
			CALL lcs.check_order(new.Discount);
		END$$   
		DELIMITER ; 

		/*Update*/
		DELIMITER $$
		CREATE TRIGGER lcs.check_order_update BEFORE UPDATE ON lcs.orders
		FOR EACH ROW
		BEGIN
			CALL lcs.chec_order(new.Discount);
           
		END$$   
		DELIMITER ;
		
		/*Validation for quantity in stock order*/
		DELIMITER $$
		CREATE PROCEDURE lcs.check_stockOrder(IN Quantity INT(3))
		BEGIN
			IF Quantity < 0 THEN
				SIGNAL SQLSTATE '45000'
					SET MESSAGE_TEXT = 'check constraint on StockOrder.quantity failed';
			END IF;   
		END$$ 
		DELIMITER ;

		/*Calls above procedure on:*/
		/*Insert*/
		DELIMITER $$
		CREATE TRIGGER lcs.stockOrder_before_insert BEFORE INSERT ON lcs.StockOrder
		FOR EACH ROW
		BEGIN
			CALL lcs.check_stockOrder(new.StockItemQunatity);
		END$$   
		DELIMITER ; 

		/*Update*/
		DELIMITER $$
		CREATE TRIGGER lcs.StockOrder_before_update BEFORE UPDATE ON lcs.StockOrder
		FOR EACH ROW
		BEGIN
			CALL lcs.check_stockOrder(new.StockItemQunatity);
		END$$   
		DELIMITER ;
     
        
         
/*DML*/
	/*Inert data into tables*/
		/*Insert customers*/
			INSERT INTO lcs.customer (Email, FirstName, LastName, City, Street, Postcode, HouseNum, DOB)
			VALUES ("henryclop@gmail.com", "Henry", "Clop", "Coventry", "Box Rd", "CV1 1DD", 36, '1999-09-15');

			INSERT INTO lcs.customer (Email, FirstName, LastName, City, Street, Postcode, HouseNum, DOB)
			VALUES ("janebloggs@gmail.com", "Jane", "Blogs", "Coventry", "Box Rd", "CV1 1DD", 34, '1983-02-16');

			INSERT INTO lcs.customer (Email, FirstName, LastName, City, Street, Postcode, HouseNum, DOB)
			VALUES ("ericsmith@hotmail.com", "Eric", "Smith", "Derby", "Elm Rd", "DE10 3PJ", 4, '1993-11-30');

			INSERT INTO lcs.customer (Email, FirstName, LastName, City, Street, Postcode, HouseNum, DOB)
			VALUES ("alicehall@hotmail.com", "Alice", "Hall", "Nottingham", "Dovedale Avenue", "NG5 5KO", 5, '1959-04-19');

			INSERT INTO lcs.customer (Email, FirstName, LastName, City, Street, Postcode, HouseNum, DOB)
			VALUES ("janecodd@hotmail.com", "Jane", "Codd", "Edinburgh", "Arthur Lane", "EH1 1HN", 1, '1980-07-15');

			INSERT INTO lcs.customer (Email, FirstName, LastName, City, Street, Postcode, HouseNum, DOB)
			VALUES ("trevormcdonald@hotmail.com", "Trevor", "McDonald", "Lincoln", "West Parade", "LN1 1PQ", 45, '1975-06-20');

			INSERT INTO lcs.customer (Email, FirstName, LastName, City, Street, Postcode, HouseNum, DOB)
			VALUES ("rogerwoods@hotmail.com", "Roger", "Woods", "Lincoln", "Carholme Road", "LN1 1ST", 52, '1988-08-23');

			INSERT INTO lcs.customer (Email, FirstName, LastName, City, Street, Postcode, HouseNum, DOB)
			VALUES ("adamelse@gmail.com", "Adam", "Else", "St Ives", "Bell Road", "PE27 3EB", 8, '1995-05-03');

			INSERT INTO lcs.customer (Email, FirstName, LastName, City, Street, Postcode, HouseNum, DOB)
			VALUES ("paulinerichards33@gmail.com", "Pauline", "Richards", "London", "King's Road", "SW1 1AA", 67, '1990-09-04');

			INSERT INTO lcs.customer (Email, FirstName, LastName, City, Street, Postcode, HouseNum, DOB)
			VALUES ("miajohnson@gmail.com", "Mia", "Johnson", "Lincoln", "Drake Street", "LN1 1RK", 4, '2000-10-15');

			
		/*Insert Staff*/
			INSERT INTO lcs.staff (Email, FirstName, LastName, City, Street, Postcode, HouseNum, DOB)
			VALUES ("sbiggles@lcs.com", "Sheila", "Biggles", "Lincoln", "Moor Street", "LN1 1PN", 9, '1969-11-01');

			INSERT INTO lcs.staff (Email, FirstName, LastName, City, Street, Postcode, HouseNum, DOB)
			VALUES ("dprowse@lcs.com", "David", "Prowse", "Lincoln", "Vader Avenue", "LN1 6RS", 1, '1984-01-15');

			INSERT INTO lcs.staff (Email, FirstName, LastName, City, Street, Postcode, HouseNum, DOB)
			VALUES ("gboole@lcs.com", "George", "Boole", "Lincoln", "Pottergate", "LN2 1PH", 3, '1992-12-13');

			INSERT INTO lcs.staff (Email, FirstName, LastName, City, Street, Postcode, HouseNum, DOB)
			VALUES ("cshannon@lcs.com", "Claude", "Shannon", "Lincoln", "Petoskey Drive", "LN3 1PS", 26, '1972-01-14');

			INSERT INTO lcs.staff (Email, FirstName, LastName, City, Street, Postcode, HouseNum, DOB)
			VALUES ("landerson@lcs.com", "Leroy", "Anderson", "Newark", "Sleigh Drive", "LN9 1SZ", 4, '1981-02-22');

			INSERT INTO lcs.staff (Email, FirstName, LastName, City, Street, Postcode, HouseNum, DOB)
			VALUES ("alovelace@lcs.com", "Ava", "Lovelace", "Hykeham", "Babbage Street", "LN6 1OP", 12, '1996-06-21');

			
		/*Insert Supplier*/
			INSERT INTO lcs.supplier (SupplierName, city, Street, postcode, HouseNum)
			VALUES ("Sibelius' Hard Drives", "Grantham", "Finlandia Street", "NG31 6NZ", 2);

			INSERT INTO lcs.supplier (SupplierName, city, Street, postcode, HouseNum)
			VALUES ("Cables By Berlioz", "Grantham", "Hungarian March", "NG31 6HY", 65);

			INSERT INTO lcs.supplier (SupplierName, city, Street, postcode, HouseNum)
			VALUES ("IBM", "Nottingham", "City Gate East", "NG1 5FS", 6);

			
		/*Insert to SupplierContacts*/
			INSERT INTO lcs.suppliercontacts (Email, SupplierName)
			VALUES ("distribution@sibeliushd.com", "Sibelius' Hard Drives");

			INSERT INTO lcs.suppliercontacts (Email, SupplierName)
			VALUES ("md@sibeliushd.com", "Sibelius' Hard Drives");

			INSERT INTO lcs.suppliercontacts (Email, SupplierName)
			VALUES ("jean@sibeliushd.com", "Sibelius' Hard Drives");

			INSERT INTO lcs.suppliercontacts (Email, SupplierName)
			VALUES ("cabledistribution@berliozcables.com", "Cables By Berlioz");

			INSERT INTO lcs.suppliercontacts (Email, SupplierName)
			VALUES ("warehousemanager@berliozcables.com", "Cables By Berlioz");

			INSERT INTO lcs.suppliercontacts (Email, SupplierName)
			VALUES ("hector@berliozcables.com", "Cables By Berlioz");

			INSERT INTO lcs.suppliercontacts (Email, SupplierName)
			VALUES ("watson@ibm.com", "IBM");

			INSERT INTO lcs.suppliercontacts (Email, SupplierName)
			VALUES ("watsonjr@ibm.com", "IBM");

		/*Insert Services*/
			INSERT INTO lcs.service(ServiceName, ServicePrice, ServiceDescription)
			VALUES ("Replace Keyboard on Laptop", 20.0, "Remove old keyboard and dispose of it, wire new keyboard and fit it");

			INSERT INTO lcs.service(ServiceName, ServicePrice, ServiceDescription)
			VALUES ("Repair Screen on Laptop", 100.0, "Remove old Screen and dispose of it, fit new screen and wire it");

			INSERT INTO lcs.service(ServiceName, ServicePrice, ServiceDescription)
			VALUES ("Add 8GB RAM", 120.0, "Open Computer, fit ram, seal computer up again");

			INSERT INTO lcs.service(ServiceName, ServicePrice, ServiceDescription)
			VALUES ("Replace Hard Drive", 100.0, "Open Computer, remove and destroy old HD, replace with new HD, seal computer up again");
			
		/*Insert stock*/
			INSERT INTO lcs.Stock(StockItemName, Description, Quantity)
			VALUES ("1TB HD", "1TB empty hard drives", 3);

			INSERT INTO lcs.Stock(StockItemName, Description, Quantity)
			VALUES ("1m USB", "1 metre long usb male to usb male cables", 1);

			INSERT INTO lcs.Stock(StockItemName, Description, Quantity)
			VALUES ("Laptop Screen", "New Laptop Screens with associative cables", 0);

			INSERT INTO lcs.Stock(StockItemName, Description, Quantity)
			VALUES ("4GB DDR4 RAM", "Sticks of 4GB DDR4 RAM", 6);

			INSERT INTO lcs.Stock(StockItemName, Quantity)
			VALUES ("Laptop Keyboard", 7);
			
			INSERT INTO lcs.Stock(StockItemName, Quantity)
			VALUES ("Ribbon Cable", 9);

			INSERT INTO lcs.Stock(StockItemName)
			VALUES ("ZIF Connector");

			
		

			
		/*Insert orders*/
			INSERT INTO lcs.Orders(Discount, Location, CustomerEmail, StaffEmail, orderDateTime, OrderStatus)
			VALUES (0, "Customer Location", "janecodd@hotmail.com", "dprowse@lcs.com", 1384204028, "Recieved");

			INSERT INTO lcs.Orders(Discount, Location, CustomerEmail, StaffEmail, orderDateTime, OrderStatus)
			VALUES (0, "Offices", "alicehall@hotmail.com", "dprowse@lcs.com", 1546721898, "Pending");

			INSERT INTO lcs.Orders(Discount, Location, CustomerEmail, StaffEmail, orderDateTime, OrderStatus)
			VALUES (0, "Customer Location", "paulinerichards33@gmail.com", "landerson@lcs.com", 1546721828, "Dispatched");

			INSERT INTO lcs.Orders(Discount, Location, CustomerEmail, StaffEmail, orderDateTime, OrderStatus)
			VALUES (0, "Customer Location", "adamelse@gmail.com", "cshannon@lcs.com", 1546720828, "Confirmed");


		/*Insert Service for orders*/
			INSERT INTO lcs.ServicesForOrder(ServiceID, CustomerEmail, orderDateTime)
			VALUES ("Replace Keyboard on Laptop", "alicehall@hotmail.com", 1546721898);

			INSERT INTO lcs.ServicesForOrder(ServiceID, CustomerEmail, orderDateTime)
			VALUES ("Repair Screen on Laptop", "alicehall@hotmail.com", 1546721898);

			INSERT INTO lcs.ServicesForOrder(ServiceID, CustomerEmail, orderDateTime)
			VALUES ("Replace Hard Drive", "janecodd@hotmail.com", 1384204028);

			INSERT INTO lcs.ServicesForOrder(ServiceID, CustomerEmail, orderDateTime)
			VALUES ("Add 8GB RAM", "paulinerichards33@gmail.com", 1546721828);

			INSERT INTO lcs.ServicesForOrder(ServiceID, CustomerEmail, orderDateTime)
			VALUES ("Add 8GB RAM", "adamelse@gmail.com", 1546720828);

			INSERT INTO lcs.ServicesForOrder(ServiceID, CustomerEmail, orderDateTime)
			VALUES ("Replace Hard Drive", "adamelse@gmail.com", 1546720828);


		/*Insert invoice*/
			INSERT INTO lcs.invoice(DateTimeSent, CustomerEmail, TotalPrice)
			VALUES (1384204028, "janecodd@hotmail.com", 100.00);

			INSERT INTO lcs.invoice(DateTimeSent, CustomerEmail, TotalPrice)
			VALUES (1546721898, "alicehall@hotmail.com", 120.00);

			INSERT INTO lcs.invoice(DateTimeSent, CustomerEmail, TotalPrice)
			VALUES (1546721828, "paulinerichards33@gmail.com", 120.00);

			INSERT INTO lcs.invoice(DateTimeSent, CustomerEmail, TotalPrice)
			VALUES (1546720828, "adamelse@gmail.com", 220.00);

			
		/*Insert stock order*/
			INSERT INTO lcs.StockOrder(DateTimeSent, DateTimeRecieved, SupplierName, StockItem, OrderCost)
			VALUES (1384204028, 1403467856, "Sibelius' Hard Drives", "1TB HD", 50.0);

			INSERT INTO lcs.StockOrder(DateTimeSent, SupplierName, StockItem, OrderCost)
			VALUES (1534204028, "Sibelius' Hard Drives", "1TB HD", 45.0);

			
		/*Insert StockForService*/
			
			INSERT INTO lcs.StockForService(StockItemName, ServiceName, Quantity)
			VALUES ("Laptop Keyboard", "Replace Keyboard on Laptop", 1);

			INSERT INTO lcs.StockForService(StockItemName, ServiceName, Quantity)
			VALUES ("ZIF Connector", "Replace Keyboard on Laptop", 1);

			INSERT INTO lcs.StockForService(StockItemName, ServiceName, Quantity)
			VALUES ("Ribbon Cable", "Replace Keyboard on Laptop", 1);

			INSERT INTO lcs.StockForService(StockItemName, ServiceName, Quantity)
			VALUES ("Laptop Screen", "Repair Screen on Laptop", 1);

			INSERT INTO lcs.StockForService(StockItemName, ServiceName, Quantity)
			VALUES ("4GB DDR4 RAM", "Add 8GB RAM", 2);

			INSERT INTO lcs.StockForService(StockItemName, ServiceName, Quantity)
			VALUES ("1TB HD", "Replace Hard Drive", 1);
			


	/*Delete statements*/
		/*Example of deleting a user*/
		DELETE FROM lcs.customer WHERE Email = "ericsmith@hotmail.com";

		/*Example of clearing all completed orders from DB*/
		DELETE FROM lcs.ServicesForOrder WHERE (customerEmail = (SELECT CustomerEmail FROM lcs.orders WHERE OrderStatus = "Recieved")) AND (OrderDateTime = ((SELECT OrderDateTime FROM lcs.orders WHERE OrderStatus = "Recieved")));
		DELETE FROM lcs.orders WHERE OrderStatus = "Recieved";

		/*Example of clearing all invoices from DB from before Jan 1st 2013*/
		DELETE FROM lcs.invoice WHERE DateTimeSent <= 1356998400;

	/*Update statements*/	
		/*Change customer email address*/
		UPDATE lcs.customer SET Email = "janebloggs23@hotmail.com" WHERE Email="janebloggs@gmail.com";
			
		/*Change staff members address*/
		UPDATE lcs.staff SET Street = "King Street", HouseNum = 3, City = "Grantham", Postcode="LN20 1KX" WHERE Email="sbiggles@lcs.com";
		
		
	
		
	/*Create a copy of all tables in DB  */
		CREATE TABLE lcs.copy_of_customer LIKE lcs.customer; 
		INSERT lcs.copy_of_customer SELECT * FROM lcs.customer;

		CREATE TABLE lcs.copy_of_staff LIKE lcs.staff; 
		INSERT lcs.copy_of_staff SELECT * FROM lcs.staff;

		CREATE TABLE lcs.copy_of_supplier LIKE lcs.supplier; 
		INSERT lcs.copy_of_supplier SELECT * FROM lcs.supplier;

		CREATE TABLE lcs.copy_of_suppliercontacts LIKE lcs.suppliercontacts; 
		INSERT lcs.copy_of_suppliercontacts SELECT * FROM lcs.suppliercontacts;

		CREATE TABLE lcs.copy_of_stock LIKE lcs.stock; 
		INSERT lcs.copy_of_stock SELECT * FROM lcs.stock;

		CREATE TABLE lcs.copy_of_service LIKE lcs.service; 
		INSERT lcs.copy_of_service SELECT * FROM lcs.service;

		CREATE TABLE lcs.copy_of_StockForService LIKE lcs.StockForService; 
		INSERT lcs.copy_of_StockForService SELECT * FROM lcs.StockForService;

		CREATE TABLE lcs.copy_of_Orders LIKE lcs.Orders; 
		INSERT lcs.copy_of_Orders SELECT * FROM lcs.Orders;

		CREATE TABLE lcs.copy_of_ServicesForOrder LIKE lcs.ServicesForOrder; 
		INSERT lcs.copy_of_ServicesForOrder SELECT * FROM lcs.ServicesForOrder;

		CREATE TABLE lcs.copy_of_Invoice LIKE lcs.Invoice; 
		INSERT lcs.copy_of_Invoice SELECT * FROM lcs.Invoice;

		CREATE TABLE lcs.copy_of_StockOrder LIKE lcs.StockOrder; 
		INSERT lcs.copy_of_StockOrder SELECT * FROM lcs.StockOrder;

		
	/*Create a new user w/ read only permissions*/

		DROP USER IF EXISTS ReadOnlyUser@localhost;
        
		CREATE USER ReadOnlyUser@localhost IDENTIFIED BY "Passw0rd";
		
		GRANT SELECT ON lcs.* TO ReadOnlyUser@localhost;
		
		REVOKE DROP ON lcs.* FROM ReadOnlyUser@localhost;
			
		FLUSH PRIVILEGES;
	
	/*Queries*/
	  
		/*Inner Join - Gets postcode of all orders that must be performed at customer location*/
		SELECT orders.Location, customer.Postcode FROM lcs.orders INNER JOIN lcs.customer ON lcs.orders.CustomerEmail = lcs.customer.Email WHERE orders.Location = "Customer Location";
	  
		/*Left Join - See all customer names with discounts on their orders*/
		SELECT customer.FirstName, customer.LastName, orders.Discount FROM lcs.customer LEFT JOIN lcs.orders ON customer.Email = orders.CustomerEmail ORDER BY customer.LastName;
		
		/*Right Join - See all staff and the orders they are working on*/
		SELECT Orders.OrderDateTime, orders.customerEmail, staff.LastName, staff.FirstName FROM lcs.Orders RIGHT JOIN lcs.staff ON Orders.StaffEmail = staff.Email ORDER BY Orders.OrderDateTime;

		/*UNION to see all the cities inhabitted by staff or customers*/
		SELECT City FROM lcs.Customer UNION SELECT City FROM lcs.staff ORDER BY City;	
       
    /*Stored Procedure for a financial report (income, outgoings and profit)*/
       	DELIMITER $$			
        CREATE PROCEDURE lcs.CurrentFinanceReport()
       	BEGIN	
       		DECLARE  income DECIMAL(6,2) default 0;
        	DECLARE  outgoings DECIMAL(6,2) default 0; 
           	SET income = (SELECT SUM(invoice.TotalPrice) FROM lcs.invoice INNER JOIN lcs.orders ON orders.CustomerEmail = invoice.CustomerEmail WHERE orders.OrderStatus != "Recieved");
       	    SET outgoings =(SELECT SUM(OrderCost) FROM lcs.stockOrder WHERE DateTimeRecieved = 0);
            SELECT (income - outgoings);
       	END $$
       	DELIMITER ;   
		/*Above can be called with 'CALL lcs.CurrentFinanceReport();'   */
	