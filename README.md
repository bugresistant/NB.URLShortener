![3b257b7e-5d8a-4c7b-bb0c-f94f75386096](https://github.com/user-attachments/assets/15656410-1b96-48b1-8368-d5a9639804e2)
*the picture is made by AI*

⚠️ Not production-ready — This is a demonstration project. Use it as a reference or for experimentation. You can fork it or suggest any improvements. Contributions welcome
📌 Overview

    Functional prototype of small URL shortener API with web-interface.
    Basic UI included for testing — treat it as a conceptual preview.

🚀 Features

    Shorten URLs via API endpoints.
    Redirect to original URLs.
    Built with modern ASP.NET Core practices.
    RESTful architecture.

🛠️ Getting Started
Prerequisites

    .NET 8+ SDK 

Installation

    Clone the repo:

    git clone https://github.com/bugresistant/url-shortener.git

    Configure the project:
    
    Tweak appseting.json to your preferences, I suggest deleting segment with additional Kestrel configuration
    as it was made to be used with docker container.

    Restore dependencies:

    dotnet restore

    Run the project:

    dotnet run

🔧 API Usage (via CLI)

Example with curl:

# Shorten a URL
curl -X POST https://localhost:7081/api/urls/shorten \
  -H "Content-Type: application/json" \
  -d "{\"originalUrl\": \"https://example.com\"}" \
  -k

# Redirect (use the returned short key)
curl -L https://localhost:7081/r/{KEY}

⚠️ Frontend Disclaimer

The included UI is "ViBe CoDeD" and intended for basic testing as the API was intended to be used with utilities like curl.
Maybe, one day I'll rewrite it myself.
TIPS AND FACTS:

    Built quickly to demonstrate API functionality.
    Not optimized for production.
    Use tools like curl, Postman, or API clients for "serious" interactions.

[You can try out this small demo](https://urlshortener.lol)
