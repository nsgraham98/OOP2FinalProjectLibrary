-- Script Date: 2024-12-12 1:52 PM  - ErikEJ.SqlCeScripting version 3.5.2.95
CREATE TABLE [Dvd] (
  [itemId] bigint NOT NULL
, [title] TEXT NOT NULL
, [director] text NOT NULL
, [duration] text NOT NULL
, [format] text NOT NULL
, CONSTRAINT [sqlite_master_PK_Dvd] PRIMARY KEY ([itemId])
, CONSTRAINT [FK_Dvd_0_0] FOREIGN KEY ([itemId]) REFERENCES [Item] ([itemId]) ON DELETE NO ACTION ON UPDATE NO ACTION
);
