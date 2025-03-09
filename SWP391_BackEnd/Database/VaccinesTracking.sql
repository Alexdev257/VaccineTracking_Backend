CREATE TABLE "User"(
    "id" INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    "name" NVARCHAR(255) NOT NULL,
    "username" NVARCHAR(255) NOT NULL,
    "password" NVARCHAR(255) NOT NULL,
    "gmail" NVARCHAR(255) NOT NULL,
    "phone_number" NVARCHAR(255) NOT NULL,
    "date_of_birth" DATETIME NOT NULL,
    "avatar" NVARCHAR(255) NOT NULL,
    "gender" INT NOT NULL,
    "role" NVARCHAR(255) NOT NULL,
    "created_at" DATETIME NOT NULL,
    "status" NVARCHAR(255) NOT NULL,
    "isDeleted" BIT NOT NULL DEFAULT 0
);
CREATE UNIQUE INDEX "user_username_unique" ON "User"("username");
CREATE UNIQUE INDEX "user_phone_number_unique" ON "User"("phone_number");
CREATE UNIQUE INDEX "user_gmail_unique" ON "User"("gmail");

CREATE TABLE "Address"(
    "id" INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    "name" NVARCHAR(255) NOT NULL,
    "isDeleted" BIT NOT NULL DEFAULT 0
);

CREATE TABLE "Vaccines"(
    "id" INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    "name" NVARCHAR(255) NOT NULL,
    "quantity" INT NOT NULL,
    "description" NVARCHAR(255) NOT NULL,
    "price" DECIMAL(16, 2) NOT NULL,
    "does_times" INT NOT NULL,
    "suggest_age_min" INT NOT NULL,
    "suggest_age_max" INT NOT NULL,
    "entry_date" DATETIME NOT NULL,
    "time_expired" DATETIME NOT NULL,
    "address_ID" INT NOT NULL,
    "status" NVARCHAR(255) NOT NULL,
    "minimum_interval_date" INT NULL,
    "maximum_interval_date" INT NULL,
    "from_country" NVARCHAR(255) NOT NULL,
    "isDeleted" BIT NOT NULL DEFAULT 0,
    CONSTRAINT "vaccines_address_id_foreign" FOREIGN KEY("address_ID") REFERENCES "Address"("id")
);

CREATE TABLE "Vaccines_Combo"(
    "id" INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    "combo_name" NVARCHAR(255) NOT NULL,
    "discount" INT NOT NULL,
    "total_price" DECIMAL(16, 2) NOT NULL,
    "final_price" DECIMAL(16, 2) NOT NULL,
    "status" NVARCHAR(255) NOT NULL,
    "isDeleted" BIT NOT NULL DEFAULT 0
);

CREATE TABLE "Child"(
    "id" INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    "parent_ID" INT NOT NULL,
    "name" NVARCHAR(255) NOT NULL,
    "date_of_birth" DATETIME NOT NULL,
    "gender" INT NOT NULL,
    "status" NVARCHAR(255) NOT NULL,
    "created_at" DATETIME NOT NULL,
    "isDeleted" BIT NOT NULL DEFAULT 0,
    CONSTRAINT "child_parent_id_foreign" FOREIGN KEY("parent_ID") REFERENCES "User"("id")
);

CREATE TABLE "Booking"(
    "id" INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    "parent_id" INT NOT NULL,
    "advisory_details" NVARCHAR(255) NOT NULL,
    "created_at" DATETIME NOT NULL,
    "arrived_at" DATETIME NOT NULL,
    "status" NVARCHAR(255) NOT NULL,
    "isDeleted" BIT NOT NULL DEFAULT 0,
    CONSTRAINT "booking_parent_id_foreign" FOREIGN KEY("parent_id") REFERENCES "User"("id")
);

CREATE TABLE "Payment_Method"(
    "id" INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    "name" NVARCHAR(255) NOT NULL,
    "description" NVARCHAR(255) NOT NULL,
    "isDeleted" BIT NOT NULL DEFAULT 0
);

