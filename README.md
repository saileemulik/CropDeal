# 🌾 CropDeal

> A full-stack agricultural marketplace platform that connects **Farmers and Dealers**, enabling crop listing, discovery, negotiation, subscription-based access, payments, reviews, transactions, pickup scheduling, notifications, and AI-powered assistance.

---

## 📌 Overview

**CropDeal** is a full-stack web application designed to simplify the process of buying and selling agricultural crops through a centralized digital platform.

The application provides separate experiences for **Farmers, Dealers, and Administrators**. Farmers can list their crops, manage listings, interact with dealers, negotiate prices, and track transactions. Dealers can discover available crops, subscribe to the platform, negotiate with farmers, make payments, schedule pickups, and provide reviews.

The platform also includes authentication and authorization, payment integration, email notifications, reporting, and an AI-powered chatbot to improve the overall user experience.

The project follows a client-server architecture with an **Angular frontend** and **ASP.NET Core Web API backend**, backed by **SQL Server** using **Entity Framework Core**.

---

## ✨ Key Features

### 👨‍🌾 Farmer

* User registration and login
* Farmer profile management
* Add and manage crop listings
* Update crop information
* View crop listing status
* Receive dealer interactions
* Price negotiation with dealers
* View transactions
* Manage bank account information
* Pickup scheduling and tracking
* Notifications
* Review and rating management

### 🏪 Dealer

* User registration and login
* Dealer profile management
* Browse available crops
* Search and filter crop listings
* View crop details
* Subscription management
* Initiate price negotiations
* Accept or reject negotiated prices
* Make payments through Razorpay
* View transaction history
* Schedule crop pickups
* Track pickup status
* Submit reviews and ratings
* Receive notifications

### 👨‍💼 Admin

* Admin authentication
* User management
* Farmer and dealer management
* Crop listing management
* Subscription management
* Transaction monitoring
* Review management
* Application reports
* Administrative operations

---

## 🤝 Price Negotiation

CropDeal provides an interactive negotiation workflow between Farmers and Dealers.

The negotiation process allows users to:

1. Select a crop listing.
2. Initiate a negotiation.
3. Propose a price.
4. Farmer reviews the proposed price.
5. Farmer can accept, reject, or respond with another price.
6. Dealer can continue the negotiation.
7. Once both parties agree, the negotiated price can proceed toward the transaction and payment process.

This helps provide a more realistic agricultural marketplace experience instead of relying only on fixed crop prices.

---

## 💳 Payment Integration

CropDeal integrates **Razorpay** for online payment processing.

The payment workflow includes:

```text
Crop Selection
      ↓
Price / Negotiation
      ↓
Order Creation
      ↓
Razorpay Payment
      ↓
Payment Verification
      ↓
Transaction Creation
      ↓
Pickup Scheduling
```

Payment-related operations are handled through the backend API to prevent sensitive payment credentials from being exposed to the frontend.

> ⚠️ API keys and payment secrets are intentionally excluded from the repository.

---

## 📅 Pickup Scheduling

After a successful transaction, users can manage the crop pickup process.

The pickup workflow supports statuses such as:

```text
Proposed
   ↓
Scheduled
   ↓
Picked Up
   ↓
Delivered
```

This provides visibility into the physical fulfillment process after an online transaction.

---

## 🔐 Authentication & Authorization

CropDeal uses secure authentication and role-based authorization.

### Authentication

* JWT-based authentication
* ASP.NET Core Identity
* User registration
* User login
* Password reset using OTP
* Google authentication support
* Token-based API authorization

### User Roles

```text
                 ┌──────────────┐
                 │    Admin     │
                 └──────┬───────┘
                        │
             ┌──────────┴──────────┐
             │                     │
       ┌─────▼─────┐        ┌──────▼──────┐
       │   Farmer   │        │   Dealer    │
       └────────────┘        └─────────────┘
```

Role-based access ensures that users can access only the functionality appropriate to their role.

---

## 🤖 AI Chatbot

CropDeal includes an AI-powered chatbot to provide assistance to users.

The chatbot can:

* Accept user questions
* Process natural-language requests
* Generate AI-based responses
* Provide fallback responses when the AI service is unavailable

The backend retrieves the OpenAI API key through application configuration rather than hardcoding credentials in source code.

