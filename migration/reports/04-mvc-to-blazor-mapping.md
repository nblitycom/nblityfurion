# MVC → Blazor Mapping

> Generated: 2026-03-06  
> Phase C: MVC View Migration to Blazor

---

## MVC → Blazor Route Mapping

### Account Module Pages

| Original MVC Page | Route | Blazor Equivalent | Status | Notes |
|-------------------|-------|-------------------|--------|-------|
| `Login.cshtml` | `/Account/Login` | `LoginComponent.razor` (embedded) | ✅ Already Blazor | Razor Page wraps Blazor `LoginComponent`; kept as Razor Page for OpenIddict form POST |
| `Register.cshtml` | `/Account/Register` | — | 🔶 `manual-required` | Uses ABP tag helpers + external login form POST. Requires server-side form processing. |
| `Logout.cshtml` | `/Account/Logout` | — | 🔶 `manual-required` | OpenIddict logout flow requires server-side redirect |
| `ForgotPassword.cshtml` | `/Account/ForgotPassword` | — | ✅ Migrated styling | Updated to MudBlazor layout + styling. Server POST kept for `SendPasswordResetCodeAsync`. |
| `ResetPassword.cshtml` | `/Account/ResetPassword` | — | 🔶 `manual-required` | Uses ABP tag helpers + server form POST with reset token |
| `ResetPasswordConfirmation.cshtml` | `/Account/ResetPasswordConfirmation` | — | ✅ Migrated styling | Updated to MudBlazor layout + styling (informational page) |
| `PasswordResetLinkSent.cshtml` | `/Account/PasswordResetLinkSent` | — | ✅ Migrated styling | Updated to MudBlazor layout + styling (informational page) |
| `AccessDenied.cshtml` | `/Account/AccessDenied` | — | ✅ Migrated styling | Updated to MudBlazor layout + styling (informational page) |
| `LoggedOut.cshtml` | `/Account/LoggedOut` | — | 🔶 `manual-required` | Uses custom JS/CSS + OIDC signout iframe |
| `Manage.cshtml` | `/Account/Manage` | `AccountManage.razor` | ✅ Already Blazor | Blazor `AccountManage` component exists with MudBlazor tabs |

### Main Application Pages

| Original | Route | Blazor Component | Status |
|----------|-------|-------------------|--------|
| — (no MVC) | `/` | `Index.razor` | ✅ Already Blazor |
| — | `/*` | `MainLayout.razor` (MudBlazor) | ✅ Already Blazor |

### ABP Module Pages (already Blazor)

| Module | Route | Component | Status |
|--------|-------|-----------|--------|
| Identity | `/identity/users` | `UserManagement.razor` | ✅ Already Blazor |
| Identity | `/identity/roles` | `RoleManagement.razor` | ✅ Already Blazor |
| TenantManagement | `/tenant-management/tenants` | `TenantManagement.razor` | ✅ Already Blazor |
| SettingManagement | `/setting-management` | `SettingManagement.razor` | ✅ Already Blazor |

---

## Layout Migration

| Original MVC Layout | Blazor Equivalent | Status |
|---------------------|-------------------|--------|
| `Themes/LeptonXLite/Layouts/Application.cshtml` | `Nblity.Blazor.Client/Shared/Layout/MainLayout.razor` | ✅ Replaced by MudBlazor layout |
| `Themes/LeptonXLite/Layouts/Account.cshtml` | `_BlazorAccountLayout.cshtml` | ✅ Account pages use MudBlazor layout |
| `Themes/LeptonXLite/Layouts/Empty.cshtml` | `NullLayout.razor` (LeptonXLite module) | ✅ Already exists |

---

## Pages Marked `manual-required`

### 1. Register.cshtml
**Reason:** Uses multiple ABP tag helpers (`abp-input`, `abp-button`) for form fields that generate Bootstrap-based HTML. Also has external login provider form POST flow.  
**Recommended approach:** Create a `RegisterComponent.razor` similar to `LoginComponent.razor` using MudBlazor form components, then wrap in the existing Razor Page.

### 2. Logout.cshtml
**Reason:** Part of OpenIddict signout flow. Handles server-side redirect and session cleanup.  
**Recommended approach:** Keep as Razor Page; update layout to use `_BlazorAccountLayout`.

### 3. ResetPassword.cshtml
**Reason:** Uses ABP tag helpers for password input fields + anti-forgery token + form POST with reset token validation.  
**Recommended approach:** Create a `ResetPasswordComponent.razor` with MudBlazor form fields, wrap in Razor Page.

### 4. LoggedOut.cshtml
**Reason:** Has custom CSS/JS assets (`LoggedOut.css`, `LoggedOut.js`) and uses an OIDC signout iframe. Complex post-logout flow.  
**Recommended approach:** Requires careful testing of OIDC signout flow. Update styling but keep page structure.

---

## Migration Statistics

| Category | Count |
|----------|-------|
| Account pages total | 10 |
| Already Blazor | 2 (Login via component, Manage via component) |
| Migrated styling (this phase) | 4 (AccessDenied, ResetPasswordConfirmation, PasswordResetLinkSent, ForgotPassword) |
| Manual-required | 4 (Register, Logout, ResetPassword, LoggedOut) |
| Main app pages | All Blazor (0 MVC) |
| Module pages | All Blazor (0 MVC) |
