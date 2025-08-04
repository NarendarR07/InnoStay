CREATE DATABASE InnoStayDB
GO
USE InnoStayDB
GO

--User Table
CREATE TABLE Users(
	UserID INT IDENTITY(1,1) CONSTRAINT pk_Users_UserID PRIMARY KEY,
	FirstName VARCHAR(25) NOT NULL,
	LastName VARCHAR(25) NOT NULL,
	Email VARCHAR(50) CONSTRAINT uq_Users_Email UNIQUE,
	[Password] VARCHAR(30) NOT NULL,
	[Role] VARCHAR(5) CONSTRAINT chk_Users_Role CHECK([Role] = 'User' OR [Role] = 'Admin'),
	CreatedAt DATETIME CONSTRAINT df_Users_CreatedAt DEFAULT GETDATE(),
);
--Room Table
CREATE TABLE Rooms(
    RoomID INT IDENTITY(101,1) CONSTRAINT pk_Rooms_RoomID PRIMARY KEY,
    RoomNumber INT CONSTRAINT uq_Rooms_RoomNumber UNIQUE,
    RoomType VARCHAR(25) CONSTRAINT chk_Rooms_RoomType 
        CHECK(RoomType IN ('Sunrise Alcove', 'Lakeside Nest', 'Evergreen Sanctuary')),
    PricePerNight DECIMAL,
    Capacity INT NOT NULL,
    IsAvailable BIT NOT NULL
);

--Booking Table
CREATE TABLE Bookings(
	BookingID INT IDENTITY(1,1) CONSTRAINT pk_Bookings_BookingID PRIMARY KEY,
	UserID INT CONSTRAINT fk_Bookings_UserID
		FOREIGN KEY(UserID) REFERENCES Users(UserID),
	RoomID INT CONSTRAINT fk_Bookings_RoomID 
		FOREIGN KEY(RoomID) REFERENCES Rooms(RoomID),
	CheckInDate DATE,
	CheckOutDate DATE,
	BookingStatus VARCHAR(15) CONSTRAINT chk_Bookings_BookingStatus CHECK(BookingStatus = 'Confirmed' OR BookingStatus = 'Cancelled' OR BookingStatus = 'Pending'),
);
--Payment Table
CREATE TABLE Payments(
	PaymentID INT IDENTITY(1,1) CONSTRAINT pk_Payments_PaymentID PRIMARY KEY,
	BookingID INT CONSTRAINT fk_Payments_BookingID 
		FOREIGN KEY(BookingID) REFERENCES Bookings(BookingID) CONSTRAINT uq_Payments_BookingID UNIQUE,
	PaymentMethod VARCHAR(15) CONSTRAINT chk_Payments_PaymentMethods CHECK(PaymentMethod = 'Cash' OR PaymentMethod = 'UPI' OR PaymentMethod = 'Card'),
	Amouont DECIMAL NOT NULL,
	PaymentStatus VARCHAR(10) CONSTRAINT chk_Payments_PaymentStatus CHECK(PaymentStatus = 'Completed' OR PaymentStatus = 'Pending'),
	TransactionID INT,
);

--Feedback Table
CREATE TABLE Feedback(
	FeedBackID INT IDENTITY(1,1) CONSTRAINT pk_Feedback_FeedBackID PRIMARY KEY,
	UserID INT CONSTRAINT fk_Feedback_UserID 
		FOREIGN KEY(UserID) REFERENCES Users(UserID),
	RoomID INT CONSTRAINT fk_Feedback_RoomID
		FOREIGN KEY(RoomID) REFERENCES Rooms(RoomID),
	Rating INT CONSTRAINT chk_Feedback_Rating CHECK(Rating BETWEEN 1 AND 5),
	Comments VARCHAR(200),
);

--Notifications Table
CREATE TABLE Notifications(
	NotificationID INT IDENTITY(1,1) CONSTRAINT pk_Notifications_ID PRIMARY KEY,
	UserID INT CONSTRAINT fk_Notifications_UserID
		FOREIGN KEY(UserID) REFERENCES Users(UserID),
	[Message] VARCHAR(255) NOT NULL,
	IsRead BIT CONSTRAINT df_Notifications_IsRead DEFAULT 0,
	SendAt DATETIME CONSTRAINT df_Notifications_SendAt DEFAULT GETDATE()
)


-- Insert Admin Users
INSERT INTO Users (FirstName, LastName, Email, Password, Role)
VALUES 
('John', 'Smith', 'john.smith@innostay.com', 'Admin@123', 'Admin'),
('Sarah', 'Johnson', 'sarah.j@innostay.com', 'Sarah@456', 'Admin');

-- Insert Customer Users
INSERT INTO Users (FirstName, LastName, Email, Password, Role)
VALUES 
('Michael', 'Brown', 'michael.b@gmail.com', 'User@123', 'User'),
('Emily', 'Davis', 'emily.d@yahoo.com', 'Emily@321', 'User');


INSERT INTO Rooms (RoomNumber, RoomType, PricePerNight, Capacity, IsAvailable)
VALUES
  (101, 'Sunrise Alcove', 2000.00, 1, 1),
  (102, 'Sunrise Alcove', 2100.00, 1, 1),
  (201, 'Lakeside Nest', 3200.00, 2, 1),
  (202, 'Lakeside Nest', 3500.00, 2, 1),
  (301, 'Evergreen Sanctuary', 6000.00, 4, 1),
  (302, 'Evergreen Sanctuary', 6500.00, 4, 0);

SELECT * FROM Users
SELECT * FROM Rooms
SELECT * FROM Bookings
SELECT * FROM Payments
SELECT * FROM Notifications
SELECT * FROM Feedback
DROP TABLE Notifications
DROP TABLE Feedback
DROP TABLE Payments
DROP TABLE Bookings
DROP TABLE Rooms
DROP TABLE Users	

