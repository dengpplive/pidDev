CREATE TABLE [Base_CommandManage]
(
	Cmd_Id INT IDENTITY(1,1)  NOT NULL,
	Cmd_Command VARCHAR(20) NULL,
	Cmd_IsUp	BIT NULL,
	Cmd_IsDown	BIT NULL,
	Cmd_UpCommand	VARCHAR(1000) NULL,
	Cmd_DownCommand VARCHAR(1000) NULL,
	CONSTRAINT Cmd_Id_MainKey PRIMARY KEY
	(
		Cmd_id
	)
)