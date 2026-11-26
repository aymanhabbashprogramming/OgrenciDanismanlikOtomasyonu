-- OfficeHoursDemo.sql
-- Adds 2 office-hour slots per active demo advisor for the current semester.
-- Idempotent: safe to run multiple times.
-- Run with: sqlcmd -S EYMEN -d OgrenciDanismanlikDB -i DatabaseScripts/OfficeHoursDemo.sql -f 65001

SET NOCOUNT ON;
SET XACT_ABORT ON;

DECLARE @CurrentSemesterId INT = (
    SELECT TOP 1 SemesterId FROM Semesters
    WHERE IsCurrent = 1
       OR (CAST(GETDATE() AS DATE) BETWEEN StartDate AND EndDate)
    ORDER BY IsCurrent DESC, StartDate DESC
);

IF @CurrentSemesterId IS NULL
BEGIN
    PRINT N'HATA: Aktif dönem bulunamadı. Script durduruldu.';
    RETURN;
END

PRINT N'Aktif dönem: ' + CAST(@CurrentSemesterId AS NVARCHAR(10));

-- ── Teardown: delete only our demo advisors' rows in the current semester ──

BEGIN TRANSACTION;

DELETE FROM OfficeHours
WHERE SemesterId = @CurrentSemesterId
  AND AdvisorId IN (
      SELECT AdvisorId FROM Advisors
      WHERE SicilNo IN (
          N'10001',
          N'11001', N'11002', N'11003', N'11004', N'11005',
          N'12001', N'12002', N'12003',
          N'13001', N'13002', N'13003',
          N'14001', N'14002', N'14003'
      )
  );

COMMIT TRANSACTION;
PRINT N'Önceki görüşme saatleri temizlendi (ya da zaten yoktu).';

-- ── Insert two slots per active advisor ──────────────────────────────────────

DECLARE @AdvisorAhmet   INT = (SELECT advisor.AdvisorId FROM Advisors AS advisor
                                INNER JOIN Users AS advisorUser ON advisor.UserId = advisorUser.UserId
                                WHERE advisorUser.Username = N'ahmet');
DECLARE @Advisor11001   INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'11001');
DECLARE @Advisor11002   INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'11002');
DECLARE @Advisor11003   INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'11003');
DECLARE @Advisor11004   INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'11004');
DECLARE @Advisor11005   INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'11005');
DECLARE @Advisor12001   INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'12001');
DECLARE @Advisor12002   INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'12002');
DECLARE @Advisor12003   INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'12003');
DECLARE @Advisor13001   INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'13001');
DECLARE @Advisor13002   INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'13002');
DECLARE @Advisor13003   INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'13003');
DECLARE @Advisor14001   INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'14001');
DECLARE @Advisor14002   INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'14002');
DECLARE @Advisor14003   INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'14003');

BEGIN TRANSACTION;

INSERT INTO OfficeHours (AdvisorId, DayOfWeek, StartTime, EndTime, Location, SemesterId) VALUES
-- ahmet (Bilgisayar, Dr. Öğr. Üyesi)
(@AdvisorAhmet,  N'Salı',     '10:00', '12:00', N'Teknoloji Fakültesi, A-101', @CurrentSemesterId),
(@AdvisorAhmet,  N'Perşembe', '14:00', '16:00', N'Teknoloji Fakültesi, A-101', @CurrentSemesterId),

-- Mehmet Çelik (Prof. Dr.)
(@Advisor11001,  N'Pazartesi', '09:00', '11:00', N'Teknoloji Fakültesi, A-203', @CurrentSemesterId),
(@Advisor11001,  N'Çarşamba',  '13:00', '15:00', N'Teknoloji Fakültesi, A-203', @CurrentSemesterId),

-- Ayşe Kaya (Doç. Dr.)
(@Advisor11002,  N'Salı',      '11:00', '13:00', N'Teknoloji Fakültesi, B-115', @CurrentSemesterId),
(@Advisor11002,  N'Perşembe',  '10:00', '12:00', N'Teknoloji Fakültesi, B-115', @CurrentSemesterId),

-- Fatih Demir (Dr. Öğr. Üyesi)
(@Advisor11003,  N'Pazartesi', '14:00', '16:00', N'Teknoloji Fakültesi, B-204', @CurrentSemesterId),
(@Advisor11003,  N'Cuma',      '09:00', '11:00', N'Teknoloji Fakültesi, B-204', @CurrentSemesterId),

-- Zeynep Arslan (Dr. Öğr. Üyesi)
(@Advisor11004,  N'Çarşamba',  '10:00', '12:00', N'Teknoloji Fakültesi, C-312', @CurrentSemesterId),
(@Advisor11004,  N'Cuma',      '13:00', '15:00', N'Teknoloji Fakültesi, C-312', @CurrentSemesterId),