CREATE TABLE "Payment"(
    "payment_id" NVARCHAR(255) NOT NULL PRIMARY KEY,
    "booking_id" INT NOT NULL,
    "transaction_id" NVARCHAR(255) NOT NULL,
    "payer_id" NVARCHAR(255) NOT NULL,
    "payment_method" INT NOT NULL,
    "currency" NVARCHAR(255) NOT NULL,
    "total_price" DECIMAL(16, 2) NOT NULL,
    "payment_date" DATETIME NOT NULL,
    "status" NVARCHAR(255) NOT NULL,
    "isDeleted" BIT NOT NULL DEFAULT 0,
    CONSTRAINT "payment_payment_method_foreign" FOREIGN KEY("payment_method") REFERENCES "Payment_Method"("id"),
    CONSTRAINT "payment_booking_id_foreign" FOREIGN KEY("booking_id") REFERENCES "Booking"("id")
);

CREATE TABLE "Refresh_Token"(
    "id" BIGINT NOT NULL PRIMARY KEY,
    "user_id" INT NOT NULL,
    "refresh_token" NVARCHAR(255) NOT NULL,
    "access_token" NVARCHAR(1024) NOT NULL,
    "is_used" BIT NOT NULL,
    "is_revoked" BIT NOT NULL,
    "issued_at" DATETIME NOT NULL,
    "expired_at" DATETIME NOT NULL,
    CONSTRAINT "refresh_token_user_id_foreign" FOREIGN KEY("user_id") REFERENCES "User"("id")
);

CREATE TABLE "Vaccines_Tracking"(
    "id" INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    "vaccine_id" INT NOT NULL,
    "user_id" INT NOT NULL,
    "child_id" INT NOT NULL,
    "booking_id" INT NOT NULL,
    "minimum_interval_date" DATETIME NULL,
    "vaccination_date" DATETIME NULL,
    "maximum_interval_date" DATETIME NULL,
    "previous_vaccination" INT NULL,
    "status" NVARCHAR(255) NOT NULL,
    "administered_by" INT NOT NULL,
    "reaction" NVARCHAR(255) NOT NULL,
    "isDeleted" BIT NOT NULL DEFAULT 0,
    CONSTRAINT "vaccines_tracking_vaccine_id_foreign" FOREIGN KEY("vaccine_id") REFERENCES "Vaccines"("id"),
    CONSTRAINT "vaccines_tracking_user_id_foreign" FOREIGN KEY("user_id") REFERENCES "User"("id") ON DELETE CASCADE,
    CONSTRAINT "vaccines_tracking_child_id_foreign" FOREIGN KEY("child_id") REFERENCES "Child"("id") ON DELETE CASCADE,
    CONSTRAINT "vaccines_tracking_booking_id_foreign" FOREIGN KEY("booking_id") REFERENCES "Booking"("id")
);

-- Create junction tables with composite primary keys
CREATE TABLE "VaccinesCombo_Vaccines"(
    "vacine_combo" INT NOT NULL,
    "vaccine_id" INT NOT NULL,
    CONSTRAINT "vaccinescombo_vaccines_pk" PRIMARY KEY ("vacine_combo", "vaccine_id"),
    CONSTRAINT "vaccinescombo_vaccines_vacine_combo_foreign" FOREIGN KEY("vacine_combo") REFERENCES "Vaccines_Combo"("id"),
    CONSTRAINT "vaccinescombo_vaccines_vaccine_id_foreign" FOREIGN KEY("vaccine_id") REFERENCES "Vaccines"("id")
);

CREATE TABLE "Booking_ChildID"(
    "booking_id" INT NOT NULL,
    "child_id" INT NOT NULL,
    CONSTRAINT "booking_childid_pk" PRIMARY KEY("booking_id", "child_id"),
    CONSTRAINT "booking_childid_booking_id_foreign" FOREIGN KEY("booking_id") REFERENCES "Booking"("id"),
    CONSTRAINT "booking_childid_child_id_foreign" FOREIGN KEY("child_id") REFERENCES "Child"("id")
);

CREATE TABLE "Booking_Vaccine"(
    "booking_id" INT NOT NULL,
    "vaccine_id" INT NOT NULL,
    CONSTRAINT "booking_vaccine_pk" PRIMARY KEY("booking_id", "vaccine_id"),
    CONSTRAINT "booking_vaccine_booking_id_foreign" FOREIGN KEY("booking_id") REFERENCES "Booking"("id"),
    CONSTRAINT "booking_vaccine_vaccine_id_foreign" FOREIGN KEY("vaccine_id") REFERENCES "Vaccines"("id")
);

