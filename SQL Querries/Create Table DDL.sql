USE [NaturalGasShipmentDataDB]
GO

/****** Object:  Table [dbo].[GasShipments]    Script Date: 8/14/2022 1:35:19 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GasShipments](
	[Loc] [int] NOT NULL,
	[LocZn] [varchar](30) NOT NULL,
	[LocName] [varchar](50) NOT NULL,
	[LocPurpDesc] [nchar](2) NOT NULL,
	[LocQTI] [nchar](3) NOT NULL,
	[FlowInd] [nchar](1) NOT NULL,
	[DC] [int] NOT NULL,
	[OPC] [int] NOT NULL,
	[TSQ] [int] NOT NULL,
	[OAC] [int] NOT NULL,
	[IT] [bit] NOT NULL,
	[AuthOverrunInd] [bit] NOT NULL,
	[NomCapExceedInd] [bit] NOT NULL,
	[AllQtyAvail] [bit] NOT NULL,
	[QtyReason] [varchar](150) NULL,
	[Date] [date] NOT NULL,
	[Cycle] [varchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Loc] ASC,
	[FlowInd] ASC,
	[Date] ASC,
	[Cycle] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO


