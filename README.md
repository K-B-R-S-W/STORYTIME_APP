# 🎬 StoryTimeDEV

A **modern Windows desktop application** built with WPF (.NET Framework) to manage storytelling and narrative content — perfect for movie or narrative booking systems. Integrates robustly with MongoDB and emphasizes clean configuration practices.

## ✨ Features

* 🎨 **Modern UI/UX**
  * Sleek WPF interface with smooth animations
  * FontAwesome Sharp icons for visual appeal
  * Gradient-based color schemes
  * Responsive and interactive elements
  * Custom-styled controls and buttons
  * Draggable windows with custom title bar

* 🔐 **User Authentication System**
  * Secure user login functionality
  * Username and password authentication
  * Self-service password reset
  * User session management
  * Role-based access control
  * User account management

* 🗃️ **MongoDB Integration**
  * Efficient data storage and retrieval
  * Real-time database operations
  * Secure connection handling
  * Automated database initialization
  * Collection management
  * Query optimization

* ⚙️ **Configuration Management**
  * Flexible JSON-based settings
  * Environment-specific configurations
  * Dynamic configuration loading
  * Secure credential management
  * Connection string management
  * Error handling and logging

* 💼 **Core Functionality**
  * Story and narrative management
  * Movie booking system integration
  * Content organization tools
  * Search and filter capabilities
  * Data validation and verification
  * Error handling and recovery

## 📦 Dependencies

### Main Packages

| Package | Version |
|---------|---------|
| FontAwesome.Sharp | 6.3.0 |
| Microsoft.Extensions.Configuration | 7.0.0 |
| MongoDB.Driver | 2.22.0 |
| System.Text.Json | 6.0.0 |

### Supporting Packages

* `DnsClient` (1.6.1)
* AWS SDK components (for MongoDB cloud support)
* Additional `Microsoft.Extensions.*` packages

## 🛠 Prerequisites

* ✅ .NET Framework 4.8
* ✅ Visual Studio 2019 or later with:
   * `.NET Desktop Development` workload
* ✅ MongoDB Community Server
* ⚙️ (Optional) Node.js – only if you plan to use Node features

## 🚀 Getting Started

### 1. Clone the Repository

```powershell
git clone https://github.com/K-B-R-S-W/STORYTIME_APP
cd StortytimeDEV-menu
```

### 2. Install Dependencies

```powershell
# Restore NuGet packages
nuget restore StortytimeDEV.sln

# (Optional) Install Node packages
npm install
```

### 3. MongoDB Configuration

Create a `appsettings.json` file in the project root:

```json
{
  "MongoDB": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "storytimedev"
  }
}
```

Ensure MongoDB service is **running** locally and accessible.

### 4. Build & Run the Application

* Open `StortytimeDEV.sln` in Visual Studio
* Press `F5` to build and launch in Debug mode
* Or use `Ctrl + F5` to run without debugging

## 🧩 Project Structure

```bash
📁 StortytimeDEV-menu/
├── 📄 .gitignore                
├── 📄 StortytimeDEV.sln         
├── 📄 README.md                 
├── 📄 package.json              
└── 📁 StortytimeDEV/           
    ├── 📁 Config/              
    ├── 📁 Views/              
    ├── 📁 Models/             
    ├── 📁 Services/           
    ├── 📁 Images/             
    └── 📄 appsettings.json    
```

## ❗ Troubleshooting

* **MongoDB connection issues?**
   * Check if MongoDB service is running
   * Verify your `ConnectionString` and `DatabaseName`
   * Make sure port `27017` is not blocked by a firewall

* **Build fails in Visual Studio?**
   * Use `Clean Solution` then `Rebuild`
   * Ensure all NuGet packages are restored
   * Check the Output window for error details

## 🤝 Contributing

This project is a **Work in Progress** — contributions are encouraged and appreciated!

1. Fork the repo
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Commit your changes
4. Push the branch: `git push origin feature/your-feature`
5. Open a Pull Request

## 📜 License

This project is open-source under the MIT License.

## 📮 Support

- **📧 Email:** [k.b.ravindusankalpaac@gmail.com](mailto:k.b.ravindusankalpaac@gmail.com)  
- **🐞 Bug Reports:** [GitHub Issues](https://github.com/K-B-R-S-W/STORYTIME_APP/issues)  
- **📚 Documentation:** See the project [Wiki](https://github.com/K-B-R-S-W/STORYTIME_APP/wiki)  
- **💭 Discussions:** Join the [GitHub Discussions](https://github.com/K-B-R-S-W/STORYTIME_APP/discussions)

## ⭐ Support This Project

If you find this project helpful, please consider giving it a **⭐ star** on GitHub!

## 🙏 Special Thanks

A heartfelt thank you to **Sahindu Gayanuka** The Owner of **SKIDROW** for their invaluable contributions and support to this project!

* 🌟 GitHub: [SahiDemon](https://github.com/SahiDemon)
* 🎮 Discord Server: [Join the Community](https://discord.gg/TFv9yGa3sc)

Their Lead expertise and guidance have been instrumental in making this project better!