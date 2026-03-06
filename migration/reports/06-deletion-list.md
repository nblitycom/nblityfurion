# Deletion List

> Generated: 2026-03-06  
> Phase D: Items removed or candidates for future removal

---

## Removed from Bundling (NOT file deletion — bundle registration removed)

| Asset | Removed From | Deletion Basis |
|-------|-------------|----------------|
| `jquery.min.js` bundle ref | Server + WASM script contributors | 0 references in any Blazor component. Only consumer (bootstrap-datepicker) also removed. |
| `bootstrap-datepicker.min.js` bundle ref | Server + WASM script contributors | 0 references in any Blazor component. Replaced by MudBlazor `MudDatePicker`. |
| `bootstrap-datepicker.min.css` bundle ref | Server + WASM style contributors | 0 references in any Blazor component. Paired with JS above. |
| `Chart.min.css` bundle ref | Server + WASM style contributors | 0 references in any Blazor component. No Chart.js usage in Blazor pages. |

**Note:** The physical files in `modules/.../wwwroot/` are NOT deleted. They may still be used by the MVC theme layouts for Account pages. Only the Blazor bundle registrations were removed.

---

## MVC Artifacts — NOT Deleted (Kept for Compatibility)

These MVC artifacts are intentionally NOT deleted because they serve active functions:

| Artifact | Reason to Keep |
|----------|---------------|
| `Themes/LeptonXLite/Layouts/Application.cshtml` | Used as MVC layout for any remaining MVC-rendered pages |
| `Themes/LeptonXLite/Layouts/Account.cshtml` | Used by Account pages (Register, ResetPassword, Logout, LoggedOut) |
| `Themes/LeptonXLite/Layouts/Empty.cshtml` | Utility layout |
| `Themes/LeptonXLite/Components/*.cshtml` | MVC ViewComponents used by layouts |
| `Login.cshtml` + code-behind | OpenIddict auth flow — cannot be pure Blazor |
| `Register.cshtml` + code-behind | OpenIddict registration flow |
| `Logout.cshtml` + code-behind | OpenIddict logout flow |
| `ResetPassword.cshtml` + code-behind | Token-based form POST |
| `LoggedOut.cshtml` + code-behind | OIDC signout iframe |
| `NblityController.cs` | API base controller — not MVC UI controller |

---

## Future Deletion Candidates

Once manual migration is completed for the remaining 4 Account pages:

| File | Prerequisite |
|------|-------------|
| `_Layout.cshtml` (Account) | All Account pages migrated to `_BlazorAccountLayout` |
| `_ViewStart.cshtml` (Account) | All Account pages migrated to `_BlazorAccountLayout` |
| `Components/ProfileManagementGroup/Password/Default.cshtml` | `AccountManage.razor` fully replaces Manage page |
| `Components/ProfileManagementGroup/PersonalInfo/Default.cshtml` | `AccountManage.razor` fully replaces Manage page |
| `Themes/LeptonXLite/Layouts/Application.cshtml` | No MVC pages remain |
| `Themes/LeptonXLite/Components/*.cshtml` | No MVC pages remain |

---

## Startup Configuration — No Changes Needed

The current `NblityBlazorModule.cs` configuration is correct for the hybrid state:
- `AddRazorComponents()` with `InteractiveServerComponents` and `InteractiveWebAssemblyComponents` — needed for Blazor
- `MapRazorComponents<App>()` — Blazor endpoint mapping
- `AbpAccountWebOpenIddictModule` — Account Razor Pages (kept for auth flow)
- `AbpAspNetCoreMvcUiLeptonXLiteThemeModule` — MVC theme (kept for Account page layouts)
- API controllers (auto-configured) — kept for API layer

No MVC-only services need to be removed at this time.
