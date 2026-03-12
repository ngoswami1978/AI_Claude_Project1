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
> Theme: **Crimson Red (Brand) + Teal Green (CTA)** on White
> Applied to: Login page, Navbar, Sidebar, Buttons, Table headers, Modals.

---

### 📸 Login Screen Layout (Extracted from Screenshot)

```
┌─────────────────────────────────────────────────────────────────────────┐
│                                                                         │
│  ┌──────────────────────────┬──────────────────────────────────────┐    │
│  │                          │           ◯ (gray decorative circle)│    │
│  │   LEFT PANEL             │                                     │    │
│  │   (Solid Crimson Red)    │   "Manpower Contract Management"    │    │
│  │   Background: #CC2936    │    (dark heading, bold)             │    │
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
│  │                          │   (Crimson red text: #CC2936)      │    │
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
  /* ═══ PRIMARY BRAND — Crimson Red ═══ */
  --primary-color:          #CC2936;   /* Brand red — left panel, accents */
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
  --login-left-bg:          #CC2936;   /* Solid crimson red */
  --sidebar-bg:             #1A1A2E;   /* Dark sidebar (navy/charcoal) */
  --sidebar-hover:          #2A2A42;   /* Sidebar item hover */
  --sidebar-active:         #CC2936;   /* Active item — brand red highlight */
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
  --text-brand-red:         #CC2936;   /* Red branded text (footer tagline) */

  /* ═══ TABLE / GRID ═══ */
  --table-header-bg:        #CC2936;   /* Crimson red header */
  --table-header-text:      #FFFFFF;   /* White header text */
  --table-row-hover:        #FFF3F4;   /* Very light pink hover */
  --table-border:           #DEE2E6;   /* Light gray borders */

  /* ═══ MODAL ═══ */
  --modal-header-bg:        #CC2936;   /* Red modal header */
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
  --btn-secondary-bg:       #CC2936;   /* Red — branded secondary */
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
| **Login left panel** | `--login-left-bg` | `#CC2936` | 🟥 Solid crimson red |
| **Login left panel text** | `--text-on-dark` | `#FFFFFF` | ⬜ White on red |
| **Gold accent bar** | `--accent-gold` | `#FFB300` | 🟨 Gold vertical bar |
| **Login right panel** | `--card-bg` | `#FFFFFF` | ⬜ White |
| **Login button** | `--btn-primary-bg` | `#009688` | 🟩 Teal green |
| **Forgot Password link** | `--teal-color` | `#009688` | 🟩 Teal text |
| **Input focus background** | `--input-focus-bg` | `#E3F2FD` | 🔵 Light blue tint |
| **Footer tagline** | `--text-brand-red` | `#CC2936` | 🟥 "Proud to be..." |
| **Decorative circles** | `--circle-decoration` | `#F0F0F0` | ⚪ Light gray |
| **Sidebar background** | `--sidebar-bg` | `#1A1A2E` | ⚫ Dark charcoal |
| **Sidebar active item** | `--sidebar-active` | `#CC2936` | 🟥 Red highlight |
| **Top Navbar** | `--navbar-bg` | `#FFFFFF` | ⬜ White (with border) |
| **Page background** | `--page-bg` | `#F0F2F5` | ⬜ Light gray |
| **Table header** | `--table-header-bg` | `#CC2936` | 🟥 Crimson red |
| **Table header text** | `--table-header-text` | `#FFFFFF` | ⬜ White |
| **Table row hover** | `--table-row-hover` | `#FFF3F4` | 🩷 Light pink |
| **Modal header** | `--modal-header-bg` | `#CC2936` | 🟥 Crimson red |
| **Save/Submit buttons** | `--btn-primary-bg` | `#009688` | 🟩 Teal |
| **Branded buttons** | `--btn-secondary-bg` | `#CC2936` | 🟥 Red |
| **Delete buttons** | `--btn-danger-bg` | `#DC3545` | 🟥 Standard red |
| **Input focus border** | `--input-focus-border` | `#009688` | 🟩 Teal |
| **Headings** | `--text-heading` | `#212529` | ⚫ Dark |
| **Body text** | `--text-primary` | `#212529` | ⚫ Dark |
| **Links** | `--teal-color` | `#009688` | 🟩 Teal |

---

### 🖼️ Login Page CSS (Generated from Actual Screenshot)

```css
/* Login Page — Crimson Red + Teal Theme */
.login-wrapper {
    min-height: 100vh;
    display: flex;
    align-items: stretch;
    background: var(--card-bg);
}

/* ── LEFT PANEL: Solid Crimson Red ── */
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
    color: var(--primary-color);             /* #CC2936 red text */
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
/* Grid / DataTable — Crimson Red Header */
#gridTable thead th {
    background-color: var(--table-header-bg) !important;  /* #CC2936 */
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
    background: var(--btn-secondary-bg);     /* #CC2936 red */
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

## 🔄 Version Control (GitLab)

```bash
# Standard branch strategy
main          ← Production (protected)
staging       ← Testing
feature/xxx   ← Your new screen

# Before creating Merge Request:
git pull origin main
git checkout -b feature/your-screen-name
# ... make changes ...
git add .
git commit -m "feat: add YourScreenName screen"
git push origin feature/your-screen-name
# → Create MR in GitLab
```

---

*Last Updated: 2026 | Manpower Contract Project*
