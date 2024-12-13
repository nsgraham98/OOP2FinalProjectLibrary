-- Script Date: 2024-12-12 1:52 PM  - ErikEJ.SqlCeScripting version 3.5.2.95
CREATE TABLE [Magazine] (
  [itemId] bigint NOT NULL
, [title] text NOT NULL
, [publication] TEXT NOT NULL
, [issn] text NOT NULL
, [coverDate] text NOT NULL
, CONSTRAINT [sqlite_autoindex_Magazine_1] PRIMARY KEY ([itemId])
, CONSTRAINT [FK_Magazine_0_0] FOREIGN KEY ([itemId]) REFERENCES [Item] ([itemId]) ON DELETE NO ACTION ON UPDATE NO ACTION
);
