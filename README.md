dbversion
=========

A tool for creating and upgrading databases using SQL scripts.

## Downloads

Download the latest version from https://s3-eu-west-1.amazonaws.com/dbversion/dbversion_0.2.0.0_36.zip.

## What is it?
dbversion is a tool for managing the different versions of a database, and automatically applying changes.

## How does it work?
dbversion works by placing all of the scripts that make up the versions of your database into an [[Documentation:Archives|archive]], along with XML metadata files that describe what scripts need to be executed to create each version. It then compares this against information stored in version tables in the database to figure out what scripts should be run.

## How can I use it?
Typically you define a baseline set of scripts, and then create a new version each time you want to make a change to the database. A baseline is a set of scripts to create a snapshot of your database as it currently is.

To find out how to use dbversion, take a look at the [[Documentation|documentation]] section, or look at the [[Documentation:Quick Start|quick start guide]].
