CREATE TABLE Base_ProtocolType
(
[Protocol_Id] int IDENTITY(1,1) NOT NULL,
ProtocolType varchar(20) NULL,
CONSTRAINT [Protocol_Id_MainKey] PRIMARY KEY 
(
	[Protocol_Id]	
)
)