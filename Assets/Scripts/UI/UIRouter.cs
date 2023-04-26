using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRouter : Singleton<UIRouter> {
    public enum Route {
        Game,
        Menu,
        Dialogue,
    }

    public Route DebugRoute = Route.Game;

    public event EventHandler<string> OnRouteUpdate;

    public void SwitchRoutes(Route routeName) {
        OnRouteUpdate(this, RouteNameToPath(routeName));
    }

    void OnValidate() {
        if (!Application.isPlaying) { return; }
        OnRouteUpdate?.Invoke(this, RouteNameToPath(DebugRoute));
    }

    string RouteNameToPath(Route routeName) {
        string routePath = "";
        switch (routeName) {
            case Route.Menu:
                routePath = "/menu";
                break;
            case Route.Dialogue:
                routePath = "/dialogue";
                break;
        }
        return routePath;
    }
}
