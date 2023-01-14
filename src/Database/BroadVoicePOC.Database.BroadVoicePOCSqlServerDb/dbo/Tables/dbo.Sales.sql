CREATE TABLE [dbo].[Sales] (
    [Id] INT            IDENTITY (1, 1) NOT NULL,
    [SalespersonId] INT NOT NULL,
    [CustomerId] INT    NOT NULL,
    [ProductId] INT     NOT NULL,
    [City]        NVARCHAR (255) NULL,
    [State]       NVARCHAR (255) NULL,
    [Date]        DATETIME NULL,
    [Price] DECIMAL(19, 4) NULL, 
    CONSTRAINT [PK_Sales] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Sales_Customer] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customer]([Id]),
    CONSTRAINT [FK_Sales_Salesperson] FOREIGN KEY ([SalespersonId]) REFERENCES [dbo].[Salesperson]([Id]),
    CONSTRAINT [FK_Sales_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product]([Id])
);


go