> ⚠️ The OpenAI API key is not stored in the repository.

---

## 🔔 Notifications

The application provides notification functionality for important user activities such as:

* Negotiation updates
* Transaction updates
* Payment-related events
* Pickup updates
* Other application activities

Notifications are handled through backend APIs and can be consumed by the Angular frontend.

---

## 📧 Email & OTP

CropDeal supports email-based functionality for authentication and account management.

The application includes:

* Forgot password
* OTP generation
* OTP verification
* Password reset
* SMTP-based email communication

Email credentials are stored outside the source-controlled configuration.

---

## ⭐ Reviews & Ratings

After completing transactions, users can provide reviews and ratings.

The review system helps users:

* Submit ratings
* Add feedback
* View reviews
* Build trust between marketplace participants

---

## 📊 Reports

The backend contains reporting functionality to support application-level reporting and administrative operations.

Reports can be generated from application data and exported for further analysis.

---

# 🛠️ Technology Stack

## Frontend

| Technology       | Purpose                              |
| ---------------- | ------------------------------------ |
| Angular 19       | Frontend framework                   |
| TypeScript       | Application programming              |
| HTML5            | UI structure                         |
| CSS3             | Styling                              |
| Angular Services | API communication and business logic |
| Angular Forms    | User input and validation            |

## Backend

| Technology            | Purpose                            |
| --------------------- | ---------------------------------- |
| ASP.NET Core Web API  | Backend REST APIs                  |
| C#                    | Backend programming                |
| Entity Framework Core | ORM / database access              |
| ASP.NET Core Identity | Authentication and user management |
| JWT                   | Token-based authentication         |
| REST APIs             | Frontend-backend communication     |

## Database

| Technology            | Purpose                    |
| --------------------- | -------------------------- |
| Microsoft SQL Server  | Relational database        |
| Entity Framework Core | Data access                |
| EF Core Migrations    | Database schema versioning |

## Integrations

| Technology   | Purpose               |
| ------------ | --------------------- |
| Razorpay     | Online payments       |
| SMTP         | Email communication   |
| Google OAuth | Social authentication |
| OpenAI       | AI chatbot            |
| QuestPDF     | PDF/report generation |

## Development Tools

* Visual Studio / Visual Studio Code
* Git
* GitHub
* Postman
* SQL Server Management Studio
* Angular CLI
* .NET CLI

---

# 🏗️ Application Architecture

CropDeal follows a client-server architecture.

```text
┌─────────────────────────────────────────────┐
│              Angular Frontend               │
│                                             │
│  Components                                 │
│  Services                                   │
│  Forms                                      │
│  Authentication                             │
│  Role-based UI                              │
└─────────────────────┬───────────────────────┘
                      │
                      │ HTTP / REST APIs
                      │ JWT Authorization
                      ▼
┌─────────────────────────────────────────────┐
│            ASP.NET Core Web API             │
│                                             │
│  Controllers                                │
│      ↓                                      │
│  DTOs                                        │
│      ↓                                      │
│  Interfaces / Services                      │
│      ↓                                      │
│  Repositories                               │
│      ↓                                      │
│  Entity Framework Core                      │
└─────────────────────┬───────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────┐
│              SQL Server                     │
│                                             │
│  Users                                       │
│  Crops                                       │
│  Listings                                    │
│  Negotiations                                │
│  Transactions                                │
│  Payments                                    │
│  Reviews                                     │
│  Subscriptions                               │
│  Pickups                                     │
└─────────────────────────────────────────────┘
```

External services such as Razorpay, Google Authentication, SMTP, and OpenAI are integrated through backend services.

---

# 🔄 High-Level Application Flow

```text
                    CropDeal
                       │
        ┌──────────────┼──────────────┐
        │              │              │
        ▼              ▼              ▼
     Farmer          Dealer         Admin
        │              │              │
        │              │              │
        ▼              ▼              ▼
 Create Listing    Browse Crops    Manage Users
        │              │           & Operations
        │              │
        └──────┐  ┌────┘
               ▼  ▼
          Negotiation
               │
               ▼
         Agreed Price
               │
               ▼
            Payment
               │
               ▼
          Transaction
               │
               ▼
       Pickup Scheduling
               │
               ▼
            Delivery
               │
               ▼
         Review & Rating
```

---

# 📂 Project Structure

