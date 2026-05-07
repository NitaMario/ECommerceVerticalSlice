CREATE DATABASE ECommerceSliceDB;
GO

USE ECommerceSliceDB;
GO

CREATE TABLE Users (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	Email NVARCHAR(255) NOT NULL UNIQUE,
	PasswordHash NVARCHAR(MAX) NOT NULL,
	[Name] NVARCHAR(100) NOT NULL,
	CreatedAt DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE Products (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	[Name] NVARCHAR(255) NOT NULL,
	[Description] NVARCHAR(MAX),
	Price DECIMAL(18,2) NOT NULL,
	ImageUrl NVARCHAR(500),
	CreatedAt DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE Orders (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	UserId INT FOREIGN KEY REFERENCES Users(Id),
	TotalAmount DECIMAL(18,2) NOT NULL,
	ShippingAddress NVARCHAR(MAX) NOT NULL,
	OrderDate DATETIME DEFAULT GETDATE()
);
GO

CREATE TABLE OrderItems (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	OrderId INT FOREIGN KEY REFERENCES Orders(Id),
	ProductId INT FOREIGN KEY REFERENCES Products(Id),
	Quantity INT NOT NULL,
	UnitPrice DECIMAL(18,2) NOT NULL
);
GO

INSERT INTO Products ([Name], [Description], Price, ImageUrl)
VALUES
('Blue Top', 'Comfortable blue top.', 15.99, 'https://placehold.co/150?text=Blue+Top'),
('Men Tshirt', 'Classic white tee.', 20.00, 'https://placehold.co/150?text=Men+Tshirt'),
('Sleeveless Dress', 'Elegant evening wear.', 45.50, 'https://placehold.co/150?text=Sleeveless+Dress'),
('Winter Jacket', 'Warm and cozy for cold days.', 89.99, 'https://placehold.co/150?text=Winter+Jacket');
GO