# 🚗 Car Showroom Management System

## 📌 Overview

A desktop-based Car Showroom Management System designed to manage car inventory, customer bookings, sales transactions, and vehicle servicing with role-based access control.

The system simulates real-world showroom operations, including booking approvals, payment processing, inventory updates, and service tracking.

---

## ✨ Key Features

### 🔐 Role-Based Authentication

* Separate login for Admin and Users
* Admin has full control over inventory, bookings, and services

---

### 🚘 Car Inventory Management

* Add, update, delete car details
* Automatic stock updates after purchase
* Displays **"Out of Stock"** when inventory is empty

---

### 📅 Booking System

* Users can book cars
* Booking status: **Pending / Approved**
* Admin can approve or reject bookings
* Prevents invalid bookings when stock is unavailable

---

### 💳 Simulated Payment System

* Validates:

  * Card number (12 digits)
  * CVV (3 digits)
  * Name input
* No real payment processing (simulation only)
* Generates receipt after successful payment

---

### 🧾 Sales & Transaction History

* Tracks all purchases and booking records
* Maintains complete transaction history
* Automatically updates inventory after sales

---

### 🔧 Car Service Module

* Users can request service for their vehicle
* Service cost is calculated based on selected service type
* Admin updates service status
* Users can track service progress

---

## 🛠️ Tech Stack

* **Frontend:** VB.NET (Windows Forms)
* **Backend:** .NET Framework
* **Database:** SQL Server (SSMS)

---

## 🔄 System Workflow

### Booking Flow

1. User selects a car
2. Places booking request
3. Admin approves/rejects booking
4. On approval → user proceeds to payment
5. Inventory updates automatically

---

### Service Flow

1. User submits service request
2. Selects service → cost displayed
3. Completes payment (simulated)
4. Admin updates service status
5. User tracks service progress

---

## 📸 Screenshots

### 🔐 Login Page
<img width="465" height="342" alt="login" src="https://github.com/user-attachments/assets/4dea78f9-fd88-4d57-848d-5c11071259d7" />

### 👤 Profile Page
<img width="1002" height="546" alt="profile" src="https://github.com/user-attachments/assets/1bd5e8bc-b779-4426-bc61-c22b338bc33a" />

### 🚗 Inventory Dashboard
<img width="1919" height="1018" alt="inventory" src="https://github.com/user-attachments/assets/834fa705-7806-4cc6-9341-12a349fcb3a5" />

### 📊 Sales & Booking
<img width="1919" height="1021" alt="sales_booking" src="https://github.com/user-attachments/assets/ec9ea89d-2368-43d1-ac3a-b9a13560d867" />

### 🔧 Service Module
<img width="1918" height="1016" alt="service" src="https://github.com/user-attachments/assets/168f160a-d85b-489f-88f5-dec3c7e02ed6" />

### 🧾 Invoice / Receipt
<img width="575" height="746" alt="image" src="https://github.com/user-attachments/assets/7ca79908-e51a-4928-9bc7-79c42835f739" />


---

## ⚠️ Note

* Payment system is **simulated** (no real transactions)
* Built for learning and demonstration purposes

---

## 👨‍💻 Author

Manish
