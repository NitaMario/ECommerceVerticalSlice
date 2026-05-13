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

CREATE TABLE CartItems (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	UserId INT FOREIGN KEY REFERENCES Users(Id),
	ProductId INT FOREIGN KEY REFERENCES Products(Id),
	Quantity INT NOT NULL,
	CreatedAt DATETIME DEFAULT GETDATE()
);

INSERT INTO Products ([Name], [Description], Price, ImageUrl)
VALUES
('Blue Top', 'Comfortable blue top.', 15.99, 'https://placehold.co/150?text=Blue+Top'),
('Men Tshirt', 'Classic white tee.', 20.00, 'https://placehold.co/150?text=Men+Tshirt'),
('Sleeveless Dress', 'Elegant evening wear.', 45.50, 'https://placehold.co/150?text=Sleeveless+Dress'),
('Winter Jacket', 'Warm and cozy for cold days.', 89.99, 'https://placehold.co/150?text=Winter+Jacket');
GO

INSERT INTO Products ([Name], [Description], Price, ImageUrl)
VALUES
('Denim Jacket', 'Denim jacket with a comfortable tailored fit.', 65.00, 'https://placehold.co/150?text=Denim+Jacket'),
('Wool Sweater', 'Soft sweater made from 100% fine wool.', 85.50, 'https://placehold.co/150?text=Wool+Sweater'),
('Waterproof Hiking Boots', 'All-terrain outdoor boots with advanced grip.', 120.00, 'https://placehold.co/150?text=Hiking+Boots'),
('Running Shorts', 'Lightweight shorts for running.', 25.99, 'https://placehold.co/150?text=Running+Shorts'),
('Leather Belt', 'Genuine leather belt.', 35.00, 'https://placehold.co/150?text=Leather+Belt'),
('Polarized Aviator Sunglasses', 'Classic aviator frames with polarized lenses for UV protection.', 45.00, 'https://placehold.co/150?text=Sunglasses');
GO