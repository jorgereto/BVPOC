CREATE TABLE [dbo].[Product] (
    [Id] INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NULL,
    [Code]        NVARCHAR (255) NULL,
    [Cost] DECIMAL(19, 4) NULL, 
    CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([Id] ASC)
);


go
