-- ============================================================
-- Selçuk Üniversitesi 2026-2027 Ön Lisans-Lisans Akademik Takvimi
-- Kaynak: Senato kararıyla onaylanan resmi akademik takvim
--
-- Önemli tarihler (Güz):
--   Kayıt Yenileme        : 2026-09-07 – 2026-09-13
--   Danışman Onayı        : 2026-09-07 – 2026-09-15
--   Ders Ekleme ve Bırakma: 2026-09-14 – 2026-09-15
--   Derslerin Başlangıcı  : 2026-09-07
--   Derslerin Bitişi      : 2026-01-17
--
-- Önemli tarihler (Bahar):
--   Kayıt Yenileme        : 2027-01-18 – 2027-01-24
--   Danışman Onayı        : 2027-01-18 – 2027-01-26
--   Ders Ekleme ve Bırakma: 2027-01-25 – 2027-01-26
--   Derslerin Başlangıcı  : 2027-01-18
--   Derslerin Bitişi      : 2027-06-15
--
-- Idempotent: safe to run multiple times.
-- Updates existing rows; deletes conflicting manually-added rows.
-- ============================================================

-- ── 1. Remove any conflicting manually-added 2026-2027 semesters
--       that do NOT match the official dates below, to avoid duplicates.
DELETE FROM Semesters
WHERE  AcademicYear = N'2026-2027'
  AND  NOT (
           (Term = N'Güz'   AND StartDate = '2026-09-07' AND EndDate = '2027-01-17')
        OR (Term = N'Bahar' AND StartDate = '2027-01-18' AND EndDate = '2027-06-15')
       );

-- ── 2. Upsert semesters ──────────────────────────────────────────

-- 2026-2027 Güz
IF EXISTS (SELECT 1 FROM Semesters WHERE AcademicYear = N'2026-2027' AND Term = N'Güz')
    UPDATE Semesters
    SET StartDate = '2026-09-07',
        EndDate   = '2027-01-17',
        IsCurrent = 0
    WHERE AcademicYear = N'2026-2027' AND Term = N'Güz';
ELSE
    INSERT INTO Semesters (AcademicYear, Term, StartDate, EndDate, IsCurrent)
    VALUES (N'2026-2027', N'Güz', '2026-09-07', '2027-01-17', 0);

-- 2026-2027 Bahar
IF EXISTS (SELECT 1 FROM Semesters WHERE AcademicYear = N'2026-2027' AND Term = N'Bahar')
    UPDATE Semesters
    SET StartDate = '2027-01-18',
        EndDate   = '2027-06-15',
        IsCurrent = 0
    WHERE AcademicYear = N'2026-2027' AND Term = N'Bahar';
ELSE
    INSERT INTO Semesters (AcademicYear, Term, StartDate, EndDate, IsCurrent)
    VALUES (N'2026-2027', N'Bahar', '2027-01-18', '2027-06-15', 0);

-- ── 3. Upsert request periods ─────────────────────────────────────
--       Uses SemesterId looked up by (AcademicYear, Term).

DECLARE @guzSemesterId   INT = (SELECT SemesterId FROM Semesters WHERE AcademicYear = N'2026-2027' AND Term = N'Güz');
DECLARE @baharSemesterId INT = (SELECT SemesterId FROM Semesters WHERE AcademicYear = N'2026-2027' AND Term = N'Bahar');

-- Helper: resolve RequestTypeId by FormCode
DECLARE @rt02 INT = (SELECT RequestTypeId FROM RequestTypes WHERE FormCode = N'TF-ÖİŞL-02');
DECLARE @rt05 INT = (SELECT RequestTypeId FROM RequestTypes WHERE FormCode = N'TF-ÖİŞL-05');
DECLARE @rt06 INT = (SELECT RequestTypeId FROM RequestTypes WHERE FormCode = N'TF-ÖİŞL-06');
DECLARE @rt14 INT = (SELECT RequestTypeId FROM RequestTypes WHERE FormCode = N'TF-ÖİŞL-14');

-- Guard: only proceed when all RequestTypes are present
IF @rt02 IS NULL OR @rt05 IS NULL OR @rt06 IS NULL OR @rt14 IS NULL
BEGIN
    RAISERROR(N'RequestTypes not found — run SeedData.sql first.', 16, 1);
    RETURN;
END

-- ── Güz periods ──────────────────────────────────────────────────

-- TF-ÖİŞL-02 Güz: Ders Ekleme ve Bırakma 2026-09-14 – 2026-09-15
IF EXISTS (SELECT 1 FROM RequestPeriods WHERE SemesterId = @guzSemesterId AND RequestTypeId = @rt02)
    UPDATE RequestPeriods SET StartDate = '2026-09-14', EndDate = '2026-09-15'
    WHERE SemesterId = @guzSemesterId AND RequestTypeId = @rt02;
ELSE
    INSERT INTO RequestPeriods (SemesterId, RequestTypeId, StartDate, EndDate)
    VALUES (@guzSemesterId, @rt02, '2026-09-14', '2026-09-15');

