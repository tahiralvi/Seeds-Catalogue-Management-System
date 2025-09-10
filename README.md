# 🌱 Seed Catalog Management System

A comprehensive web application for managing and browsing seed inventory, built with modern ASP.NET Core technologies.

## 📋 Project Overview

This application allows garden centers, nurseries, and agricultural businesses to efficiently manage their seed inventory through an intuitive web interface. Users can add new seed entries to the database and search for seeds based on various criteria.

## ✨ Features

### ✅ Task 1: Data Entry Form
- **User-Friendly Form**: Intuitive interface for entering seed details including:
  - Seed ID (unique identifier)
  - Title/Name of the seed
  - Category (vegetables, flowers, herbs, etc.)
  - Color characteristics
  - Cost per Kg pricing
- **Form Controls**:
  - Submit button for saving records to the database
  - Reset button to clear form fields
  - Validation for required fields and data types
- **Database Integration**: Seamless connection to SQL Server with successful data persistence

### ✅ Task 2: Advanced Search functionality
- **Dual-Level Search**: 
  - First filter by Category to narrow down options
  - Then search by Title within selected category
- **Soft Display**: Elegant presentation of seed information including:
  - Complete seed details
  - Visual representation of color characteristics
  - Pricing information
- **Responsive Results**: Real-time filtering and display of search results

### ✅ Task 3: Enhanced UI/UX
- **Thematic Background**: Beautiful gardening/agriculture-related background image
- **Professional Styling**: Clean, responsive design using CSS
- **User-Centric Interface**: Intuitive navigation and visual hierarchy

## 🛠️ Technology Stack

- **Frontend**: HTML5, CSS3, JavaScript
- **Backend Framework**: ASP.NET Core 8
- **Architecture**: MVC Pattern with Razor Pages
- **Data Access**: Entity Framework Core (ORM)
- **Database**: Microsoft SQL Server
- **Styling**: Modern CSS with responsive design principles

## 🗂️ Project Structure

```
SeedCatalogProject/
│
├── Controllers/
│   └── SeedController.cs        # Handles form submission and search logic
│
├── Models/
│   ├── Seed.cs                  # Main data model
│   └── SeedContext.cs           # Database context
│
├── Views/
│   ├── Create.cshtml           # Data entry form
│   ├── Search.cshtml           # Search interface
│   └── Details.cshtml          # Seed information display
│
├── wwwroot/
│   ├── css/
│   │   └── site.css            # Custom styling
│   └── images/
│       └── background.jpg      # Thematic background
│
└── appsettings.json            # Database configuration
```

## 📊 Database Schema

The application uses a structured SQL Server database with the following main table:

**Seeds Table**:
- `SeedId` (Primary Key, Int)
- `Title` (NVARCHAR(100))
- `Category` (NVARCHAR(50))
- `Color` (NVARCHAR(30))
- `CostPerKg` (DECIMAL)
- `CreatedDate` (DATETIME)

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or Express edition)
- Web browser with JavaScript support

### Installation Steps

1. **Clone the Repository**
   ```bash
   git clone https://github.com/yourusername/seed-catalog-system.git
   cd seed-catalog-system
   ```

2. **Database Setup**
   - Update connection string in `appsettings.json`
   - Run EF Core migrations:
   ```bash
   dotnet ef database update
   ```

3. **Run the Application**
   ```bash
   dotnet run
   ```
   Navigate to `https://localhost:7000` in your browser

## 🎯 How to Use

### Adding New Seeds
1. Navigate to the "Add Seed" page
2. Fill in the form fields (SeedId, Title, Category, Color, CostPerKg)
3. Click "Submit" to save to database or "Reset" to clear the form

### Searching Seeds
1. Go to the "Search" page
2. Select a category from the dropdown menu
3. Choose a specific title from the filtered list
4. View complete seed details in a visually appealing format

## 🌟 Key Technical Implementations

- **EF Core Integration**: Efficient database operations using Entity Framework
- **Model Binding**: Seamless data transfer between views and controllers
- **Form Validation**: Client and server-side validation for data integrity
- **Search Algorithms**: Optimized filtering and searching capabilities
- **Responsive Design**: Mobile-friendly interface using CSS Flexbox/Grid

## 📱 UI/UX Features

- Clean, agricultural-themed design
- Responsive layout for all device sizes
- Intuitive form controls with visual feedback
- Thematic color scheme matching the gardening context
- Professional typography and spacing

## 🔧 Configuration

Modify `appsettings.json` for your environment:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SeedsDB;Trusted_Connection=True;TrustServerCertificate=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

## 🤝 Contributing

1. Fork the project
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details.

## 🆓 Free and Open Source

This project is completely free to use and modify for educational and commercial purposes.
