-- Seed data for OgrenciDanismanlikDB
-- Idempotent: safe to run multiple times

IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = N'Öğrenci')
    INSERT INTO Roles (RoleName) VALUES (N'Öğrenci');

IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = N'Danışman')
    INSERT INTO Roles (RoleName) VALUES (N'Danışman');

IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = N'Bölüm Başkanlığı')
    INSERT INTO Roles (RoleName) VALUES (N'Bölüm Başkanlığı');

-- ── Request Types (Selçuk Üniversitesi standard forms) ───────────
IF NOT EXISTS (SELECT 1 FROM RequestTypes WHERE FormCode = N'TF-ÖİŞL-02')
    INSERT INTO RequestTypes (FormCode, FormName, RequiresAdvisorApproval)
    VALUES (N'TF-ÖİŞL-02', N'Ders Ekleme/Çıkarma', 1);

IF NOT EXISTS (SELECT 1 FROM RequestTypes WHERE FormCode = N'TF-ÖİŞL-05')
    INSERT INTO RequestTypes (FormCode, FormName, RequiresAdvisorApproval)
    VALUES (N'TF-ÖİŞL-05', N'Not Yükseltme', 1);

IF NOT EXISTS (SELECT 1 FROM RequestTypes WHERE FormCode = N'TF-ÖİŞL-06')
    INSERT INTO RequestTypes (FormCode, FormName, RequiresAdvisorApproval)
    VALUES (N'TF-ÖİŞL-06', N'Üst Yarıyıldan Ders Alma', 1);

IF NOT EXISTS (SELECT 1 FROM RequestTypes WHERE FormCode = N'TF-ÖİŞL-13')
    INSERT INTO RequestTypes (FormCode, FormName, RequiresAdvisorApproval)
    VALUES (N'TF-ÖİŞL-13', N'Öğrenci Bilgi Formu', 0);

IF NOT EXISTS (SELECT 1 FROM RequestTypes WHERE FormCode = N'TF-ÖİŞL-14')
    INSERT INTO RequestTypes (FormCode, FormName, RequiresAdvisorApproval)
    VALUES (N'TF-ÖİŞL-14', N'Bölüm Dışından Ders Alma', 1);

IF NOT EXISTS (SELECT 1 FROM RequestTypes WHERE FormCode = N'TF-ÖİŞL-15')
    INSERT INTO RequestTypes (FormCode, FormName, RequiresAdvisorApproval)
    VALUES (N'TF-ÖİŞL-15', N'Kayıt Dondurma', 1);