CREATE TABLE "Booking_Combo"(
    "booking_id" INT NOT NULL,
    "combo_id" INT NOT NULL,
    CONSTRAINT "booking_combo_pk" PRIMARY KEY("booking_id", "combo_id"),
    CONSTRAINT "booking_combo_booking_id_foreign" FOREIGN KEY("booking_id") REFERENCES "Booking"("id"),
    CONSTRAINT "booking_combo_combo_id_foreign" FOREIGN KEY("combo_id") REFERENCES "Vaccines_Combo"("id")
);

CREATE TABLE "Feedback"(
    "id" INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    "user_id" INT NOT NULL,
    "rating_score" INT NOT NULL,
    "description" NVARCHAR(255),
    "isDeleted" BIT NOT NULL DEFAULT 0,
    CONSTRAINT "feedback_user_id_foreign" FOREIGN KEY("user_id") REFERENCES "User"("id")
);

-- Insert data into Address table
INSERT INTO "Address" ("name", "isDeleted") VALUES 
('Hanoi Medical Center', 0),
('Ho Chi Minh City Vaccination Clinic', 0),
('Da Nang Health Facility', 0),
('Hue Central Hospital', 0),
('Nha Trang Medical Station', 0);

-- Insert data into User table
INSERT INTO "User" ("name", "username", "password", "gmail", "phone_number", "date_of_birth", "avatar", "gender", "role", "created_at", "status", "isDeleted") VALUES
('John Smith', 'johnsmith', 'password123', 'johnsmith@gmail.com', '0901234567', '1985-06-15', 'avatar1.jpg', 1, 'User', '2023-01-10', 'ACTIVE', 0),
('Emily Johnson', 'emilyjohnson', 'secure456', 'emilyjohnson@gmail.com', '0912345678', '1990-08-22', 'avatar2.jpg', 2, 'Admin', '2023-01-15', 'ACTIVE', 0),
('Michael Brown', 'michaelbrown', 'doctor789', 'michaelbrown@gmail.com', '0923456789', '1975-03-30', 'avatar3.jpg', 1, 'Admin', '2023-01-20', 'ACTIVE', 0),
('Sarah Davis', 'sarahdavis', 'nurse321', 'sarahdavis@gmail.com', '0934567890', '1988-11-05', 'avatar4.jpg', 2, 'Staff', '2023-01-25', 'ACTIVE', 0),
('David Wilson', 'davidwilson', 'admin987', 'davidwilson@gmail.com', '0945678901', '1980-07-14', 'avatar5.jpg', 1, 'User', '2023-01-30', 'ACTIVE', 0),
('Jennifer Lee', 'jenniferlee', 'parent654', 'jenniferlee@gmail.com', '0956789012', '1992-09-18', 'avatar6.jpg', 2, 'User', '2023-02-05', 'ACTIVE', 0),
('Robert Taylor', 'roberttaylor', 'doctor123', 'roberttaylor@gmail.com', '0967890123', '1979-05-25', 'avatar7.jpg', 1, 'Staff', '2023-02-10', 'ACTIVE', 0),
('Lisa Anderson', 'lisaanderson', 'password789', 'lisaanderson@gmail.com', '0978901234', '1995-01-12', 'avatar8.jpg', 2, 'Staff', '2023-02-15', 'ACTIVE', 0),
('James Martin', 'jamesmartin', 'secure123', 'jamesmartin@gmail.com', '0989012345', '1983-04-08', 'avatar9.jpg', 1, 'Staff', '2023-02-20', 'ACTIVE', 0),
('Patricia White', 'patriciawhite', 'nurse456', 'patriciawhite@gmail.com', '0990123456', '1987-12-03', 'avatar10.jpg', 2, 'User', '2023-02-25', 'ACTIVE', 0);