The repository contains the frontend, backend, and test projects in a single repository.

```text
CropDealTrial2/
│
├── CropDeal/
│   │
│   ├── Controllers/
│   │   ├── AddressController.cs
│   │   ├── AdminController.cs
│   │   ├── AuthController.cs
│   │   ├── BankAccountController.cs
│   │   ├── ChatbotController.cs
│   │   ├── ChatController.cs
│   │   ├── CropController.cs
│   │   ├── CropListingController.cs
│   │   ├── NotificationController.cs
│   │   ├── PaymentController.cs
│   │   ├── PickupController.cs
│   │   ├── PriceNegotiationController.cs
│   │   ├── ReportController.cs
│   │   ├── ReviewController.cs
│   │   ├── SubscriptionController.cs
│   │   ├── TransactionController.cs
│   │   └── UserProfileController.cs
│   │
│   ├── DTO/
│   ├── Interface/
│   ├── Middlewares/
│   ├── Migrations/
│   ├── Models/
│   ├── Repository/
│   ├── Properties/
│   ├── wwwroot/
│   ├── Program.cs
│   ├── GlobalUsing.cs
│   └── CropDeal.csproj
│
├── cropfront/
│   ├── src/
│   │   ├── components/
│   │   ├── services/
│   │   ├── models/
│   │   └── ...
│   ├── angular.json
│   ├── package.json
│   └── tsconfig.json
│
├── Tests/
│   └── CropDeal.UnitTests/
│
├── CropDeal-CaseStudy.sln
├── EXCEPTION_HANDLING_README.md
├── CHATBOT_SETUP.md
├── .gitignore
└── README.md
```

---

# 🗄️ Database & Entity Framework Core

The backend uses **Entity Framework Core** for database access.

The application follows a Code First approach with EF Core migrations.

### Main domain areas include:

* Users
* User Profiles
* Addresses
* Bank Accounts
* Crops
* Crop Listings
* Price Negotiations
* Subscriptions
* Payments
* Transactions
* Pickups
* Reviews
* Notifications

Database migrations are maintained under:

```text
CropDeal/Migrations/
```

To apply migrations locally:

```bash
dotnet ef database update
```

> Make sure your local database connection is configured before running migrations.

---

# 🧩 Backend API Modules

The backend is organized around separate controllers for different business capabilities.

| Controller                   | Responsibility                         |
| ---------------------------- | -------------------------------------- |
| `AuthController`             | Registration, login and authentication |
| `AdminController`            | Administrative operations              |
| `CropController`             | Crop-related operations                |
| `CropListingController`      | Crop listing management                |
| `PriceNegotiationController` | Farmer-dealer negotiation              |
| `PaymentController`          | Payment processing                     |
| `SubscriptionController`     | Dealer subscriptions                   |
| `TransactionController`      | Transaction management                 |
| `PickupController`           | Pickup scheduling and tracking         |
| `ReviewController`           | Reviews and ratings                    |
| `NotificationController`     | Notifications                          |
| `UserProfileController`      | User profile management                |
| `BankAccountController`      | Bank account management                |
| `ChatController`             | AI chatbot interaction                 |
| `ChatbotController`          | Chatbot-related functionality          |
| `ReportController`           | Reports                                |

---

# 🛡️ Exception Handling

The backend includes centralized exception handling through custom middleware.

```text
HTTP Request
     ↓
Controller
     ↓
Service / Repository
     ↓
Exception
     ↓
Global Exception Middleware
     ↓
Standardized API Response
```

The middleware helps prevent repetitive exception-handling logic across individual controllers.

Relevant implementation:

```text
CropDeal/Middlewares/
```

---

# 🔑 Configuration

Sensitive configuration values are **not committed to GitHub**.

The repository contains an example configuration file:

```text
CropDeal/appsettings.example.json
```

Create your local configuration file:

```text
CropDeal/appsettings.json
```

and configure your own values.

