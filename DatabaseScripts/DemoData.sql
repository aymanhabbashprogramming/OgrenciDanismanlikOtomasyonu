-- DemoData.sql
-- Teardown-first idempotent script: safe to run multiple times.
-- Run with: sqlcmd -S EYMEN -d OgrenciDanismanlikDB -i DatabaseScripts/DemoData.sql -f 65001

SET NOCOUNT ON;
SET XACT_ABORT ON;

-- BCrypt hash of '123456' (BCrypt.Net-Next, work factor 11, verified)
DECLARE @PasswordHash NVARCHAR(100) = N'$2a$11$fVpd8gh94tnqetQc3tOFh.gxA21wicvpeD0r9B2xCxlv9XLAn7t.W';

DECLARE @RoleAdvisor INT = (SELECT RoleId FROM Roles WHERE RoleName = N'Danışman');
DECLARE @RoleStudent INT = (SELECT RoleId FROM Roles WHERE RoleName = N'Öğrenci');
DECLARE @RoleManager INT = (SELECT RoleId FROM Roles WHERE RoleName = N'Bölüm Başkanlığı');

DECLARE @DeptBilgisayar  INT = 1;
DECLARE @DeptElektrik    INT = 2;
DECLARE @DeptMakine      INT = 3;
DECLARE @DeptMekatronik  INT = 4;

-- ── Teardown: remove only the rows this script owns ──────────────────────────
-- Specific SicilNos so ahmet (SicilNo unknown) is never touched.
-- Seq filter (>= 2002) so pre-existing students like ali kaya (seq 1520) are kept.
BEGIN TRANSACTION;

-- Students we inserted: dept codes 330-333, seq part >= 2002
DROP TABLE IF EXISTS #DemoStudentUserIds;
SELECT UserId
INTO #DemoStudentUserIds
FROM Students
WHERE (   StudentNumber LIKE N'__330____'
       OR StudentNumber LIKE N'__331____'
       OR StudentNumber LIKE N'__332____'
       OR StudentNumber LIKE N'__333____')
  AND TRY_CAST(RIGHT(StudentNumber, 4) AS INT) >= 2002;

