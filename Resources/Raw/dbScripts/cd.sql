-- Script Date: 2024-12-12 12:34 PM  - ErikEJ.SqlCeScripting version 3.5.2.95
CREATE TABLE [Cd] (
  [itemId] bigint NOT NULL
, [title] text NOT NULL
, [artist] text NOT NULL
, [label] text NOT NULL
, [duration] text NOT NULL
, CONSTRAINT [sqlite_master_PK_Cd] PRIMARY KEY ([itemId])
, CONSTRAINT [FK_Cd_0_0] FOREIGN KEY ([itemId]) REFERENCES [Item] ([itemId]) ON DELETE NO ACTION ON UPDATE NO ACTION
);
