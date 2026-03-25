# UBB-SE-2026-Shroomsa

\# BoardRent — User Management Module



\## Prerequisites

\- Visual Studio 2026 (WinUI 3 / Windows App SDK workload installed)

\- SQL Server LocalDB (comes with Visual Studio — verify with `sqllocaldb info`)



\## Database setup

1\. Open a terminal and run:

```

&#x20;  sqllocaldb create "BoardRent" -s

```

2\. Open `Data/AppDbContext.cs` and confirm the connection string:

```

&#x20;  Server=(localdb)\\BoardRent;Database=BoardRentDb;Trusted\_Connection=True;

```

3\. The tables are created automatically on first launch via `EnsureCreated()`.



\## Default admin account (for testing)

| Field    | Value        |

|----------|--------------|

| Username | admin        |

| Password | Admin@1234   |

| Role     | Administrator|



> Change this before any real demo.



\## Running the app

1\. Clone the repo

2\. Open `BoardRent.sln` in Visual Studio

3\. Set `BoardRent` as the startup project

4\. Press F5



\## Git workflow

\- `main` has no direct push protection — please respect the workflow below

\- Each feature lives on its own branch (see below)

\- \*\*Merge strategy — follow this exactly:\*\*

&#x20; 1. Finish your work on your feature branch

&#x20; 2. `git checkout main \&\& git pull`

&#x20; 3. `git checkout feature/your-branch`

&#x20; 4. `git merge main` — resolve any conflicts

&#x20; 5. Verify the app still runs

&#x20; 6. Open a Pull Request to merge back into `main`

&#x20; 7. \*\*Never rebase — always merge\*\*



\## Branch ownership

| Branch                 | Owner      |

|------------------------|------------|

| `feature/domain-dtos`  | Teammate 1 |

| `feature/repositories` | Teammate 2 |

| `feature/auth`         | Teammate 3 |

| `feature/profile`      | Teammate 4 |

| `feature/admin`        | Teammate 5 |