Example structure:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_CONNECTION_STRING"
  },

  "JWT": {
    "Secret": "YOUR_JWT_SECRET",
    "ValidIssuer": "CropDealAPI",
    "ValidAudience": "YOUR_AUDIENCE"
  },

  "Authentication": {
    "Google": {
      "ClientId": "YOUR_GOOGLE_CLIENT_ID",
      "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
    }
  },

  "Razorpay": {
    "Key": "YOUR_RAZORPAY_KEY",
    "Secret": "YOUR_RAZORPAY_SECRET"
  },

  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": "587",
    "Username": "YOUR_EMAIL",
    "Password": "YOUR_EMAIL_APP_PASSWORD"
  },

  "OpenAI": {
    "ApiKey": "YOUR_OPENAI_API_KEY",
    "Model": "gpt-3.5-turbo"
  }
}
```

### ⚠️ Security

Never commit:

* API keys
* Passwords
* JWT secrets
* OAuth client secrets
* Razorpay secrets
* SMTP passwords
* Database credentials

These files are excluded using `.gitignore`.

---

# 🚀 Getting Started

## Prerequisites

Install the following before running the application:

* .NET SDK
* Node.js
* npm
* Angular CLI
* SQL Server
* Git

Verify installations:

```bash
dotnet --version
node --version
npm --version
ng version
```

---

# ⚙️ Backend Setup

Navigate to the backend:

```bash
cd CropDeal
```

Restore dependencies:

```bash
dotnet restore
```

Configure your local:

```text
appsettings.json
```

with your database and external service credentials.

Apply EF Core migrations:

```bash
dotnet ef database update
```

Build the backend:

```bash
dotnet build
```

Run the API:

```bash
dotnet run
```

The backend URL depends on the local launch configuration.

---

# 🖥️ Frontend Setup

Open another terminal and navigate to the Angular application:

```bash
cd cropfront
```

Install dependencies:

```bash
npm install
```

Run the Angular development server:

```bash
ng serve
```

Then open the URL displayed by Angular CLI, typically:

```text
http://localhost:4200
```

---

# 🔗 Frontend ↔ Backend Communication

The Angular frontend communicates with the ASP.NET Core backend through REST APIs.

```text
Angular Component
       ↓
Angular Service
       ↓
HTTP Request
       ↓
ASP.NET Core Controller
       ↓
Business Logic
       ↓
Repository
       ↓
Entity Framework Core
       ↓
