<!-- Create Database -->
CREATE DATABASE NaturalGasShipmentDataDB;

<!-- Create Table -->
USE NaturalGasShipmentDataDB
CREATE TABLE GasShipments (
	Loc INT NOT NULL,
	LocZn VARCHAR(30) NOT NULL,
	LocName VARCHAR(50) NOT NULL,
	LocPurpDesc NCHAR(2) NOT NULL,
	LocQTI NCHAR(3) NOT NULL,
	FlowInd NCHAR(1) NOT NULL,
	DC INT NOT NULL,
	OPC INT NOT NULL,
	TSQ INT NOT NULL,
	OAC INT NOT NULL,
	IT BIT NOT NULL,
	AuthOverrunInd BIT NOT NULL,
	NomCapExceedInd BIT NOT NULL,
	AllQtyAvail BIT NOT NULL,
	QtyReason VARCHAR(150) NULL,
	Date DATE NOT NULL,
	Cycle VARCHAR(20) NOT NULL,
	PRIMARY KEY (Loc, FlowInd, Date, Cycle)
);

<!-- Drop Table -->
DROP TABLE GasShipments;

<!-- Drop Database -->
DROP DATABASE NaturalGasShipmentDataDB;