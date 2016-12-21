# Pixiv Mirror
Rehost images from Pixiv. Has a JSON API and a JQuery frontend.

# Setup
- Copy appsettings.default.json to appsettings.json and edit settings.
- Run dotnet publish in Pixiv\src\Pixiv.
- Copy Pixiv\src\Pixiv\bin\Debug\netcoreapp1.1\publish to your web server.
- Run with dotnet Pixiv.dll