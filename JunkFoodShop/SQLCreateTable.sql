-- User Table
Create Table UserAccount (
	UserId int Primary Key Identity(1, 1),
	[Username] nvarchar(255) Not Null,
	FullName nvarchar(255) Not Null,
	Email nvarchar(255) Not Null,
	PhoneNumber int Not Null,
	[Password] nvarchar(20) Not Null
);

-- Category Table
Create Table FoodCategory (
	Categoryid int Primary key Identity(1, 1),
	CategoryName nvarchar(255) Not Null,
	CategoryImage nvarchar(max) Not Null,
);

-- Food Table 
Create Table Foods (
	FoodId int Primary Key Identity(1, 1),
	FoodName nvarchar(255) Not Null,
	FoodImage nvarchar(max) Not Null,
	FoodPrice int Not Null,
	FoodStock int Not Null,
	FoodDescription nvarchar(max) Not Null,
	CategoryId int FOREIGN KEY REFERENCES FoodCategory(CategoryId)
);

-- Rating Table
Create Table Rating (
	RatingId int Primary Key Identity(1, 1),
	Star int Not Null,
	UserId int FOREIGN KEY REFERENCES UserAccount(UserId),
	FoodId int FOREIGN KEY REFERENCES Foods(FoodId)
);

-- Comment Table
Create Table Comment (
	CommentId int Primary Key Identity(1, 1),
	Content nvarchar(200) Not Null,
	DateComment datetime Not Null,
	UserId int FOREIGN KEY REFERENCES UserAccount(UserId),
	FoodId int FOREIGN KEY REFERENCES Foods(FoodId)
);

-- Cart Table
Create Table Cart (
	CartId int Primary Key Identity(1, 1),
	Quantity int Not Null,
	[Address] nvarchar(255),
	PhoneReceive int Not Null,
	FoodId int FOREIGN KEY REFERENCES Foods(FoodId),
	UserId int FOREIGN KEY REFERENCES UserAccount(UserId)
);

-- Order Status Table
Create Table OrderStatus (
	StatusId int Primary Key Identity(1, 1),
	StatusName nvarchar(100) Not Null
);

-- Order PaymentType Table
Create Table OrderPaymentType (
	PaymentId int Primary Key Identity(1, 1),
	PaymentType nvarchar(100) Not Null
);

-- OrderFood Table
Create Table OrderFood (
	OrderFoodId int Primary Key Identity(1, 1),
	Quantity int Not Null,
	[Address] nvarchar(255) Not Null,
	PhoneReceive int Not Null,
	FoodId int FOREIGN KEY REFERENCES Foods(FoodId),
	UserId int FOREIGN KEY REFERENCES UserAccount(UserId)
);

-- Order Table
Create Table [Order] (
	OrderId int Primary Key Identity(1, 1),
	TotalPrice int Not Null,
	DateOrder DATETIME Not Null,
	StatusId int FOREIGN KEY REFERENCES OrderStatus(StatusId),
	PaymentId int FOREIGN KEY REFERENCES OrderPaymentType(PaymentId),
	OrderFoodId int FOREIGN KEY REFERENCES OrderFood(OrderFoodId)
);






-- Default Value for Status and OrderPayment 
Insert into OrderStatus Values ('Checking'), ('Delivering'), ('Delivery Complete'), ('Order Cancel');
Insert into OrderPaymentType Values ('Cash On Delivery'), ('Bank Transfer');