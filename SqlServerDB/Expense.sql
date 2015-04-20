CREATE TABLE [dbo].[Expense]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [EventId] INT NOT NULL, 
    [Name] NVARCHAR(MAX) NOT NULL, 
    CONSTRAINT [FK_Expense_ToEvent] FOREIGN KEY ([EventId]) REFERENCES [Event](Id) on delete cascade
)