DELETE FROM AdvisorAssignments
WHERE StudentId IN (
    SELECT StudentId FROM Students
    WHERE UserId IN (SELECT UserId FROM #DemoStudentUserIds)
);
DELETE FROM StudentEnrollmentStatuses
WHERE StudentId IN (
    SELECT StudentId FROM Students
    WHERE UserId IN (SELECT UserId FROM #DemoStudentUserIds)
);
-- Delete Students rows first (FK → Users)
DELETE FROM Students  WHERE UserId IN (SELECT UserId FROM #DemoStudentUserIds);
DELETE FROM Users     WHERE UserId IN (SELECT UserId FROM #DemoStudentUserIds);
DROP TABLE IF EXISTS #DemoStudentUserIds;

-- Orphaned student-role Users whose Students row was never created by a crashed run
DELETE FROM Users
WHERE RoleId = @RoleStudent
  AND UserId NOT IN (SELECT UserId FROM Students)
  AND (   Username LIKE N'__330____'
       OR Username LIKE N'__331____'
       OR Username LIKE N'__332____'
       OR Username LIKE N'__333____')
  AND TRY_CAST(RIGHT(Username, 4) AS INT) >= 2002;

-- Advisors we inserted: exact SicilNos only (never touches ahmet or other pre-existing advisors)
DECLARE @OurSicilNos TABLE (SicilNo NVARCHAR(20));
INSERT INTO @OurSicilNos VALUES
(N'11001'),(N'11002'),(N'11003'),(N'11004'),(N'11005'),(N'11006'),
(N'12001'),(N'12002'),(N'12003'),
(N'13001'),(N'13002'),(N'13003'),
(N'14001'),(N'14002'),(N'14003');

DELETE FROM AdvisorAssignments
WHERE AdvisorId IN (SELECT AdvisorId FROM Advisors WHERE SicilNo IN (SELECT SicilNo FROM @OurSicilNos));
DELETE FROM Advisors  WHERE SicilNo IN (SELECT SicilNo FROM @OurSicilNos);
DELETE FROM Users WHERE Username IN (
    N'mehmet.celik', N'ayse.kaya', N'fatih.demir', N'zeynep.arslan',
    N'mustafa.sahin', N'hulya.ozturk',
    N'ali.yildiz', N'sevda.konak', N'baris.gul',
    N'ibrahim.polat', N'esra.aktas', N'caner.koc',
    N'tolga.duman', N'sule.bulut', N'emre.turk'
);

-- Department managers we inserted
DELETE FROM DepartmentManagers
WHERE UserId IN (
    SELECT UserId FROM Users
    WHERE Username IN (N'baskan.elektrik', N'baskan.makine', N'baskan.mekatronik')
);
DELETE FROM DepartmentManagers WHERE UserId = 1;
DELETE FROM Users WHERE Username IN (
    N'baskan.elektrik', N'baskan.makine', N'baskan.mekatronik'
);

COMMIT TRANSACTION;
PRINT N'Önceki demo verisi temizlendi (ya da zaten yoktu).';

-- ── Main insert transaction ───────────────────────────────────────────────────
BEGIN TRANSACTION;

-- ── 1. Restore ahmet's Advisor row if a previous bad run deleted it ──────────
IF NOT EXISTS (
    SELECT 1 FROM Advisors AS advisor
    INNER JOIN Users AS advisorUser ON advisor.UserId = advisorUser.UserId
    WHERE advisorUser.Username = N'ahmet'
)
BEGIN
    DECLARE @AhmetUserId INT = (SELECT UserId FROM Users WHERE Username = N'ahmet');
    IF @AhmetUserId IS NOT NULL
        INSERT INTO Advisors (UserId, SicilNo, AcademicTitle, FirstName, LastName, DepartmentId)
        VALUES (@AhmetUserId, N'10001', N'Dr. Öğr. Üyesi', N'Ahmet', N'Yılmaz', @DeptBilgisayar);
END

-- ── 2. Link existing 'baskan' (UserId=1) to Bilgisayar ──────────────────────
IF NOT EXISTS (SELECT 1 FROM DepartmentManagers WHERE UserId = 1)
    INSERT INTO DepartmentManagers (UserId, DepartmentId, FirstName, LastName, Position)
    VALUES (1, @DeptBilgisayar, N'Berk', N'Karahan', N'Bölüm Başkanı');

-- ── 3. New department managers ───────────────────────────────────────────────
INSERT INTO Users (Username, PasswordHash, RoleId, IsActive, CreatedAt) VALUES
(N'baskan.elektrik',   @PasswordHash, @RoleManager, 1, SYSDATETIME()),
(N'baskan.makine',     @PasswordHash, @RoleManager, 1, SYSDATETIME()),
(N'baskan.mekatronik', @PasswordHash, @RoleManager, 1, SYSDATETIME());

DECLARE @UserElektrikManager   INT = (SELECT UserId FROM Users WHERE Username = N'baskan.elektrik');
DECLARE @UserMakineManager     INT = (SELECT UserId FROM Users WHERE Username = N'baskan.makine');
DECLARE @UserMekatronikManager INT = (SELECT UserId FROM Users WHERE Username = N'baskan.mekatronik');

INSERT INTO DepartmentManagers (UserId, DepartmentId, FirstName, LastName, Position) VALUES
(@UserElektrikManager,   @DeptElektrik,   N'Serhat', N'Bozkurt', N'Bölüm Başkanı'),
(@UserMakineManager,     @DeptMakine,     N'Leyla',  N'Çınar',   N'Bölüm Başkanı'),
(@UserMekatronikManager, @DeptMekatronik, N'Kemal',  N'Erdoğan', N'Bölüm Başkanı');

-- ── 4. Advisors for Bilgisayar (5 new active + 1 inactive) ──────────────────
INSERT INTO Users (Username, PasswordHash, RoleId, IsActive, CreatedAt) VALUES
(N'mehmet.celik',  @PasswordHash, @RoleAdvisor, 1, SYSDATETIME()),
(N'ayse.kaya',     @PasswordHash, @RoleAdvisor, 1, SYSDATETIME()),
(N'fatih.demir',   @PasswordHash, @RoleAdvisor, 1, SYSDATETIME()),
(N'zeynep.arslan', @PasswordHash, @RoleAdvisor, 1, SYSDATETIME()),
(N'mustafa.sahin', @PasswordHash, @RoleAdvisor, 1, SYSDATETIME()),
(N'hulya.ozturk',  @PasswordHash, @RoleAdvisor, 0, SYSDATETIME());

DECLARE @UMehmet  INT = (SELECT UserId FROM Users WHERE Username = N'mehmet.celik');
DECLARE @UAyse    INT = (SELECT UserId FROM Users WHERE Username = N'ayse.kaya');
DECLARE @UFatih   INT = (SELECT UserId FROM Users WHERE Username = N'fatih.demir');
DECLARE @UZeynep  INT = (SELECT UserId FROM Users WHERE Username = N'zeynep.arslan');
DECLARE @UMustafa INT = (SELECT UserId FROM Users WHERE Username = N'mustafa.sahin');
DECLARE @UHulya   INT = (SELECT UserId FROM Users WHERE Username = N'hulya.ozturk');

INSERT INTO Advisors (UserId, SicilNo, AcademicTitle, FirstName, LastName, DepartmentId) VALUES
(@UMehmet,  N'11001', N'Prof. Dr.',       N'Mehmet',  N'Çelik',  @DeptBilgisayar),
(@UAyse,    N'11002', N'Doç. Dr.',        N'Ayşe',    N'Kaya',   @DeptBilgisayar),
(@UFatih,   N'11003', N'Dr. Öğr. Üyesi', N'Fatih',   N'Demir',  @DeptBilgisayar),
(@UZeynep,  N'11004', N'Dr. Öğr. Üyesi', N'Zeynep',  N'Arslan', @DeptBilgisayar),
(@UMustafa, N'11005', N'Öğr. Gör. Dr.',  N'Mustafa', N'Şahin',  @DeptBilgisayar),
(@UHulya,   N'11006', N'Doç. Dr.',       N'Hülya',   N'Öztürk', @DeptBilgisayar);

DECLARE @AdvisorIdMehmet  INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'11001');
DECLARE @AdvisorIdAyse    INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'11002');
DECLARE @AdvisorIdFatih   INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'11003');
DECLARE @AdvisorIdZeynep  INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'11004');
DECLARE @AdvisorIdMustafa INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'11005');
DECLARE @AdvisorIdHulya   INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'11006');
DECLARE @AdvisorIdAhmet   INT = (
    SELECT advisor.AdvisorId
    FROM Advisors AS advisor
    INNER JOIN Users AS advisorUser ON advisor.UserId = advisorUser.UserId
    WHERE advisorUser.Username = N'ahmet'
);
PRINT N'ahmet AdvisorId = ' + ISNULL(CAST(@AdvisorIdAhmet AS NVARCHAR(10)), N'NULL — ahmet bulunamadı!');

