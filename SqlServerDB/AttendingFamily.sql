CREATE TABLE [dbo].[AttendingFamily]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [FamilyId] INT NOT NULL, 
    [ExpenseId] INT NOT NULL, 
    [Expense] MONEY NULL, 
    [Count] FLOAT NULL, 
    CONSTRAINT [FK_AttendingFamily_ToFamily] FOREIGN KEY ([FamilyId]) REFERENCES [Family]([Id]), 
    CONSTRAINT [FK_AttendingFamily_ToExpense] FOREIGN KEY ([ExpenseId]) REFERENCES [Expense]([Id]) ON DELETE CASCADE
)
