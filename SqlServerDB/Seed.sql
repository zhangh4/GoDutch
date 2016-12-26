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
insert into Family select N'张华颖' except select Name from Family;
insert into Family select 'Cindy' except select Name from Family;
insert into Family select 'Jack' except select Name from Family;
insert into Family select N'赵波' except select Name from Family;
insert into Family select 'Jason' except select Name from Family;
insert into Family select 'Ahlan' except select Name from Family;
insert into Family select 'Justin' except select Name from Family;
insert into Family select N'蒋冰' except select Name from Family;
insert into Family select 'Elaine' except select Name from Family;

insert into Family select N'龙继荣' except select Name from Family;
insert into Family select N'续濛' except select Name from Family;
insert into Family select N'杨耀华' except select Name from Family;
insert into Family select N'胡建勇' except select Name from Family;
insert into Family select N'曹爱泽' except select Name from Family;
insert into Family select N'黎江' except select Name from Family;
insert into Family select N'巩岩' except select Name from Family;
insert into Family select N'蒋丽' except select Name from Family;
insert into Family select N'王琴' except select Name from Family;
insert into Family select N'郭奕' except select Name from Family;
insert into Family select N'张玲' except select Name from Family;
insert into Family select N'李刚' except select Name from Family;
insert into Family select N'赵燕' except select Name from Family;
insert into Family select N'宋宁' except select Name from Family;
insert into Family select N'陈红玲' except select Name from Family;