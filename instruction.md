# 📘 INSTRUCTION.MD
## How to Use This Prompt System — Manpower Contract Project

---

## 📁 File Guide

| File | Purpose |
|---|---|
| `instruction.md` | **This file** — How to use the system |
| `skill.md` | All technical rules, patterns & standards |
| `mainproject.md` | **Master prompt** — Paste this into Claude AI to generate code |

---

## 🚀 Workflow — Step by Step

### PHASE 1 — Before You Generate Code

```
Step 1: Open mainproject.md
Step 2: Scroll to the bottom — "NOW — HERE IS MY SCREEN LAYOUT"
Step 3: Fill in your screen specifications:
        - Screen Name
        - Module Name
        - Primary Table Name
        - Related Lookup Tables
        - Grid Columns
        - Mandatory Fields
        - Auto-fill Fields
        - Cascading Dropdowns
        - Business Rules
Step 4: Attach your Excel screen spec OR paste content
Step 5: Copy the ENTIRE mainproject.md content
Step 6: Paste into Claude AI and send
```

### PHASE 2 — After Code is Generated

```
Step 1:  Run Database Scripts in ORDER:
         01_CreateTables.sql
         02_Auth_Tables.sql        ← NEW (Login, Roles, Permissions)
         03_StoredProcedures.sql
         04_Auth_StoredProcedures.sql ← NEW
         05_SeedData.sql

Step 2:  Update appsettings.json:
         - ConnectionStrings → DefaultConnection
         - JwtSettings → Secret, Issuer, Audience, ExpiryMinutes
         - ApiSettings → BaseUrl (your API project URL)

Step 3:  Run API Project first:
         cd MyProject.API
         dotnet restore
         dotnet run
         → Check: https://localhost:PORT/swagger

Step 4:  Run MVC Project second:
         cd MyProject.MVC
         dotnet restore
         dotnet run
         → Check: https://localhost:PORT/Login

Step 5:  Default Login Credentials:
         Username: admin@company.com
         Password: Admin@123
```

---

## 🎨 Theme / Color — EXTRACTED FROM ACTUAL LOGIN SCREENSHOT

> ✅ Colors extracted from the **Manpower Contract Management** login page.
> Theme: **Brand Red (Brand) + Teal Green (CTA)** on White
> Applied to: Login page, Navbar, Sidebar, Buttons, Table headers, Modals.

---

### 📸 Login Screen Layout (Extracted from Screenshot)

```
┌─────────────────────────────────────────────────────────────────────────┐
│                                                                         │
│  ┌──────────────────────────┬──────────────────────────────────────┐    │
│  │                          │           ◯ (gray decorative circle)│    │
│  │   LEFT PANEL             │                                     │    │
│  │   (Solid Brand Red)    │   "Manpower Contract Management"    │    │
│  │   Background: #db2128    │    (dark heading, bold)             │    │
│  │                          │                                     │    │
│  │                          │   Language: [English (United States)]│    │
│  │   ┌─ Gold accent bar     │            (dropdown)               │    │
│  │   │                      │                                     │    │
│  │   │ "Manpower Contract   │   ┌─────────────────────────────┐   │    │
│  │   │  Management"         │   │ 👤 Username                 │   │    │
│  │   │  (White text, bold)  │   └─────────────────────────────┘   │    │
│  │   │                      │   (light blue bg on focus: #E3F2FD) │    │
│  │                          │                                     │    │
│  │                          │   ┌─────────────────────────────┐   │    │
│  │                          │   │ 🔒 Password            👁  │   │    │
│  │                          │   └─────────────────────────────┘   │    │
│  │                          │                                     │    │
│  │                          │   ☐ Remember Me    Forgot Password? │    │
│  │                          │                    (#009688 teal)   │    │
│  │                          │                                     │    │
│  │                          │   ┌─────────────────────────────┐   │    │
│  │                          │   │      🔑 LOGIN               │   │    │
│  │                          │   │   (Teal #009688, white text) │   │    │
│  │                          │   └─────────────────────────────┘   │    │
│  │                          │                                     │    │
│  │                          │   "Proud to be the part of."       │    │
│  │                          │   (Crimson red text: #db2128)      │    │
│  │                          │           ◯ (gray decorative circle)│    │
│  └──────────────────────────┴──────────────────────────────────────┘    │
│                                                                         │
└─────────────────────────────────────────────────────────────────────────┘
```

