-- Seed data for OgrenciDanismanlikDB
-- Idempotent: safe to run multiple times

IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = N'Öğrenci')
    INSERT INTO Roles (RoleName) VALUES (N'Öğrenci');

IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = N'Danışman')
    INSERT INTO Roles (RoleName) VALUES (N'Danışman');

IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = N'Bölüm Başkanlığı')
    INSERT INTO Roles (RoleName) VALUES (N'Bölüm Başkanlığı');
