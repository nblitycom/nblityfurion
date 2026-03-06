# Migration Inventory Report

> Generated: 2026-03-06  
> Repository: nblitycom/nblityfurion  
> Target: MVC → Blazor + MudBlazor migration & frontend cleanup

---

## 1. Project Architecture

| Project | Type | Framework | Purpose |
|---------|------|-----------|---------|
| Nblity.Blazor | Blazor Server (Web SDK) | net10.0 | Main server host |
| Nblity.Blazor.Client | Blazor WebAssembly | net10.0 | WASM client |
| Nblity.Domain | Class Library | net10.0 | Domain entities |
| Nblity.Domain.Shared | Class Library | net10.0 | Shared constants |
| Nblity.Application | Class Library | net10.0 | Business logic |
| Nblity.Application.Contracts | Class Library | net10.0 | DTOs & interfaces |
| Nblity.EntityFrameworkCore | Class Library | net10.0 | EF Core/PostgreSQL |
| Nblity.HttpApi | Class Library | net10.0 | API controllers |
| Nblity.HttpApi.Client | Class Library | net10.0 | HTTP client proxies |
| Nblity.DbMigrator | Console | net10.0 | DB migration tool |

**Key Dependencies:** ABP Framework v10.1.0, MudBlazor v9.1.0, OpenIddict, PostgreSQL

---

## 2. MVC Views (`.cshtml` files)

### Account Module (`modules/Volo.Abp.Account/src/Volo.Abp.Account.Web/`)
| File | Purpose | Migration Status |
|------|---------|-----------------|
| `Pages/Account/Login.cshtml` | Login page (wraps Blazor LoginComponent) | `manual-required` — OpenIddict auth flow |
| `Pages/Account/Register.cshtml` | User registration | `manual-required` — OpenIddict auth flow |
| `Pages/Account/Logout.cshtml` | Logout handler | `manual-required` — OpenIddict auth flow |
| `Pages/Account/ForgotPassword.cshtml` | Password reset request | Candidate for Blazor |
| `Pages/Account/ResetPassword.cshtml` | Password reset form | Candidate for Blazor |
| `Pages/Account/ResetPasswordConfirmation.cshtml` | Reset confirmation | Candidate for Blazor |
| `Pages/Account/PasswordResetLinkSent.cshtml` | Link sent confirmation | Candidate for Blazor |
| `Pages/Account/AccessDenied.cshtml` | Access denied page | Candidate for Blazor |
| `Pages/Account/LoggedOut.cshtml` | Post-logout page | Candidate for Blazor |
| `Pages/Account/Manage.cshtml` | Profile management | Has Blazor equivalent (AccountManage.razor) |
| `Pages/Account/_BlazorAccountLayout.cshtml` | Blazor account layout | Infrastructure — keep |
| `Pages/Account/_Layout.cshtml` | Account layout wrapper | Infrastructure — keep |
| `Pages/Account/_ViewImports.cshtml` | Razor imports | Infrastructure — keep |
| `Pages/Account/_ViewStart.cshtml` | Layout assignment | Infrastructure — keep |
| `Pages/Account/Components/ProfileManagementGroup/Password/Default.cshtml` | Password change partial | Has Blazor equivalent |
| `Pages/Account/Components/ProfileManagementGroup/PersonalInfo/Default.cshtml` | Personal info partial | Has Blazor equivalent |

### LeptonXLite Theme Module (`modules/Volo.Abp.LeptonXLiteTheme/src/Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite/`)
| File | Purpose | Migration Status |
|------|---------|-----------------|
| `Themes/LeptonXLite/Layouts/Application.cshtml` | Main MVC layout | Replaced by MudBlazor MainLayout.razor |
| `Themes/LeptonXLite/Layouts/Account.cshtml` | Account MVC layout | Still needed for Account pages |
| `Themes/LeptonXLite/Layouts/Empty.cshtml` | Empty MVC layout | May be needed |
| `Themes/LeptonXLite/Components/Brand/Default.cshtml` | Brand component | Blazor equivalent exists |
| `Themes/LeptonXLite/Components/BreadCrumbs/Default.cshtml` | Breadcrumbs | Blazor equivalent exists |
| `Themes/LeptonXLite/Components/Menu/Default.cshtml` | Navigation menu | Blazor equivalent exists |
| `Themes/LeptonXLite/Components/Menu/_MenuItem.cshtml` | Menu item partial | Blazor equivalent exists |
| `Themes/LeptonXLite/Components/Toolbar/*.cshtml` (5 files) | Toolbar components | Blazor equivalents exist |
| `Themes/LeptonXLite/Components/PageAlerts/Default.cshtml` | Page alerts | Blazor equivalent exists |
| `Themes/LeptonXLite/Components/ContentTitle/Default.cshtml` | Content title | Blazor equivalent exists |

