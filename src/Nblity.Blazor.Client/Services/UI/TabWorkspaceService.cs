using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Nblity.Blazor.Client.Services.UI;

public class TabItem
{
    public string Route { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public bool IsClosable { get; set; } = true;
}

public class TabWorkspaceService : IDisposable
{
    private readonly NavigationManager _navigationManager;
    private readonly List<TabItem> _tabs = new();
    private string _activeRoute = "/";

    public event Action? OnTabsChanged;

    public IReadOnlyList<TabItem> Tabs => _tabs.AsReadOnly();
    public string ActiveRoute => _activeRoute;
    public TabItem? ActiveTab => _tabs.FirstOrDefault(t =>
        NormalizeRoute(t.Route) == NormalizeRoute(_activeRoute));

    public TabWorkspaceService(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
        _navigationManager.LocationChanged += OnLocationChanged;

        // Open home tab by default
        OpenTab("/", "Home", "home", closable: false);
        _activeRoute = "/";
    }

    public void OpenTab(string route, string title, string? icon = null, bool closable = true)
    {
        var normalized = NormalizeRoute(route);
        var existing = _tabs.FirstOrDefault(t => NormalizeRoute(t.Route) == normalized);

        if (existing == null)
        {
            _tabs.Add(new TabItem
            {
                Route = route,
                Title = title,
                Icon = icon,
                IsClosable = closable
            });
        }

        _activeRoute = route;
        OnTabsChanged?.Invoke();
    }

    public void ActivateTab(string route)
    {
        var normalized = NormalizeRoute(route);
        var tab = _tabs.FirstOrDefault(t => NormalizeRoute(t.Route) == normalized);
        if (tab != null)
        {
            _activeRoute = tab.Route;
            _navigationManager.NavigateTo(tab.Route);
            OnTabsChanged?.Invoke();
        }
    }

    public void CloseTab(string route)
    {
        var normalized = NormalizeRoute(route);
        var tab = _tabs.FirstOrDefault(t => NormalizeRoute(t.Route) == normalized);
        if (tab == null || !tab.IsClosable) return;

        var index = _tabs.IndexOf(tab);
        _tabs.Remove(tab);

        if (NormalizeRoute(_activeRoute) == normalized && _tabs.Count > 0)
        {
            var newIndex = Math.Min(index, _tabs.Count - 1);
            _activeRoute = _tabs[newIndex].Route;
            _navigationManager.NavigateTo(_tabs[newIndex].Route);
        }

        OnTabsChanged?.Invoke();
    }

    public void CloseOtherTabs(string route)
    {
        var normalized = NormalizeRoute(route);
        _tabs.RemoveAll(t => NormalizeRoute(t.Route) != normalized && t.IsClosable);
        _activeRoute = route;
        OnTabsChanged?.Invoke();
    }

    public void CloseAllTabsExceptHome()
    {
        _tabs.RemoveAll(t => t.IsClosable);
        if (_tabs.Count > 0)
        {
            _activeRoute = _tabs[0].Route;
            _navigationManager.NavigateTo(_tabs[0].Route);
        }
        OnTabsChanged?.Invoke();
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        var uri = new Uri(e.Location);
        var route = uri.AbsolutePath;
        var normalized = NormalizeRoute(route);

        // Check if this route already has a tab
        var existing = _tabs.FirstOrDefault(t => NormalizeRoute(t.Route) == normalized);
        if (existing != null)
        {
            _activeRoute = existing.Route;
        }
        else
        {
            // Auto-open a tab for unknown routes
            var title = GetTitleFromRoute(route);
            _tabs.Add(new TabItem
            {
                Route = route,
                Title = title,
                IsClosable = true
            });
            _activeRoute = route;
        }

        OnTabsChanged?.Invoke();
    }

    private static string NormalizeRoute(string route)
    {
        if (string.IsNullOrEmpty(route)) return "/";
        route = route.TrimEnd('/').ToLowerInvariant();
        return string.IsNullOrEmpty(route) ? "/" : route;
    }

    private static string GetTitleFromRoute(string route)
    {
        if (string.IsNullOrEmpty(route) || route == "/") return "Home";

        var segments = route.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0) return "Home";

        var last = segments[^1];
        // Convert kebab-case or path to title case
        return string.Join(" ", last.Split('-', '_')
            .Select(s => char.ToUpper(s[0]) + s[1..]));
    }

    public void UpdateTabTitle(string route, string title, string? icon = null)
    {
        var normalized = NormalizeRoute(route);
        var tab = _tabs.FirstOrDefault(t => NormalizeRoute(t.Route) == normalized);
        if (tab != null)
        {
            tab.Title = title;
            if (icon != null) tab.Icon = icon;
            OnTabsChanged?.Invoke();
        }
    }

    public void Dispose()
    {
        _navigationManager.LocationChanged -= OnLocationChanged;
    }
}
