CREATE TABLE [dbo].[department]
(
    [RecordNo] [int] IDENTITY(1,1) PRIMARY KEY,
    [DepartmentCode] [varchar](50) NOT NULL UNIQUE,
    [DepartmentName] [varchar](100) NOT NULL,
    [JapanHead] [varchar](1) NOT NULL,  -- 'Y' or 'N'
    [Office] [varchar](50) NOT NULL,
    [GotSection] [varchar](1) NOT NULL, -- 'Y' or 'N'
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