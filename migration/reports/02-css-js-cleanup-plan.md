# CSS/JS Cleanup Plan

> Generated: 2026-03-06  
> Phase B: MudBlazor Minimal Frontend Set Convergence

---

## Objective

Remove legacy CSS/JS assets from the Blazor bundling that are not needed by MudBlazor and have zero references in Blazor components. Keep only MudBlazor-essential and framework-required assets.

---

## Assets Targeted for Removal

### 1. jQuery (`jquery.min.js`)
- **Location:** `_content/Volo.Abp.AspNetCore.Components.Web.LeptonXLiteTheme/side-menu/libs/jquery/jquery.min.js`
- **Loaded by:** Both Server (`BlazorLeptonXLiteThemeScriptContributor`) and WASM (`LeptonXLiteThemeBundleScriptContributor`) bundling
- **Used by:** Only bootstrap-datepicker (being removed). Not used by any Blazor component.
- **Evidence:** jQuery is loaded AFTER `lepton-x.bundle.min.js` in the bundle order, confirming lepton-x does NOT depend on jQuery. Only consumer was `bootstrap-datepicker.min.js`.
- **Risk:** Low — no Blazor component references jQuery
- **Action:** Remove from both script contributors

### 2. Bootstrap Datepicker JS (`bootstrap-datepicker.min.js`)
- **Location:** `_content/Volo.Abp.AspNetCore.Components.Web.LeptonXLiteTheme/side-menu/libs/bootstrap-datepicker/js/bootstrap-datepicker.min.js`
- **Loaded by:** Both Server and WASM script contributors
- **Used by:** None — MudBlazor provides `MudDatePicker` component
- **Evidence:** No `.razor` file references bootstrap-datepicker. All date inputs in Blazor use MudBlazor.
- **Risk:** None
- **Action:** Remove from both script contributors

### 3. Bootstrap Datepicker CSS (`bootstrap-datepicker.min.css`)
- **Location:** `_content/Volo.Abp.AspNetCore.Components.Web.LeptonXLiteTheme/side-menu/libs/bootstrap-datepicker/css/bootstrap-datepicker.min.css`
- **Loaded by:** Both Server and WASM style contributors
- **Used by:** None — paired with JS above
- **Risk:** None
- **Action:** Remove from both style contributors

### 4. Chart.js CSS (`Chart.min.css`)
- **Location:** `_content/Volo.Abp.AspNetCore.Components.Web.LeptonXLiteTheme/side-menu/libs/chart.js/Chart.min.css`
- **Loaded by:** Both Server and WASM style contributors
- **Used by:** None — no Chart.js usage in any Blazor component
- **Evidence:** No `.razor` or `.cs` file in the main app references Chart.js. MudBlazor has its own charting capabilities.
- **Risk:** None
- **Action:** Remove from both style contributors

---

## Assets Retained (White-List)

### MudBlazor Essential
| Asset | Reason |
|-------|--------|
| `_content/MudBlazor/MudBlazor.min.css` | MudBlazor core styles |
| `_content/MudBlazor/MudBlazor.min.js` | MudBlazor core JavaScript |
| `_framework/blazor.web.js` | Blazor framework |
| Google Fonts (Roboto) | MudBlazor typography |
| `Nblity.Blazor.styles.css` | Scoped component styles (server) |
| `Nblity.Blazor.Client.styles.css` | Scoped component styles (client) |

### Theme/Framework Required
| Asset | Reason |
|-------|--------|
| `bootstrap.bundle.js` | Used by LeptonXLite Blazor layout (conditional InteractiveAuto) |
| `lepton-x.bundle.min.js` | LeptonXLite sidebar/menu functionality |
| `style-initializer.js` (Web) | Theme CSS variable initialization |
| `style-initializer.js` (WASM) | WASM theme initialization |
| `bootstrap-icons.css` | Icon classes used by LeptonXLite Blazor components |
| `abp-bundle.css` | ABP framework component styles |
| `blazor-bundle.css` | ABP Blazor component styles |
| `bootstrap-dim.css` | Bootstrap theming for dark mode |
| `layout-bundle.css` | LeptonXLite layout styles |
| `font-bundle.css` | Font loading |

### Application Assets
| Asset | Reason |
|-------|--------|
| `main.css` | Loader/splash screen styles |
| `global-styles.css` | Placeholder for custom global styles |
| `global-scripts.js` | Placeholder for JS interop functions |
| `dev-login-helper.js` | Development login helper |

---

## Files Modified

| File | Change |
|------|--------|
| `modules/.../Bundling/LeptonXLiteThemeBundleScriptContributor.cs` | Remove jQuery and bootstrap-datepicker |
| `modules/.../Bundling/LeptonXLiteThemeBundleStyleContributor.cs` | Remove Chart.js CSS and bootstrap-datepicker CSS |
| `modules/.../Bundling/BlazorLeptonXLiteThemeScriptContributor.cs` | Remove jQuery and bootstrap-datepicker |
| `modules/.../Bundling/BlazorLeptonXLiteThemeStyleContributor.cs` | Remove Chart.js CSS and bootstrap-datepicker CSS |
| `src/Nblity.Blazor/wwwroot/global-styles.css` | Updated comments |
| `src/Nblity.Blazor/wwwroot/global-scripts.js` | Updated comments |

---

## Rollback Plan

If issues are discovered after cleanup:
1. Re-add removed lines to the 4 bundling contributor files
2. No file deletions were made — the physical JS/CSS files remain in the module's wwwroot
3. Only the bundle registration was removed, so re-adding is a simple code revert
