-- Script Date: 2024-12-12 12:27 PM  - ErikEJ.SqlCeScripting version 3.5.2.95
CREATE TABLE [Item] (
  [itemId] INTEGER NOT NULL
, [title] TEXT NOT NULL
, [category] TEXT NOT NULL
, [publisher] TEXT NOT NULL
, [genre] TEXT NOT NULL
, [location] TEXT NOT NULL
, [status] TEXT DEFAULT ('available') NOT NULL
, [replaceCost] REAL NOT NULL
, [pubDate] TEXT NOT NULL
, CONSTRAINT [PK_Item] PRIMARY KEY ([itemId])
);