-- Insert data into Vaccines table
INSERT INTO "Vaccines" ("name", "quantity", "description", "price", "does_times", "suggest_age_min", "suggest_age_max", "entry_date", "time_expired", "address_ID", "status", "minimum_interval_date", "maximum_interval_date", "from_country", "isDeleted") VALUES
('MMR Vaccine', 500, 'Measles, Mumps, and Rubella vaccine', 45.99, 2, 12, 72, '2023-01-01', '2025-01-01', 1, 'AVAILABLE', 28, 35, 'USA', 0),
('DTaP Vaccine', 450, 'Diphtheria, Tetanus, and Pertussis vaccine', 55.50, 5, 2, 84, '2023-01-05', '2025-01-05', 2, 'AVAILABLE', 28, 42, 'UK', 0),
('Hepatitis B Vaccine', 600, 'Prevents Hepatitis B infection', 40.25, 3, 0, 240, '2023-01-10', '2025-01-10', 3, 'AVAILABLE', 28, 56, 'France', 0),
('Polio Vaccine', 550, 'Prevents poliomyelitis', 35.75, 4, 2, 72, '2023-01-15', '2025-01-15', 4, 'AVAILABLE', 28, 42, 'Germany', 0),
('Varicella Vaccine', 400, 'Prevents chickenpox', 50.00, 2, 12, 168, '2023-01-20', '2025-01-20', 5, 'AVAILABLE', 90, 180, 'Japan', 0),
('Rotavirus Vaccine', 350, 'Prevents rotavirus infections', 60.25, 3, 2, 8, '2023-01-25', '2025-01-25', 1, 'AVAILABLE', 28, 42, 'Switzerland', 0),
('Pneumococcal Vaccine', 480, 'Prevents pneumococcal infections', 65.50, 4, 2, 60, '2023-02-01', '2025-02-01', 2, 'AVAILABLE', 28, 56, 'USA', 0),
('Influenza Vaccine', 700, 'Annual flu vaccine', 30.00, 1, 6, 960, '2023-02-05', '2024-02-05', 3, 'AVAILABLE', NULL, NULL, 'UK', 0),
('Hepatitis A Vaccine', 520, 'Prevents Hepatitis A infection', 45.75, 2, 12, 240, '2023-02-10', '2025-02-10', 4, 'AVAILABLE', 180, 365, 'Netherlands', 0),
('Meningococcal Vaccine', 380, 'Prevents meningococcal disease', 70.25, 2, 11, 216, '2023-02-15', '2025-02-15', 5, 'AVAILABLE', 28, 56, 'Belgium', 0);

-- Insert data into Vaccines_Combo table
INSERT INTO "Vaccines_Combo" ("combo_name", "discount", "total_price", "final_price", "status", "isDeleted") VALUES
('Infant Basic Package', 10, 200.00, 180.00, 'AVAILABLE', 0),
('Toddler Complete Package', 15, 250.00, 212.50, 'AVAILABLE', 0),
('School Readiness Package', 12, 180.00, 158.40, 'AVAILABLE', 0),
('Teen Protection Package', 8, 150.00, 138.00, 'AVAILABLE', 0),
('Travel Vaccination Package', 20, 300.00, 240.00, 'AVAILABLE', 0);

-- Insert data into VaccinesCombo_Vaccines table
INSERT INTO "VaccinesCombo_Vaccines" ("vacine_combo", "vaccine_id") VALUES
(1, 2), (1, 3), (1, 6),
(2, 1), (2, 2), (2, 4), (2, 6),
(3, 1), (3, 4), (3, 5),
(4, 5), (4, 9), (4, 10),
(5, 8), (5, 9), (5, 10);

-- Insert data into Child table
INSERT INTO "Child" ("parent_ID", "name", "date_of_birth", "gender", "status", "created_at", "isDeleted") VALUES
(1, 'Thomas Smith', '2020-03-12', 1, 'ACTIVE', '2023-01-15', 0),
(1, 'Emma Smith', '2022-07-05', 2, 'ACTIVE', '2023-01-15', 0),
(2, 'Oliver Johnson', '2021-05-18', 1, 'ACTIVE', '2023-01-20', 0),
(6, 'Sophie Lee', '2019-11-30', 2, 'ACTIVE', '2023-02-10', 0),
(6, 'William Lee', '2023-01-05', 1, 'ACTIVE', '2023-02-10', 0),
(8, 'Charlotte Anderson', '2020-08-22', 2, 'ACTIVE', '2023-02-20', 0),
(9, 'Benjamin Martin', '2022-02-14', 1, 'ACTIVE', '2023-02-25', 0);

