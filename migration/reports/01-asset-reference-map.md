# Asset Reference Map

> Generated: 2026-03-06  
> Purpose: Map every CSS/JS file to its consumers and identify MudBlazor-essential vs. removable assets

---

## 1. Blazor Bundle Assets (loaded via `AbpStyles` / `AbpScripts` in App.razor)

### Scripts Bundle (`BlazorLeptonXLiteThemeBundles.Scripts.Global`)

| Asset | Loaded By | Used By | MudBlazor-Essential | Action |
|-------|-----------|---------|---------------------|--------|
| `bootstrap.bundle.js` | Server: `BlazorLeptonXLiteThemeScriptContributor`; WASM: conditional (InteractiveAuto) | LeptonXLite Blazor layout (dropdowns, navbar) | ❌ No | **Keep** — used by LeptonXLite Blazor layout as fallback |
| `lepton-x.bundle.min.js` | Both Server & WASM contributors | LeptonXLite layout (sidebar toggle, mobile menu) | ❌ No | **Keep** — used by LeptonXLite layout |
| `jquery.min.js` | Both Server & WASM contributors | bootstrap-datepicker only | ❌ No | **REMOVE** — only dependency (datepicker) being removed |
| `bootstrap-datepicker.min.js` | Both Server & WASM contributors | None (MudBlazor has MudDatePicker) | ❌ No | **REMOVE** — replaced by MudBlazor |
| `style-initializer.js` (Web) | Both Server & WASM contributors | Theme initialization | ⚠️ Theme-essential | **Keep** |
| `style-initializer.js` (WASM) | WASM contributor only | WASM theme init | ⚠️ Theme-essential | **Keep** |

### Styles Bundle (`BlazorLeptonXLiteThemeBundles.Styles.Global`)

| Asset | Loaded By | Used By | MudBlazor-Essential | Action |
|-------|-----------|---------|---------------------|--------|
| `Chart.min.css` | Both Server & WASM style contributors | None (no Chart.js usage in Blazor pages) | ❌ No | **REMOVE** — unused |
| `bootstrap-datepicker.min.css` | Both Server & WASM style contributors | None (MudBlazor has MudDatePicker) | ❌ No | **REMOVE** — replaced by MudBlazor |
| `bootstrap-icons.css` | Both Server & WASM style contributors (Server only) | LeptonXLite Blazor layout (icon classes `bi bi-*`) | ❌ No | **Keep** — used by LeptonXLite components |
| `abp-bundle.css` | Server style contributor | ABP framework components | ⚠️ Framework-essential | **Keep** |
| `blazor-bundle.css` | Server style contributor | ABP Blazor components | ⚠️ Framework-essential | **Keep** |
| `bootstrap-dim.css` | Server style contributor | Bootstrap theming | ❌ No | **Keep** — part of theme |
| `layout-bundle.css` | Server style contributor | LeptonXLite layout | ⚠️ Theme-essential | **Keep** |
| `font-bundle.css` | Server style contributor | Font loading | ⚠️ Theme-essential | **Keep** |

---

## 2. Directly-Loaded Assets (in App.razor `<head>`)

| Asset | Purpose | MudBlazor-Essential | Action |
|-------|---------|---------------------|--------|
| `_content/MudBlazor/MudBlazor.min.css` | MudBlazor styles | ✅ Yes | **Keep** |
| `_content/MudBlazor/MudBlazor.min.js` | MudBlazor JavaScript | ✅ Yes | **Keep** |
| `_framework/blazor.web.js` | Blazor framework | ✅ Yes | **Keep** |
| `Nblity.Blazor.styles.css` | Scoped component styles | ✅ Yes | **Keep** |
| `Nblity.Blazor.Client.styles.css` | Scoped component styles | ✅ Yes | **Keep** |
| Google Fonts (Roboto) | MudBlazor typography | ✅ Yes | **Keep** |

---

## 3. Application wwwroot Assets

### `src/Nblity.Blazor/wwwroot/`

| File | Referenced By | Action |
|------|--------------|--------|
| `global-styles.css` | App.razor `GlobalStyles` list → `AbpStyles` WebAssembly bundle | **Clean up** — only contains a comment |
| `global-scripts.js` | App.razor `GlobalScripts` list → `AbpScripts` WebAssembly bundle | **Clean up** — only contains a comment |
| `dev-login-helper.js` | Used in dev mode login flow | **Keep** |
| `favicon.ico` | Browser tab icon | **Keep** |
| `images/**` | Logo, getting-started graphics | **Keep** |

### `src/Nblity.Blazor.Client/wwwroot/`

| File | Referenced By | Action |
|------|--------------|--------|
| `main.css` | `NblityStyleBundleContributor` → WebAssembly style bundle | **Keep** — loader styles |
| `appsettings*.json` | Runtime configuration | **Keep** |
| `manifest.json` | PWA manifest | **Keep** |
| `service-worker*.js` | PWA functionality | **Keep** |
| `icon-*.png` | PWA icons | **Keep** |

---

## 4. MVC Theme Assets (LeptonXLite module — NOT in Blazor bundle path)

These assets exist in `modules/Volo.Abp.LeptonXLiteTheme/src/Volo.Abp.AspNetCore.Mvc.UI.Theme.LeptonXLite/wwwroot/` and are loaded only by MVC layouts (Account pages):

| Category | Files | Status |
|----------|-------|--------|
| Bootstrap CSS | `bootstrap.min.css`, `bootstrap-grid.min.css`, `bootstrap-reboot.min.css` | Used by MVC Account layout |
| Bootstrap JS | `bootstrap.bundle.min.js` | Used by MVC Account layout |
| jQuery | `jquery.js`, `jquery.slim.js` | Used by MVC pages |
| DataTables | `jquery.dataTables.js` | ⚠️ Check usage |
| Chart.js | `Chart.js`, `Chart.bundle.js` | Not used by Account pages |
| Bootstrap Datepicker | `bootstrap-datepicker.min.js`, `bootstrap-datepicker.min.css` | Not used by Account pages |
| Bootstrap Icons | `bootstrap-icons.css` + fonts | Used by theme components |

**Note:** These MVC theme assets are NOT modified as part of the Blazor migration. They are only loaded by MVC-rendered pages (Account module).

---

## 5. Summary: Removal Candidates

### Safe to Remove (from Blazor bundling)
1. `jquery.min.js` — Only dependency was bootstrap-datepicker (being removed)
2. `bootstrap-datepicker.min.js` — Replaced by MudBlazor's MudDatePicker
3. `bootstrap-datepicker.min.css` — Replaced by MudBlazor's MudDatePicker
4. `Chart.min.css` — No Chart.js usage in Blazor components

### Risk Candidates (dynamic/conditional)
1. `bootstrap.bundle.js` — Conditionally loaded in WASM (InteractiveAuto check). Keep for now.
2. `lepton-x.bundle.min.js` — Used by LeptonXLite Blazor layout. Keep for ABP module compatibility.
3. `style-initializer.js` — Required for theme CSS variable initialization. Keep.

### MudBlazor Essential (white-list)
1. `_content/MudBlazor/MudBlazor.min.css`
2. `_content/MudBlazor/MudBlazor.min.js`
3. `_framework/blazor.web.js`
4. Google Fonts Roboto
5. `Nblity.Blazor.styles.css`
6. `Nblity.Blazor.Client.styles.css`
7. `main.css` (loader)
