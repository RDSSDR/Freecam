using Landfall.Haste;
using Landfall.Modding;
using System.Reflection;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Rendering.PostProcessing;
using Zorro.Settings;
using Object = UnityEngine.Object;

namespace FC;

[LandfallPlugin]
public class Program
{
    public static GUILoader gui;
    public static FreeCam freeCam;

    static Program()
    {
        // im not sure half this stuff is needed but i dont think its hurting anything so
        On.TimeHandler.Update += TimeHandler_Update;
        On.CursorHandler.Update += CursorHandler_Update;
        On.HasteInputSystem.CanTakeInput += HasteInputSystem_CanTakeInput;
        On.CameraMovement.Update += CameraMovement_Update;
        On.CameraMovement.LateUpdate += CameraMovement_LateUpdate;
        On.CameraMovement.Move += CameraMovement_Move;
        On.MainCamera.Awake += MainCamera_Awake;
        On.MainCamera.SetTransformExternal += MainCamera_SetTransformExternal;

        var go = new GameObject(nameof(GUILoader));
        Object.DontDestroyOnLoad(go);
        gui = go.AddComponent<GUILoader>();
    }

    private static void CameraMovement_Move(On.CameraMovement.orig_Move orig, CameraMovement self, Vector3 position, Vector3 offset, Vector3 velocity)
    {
        if (gui.isFreeCam || gui.isMenuOpen)
        {
            return;
        }
        else
        {
            orig(self, position, offset, velocity);
        }
    }

    private static bool MainCamera_SetTransformExternal(On.MainCamera.orig_SetTransformExternal orig, MainCamera self, Vector3 position, Quaternion rotation, float setFOV, bool hideUI, bool transitionIn, bool transitionOut)
    {
        if (gui.isFreeCam || gui.isMenuOpen)
        {
            return false;
        }
        else
        {
            return orig(self, position, rotation, setFOV, hideUI, transitionIn, transitionOut);
        }
    }

    private static void CameraMovement_LateUpdate(On.CameraMovement.orig_LateUpdate orig, CameraMovement self)
    {
        if (gui.isFreeCam || gui.isMenuOpen)
        {
            return;
        }
        else
        {
            orig(self);
        }
    }

    private static void MainCamera_Awake(On.MainCamera.orig_Awake orig, MainCamera self)
    {
        orig(self);
        Debug.Log("MainCamera Awake");
        freeCam = self.gameObject.AddComponent<FreeCam>();
    }

    private static void CameraMovement_Update(On.CameraMovement.orig_Update orig, CameraMovement self)
    {
        if (gui.isFreeCam || gui.isMenuOpen)
        {
            return;
        }
        else
        {
            orig(self);
        }
    }

    private static bool HasteInputSystem_CanTakeInput(On.HasteInputSystem.orig_CanTakeInput orig)
    {
        if (gui.isPlayerControlled || gui.isMenuOpen)
        {
            return false;
        }
        return orig();
    }

    private static void CursorHandler_Update(On.CursorHandler.orig_Update orig, CursorHandler self)
    {
        if (gui.isMenuOpen)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
        else if (!gui.isMenuOpen && gui.isFreeCam)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            orig(self);
        }
    }

    private static void TimeHandler_Update(On.TimeHandler.orig_Update orig, TimeHandler self)
    {
        if (gui.isTimeFrozen)
        {
            Shader.SetGlobalFloat("UnscaledTime", Time.unscaledTime);
            Time.timeScale = 0f;
        }
        else
        {
            orig(self);
        }
    }
}