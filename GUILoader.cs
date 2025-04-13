using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using Zorro.Core;
using Zorro.Core.CLI;
using Object = UnityEngine.Object;

namespace FC
{
    public class GUILoader : MonoBehaviour
    {
        public bool isMenuOpen;
        private bool wasFreeCam = false;

        private float MENUWIDTH;
        private float MENUHEIGHT;
        private float MENUX;
        private float MENUY;
        private float ITEMWIDTH;
        private float ITEMHEIGHT;
        private float CENTERX;

        private Vector2 scrollPos;

        #region Public Values
        public bool isTimeFrozen = false;
        public bool isFreeCam = false;
        public bool isPlayerControlled = true;
        public bool isFreeCamControlled = false;
        public float movementSpeed = 10f;
        public float fastMovementSpeed = 100f;
        public float freeLookSensitivity = 3f;
        public float zoomSensitivity = 10f;
        public float fastZoomSensitivity = 50f;
        #endregion

        private GUIStyle menuStyle;
        private GUIStyle buttonStyle;
        private GUIStyle labelStyle;
        private GUIStyle toggleStyle;
        private GUIStyle hScrollStyle;
        private GUIStyle vScrollStyle;
        private GUIStyle textFeildStyle;

        private void Awake()
        {
            Debug.Log("The GUILoader has awaken :)");
            isMenuOpen = false;
            MENUX = 0f;
            MENUY = 0f;
            ITEMWIDTH = 300f / 1920f * Screen.width;
            ITEMHEIGHT = 30f / 1080f * Screen.height;
            MENUWIDTH = 300f / 1920f * Screen.width;
            MENUHEIGHT = 1080f / 1080f * Screen.height;

            Debug.Log($"MENUWIDTH: {MENUWIDTH}, MENUHEIGHT: {MENUHEIGHT}, MENUX: {MENUX}, MENUY: {MENUY}, ITEMWIDTH: {ITEMWIDTH}, ITEMHEIGHT: {ITEMHEIGHT}, CENTERX: {CENTERX}");
            Debug.Log($"Screen.width: {Screen.width}, Screen.height: {Screen.height}");
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private void IntitializeMenu()
        {
            if (menuStyle == null)
            {
                menuStyle = new GUIStyle(GUI.skin.box);
                buttonStyle = new GUIStyle(GUI.skin.button);
                labelStyle = new GUIStyle(GUI.skin.label);
                toggleStyle = new GUIStyle(GUI.skin.toggle);
                hScrollStyle = new GUIStyle(GUI.skin.horizontalSlider);
                vScrollStyle = new GUIStyle(GUI.skin.verticalScrollbar);
                textFeildStyle = new GUIStyle(GUI.skin.textField);

                menuStyle.normal.textColor = Color.white;
                menuStyle.normal.background = MakeTex(2, 2, new Color(0.01f, 0.01f, 0.1f, .9f));
                menuStyle.fontSize = 18;
                menuStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

                buttonStyle.normal.textColor = Color.white;
                buttonStyle.fontSize = 18;

                labelStyle.normal.textColor = Color.white;
                labelStyle.normal.background = MakeTex(2, 2, new Color(0.01f, 0.01f, 0.1f, .9f));
                labelStyle.fontSize = 18;
                labelStyle.alignment = TextAnchor.MiddleCenter;
                labelStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

                toggleStyle.normal.textColor = Color.white;
                toggleStyle.fontSize = 18;

                Texture2D hScrollBackground = MakeTex(2, 2, new Color(1f, 1f, 1f, .9f));
                hScrollBackground.hideFlags = HideFlags.HideAndDontSave;

                hScrollStyle.normal.textColor = Color.white;
                hScrollStyle.normal.background = hScrollBackground;
                hScrollStyle.active.background = hScrollBackground;
                hScrollStyle.hover.background = hScrollBackground;

                vScrollStyle.normal.textColor = Color.white;
                vScrollStyle.normal.background = MakeTex(2, 2, new Color(0.01f, 0.01f, 0.1f, .9f));
                vScrollStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;
                vScrollStyle.alignment = TextAnchor.UpperLeft;

                textFeildStyle.normal.textColor = Color.white;
                textFeildStyle.fontSize = 18;
            }
        }


        public void OnDestroy()
        {
            Debug.Log("The GUILoader was destroyed :(");
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F4))
            {
                isMenuOpen = !isMenuOpen;
                if (isMenuOpen)
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.Confined;
                }
                else
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
            }

