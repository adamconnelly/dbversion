create table books
(
	BookId      char(36),
	Name        varchar(50),
	AuthorId    int,
	Description varchar(200),
	ISBN        varchar(50),
	primary key(BookId)
);

create table authors
(
	AuthorId    char(36),
	Name	    varchar(50),
	DateOfBirth date,
	primary key(AuthorId)
);

create table users
(
	UserId	    char(36),
	UserName    varchar(20),
	Name        varchar(100),
	Password    varchar(40),
	DateOfBirth date,
	Email       varchar(100),
	Email1      varchar(100),
        primary key(UserId)
);
