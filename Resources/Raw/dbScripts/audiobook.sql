-- Script Date: 2024-12-12 12:36 PM  - ErikEJ.SqlCeScripting version 3.5.2.95
CREATE TABLE [Audiobook] (
  [itemId] INTEGER NOT NULL
, [title] TEXT NOT NULL
, [isbn] TEXT NOT NULL
, [author] TEXT NOT NULL
, [duration] TEXT NOT NULL
, [narrator] TEXT NOT NULL
, CONSTRAINT [PK_Audiobook] PRIMARY KEY ([itemId])
, FOREIGN KEY (itemId) REFERENCES Item(itemId)
);