SQL Server
```

JWT tokens are used to authorize protected API requests.

---

# 🧪 Testing

The repository includes a dedicated test project:

```text
Tests/CropDeal.UnitTests/
```

Run tests using:

```bash
dotnet test
```

To build the complete solution:

```bash
dotnet build CropDeal-CaseStudy.sln
```

---

# 📸 Screenshots

The following screenshots showcase the major features and user workflows implemented in CropDeal.

---

## 🏠 Home & Authentication

### Home Page

<img width="1917" height="928" alt="Crop Deal - home page" src="https://github.com/user-attachments/assets/cf4bdfd6-9af9-4685-82e6-8b6146159327" />

### Login & Signup

<img width="1920" height="1080" alt="Crop Deal - Login and Signup feature" src="https://github.com/user-attachments/assets/a8861bc3-e12b-4283-85c4-adde49256e06" />

---

## 👨‍💼 Admin Features

### Admin Dashboard

<img width="1920" height="1080" alt="Crop Deal - Admin Dashboard" src="https://github.com/user-attachments/assets/4c2aab98-6736-450c-b227-75357a7de353" />

### Manage Users

<img width="1920" height="1080" alt="Crop Deal- Admin manage user feature" src="https://github.com/user-attachments/assets/ab54e136-b810-4ca9-b6b8-85273063e403" />

### Add Crops

<img width="1920" height="1080" alt="Crop Deal-Admin Crop Adding feature" src="https://github.com/user-attachments/assets/6672b218-d457-4983-9a1e-1a5f3c4cc557" />

### Report Generation

<img width="1920" height="1080" alt="Crop Deal - Admin Report generation feature" src="https://github.com/user-attachments/assets/105aac73-47a7-4797-8063-7e16a27de74f" />

---

## 👨‍🌾 Farmer Features

### Farmer Dashboard

<img width="1920" height="1080" alt="Crop Deal - Farmer dashboard" src="https://github.com/user-attachments/assets/b17e28c6-3a60-4f19-9059-e57b1a89cc50" />

### Crop Listing

<img width="1920" height="1080" alt="Crop Deal - Farmer Listing feature" src="https://github.com/user-attachments/assets/7ebab913-6f9a-434a-bd59-1198b3cfb92f" />

### Price Negotiation

<img width="1920" height="1080" alt="Crop Deal - farmer Negotiation" src="https://github.com/user-attachments/assets/9ae47603-85a4-4660-b08e-79d22f159a3d" />

### Subscription

<img width="1920" height="1080" alt="Crop Deal - farmer Subscriptions feature" src="https://github.com/user-attachments/assets/daa68a09-b9f1-4d9e-84dc-bb572098d0f7" />

### Pickup Scheduling

<img width="1920" height="1080" alt="Crop Deal - Farmer Pickup Schedule" src="https://github.com/user-attachments/assets/f8f90e49-4f86-4c51-bb91-194dcbedd8fa" />

---

## 🏪 Dealer Features

### Dealer Dashboard

<img width="1920" height="1080" alt="Crop Deal - Dealaer dashboard" src="https://github.com/user-attachments/assets/969e5b12-a85d-4e2a-be28-911f12264cf9" />

### View Listings & Subscription

<img width="1920" height="1080" alt="Crop Deal - Dealer View listing and Subscription" src="https://github.com/user-attachments/assets/9f45b15f-3bfd-48fd-8b67-cf208c0c60fc" />

### Subscription

<img width="1920" height="1080" alt="Crop Deal - Dealer - Subscription feature" src="https://github.com/user-attachments/assets/e448aae6-c34c-46cd-a143-a671163e1914" />

### Price Negotiation

<img width="1920" height="1080" alt="Crop Deal - Dealer Negotiation" src="https://github.com/user-attachments/assets/ae259b45-b99c-42f9-9570-b3f81f5c4280" />

### Pickup Scheduling

<img width="1920" height="1080" alt="Crop Deal - Dealer Pickup Scheduling" src="https://github.com/user-attachments/assets/f34df6c2-bc04-4505-874d-d4be709b9851" />

### Payment

<img width="1920" height="1080" alt="Crop Deal - Dealer Payment feature" src="https://github.com/user-attachments/assets/177458b1-f79e-411a-8147-1c59d679617d" />

### Invoice Generation

<img width="1920" height="1080" alt="Crop Deal - Invoice generation" src="https://github.com/user-attachments/assets/2ba3cd19-4ebc-40ef-b277-b37263f36fdc" />

### Ratings & Reviews

<img width="1920" height="1080" alt="Crop Dealer - Dealer Rating feature" src="https://github.com/user-attachments/assets/f868c1a9-4ef1-4310-b0d6-30ecec527f59" />




# 📈 Future Enhancements

Potential improvements include:

* Microservices-based architecture
* Advanced crop recommendation system
* Real-time negotiation using SignalR
* Real-time notifications
* Advanced analytics dashboard
* Cloud deployment
* Containerization using Docker
* CI/CD pipeline
* Azure cloud integration
* Improved AI-based agricultural recommendations
* Location-based delivery optimization
* More advanced search and filtering
* Automated testing and increased code coverage

---

# 💡 What This Project Demonstrates

CropDeal demonstrates practical experience with:

* Full-stack application development
* Angular application architecture
* ASP.NET Core Web API development
* C# programming
* Entity Framework Core
* SQL Server
* REST API design
* JWT authentication
* Role-based authorization
* Payment gateway integration
* OAuth authentication
* SMTP/email integration
* OTP-based password reset
* AI API integration
* Exception-handling middleware
* Database migrations
* Unit testing
* Git and GitHub
* Frontend-backend integration

---

# 📚 Learning Outcomes

Through CropDeal, the project demonstrates how a complete business application can be designed and developed from frontend to backend and database.

The project covers the complete lifecycle:

```text
Requirements
     ↓
UI Development
     ↓
API Development
     ↓
Business Logic
     ↓
Database Design
     ↓
Authentication
     ↓
Third-party Integrations
     ↓
Testing
     ↓
Version Control
```

---

# 👩‍💻 Author

**Sailee Mulik**

Full-Stack Developer | .NET | Angular | SQL | Azure

### Technical Interests

* ASP.NET Core
* Angular
* SQL Server
* Azure
* Full-Stack Development
* AI / RAG Applications
* Cloud Technologies

---

# ⭐ Project

If you find this project useful or interesting, consider giving the repository a ⭐ on GitHub.

---

## 📄 License

This project is intended for educational, portfolio, and demonstration purposes.