---

### 🎨 COLOR PALETTE (Extracted from Actual Screenshot)

```css
/* wwwroot/css/theme.css — Manpower Contract Management Colors */
:root {
  /* ═══ PRIMARY BRAND — Brand Red ═══ */
  --primary-color:          #db2128;   /* Brand red — left panel, accents */
  --primary-dark:           #A31D28;   /* Darker red — hover states */
  --primary-light:          #E8434F;   /* Lighter red — highlights */

  /* ═══ CTA / ACTION — Teal Green ═══ */
  --teal-color:             #009688;   /* Teal — Login btn, Save btn, links */
  --teal-dark:              #00796B;   /* Dark teal — button hover */
  --teal-light:             #4DB6AC;   /* Light teal — highlights */

  /* ═══ ACCENT — Gold/Yellow (Title bar accent) ═══ */
  --accent-gold:            #FFB300;   /* Gold accent bar next to title */
  --accent-gold-light:      #FFD54F;   /* Light gold — badges */

  /* ═══ BACKGROUNDS ═══ */
  --login-left-bg:          #db2128;   /* Solid crimson red */
  --sidebar-bg:             #1A1A2E;   /* Dark sidebar (navy/charcoal) */
  --sidebar-hover:          #2A2A42;   /* Sidebar item hover */
  --sidebar-active:         #db2128;   /* Active item — brand red highlight */
  --navbar-bg:              #FFFFFF;   /* White top navbar */
  --navbar-border:          #E0E0E0;   /* Bottom border line */
  --page-bg:                #F0F2F5;   /* Light gray page background */
  --card-bg:                #FFFFFF;   /* White cards */

  /* ═══ TEXT COLORS ═══ */
  --text-on-dark:           #FFFFFF;   /* White text on dark/red bg */
  --text-on-dark-muted:     #E0E0E0;   /* Muted text on dark bg */
  --text-primary:           #212529;   /* Main body text */
  --text-secondary:         #6C757D;   /* Secondary/muted text */
  --text-heading:           #212529;   /* Headings — dark */
  --text-brand-red:         #db2128;   /* Red branded text (footer tagline) */

  /* ═══ TABLE / GRID ═══ */
  --table-header-bg:        #db2128;   /* Crimson red header */
  --table-header-text:      #FFFFFF;   /* White header text */
  --table-row-hover:        #FFF3F4;   /* Very light pink hover */
  --table-border:           #DEE2E6;   /* Light gray borders */

  /* ═══ MODAL ═══ */
  --modal-header-bg:        #db2128;   /* Red modal header */
  --modal-header-text:      #FFFFFF;   /* White modal title */

  /* ═══ FORM ELEMENTS ═══ */
  --input-border:           #CED4DA;   /* Default input border */
  --input-focus-border:     #009688;   /* Teal focus border */
  --input-focus-bg:         #E3F2FD;   /* Light blue focus background */
  --input-focus-shadow:     rgba(0,150,136,0.25);  /* Teal glow */
  --input-bg:               #FFFFFF;
  --input-disabled-bg:      #E9ECEF;   /* Readonly/disabled fields */

  /* ═══ BUTTONS ═══ */
  --btn-primary-bg:         #009688;   /* Teal — Login, Save, Submit */
  --btn-primary-text:       #FFFFFF;
  --btn-primary-hover:      #00796B;   /* Dark teal hover */
  --btn-secondary-bg:       #db2128;   /* Red — branded secondary */
  --btn-secondary-text:     #FFFFFF;
  --btn-outline-color:      #009688;   /* Teal outline buttons */
  --btn-danger-bg:          #DC3545;   /* Red — delete */
  --btn-success-bg:         #28A745;   /* Green — success states */

  /* ═══ ALERTS / STATUS ═══ */
  --success-color:          #28A745;
  --warning-color:          #FFC107;
  --danger-color:           #DC3545;
  --info-color:             #17A2B8;

  /* ═══ DECORATIVE ═══ */
  --circle-decoration:      #F0F0F0;   /* Light gray decorative circles */
}
```

---

