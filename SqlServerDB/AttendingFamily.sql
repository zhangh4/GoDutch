CREATE TABLE [dbo].[AttendingFamily]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [FamilyId] INT NOT NULL, 
    [EventId] INT NOT NULL, 
    [Expense] MONEY NULL, 
    [Count] FLOAT NULL, 
    CONSTRAINT [FK_AttendingFamily_ToFamily] FOREIGN KEY ([FamilyId]) REFERENCES [Family]([Id]), 
    CONSTRAINT [FK_AttendingFamily_ToEvent] FOREIGN KEY ([EventId]) REFERENCES [Event]([Id]) ON DELETE CASCADE
)
