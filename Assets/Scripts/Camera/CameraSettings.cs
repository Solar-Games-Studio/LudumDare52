using UnityEngine;
using qASIC.SettingsSystem;

public class CameraSettings : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] int scpslModeTargetFramerate = 15;

    public static bool SCPSLMode { get; private set; }

    private void Reset()
    {
        cam = GetComponent<Camera>();
    }

    static int _scpslModeRenderCount = 8;

    private void Update()
    {
        if (SCPSLMode)
        {
            for (int i = 0; i < _scpslModeRenderCount; i++)
                cam.Render();

            _scpslModeRenderCount += Time.deltaTime < 1f / scpslModeTargetFramerate ? 1 : -1;
            qASIC.qDebug.DisplayValue("_scpslModeRenderCount", _scpslModeRenderCount);
        }
    }

    [OptionsSetting("scpsl_mode", false)]
    static void ChangeSCPSLMode(bool value) =>
        SCPSLMode = value;
}