### 🖌️ WHERE EACH COLOR IS APPLIED

| UI Element | Color Variable | Hex Value | Visual |
|---|---|---|---|
| **Login left panel** | `--login-left-bg` | `#db2128` | 🟥 Solid crimson red |
| **Login left panel text** | `--text-on-dark` | `#FFFFFF` | ⬜ White on red |
| **Gold accent bar** | `--accent-gold` | `#FFB300` | 🟨 Gold vertical bar |
| **Login right panel** | `--card-bg` | `#FFFFFF` | ⬜ White |
| **Login button** | `--btn-primary-bg` | `#009688` | 🟩 Teal green |
| **Forgot Password link** | `--teal-color` | `#009688` | 🟩 Teal text |
| **Input focus background** | `--input-focus-bg` | `#E3F2FD` | 🔵 Light blue tint |
| **Footer tagline** | `--text-brand-red` | `#db2128` | 🟥 "Proud to be..." |
| **Decorative circles** | `--circle-decoration` | `#F0F0F0` | ⚪ Light gray |
| **Sidebar background** | `--sidebar-bg` | `#1A1A2E` | ⚫ Dark charcoal |
| **Sidebar active item** | `--sidebar-active` | `#db2128` | 🟥 Red highlight |
| **Top Navbar** | `--navbar-bg` | `#FFFFFF` | ⬜ White (with border) |
| **Page background** | `--page-bg` | `#F0F2F5` | ⬜ Light gray |
| **Table header** | `--table-header-bg` | `#db2128` | 🟥 Crimson red |
| **Table header text** | `--table-header-text` | `#FFFFFF` | ⬜ White |
| **Table row hover** | `--table-row-hover` | `#FFF3F4` | 🩷 Light pink |
| **Modal header** | `--modal-header-bg` | `#db2128` | 🟥 Crimson red |
| **Save/Submit buttons** | `--btn-primary-bg` | `#009688` | 🟩 Teal |
| **Branded buttons** | `--btn-secondary-bg` | `#db2128` | 🟥 Red |
| **Delete buttons** | `--btn-danger-bg` | `#DC3545` | 🟥 Standard red |
| **Input focus border** | `--input-focus-border` | `#009688` | 🟩 Teal |
| **Headings** | `--text-heading` | `#212529` | ⚫ Dark |
| **Body text** | `--text-primary` | `#212529` | ⚫ Dark |
| **Links** | `--teal-color` | `#009688` | 🟩 Teal |

---

### 🖼️ Login Page CSS (Generated from Actual Screenshot)

