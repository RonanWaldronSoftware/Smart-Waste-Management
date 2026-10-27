-- Migration: Add "Paper" waste type (ContentType)
-- Target DB: smartwaste (AWS RDS SQL Server)
-- Id is forced to 19 because the frontend maps icon/color to this exact Id.

-- 1) Review current waste types BEFORE inserting.
--    If Id = 19 is already taken by another type, STOP and use a different free Id
--    (and update the frontend + backend enum accordingly).
SELECT Id, Name, TranslationKey FROM ContentType ORDER BY Id;

-- 2) Insert Paper with a fixed Id = 19.
SET IDENTITY_INSERT ContentType ON;

IF NOT EXISTS (SELECT 1 FROM ContentType WHERE Id = 19)
BEGIN
    INSERT INTO ContentType (Id, Name, TranslationKey)
    VALUES (19, 'Paper', 'paperContentType');
END

SET IDENTITY_INSERT ContentType OFF;

-- 3) Verify.
SELECT Id, Name, TranslationKey FROM ContentType WHERE Id = 19;