-- Mustafa Şahin (Öğr. Gör. Dr.)
(@Advisor11005,  N'Salı',      '14:00', '16:00', N'Teknoloji Fakültesi, C-408', @CurrentSemesterId),
(@Advisor11005,  N'Perşembe',  '09:00', '11:00', N'Teknoloji Fakültesi, C-408', @CurrentSemesterId),

-- Ali Yıldız (Prof. Dr., Elektrik)
(@Advisor12001,  N'Pazartesi', '10:00', '12:00', N'Teknoloji Fakültesi, D-101', @CurrentSemesterId),
(@Advisor12001,  N'Çarşamba',  '14:00', '16:00', N'Teknoloji Fakültesi, D-101', @CurrentSemesterId),

-- Sevda Konak (Doç. Dr., Elektrik)
(@Advisor12002,  N'Salı',      '09:00', '11:00', N'Teknoloji Fakültesi, D-205', @CurrentSemesterId),
(@Advisor12002,  N'Perşembe',  '13:00', '15:00', N'Teknoloji Fakültesi, D-205', @CurrentSemesterId),

-- Barış Gül (Dr. Öğr. Üyesi, Elektrik)
(@Advisor12003,  N'Çarşamba',  '09:00', '11:00', N'Teknoloji Fakültesi, D-312', @CurrentSemesterId),
(@Advisor12003,  N'Cuma',      '14:00', '16:00', N'Teknoloji Fakültesi, D-312', @CurrentSemesterId),

-- İbrahim Polat (Prof. Dr., Makine)
(@Advisor13001,  N'Pazartesi', '13:00', '15:00', N'Teknoloji Fakültesi, E-110', @CurrentSemesterId),
(@Advisor13001,  N'Salı',      '10:00', '12:00', N'Teknoloji Fakültesi, E-110', @CurrentSemesterId),

-- Esra Aktaş (Doç. Dr., Makine)
(@Advisor13002,  N'Çarşamba',  '11:00', '13:00', N'Teknoloji Fakültesi, E-215', @CurrentSemesterId),
(@Advisor13002,  N'Cuma',      '10:00', '12:00', N'Teknoloji Fakültesi, E-215', @CurrentSemesterId),

-- Caner Koç (Dr. Öğr. Üyesi, Makine)
(@Advisor13003,  N'Salı',      '13:00', '15:00', N'Teknoloji Fakültesi, E-308', @CurrentSemesterId),
(@Advisor13003,  N'Perşembe',  '11:00', '13:00', N'Teknoloji Fakültesi, E-308', @CurrentSemesterId),

-- Tolga Duman (Prof. Dr., Mekatronik)
(@Advisor14001,  N'Pazartesi', '11:00', '13:00', N'Teknoloji Fakültesi, F-104', @CurrentSemesterId),
(@Advisor14001,  N'Perşembe',  '15:00', '17:00', N'Teknoloji Fakültesi, F-104', @CurrentSemesterId),

-- Şule Bulut (Dr. Öğr. Üyesi, Mekatronik)
(@Advisor14002,  N'Salı',      '15:00', '17:00', N'Teknoloji Fakültesi, F-209', @CurrentSemesterId),
(@Advisor14002,  N'Cuma',      '11:00', '13:00', N'Teknoloji Fakültesi, F-209', @CurrentSemesterId),

-- Emre Türk (Dr. Öğr. Üyesi, Mekatronik)
(@Advisor14003,  N'Çarşamba',  '15:00', '17:00', N'Teknoloji Fakültesi, F-315', @CurrentSemesterId),
(@Advisor14003,  N'Cuma',      '09:00', '11:00', N'Teknoloji Fakültesi, F-315', @CurrentSemesterId);

COMMIT TRANSACTION;
PRINT N'Görüşme saatleri başarıyla eklendi.';

-- ── Verification ─────────────────────────────────────────────────────────────
SELECT
    ISNULL(advisor.AcademicTitle + N' ', N'') + advisor.FirstName + N' ' + advisor.LastName AS AdvisorFullName,
    officeHour.DayOfWeek,
    CONVERT(NVARCHAR(5), officeHour.StartTime, 108) + N' – ' +
    CONVERT(NVARCHAR(5), officeHour.EndTime, 108) AS Saat,
    officeHour.Location
FROM OfficeHours AS officeHour
INNER JOIN Advisors AS advisor ON officeHour.AdvisorId = advisor.AdvisorId
WHERE officeHour.SemesterId = @CurrentSemesterId
ORDER BY advisor.LastName, advisor.FirstName,
    CASE officeHour.DayOfWeek
        WHEN N'Pazartesi' THEN 1
        WHEN N'Salı'      THEN 2
        WHEN N'Çarşamba'  THEN 3
        WHEN N'Perşembe'  THEN 4
        WHEN N'Cuma'      THEN 5
        ELSE 6
    END;
