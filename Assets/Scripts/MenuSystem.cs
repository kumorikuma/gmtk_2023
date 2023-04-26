using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSystem : Singleton<MenuSystem> {
    public void PauseGame() {
        Time.timeScale = 0;
        PlayerManager.Instance.SwitchActionMaps("menu");
        UIRouter.Instance.SwitchRoutes(UIRouter.Route.Menu);
    }

    public void UnpauseGame() {
        Time.timeScale = 1;
        PlayerManager.Instance.SwitchActionMaps("gameplay");
        UIRouter.Instance.SwitchRoutes(UIRouter.Route.Game);
    }
}
