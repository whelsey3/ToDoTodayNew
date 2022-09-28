BEGIN TRANSACTION;
DROP TABLE IF EXISTS `Tracks`;
CREATE TABLE IF NOT EXISTS `Tracks` (
	`TrackID`	INTEGER,
	`ProjectID`	int,
	`Item`	nvarchar ( 35 ) NOT NULL,
	`StartDate`	datetime NOT NULL,
	`DetailedDesc`	nvarchar ( 2000 ) NOT NULL,
	`ElapsedTime`	decimal,
	`BillTime`	decimal,
	`BillRef`	nvarchar ( 10 ),
	`EndDate`	datetime,
	`Mileage`	decimal,
	`Expenses`	decimal,
	`RespPerson`	nvarchar ( 30 ),
	`Status`	nvarchar ( 1 ) NOT NULL,
	`DateCreated`	datetime NOT NULL,
	`DateModified`	datetime NOT NULL,
	`IsDirty`	bit NOT NULL,
	FOREIGN KEY(`ProjectID`) REFERENCES `Projects`(`ProjectID`),
	PRIMARY KEY(`TrackID`)
);
DROP TABLE IF EXISTS `ToDos`;
CREATE TABLE IF NOT EXISTS `ToDos` (
	`ToDoID`	INTEGER,
	`ProjectID`	int NOT NULL,
	`TDTSortOrder`	nvarchar ( 9 ) NOT NULL,
	`Item`	nvarchar ( 35 ) NOT NULL,
	`DetailedDesc`	nvarchar ( 2000 ) NOT NULL,
	`Priority`	nvarchar ( 1 ) NOT NULL,
	`Status`	nvarchar ( 1 ) NOT NULL,
	`StartDate`	datetime NOT NULL,
	`DueDate`	datetime NOT NULL,
	`RevDueDate`	datetime NOT NULL,
	`DoneDate`	datetime,
	`RespPerson`	nvarchar ( 30 ) NOT NULL,
	`Hide`	bit NOT NULL,
	`DispLevel`	nvarchar ( 1 ) NOT NULL,
	`Done`	bit NOT NULL,
	`ElapsedTime`	decimal NOT NULL,
	`DateCreated`	datetime NOT NULL,
	`DateModified`	datetime NOT NULL,
	`IsDirty`	bit NOT NULL,
	PRIMARY KEY(`ToDoID`),
	FOREIGN KEY(`ProjectID`) REFERENCES `Projects`(`ProjectID`) ON DELETE CASCADE
);
DROP TABLE IF EXISTS `PropertyErrors`;
CREATE TABLE IF NOT EXISTS `PropertyErrors` (
	`PropertyName`	nvarchar ( 128 ) NOT NULL,
	`Error`	nvarchar,
	`Added`	bit NOT NULL,
	`DateCreated`	datetime NOT NULL,
	`DateModified`	datetime NOT NULL,
	`IsDirty`	bit NOT NULL,
	`ToDo_ToDoID`	int,
	`Track_TrackID`	int,
	PRIMARY KEY(`PropertyName`),
	FOREIGN KEY(`Track_TrackID`) REFERENCES `Tracks`(`TrackID`),
	FOREIGN KEY(`ToDo_ToDoID`) REFERENCES `ToDos`(`ToDoID`)
);
DROP TABLE IF EXISTS `Projects`;
CREATE TABLE IF NOT EXISTS `Projects` (
	`ProjectID`	INTEGER,
	`FolderID`	int,
	`PPartNum`	nvarchar ( 9 ),
	`PSortOrder`	nvarchar ( 9 ),
	`Item`	nvarchar ( 35 ),
	`DetailedDesc`	nvarchar ( 2000 ),
	`Priority`	nvarchar ( 1 ),
	`Status`	nvarchar ( 1 ),
	`StartDate`	datetime NOT NULL,
	`DueDate`	datetime NOT NULL,
	`RevDueDate`	datetime NOT NULL,
	`DoneDate`	datetime,
	`RespPerson`	nvarchar ( 30 ),
	`Hide`	bit NOT NULL,
	`DispLevel`	nvarchar ( 1 ),
	`Done`	bit NOT NULL,
	`DateCreated`	datetime NOT NULL,
	`DateModified`	datetime NOT NULL,
	`IsDirty`	bit NOT NULL,
	PRIMARY KEY(`ProjectID`),
	FOREIGN KEY(`FolderID`) REFERENCES `Folders`(`FolderID`)
);
DROP TABLE IF EXISTS `Folders`;
CREATE TABLE IF NOT EXISTS `Folders` (
	`FolderID`	INTEGER,
	`FPartNum`	nvarchar ( 9 ),
	`FSortOrder`	nvarchar ( 9 ),
	`FolderName`	nvarchar ( 35 ),
	`DetailedDesc`	nvarchar ( 2000 ),
	`Hide`	bit,
	`DispLevel`	nvarchar ( 1 ),
	`DateCreated`	datetime NOT NULL,
	`DateModified`	datetime NOT NULL,
	`IsDirty`	bit NOT NULL,
	PRIMARY KEY(`FolderID`)
);
DROP TABLE IF EXISTS `CustomHistories`;
CREATE TABLE IF NOT EXISTS `CustomHistories` (
	`Id`	INTEGER,
	`Hash`	nvarchar,
	`Context`	nvarchar,
	`CreateDate`	datetime NOT NULL,
	PRIMARY KEY(`Id`)
);
DROP INDEX IF EXISTS `IX_Track_ProjectID`;
CREATE INDEX IF NOT EXISTS `IX_Track_ProjectID` ON `Tracks` (
	`ProjectID`
);
DROP INDEX IF EXISTS `IX_ToDos_ProjectID`;
CREATE INDEX IF NOT EXISTS `IX_ToDos_ProjectID` ON `ToDos` (
	`ProjectID`
);
DROP INDEX IF EXISTS `IX_PropertyError_Track_TrackID`;
CREATE INDEX IF NOT EXISTS `IX_PropertyError_Track_TrackID` ON `PropertyErrors` (
	`Track_TrackID`
);
DROP INDEX IF EXISTS `IX_PropertyError_ToDo_ToDoID`;
CREATE INDEX IF NOT EXISTS `IX_PropertyError_ToDo_ToDoID` ON `PropertyErrors` (
	`ToDo_ToDoID`
);
DROP INDEX IF EXISTS `IX_Project_FolderID`;
CREATE INDEX IF NOT EXISTS `IX_Project_FolderID` ON `Projects` (
	`FolderID`
);
COMMIT;
