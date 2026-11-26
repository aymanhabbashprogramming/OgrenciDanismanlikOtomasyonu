-- RequestPeriodsDemoColumn.sql
-- Adds IsDemo column to RequestPeriods. Run once before OpenRequestPeriodsDemo.sql.
-- Run with: sqlcmd -S EYMEN -d OgrenciDanismanlikDB -i DatabaseScripts/RequestPeriodsDemoColumn.sql -f 65001

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'RequestPeriods') AND name = N'IsDemo'
)
BEGIN
    ALTER TABLE RequestPeriods ADD IsDemo BIT NOT NULL CONSTRAINT DF_RequestPeriods_IsDemo DEFAULT 0;
    PRINT N'IsDemo sütunu RequestPeriods tablosuna eklendi.';
END
ELSE
    PRINT N'IsDemo sütunu zaten mevcut.';
