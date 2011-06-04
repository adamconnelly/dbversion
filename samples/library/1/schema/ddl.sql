create table books
(
	BookId      uniqueidentifier primary key,
	Name        varchar(50),
	AuthorId    int,
	Description varchar(200),
	ISBN        varchar(50)
);

create table authors
(
	AuthorId	uniqueidentifier primary key,
	Name		varchar(50),
	DateOfBirth date
);

create table users
(
	UserId		uniqueidentifier primary key,
	UserName	varchar(20),
	Name		varchar(100),
	Password	varchar(40),
	DateOfBirth date,
	Email		varchar(100),
	Email1		varchar(100)
);