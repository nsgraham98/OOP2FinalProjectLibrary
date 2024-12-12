CREATE TABLE [ItemInRental] (
  [itemId] bigint NOT NULL
, [rentalId] bigint NOT NULL
, [itemNum] integer NOT NULL
, [itemStatus] text DEFAULT ('rented') NOT NULL
, CONSTRAINT [sqlite_autoindex_ItemInRental_1] PRIMARY KEY ([itemId],[rentalId], [itemNum])
, CONSTRAINT [FK_ItemInRental_0_0] FOREIGN KEY ([rentalId]) REFERENCES [Rental] ([rentalId]) ON DELETE NO ACTION ON UPDATE NO ACTION
, CONSTRAINT [FK_ItemInRental_1_0] FOREIGN KEY ([itemId]) REFERENCES [Item] ([itemId]) ON DELETE NO ACTION ON UPDATE NO ACTION
);
