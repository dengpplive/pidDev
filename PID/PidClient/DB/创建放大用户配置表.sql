CREATE TABLE Base_UserConfigManage
(
[User_Id] int IDENTITY(1,1) NOT NULL,
Original_Id INT NULL,
[User_Name] Varchar(50) NULL,
User_Pass Varchar(50) NULL,
User_DisableCmd Varchar(500) NULL,
User_Disable BIT NULL,
User_BeginDate DATETIME NULL,
User_EndDate DATETIME NULL,
User_SendCount INT NULL,
User_Description Varchar(500) NULL,
CONSTRAINT [User_Id_MainKey] PRIMARY KEY 
(
	[User_Id]	
)
)