            Program.freeCam.movementSpeed = movementSpeed;
            Program.freeCam.fastMovementSpeed = fastMovementSpeed;
            Program.freeCam.freeLookSensitivity = freeLookSensitivity;
            Program.freeCam.zoomSensitivity = zoomSensitivity;
            Program.freeCam.fastZoomSensitivity = fastZoomSensitivity;
        }

        public void OnGUI()
        {
            if (menuStyle == null) { IntitializeMenu(); }

            if (isMenuOpen)
            {
                GUI.Box(new Rect(MENUX, MENUY, MENUWIDTH, MENUHEIGHT), "Freecam", menuStyle);

                float yPos = MENUY;

                isTimeFrozen = GUI.Toggle(new Rect(MENUX, yPos += ITEMHEIGHT, ITEMWIDTH, ITEMHEIGHT), isTimeFrozen, "Toggle Time Frozen", toggleStyle);
                //GUI.Label(new Rect(MENUX, yPos += ITEMHEIGHT, ITEMWIDTH, ITEMHEIGHT), "Some Post Processing Effects Are Baked In", labelStyle);

                isFreeCam = GUI.Toggle(new Rect(MENUX, yPos += ITEMHEIGHT, ITEMWIDTH, ITEMHEIGHT), isFreeCam, "Toggle Freecam", toggleStyle);

                isPlayerControlled = GUI.Toggle(new Rect(MENUX, yPos += ITEMHEIGHT, ITEMWIDTH, ITEMHEIGHT), isPlayerControlled, "Toggle Player Controlled", toggleStyle);

                isFreeCamControlled = GUI.Toggle(new Rect(MENUX, yPos += ITEMHEIGHT, ITEMWIDTH, ITEMHEIGHT), isFreeCamControlled, "Toggle Freecam Controlled", toggleStyle);

                GUI.Label(new Rect(MENUX, yPos += ITEMHEIGHT, ITEMWIDTH, ITEMHEIGHT), $"Movement Speed: {movementSpeed}", labelStyle);
                movementSpeed = GUI.HorizontalSlider(new Rect(MENUX, yPos += ITEMHEIGHT, ITEMWIDTH, ITEMHEIGHT), movementSpeed, 0f, 1000f);

                GUI.Label(new Rect(MENUX, yPos += ITEMHEIGHT, ITEMWIDTH, ITEMHEIGHT), $"Fast Movement Speed: {fastMovementSpeed}", labelStyle);
                fastMovementSpeed = GUI.HorizontalSlider(new Rect(MENUX, yPos += ITEMHEIGHT, ITEMWIDTH, ITEMHEIGHT), fastMovementSpeed, 0f, 1000f);

                GUI.Label(new Rect(MENUX, yPos += ITEMHEIGHT, ITEMWIDTH, ITEMHEIGHT), $"Free Look Sensitivity: {freeLookSensitivity}", labelStyle);
                freeLookSensitivity = GUI.HorizontalSlider(new Rect(MENUX, yPos += ITEMHEIGHT, ITEMWIDTH, ITEMHEIGHT), freeLookSensitivity, 0f, 1000f);

                GUI.Label(new Rect(MENUX, yPos += ITEMHEIGHT, ITEMWIDTH, ITEMHEIGHT), $"Zoom Sensitivity: {zoomSensitivity}", labelStyle);
                zoomSensitivity = GUI.HorizontalSlider(new Rect(MENUX, yPos += ITEMHEIGHT, ITEMWIDTH, ITEMHEIGHT), zoomSensitivity, 0f, 1000f);

                GUI.Label(new Rect(MENUX, yPos += ITEMHEIGHT, ITEMWIDTH, ITEMHEIGHT), $"Fast Zoom Sensitivity: {fastZoomSensitivity}", labelStyle);
                fastZoomSensitivity = GUI.HorizontalSlider(new Rect(MENUX, yPos += ITEMHEIGHT, ITEMWIDTH, ITEMHEIGHT), fastZoomSensitivity, 0f, 1000f);
            }
        }
    }
}