-- Insert data into Booking table
INSERT INTO "Booking" ("parent_id", "advisory_details", "created_at", "arrived_at", "status", "isDeleted") VALUES
(1, 'Regular checkup and scheduled vaccinations', '2023-03-01', '2023-03-10 09:30:00', 'COMPLETED', 0),
(2, 'First vaccination appointment', '2023-03-05', '2023-03-15 10:00:00', 'COMPLETED', 0),
(6, 'Annual vaccination update', '2023-03-10', '2023-03-20 11:15:00', 'COMPLETED', 0),
(8, 'Scheduled booster shots', '2023-03-15', '2023-03-25 09:00:00', 'COMPLETED', 0),
(9, 'Initial vaccination series', '2023-03-20', '2023-03-30 14:30:00', 'COMPLETED', 0),
(1, 'Follow-up vaccination appointment', '2023-04-01', '2023-04-10 13:45:00', 'COMPLETED', 0),
(2, 'Booster shots for DTaP and Polio', '2023-04-05', '2023-04-15 10:30:00', 'COMPLETED', 0),
(6, 'Vaccination before travel', '2023-04-10', '2023-04-20 15:00:00', 'COMPLETED', 0),
(1, 'Annual flu vaccination', '2023-04-15', '2023-04-25 11:00:00', 'SCHEDULED', 0),
(2, 'School required vaccination update', '2023-04-20', '2023-04-30 09:30:00', 'SCHEDULED', 0);

-- Insert data into Booking_ChildID table
INSERT INTO "Booking_ChildID" ("booking_id", "child_id") VALUES
(1, 1),
(1, 2),
(2, 3),
(3, 4),
(3, 5),
(4, 6),
(5, 7),
(6, 1),
(7, 3),
(8, 4),
(9, 1),
(10, 3);

-- Insert data into Booking_Vaccine table
INSERT INTO "Booking_Vaccine" ("booking_id", "vaccine_id") VALUES
(1, 1), (1, 2),
(2, 3), (2, 6),
(3, 1), (3, 5),
(4, 2), (4, 4),
(5, 3), (5, 6),
(6, 4), (6, 7),
(7, 2), (7, 4),
(8, 8), (8, 9),
(9, 8),
(10, 1), (10, 5);

-- Insert data into Booking_Combo table
INSERT INTO "Booking_Combo" ("booking_id", "combo_id") VALUES
(1, 1),
(2, 1),
(3, 2),
(4, 3),
(5, 1),
(6, 4),
(7, 2),
(8, 5);

-- Insert data into Payment_Method table
INSERT INTO "Payment_Method" ("name", "description", "isDeleted") VALUES
('Cash', 'Payment via Visa, Mastercard, or American Express', 0),
('Momo', 'Direct bank transfer to clinic account', 0),
('VnPay', 'Cash payment at clinic', 0),
('PayPal', 'Online payment through PayPal service', 0);

-- Insert data into Payment table
INSERT INTO "Payment" ("payment_id", "booking_id", "transaction_id", "payer_id", "payment_method", "currency", "total_price", "payment_date", "status", "isDeleted") VALUES
('PAY-001-2023', 1, 'TXN-001-2023', 'USR-001', 1, 'USD', 180.00, '2023-03-10 10:30:00', 'COMPLETED', 0),
('PAY-002-2023', 2, 'TXN-002-2023', 'USR-002', 3, 'USD', 100.50, '2023-03-15 11:15:00', 'COMPLETED', 0),
('PAY-003-2023', 3, 'TXN-003-2023', 'USR-006', 1, 'USD', 212.50, '2023-03-20 12:00:00', 'COMPLETED', 0),
('PAY-004-2023', 4, 'TXN-004-2023', 'USR-008', 2, 'USD', 158.40, '2023-03-25 09:45:00', 'COMPLETED', 0),
('PAY-005-2023', 5, 'TXN-005-2023', 'USR-009', 4, 'USD', 180.00, '2023-03-30 15:30:00', 'COMPLETED', 0),
('PAY-006-2023', 6, 'TXN-006-2023', 'USR-001', 1, 'USD', 138.00, '2023-04-10 14:30:00', 'COMPLETED', 0),
('PAY-008-2023', 8, 'TXN-008-2023', 'USR-006', 1, 'USD', 240.00, '2023-04-20 16:00:00', 'COMPLETED', 0),
('PAY-009-2023', 9, 'TXN-009-2023', 'USR-001', 3, 'USD', 30.00, '2023-04-25 11:45:00', 'PENDING', 0),
('PAY-010-2023', 10, 'TXN-010-2023', 'USR-002', 2, 'USD', 95.00, '2023-04-30 10:15:00', 'PENDING', 0),
('PAY-011-2023', 1, 'TXN-011-2023', 'USR-001', 4, 'USD', 35.50, '2023-03-11 14:20:00', 'COMPLETED', 0);

