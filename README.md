# BoardRent — User Management & Profile Section

## Prerequisites
- Visual Studio 2026 (WinUI 3 / Windows App SDK workload installed)
- SQL Server LocalDB (comes with Visual Studio — verify with `sqllocaldb info`)

---

## Database setup
1. Open a terminal and run:
```
   sqllocaldb start MSSQLLocalDB
```
2. The connection string in `Data/AppDbContext.cs` is:
```
   Server=(localdb)\MSSQLLocalDB;Database=BoardRentDb;Trusted_Connection=True;
```
3. The tables and seed data are created automatically on first launch via `EnsureCreated()`.

### Troubleshooting
**"Cannot open database BoardRentDb"**  
Run `sqllocaldb start MSSQLLocalDB` in the terminal then restart the app.

**"Cannot create file BoardRentDb.mdf because it already exists"**  
Go to `C:\Users\<yourname>\` and delete `BoardRentDb.mdf` and `BoardRentDb_log.ldf`, then restart the app.

---

## Default admin account - for testing
| Field    | Value         |
|----------|---------------|
| Username | admin         |
| Password | Admin@1234    |
| Role     | Administrator |

> ⚠️ Change this before any real demo.

---

## Running the app
1. Clone the repo:
```
   git clone <repo-url>
```
2. Open the cloned folder and double-click `BoardRent.slnx`
3. Visual Studio will restore NuGet packages automatically on first load
4. Follow the **Database setup** steps above
5. Press F5

---

## Development conventions
- All ViewModels must extend `BaseViewModel`
- All service methods must return `ServiceResult<T>`
- Use `App.NavigateTo(typeof(YourPage))` for navigation
- Never push directly to `main` — always use your feature branch and open a PR

---

## Git workflow
- `main` has no direct push protection — please respect the workflow below
- Each feature lives on its own branch (see table below)
- **Merge strategy — follow this exactly:**
  1. Finish your work on your feature branch
  2. `git checkout main && git pull`
  3. `git checkout feature/your-branch`
  4. `git merge main` — resolve any conflicts
  5. Verify the app still runs
  6. Open a Pull Request to merge back into `main`
  7. **Never rebase — always merge**

---

## Branch ownership
| Branch                   | Owner      | Scope                        |
|--------------------------|------------|------------------------------|
| `feature/domain-dtos`    | Teammate 1 | Domain models · DTOs · Utils |
| `feature/repositories`   | Teammate 2 | Repositories                 |
| `feature/auth`           | Teammate 3 | Auth · Login · Register      |
| `feature/profile`        | Teammate 4 | User Service · Profile       |
| `feature/admin`          | Teammate 5 | Admin Service · Admin        |
