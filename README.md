# Bookworm

Bookworm is a comprehensive web application designed for managing an online bookstore. It provides functionalities for managing books, user accounts, shopping carts, and more. The application is built using ASP.NET Core MVC and leverages various technologies to deliver a robust and user-friendly experience.

## Features

### User Management
- **Registration and Login**: Users can register and log in to their accounts.
- **Role Management**: Supports different user roles such as Admin, Author, and Customer.
- **Profile Management**: Users can update their profiles, including profile pictures.

### Book Management
- **CRUD Operations**: Admins and Authors can create, read, update, and delete books.
- **Categories**: Books can be categorized for better organization and searchability.
- **Reviews and Ratings**: Users can leave reviews and ratings for books.

### Shopping Cart
- **Add to Cart**: Users can add books to their shopping cart.
- **Checkout**: Integrated with Stripe for secure payment processing.
- **Order History**: Users can view their past purchases.

### Admin Panel
- **Dashboard**: Admins have access to a dashboard for managing the application.
- **User Management**: Admins can manage user roles and permissions.
- **Book Management**: Admins can manage all books in the store.

### Other Features
- **Search**: Users can search for books by title, author, or category.
- **Responsive Design**: The application is designed to be responsive and works well on all devices.
- **Error Handling**: Custom error pages for handling different types of errors.

## Technologies Used

- **ASP.NET Core MVC**: For building the web application.
- **Entity Framework Core**: For database interactions.
- **ASP.NET Core Identity**: For managing user authentication and authorization.
- **Stripe**: For handling payments.
- **Bootstrap**: For responsive design.
- **jQuery**: For client-side scripting.
- **MailKit**: For sending emails.
- **HtmlAgilityPack**: For parsing HTML.

## Project Structure

### Key Files and Directories

- **Controllers/**: Contains the controllers for handling HTTP requests.
- **Models/**: Contains the data models used in the application.
- **Views/**: Contains the Razor views for rendering HTML pages.
- **wwwroot/**: Contains static files such as CSS, JavaScript, and images.
- **Data/**: Contains the database context and migration files.
- **Interfaces/**: Contains the interfaces for the repository pattern.
- **Repository/**: Contains the repository implementations.
- **ViewModel/**: Contains the view models used for passing data between controllers and views.
- **Program.cs**: The entry point of the application.
- **appsettings.json**: Configuration file for application settings.

## Getting Started

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Node.js](https://nodejs.org/) (for managing client-side dependencies)

### Installation

1. Clone the repository:
   ```sh
   git clone https://github.com/yourusername/bookworm.git
   cd bookworm
