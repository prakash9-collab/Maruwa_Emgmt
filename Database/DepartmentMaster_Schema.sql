-- SQL Script to create Department Master table
-- Run this script on your database to create the required table

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[department]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[department]
    (
        [RecordNo] [int] IDENTITY(1,1) PRIMARY KEY,
        [DepartmentCode] [varchar](50) NOT NULL UNIQUE,
        [DepartmentName] [varchar](100) NOT NULL,
        [JapanHead] [varchar](1) NOT NULL,
        [Office] [varchar](50) NOT NULL,
        [GotSection] [varchar](1) NOT NULL,
        [Prefix] [varchar](50) NOT NULL,
        [ParentPositionCode] [varchar](100) NULL,
        [ParentDepartmentId] [int] NULL,
        [CreatedBy] [varchar](100) NULL,
        [CreatedOn] [datetime] NULL,
        [EditedBy] [varchar](100) NULL,
        [EditedOn] [datetime] NULL,
        [IsActive] [bit] NOT NULL DEFAULT 1,
        [Remarks] [varchar](500) NULL
    )

    -- Create indexes
    CREATE INDEX IX_DepartmentCode ON [dbo].[department]([DepartmentCode])
    CREATE INDEX IX_DepartmentName ON [dbo].[department]([DepartmentName])
    CREATE INDEX IX_IsActive ON [dbo].[department]([IsActive])
    CREATE INDEX IX_CreatedOn ON [dbo].[department]([CreatedOn])

    PRINT 'Department table created successfully'
END
ELSE
BEGIN
    PRINT 'Department table already exists'
END
GO

-- Sample data (optional)
-- INSERT INTO [dbo].[department] 
-- VALUES ('3200', 'CV', 'Y', 'O', 'Y', 'CV', NULL, NULL, 'ADMIN', GETDATE(), NULL, NULL, 1, NULL)
