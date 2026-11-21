-- Migration: add Email and Phone to DepartmentManagers, seed demo values.
-- Idempotent: safe to run multiple times.
-- Run with: sqlcmd -S EYMEN -d OgrenciDanismanlikDB -i DatabaseScripts/DepartmentManagerContactColumns.sql -f 65001

-- ── 1. Add columns ─────────────────────────────────────────────────────────────
IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'DepartmentManagers' AND COLUMN_NAME = 'Email'
)
    ALTER TABLE DepartmentManagers ADD Email NVARCHAR(100) NULL;

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'DepartmentManagers' AND COLUMN_NAME = 'Phone'
)
    ALTER TABLE DepartmentManagers ADD Phone NVARCHAR(20) NULL;

GO

-- ── 2. Seed demo contact values ────────────────────────────────────────────────
UPDATE DepartmentManager
SET
    Email = N'bilgisayar@selcuk.edu.tr',
    Phone = N'0332 223 4001'
FROM DepartmentManagers AS DepartmentManager
INNER JOIN Users AS ManagerUser ON ManagerUser.UserId = DepartmentManager.UserId
WHERE ManagerUser.Username = N'baskan';

UPDATE DepartmentManager
SET
    Email = N'elektrik@selcuk.edu.tr',
    Phone = N'0332 223 4002'
FROM DepartmentManagers AS DepartmentManager
INNER JOIN Users AS ManagerUser ON ManagerUser.UserId = DepartmentManager.UserId
WHERE ManagerUser.Username = N'baskan.elektrik';

UPDATE DepartmentManager
SET
    Email = N'makine@selcuk.edu.tr',
    Phone = N'0332 223 4003'
FROM DepartmentManagers AS DepartmentManager
INNER JOIN Users AS ManagerUser ON ManagerUser.UserId = DepartmentManager.UserId
WHERE ManagerUser.Username = N'baskan.makine';

UPDATE DepartmentManager
SET
    Email = N'mekatronik@selcuk.edu.tr',
    Phone = N'0332 223 4004'
FROM DepartmentManagers AS DepartmentManager
INNER JOIN Users AS ManagerUser ON ManagerUser.UserId = DepartmentManager.UserId
WHERE ManagerUser.Username = N'baskan.mekatronik';

-- ── 3. Verify ──────────────────────────────────────────────────────────────────
SELECT
    DepartmentManager.FirstName,
    DepartmentManager.LastName,
    DepartmentManager.Email,
    DepartmentManager.Phone,
    ManagerUser.Username
FROM DepartmentManagers AS DepartmentManager
INNER JOIN Users AS ManagerUser ON ManagerUser.UserId = DepartmentManager.UserId
ORDER BY DepartmentManager.ManagerId;
