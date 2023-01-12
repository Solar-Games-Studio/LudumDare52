using qASIC.Input.Devices;
using UnityEngine;

namespace Game.Input
{
    public class GameInput : MonoBehaviour
    {
        private static IInputDevice _lastDevice = null;
        public static IInputDevice LastDevice
        { 
            get
            {
                if (_lastDevice == null)
                    _lastDevice = DeviceManager.Devices[0];

                return _lastDevice;
            }
            private set
            {
                _lastDevice = value;
            }
        }

        private void Update()
        {
            foreach (var device in DeviceManager.Devices)
            {
                if (string.IsNullOrEmpty(device.GetAnyKeyDown())) continue;
                LastDevice = device;
                return;
            }
        }
    }
}