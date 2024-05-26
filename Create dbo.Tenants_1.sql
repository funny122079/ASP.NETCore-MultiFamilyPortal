USE [aspnet-53bc9b9d-9d6a-45d4-8429-2a2761773502]
GO

/****** Object: Table [dbo].[Tenants] Script Date: 4/9/2024 4:35:24 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Tenants] (
    [Id]                     INT                IDENTITY (1, 1) NOT NULL,
    [Host]                   NVARCHAR (450)     NULL,
    [Environment]            NVARCHAR (MAX)     NULL,
    [Disabled]               BIT                NOT NULL,
    [DatabaseName]           NVARCHAR (MAX)     NULL,
    [IsREMentorStudent]      BIT                NOT NULL,
    [GoogleSiteVerification] NVARCHAR (MAX)     NULL,
    [Created]                DATETIMEOFFSET (7) NOT NULL
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_Tenants_Host]
    ON [dbo].[Tenants]([Host] ASC) WHERE ([Host] IS NOT NULL);


GO
ALTER TABLE [dbo].[Tenants]
    ADD CONSTRAINT [PK_Tenants] PRIMARY KEY CLUSTERED ([Id] ASC);

Go
INSERT INTO [dbo].[Tenants] ([Host], [Environment], [Disabled], [DatabaseName], [IsREMentorStudent], [GoogleSiteVerification], [Created]) 
VALUES ('localhost', 'Production', 0, 'multifamilyportal', 1, 'google-site-verification-code', GETDATE());