### Other
| File | Purpose | Status |
|------|---------|--------|
| `src/Nblity.Blazor/Themes/_ViewImports.cshtml` | Theme imports | Infrastructure — keep |
| `modules/Volo.Abp.Account/src/Volo.Abp.Account.Web.OpenIddict/Pages/_ViewImports.cshtml` | OpenIddict imports | Infrastructure — keep |

---

## 3. Razor Components (`.razor` files)

### Main Application
| File | Purpose |
|------|---------|
| `src/Nblity.Blazor/Components/App.razor` | Root Blazor component |
| `src/Nblity.Blazor/_Imports.razor` | Server-side imports |
| `src/Nblity.Blazor.Client/Pages/Index.razor` | Home page |
| `src/Nblity.Blazor.Client/Routes.razor` | Router configuration |
| `src/Nblity.Blazor.Client/Shared/Layout/MainLayout.razor` | **MudBlazor layout** (primary) |
| `src/Nblity.Blazor.Client/_Imports.razor` | Client-side imports |

### ABP Module Components (Blazor)
| Module | Components |
|--------|-----------|
| Account | `LoginComponent.razor`, `AccountManage.razor` |
| Identity | `UserManagement.razor`, `RoleManagement.razor`, `RoleNameComponent.razor` |
| TenantManagement | `TenantManagement.razor` |
| SettingManagement | `SettingManagement.razor`, `EmailSettingGroupViewComponent.razor`, `TimeZoneSettingGroupViewComponent.razor` |
| FeatureManagement | `FeatureManagementModal.razor`, `FeatureSettingManagementComponent.razor` |
| PermissionManagement | `PermissionManagementModal.razor`, `ResourcePermissionManagementModal.razor` |
| LeptonXLite Web | `MainLayout.razor`, `App.razor`, `Branding.razor`, `Breadcrumbs.razor`, `MainMenu.razor`, `MainMenuItem.razor`, etc. |
| LeptonXLite Server | `UserMenuComponent.razor`, `MobileUserMenuComponent.razor` |

---

## 4. Controllers

| File | Purpose | Status |
|------|---------|--------|
| `src/Nblity.HttpApi/Controllers/NblityController.cs` | API base controller | Keep (API layer) |

---

## 5. Frontend Assets (wwwroot)

### Main Application (`src/Nblity.Blazor/wwwroot/`)
| File | Purpose | Status |
|------|---------|--------|
| `global-styles.css` | Placeholder (empty) | Clean up |
| `global-scripts.js` | Placeholder (empty) | Clean up |
| `dev-login-helper.js` | Dev login helper | Keep (dev tool) |
| `favicon.ico` | Site icon | Keep |
| `images/**` | Logo and getting-started images | Keep |

### Client Application (`src/Nblity.Blazor.Client/wwwroot/`)
| File | Purpose | Status |
|------|---------|--------|
| `main.css` | Loader styles | Keep |
| `appsettings*.json` | Configuration | Keep |
| `manifest.json` | PWA manifest | Keep |
| `service-worker*.js` | PWA service worker | Keep |
| `icon-*.png` | PWA icons | Keep |

---

## 6. Entry Points & Configuration

| File | Purpose |
|------|---------|
| `src/Nblity.Blazor/Program.cs` | Server entry point (Serilog, Autofac) |
| `src/Nblity.Blazor.Client/Program.cs` | WASM entry point |
| `src/Nblity.Blazor/NblityBlazorModule.cs` | Main ABP module configuration |
| `src/Nblity.Blazor.Client/NblityBlazorClientModule.cs` | Client ABP module |

---

## 7. Migration Priority

### High Priority
- CSS/JS bundle cleanup (remove unused jQuery, datepicker, Chart.js)
- Consolidate bundling to MudBlazor-essential minimum

### Medium Priority
- Account page modernization (ForgotPassword, ResetPassword, AccessDenied → Blazor)
- Profile management page cleanup (already has Blazor component)

### Low Priority / Manual-Required
- Login/Register/Logout pages (OpenIddict authentication flow requires server-side form POST)
- LeptonXLite MVC theme components (needed as fallback for Account pages)
