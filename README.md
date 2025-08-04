InnoStay

Description: Hotel_Management_System

InnoStay is a backend API for managing hotel room bookings, built with ASP.NET Core and Entity Framework Core. It includes user authentication (JWT), room inventory, bookings, payments, feedback, and notifications. The architecture separates controllers from a repository (DAL) layer and is fully unit-testable with both real in-memory repository tests and mocked controller tests.

🚀 Features

User Management
- Sign up / register (role = User)
- Validate credentials and issue JWT tokens
- Role-based authorization (Admin / User)
- CRUD operations on users

Room Management
- List all rooms
- Get room by ID
- Add / update / delete rooms
- Update room price
- Search for available room by type and date range

Booking
- List all bookings with joined user & room info
- Get booking by ID (enriched with room/user details and total price calculation)
- Get bookings by user
- Create booking with availability guard logic
- Update booking status (with cancellation guard)
- Delete bookings

Payments
- List all payments
- Get payment by ID / by user
- Add payment
- Update payment method and status
- Delete payment

Feedback
- CRUD feedback entries (with related user and room)

Notifications
- CRUD notifications
- Mark as read
- Filter notifications by user

Authentication & Security
- JWT issuance with user claims (role, name, userID)
- Protected endpoints via `[Authorize]`
- Basic input validation on controllers

Testing
- Repository-level tests using EF Core InMemory provider
- Controller-level tests using `Moq` to mock `IInnoStayRepository`
- Coverage of success / failure / edge cases

📦Getting Started

Prerequisites
- .NET SDK (7+ or 8+ depending on project configuration)
- Git

Clone Repository
```bash
git clone https://github.com/NarendarR07/InnoStay.git
cd InnoStay