```css
/* Login Page — Brand Red + Teal Theme */
.login-wrapper {
    min-height: 100vh;
    display: flex;
    align-items: stretch;
    background: var(--card-bg);
}

/* ── LEFT PANEL: Solid Brand Red ── */
.login-left-panel {
    flex: 0 0 40%;
    background: var(--login-left-bg);
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: flex-start;
    padding: 60px 50px;
    position: relative;
}

.login-left-panel .title-group {
    display: flex;
    align-items: stretch;
    gap: 16px;
}

.login-left-panel .accent-bar {
    width: 4px;
    background: var(--accent-gold);          /* Gold vertical bar */
    border-radius: 2px;
}

.login-left-panel .title-text {
    color: var(--text-on-dark);
    font-size: 28px;
    font-weight: 700;
    line-height: 1.3;
}

/* ── RIGHT PANEL: White with form ── */
.login-right-panel {
    flex: 1;
    background: var(--card-bg);
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 60px;
    position: relative;
    overflow: hidden;
}

/* Decorative gray circles (top-right & bottom-right) */
.login-right-panel::before,
.login-right-panel::after {
    content: '';
    position: absolute;
    border-radius: 50%;
    background: var(--circle-decoration);
    opacity: 0.6;
}
.login-right-panel::before {
    width: 200px; height: 200px;
    top: -60px; right: -60px;
}
.login-right-panel::after {
    width: 150px; height: 150px;
    bottom: -40px; right: 40px;
}

.login-card {
    width: 100%;
    max-width: 420px;
    z-index: 1;                              /* above decorative circles */
}

.login-card h2 {
    color: var(--text-heading);
    font-weight: 700;
    font-size: 24px;
    margin-bottom: 24px;
}

.login-card .lang-select {
    margin-bottom: 24px;
    font-size: 13px;
}

.login-card .form-control {
    border: 1px solid var(--input-border);
    border-radius: 6px;
    padding: 12px 16px;
    font-size: 14px;
    transition: all 0.2s;
}

.login-card .form-control:focus {
    border-color: var(--input-focus-border);
    background-color: var(--input-focus-bg); /* Light blue tint on focus */
    box-shadow: 0 0 0 0.2rem var(--input-focus-shadow);
}

.login-card .input-group-text {
    background: transparent;
    border-right: none;
    color: var(--text-secondary);
}

.login-card .toggle-password {
    background: transparent;
    border-left: none;
    cursor: pointer;
    color: var(--text-secondary);
}

.login-card .btn-login {
    background: var(--btn-primary-bg);       /* Teal #009688 */
    color: var(--btn-primary-text);
    border: none;
    border-radius: 6px;
    padding: 12px;
    font-size: 15px;
    font-weight: 600;
    width: 100%;
    text-transform: uppercase;
    letter-spacing: 1px;
    transition: background 0.2s;
}

.login-card .btn-login:hover {
    background: var(--btn-primary-hover);    /* Dark teal #00796B */
}

.login-card .remember-row {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin: 16px 0 24px;
    font-size: 13px;
}

.login-card .forgot-link {
    color: var(--teal-color);                /* Teal link */
    text-decoration: none;
    font-size: 13px;
}

.login-card .forgot-link:hover {
    text-decoration: underline;
}

.login-card .footer-tagline {
    text-align: center;
    margin-top: 32px;
    color: var(--text-brand-red);            /* "Proud to be..." in red */
    font-size: 13px;
    font-style: italic;
}

/* Responsive — mobile: hide left panel */
@media (max-width: 768px) {
    .login-left-panel { display: none; }
    .login-right-panel { padding: 30px 20px; }
}
```

---

### 🧭 Sidebar CSS (Dark Charcoal + Red Active)

```css
/* Sidebar — Dark with Red active items */
.sidebar {
    width: 260px;
    min-height: 100vh;
    background: var(--sidebar-bg);           /* #1A1A2E dark charcoal */
    color: var(--text-on-dark);
    position: fixed;
    top: 0; left: 0;
    z-index: 1000;
    transition: width 0.3s;
    overflow-y: auto;
}

.sidebar .brand-area {
    padding: 20px;
    border-bottom: 1px solid rgba(255,255,255,0.08);
    display: flex;
    align-items: center;
    gap: 12px;
}

.sidebar .brand-area .brand-dot {
    width: 8px; height: 8px;
    background: var(--primary-color);        /* Red dot indicator */
    border-radius: 50%;
}

.sidebar .brand-area .brand-title {
    font-size: 16px;
    font-weight: 600;
    color: var(--text-on-dark);
}

.sidebar .nav-link {
    color: var(--text-on-dark-muted);
    padding: 12px 20px;
    font-size: 14px;
    display: flex;
    align-items: center;
    gap: 12px;
    transition: all 0.2s;
    border-left: 3px solid transparent;
}

.sidebar .nav-link:hover {
    background: var(--sidebar-hover);        /* #2A2A42 */
    color: var(--text-on-dark);
}

.sidebar .nav-link.active {
    background: rgba(204,41,54,0.15);        /* Red tint background */
    color: var(--primary-color);             /* #db2128 red text */
    border-left-color: var(--primary-color); /* Red left border */
    font-weight: 600;
}

.sidebar .nav-link i {
    font-size: 18px;
    width: 24px;
    text-align: center;
}
```

---

### 🔝 Navbar CSS (White Navbar)

