-- OpenRequestPeriodsDemo.sql
-- Inserts demo request periods (IsDemo = 1) for today + 7 days.
-- NEVER updates or upserts existing rows — only INSERTs new rows.
-- Run RequestPeriodsDemoColumn.sql first, then SeedData.sql if needed.
-- To remove: run CloseRequestPeriodsDemo.sql.
-- Run with: sqlcmd -S EYMEN -d OgrenciDanismanlikDB -i DatabaseScripts/OpenRequestPeriodsDemo.sql -f 65001

SET NOCOUNT ON;

DECLARE @today   DATE = CAST(GETDATE() AS DATE);
DECLARE @endDate DATE = DATEADD(DAY, 7, @today);

DECLARE @currentSemesterId INT = (
    SELECT TOP 1 SemesterId
    FROM   Semesters
    WHERE  StartDate <= @today
      AND  EndDate   >= @today
    ORDER BY StartDate DESC
);

IF @currentSemesterId IS NULL
BEGIN
    PRINT N'Hata: Bugünün tarihiyle örtüşen dönem bulunamadı.';
    PRINT N'Semesters tablosunu kontrol edin veya tarih aralığını kapsayan bir satır ekleyin.';
    RETURN;
END

DECLARE @rt02 INT = (SELECT RequestTypeId FROM RequestTypes WHERE FormCode = N'TF-ÖİŞL-02');
DECLARE @rt05 INT = (SELECT RequestTypeId FROM RequestTypes WHERE FormCode = N'TF-ÖİŞL-05');
DECLARE @rt06 INT = (SELECT RequestTypeId FROM RequestTypes WHERE FormCode = N'TF-ÖİŞL-06');
DECLARE @rt14 INT = (SELECT RequestTypeId FROM RequestTypes WHERE FormCode = N'TF-ÖİŞL-14');

IF @rt02 IS NULL OR @rt05 IS NULL OR @rt06 IS NULL OR @rt14 IS NULL
BEGIN
    PRINT N'Hata: Talep türleri bulunamadı. Önce SeedData.sql çalıştırın.';
    RETURN;
END

INSERT INTO RequestPeriods (SemesterId, RequestTypeId, StartDate, EndDate, IsDemo)
VALUES
    (@currentSemesterId, @rt02, @today, @endDate, 1),
    (@currentSemesterId, @rt05, @today, @endDate, 1),
    (@currentSemesterId, @rt06, @today, @endDate, 1),
    (@currentSemesterId, @rt14, @today, @endDate, 1);

PRINT N'Demo dönemleri eklendi.';
PRINT N'  Dönem SemesterId : ' + CAST(@currentSemesterId AS NVARCHAR(10));
PRINT N'  Başlangıç        : ' + CONVERT(NVARCHAR(10), @today,   120);
PRINT N'  Bitiş            : ' + CONVERT(NVARCHAR(10), @endDate, 120);
PRINT N'Kapatmak için CloseRequestPeriodsDemo.sql çalıştırın.';
