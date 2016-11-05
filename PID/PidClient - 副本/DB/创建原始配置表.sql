CREATE TABLE Base_OriginalConfigManage
(
Original_Id INT IDENTITY(1,1) NOT NULL,
Original_Ip varchar(20) NULL,
Original_Port SMALLINT NULL,
Original_ConfigName Varchar(20) NULL,
Original_ConfigPass Varchar(50) NULL,
Original_ConfigSi Varchar(50) NULL,
ProtocolType varchar(20) NULL,
Original_CheckingType Varchar(20) NULL,
Original_OfficeNumber Varchar(20) NULL,
Original_CorporationName Varchar(100) NULL,
Original_Description Varchar(1000) NULL,
CONSTRAINT  Original_Id_MainKey PRIMARY KEY  
(
	Original_Id
)
)