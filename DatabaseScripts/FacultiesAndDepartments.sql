-- FacultiesAndDepartments.sql
-- Idempotent: inserts missing rows and corrects names if different.
-- Run with: sqlcmd -S EYMEN -d OgrenciDanismanlikDB -i DatabaseScripts\FacultiesAndDepartments.sql -f 65001

SET NOCOUNT ON;

-- ── Faculty: Teknoloji Fakültesi ──────────────────────────────────
-- If the table is empty, insert. If one row exists, correct its name.
-- Multiple faculties are left untouched; only the one named correctly is kept.

DECLARE @FacultyId INT = (
    SELECT FacultyId
    FROM   Faculties
    WHERE  FacultyName = N'Teknoloji Fakültesi'
);

IF @FacultyId IS NULL
BEGIN
    -- No correctly-named faculty found.
    IF (SELECT COUNT(*) FROM Faculties) = 0
    BEGIN
        -- Empty table — insert fresh.
        INSERT INTO Faculties (FacultyName) VALUES (N'Teknoloji Fakültesi');
        SET @FacultyId = SCOPE_IDENTITY();
        PRINT N'Fakülte eklendi: Teknoloji Fakültesi (ID=' + CAST(@FacultyId AS NVARCHAR) + N')';
    END
    ELSE
    BEGIN
        -- At least one faculty exists but none named correctly — fix the first one.
        SELECT @FacultyId = MIN(FacultyId) FROM Faculties;
        UPDATE Faculties
        SET    FacultyName = N'Teknoloji Fakültesi'
        WHERE  FacultyId   = @FacultyId;
        PRINT N'Fakülte adı düzeltildi (ID=' + CAST(@FacultyId AS NVARCHAR) + N')';
    END
END
ELSE
BEGIN
    PRINT N'Fakülte zaten doğru: Teknoloji Fakültesi (ID=' + CAST(@FacultyId AS NVARCHAR) + N')';
END

-- ── Departments ──────────────────────────────────────────────────

IF NOT EXISTS (
    SELECT 1 FROM Departments
    WHERE  FacultyId      = @FacultyId
      AND  DepartmentName = N'Bilgisayar Mühendisliği'
)
BEGIN
    INSERT INTO Departments (DepartmentName, FacultyId)
    VALUES (N'Bilgisayar Mühendisliği', @FacultyId);
    PRINT N'Bölüm eklendi: Bilgisayar Mühendisliği';
END
ELSE
    PRINT N'Bölüm zaten mevcut: Bilgisayar Mühendisliği';

IF NOT EXISTS (
    SELECT 1 FROM Departments
    WHERE  FacultyId      = @FacultyId
      AND  DepartmentName = N'Elektrik-Elektronik Mühendisliği'
)
BEGIN
    INSERT INTO Departments (DepartmentName, FacultyId)
    VALUES (N'Elektrik-Elektronik Mühendisliği', @FacultyId);
    PRINT N'Bölüm eklendi: Elektrik-Elektronik Mühendisliği';
END
ELSE
    PRINT N'Bölüm zaten mevcut: Elektrik-Elektronik Mühendisliği';

IF NOT EXISTS (
    SELECT 1 FROM Departments
    WHERE  FacultyId      = @FacultyId
      AND  DepartmentName = N'Makine Mühendisliği'
)
BEGIN
    INSERT INTO Departments (DepartmentName, FacultyId)
    VALUES (N'Makine Mühendisliği', @FacultyId);
    PRINT N'Bölüm eklendi: Makine Mühendisliği';
END
ELSE
    PRINT N'Bölüm zaten mevcut: Makine Mühendisliği';

IF NOT EXISTS (
    SELECT 1 FROM Departments
    WHERE  FacultyId      = @FacultyId
      AND  DepartmentName = N'Mekatronik Mühendisliği'
)
BEGIN
    INSERT INTO Departments (DepartmentName, FacultyId)
    VALUES (N'Mekatronik Mühendisliği', @FacultyId);
    PRINT N'Bölüm eklendi: Mekatronik Mühendisliği';
END
ELSE
    PRINT N'Bölüm zaten mevcut: Mekatronik Mühendisliği';

-- ── Verification ─────────────────────────────────────────────────
PRINT N'';
PRINT N'Mevcut fakülteler:';
SELECT
    FacultyId,
    FacultyName,
    UNICODE(SUBSTRING(FacultyName, 1, 1)) AS FirstCharUnicode
FROM Faculties
ORDER BY FacultyId;

PRINT N'';
PRINT N'Mevcut bölümler (Teknoloji Fakültesi):';
SELECT
    DepartmentId,
    DepartmentName,
    UNICODE(SUBSTRING(DepartmentName, 1, 1)) AS FirstCharUnicode
FROM Departments
WHERE FacultyId = @FacultyId
ORDER BY DepartmentName;
