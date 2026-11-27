-- =====================================================
-- Drop (børnetabeller først). Fjern denne sektion hvis
-- tabellerne ikke må droppes. Sletter data!
-- =====================================================
IF OBJECT_ID('dbo.SaveSlotProgress', 'U') IS NOT NULL DROP TABLE dbo.SaveSlotProgress;
IF OBJECT_ID('dbo.BundleItemSlot_has_Bundle', 'U') IS NOT NULL DROP TABLE dbo.BundleItemSlot_has_Bundle; -- gammel tabel, findes ikke længere i modellen
IF OBJECT_ID('dbo.[Save]', 'U') IS NOT NULL DROP TABLE dbo.[Save];
IF OBJECT_ID('dbo.[User]', 'U') IS NOT NULL DROP TABLE dbo.[User];
IF OBJECT_ID('dbo.BundleItemSlot', 'U') IS NOT NULL DROP TABLE dbo.BundleItemSlot;
IF OBJECT_ID('dbo.Bundle', 'U') IS NOT NULL DROP TABLE dbo.Bundle;
IF OBJECT_ID('dbo.GoldenScroll', 'U') IS NOT NULL DROP TABLE dbo.GoldenScroll;
GO

-- -----------------------------------------------------
-- Table GoldenScroll (én pr. rum)
-- -----------------------------------------------------
CREATE TABLE dbo.GoldenScroll (
  GoldenScrollID INT IDENTITY(1,1) NOT NULL,
  RoomName NVARCHAR(45) NOT NULL,
  CONSTRAINT PK_GoldenScroll PRIMARY KEY (GoldenScrollID),
  CONSTRAINT RoomName_UNIQUE UNIQUE (RoomName)
);
GO

-- -----------------------------------------------------
-- Table Bundle (hører til præcis én GoldenScroll)
-- -----------------------------------------------------
CREATE TABLE dbo.Bundle (
  BundleID INT IDENTITY(1,1) NOT NULL,
  BundleIndex INT NOT NULL,
  InternalName NVARCHAR(100) NOT NULL,
  Name NVARCHAR(45) NOT NULL,
  GoldenScroll_GoldenScrollID INT NOT NULL,
  CONSTRAINT PK_Bundle PRIMARY KEY (BundleID),
  CONSTRAINT fk_Bundle_GoldenScroll1
    FOREIGN KEY (GoldenScroll_GoldenScrollID)
    REFERENCES dbo.GoldenScroll (GoldenScrollID)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);
CREATE INDEX fk_Bundle_GoldenScroll1_idx ON dbo.Bundle (GoldenScroll_GoldenScrollID);
GO

-- -----------------------------------------------------
-- Table BundleItemSlot (ID er tekst, kan ikke være IDENTITY)
-- -----------------------------------------------------
CREATE TABLE dbo.BundleItemSlot (
  BundleItemSlotID NVARCHAR(50) NOT NULL,
  BundleItemSlotIndex INT NOT NULL,
  Bundle_BundleID INT NOT NULL,
  CONSTRAINT PK_BundleItemSlot PRIMARY KEY (BundleItemSlotID),
  CONSTRAINT fk_BundleItemSlot_Bundle1
    FOREIGN KEY (Bundle_BundleID)
    REFERENCES dbo.Bundle (BundleID)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);
CREATE INDEX fk_BundleItemSlot_Bundle1_idx ON dbo.BundleItemSlot (Bundle_BundleID);
GO

-- -----------------------------------------------------
-- Table User
-- -----------------------------------------------------
CREATE TABLE dbo.[User] (
  UserID INT IDENTITY(1,1) NOT NULL,
  UserName NVARCHAR(100) NULL,
  CONSTRAINT PK_User PRIMARY KEY (UserID)
);
GO

-- -----------------------------------------------------
-- Table Save
-- -----------------------------------------------------
CREATE TABLE dbo.[Save] (
  ID INT IDENTITY(1,1) NOT NULL,
  SaveFolderName NVARCHAR(100) NULL,
  User_UserID INT NOT NULL,
  CONSTRAINT PK_Save PRIMARY KEY (ID),
  CONSTRAINT fk_Save_User1
    FOREIGN KEY (User_UserID)
    REFERENCES dbo.[User] (UserID)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);
CREATE INDEX fk_Save_User1_idx ON dbo.[Save] (User_UserID);
-- Filtreret, så flere NULL-værdier er tilladt
CREATE UNIQUE INDEX Name_UNIQUE ON dbo.[Save] (SaveFolderName) WHERE SaveFolderName IS NOT NULL;
GO

-- -----------------------------------------------------
-- Table SaveSlotProgress
-- -----------------------------------------------------
CREATE TABLE dbo.SaveSlotProgress (
  IsFilled BIT NOT NULL CONSTRAINT DF_SaveSlotProgress_IsFilled DEFAULT 0,
  Save_ID INT NOT NULL,
  BundleItemSlot_BundleItemSlotID NVARCHAR(50) NOT NULL,
  CONSTRAINT PK_SaveSlotProgress
    PRIMARY KEY (Save_ID, BundleItemSlot_BundleItemSlotID),
  CONSTRAINT fk_SaveSlotProgress_Save1
    FOREIGN KEY (Save_ID)
    REFERENCES dbo.[Save] (ID)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT fk_SaveSlotProgress_BundleItemSlot1
    FOREIGN KEY (BundleItemSlot_BundleItemSlotID)
    REFERENCES dbo.BundleItemSlot (BundleItemSlotID)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION
);
CREATE INDEX fk_SaveSlotProgress_Save1_idx ON dbo.SaveSlotProgress (Save_ID);
CREATE INDEX fk_SaveSlotProgress_BundleItemSlot1_idx ON dbo.SaveSlotProgress (BundleItemSlot_BundleItemSlotID);
GO