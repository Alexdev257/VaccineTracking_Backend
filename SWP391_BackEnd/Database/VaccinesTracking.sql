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
('John Smith', 'admin1', 'password123', 'admin1@gmail.com', '0901234567', '1985-06-15', 'avatar1.jpg', 1, 'Admin', '2023-01-10', 'ACTIVE', 0),
('Emily Johnson', 'admin2', 'secure456', 'admin2@gmail.com', '0912345678', '1990-08-22', 'avatar2.jpg', 2, 'Admin', '2023-01-15', 'ACTIVE', 0);

-- Insert data into Vaccines table
INSERT INTO Vaccines (name, quantity, description, price, does_times, from_country, suggest_age_min, suggest_age_max, entry_date, time_expired, minimum_interval_date, maximum_interval_date, status, address_ID, isDeleted)
VALUES 
('Pfizer-BioNTech COVID-19 Vaccine', 500000, 'An mRNA vaccine to protect against COVID-19, requiring two doses.', 750000, 2, 'USA/Germany', 6, 60, '2023-01-01T00:00:00Z', '2025-04-05T00:00:00Z', 21, 28, 'Instock', 1, 0),
('DPT-VGB-Hib', 1000000, 'A pentavalent vaccine protecting against diphtheria, pertussis, tetanus, hepatitis B, and Haemophilus influenzae type B.', 100000, 3, 'Vietnam/India', 2, 24, '2023-01-01T00:00:00Z', '2025-04-05T00:00:00Z', 28, 35, 'Instock', 2, 0),
('Measles Vaccine', 800000, 'A live attenuated vaccine to prevent measles, part of Vietnam''s EPI.', 50000, 2, 'Vietnam', 9, 18, '2023-01-01T00:00:00Z', '2025-04-05T00:00:00Z', 28, 35, 'Instock', 3, 0),
('Hepatitis B Vaccine', 600000, 'A vaccine to prevent hepatitis B, administered at birth and in childhood.', 80000, 4, 'Vietnam/Korea', 4, 60, '2023-01-01T00:00:00Z', '2025-04-05T00:00:00Z', 30, 60, 'Instock', 4, 0),
('Japanese Encephalitis Vaccine', 450000, 'A vaccine to prevent Japanese encephalitis, common in rural areas.', 120000, 3, 'Vietnam/Japan', 12, 65, '2023-01-01T00:00:00Z', '2025-04-05T00:00:00Z', 14, 28, 'Instock', 1, 0),
('BCG Vaccine', 1200000, 'A vaccine to prevent tuberculosis, given at birth in Vietnam''s EPI.', 40000, 1, 'Vietnam', 4, 1, '2023-01-01T00:00:00Z', '2025-04-05T00:00:00Z', 0, 0, 'Instock', 2, 0),
('Polio Vaccine (OPV)', 900000, 'An oral vaccine to prevent poliomyelitis, part of Vietnam''s EPI.', 60000, 4, 'Vietnam/India', 2, 60, '2023-01-01T00:00:00Z', '2025-04-05T00:00:00Z', 28, 35, 'Instock', 3, 0),
('Rabies Vaccine (Verorab)', 200000, 'A vaccine to prevent rabies, used post-exposure or pre-exposure.', 350000, 3, 'France', 2, 60, '2023-01-01T00:00:00Z', '2025-04-05T00:00:00Z', 7, 14, 'Instock', 4, 0),
('Typhoid Vaccine (Typhim Vi)', 300000, 'A vaccine to prevent typhoid fever, recommended for travelers.', 300000, 1, 'France', 2, 65, '2023-01-01T00:00:00Z', '2025-04-05T00:00:00Z', 0, 0, 'Instock', 1, 0),
('Rotavirus Vaccine (Rotarix)', 400000, 'An oral vaccine to prevent rotavirus gastroenteritis in infants.', 650000, 2, 'Belgium', 6, 24, '2023-01-01T00:00:00Z', '2025-04-05T00:00:00Z', 28, 35, 'Instock', 2, 0),
('Rabies Vaccine (Rabipur)', 150000, 'A vaccine to prevent rabies, used for pre-exposure and post-exposure prophylaxis.', 320000, 3, 'India', 2, 60, '2023-01-01T00:00:00Z', '2025-04-05T00:00:00Z', 7, 14, 'Instock', 3, 0),
('Dengue Vaccine (Dengvaxia)', 200000, 'A vaccine to prevent dengue fever, recommended for individuals with prior dengue infection.', 950000, 3, 'France', 9, 45, '2023-01-01T00:00:00Z', '2025-04-05T00:00:00Z', 180, 180, 'Instock', 4, 0),
('Hepatitis A Vaccine (Havrix)', 250000, 'A vaccine to prevent hepatitis A, common in areas with poor sanitation.', 450000, 2, 'Belgium', 1, 60, '2023-01-01T00:00:00Z', '2025-04-05T00:00:00Z', 180, 365, 'Instock', 1, 0),
('Cholera Vaccine (Shanchol)', 300000, 'An oral vaccine to prevent cholera, recommended in endemic areas.', 150000, 2, 'India', 1, 60, '2023-01-01T00:00:00Z', '2025-04-05T00:00:00Z', 14, 28, 'Instock', 2, 0),
('Meningococcal Vaccine (Menactra)', 180000, 'A vaccine to prevent meningococcal disease, recommended for high-risk groups.', 850000, 1, 'USA', 2, 55, '2023-01-01T00:00:00Z', '2025-04-05T00:00:00Z', 0, 0, 'Instock', 3, 0),
('Pneumococcal Vaccine (Prevenar 13)', 220000, 'A vaccine to prevent pneumococcal diseases like pneumonia, common in children and the elderly.', 950000, 4, 'USA', 2, 65, '2023-01-01T00:00:00Z', '2025-04-05T00:00:00Z', 28, 56, 'Instock', 1, 0),
('MMR Vaccine (Priorix)', 300000, 'A combined vaccine to prevent measles, mumps, and rubella, recommended for children.', 250000, 2, 'Belgium', 9, 18, '2023-01-01T00:00:00Z', '2025-04-05T00:00:00Z', 28, 35, 'Instock', 2, 0),
('Varicella Vaccine (Varivax)', 180000, 'A vaccine to prevent chickenpox, recommended for children and adults without immunity.', 700000, 2, 'USA', 12, 60, '2023-01-01T00:00:00Z', '2025-04-05T00:00:00Z', 28, 84, 'Instock', 3, 0),
('Influenza Vaccine (Vaxigrip Tetra)', 400000, 'A vaccine to prevent seasonal influenza, recommended annually.', 350000, 1, 'France', 6, 65, '2023-01-01T00:00:00Z', '2025-04-05T00:00:00Z', 0, 0, 'Instock', 4, 0),
('HPV Vaccine (Gardasil 9)', 150000, 'A vaccine to prevent human papillomavirus infections, which can lead to cervical cancer.', 1450000, 3, 'USA', 9, 45, '2023-01-01T00:00:00Z', '2025-04-05T00:00:00Z', 60, 180, 'Instock', 1, 0);









