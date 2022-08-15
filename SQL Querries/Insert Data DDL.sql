USE [NaturalGasShipmentDataDB]
GO

INSERT INTO [dbo].[GasShipments]
           ([Loc]
           ,[LocZn]
           ,[LocName]
           ,[LocPurpDesc]
           ,[LocQTI]
           ,[FlowInd]
           ,[DC]
           ,[OPC]
           ,[TSQ]
           ,[OAC]
           ,[IT]
           ,[AuthOverrunInd]
           ,[NomCapExceedInd]
           ,[AllQtyAvail]
           ,[QtyReason]
           ,[Date]
           ,[Cycle])
     VALUES
           (<Loc, int,>
           ,<LocZn, varchar(30),>
           ,<LocName, varchar(50),>
           ,<LocPurpDesc, nchar(2),>
           ,<LocQTI, nchar(3),>
           ,<FlowInd, nchar(1),>
           ,<DC, int,>
           ,<OPC, int,>
           ,<TSQ, int,>
           ,<OAC, int,>
           ,<IT, bit,>
           ,<AuthOverrunInd, bit,>
           ,<NomCapExceedInd, bit,>
           ,<AllQtyAvail, bit,>
           ,<QtyReason, varchar(150),>
           ,<Date, date,>
           ,<Cycle, varchar(20),>)
GO


