CREATE TABLE Pid.Customers
(
CustomerID INT	Identity(1,1) NOT NULL,
CustomerLoginName	Varchar(20) NULL,
CustomerLoginPass	Varchar(20) NULL,
CustomerName	Varchar(20) NULL,
CustomerAddress	Varchar(50) NULL,
CustomerPhone	Varchar(50) NULL,
CustomerBeginDate	DATETIME NULL,
CustomerEndDate	DATETIME NULL,
CustomerPrice	INT NULL,
CustomerDescription	Varchar(500) NULL,
CONSTRAINT MainKey_CustomerID PRIMARY KEY(CustomerID)
)