INSERT INTO Vaccines_Combo(combo_name, discount, total_price, final_price, status, isDeleted)
VALUES 
('Childhood Protection Pack', 10, 800000.00, 720000.00, 'Instock', 0),
('Travel Safety Bundle', 15, 770000.00, 654500.00, 'Instock', 0),
('Newborn Essentials', 5, 120000.00, 114000.00, 'Instock', 0),
('Adult Health Booster', 20, 930000.00, 744000.00, 'Instock', 0),
('Tropical Disease Defense', 15, 1450000.00, 1232500.00, 'Instock', 0),
('Comprehensive Rabies Protection', 10, 670000.00, 603000.00, 'Instock', 0),
('Adolescent Health Pack', 20, 2050000.00, 1640000.00, 'Instock', 0);


INSERT INTO VaccinesCombo_Vaccines (vacine_combo, vaccine_id)
VALUES 
(1, 2), (1, 3), (1, 10),  
(2, 8), (2, 9), (2, 5),  
(3, 4), (3, 6),           
(4, 1), (4, 7), (4, 5),
(5, 12), (5, 14), (5, 9), 
(6, 8), (6, 11),         
(7, 15), (7, 13), (7, 1);

-- Insert data into Child table

-- Insert data into Booking table

-- Insert data into Booking_ChildID table

-- Insert data into Booking_Vaccine table

-- Insert data into Booking_Combo table

-- Insert data into Payment_Method table
INSERT INTO "Payment_Method" ("name", "description", "isDeleted") VALUES
('Cash', 'Payment via Visa, Mastercard, or American Express', 0),
('Momo', 'Direct bank transfer to clinic account', 0),
('VnPay', 'Cash payment at clinic', 0),
('PayPal', 'Online payment through PayPal service', 0);

-- Insert data into Payment table

-- Insert data into Vaccines_Tracking table

-- Insert data into Refresh_Token table

-- Insert data into Feedback table
INSERT INTO Feedback (user_id, rating_score, description, isDeleted) VALUES
(6, 5, 'The vaccine tracking system is very useful and easy to use.', 0),
(6, 4, 'Helpful reminders, but sometimes the notifications are delayed.', 0),
(6, 5, 'Great service! It helps me keep track of my child’s vaccinations.', 0),
(6, 3, 'The interface could be improved for better usability.', 0),
(6, 2, 'Some vaccine records were missing, please fix this issue.', 0);