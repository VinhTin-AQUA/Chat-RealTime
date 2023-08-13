use NotifyBot;

select * from Groups;

select * from MessageChats;
select * from MessageChats
where GroupId = 'd5b57528-82d8-4465-a3a7-c0f422626366';

select FirstName, LastName from Users
join GroupUsers On Users.Id = GroupUsers.UserId
Join Groups On GroupUsers.GroupId = Groups.Id
where Groups.[Name]='UTC2';

delete from MessageChats;
delete from Groups;