-- ── 5. Advisors for Elektrik, Makine, Mekatronik (3 each) ───────────────────
INSERT INTO Users (Username, PasswordHash, RoleId, IsActive, CreatedAt) VALUES
(N'ali.yildiz',    @PasswordHash, @RoleAdvisor, 1, SYSDATETIME()),
(N'sevda.konak',   @PasswordHash, @RoleAdvisor, 1, SYSDATETIME()),
(N'baris.gul',     @PasswordHash, @RoleAdvisor, 1, SYSDATETIME()),
(N'ibrahim.polat', @PasswordHash, @RoleAdvisor, 1, SYSDATETIME()),
(N'esra.aktas',    @PasswordHash, @RoleAdvisor, 1, SYSDATETIME()),
(N'caner.koc',     @PasswordHash, @RoleAdvisor, 1, SYSDATETIME()),
(N'tolga.duman',   @PasswordHash, @RoleAdvisor, 1, SYSDATETIME()),
(N'sule.bulut',    @PasswordHash, @RoleAdvisor, 1, SYSDATETIME()),
(N'emre.turk',     @PasswordHash, @RoleAdvisor, 1, SYSDATETIME());

DECLARE @UAliY    INT = (SELECT UserId FROM Users WHERE Username = N'ali.yildiz');
DECLARE @USevda   INT = (SELECT UserId FROM Users WHERE Username = N'sevda.konak');
DECLARE @UBaris   INT = (SELECT UserId FROM Users WHERE Username = N'baris.gul');
DECLARE @UIbrahim INT = (SELECT UserId FROM Users WHERE Username = N'ibrahim.polat');
DECLARE @UEsra    INT = (SELECT UserId FROM Users WHERE Username = N'esra.aktas');
DECLARE @UCaner   INT = (SELECT UserId FROM Users WHERE Username = N'caner.koc');
DECLARE @UTolga   INT = (SELECT UserId FROM Users WHERE Username = N'tolga.duman');
DECLARE @USule    INT = (SELECT UserId FROM Users WHERE Username = N'sule.bulut');
DECLARE @UEmre    INT = (SELECT UserId FROM Users WHERE Username = N'emre.turk');

