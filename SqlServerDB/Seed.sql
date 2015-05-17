/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

insert into Family select 'Alvin' except select Name from Family;
insert into Family select 'Brayden' except select Name from Family;
insert into Family select 'Cindy' except select Name from Family;
insert into Family select 'Debra' except select Name from Family;
insert into Family select 'Devin' except select Name from Family;
insert into Family select 'Jason' except select Name from Family;
insert into Family select 'Joanna' except select Name from Family;
insert into Family select 'Justin' except select Name from Family;
insert into Family select 'Roger' except select Name from Family;
insert into Family select 'Elaine' except select Name from Family;