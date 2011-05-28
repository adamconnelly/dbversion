create table books
(
	bookId      int,
	name        varchar(50),
	authorId    int,
	description varchar(200),
	ISBN        varchar(50)
);

create table authors
(
	authorId	int,
	name		varchar(50),
	dateOfBirth date
);