```css
/* Top Navbar — White with bottom border */
.top-navbar {
    background: var(--navbar-bg);            /* White */
    height: 60px;
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0 24px;
    margin-left: 260px;
    position: fixed;
    top: 0; right: 0; left: 0;
    z-index: 999;
    border-bottom: 1px solid var(--navbar-border);
    box-shadow: 0 1px 3px rgba(0,0,0,0.05);
}

.top-navbar .page-title {
    color: var(--text-heading);
    font-weight: 600;
    font-size: 16px;
}

.top-navbar .user-info {
    color: var(--text-primary);
    font-size: 14px;
}

.top-navbar .user-info .role-badge {
    background: var(--primary-color);        /* Red badge */
    color: #FFFFFF;
    padding: 2px 10px;
    border-radius: 12px;
    font-size: 11px;
    margin-left: 8px;
}

.top-navbar .btn-logout {
    background: transparent;
    color: var(--primary-color);             /* Red text */
    border: 1px solid var(--primary-color);  /* Red border */
    border-radius: 6px;
    padding: 6px 16px;
    font-size: 13px;
    transition: all 0.2s;
}

.top-navbar .btn-logout:hover {
    background: var(--primary-color);
    color: #FFFFFF;
}
```

---

### 📊 DataTable Header Override

```css
/* Grid / DataTable — Brand Red Header */
#gridTable thead th {
    background-color: var(--table-header-bg) !important;  /* #db2128 */
    color: var(--table-header-text) !important;            /* White */
    font-weight: 600;
    font-size: 13px;
    padding: 10px 12px;
    position: sticky;
    top: 0;
    z-index: 10;
}

#gridTable tbody tr:hover {
    background-color: var(--table-row-hover) !important;   /* #FFF3F4 light pink */
}
```

---

### 🏷️ Button Styles

```css
/* Primary CTA — Login, Save, Submit (TEAL) */
.btn-brand-primary {
    background: var(--btn-primary-bg);       /* #009688 teal */
    color: var(--btn-primary-text);
    border: none;
    border-radius: 6px;
    font-weight: 600;
}
.btn-brand-primary:hover {
    background: var(--btn-primary-hover);    /* #00796B dark teal */
    color: var(--btn-primary-text);
}

/* Branded secondary (RED) */
.btn-brand-secondary {
    background: var(--btn-secondary-bg);     /* #db2128 red */
    color: var(--btn-secondary-text);
    border: none;
    border-radius: 6px;
}
.btn-brand-secondary:hover {
    background: var(--primary-dark);         /* #A31D28 dark red */
}

/* Teal outline */
.btn-brand-outline {
    background: transparent;
    color: var(--teal-color);
    border: 1px solid var(--teal-color);
    border-radius: 6px;
}
.btn-brand-outline:hover {
    background: var(--teal-color);
    color: #FFFFFF;
}

/* Red outline */
.btn-brand-outline-red {
    background: transparent;
    color: var(--primary-color);
    border: 1px solid var(--primary-color);
    border-radius: 6px;
}
.btn-brand-outline-red:hover {
    background: var(--primary-color);
    color: #FFFFFF;
}
```

---

## 🔐 Authentication Flow (How It Works)

```
User visits any page
    ↓
[Middleware] Checks JWT token in cookie/session
    ↓
Token valid?  ──YES──→  Allow page, check permissions
    ↓NO
Redirect to /Login
    ↓
User enters username + password
    ↓
MVC calls → API /api/auth/login
    ↓
API validates → returns JWT token
    ↓
MVC stores token in HttpOnly Cookie + Session
    ↓
User redirected to Dashboard
```

---

## 🛡️ User Roles & Permissions (How It Works)

```
Each User has ONE Role (e.g. Admin, Manager, Viewer)
Each Role has MANY Permissions
Each Permission links to a MODULE + ACTION

Example:
  Role: Manager
    ├── ManpowerContract → View   ✅
    ├── ManpowerContract → Add    ✅
    ├── ManpowerContract → Edit   ✅
    ├── ManpowerContract → Delete ❌
    └── UserManagement  → View   ❌

In code — controller checks permission:
  [HasPermission("ManpowerContract", "Edit")]
  public async Task<IActionResult> Save(...)
```

---

## 🗂️ New Screens Added (vs Original Prompt)

| Screen | Location | Description |
|---|---|---|
| Login | `/Login` | Company-branded login page |
| Dashboard | `/Dashboard` | Home after login |
| User Management | `/UserManagement` | Add/Edit/Delete users |
| Role Management | `/RoleManagement` | Add/Edit roles |
| Permission Matrix | `/RolePermission` | Assign permissions to roles |
| Change Password | `/Account/ChangePassword` | User changes own password |
| Unauthorized | `/Unauthorized` | Shown when no permission |

