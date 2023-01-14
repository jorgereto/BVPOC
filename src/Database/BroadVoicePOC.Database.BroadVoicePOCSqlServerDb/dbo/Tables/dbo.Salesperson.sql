CREATE TABLE [dbo].[Salesperson] (
    [Id] INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NULL,
    [Email]        NVARCHAR (255) NULL,
    CONSTRAINT [PK_Salesperson] PRIMARY KEY CLUSTERED ([Id] ASC)
);


go
