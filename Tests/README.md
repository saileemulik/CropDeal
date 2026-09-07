# 🧪 Simple Tests for CropDeal App

## What is this? 🤔
Think of tests like playing pretend! We pretend to use the app and check if it works correctly.

## What's inside? 📁
```
Tests/
└── CropDeal.UnitTests/          (Our test playground)
    ├── Controllers/             (Tests for app controllers)
    │   ├── AuthControllerTests.cs    (Tests for login/signup)
    │   └── UserControllerTests.cs    (Tests for user profile)
    ├── Helpers/                 (Helper tools)
    │   └── TestDbContextFactory.cs   (Makes fake databases)
    ├── CropDeal.UnitTests.csproj    (Project settings)
    └── GlobalUsings.cs              (Imports all our toys)
```

## What makes it special? ✨
- 🏠 **Safe playground** - Won't break the real app
- 🎭 **Uses fake things** - Fake database, fake users
- 🎮 **Easy to understand** - Written like a story
- 🚀 **Fast** - Runs quickly
- 🔄 **Fresh start** - Each test starts clean

## How to run the tests? 🏃‍♂️
```bash
cd Tests/CropDeal.UnitTests
dotnet test
```

## What do we test? 🎯
- **Login Tests**: Can people sign up? Can they log in?
- **Profile Tests**: Can people see their info? Can they change it?

## How does it work? 🛠️
1. We make **fake helpers** (like toy versions of real things)
2. We tell them what to pretend
3. We try using the app
4. We check if it did what we expected

## Example Test Story 📖
```
"Let's pretend John wants to sign up:
1. John gives good information
2. We tell our fake helper to say 'OK'
3. We try to sign John up
4. We check: Did it work? ✅"
```