---

## 📋 Generating Additional Screens

For each new screen after the project is set up:

```
1. Copy mainproject.md content
2. Fill in ONLY the "SCREEN LAYOUT" section at the bottom
3. Keep all other instructions as-is
4. Send to Claude
5. Claude will generate ONLY the new screen files
   (it already knows the architecture from the prompt)
```

---

## ⚠️ Common Mistakes to Avoid

```
❌ Running MVC before API is running → Login will fail
❌ Wrong JWT Secret in appsettings → 401 Unauthorized errors
❌ Not running Auth SQL scripts → Login table missing
❌ Forgetting to seed admin user → Can't login at all
❌ Running seed data twice → Duplicate user error (safe — IF NOT EXISTS used)
```

---

## 📞 Screenshot Guidelines (for Color Extraction)

When sharing screenshots for color/theme:
```
✅ Share the FULL login page screenshot
✅ Share the main navigation/sidebar screenshot
✅ Make sure company logo is visible
✅ Share any existing application page for color reference
```

Claude will extract:
- Primary brand color
- Background color
- Text colors
- Button styles
- Logo placement

---

## 🔄 Version Control — GitHub / GitLab Setup & Workflow

---

### A. ONE-TIME SETUP (Do This First)

#### Step 1 — Install GitHub CLI (Optional but Recommended)
```
Open browser → https://cli.github.com → Download for Windows → Install
Then in terminal:
  gh auth login
  → Select GitHub.com
  → Select HTTPS
  → Login with browser
```

#### Step 2 — Create Repository on GitHub
```
Option A — Using GitHub CLI (if installed):
  cd "C:\Workarea\Practice\Claude Project Auto generated\ManpowerContract"
  gh repo create ManpowerContract --private --source=. --remote=origin --push

Option B — Using Browser (if CLI not installed):
  1. Go to https://github.com/new
  2. Repository name: ManpowerContract
  3. Description: "Manpower Contract Management — .NET Core MVC + Web API"
  4. Select: Private
  5. DO NOT check "Initialize with README" (we already have code)
  6. Click "Create repository"
  7. Copy the HTTPS URL (e.g., https://github.com/YourUsername/ManpowerContract.git)
  8. Run in terminal:
     cd "C:\Workarea\Practice\Claude Project Auto generated\ManpowerContract"
     git remote add origin https://github.com/YourUsername/ManpowerContract.git
     git push -u origin main
```

#### Step 3 — Verify Setup
```bash
git remote -v
# Should show:
# origin  https://github.com/YourUsername/ManpowerContract.git (fetch)
# origin  https://github.com/YourUsername/ManpowerContract.git (push)

git log --oneline
# Should show your commits
```

---

### B. DAILY WORKFLOW — How to Use Git Day-to-Day

#### Scenario 1: Update skill.md / instruction.md / mainproject.md
```bash
# 1. Always pull latest first (in case team members changed something)
git pull origin main

# 2. Make your changes to the files (edit in VS Code / Claude / etc.)

# 3. Check what changed
git status
git diff

# 4. Stage and commit
git add instruction.md skill.md mainproject.md
git commit -m "docs: update permission matrix and add copy permission feature"

# 5. Push to GitHub
git push origin main
```

#### Scenario 2: Add a New Screen (Feature Branch)
```bash
# 1. Create a feature branch
git checkout -b feature/permission-management

# 2. Generate code using Claude + mainproject.md
# 3. Add all new files
git add Controllers/PermissionController.cs
git add Views/Permission/
git add wwwroot/js/permission.js
git add Database/04_Permission_SP.sql

# 4. Commit
git commit -m "feat: add Permission Management screen with copy permission"

# 5. Push feature branch
git push -u origin feature/permission-management

# 6. Create Pull Request (PR) on GitHub
#    Option A (CLI):  gh pr create --title "Add Permission Management" --body "..."
#    Option B (Browser): Go to GitHub → "Compare & pull request"

# 7. After PR is approved and merged → switch back to main
git checkout main
git pull origin main

# 8. Delete old feature branch
git branch -d feature/permission-management
```

