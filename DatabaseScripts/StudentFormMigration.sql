-- Migration: add HousingTypeDescription and DisabilityDescription to StudentFiles
-- Run with: sqlcmd -f 65001 -S <server> -d <database> -i StudentFormMigration.sql

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'StudentFiles')
      AND name = N'HousingTypeDescription'
)
BEGIN
    ALTER TABLE StudentFiles ADD HousingTypeDescription nvarchar(500) NULL;
    PRINT N'Added column StudentFiles.HousingTypeDescription';
END
ELSE
BEGIN
    PRINT N'Column StudentFiles.HousingTypeDescription already exists – skipped.';
END;

IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'StudentFiles')
      AND name = N'DisabilityDescription'
)
BEGIN
    ALTER TABLE StudentFiles ADD DisabilityDescription nvarchar(500) NULL;
    PRINT N'Added column StudentFiles.DisabilityDescription';
END
ELSE
BEGIN
    PRINT N'Column StudentFiles.DisabilityDescription already exists – skipped.';
END;
