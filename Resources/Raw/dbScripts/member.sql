-- Script Date: 2024-12-12 12:07 PM  - ErikEJ.SqlCeScripting version 3.5.2.95
CREATE TABLE [Member] (
  [memberId] INTEGER NOT NULL
, [lastName] TEXT NOT NULL
, [firstName] TEXT NOT NULL
, [phone] TEXT NULL
, [email] TEXT NOT NULL
, [address] TEXT NOT NULL
, CONSTRAINT [PK_Member] PRIMARY KEY ([memberId])
);