INSERT INTO Advisors (UserId, SicilNo, AcademicTitle, FirstName, LastName, DepartmentId) VALUES
-- Elektrik
(@UAliY,    N'12001', N'Prof. Dr.',       N'Ali',     N'Yıldız', @DeptElektrik),
(@USevda,   N'12002', N'Doç. Dr.',        N'Sevda',   N'Konak',  @DeptElektrik),
(@UBaris,   N'12003', N'Dr. Öğr. Üyesi', N'Barış',   N'Gül',    @DeptElektrik),
-- Makine
(@UIbrahim, N'13001', N'Prof. Dr.',       N'İbrahim', N'Polat',  @DeptMakine),
(@UEsra,    N'13002', N'Doç. Dr.',        N'Esra',    N'Aktaş',  @DeptMakine),
(@UCaner,   N'13003', N'Dr. Öğr. Üyesi', N'Caner',   N'Koç',    @DeptMakine),
-- Mekatronik
(@UTolga,   N'14001', N'Prof. Dr.',       N'Tolga',   N'Duman',  @DeptMekatronik),
(@USule,    N'14002', N'Dr. Öğr. Üyesi', N'Şule',    N'Bulut',  @DeptMekatronik),
(@UEmre,    N'14003', N'Dr. Öğr. Üyesi', N'Emre',    N'Türk',   @DeptMekatronik);

DECLARE @AdvisorIdAliY    INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'12001');
DECLARE @AdvisorIdSevda   INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'12002');
DECLARE @AdvisorIdBaris   INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'12003');
DECLARE @AdvisorIdIbrahim INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'13001');
DECLARE @AdvisorIdEsra    INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'13002');
DECLARE @AdvisorIdCaner   INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'13003');
DECLARE @AdvisorIdTolga   INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'14001');
DECLARE @AdvisorIdSule    INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'14002');
DECLARE @AdvisorIdEmre    INT = (SELECT AdvisorId FROM Advisors WHERE SicilNo = N'14003');

-- ── 6. Students ──────────────────────────────────────────────────────────────
-- One big temp table of all 1041 students; joined back via Username=StudentNum.
-- Student number: YY(2) + dept_code(3) + seq(4) = 9 digits, seq starts at 2002.
-- Advisor groups:
--   Bilgisayar: 1=mehmet(95) 2=ayse(78) 3=fatih(72) 4=zeynep(68) 5=mustafa(64)
--               6=ahmet(60)  7=hulya(11) 0=no advisor(3)
--   Elektrik:   11=ali.yildiz(70) 12=sevda.konak(65) 13=baris.gul(60)
--   Makine:     21=ibrahim.polat(75) 22=esra.aktas(65) 23=caner.koc(60)
--   Mekatronik: 31=tolga.duman(70) 32=sule.bulut(65) 33=emre.turk(60)

