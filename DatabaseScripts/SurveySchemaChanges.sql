-- Survey schema changes: add Status, Description, QuestionType; remove AnsweredAt for anonymity.
-- Safe to re-run (idempotent).

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Surveys' AND COLUMN_NAME = 'Status'
)
    ALTER TABLE Surveys
        ADD Status nvarchar(20) NOT NULL
        CONSTRAINT DF_Surveys_Status DEFAULT 'Taslak';

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Surveys' AND COLUMN_NAME = 'Description'
)
    ALTER TABLE Surveys ADD Description nvarchar(500) NULL;

IF NOT EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'SurveyQuestions' AND COLUMN_NAME = 'QuestionType'
)
    ALTER TABLE SurveyQuestions
        ADD QuestionType nvarchar(20) NOT NULL
        CONSTRAINT DF_SurveyQuestions_QuestionType DEFAULT 'Puanlama';

-- Make Score nullable: SerbestMetin answers store NULL; Puanlama answers store 1-10.
IF EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'SurveyAnswers' AND COLUMN_NAME = 'Score' AND IS_NULLABLE = 'NO'
)
BEGIN
    IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_SurveyAnswers_Score')
        ALTER TABLE SurveyAnswers DROP CONSTRAINT CK_SurveyAnswers_Score;

    ALTER TABLE SurveyAnswers ALTER COLUMN Score int NULL;

    ALTER TABLE SurveyAnswers ADD CONSTRAINT CK_SurveyAnswers_Score CHECK (Score >= 1 AND Score <= 10);
END

-- Confirm unique constraint on SurveyParticipations (SurveyId, StudentId).
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = 'UQ_SurveyParticipations'
      AND object_id = OBJECT_ID('SurveyParticipations')
)
    ALTER TABLE SurveyParticipations
        ADD CONSTRAINT UQ_SurveyParticipations UNIQUE (SurveyId, StudentId);

-- Remove AnsweredAt: answers carry no timestamp so they cannot be linked to a participation record.
IF EXISTS (
    SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'SurveyAnswers' AND COLUMN_NAME = 'AnsweredAt'
)
BEGIN
    DECLARE @ConstraintName nvarchar(200);
    SELECT @ConstraintName = dc.name
    FROM   sys.default_constraints AS dc
    INNER  JOIN sys.columns        AS col ON dc.parent_object_id = col.object_id
                                         AND dc.parent_column_id = col.column_id
    INNER  JOIN sys.tables         AS tbl ON col.object_id = tbl.object_id
    WHERE  tbl.name = 'SurveyAnswers' AND col.name = 'AnsweredAt';

    IF @ConstraintName IS NOT NULL
        EXEC('ALTER TABLE SurveyAnswers DROP CONSTRAINT ' + @ConstraintName);

    ALTER TABLE SurveyAnswers DROP COLUMN AnsweredAt;
END
