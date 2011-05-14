create table books
(
	bookId      int,
	name        varchar,
	authorId    int,
	description varchar,
	ISBN        varchar
);

create table authors
(
	authorId	int,
	name		varchar,
	dateOfBirth date
);