#### Scenario 3: Collaborating with Team Members
```bash
# Before starting work every day:
git pull origin main

# If there's a conflict:
git status                    # See conflicted files
# Open the file → resolve <<<< ==== >>>> markers
git add <resolved-file>
git commit -m "fix: resolve merge conflict in skill.md"
git push origin main
```

---

### C. WHAT GOES WHERE — File Tracking Rules

```
✅ ALWAYS COMMIT (track in Git):
   instruction.md          ← Workflow guide
   skill.md                ← Architecture patterns
   mainproject.md          ← Master prompt
   *.cs                    ← C# source code
   *.cshtml                ← Razor views
   *.js                    ← JavaScript files
   *.css                   ← Stylesheets
   *.sql                   ← Database scripts
   .gitignore              ← Git ignore rules
   *.csproj, *.sln         ← Project files

❌ NEVER COMMIT (already in .gitignore):
   bin/, obj/              ← Build output
   .vs/                    ← Visual Studio cache
   appsettings.Development.json  ← Local secrets
   appsettings.Production.json   ← Production secrets
   node_modules/           ← NPM packages
   *.pfx, *.key            ← Certificates
   .env                    ← Environment variables
```

---

### D. BRANCH STRATEGY

```
main              ← Production-ready (protected)
├── staging       ← Testing/QA
├── feature/xxx   ← New screens
├── fix/xxx       ← Bug fixes
└── docs/xxx      ← Documentation updates (like updating MD files)

Branch Naming Convention:
  feature/permission-management
  feature/login-screen
  fix/role-permission-save-bug
  docs/update-skill-md-colors
```

---

### E. COMMIT MESSAGE FORMAT

```
Type: description

Types:
  feat:     New feature         → feat: add permission management screen
  fix:      Bug fix             → fix: role dropdown not loading on edit
  docs:     Documentation       → docs: update skill.md with teal theme colors
  refactor: Code restructure    → refactor: extract BaseRepository from repos
  style:    CSS/formatting      → style: update sidebar active color to red
  db:       Database changes    → db: add usp_RolePermission_COPYFROMROLE SP
  test:     Tests               → test: add unit tests for AuthService
  chore:    Build/config        → chore: update .gitignore for node_modules
```

---

### F. QUICK REFERENCE — COMMON GIT COMMANDS

```bash
# ═══ STATUS & HISTORY ═══
git status                          # What files changed?
git diff                            # See exact changes
git log --oneline -10               # Last 10 commits
git log --oneline --graph           # Visual branch tree

# ═══ BRANCHING ═══
git branch                          # List local branches
git branch -a                       # List all (incl. remote)
git checkout -b feature/xyz         # Create + switch to new branch
git checkout main                   # Switch to main
git branch -d feature/xyz           # Delete merged branch

# ═══ STAGING & COMMITTING ═══
git add filename.cs                 # Stage specific file
git add *.md                        # Stage all .md files
git add .                           # Stage everything (use carefully)
git commit -m "feat: description"   # Commit with message
git commit --amend                  # Fix last commit message

# ═══ SYNCING WITH REMOTE ═══
git pull origin main                # Download latest from GitHub
git push origin main                # Upload to GitHub
git push -u origin feature/xyz      # Push new branch + set upstream
git fetch                           # Download info without merging

# ═══ UNDO (USE WITH CAUTION) ═══
git checkout -- filename.cs         # Discard changes in one file
git stash                           # Temporarily save uncommitted changes
git stash pop                       # Restore stashed changes
```

---

### G. GITHUB + CLAUDE CODE WORKFLOW

```
The ideal workflow for using Claude Code with this project:

1. Pull latest:     git pull origin main
2. Open Claude:     Ask Claude to update/generate code
3. Review changes:  git diff  (check what Claude changed)
4. Stage files:     git add <specific-files>
5. Commit:          git commit -m "feat: description"
6. Push:            git push origin main

For major features:
1. Create branch:   git checkout -b feature/xyz
2. Work with Claude: Generate code, update MD files
3. Commit often:    Small, focused commits
4. Push branch:     git push -u origin feature/xyz
5. Create PR:       On GitHub (or via gh pr create)
6. Review + Merge:  On GitHub
7. Clean up:        git checkout main && git pull
```

---

*Last Updated: 2026 | Manpower Contract Project*