-- TF-ÖİŞL-05 Güz: Not Yükseltme 2026-09-07 – 2026-09-15
IF EXISTS (SELECT 1 FROM RequestPeriods WHERE SemesterId = @guzSemesterId AND RequestTypeId = @rt05)
    UPDATE RequestPeriods SET StartDate = '2026-09-07', EndDate = '2026-09-15'
    WHERE SemesterId = @guzSemesterId AND RequestTypeId = @rt05;
ELSE
    INSERT INTO RequestPeriods (SemesterId, RequestTypeId, StartDate, EndDate)
    VALUES (@guzSemesterId, @rt05, '2026-09-07', '2026-09-15');

-- TF-ÖİŞL-06 Güz: Üst Yarıyıldan Ders Alma 2026-09-07 – 2026-09-15
IF EXISTS (SELECT 1 FROM RequestPeriods WHERE SemesterId = @guzSemesterId AND RequestTypeId = @rt06)
    UPDATE RequestPeriods SET StartDate = '2026-09-07', EndDate = '2026-09-15'
    WHERE SemesterId = @guzSemesterId AND RequestTypeId = @rt06;
ELSE
    INSERT INTO RequestPeriods (SemesterId, RequestTypeId, StartDate, EndDate)
    VALUES (@guzSemesterId, @rt06, '2026-09-07', '2026-09-15');

-- TF-ÖİŞL-14 Güz: Bölüm Dışından Ders Alma 2026-09-07 – 2026-09-15
IF EXISTS (SELECT 1 FROM RequestPeriods WHERE SemesterId = @guzSemesterId AND RequestTypeId = @rt14)
    UPDATE RequestPeriods SET StartDate = '2026-09-07', EndDate = '2026-09-15'
    WHERE SemesterId = @guzSemesterId AND RequestTypeId = @rt14;
ELSE
    INSERT INTO RequestPeriods (SemesterId, RequestTypeId, StartDate, EndDate)
    VALUES (@guzSemesterId, @rt14, '2026-09-07', '2026-09-15');

-- ── Bahar periods ─────────────────────────────────────────────────

-- TF-ÖİŞL-02 Bahar: Ders Ekleme ve Bırakma 2027-01-25 – 2027-01-26
IF EXISTS (SELECT 1 FROM RequestPeriods WHERE SemesterId = @baharSemesterId AND RequestTypeId = @rt02)
    UPDATE RequestPeriods SET StartDate = '2027-01-25', EndDate = '2027-01-26'
    WHERE SemesterId = @baharSemesterId AND RequestTypeId = @rt02;
ELSE
    INSERT INTO RequestPeriods (SemesterId, RequestTypeId, StartDate, EndDate)
    VALUES (@baharSemesterId, @rt02, '2027-01-25', '2027-01-26');

-- TF-ÖİŞL-05 Bahar: Not Yükseltme 2027-01-18 – 2027-01-26
IF EXISTS (SELECT 1 FROM RequestPeriods WHERE SemesterId = @baharSemesterId AND RequestTypeId = @rt05)
    UPDATE RequestPeriods SET StartDate = '2027-01-18', EndDate = '2027-01-26'
    WHERE SemesterId = @baharSemesterId AND RequestTypeId = @rt05;
ELSE
    INSERT INTO RequestPeriods (SemesterId, RequestTypeId, StartDate, EndDate)
    VALUES (@baharSemesterId, @rt05, '2027-01-18', '2027-01-26');

-- TF-ÖİŞL-06 Bahar: Üst Yarıyıldan Ders Alma 2027-01-18 – 2027-01-26
IF EXISTS (SELECT 1 FROM RequestPeriods WHERE SemesterId = @baharSemesterId AND RequestTypeId = @rt06)
    UPDATE RequestPeriods SET StartDate = '2027-01-18', EndDate = '2027-01-26'
    WHERE SemesterId = @baharSemesterId AND RequestTypeId = @rt06;
ELSE
    INSERT INTO RequestPeriods (SemesterId, RequestTypeId, StartDate, EndDate)
    VALUES (@baharSemesterId, @rt06, '2027-01-18', '2027-01-26');

-- TF-ÖİŞL-14 Bahar: Bölüm Dışından Ders Alma 2027-01-18 – 2027-01-26
IF EXISTS (SELECT 1 FROM RequestPeriods WHERE SemesterId = @baharSemesterId AND RequestTypeId = @rt14)
    UPDATE RequestPeriods SET StartDate = '2027-01-18', EndDate = '2027-01-26'
    WHERE SemesterId = @baharSemesterId AND RequestTypeId = @rt14;
ELSE
    INSERT INTO RequestPeriods (SemesterId, RequestTypeId, StartDate, EndDate)
    VALUES (@baharSemesterId, @rt14, '2027-01-18', '2027-01-26');

-- ── 4. Verify ────────────────────────────────────────────────────
SELECT s.AcademicYear, s.Term, s.StartDate, s.EndDate,
       rt.FormCode, rt.FormName, rp.StartDate AS PeriodStart, rp.EndDate AS PeriodEnd
FROM   Semesters s
JOIN   RequestPeriods rp ON rp.SemesterId     = s.SemesterId
JOIN   RequestTypes   rt ON rp.RequestTypeId  = rt.RequestTypeId
WHERE  s.AcademicYear = N'2026-2027'
ORDER BY s.StartDate, rt.FormCode;