DROP TABLE IF EXISTS #AllStudents;
WITH FirstNames AS (
    SELECT seq, name FROM (VALUES
        ( 1, N'Ahmet'),    ( 2, N'Mehmet'),   ( 3, N'Mustafa'),  ( 4, N'Ali'),
        ( 5, N'Fatih'),    ( 6, N'Ömer'),     ( 7, N'İbrahim'),  ( 8, N'Hasan'),
        ( 9, N'Hüseyin'),  (10, N'Murat'),    (11, N'Emre'),     (12, N'Burak'),
        (13, N'Serkan'),   (14, N'Tolga'),    (15, N'Cem'),      (16, N'Oğuz'),
        (17, N'Kerem'),    (18, N'Furkan'),   (19, N'Volkan'),   (20, N'Erhan'),
        (21, N'Ayşe'),     (22, N'Fatma'),    (23, N'Zeynep'),   (24, N'Elif'),
        (25, N'Hatice'),   (26, N'Merve'),    (27, N'Büşra'),    (28, N'Seda'),
        (29, N'Gül'),      (30, N'Sibel'),    (31, N'Cansu'),    (32, N'Neslihan'),
        (33, N'Derya'),    (34, N'Pınar'),    (35, N'Yasemin'),  (36, N'Songül'),
        (37, N'Melek'),    (38, N'Gizem'),    (39, N'Şeyma'),    (40, N'Bahar')
    ) AS T(seq, name)
),
LastNames AS (
    SELECT seq, name FROM (VALUES
        ( 1, N'Yılmaz'),  ( 2, N'Kaya'),     ( 3, N'Demir'),    ( 4, N'Çelik'),
        ( 5, N'Şahin'),   ( 6, N'Arslan'),   ( 7, N'Doğan'),    ( 8, N'Kılıç'),
        ( 9, N'Aslan'),   (10, N'Çetin'),    (11, N'Aydın'),    (12, N'Özdemir'),
        (13, N'Erdoğan'), (14, N'Kurt'),     (15, N'Öztürk'),   (16, N'Yıldız'),
        (17, N'Güneş'),   (18, N'Polat'),    (19, N'Aktaş'),    (20, N'Koç'),
        (21, N'Türk'),    (22, N'Bozkurt'),  (23, N'Işık'),     (24, N'Duman'),
        (25, N'Bulut'),   (26, N'Çınar'),    (27, N'Karahan'),  (28, N'Tekin'),
        (29, N'Keskin'),  (30, N'Korkmaz'),  (31, N'Bayrak'),   (32, N'Pekdemir'),
        (33, N'Çakır'),   (34, N'Topal'),    (35, N'Sarı')
    ) AS T(seq, name)
),
Pairs AS (
    SELECT
        ROW_NUMBER() OVER (ORDER BY FirstNames.seq, LastNames.seq) AS PairNum,
        FirstNames.name AS FirstName,
        LastNames.name  AS LastName
    FROM FirstNames CROSS JOIN LastNames
)
SELECT
    PairNum,
    FirstName,
    LastName,
    CASE
        WHEN PairNum <=  451 THEN @DeptBilgisayar
        WHEN PairNum <=  646 THEN @DeptElektrik
        WHEN PairNum <=  846 THEN @DeptMakine
        ELSE                      @DeptMekatronik
    END AS DepartmentId,
    CASE
        WHEN PairNum <=  451 THEN
            CAST(22 + (PairNum-1)%4   AS NVARCHAR(2)) + N'330'
            + RIGHT(N'0000' + CAST(2001 + PairNum AS NVARCHAR(4)), 4)
        WHEN PairNum <=  646 THEN
            CAST(22 + (PairNum-452)%4 AS NVARCHAR(2)) + N'331'
            + RIGHT(N'0000' + CAST(2001 + (PairNum-451)  AS NVARCHAR(4)), 4)
        WHEN PairNum <=  846 THEN
            CAST(22 + (PairNum-647)%4 AS NVARCHAR(2)) + N'332'
            + RIGHT(N'0000' + CAST(2001 + (PairNum-646)  AS NVARCHAR(4)), 4)
        ELSE
            CAST(22 + (PairNum-847)%4 AS NVARCHAR(2)) + N'333'
            + RIGHT(N'0000' + CAST(2001 + (PairNum-846)  AS NVARCHAR(4)), 4)
    END AS StudentNum,
    -- Advisor group
    CASE
        -- Bilgisayar (PairNum 1–451)
        WHEN PairNum <=  95  THEN 1   -- mehmet  (95)
        WHEN PairNum <= 173  THEN 2   -- ayse    (78)
        WHEN PairNum <= 245  THEN 3   -- fatih   (72)
        WHEN PairNum <= 313  THEN 4   -- zeynep  (68)
        WHEN PairNum <= 377  THEN 5   -- mustafa (64)
        WHEN PairNum <= 437  THEN 6   -- ahmet   (60)
        WHEN PairNum <= 448  THEN 7   -- hulya   (11)
        WHEN PairNum <= 451  THEN 0   -- no advisor (3)
        -- Elektrik (PairNum 452–646)
        WHEN PairNum <= 521  THEN 11  -- ali.yildiz   (70)
        WHEN PairNum <= 586  THEN 12  -- sevda.konak  (65)
        WHEN PairNum <= 646  THEN 13  -- baris.gul    (60)
        -- Makine (PairNum 647–846)
        WHEN PairNum <= 721  THEN 21  -- ibrahim.polat (75)
        WHEN PairNum <= 786  THEN 22  -- esra.aktas    (65)
        WHEN PairNum <= 846  THEN 23  -- caner.koc     (60)
        -- Mekatronik (PairNum 847–1041)
        WHEN PairNum <= 916  THEN 31  -- tolga.duman   (70)
        WHEN PairNum <= 981  THEN 32  -- sule.bulut    (65)
        ELSE                     33   -- emre.turk     (60)
    END AS AdvisorGroup,
    (PairNum - 1) % 4 + 1 AS ClassYear,
    CASE
        WHEN PairNum % 10 = 0        THEN N'Uzaktan'
        WHEN PairNum % 10 IN (1, 2)  THEN N'İkinci'
        ELSE                              N'Örgün'
    END AS EducationType,
    CAST(CASE WHEN PairNum % 20 = 0 THEN 1 ELSE 0 END AS BIT) AS IsInternational
