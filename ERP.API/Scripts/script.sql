---USAMA test 2-----------------------------------------------------------------Functions-----------------------------------------------------------------------
IF EXISTS (SELECT * FROM   sys.objects WHERE  object_id = Object_id(N'Split') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].[Split]
GO
CREATE FUNCTION [dbo].[Split](@String NVARCHAR(4000), @Delimiter CHAR(1))       
RETURNS @temptable TABLE (ID int,items NVARCHAR(4000))       
AS       
BEGIN
    DECLARE @idx INT
	DECLARE @_index int
    DECLARE @slice NVARCHAR(4000)       
	SET @_index = 1      
    SELECT @idx = 1       
        IF LEN(@String)<1 or @String IS NULL  RETURN       
    WHILE @idx!= 0       
    BEGIN       
		
        SET @idx = CHARINDEX(@Delimiter,@String)       
        IF @idx!=0       
            SET @slice = LEFT(@String,@idx - 1)       
        ELSE       
            SET @slice = @String       
          
        IF(LEN(@slice)>0)  
            INSERT INTO @temptable(ID ,Items) VALUES(@_index,@slice)       
		SET @_index = @_index + 1
        SET @String = RIGHT(@String,LEN(@String) - @idx)       
        IF LEN(@String) = 0 break       
		
    END   
RETURN  
END    
Go
IF EXISTS (SELECT * FROM   sys.objects WHERE  object_id = Object_id(N'fnGetUnitConversionValue') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].[fnGetUnitConversionValue]
GO
Create FUNCTION [dbo].[fnGetUnitConversionValue] (@input VARCHAR(500),@value decimal(18,4))
RETURNS decimal(18,4)
AS 
BEGIN
   RETURN (
				CASE WHEN @input = 'BarToKPascal' THEN (@value * 100)
                
					 WHEN @input = 'KPascalToBar' THEN (@value / 100)
					
					 WHEN @input = 'BarToPSI' THEN (@value * 14.5038)

					 WHEN @input = 'PSIToBar' THEN (@value / 14.5038)
					
					 WHEN @input = 'BarToATM' THEN (@value / 1.013)
					
					 WHEN @input = 'ATMToBar' THEN (@value * 1.013)
					
					 WHEN @input = 'BarToTorr' THEN (@value * 750.062)
					
					 WHEN @input = 'TorrToBar' THEN (@value / 750.062)
					
					 WHEN @input = 'KPascalToPSI' THEN (@value / 6.89476)
					
					 WHEN @input = 'PSIToKPascal' THEN (@value * 6.89476)
					
					 WHEN @input = 'KPascalToATM' THEN (@value / 101.325)
					
					 WHEN @input = 'ATMToKPascal' THEN (@value * 101.325)
					
					 WHEN @input = 'KPascalToTorr' THEN (@value * 7.501)
					
					 WHEN @input = 'TorrToKPascal' THEN (@value / 7.501)
					
					 WHEN @input = 'PSIToATM' THEN (@value / 14.696)
					
					 WHEN @input = 'ATMToPSI' THEN (@value * 14.696)
					
					 WHEN @input = 'PSIToTorr' THEN (@value * 51.7149)
					
					 WHEN @input = 'TorrToPSI' THEN (@value / 51.7149)
					
					 WHEN @input = 'ATMToTorr' THEN (@value * 760)
					
					 WHEN @input = 'TorrToATM' THEN (@value / 760)

					 WHEN @input ='FtoC' THEN  (@value - 32) * 5 / 9

					 WHEN @input ='CtoF' THEN  (@value * 9 / 5) + 32

					 WHEN @input ='KtoF' THEN  ((@value - 273.15) * 9/5)+32

					 WHEn @input ='FtoK' THEN (@value - 32) * 5 / 9 +(273.15)

					 WHEN @input = 'CtoK' Then (@value + 273.15)

					 WHEN @input = 'KtoC' Then (@value - 273.15)
					 
					 When @input = 'PSItoPSIG' Then (@value - 14.696)

					 When @input = 'PSIGtoPSI' Then (@value + 14.696)

					 When @input = 'PoundsPerHourtoKilogramsPerSecond' Then (@value * 0.0001259)

					 When @input = 'KilogramsPerSecondtoPoundsPerHour' Then (@value / 0.0001259)

					 When @input = 'PoundsPerHourtoSlugsPerHour' Then (@value / 32.174)

					 When @input = 'SlugsPerHourtoPoundsPerHour' Then (@value * 32.174)

					 When @input = 'KilogramsPerSecondtoSlugsPerHour' Then (@value * 246.678)

					 When @input = 'SlugsPerHourtoKilogramsPerSecond' Then (@value / 246.678)

					
					 ELSE @value
					 END
	  )
END
GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  object_id = Object_id(N'fnGetSensorTotalDataPackets') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].[fnGetSensorTotalDataPackets]
GO
CREATE FUNCTION [dbo].[fnGetSensorTotalDataPackets] (@SensorId VARCHAR(500))  
RETURNS int  
AS   
BEGIN
	Declare @TotalDataPackets int = 0  
		Select @TotalDataPackets += Count(1) From Sensor where SensorId = @SensorId
		Select @TotalDataPackets += Count(1) From GatewayPortsSensors where SensorId = @SensorId
		Select @TotalDataPackets += Count(1) From DaysOff where SensorId = @SensorId
		Select @TotalDataPackets += Count(1) From GroupSensors where SensorId = @SensorId
		Select @TotalDataPackets += Count(1) From SensorFiles where SensorId = @SensorId
		Select @TotalDataPackets += Count(1) From SensorHistory where SensorId = @SensorId
		Select @TotalDataPackets += Count(1) From SensorHistoryLogs where SensorId = @SensorId
		Select @TotalDataPackets += Count(1) From TechnicianSensorsResolvedHistory where SensorId = @SensorId
	RETURN @TotalDataPackets
END  
Go
--------------------------------------------------------------------Stored Procedures-----------------------------------------------------------------------

IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetNotAssignedGatewayPortsCombo')
	DROP PROCEDURE [dbo].[spGetNotAssignedGatewayPortsCombo]
GO
CREATE PROCEDURE [spGetNotAssignedGatewayPortsCombo]
@GatewayId varchar(50),
@SensorId varchar(150) = NULL
AS
BEGIN
	SELECT GatewayPortID, GatewayID, PortName from GatewayPorts 
	WHERE GatewayId = @GatewayId 
	AND GatewayPortID NOT in (Select GatewayPortID from GatewayPortsSensors WHERE (@SensorId IS NULL OR SensorId <> @SensorId)) 
END
Go
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetOperators')
	DROP PROCEDURE [dbo].[spGetOperators]
GO
CREATE Procedure spGetOperators
@CompanyId int ,
@Search nvarchar(MAX)
AS
BEGIN
SELECT   Id, Email, FirstName, LastName, CreatedDate, CompanyId, NULL as IndustryId, Title, ProfileBlobURl
FROM    AspNetUsers where (@Search is null OR FirstName like '%'+@Search+'%' OR LastName like '%'+@Search+'%' OR Email like '%'+@Search+'%') 
AND    CompanyId = @CompanyId AND Id in (Select UserId from AspNetUserRoles where RoleId in (Select Id from AspNetRoles where Name = 'Operator'))
END
Go
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetTechnicians')
	DROP PROCEDURE [dbo].[spGetTechnicians]
GO
CREATE Procedure spGetTechnicians
@CompanyId int ,
@Search nvarchar(MAX)
AS
BEGIN
SELECT   Id, Email, FirstName, LastName, CreatedDate, CompanyId,NULL as IndustryId, Title, ProfileBlobURl
FROM    AspNetUsers where (@Search is null OR FirstName like '%'+@Search+'%' OR LastName like '%'+@Search+'%' OR Email like '%'+@Search+'%') 
AND    CompanyId = @CompanyId AND Id in (Select UserId from AspNetUserRoles where RoleId in (Select Id from AspNetRoles where Name = 'Technician')) AND Id NOT IN(SELECT UserId FROM MachineTechnicians)
END
Go
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetMachineAlarms')
	DROP PROCEDURE [dbo].[spGetMachineAlarms]
GO
Create PROCEDURE spGetMachineAlarms    
@MachineId int    
AS     
BEGIN    
  Select SH.SensorHistoryId,   SH.[AVG],   
   SH.SensorID, SH.Voltage,  STU.Alias as SensorTypeUnitName,   
  CASE WHEN STUC.FuncName = STU.FuncName THEN SH.VALUE ELSE (dbo.fnGetUnitConversionValue(CONCAT(STUC.FuncName,'To',STU.FuncName), SH.Value)) END as LiveValue    
  ,S.SensorName,S.FrequencyNumber,S.FrequencyNumberUnit,ST.SensorTypeName,ST.SensorTypeBlobUrl, SS.SensorStatusName,    
  SH.SensorStatusID, SH.DateTime, SH.TimeElapsed,    
  CASE WHEN S.DataTypeID = 1 THEN NULL ELSE (CASE WHEN STUC.FuncName = STU.FuncName THEN SH.DayMin ELSE (dbo.fnGetUnitConversionValue(CONCAT(STUC.FuncName,'To',STU.FuncName), SH.DayMin)) END)  END AS DayMin,       
  CASE WHEN S.DataTypeID = 1 THEN NULL ELSE (CASE WHEN STUC.FuncName = STU.FuncName THEN SH.DayMax ELSE (dbo.fnGetUnitConversionValue(CONCAT(STUC.FuncName,'To',STU.FuncName), SH.DayMax)) END) END AS  DayMax       
  from (    
  Select Row_Number() over(Partition by SH.SensorId order by  SH.[Datetime] Desc) RN,SH.[AVG], SH.SensorHistoryId, SH.SensorID, SH.Voltage, SH.Value,     
  SH.SensorStatusID, SH.DateTime, SH.TimeElapsed,MIN(SH.[MIN]) as DayMin ,Max(SH.[Max]) as DayMax    
  from SensorHistory SH     
  Group by SH.SensorHistoryId, SH.SensorID, SH.Voltage, SH.Value, SH.SensorStatusID, SH.DateTime, SH.TimeElapsed,SH.[AVG]    
 ) SH    
  JOIN Sensor S on S.SensorID = SH.SensorID  AND S.IsActive = 1  
  JOIN SensorType ST on ST.SensorTypeId = S.SensorTypeId    
  JOIN SensorStatus SS on SS.SensorStatusId = SH.SensorStatusId    
  LEFT JOIN   SensorTypeUnits STU on STU.SensorTypeUnitId = S.SensorTypeUnitId    
  LEFT JOIN   SensorTypeUnits STUC on STUC.SensorTypeUnitId = S.ComingUnitId    
  WHERE (SH.SensorStatusId = 1 OR SH.SensorStatusId = 2)    
  AND SH.RN = 1 AND S.MachineID = @MachineId    
END   
Go
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetMachines')
	DROP PROCEDURE [dbo].[spGetMachines]
