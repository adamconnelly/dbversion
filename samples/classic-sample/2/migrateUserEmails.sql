declare @UserId uniqueidentifier;
declare @Email varchar(100);
declare @Email1 varchar(100);
declare @EmailId uniqueidentifier;

declare UserCursor cursor for
	select UserId, Email, Email1
	from Users;
	
open UserCursor;
fetch next from UserCursor into @UserId, @Email, @Email1;

while (@@FETCH_STATUS = 0)
begin
	if (@Email is not null)
	begin
		set @EmailId = NEWID();
		insert into EmailAddresses (EmailId, EmailAddress)
			values (@EmailId, @Email);
			
		insert into UserEmailAddresses (UserId, EmailId)
			values (@UserId, @EmailId);
	end;
	
	if (@Email1 is not null)
	begin
		set @EmailId = NEWID();
		insert into EmailAddresses (EmailId, EmailAddress)
			values (@EmailId, @Email1);
			
		insert into UserEmailAddresses (UserId, EmailId)
			values (@UserId, @EmailId);
	end

	fetch next from UserCursor into @UserId, @Email, @Email1;
end;

close UserCursor;
deallocate UserCursor;

alter table Users drop column Email;
alter table Users drop column Email1;