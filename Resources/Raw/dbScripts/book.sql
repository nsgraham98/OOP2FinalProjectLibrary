-- Script Date: 2024-12-12 1:50 PM  - ErikEJ.SqlCeScripting version 3.5.2.95
CREATE TABLE [Book] (
  [itemId] bigint NOT NULL
, [title] TEXT NOT NULL
, [isbn] text NOT NULL
, [author] text NOT NULL
, [format] text NOT NULL
, CONSTRAINT [sqlite_master_PK_Book] PRIMARY KEY ([itemId])
, CONSTRAINT [FK_Book_0_0] FOREIGN KEY ([itemId]) REFERENCES [Item] ([itemId]) ON DELETE NO ACTION ON UPDATE NO ACTION
);
