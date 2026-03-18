# BoardRent — User Management Module

## Prerequisites
- Visual Studio 2026 (WinUI 3 / Windows App SDK workload installed)
- SQL Server LocalDB (comes with Visual Studio — verify with `sqllocaldb info`)

## Database setup
1. Open a terminal and run:
```
   sqllocaldb create "BoardRent" -s
```
2. Open `Data/AppDbContext.cs` and confirm the connection string:
```
   Server=(localdb)\BoardRent;Database=BoardRentDb;Trusted_Connection=True;
```
3. The tables are created automatically on first launch via `EnsureCreated()`.

## Default admin account (for testing)
| Field    | Value        |
|----------|--------------|
| Username | admin        |
| Password | Admin@1234   |
| Role     | Administrator|

> Change this before any real demo.

## Running the app
1. Clone the repo
2. Open `BoardRent.sln` in Visual Studio
3. Set `BoardRent` as the startup project
4. Press F5

## Git workflow
- `main` has no direct push protection — please respect the workflow below
- Each feature lives on its own branch (see below)
- **Merge strategy — follow this exactly:**
  1. Finish your work on your feature branch
  2. `git checkout main && git pull`
  3. `git checkout feature/your-branch`
  4. `git merge main` — resolve any conflicts
  5. Verify the app still runs
  6. Open a Pull Request to merge back into `main`
  7. **Never rebase — always merge**

## Development conventions
- All ViewModels must extend `BaseViewModel`
- All service methods must return `ServiceResult<T>`

## Branch ownership
| Branch                 | Owner      |
|------------------------|------------|
| `feature/domain-dtos`  | Teammate 1 |
| `feature/repositories` | Teammate 2 |
| `feature/auth`         | Teammate 3 |
| `feature/profile`      | Teammate 4 |
| `feature/admin`        | Teammate 5 |
