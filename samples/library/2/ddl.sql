create table EmailAddresses
(
	EmailId			uniqueidentifier primary key,
	EmailAddress	varchar(100)
);

create table UserEmailAddresses
(
	UserId	uniqueidentifier,
	EmailId	uniqueidentifier,
	primary key(UserId, EmailId),
	foreign key (UserId) references Users (UserId),
	foreign key (EmailId) references EmailAddresses (EmailId)
);