GO
CREATE Procedure [dbo].[spGetMachines]  
@CompanyId int,  
@Search nvarchar(MAX) = null  
AS  
BEGIN  
 SELECT          Row_Number() over (order by (SELECT 1))RowNumber, M.MachineID,MS.StateName as MachineStatusName,ISNULL(S.TotalSensors,0) as TotalSensors,ISNULL(SH.TotalActiveAlarms,0) TotalActiveAlarms,
				 U.Id as UserId,Concat(U.FirstName,' ',U.LastName) FullName,M.[Description], U.ProfileBlobUrl, ISNULL(MT.TotalTechnicians,0) TotalTechnicians,ISNULL(MO.TotalOperators,0) TotalOperators, M.MachineName,  M.CompanyId, M.MachineBlobURl  
 FROM            Machine M  
 Left Join  (  
     Select Count(1) TotalSensors,MachineId From Sensor Where IsActive = 1 Group By MachineId  
     )S on S.MachineId = M.MachineId  
 Left Join  (  
     Select Count(1) TotalOperators,MachineId From MachineOperators Group By MachineId  
     )MO on MO.MachineId = M.MachineId  
 Left Join  (  
     Select Count(1) TotalTechnicians,MachineId From MachineTechnicians Group By MachineId  
     )MT on MT.MachineId = M.MachineId  
Left Join  (  
    Select Count(1) TotalActiveAlarms,S.MachineID from (
		Select Row_Number() over(Partition by SS.SensorId order by  SS.[Datetime] Desc) RN,SS.[AVG], SS.SensorHistoryId, SS.SensorID, SS.Voltage, SS.Value, 
		SS.SensorStatusID, SS.DateTime, SS.TimeElapsed,MIN(SS.MIN) as DayMin ,Max(SS.Max) as DayMax
		from SensorHistory SS 
		Group by SS.SensorHistoryId, SS.SensorID, SS.Voltage, SS.Value, SS.SensorStatusID, SS.DateTime, SS.TimeElapsed,SS.[AVG]
	) SH
		JOIN Sensor S on S.SensorID = SH.SensorID  AND S.IsActive = 1  
		WHERE (SH.SensorStatusId = 1 OR SH.SensorStatusId = 2)
		AND SH.RN = 1 
		Group By S.MachineID
     ) SH on SH.MachineId = M.MachineId
LEFT JOIN MachineStates MS on MS.StateID = M.MachineStateId
LEFT JOIN MachineTechnicians MTU on MTU.MachineId = M.MachineId
LEFT JOIN AspNetUsers U on U.Id = MTU.UserId
Where   M.CompanyId = @CompanyId AND (@Search IS NULL OR M.MachineID like '%'+@Search+'%' OR M.MachineName like '%'+@Search+'%')  
END 
 

Go
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetAssetAssignedTechnicians')
	DROP PROCEDURE [dbo].[spGetAssetAssignedTechnicians]
GO
CREATE Procedure spGetAssetAssignedTechnicians
@MachineId int 
AS
BEGIN
SELECT   Id, Email, FirstName, LastName, CreatedDate, CompanyId, NULL AS IndustryId, Title, ProfileBlobUrl
FROM    AspNetUsers where Id in (Select UserId from MachineTechnicians where MachineId = @MachineId)
END
Go
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetAssetAssignedOperators')
	DROP PROCEDURE [dbo].[spGetAssetAssignedOperators]
GO
CREATE Procedure spGetAssetAssignedOperators
@MachineId int 
AS
BEGIN
SELECT   Id, Email, FirstName, LastName, CreatedDate, CompanyId, NULL AS IndustryId, Title, ProfileBlobUrl
FROM    AspNetUsers where Id in (Select UserId from MachineOperators where MachineId = @MachineId)
END
Go
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetMachineSensors')
	DROP PROCEDURE [dbo].[spGetMachineSensors]
GO
CREATE Procedure spGetMachineSensors
@MachineId int 
AS
BEGIN
SELECT        S.SensorID, ST.SensorTypeName,ST.SensorTypeBlobUrl, S.SensorName, S.MachineID, S.SensorTypeID, S.FrequencyNumber, S.CriticalMin, S.CriticalMax, S.WarningMin, S.WarningMax, S.DataTypeID, S.CustomEquation, GP.GatewayID, CAST(GPS.GatewayPortID AS INT) as PortNumber, S.SleepStart, S.SleepEnd, S.DigitalAlarm,
                         S.DateCreated, S.DigitalLowMin, S.DigitalLowMax, S.DigitalHighMin, S.DigitalHighMax, S.DateModified, S.LastModifiedBy, S.CreatedBy, S.SensorTemplateID, S.VoltageCriticalMin, S.VoltageCriticalMax, S.VoltageWarningMin, S.VoltageWarningMax,
                         S.CriticalityTypeId, S.ComingUnitId, S.Description, S.SensorTypeUnitId, S.CompanyId, S.SensorBlobURl
FROM            Sensor S
JOIN			SensorType ST on ST.SensorTypeId = S.SensorTypeId
LEFT JOIN GatewayPortsSensors as GPS ON S.SensorID = GPS.SensorID
LEFT JOIN GatewayPorts as GP ON GPS.GatewayPortID = GP.GatewayPortID
Where S.MachineID = @MachineId AND S.IsActive = 1
END
Go
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetSensorById')
	DROP PROCEDURE [dbo].[spGetSensorById]
GO
CREATE Procedure [dbo].[spGetSensorById]   
@SensorId varchar(MAX)  ,
@UserId uniqueidentifier 
As  
BEGIN  
SELECT        Sen.SensorID,ST.SensorTypeName,ST.SensorTypeBlobUrl, Sen.SensorName,Sen.FrequencyNumberUnit,M.MachineName,D.DataTypeName, Sen.MachineID, Sen.SensorTypeID, 
Sen.FrequencyNumber, Sen.CriticalMin, Sen.CriticalMax, Sen.WarningMin, Sen.WarningMax,Sen.DataTypeID, Sen.CustomEquation,  
GP.GatewayID, CAST(GPS.GatewayPortID AS INT) as PortNumber, Sen.SleepStart, Sen.SleepEnd, Sen.DigitalAlarm,   
Sen.DateCreated,Sen.IsActive, Sen.DigitalLowMin, Sen.DigitalLowMax, Sen.DigitalHighMin, Sen.DigitalHighMax, Sen.DateModified, Sen.LastModifiedBy, Sen.SensorTemplateID, 
Sen.VoltageCriticalMin, Sen.VoltageCriticalMax, Sen.VoltageWarningMin, Sen.VoltageWarningMax,   Sen.CriticalityTypeId, Sen.ComingUnitId, Sen.Description, Sen.SensorTypeUnitId, 
Sen.CompanyId, Sen.SensorBlobURl, Sen.CreatedBy,STU.Alias as SensorTypeUnitName,Sen.MachineLocationId,  GP.GatewayPortID As PortId, 
Trim(Cast(replace(GP.PortName,'Port','') as varchar(100))) as PortName,(Select  dbo.fnGetSensorTotalDataPackets(@SensorId)) as TotalDataPackets  ,
Sen.NotificationTemplateId AS GlobalNotificationTemplateId, N.NotificationTemplateName AS GlobalNotificationTemplateName, 
SAT.NotificationTemplateId AS UserNotificationTemplateId,NT.NotificationTemplateName AS UserNotificationTemplateName,
Sen.CriticalMinState,Sen.CriticalMaxState,Sen.WarningMinState,Sen.WarningMaxState,
Sen.BinaryStateId, Sen.HighActivation, Sen.OffStateId,Sen.OnStateId, Sen.ReverseEquation, Sen.SensorHardwareTypeId , Sen.IsSensyrtechProvidedSensor  
FROM        Sensor Sen  
Left Join   Machine M on M.MachineID = Sen.MachineID  
Left Join   DataType D on D.DataTypeId = Sen.DataTypeId   
Left Join   SensorType ST on ST.SensorTypeId = Sen.SensorTypeId  
Left Join   SensorTypeUnits STU on STU.SensorTypeUnitId = Sen.SensorTypeUnitId  
Left Join   GatewayPortsSensors GPS on GPS.SensorId = Sen.SensorID  
Left Join   GatewayPorts GP on GP.GatewayPortID = GPS.GatewayPortID  
Left Join   NotificationTemplates N on Sen.NotificationTemplateId = N.NotificationTemplateId
Left join   SensorAssociatedTemplate SAT on Sen.SensorID = SAT.SensorId AND SAT.UserId =  @UserId
Left Join   NotificationTemplates NT on SAT.NotificationTemplateId = NT.NotificationTemplateId
Where Sen.SensorID = @SensorId 
END 
GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetGroupsCombo')
	DROP PROCEDURE [dbo].[spGetGroupsCombo]
GO
Create PROCEDURE [dbo].[spGetGroupsCombo]
@CompanyId int
AS
BEGIN
SELECT			GroupId,GroupName from (
					SELECT          MG.GroupId,MGS.TotalSensors , MG.GroupName
					FROM            Groups MG
					Left Join		(Select Count(1) TotalSensors, GroupId From GroupSensors WHERE SensorId in (Select SensorId from Sensor where IsActive = 1)
					GROUP BY GroupId) MGS on MGS.GroupId = MG.GroupId
					Where MG.CompanyId = @CompanyId
)S Where		ISNULL(S.TotalSensors,0) < 10 
END
Go
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetSensorsByGroupId')
	DROP PROCEDURE [dbo].[spGetSensorsByGroupId]
