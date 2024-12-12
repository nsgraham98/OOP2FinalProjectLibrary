CREATE TABLE [Rental] (
  [rentalId] INTEGER NOT NULL
, [memberId] INTEGER NOT NULL
, [startDate] TEXT NOT NULL
, [dueDate] TEXT NOT NULL
, [returnedDate] TEXT NULL
, [rentStatus] TEXT default ('active') NOT NULL
, CONSTRAINT [PK_Rental] PRIMARY KEY ([rentalId])
, FOREIGN KEY (memberId) REFERENCES Member(memberId)
, FOREIGN KEY (rentalId) REFERENCES ItemInRental(rentalId)
);