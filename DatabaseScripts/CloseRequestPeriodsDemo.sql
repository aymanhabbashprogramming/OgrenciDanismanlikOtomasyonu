-- CloseRequestPeriodsDemo.sql
-- Deletes only demo request periods (IsDemo = 1). Never touches official rows.
-- Run with: sqlcmd -S EYMEN -d OgrenciDanismanlikDB -i DatabaseScripts/CloseRequestPeriodsDemo.sql -f 65001

SET NOCOUNT ON;

DELETE FROM RequestPeriods WHERE IsDemo = 1;
PRINT N'Demo dönemleri silindi. Etkilenen satır sayısı: ' + CAST(@@ROWCOUNT AS NVARCHAR(10));