INTO #AllStudents
FROM Pairs
WHERE PairNum <= 1041;

-- Insert all student Users at once
INSERT INTO Users (Username, PasswordHash, RoleId, IsActive, CreatedAt)
SELECT StudentNum, @PasswordHash, @RoleStudent, 1, SYSDATETIME()
FROM #AllStudents
ORDER BY PairNum;

-- Insert Students rows, joining back via Username = StudentNum to get UserId
INSERT INTO Students (UserId, StudentNumber, FirstName, LastName, NationalId,
                      DepartmentId, EducationType, ClassYear, IsInternational)
SELECT
    u.UserId,
    temp.StudentNum,
    temp.FirstName,
    temp.LastName,
    N'10' + temp.StudentNum,   -- unique 11-char fake TC: '10' + 9-digit student number
    temp.DepartmentId,
    temp.EducationType,
    temp.ClassYear,
    temp.IsInternational
FROM #AllStudents AS temp
INNER JOIN Users AS u ON u.Username = temp.StudentNum;

-- Enrollment statuses
INSERT INTO StudentEnrollmentStatuses (StudentId, Status, StartDate, EndDate, Note)
SELECT student.StudentId, N'Aktif', CAST(GETDATE() AS DATE), NULL, NULL
FROM #AllStudents AS temp
INNER JOIN Users AS u ON u.Username = temp.StudentNum
INNER JOIN Students AS student ON student.UserId = u.UserId;

