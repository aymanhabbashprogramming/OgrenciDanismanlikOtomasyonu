-- AdvisorEmails.sql
-- Fills Email for all demo advisors (SicilNo 10001–14003).
-- The Email column already exists (nullable NVARCHAR); this is a data-only update.
-- Run with: sqlcmd -S EYMEN -d OgrenciDanismanlikDB -i DatabaseScripts/AdvisorEmails.sql -f 65001

SET NOCOUNT ON;

UPDATE Advisors SET Email = N'ahmet.yilmaz@selcuk.edu.tr'   WHERE SicilNo = N'10001';
UPDATE Advisors SET Email = N'mehmet.celik@selcuk.edu.tr'   WHERE SicilNo = N'11001';
UPDATE Advisors SET Email = N'ayse.kaya@selcuk.edu.tr'      WHERE SicilNo = N'11002';
UPDATE Advisors SET Email = N'fatih.demir@selcuk.edu.tr'    WHERE SicilNo = N'11003';
UPDATE Advisors SET Email = N'zeynep.arslan@selcuk.edu.tr'  WHERE SicilNo = N'11004';
UPDATE Advisors SET Email = N'mustafa.sahin@selcuk.edu.tr'  WHERE SicilNo = N'11005';
UPDATE Advisors SET Email = N'hulya.ozturk@selcuk.edu.tr'   WHERE SicilNo = N'11006';
UPDATE Advisors SET Email = N'ali.yildiz@selcuk.edu.tr'     WHERE SicilNo = N'12001';
UPDATE Advisors SET Email = N'sevda.konak@selcuk.edu.tr'    WHERE SicilNo = N'12002';
UPDATE Advisors SET Email = N'baris.gul@selcuk.edu.tr'      WHERE SicilNo = N'12003';
UPDATE Advisors SET Email = N'ibrahim.polat@selcuk.edu.tr'  WHERE SicilNo = N'13001';
UPDATE Advisors SET Email = N'esra.aktas@selcuk.edu.tr'     WHERE SicilNo = N'13002';
UPDATE Advisors SET Email = N'caner.koc@selcuk.edu.tr'      WHERE SicilNo = N'13003';
UPDATE Advisors SET Email = N'tolga.duman@selcuk.edu.tr'    WHERE SicilNo = N'14001';
UPDATE Advisors SET Email = N'sule.bulut@selcuk.edu.tr'     WHERE SicilNo = N'14002';
UPDATE Advisors SET Email = N'emre.turk@selcuk.edu.tr'      WHERE SicilNo = N'14003';

PRINT N'Advisor e-posta adresleri güncellendi.';
PRINT N'';
PRINT N'Doğrulama:';
SELECT Advisor.SicilNo,
       Advisor.FirstName + N' ' + Advisor.LastName AS AdSoyad,
       Advisor.Email
FROM   Advisors AS Advisor
WHERE  Advisor.SicilNo IN (
    N'10001',
    N'11001', N'11002', N'11003', N'11004', N'11005', N'11006',
    N'12001', N'12002', N'12003',
    N'13001', N'13002', N'13003',
    N'14001', N'14002', N'14003'
)
ORDER BY Advisor.SicilNo;