GO
CREATE PROCEDURE spGetSensorsByGroupId 
@GroupId int  ,
@SelectedSensorTypeUnitId int 
AS  
BEGIN  
Select S.SensorID,S.Voltage,   
CASE WHEN STUC.FuncName = STU.FuncName THEN S.VALUE ELSE (dbo.fnGetUnitConversionValue(CONCAT(STUC.FuncName,'To',STU.FuncName), S.Value)) END  as LiveValue,
STU.Alias  as SensorTypeUnitName,
S.SensorStatusID,SS.SensorStatusName,S.TimeElapsed,  
CASE WHEN Sen.DataTypeID = 1 THEN NULL ELSE (CASE WHEN STUC.FuncName = STU.FuncName THEN S.DayMin ELSE (dbo.fnGetUnitConversionValue(CONCAT(STUC.FuncName,'To',STU.FuncName), S.DayMin)) END)  END AS DayMin,     
CASE WHEN Sen.DataTypeID = 1 THEN NULL ELSE (CASE WHEN STUC.FuncName = STU.FuncName THEN S.DayMax ELSE (dbo.fnGetUnitConversionValue(CONCAT(STUC.FuncName,'To',STU.FuncName), S.DayMax)) END) END AS  DayMax,     
Sen.SensorBlobUrl,Sen.SensorName, ST.SensorTypeName,ST.SensorTypeBlobUrl, M.MachineName, S.[Datetime]  from (    
Select Row_Number() over(Partition by SS.SensorId order by  SS.[Datetime] Desc) RN,SS.[AVG], SS.SensorHistoryId, SS.SensorID, SS.Voltage,   
SS.Value, SS.SensorStatusID, SS.DateTime, SS.TimeElapsed,MIN(SH.MIN) as DayMin ,Max(SH.Max) as DayMax   
from SensorHistory SS   
Join SensorHistory SH on SH.SensorId = SS.SensorID AND SH.SensorStatusID <> 4   
WHERE SS.SensorStatusID <> 4   
Group by SS.SensorHistoryId, SS.SensorID, SS.Voltage, SS.Value, SS.SensorStatusID, SS.DateTime, SS.TimeElapsed,SS.[AVG]  
) S    
Join  SensorStatus SS on S.SensorStatusID = SS.SensorStatusID   
Join  Sensor Sen on Sen.SensorID = S.SensorID AND Sen.IsActive = 1  
LEFT JOIN   SensorTypeUnits STU on  STU.SensorTypeUnitId = (CASE WHEN @SelectedSensorTypeUnitId = 0 THEN  Sen.SensorTypeUnitId ELSE @SelectedSensorTypeUnitId END  )
LEFT JOIN   SensorTypeUnits STUC on STUC.SensorTypeUnitId = Sen.ComingUnitId  
Join SensorType ST on ST.SensorTypeID = Sen.SensorTypeID   
Join Machine M on Sen.MachineID = M.MachineID  where S.RN = 1  AND Sen.SensorId in (Select SensorId from GroupSensors where GroupId = @GroupId)   
GROUP BY S.SensorID, S.Voltage, S.Value, S.SensorStatusID, SS.SensorStatusName, Sen.SensorName,ST.SensorTypeBlobUrl, ST.SensorTypeName, M.MachineName, S.[Datetime],  
Sen.DataTypeID, S.TimeElapsed,S.DayMin,S.DayMax,Sen.SensorBlobUrl,S.[AVG],STU.FuncName,STUC.FuncName,  STU.Alias  
END  
GO 
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetSensorGroups')
	DROP PROCEDURE [dbo].spGetSensorGroups
GO
CREATE PROCEDURE spGetSensorGroups
@BranchId int,
@CompanyId int,
@SensorTypeIds varchar(MAX) = '',    
@CriticalityTypeIds varchar(MAX) = '',    
@MachineIds varchar(MAX) = '',    
@Search varchar(50) = NULL    
AS
BEGIN
Select MG.GroupId, MG.GroupName,ISNULL(MGS.TotalGroupSensors,0) TotalGroupSensors  
from Groups MG  
Left Join(  
	 Select Count(1) TotalGroupSensors, MGS.GroupId FROM GroupSensors MGS  
	 JOIN SENSOR S on S.SensorID = MGS.SensorId AND S.IsActive = 1  
	 WHERE  (@SensorTypeIds  = '' OR S.SensorTypeId IN (Select Items from dbo.Split(@SensorTypeIds,',')))      
	 AND  (@CriticalityTypeIds = '' OR S.CriticalityTypeId IN (Select Items from dbo.Split(@CriticalityTypeIds,',')))      
	 AND  (@MachineIds = '' OR  S.MachineId  IN (Select Items from dbo.Split(@MachineIds,',')))      
	 Group BY GroupId  
) MGS on MGS.GroupId = MG.GroupId     
Where MG.CompanyId = @CompanyId   
AND (@Search IS NULL OR MG.GroupName like '%' + @Search +'%');  
END
GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetIndividualSensors')
	DROP PROCEDURE [dbo].[spGetIndividualSensors]
GO
Create PROCEDURE [dbo].[spGetIndividualSensors]   
@BranchId int,    
@CompanyId int,  
@SensorTypeIds varchar(MAX) = '',    
@CriticalityTypeIds varchar(MAX) = '',    
@MachineIds varchar(MAX) = '',    
@Search varchar(50) = NULL    
AS    
BEGIN    
Select S.SensorID,S.Voltage,  
CASE WHEN Sen.DataTypeID = 1 THEN S.Value ELSE (CASE WHEN STUC.FuncName = STU.FuncName THEN S.VALUE ELSE (dbo.fnGetUnitConversionValue(CONCAT(STUC.FuncName,'To',STU.FuncName), S.Value)) END) END as LiveValue,
STU.Alias SensorTypeUnitName,   S.SensorStatusID,SS.SensorStatusName,S.TimeElapsed,Sen.SensorBlobUrl, ISNULL(TSM.TotalSensorsOfMachine,0) TotalSensorsOfMachine,   
CASE WHEN Sen.DataTypeID = 1 THEN NULL ELSE (CASE WHEN STUC.FuncName = STU.FuncName THEN S.DayMin ELSE (dbo.fnGetUnitConversionValue(CONCAT(STUC.FuncName,'To',STU.FuncName), S.DayMin)) END)  END AS DayMin,     
CASE WHEN Sen.DataTypeID = 1 THEN NULL ELSE (CASE WHEN STUC.FuncName = STU.FuncName THEN S.DayMax ELSE (dbo.fnGetUnitConversionValue(CONCAT(STUC.FuncName,'To',STU.FuncName), S.DayMax)) END) END AS  DayMax,     
Sen.SensorName, ST.SensorTypeName,ST.SensorTypeBlobUrl, M.MachineName, S.[Datetime],
Sen.DataTypeID,D.DataTypeName, Sen.BinaryStateId , CASE WHEN S.Value = 1 THEN B.[On] ELSE B.[Off] END AS BinaryStateName
from (  
 Select Row_Number() over(Partition by SS.SensorId order by  SS.[Datetime] Desc) RN,    
 SS.[AVG], SS.SensorHistoryId, SS.SensorID, SS.Voltage, SS.Value, SS.SensorStatusID,     
 SS.DateTime, SS.TimeElapsed,MIN(SH.MIN) as DayMin ,Max(SH.Max) as DayMax     
 from SensorHistory SS     
 Join SensorHistory SH on SH.SensorId = SS.SensorID     
 Group by SS.SensorHistoryId, SS.SensorID, SS.Voltage,     
 SS.Value, SS.SensorStatusID, SS.DateTime, SS.TimeElapsed,SS.[AVG]  
) S      
Join  SensorStatus SS on S.SensorStatusID = SS.SensorStatusID     
Join  Sensor Sen on Sen.SensorID = S.SensorID AND Sen.IsActive = 1   
Join  SensorType ST on ST.SensorTypeID = Sen.SensorTypeID    
LEFT JOIN   SensorTypeUnits STU on STU.SensorTypeUnitId = Sen.SensorTypeUnitId  
LEFT JOIN   SensorTypeUnits STUC on STUC.SensorTypeUnitId = Sen.ComingUnitId  
LEFT JOIN DataType D on Sen.DataTypeID = D.DataTypeID
LEFT JOIN BinaryStates B on Sen.BinaryStateId = B.Id
Join  Machine M on Sen.MachineID = M.MachineID   
LEFT JOIN (Select COUNT(1) TotalSensorsOfMachine,MachineId FROM SENSOR WHERE SensorId in (Select SensorId from SensorHistory) AND IsActive = 1 Group BY MachineId) TSM ON TSM.MachineId = M.MachineId   
where S.RN = 1 AND M.BranchID = @BranchId AND Sen.CompanyId = @CompanyId      
AND  (@SensorTypeIds  = '' OR Sen.SensorTypeId IN (Select Items from dbo.Split(@SensorTypeIds,',')))    
AND  (@CriticalityTypeIds = '' OR Sen.CriticalityTypeId IN (Select Items from dbo.Split(@CriticalityTypeIds,',')))    
AND  (@MachineIds = '' OR  Sen.MachineId  IN (Select Items from dbo.Split(@MachineIds,',')))    
AND  (@Search IS NULL OR ST.SensorTypeName like '%' + @Search +'%' OR Sen.SensorName like '%' + @Search +'%' OR Sen.SensorId like '%' + @Search +'%'     
OR   Sen.GatewayID like '%' + @Search +'%' OR M.MachineName like '%' + @Search +'%')   
GROUP BY S.SensorID, S.Voltage, S.Value, S.SensorStatusID,M.MachineID,STU.FuncName,STUC.FuncName,  STU.Alias, TSM.TotalSensorsOfMachine,  
SS.SensorStatusName, Sen.SensorName, ST.SensorTypeName,ST.SensorTypeBlobUrl, M.MachineName, S.[Datetime], Sen.DataTypeID,Sen.SensorBlobUrl, S.TimeElapsed,S.DayMin,S.DayMax,S.[AVG],
D.DataTypeName, Sen.BinaryStateId ,B.[On], B.[Off]
ORDER BY M.MachineName,Sen.SensorName   
END   
Go
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetAlarms')
	DROP PROCEDURE [dbo].[spGetAlarms]
GO
CREATE PROCEDURE spGetAlarms
@CompanyId int
AS
BEGIN
Select S.SensorID,S.Voltage,
CASE WHEN STUC.FuncName = STU.FuncName THEN S.VALUE ELSE (dbo.fnGetUnitConversionValue(CONCAT(STUC.FuncName,'To',STU.FuncName), S.Value)) END as LiveValue,STU.Alias SensorTypeUnitName, 
S.SensorStatusID,SS.SensorStatusName,S.TimeElapsed,Sen.SensorBlobUrl,
CASE WHEN Sen.DataTypeID = 1 THEN NULL ELSE (CASE WHEN STUC.FuncName = STU.FuncName THEN S.DayMin ELSE (dbo.fnGetUnitConversionValue(CONCAT(STUC.FuncName,'To',STU.FuncName), S.DayMin)) END)  END AS DayMin,   
CASE WHEN Sen.DataTypeID = 1 THEN NULL ELSE (CASE WHEN STUC.FuncName = STU.FuncName THEN S.DayMax ELSE (dbo.fnGetUnitConversionValue(CONCAT(STUC.FuncName,'To',STU.FuncName), S.DayMax)) END) END AS  DayMax,   
Sen.SensorName, ST.SensorTypeName,ST.SensorTypeBlobUrl, M.MachineName, S.[Datetime]  from (  
	  Select Row_Number() over(Partition by SS.SensorId order by  SS.[Datetime] Desc) RN,SS.[AVG], SS.SensorHistoryId, SS.SensorID, SS.Voltage, SS.Value, 
	  SS.SensorStatusID, SS.DateTime, SS.TimeElapsed,MIN(SH.MIN) as DayMin ,Max(SH.Max) as DayMax from SensorHistory SS 
	  Join SensorHistory SH on SH.SensorId = SS.SensorID  
	  Group by SS.SensorHistoryId, SS.SensorID, SS.Voltage, SS.Value, SS.SensorStatusID, SS.DateTime, SS.TimeElapsed,SS.[AVG]
) S  
Join SensorStatus SS on S.SensorStatusID = SS.SensorStatusID 
Join Sensor Sen on Sen.SensorID = S.SensorID AND Sen.IsActive = 1
Join SensorType ST on ST.SensorTypeID = Sen.SensorTypeID 
LEFT JOIN   SensorTypeUnits STU on STU.SensorTypeUnitId = Sen.SensorTypeUnitId
LEFT JOIN   SensorTypeUnits STUC on STUC.SensorTypeUnitId = Sen.ComingUnitId
Join Machine M on Sen.MachineID = M.MachineID  
where S.RN = 1 AND Sen.CompanyId = @CompanyId
GROUP BY S.SensorID, S.Voltage, S.Value, S.SensorStatusID, SS.SensorStatusName, Sen.SensorName, ST.SensorTypeName,ST.SensorTypeBlobUrl,STU.FuncName,STUC.FuncName,  STU.Alias, 
M.MachineName, S.[Datetime],Sen.SensorBlobUrl, Sen.DataTypeID, S.TimeElapsed,S.DayMin,S.DayMax,S.[AVG]
END
Go
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetSensorDetailLastTransactions')
	DROP PROCEDURE [dbo].[spGetSensorDetailLastTransactions]