-- Advisor assignments (group 0 = no advisor, skip those; NULL advisor ID also skipped)
INSERT INTO AdvisorAssignments (StudentId, AdvisorId, StartDate, EndDate, ChangeReason)
SELECT
    student.StudentId,
    CASE temp.AdvisorGroup
        WHEN  1 THEN @AdvisorIdMehmet
        WHEN  2 THEN @AdvisorIdAyse
        WHEN  3 THEN @AdvisorIdFatih
        WHEN  4 THEN @AdvisorIdZeynep
        WHEN  5 THEN @AdvisorIdMustafa
        WHEN  6 THEN @AdvisorIdAhmet
        WHEN  7 THEN @AdvisorIdHulya
        WHEN 11 THEN @AdvisorIdAliY
        WHEN 12 THEN @AdvisorIdSevda
        WHEN 13 THEN @AdvisorIdBaris
        WHEN 21 THEN @AdvisorIdIbrahim
        WHEN 22 THEN @AdvisorIdEsra
        WHEN 23 THEN @AdvisorIdCaner
        WHEN 31 THEN @AdvisorIdTolga
        WHEN 32 THEN @AdvisorIdSule
        WHEN 33 THEN @AdvisorIdEmre
    END AS AdvisorId,
    DATEADD(DAY, -(365 * temp.ClassYear + temp.PairNum % 90), CAST(GETDATE() AS DATE)),
    NULL,
    NULL
FROM #AllStudents AS temp
INNER JOIN Users AS u ON u.Username = temp.StudentNum
INNER JOIN Students AS student ON student.UserId = u.UserId
WHERE temp.AdvisorGroup <> 0
  AND CASE temp.AdvisorGroup
          WHEN  1 THEN @AdvisorIdMehmet  WHEN  2 THEN @AdvisorIdAyse
          WHEN  3 THEN @AdvisorIdFatih   WHEN  4 THEN @AdvisorIdZeynep
          WHEN  5 THEN @AdvisorIdMustafa WHEN  6 THEN @AdvisorIdAhmet
          WHEN  7 THEN @AdvisorIdHulya   WHEN 11 THEN @AdvisorIdAliY
          WHEN 12 THEN @AdvisorIdSevda   WHEN 13 THEN @AdvisorIdBaris
          WHEN 21 THEN @AdvisorIdIbrahim WHEN 22 THEN @AdvisorIdEsra
          WHEN 23 THEN @AdvisorIdCaner   WHEN 31 THEN @AdvisorIdTolga
          WHEN 32 THEN @AdvisorIdSule    WHEN 33 THEN @AdvisorIdEmre
      END IS NOT NULL;

COMMIT TRANSACTION;

-- ── Verification ──────────────────────────────────────────────────────────────
PRINT N'';
PRINT N'Demo verisi yüklendi. Özet:';

SELECT
    dept.DepartmentName,
    COUNT(DISTINCT advisor.AdvisorId)  AS DanismanSayisi,
    COUNT(DISTINCT student.StudentId)  AS OgrenciSayisi
FROM Departments AS dept
LEFT JOIN Advisors AS advisor ON advisor.DepartmentId = dept.DepartmentId
LEFT JOIN Students AS student ON student.DepartmentId = dept.DepartmentId
WHERE dept.DepartmentId BETWEEN 1 AND 4
GROUP BY dept.DepartmentId, dept.DepartmentName
ORDER BY dept.DepartmentId;

PRINT N'';
PRINT N'Danışman istatistikleri (Bilgisayar):';
SELECT
    ISNULL(advisor.AcademicTitle + N' ', N'') + advisor.FirstName + N' ' + advisor.LastName AS AdSoyad,
    advisorUser.IsActive,
    COUNT(assignment.AssignmentId) AS OgrenciSayisi
FROM Advisors AS advisor
INNER JOIN Users AS advisorUser ON advisor.UserId = advisorUser.UserId
LEFT JOIN AdvisorAssignments AS assignment
    ON assignment.AdvisorId = advisor.AdvisorId AND assignment.EndDate IS NULL
WHERE advisor.DepartmentId = 1
GROUP BY advisor.AdvisorId, advisor.AcademicTitle, advisor.FirstName, advisor.LastName, advisorUser.IsActive
ORDER BY OgrenciSayisi DESC;

PRINT N'';
PRINT N'Türkçe karakter doğrulama (ü = 252 beklenir):';
SELECT dept.DepartmentName,
       UNICODE(SUBSTRING(dept.DepartmentName,
           CHARINDEX(N'ü', LOWER(dept.DepartmentName)), 1)) AS UnicodeU
FROM Departments AS dept
WHERE dept.DepartmentId BETWEEN 1 AND 4;

DROP TABLE IF EXISTS #AllStudents;

PRINT N'Bitti.';