-- Insert data into Vaccines_Tracking table
INSERT INTO "Vaccines_Tracking" ("vaccine_id", "user_id", "child_id", "booking_id", "minimum_interval_date", "vaccination_date", "maximum_interval_date", "previous_vaccination", "status", "administered_by", "reaction", "isDeleted") VALUES
(1, 1, 1, 1, '2023-04-10', '2023-03-10', '2023-04-20', NULL, 'COMPLETED', 3, 'NO_REACTION', 0),
(2, 1, 2, 1, '2023-04-10', '2023-03-10', '2023-04-20', NULL, 'COMPLETED', 3, 'MILD_REACTION', 0),
(3, 2, 3, 2, '2023-04-15', '2023-03-15', '2023-05-15', NULL, 'COMPLETED', 7, 'NO_REACTION', 0),
(6, 2, 3, 2, '2023-04-15', '2023-03-15', '2023-05-15', NULL, 'COMPLETED', 7, 'NO_REACTION', 0),
(1, 6, 4, 3, '2023-04-20', '2023-03-20', '2023-05-20', NULL, 'COMPLETED', 3, 'NO_REACTION', 0),
(5, 6, 5, 3, '2023-06-20', '2023-03-20', '2023-09-20', NULL, 'COMPLETED', 3, 'MILD_REACTION', 0),
(2, 8, 6, 4, '2023-04-25', '2023-03-25', '2023-05-25', NULL, 'COMPLETED', 7, 'NO_REACTION', 0),
(4, 8, 6, 4, '2023-04-25', '2023-03-25', '2023-05-25', NULL, 'COMPLETED', 7, 'NO_REACTION', 0),
(3, 9, 7, 5, '2023-04-30', '2023-03-30', '2023-05-30', NULL, 'COMPLETED', 3, 'NO_REACTION', 0),
(6, 9, 7, 5, '2023-04-30', '2023-03-30', '2023-05-30', NULL, 'COMPLETED', 3, 'MODERATE_REACTION', 0);



-- Insert data into Refresh_Token table
INSERT INTO "Refresh_Token" ("id", "user_id", "refresh_token", "access_token", "is_used", "is_revoked", "issued_at", "expired_at") VALUES
(1, 1, 'refresh_token_1', 'access_token_1', 0, 0, '2023-03-01 09:00:00', '2023-04-01 09:00:00'),
(2, 2, 'refresh_token_2', 'access_token_2', 0, 0, '2023-03-05 10:30:00', '2023-04-05 10:30:00'),
(3, 3, 'refresh_token_3', 'access_token_3', 0, 0, '2023-03-10 11:45:00', '2023-04-10 11:45:00'),
(4, 4, 'refresh_token_4', 'access_token_4', 0, 0, '2023-03-15 13:15:00', '2023-04-15 13:15:00'),
(5, 5, 'refresh_token_5', 'access_token_5', 0, 0, '2023-03-20 14:30:00', '2023-04-20 14:30:00');

-- Insert data into Feedback table
INSERT INTO "Feedback" ("user_id", "rating_score", "description", "isDeleted") VALUES
(1, 5, 'Excellent service and very professional staff. My children were comfortable throughout the process.', 0),
(2, 4, 'Good experience overall. The nurses were very friendly and took time to explain everything.', 0),
(6, 5, 'Very satisfied with the vaccination service. The facility was clean and well-maintained.', 0),
(8, 3, 'The service was okay but had to wait longer than expected. Staff was friendly though.', 0),
(9, 4, 'Good care provided to my child. The doctor was knowledgeable and patient.', 0),
(1, 5, 'Second visit was even better than the first. Highly recommend this vaccination center.', 0),
(2, 4, 'Consistent quality service. Appreciate the reminder system for follow-up vaccinations.', 0),
(6, 5, 'The travel vaccination package was comprehensive and reasonably priced.', 0);