GO
CREATE PROCEDURE spGetSensorDetailLastTransactions
@SensorId varchar(500)
AS
BEGIN
SELECT Top 10 SensorHistoryID, SensorID, Voltage, Value, SensorStatusID, DateTime, [AVG], [MIN], [MAX], TimeElapsed into #SensorHistoryRecords FROM SensorHistory where SensorId = @SensorId AND SensorId in (Select SensorId from Sensor where IsActive = 1) order by [DateTime] Desc;
SELECT Top (10 - (Select Count(1) from #SensorHistoryRecords)) SensorHistoryID, SensorID, Voltage, Value, SensorStatusID, DateTime, [AVG], [MIN], [MAX], TimeElapsed 
into #SensorHistoryLogRecords FROM  SensorHistoryLogs where SensorId = @SensorId AND SensorId in (Select SensorId from Sensor where IsActive = 1) order by [DateTime] Desc;

Select S.SensorHistoryID, S.SensorID, S.Voltage,STU.Alias SensorTypeUnitName, 
CASE WHEN STUC.FuncName = STU.FuncName THEN S.VALUE ELSE (dbo.fnGetUnitConversionValue(CONCAT(STUC.FuncName,'To',STU.FuncName), S.Value)) END as Value,
S.SensorStatusID, S.DateTime, S.[AVG], 
CASE WHEN STUC.FuncName = STU.FuncName THEN S.[MIN] ELSE (dbo.fnGetUnitConversionValue(CONCAT(STUC.FuncName,'To',STU.FuncName), S.[MIN])) END [MIN], 
CASE WHEN STUC.FuncName = STU.FuncName THEN S.[MAX] ELSE (dbo.fnGetUnitConversionValue(CONCAT(STUC.FuncName,'To',STU.FuncName), S.[MAX])) END [MAX],
S.TimeElapsed,SS.SensorStatusName from (
	SELECT * from #SensorHistoryRecords
	UNION ALL
	SELECT * from #SensorHistoryLogRecords
) S 
Join		SensorStatus SS on S.SensorStatusID = SS.SensorStatusID
Join		Sensor Sen on Sen.SensorID = S.SensorID AND Sen.IsActive = 1
Join		SensorType ST on ST.SensorTypeID = Sen.SensorTypeID 
LEFT JOIN   SensorTypeUnits STU on STU.SensorTypeUnitId = Sen.SensorTypeUnitId
LEFT JOIN   SensorTypeUnits STUC on STUC.SensorTypeUnitId = Sen.ComingUnitId
END
Go
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetSensorDetailAnalyticsPerformance')
	DROP PROCEDURE [dbo].[spGetSensorDetailAnalyticsPerformance]
GO
Create PROCEDURE spGetSensorDetailAnalyticsPerformance
@SensorId varchar(500)
AS
BEGIN
SELECT Row_number() over (order by (Select 1))Id, H.SensorID, H.Voltage, 
CASE WHEN STUC.FuncName = STU.FuncName THEN H.VALUE ELSE (dbo.fnGetUnitConversionValue(CONCAT(STUC.FuncName,'To',STU.FuncName), H.Value)) END as Value,
H.SensorStatusID, H.[DateTime],S.CriticalMin,S.CriticalMax,S.WarningMin,S.WarningMax, 0.0000 as SafeZone FROM ( 
	SELECT  SensorID, Voltage, [Value], SensorStatusID, [DateTime]
	FROM   SensorHistory
	Where SensorId = @SensorId AND SensorId in (Select SensorId from Sensor where SensorId = @SensorId AND IsActive = 1)
	Order by [DateTime] Desc
	offset 0 rows      
	FETCH NEXT 30000 rows only 

	UNION ALL

	SELECT  SensorID, Voltage, [Value], SensorStatusID, [DateTime]
	FROM   SensorHistoryLogs

	Where SensorId = @SensorId AND SensorId in (Select SensorId from Sensor where SensorId = @SensorId AND IsActive = 1)
	Order by [DateTime] Desc
	offset 0 rows      
	FETCH NEXT 30000 rows only 
) H
JOIN		Sensor S on S.SensorID = H.SensorID AND S.IsActive = 1
Join		SensorType ST on ST.SensorTypeID = S.SensorTypeID 
LEFT JOIN   SensorTypeUnits STU on STU.SensorTypeUnitId = S.SensorTypeUnitId
LEFT JOIN   SensorTypeUnits STUC on STUC.SensorTypeUnitId = S.ComingUnitId
Order by [DateTime] Desc
offset 0 rows      
FETCH NEXT 30000 rows only
END
GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetSensorDetailAnalyticsStatus')
	DROP PROCEDURE [dbo].[spGetSensorDetailAnalyticsStatus]
GO
Create PROCEDURE spGetSensorDetailAnalyticsStatus
@SensorId varchar(500)
AS
BEGIN
	Declare  @Dates table ([Datetime] date null)
	Declare @DayNumber int = 0
	WHILE @DayNumber < 30
	BEGIN
		INSERT INTO @Dates (DateTime)
		Select DATEADD(DAY,-(@DayNumber),GETDATE())
		set @DayNumber = @DayNumber + 1
	END

SELECT SensorID,Datetime,ISNULL(Critical,0) Critical,ISNULL(Warning,0) Warning into #Result FROM  (  
	
	Select SH.SensorId,Count(Value) Value,SS.SensorStatusName,Cast(SH.[Datetime] as Date) [Datetime] 
	from SensorHistory SH 
	Join SensorStatus SS on SS.SensorStatusId = SH.SensorStatusId  
	Where SH.SensorId = @SensorId AND Cast(SH.[Datetime] as Date) >= DATEADD(DAY,-30,GETDATE())
	Group by Cast(SH.[Datetime] as Date),SH.SensorId,SS.SensorStatusName 

	UNION ALL 
	
	Select SHL.SensorId,Count(Value) Value,SS.SensorStatusName,Cast(SHL.[Datetime] as Date) [Datetime] 
	from SensorHistoryLogs SHL 
	Join SensorStatus SS on SS.SensorStatusId = SHL.SensorStatusId 
	Where SHL.SensorId = @SensorId AND Cast(SHL.[Datetime] as Date) >= DATEADD(DAY,-30,GETDATE()) 
	Group by Cast(SHL.[Datetime] as Date),SHL.SensorId,SS.SensorStatusName 

) t 
PIVOT( MAX(Value) FOR SensorStatusName IN ( [Critical],[Warning]) ) AS pivot_table 

SELECT * from (
	SELECT * from #Result
	UNION ALL
	SELECT @SensorId as SensorId,[Datetime],0 as Critical, 0 as Warning From @Dates Where [Datetime] NOT IN (Select [Datetime] from #Result)
	) S
Order by S.[DateTime] Desc
END
GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetTechniciansResolvedSensorHistoryById')
	DROP PROCEDURE [dbo].[spGetTechniciansResolvedSensorHistoryById]
GO
Create Procedure [dbo].[spGetTechniciansResolvedSensorHistoryById]
@SensorId varchar(MAX)
AS
BEGIN
SELECT      Row_Number() over (order by (Select 1)) RowNumber, TSRH.SensorHistoryId,  TSRH.ResolvedOn, TSRH.UserId,STU.Alias as SensorTypeUnitName,
			ISNULL(SH.[DateTime],SHL.[DateTime]) as TriggeredOn,LSH.[TimeElapsed],LSH.SensorId,LSH.[DateTime] as CurrentDateTime,
			CASE WHEN STUC.FuncName = STU.FuncName THEN ISNULL(SH.[VALUE],SHL.[Value]) ELSE (dbo.fnGetUnitConversionValue(CONCAT(STUC.FuncName,'To',STU.FuncName), ISNULL(SH.[VALUE],SHL.[Value]))) END [Value], 
			CASE WHEN STUC.FuncName = STU.FuncName THEN LSH.[Value] ELSE (dbo.fnGetUnitConversionValue(CONCAT(STUC.FuncName,'To',STU.FuncName), LSH.[Value])) END [LiveValue], 
			CASE WHEN STUC.FuncName = STU.FuncName THEN LSH.[MIN] ELSE (dbo.fnGetUnitConversionValue(CONCAT(STUC.FuncName,'To',STU.FuncName), LSH.[MIN])) END [MIN], 
			CASE WHEN STUC.FuncName = STU.FuncName THEN LSH.[MAX] ELSE (dbo.fnGetUnitConversionValue(CONCAT(STUC.FuncName,'To',STU.FuncName), LSH.[MAX])) END [MAX],
			LSH.SensorStatusID,LSH.SensorStatusName
FROM        TechnicianSensorsResolvedHistory TSRH
LEFT JOIN		SensorHistory SH on TSRH.SensorHistoryId = SH.SensorHistoryID
LEFT JOIN		SensorHistoryLogs SHL on TSRH.SensorHistoryId = SHL.SensorHistoryID
RIGHT JOIN  (Select Top 1 SH.[Min],SH.[MAX],SH.[VALUE],SH.SensorStatusID, SS.SensorStatusName,SH.SensorId,SH.[TimeElapsed],SH.[DateTime]
				from SensorHistory SH 
				JOIN SensorStatus SS on SS.SensorStatusId = SH.SensorStatusId
				where SensorId = @SensorId Order by [Datetime] desc
			) LSH on TSRH.SensorID = LSH.SensorId
JOIN		Sensor S on S.SensorID = LSH.SensorID AND S.IsActive = 1
LEFT JOIN   SensorTypeUnits STU on STU.SensorTypeUnitId = S.SensorTypeUnitId
LEFT JOIN   SensorTypeUnits STUC on STUC.SensorTypeUnitId = S.ComingUnitId
Where		LSH.SensorId = @SensorId
END
GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetMachineDetail')
	DROP PROCEDURE [dbo].[spGetMachineDetail]
GO
CREATE Procedure spGetMachineDetail
@MachineId int
AS
BEGIN
SELECT          Row_Number() over (order by (SELECT 1))RowNumber, M.MachineID,MS.StateName as MachineStatusName,
				ISNULL(SHTier1.TotalActiveAlarms,0) Tier1TotalActiveAlarms,ISNULL(SHTier2.TotalActiveAlarms,0) Tier2TotalActiveAlarms,
				  ISNULL(MT.TotalTechnicians,0) TotalTechnicians, M.MachineName,  M.CompanyId, M.MachineBlobURl  
 FROM            Machine M  
 Left Join  (  
     Select Count(1) TotalTechnicians,MachineId From MachineTechnicians --Where MachineId = 1
	 Group By MachineId  
     )MT on MT.MachineId = M.MachineId 
Left Join  (  
    Select Count(1) TotalActiveAlarms,S.MachineID from (
		Select Row_Number() over(Partition by SS.SensorId order by  SS.[Datetime] Desc) RN,SS.[AVG], SS.SensorHistoryId, SS.SensorID, SS.Voltage, SS.Value, 
		SS.SensorStatusID, SS.DateTime, SS.TimeElapsed,MIN(SS.MIN) as DayMin ,Max(SS.Max) as DayMax
		from SensorHistory SS 
		Group by SS.SensorHistoryId, SS.SensorID, SS.Voltage, SS.Value, SS.SensorStatusID, SS.DateTime, SS.TimeElapsed,SS.[AVG]
	) SH
		JOIN Sensor S on S.SensorID = SH.SensorID AND S.IsActive = 1
		WHERE (SH.SensorStatusId = 1 OR SH.SensorStatusId = 2) AND S.CriticalityTypeId = 1
		AND SH.RN = 1 --AND MachineID = 1
		Group By S.MachineID
     ) SHTier1 on SHTier1.MachineId = M.MachineId
Left Join  (  
    Select Count(1) TotalActiveAlarms,S.MachineID from (
		Select Row_Number() over(Partition by SS.SensorId order by  SS.[Datetime] Desc) RN,SS.[AVG], SS.SensorHistoryId, SS.SensorID, SS.Voltage, SS.Value, 
		SS.SensorStatusID, SS.DateTime, SS.TimeElapsed,MIN(SS.MIN) as DayMin ,Max(SS.Max) as DayMax
		from SensorHistory SS 
		Group by SS.SensorHistoryId, SS.SensorID, SS.Voltage, SS.Value, SS.SensorStatusID, SS.DateTime, SS.TimeElapsed,SS.[AVG]
	) SH
		JOIN Sensor S on S.SensorID = SH.SensorID AND S.IsActive = 1
		WHERE (SH.SensorStatusId = 1 OR SH.SensorStatusId = 2) AND S.CriticalityTypeId = 2
		AND SH.RN = 1 --AND MachineID = 1
		Group By S.MachineID
     ) SHTier2 on SHTier2.MachineId = M.MachineId
LEFT JOIN MachineStates MS on MS.StateID = M.MachineStateId
--LEFT JOIN MachineTechnicians MTU on MTU.MachineId = M.MachineId
--LEFT JOIN AspNetUsers U on U.Id = MTU.UserId
Where M.MachineId = @MachineId
END
Go
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetToNotifyUsers')
	DROP PROCEDURE [dbo].[spGetToNotifyUsers]
GO
CREATE Procedure [dbo].[spGetToNotifyUsers]    
@SensorId varchar(MAX)      
AS      
BEGIN      
  
DECLARE @CompanyId INT ,@UserIds nvarchar(MAX) = NULL, @Emails nvarchar(MAX) = NULL, @Phones nvarchar(MAX) = NULL,@PhonesForSMS nvarchar(MAX) = NULL    
  
SELECT @CompanyId = CompanyId FROM Sensor WHERE SensorID = @SensorId  
  
SELECT U.ID,U.CompanyId,ISNULL(SAT.NotificationTemplateId,Sen.NotificationTemplateId) NotificationTemplateId, U.Email,U.PhoneNumber,
CASE WHEN NT.UserId IS NULL THEN  ISNULL(UTC.IsSMS,CONVERT(BIT,0)) ELSE  NT.IsSMS END IsSMS,
CASE WHEN NT.UserId IS NULL THEN  ISNULL(UTC.IsCall,CONVERT(BIT,0)) ELSE  NT.IsCall END IsCall,  
CASE WHEN NT.UserId IS NULL THEN  ISNULL(UTC.IsEmail,CONVERT(BIT,0)) ELSE  NT.IsEmail END IsEmail,
ISNULL(UTC.IsPushNotification,NT.IsPushNotification) IsPushNotification into #Users 
FROM AspNetUsers  U 
LEFT JOIN (Select SensorId,NotificationTemplateId from Sensor) Sen on Sen.SensorID = @SensorId
LEFT JOIN SensorAssociatedTemplate SAT ON U.Id = SAT.UserId AND SAT.SensorId = @SensorId  
LEFT JOIN UserNotificationTemplates UTC ON  U.Id = UTC.UserId AND UTC.NotificationTemplateId = ISNULL(SAT.NotificationTemplateId,Sen.NotificationTemplateId)
LEFT JOIN NotificationTemplates NT on NT.NotificationTemplateId = ISNULL(SAT.NotificationTemplateId,Sen.NotificationTemplateId)
WHERE U.CompanyId = @CompanyId   

SELECT @Emails = STRING_AGG(S.Email, ', ') FROM (    
SELECT LOWER(U.Email) Email,U.IsEmail FROM #Users U )S  WHERE S.IsEmail = 1  
  
 SELECT @UserIds = STRING_AGG(S.ID, ', ') FROM (    
 SELECT LOWER(U.Id) Id,U.IsPushNotification FROM #Users U ) S  WHERE S.IsPushNotification = 1  
   
 SELECT @Phones = STRING_AGG(S.PhoneNumber, ', ') FROM (    
 SELECT LOWER(U.Id) Id,U.PhoneNumber,U.IsCall  FROM #Users U ) S  WHERE S.IsCall = 1  
  
 SELECT @PhonesForSMS = STRING_AGG(S.PhoneNumber, ', ') FROM (    
 SELECT U.PhoneNumber PhoneNumber,U.IsSMS FROM #Users U )S  WHERE S.IsSMS = 1  
  
SELECT 1 as Id, @UserIds NotifyPushUserIds, @Emails NotifyEmails, @Phones NotifyCalls, @PhonesForSMS NotifySMS    

END  

GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spDeleteSensors')
	DROP PROCEDURE [dbo].[spDeleteSensors]
GO
Create PROCEDURE [dbo].[spDeleteSensors]
@SensorIds nvarchar(MAX),
@IsSoftDelete bit = 0
AS
BEGIN
Select items into #SensorIds from  dbo.Split(@SensorIds,',')
 IF (@IsSoftDelete = 1)
 BeGIN
 Update Sensor Set IsActive = 0 , InactiveDate= GetDATE() Where SensorId IN (Select items from #SensorIds) 
 END
 Else IF(@IsSoftDelete = 0)
 Begin
 BEGIN TRY
BEGIN TRANSACTION
 Delete From TechnicianSensorsResolvedHistory Where SensorId IN (Select items from #SensorIds)
 Delete From SensorHistoryLogs Where SensorId IN (Select items from #SensorIds) 
 Delete From SensorHistory Where SensorId IN (Select items from #SensorIds) 
 Delete From SensorFiles Where SensorId IN (Select items from #SensorIds) 
 Delete From GroupSensors Where SensorId IN (Select items from #SensorIds)  
 Delete From DaysOff Where SensorId IN (Select items from #SensorIds) 
 Delete From GatewayPortsSensors Where SensorId IN (Select items from #SensorIds) 
 Delete From Sensor Where SensorId IN (Select items from #SensorIds) 
COMMIT
END TRY
BEGIN CATCH   
ROLLBACK
END CATCH

End
End
GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spDeleteSensorFiles')
	DROP PROCEDURE [dbo].[spDeleteSensorFiles]
GO
Create PROCEDURE [dbo].[spDeleteSensorFiles]
@SensorId nvarchar(MAX),
@FileIds nvarchar(MAX) = NULL
AS
BEGIN

BEGIN TRY
	BEGIN TRANSACTION
		 Delete From SensorFiles Where SensorId = @SensorId AND (@FileIds IS NULL OR SensorFileId in (Select items from dbo.Split(@FileIds,',')))
	COMMIT
END TRY
BEGIN CATCH   
	ROLLBACK
END CATCH

End
GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetSensorGroupSensorsPerformance')
	DROP PROCEDURE [dbo].[spGetSensorGroupSensorsPerformance]
GO
CREATE PROCEDURE [dbo].[spGetSensorGroupSensorsPerformance]  
@GroupId int  
AS  
BEGIN  
	--Declare  @Dates table ([Datetime] date null)
	--	Declare @DayNumber int = 0
	--	WHILE @DayNumber < 30
	--	BEGIN
	--		INSERT INTO @Dates (DateTime)
	--		Select DATEADD(DAY,-(@DayNumber),GETDATE())
	--		set @DayNumber = @DayNumber + 1
	--	END

	SELECT * FROM (
		SELECT SensorHistoryID, SensorID, Voltage, Value, SensorStatusID, DateTime, [AVG], [MIN], [MAX], TimeElapsed FROM SensorHistoryLogs 
		where SensorId in (Select SensorID from GroupSensors where GroupId = @GroupId AND SensorId in (Select SensorId from Sensor where IsActive = 1))  AND Cast([Datetime] as Date) >= DATEADD(DAY,-30,GETDATE())
	
		Union All 
	
		SELECT SensorHistoryID, SensorID, Voltage, Value, SensorStatusID, DateTime, [AVG], [MIN], [MAX], TimeElapsed FROM SensorHistory 
		where SensorId in (Select SensorID from GroupSensors where GroupId =  @GroupId AND SensorId in (Select SensorId from Sensor where IsActive = 1))  AND Cast([Datetime] as Date) >= DATEADD(DAY,-30,GETDATE())
	)S  Order by [DateTime]
END 
Go
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetCompanyById')
	DROP PROCEDURE [dbo].[spGetCompanyById]
GO
CREATE PROCEDURE spGetCompanyById 
	-- Add the parameters for the stored procedure here
	@CompanyId As INT,
	@LoggedInUserId As [uniqueidentifier]
AS
BEGIN
     DECLARE @OwnerEmail nvarchar(100) = (SELECT TOP 1 U.Email From AspNetUsers U
											INNER JOIN AspNetUserRoles r on r.UserId = u.Id
											Where U.CompanyId = @CompanyId AND r.RoleId = '93F204A2-747D-4F00-9A8F-A27043565BA8')

	 SELECT CompanyID,CompanyName,CompanyAddress,CompanyCity,CompanyState,CompanyZipCode,CompanyCountry,CompanyBlobUrl,IsPermitNewUsersSameDomain,
	(SELECT Count(Id) From AspNetUsers Where CompanyId = @CompanyId) As NumberOfUsers,
	IndustryId,
	CONVERT(BIT,
		(SELECT CASE 
					WHEN (RoleId = '93F204A2-747D-4F00-9A8F-A27043565BA8' OR RoleId ='38D42A6F-E91E-4498-B1B3-7CA8DA85E8CE')  
					AND NOT (U.Email like '%gmail.com%' OR U.Email like '%msn.com%' OR U.Email like '%hotmail.com%' OR U.Email like '%yahoo.com%' OR U.Email like '%outlook.com%')
					AND (RIGHT(U.Email, LEN(U.Email) - CHARINDEX('@', U.Email)) = RIGHT(@OwnerEmail, LEN(@OwnerEmail) - CHARINDEX('@', @OwnerEmail)))
					THEN 1
					ELSE 0
				END
			FROM AspNetUserRoles R
			INNER JOIN AspNetUsers U on U.Id = R.UserId
			
			WHERE UserId = @LoggedInUserId)) AS IsPermitCheckboxShow
	From company 
	WHERE CompanyId = @CompanyId;
END
GO

IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetGeneralSettingsById')
	DROP PROCEDURE [dbo].[spGetGeneralSettingsById]
GO
Create PROCEDURE [dbo].[spGetGeneralSettingsById] 
	-- Add the parameters for the stored procedure here
	@UserId As varchar(100) = NULL
AS
BEGIN

	Select U.Id,U.FirstName,U.LastName,U.NormalizedEmail as Email,U.Title,U.PhoneNumber ,U.ProfileBlobUrl,U.TimeZone, U.CompanyId, R.Id as RoleId, R.Name as RoleName
	From AspNetUsers U
	LEFT JOIN AspNetUserRoles UR on UR.UserId = U.Id
	LEFT JOIN AspNetRoles R on R.Id = UR.RoleId
	Where U.Id = @UserId
END
Go
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetUsers')
	DROP PROCEDURE [dbo].spGetUsers
GO
Create Procedure [dbo].[spGetUsers]
@CompanyId int ,
@RoleId uniqueIdentifier=NULL
AS
BEGIN
	DECLARE @IsActive bit=1
	DECLARE @IsFinished bit=0
	DECLARE @RoleType VARCHAR(30)=(SELECT Top(1) RoleType FROM OtherRoles WHERE RoleId=@RoleId )
	IF @RoleType='DEACTIVATED'
	BEGIN
		SET @IsActive=0
		SET @RoleId=NULL -- So that Below Query will Fetch All User
	END
	ELSE IF @RoleType='ALL'
	BEGIN
		SET @RoleId=NULL -- So that Below Query will Fetch All User
	END
	ELSE IF @RoleType='INVITE_PENDING'
	BEGIN
		SELECT Row_Number() Over(Order By (Select 1)) as RowNumber,Id,'' FirstName,'' LastName, UI.UserEmail Email,'' Title,
		'' PhoneNumber ,'' ProfileBlobUrl,CompanyId,null as RoleId, '' RoleName, NULL as StartTime, NULL as EndTime,CAST(0 as bit) as IsActive,
		UI.DateCreated AS InvitationDate,UI.ReminderDate
		FROM UserInvitations UI
		WHERE IsPending=1 AND CompanyId = @CompanyId
				
	END
	ELSE IF @RoleId='38D42A6F-E91E-4498-B1B3-7CA8DA85E8CE'
	BEGIN
		SET DATEFIRST 1 -- Setting SQL WeekDay Monday to be the first day of week 
		SELECT Row_Number() Over(Order By (Select 1)) as RowNumber, U.Id,U.FirstName,U.LastName, U.Email,U.Title,
		U.PhoneNumber ,U.ProfileBlobUrl, U.CompanyId,R.Id as RoleId, R.Name as RoleName, US.StartTime,US.EndTime,
		NULL AS InvitationDate,U.IsActive, NULL as ReminderDate
		FROM    AspNetUsers U
		LEFT JOIN  AspNetUserRoles UR ON UR.UserId = U.Id
		LEFT JOIN AspNetRoles R ON R.Id = UR.RoleId

		OUTER APPLY 
		( SELECT Top 1 StartTime,EndTime
		  FROM UserShifts 
		  WHERE UserId=U.Id AND
			[DayOfWeek]=DATEPART(WEEKDAY,GetDate())
		) AS US
		WHERE
		  CompanyId = @CompanyId AND 
		  (@RoleId IS NULL  OR UR.RoleID=@RoleId OR UR.RoleID  = '93F204A2-747D-4F00-9A8F-A27043565BA8') AND
		  (@IsActive=1 OR U.IsActive=@IsActive)

		  SET @IsFinished=1 -- Mean Response Should be sent without executing further
	END
	IF @IsFinished=0
	BEGIN
		SET DATEFIRST 1 -- Setting SQL WeekDay Monday to be the first day of week 
		SELECT Row_Number() Over(Order By (Select 1)) as RowNumber, U.Id,U.FirstName,U.LastName, U.Email,U.Title,
		U.PhoneNumber ,U.ProfileBlobUrl, U.CompanyId,R.Id as RoleId, R.Name as RoleName, US.StartTime,US.EndTime,
		NULL AS InvitationDate,U.IsActive, NULL as ReminderDate
		FROM    AspNetUsers U
		LEFT JOIN  AspNetUserRoles UR ON UR.UserId = U.Id
		LEFT JOIN AspNetRoles R ON R.Id = UR.RoleId

		OUTER APPLY 
		( SELECT Top 1 StartTime,EndTime
		  FROM UserShifts 
		  WHERE UserId=U.Id AND
			[DayOfWeek]=DATEPART(WEEKDAY,GetDate())
		) AS US
		WHERE
		  CompanyId = @CompanyId AND 
		  (@RoleId IS NULL  OR UR.RoleID=@RoleId) AND
		  (@IsActive=1 OR U.IsActive=@IsActive)
	END
END
GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetGeneralUsers')
	DROP PROCEDURE [dbo].spGetGeneralUsers
GO
Create Procedure [dbo].[spGetGeneralUsers]
@CompanyId int, 
@UserId uniqueIdentifier
AS
BEGIN
		SELECT u.Id,u.FirstName,u.LastName,roles.Name as RoleName
		FROM AspNetUsers u
		INNER JOIN AspNetUserRoles ur ON Ur.UserId=u.Id
		INNER JOIN AspNetRoles roles ON roles.Id=ur.RoleId
		WHERE roles.NormalizedName <> 'SUPERADMIN' AND u.Id <> @UserId AND
		CompanyId=@CompanyId
END
GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetTotalUserRoles')
	DROP PROCEDURE [dbo].spGetTotalUserRoles
GO
CREATE Procedure spGetTotalUserRoles
@CompanyId int
AS
BEGIN
	SELECT ROW_NUMBER() OVER (Order By RoleId)as Id, RoleId,RoleName,TotalUsers,SortOrder 
	FROM 
	(
		SELECT (SELECT Top(1) RoleId FROM OtherRoles WHERE RoleType='ALL') as RoleId,'Total Users' as RoleName,
			ISNULL((SELECT Count(Id) as TotalUsers
				FROM AspNetUsers users
				WHERE CompanyId=@CompanyId
				GROUP BY users.CompanyId 
			),0)as TotalUsers ,0 SortOrder

		UNION 

		SELECT Distinct roles.Id as RoleId,roles.[Name] as RoleName ,Count(userRoles.UserId) as TotalUsers,SortOrder
		FROM AspNetRoles roles
			LEFT JOIN 
			(SELECT userRoles.RoleId,userRoles.UserId
			FROM AspNetUserRoles userRoles 
				INNER JOIN AspNetUsers users ON
					users.Id=userRoles.UserId 
			WHERE CompanyId=@CompanyId
			) as userRoles ON roles.Id=userRoles.RoleId
		GROUP BY roles.Id,roles.[Name],SortOrder
		HAVING roles.Id NOT IN('93F204A2-747D-4F00-9A8F-A27043565BA8', '38D42A6F-E91E-4498-B1B3-7CA8DA85E8CE')

		UNION
		
		SELECT '38D42A6F-E91E-4498-B1B3-7CA8DA85E8CE' As RoleId,
		'Admin' as RoleName ,COUNT(userRoles.UserId) as TotalUsers,1 SortOrder
		FROM AspNetRoles roles
			LEFT JOIN 
			(SELECT userRoles.RoleId,userRoles.UserId
			FROM AspNetUserRoles userRoles 
				INNER JOIN AspNetUsers users ON
					users.Id=userRoles.UserId 
			WHERE  CompanyId=@CompanyId
			) as userRoles ON roles.Id=userRoles.RoleId
		where  userRoles.RoleId IN ('93F204A2-747D-4F00-9A8F-A27043565BA8', '38D42A6F-E91E-4498-B1B3-7CA8DA85E8CE')	

		UNION

		SELECT 
			(SELECT Top(1) RoleId From OtherRoles WHERE RoleType='DEACTIVATED') As RoleId
			,'Deactivated Users' as RoleName
			,Count(Id) as TotalUsers,7 SortOrder
		FROM AspNetUsers 
		WHERE IsActive=0 AND CompanyId=@CompanyId

		UNION

		SELECT 
			(SELECT Top(1) RoleId From OtherRoles WHERE RoleType='INVITE_PENDING') As RoleId
			,'Invite Pending' as RoleName
			,Count(Id) as TotalUsers,8 as SortOrder
		FROM UserInvitations 
		WHERE IsPending=1 AND CompanyId=@CompanyId

	) as RolesData
	ORDER BY SortOrder
END
GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetUserAssets')
	DROP PROCEDURE [dbo].spGetUserAssets
GO
CREATE Procedure spGetUserAssets
@UserId uniqueIdentifier
AS
BEGIN
	SELECT UA.MachineId,M.MachineName 
	FROM UserAssets UA
	INNER JOIN Machine M ON
		M.MachineId=UA.MachineId
	WHERE UA.UserId=@UserId
END
GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetUserShifts')
	DROP PROCEDURE [dbo].spGetUserShifts
GO
CREATE Procedure spGetUserShifts
@UserId uniqueIdentifier
AS
BEGIN
	SELECT ROW_NUMBER() OVER(Order by [DayOfWeek]) Id, UserId,[DayOfWeek],DateName(DW,([DayOfWeek])-1) As [DayName],StartTime,EndTime,IsCustom
	FROM UserShifts
	WHERE UserId=@UserId
END
GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetGatewayShifts')
	DROP PROCEDURE [dbo].spGetGatewayShifts
GO
CREATE Procedure spGetGatewayShifts
@GatewayId varchar(50)
AS
BEGIN
	SELECT ROW_NUMBER() OVER(Order by [DayOfWeek]) Id, GatewayId,[DayOfWeek],DateName(DW,([DayOfWeek])-1) As [DayName],StartTime,EndTime,IsCustom
	FROM GatewayShifts
	WHERE GatewayId=@GatewayId
END
GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spTransferOwnership')
	DROP PROCEDURE [dbo].spTransferOwnership
GO
CREATE Procedure spTransferOwnership
@TransferFromUserId uniqueIdentifier,
@TransferToUserId uniqueIdentifier,
@NewRoleId uniqueIdentifier
AS
BEGIN
	-- Get Role of User to be transfer
	DECLARE @FromUserRoleId UniqueIdentifier = (SELECT RoleId FROM AspNetUserRoles WHERE UserId=@TransferFromUserId)
	IF @FromUserRoleId IS NOT NULL
	BEGIN
		-- Update To User
		-- Check for second user existence in roles table
		IF EXISTS (SELECT UserId FROM AspNetUserRoles WHERE UserId=@TransferToUserId )
		BEGIN
			UPDATE AspNetUserRoles 
			SET RoleId=@FromUserRoleId
			WHERE UserId=@TransferToUserId
		END
		ELSE
		BEGIN
			INSERT INTO AspNetUserRoles(UserId,RoleId)
			VALUES(@TransferToUserId,@FromUserRoleId)
		END
		-- Update From User
		UPDATE AspNetUserRoles 
		SET RoleId=@NewRoleId
		WHERE UserId=@TransferFromUserId
	END
END
GO
IF EXISTS(SELECT * FROM sys.tables WHERE type='U' AND NAME='OtherRoles')
DROP TABLE OtherRoles
GO
CREATE TABLE OtherRoles
(
	RoleId uniqueIdentifier NOT NULL Primary Key,
	RoleType VARCHAR(50) NOT NULL 
)
GO 
INSERT INTO OtherRoles VALUES('57c1fe0a-44b6-4ac2-8438-e3acbd925277','ALL')
INSERT INTO OtherRoles VALUES('0de47a42-86ff-49f5-a688-b7916122c02e','DEACTIVATED')
INSERT INTO OtherRoles VALUES('ad0c43a3-7dde-4e10-89d0-f4caea6fc7e8','INVITE_PENDING')
GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spRegisterWithEmail')
	DROP PROCEDURE [dbo].spRegisterWithEmail
GO
CREATE Procedure spRegisterWithEmail
@Email varchar(500)
AS 
BEGIN
Declare @CompanyId int = NULL

		 IF (SELECT Count(1) From AspNetUsers where Email like '%'+@Email+'%') > 0
		 BEGIN
			Select 1 as Id, 'Email Already Exists' as ErrorMessage,NULL as CompanyId, NULL as RoleId
		 END
		 ELSE
		 BEGIN
			IF @Email like '%gmail.com%' OR @Email like '%msn.com%' OR @Email like '%hotmail.com%' OR @Email like '%yahoo.com%' OR @Email like '%outlook.com%'
			BEGIN
				Select 1 as Id,  NULL as CompanyId,Convert(uniqueidentifier,'93F204A2-747D-4F00-9A8F-A27043565BA8') RoleId, NULL as ErrorMessage
			END
			ELSE
			BEGIN
				Select Top 1 @CompanyId = CompanyId
				From AspNetUsers where Email like  '%'+RIGHT(@Email, CHARINDEX('@', REVERSE(@Email)) - 1) +'%'
				AND Id in (Select UserId from AspNetUserRoles where RoleId = '93F204A2-747D-4F00-9A8F-A27043565BA8')

				if @CompanyId IS NOT NULL
				BEGIN
					IF (Select ISNULL(IsPermitNewUsersSameDomain,0) From Company where CompanyId = @CompanyId) = 1
					BEGIN 
						Select 1 as Id, @CompanyId as CompanyId,Convert(uniqueidentifier,'8715E1F3-D0D0-4E4B-B458-06B9AA378103') RoleId, NULL as ErrorMessage
					END
					ELSE
					BEGIN
						Select 1 as Id, 'Please ask admin to invite you' as ErrorMessage,NULL as CompanyId, NULL as RoleId
					END
				END
				ELSE
				BEGIN
					Select 1 as Id, @CompanyId as CompanyId,Convert(uniqueidentifier,'93F204A2-747D-4F00-9A8F-A27043565BA8') RoleId, NULL as ErrorMessage
				END
			END
	 END
END
Go
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spDeleteCompany')
	DROP PROCEDURE [dbo].[spDeleteCompany]
GO
Create PROCEDURE [dbo].[spDeleteCompany]
@CompanyID Int 
AS
BEGIN
BEGIN TRY
BEGIN TRANSACTION

DELETE FROM TechnicianSensorsResolvedHistory WHERE SensorId IN (SELECT SensorID FROM Sensor WHERE CompanyId = @CompanyID)
DELETE FROM SensorHistoryLogs WHERE SensorId IN (SELECT SensorID FROM Sensor WHERE CompanyId = @CompanyID)
DELETE FROM SensorHistory WHERE SensorId IN (SELECT SensorID FROM Sensor WHERE CompanyId = @CompanyID)
DELETE FROM SensorFiles WHERE SensorId IN (SELECT SensorID FROM Sensor WHERE CompanyId = @CompanyID)
DELETE FROM GroupSensors WHERE SensorId IN (SELECT SensorID FROM Sensor WHERE CompanyId = @CompanyID)
DELETE FROM Groups WHERE CompanyId = @CompanyID
DELETE FROM DaysOff WHERE SensorId IN (SELECT SensorID FROM Sensor WHERE CompanyId = @CompanyID)
DELETE FROM Sensor WHERE CompanyId = @CompanyID
----Delete Gateway data by company ID
DELETE FROM Gateway WHERE CompanyId = @CompanyID
DELETE FROM GatewayPortsSensors  WHERE GatewayPortID IN  (
							SELECT GatewayPortID FROM GatewayPorts WHERE GatewayID IN (SELECT GatewayID FROM Gateway WHERE CompanyId = @CompanyID))
DELETE FROM GatewayPorts WHERE GatewayID IN (SELECT GatewayID FROM Gateway WHERE CompanyId = @CompanyID)
DELETE FROM Gateway WHERE CompanyId = @CompanyID

---Delete Machine Data

DELETE FROM MachineLocations WHERE MachineId IN (SELECT MachineID FROM Machine WHERE CompanyId = @CompanyID)
DELETE FROM MachineOperators WHERE MachineId IN (SELECT MachineID FROM Machine WHERE CompanyId = @CompanyID)
DELETE FROM MachineTechnicians WHERE MachineId IN (SELECT MachineID FROM Machine WHERE CompanyId = @CompanyID)
DELETE FROM Machine WHERE CompanyId = @CompanyID

---- Delete Users Roles
DELETE FROM AspNetUserRoles WHERE UserId IN (
										SELECT Id FROM AspNetUsers WHERE CompanyId = @CompanyID)

---- Delete company Users
DELETE FROM AspNetUsers WHERE CompanyId = @CompanyID
---- Delete Company

DELETE FROM Company WHERE CompanyID = @CompanyID

COMMIT
END TRY
BEGIN CATCH
ROLLBACK
END CATCH
END
GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetAssignedTechniciansToAsset')
	DROP PROCEDURE [dbo].[spGetAssignedTechniciansToAsset]
GO
CREATE PROCEDURE spGetAssignedTechniciansToAsset
@MachineId INT   
AS  
BEGIN  
SELECT   Id AS UserId, FirstName, LastName, CompanyId, Title, ProfileBlobUrl , MT.Description,MT.IsAchknowledged, MT.IsResolved , ASP.PhoneNumber 
--,CreatedDate, NULL AS IndustryId, Email  
FROM    AspNetUsers ASP   
  INNER JOIN MachineTechnicians MT on ASP.Id = MT.UserId   
WHERE   MT.MachineId = @MachineId  
END 
GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetUserNotificationTemplates')
	DROP PROCEDURE [dbo].[spGetUserNotificationTemplates]
GO
CREATE PROCEDURE [dbo].spGetUserNotificationTemplates
@UserId [uniqueidentifier],  
@CompanyId INT,  
@NotificationTemplateId INT = NULL  
AS  
BEGIN  
 
SELECT * INTO #UserNotificationTemplates FROM( 
SELECT NotificationTemplateId,CompanyId, UserId, IsSMS, IsCall, IsEmail,IsPushNotification,CONVERT(BIT,1) AS IsCompany FROM UserNotificationTemplates WHERE UserId = @UserId  
UNION ALL
SELECT NotificationTemplateId,CompanyId, UserId, IsSMS, IsCall, IsEmail,IsPushNotification, CONVERT(BIT,0) AS IsCompany FROM NotificationTemplates WHERE UserId = @UserId  
)S
  
SELECT  TEMP.NotificationTemplateId, NT.NotificationTemplateName,TEMP.CompanyId, TEMP.UserId, TEMP.IsSMS, TEMP.IsCall, TEMP.IsEmail, NT.Description,  
		TEMP.IsPushNotification, TEMP.IsCompany   
FROM	#UserNotificationTemplates TEMP  
INNER JOIN  NotificationTemplates NT ON NT.NotificationTemplateId = TEMP.NotificationTemplateId  
WHERE  TEMP.CompanyId = @CompanyId AND (@NotificationTemplateId IS NULL OR TEMP.NotificationTemplateId = @NotificationTemplateId)  
  
UNION ALL  
  
SELECT   NotificationTemplateId,NotificationTemplateName, CompanyId, UserId, CONVERT(BIT,0) IsSMS, CONVERT(BIT,0) AS IsCall, CONVERT(BIT,0) AS IsEmail, [Description],  
		 IsPushNotification, CONVERT(BIT,1) AS IsCompany   
FROM     NotificationTemplates   
WHERE   ((CompanyId IS NULL OR CompanyId = @CompanyId) AND UserId IS NULL AND  (NotificationTemplateId NOT IN (SELECT NotificationTemplateId FROM #UserNotificationTemplates)))  
AND		(@NotificationTemplateId IS NULL OR NotificationTemplateId = @NotificationTemplateId) 
  
END  
Go
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetSensorTypeUnitsByGroupIdCombo')
	DROP PROCEDURE [dbo].spGetSensorTypeUnitsByGroupIdCombo
GO
CREATE PROCEDURE spGetSensorTypeUnitsByGroupIdCombo 
@GroupId int
AS
BEGIN
	SELECT STU.SensorTypeUnitId, STU.UnitName , STU.Alias
	FROM SensorTypeUnits STU 
	LEFT JOIN Groups G ON STU.SensorTypeId = G.SensorTypeId
	WHERE G.GroupId = @GroupId
END
GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spUpdateMachineState')
	DROP PROCEDURE [dbo].spUpdateMachineState
GO
CREATE PROC spUpdateMachineState  --'20',0.00 ,'1F12C835-F149-4A09-ABFB-414C07262E60'
 @SensorId varchar(MAX) ,
 @Value Decimal(18,2),
 @UserId uniqueidentifier
 AS
 BEGIN

DECLARE @MachineId INT ,@SensorState INT, @MachineState INT, @MachineSortOrder INT , @SensorSortOrder INT ,@DataType INT, @MachineStateId INT

SELECT @MachineId = MachineId , @DataType = DataTypeID
FROM Sensor 
WHERE SensorID = @SensorId

If(@DataType = 2 )
BEGIN
	SELECT @SensorState = 
	CASE 
	WHEN (@Value <= CriticalMin) THEN (  CriticalMinState )
	WHEN (@Value >= CriticalMax) THEN ( CriticalMaxState)
	WHEN (@Value <= WarningMin) THEN ( WarningMinState )
	WHEN (@Value >= WarningMax) THEN ( WarningMaxState )
	ELSE 0
	END 
	FROM Sensor 
	WHERE SensorID = @SensorId
END
ELSE IF (@DataType = 1)
BEGIN
	IF (@Value = convert(decimal,01.00))
		SELECT @SensorState = OnStateId FROM Sensor WHERE SensorID = @SensorId
	ELSE 
		SELECT @SensorState = OffStateId FROM Sensor WHERE SensorID = @SensorId
END

SELECT @MachineState = ISNULL(MachineStateId,4) -- Assuming that if MachineId is null then it is in Operational state for which the state id = 4 
FROM Machine
WHERE MachineID = @MachineId 

SELECT @MachineSortOrder = SortOrder FROM MachineStates WHERE StateID = @MachineState
SELECT @SensorSortOrder = SortOrder FROM MachineStates WHERE StateID = @SensorState

IF @MachineSortOrder <= @SensorSortOrder
	SET @MachineSortOrder = @MachineSortOrder
ELSE 
	SET @MachineSortOrder = @SensorSortOrder

UPDATE Machine
SET MachineStateId = ISNULL((SELECT StateID FROM MachineStates WHERE SortOrder = @MachineSortOrder ),4)
WHERE MachineID = @MachineId 

SELECT @MachineStateId = MachineStateId FROM Machine WHERE MachineID = @MachineId

--SELECT @MachineStateId,@SensorState,@MachineState,@MachineSortOrder,@SensorSortOrder

SELECT MachineID,MachineName,MachineStateId FROM Machine WHERE MachineID = @MachineId  

-- Creating Incident at the time of determining machine state

DECLARE @CompanyId INT,@MainIncidentId INT = NULL, @ReportTypeId INT, @IsAutoGenerateReport BIT, @IsResolved BIT, @ResolvedDate DATETIME

SELECT @CompanyId = CompanyId FROM Machine WHERE MachineID = @MachineId
SELECT @ReportTypeId = Id from ReportType where Name = 'Incident'
SELECT @IsAutoGenerateReport = IsGenerateReport FROM MachineStates WHERE StateID = @MachineStateId

SELECT TOP 1 @IsResolved = IsResolved, @ResolvedDate = ResolvedDate , @MainIncidentId = Id
FROM MachineMainIncidents WHERE MachineId = @MachineId AND CompanyId = @CompanyId AND ReportTypeId = @ReportTypeId
ORDER BY Id DESC


IF (@IsAutoGenerateReport = 1)
BEGIN

	BEGIN TRANSACTION

		IF(@IsResolved = 1 AND DATEDIFF(MINUTE,@ResolvedDate,GETDATE()) >= 10)
		BEGIN
				UPDATE MachineMainIncidents 
				SET IsResolved = 0 ,
				ResolvedDate = NULL
				WHERE Id = @MainIncidentId
		END
		ELSE
		BEGIN
			INSERT INTO MachineMainIncidents (MachineId, CompanyId, ReportTypeId, MachineStateId, CreatedBy, DateCreated,IsResolved, ResolvedDate) VALUES
			(@MachineId, @CompanyId, @ReportTypeId, @MachineStateId,  @UserId, GETDATE(), 0, NULL)
		END

		SELECT @MainIncidentId = SCOPE_IDENTITY() 
	
		INSERT INTO MachineIncidents ( MachineId, CompanyId, UserId, Description, MainIncidentId, CreatedBy, DateCreated) VALUES
		(@MachineId, @CompanyId, @UserId, 'Incident happened on this asset', @MainIncidentId, @UserId, GETDATE())

	COMMIT TRANSACTION

END

END

GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetSensorsByCompanyId')
	DROP PROCEDURE [dbo].spGetSensorsByCompanyId
GO
CREATE PROCEDURE spGetSensorsByCompanyId   
@CompanyId int,  
@NotificationTemplateId int = NULL,
@Search varchar(100) = NULL,
@UserId uniqueidentifier = NULL
AS    
BEGIN    

SELECT S.SensorID,S.SensorName, ST.SensorTypeName, M.MachineName,S.NotificationTemplateId AS GlobalNotificationTemplateId,N.NotificationTemplateName AS GlobalNotificationTemplateName, 
	   SA.NotificationTemplateId AS UserNotificationTemplateId,NT.NotificationTemplateName AS UserNotificationTemplateName
FROM Sensor S
LEFT JOIN SensorType ST ON S.SensorTypeID = ST.SensorTypeID
LEFT JOIN Machine M ON S.MachineId = M.MachineId
LEFT JOIN NotificationTemplates N ON S.NotificationTemplateId = N.NotificationTemplateId
LEFT JOIN SensorAssociatedTemplate SA on SA.SensorId = S.SensorID AND SA.UserId = @UserId
LEFT JOIN NotificationTemplates NT ON SA.NotificationTemplateId = NT.NotificationTemplateId
WHERE S.CompanyId = @CompanyId
AND (@Search IS NULL OR S.SensorName LIKE '%' + @Search + '%' OR M.MachineName LIKE '%' + @Search + '%')
AND (@NotificationTemplateId IS NULL OR @NotificationTemplateId = 0 OR S.NotificationTemplateId = @NotificationTemplateId OR SA.NotificationTemplateId = @NotificationTemplateId)

END    
GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spMoveAssociatedTemplate')
	DROP PROCEDURE [dbo].spMoveAssociatedTemplate
GO
CREATE PROCEDURE spMoveAssociatedTemplate 
@SensorId varchar(max),
@NotificationTemplateId int,
@UserId uniqueidentifier,
@ForEveryone bit
AS
BEGIN

IF (@ForEveryone = 1)
BEGIN
	 IF (SELECT UserId FROM NotificationTemplates WHERE NotificationTemplateId = @NotificationTemplateId) IS NOT NULL
		 BEGIN
			Select 1 as Id, 'This Notification Template Id exisst for a specific User.' as ErrorMessage
		 END
	ELSE
	BEGIN
		UPDATE S 
		SET S.NotificationTemplateId = @NotificationTemplateId 
		FROM Sensor S 
		JOIN (SELECT items FROM dbo.Split(@SensorId,',')) SI ON SI.items = S.SensorID

		Select 1 as Id, NULL as ErrorMessage
	END
END
ELSE
BEGIN
	IF EXISTS (SELECT 8 FROM SensorAssociatedTemplate WHERE UserId = @UserId AND SensorId IN (SELECT items FROM dbo.Split(@SensorId,',')))
	BEGIN
		UPDATE SensorAssociatedTemplate
		SET NotificationTemplateId = @NotificationTemplateId 
		WHERE UserId = @UserId AND SensorId IN (SELECT items FROM dbo.Split(@SensorId,','))
	END
	--ELSE
	--BEGIN
		INSERT INTO SensorAssociatedTemplate (NotificationTemplateId, UserId, SensorId)  
		SELECT @NotificationTemplateId,@UserId,items FROM dbo.Split(@SensorId,',')
		WHERE items NOT IN (SELECT SensorId FROM SensorAssociatedTemplate WHERE UserId = @UserId AND SensorId IN (items))
	--END

	Select 1 as Id, NULL as ErrorMessage
END
END

GO

IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetMachineCommentsbyId')
	DROP PROCEDURE [dbo].spGetMachineCommentsbyId
GO
CREATE PROCEDURE spGetMachineCommentsbyId
@MachineId int
AS
BEGIN

SELECT MC.Id,MC.Description, MC.DateCreated , A.ProfileBlobUrl,A.Id AS UserId ,A.FirstName + ' ' + A.LastName AS FullName, (SELECT COUNT(*) from MachineComments WHERE MachineId = @MachineId) AS TotalComments  
FROM MachineComments MC  
INNER JOIN AspNetUsers A ON MC.UserId = A.Id  
WHERE MC.MachineId = @MachineId  
ORDER BY MC.DateCreated DESC , MC.Id Desc  
END

GO

IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetMachineIncidentsbyId')
	DROP PROCEDURE [dbo].spGetMachineIncidentsbyId
GO
CREATE PROCEDURE spGetMachineIncidentsbyId
@MachineId int,
@IncidentId int = null
AS
BEGIN
	IF ISNULL(@IncidentId, 0) = 0

	SELECT MI.Id, MI.Description, MI.DateCreated 
	FROM MachineIncidents MI
	INNER JOIN MachineMainIncidents M ON MI.MainIncidentId = M.Id AND M.IsResolved = 0
	LEFT JOIN AspNetUsers A ON MI.UserId = A.Id
	WHERE MI.MachineId = @MachineId 
	ORDER BY MI.DateCreated DESC , MI.Id Desc

ELSE
	SELECT MI.Id, MI.Description, MI.DateCreated 
	FROM MachineIncidents MI
	INNER JOIN MachineMainIncidents M ON MI.MainIncidentId = M.Id
	LEFT JOIN AspNetUsers A ON MI.UserId = A.Id
	WHERE MI.MachineId = @MachineId  AND MI.MainIncidentId = @IncidentId
	ORDER BY MI.DateCreated DESC , MI.Id Desc
END

GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spDeleteNotificationTemplate')
	DROP PROCEDURE [dbo].spDeleteNotificationTemplate
GO
CREATE PROC spDeleteNotificationTemplate
@NotificationTemplateId INT,
@UserId uniqueidentifier
AS
BEGIN

IF EXISTS (SELECT 8 FROM SensorAssociatedTemplate WHERE NotificationTemplateId = @NotificationTemplateId AND UserId = @UserId)
BEGIN
	DELETE FROM SensorAssociatedTemplate WHERE NotificationTemplateId = @NotificationTemplateId AND UserId = @UserId
END

DELETE FROM NotificationTemplates WHERE NotificationTemplateId = @NotificationTemplateId AND UserId = @UserId

END

GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetUsersCombo')
	DROP PROCEDURE [dbo].spGetUsersCombo
GO
CREATE PROC spGetUsersCombo 
@MachineId INT,
@companyId INT,
@SearchName VARCHAR(100) = NULL
AS
BEGIN
SELECT U.Id,U.FirstName,U.LastName,U.ProfileBlobUrl FROM AspNetUsers U
INNER JOIN Machine M ON U.Companyid = M.CompanyId
WHERE u.CompanyId = @companyId AND M.MachineId = @MachineId AND (U.FirstName LIKE '%' + ISNULL(@SearchName,'') +'%' OR U.LastName LIKE '%' + ISNULL(@SearchName,'') +'%')
END

GO
IF EXISTS (SELECT * FROM   sys.objects WHERE  type = 'P' AND NAME = 'spGetIncidents')
	DROP PROCEDURE [dbo].spGetIncidents
GO
CREATE PROC spGetIncidents 
@CompanyId int,      
@MachineId int,
@Search varchar(100)
AS    
BEGIN    

	SELECT MI.Id,R.Name,MI.DateCreated,MS.StateName, '' ResolutionTime 
	FROM MachineMainIncidents MI 
	LEFT JOIN Machine M ON M.MachineID = MI.MachineId
	LEFT JOIN ReportType R ON MI.ReportTypeId = R.Id
	LEFT JOIN MachineStates MS ON M.MachineStateId = MS.StateID
	WHERE M.MachineID = @MachineId AND M.CompanyId = @CompanyId AND (MI.MachineId Like '%'+ISNULL(@Search,'') +'%' OR M.MachineName LIKE '%'+ @Search +'%')
END

GO