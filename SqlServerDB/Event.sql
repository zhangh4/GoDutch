﻿CREATE TABLE [dbo].[Event]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(MAX) NOT NULL, 
    [LastModifiedDate] DATETIME NOT NULL, 
    [Active] BIT NOT NULL DEFAULT 1
)

GO
