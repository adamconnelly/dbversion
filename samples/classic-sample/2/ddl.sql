create table EmailAddresses
(
	EmailId		char(36),	
	EmailAddress	varchar(100),
	primary key(EmailId)
);

create table UserEmailAddresses
(
	UserId	char(36),
	EmailId	char(36),
	primary key(UserId, EmailId),
	foreign key (UserId) references Users (UserId),
	foreign key (EmailId) references EmailAddresses (EmailId)
);
