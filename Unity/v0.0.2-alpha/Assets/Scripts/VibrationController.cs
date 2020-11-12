using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class VibrationController : MonoBehaviour
{
    public SteamVR_Action_Vibration haptic_action;
    public SteamVR_Action_Single trigger_action;

    private void Update()
    {
        if (trigger_action.GetAxis(SteamVR_Input_Sources.RightHand) > 0.5f) {
            PulseRight(1f, 150, 75);
        }
        if (trigger_action.GetAxis(SteamVR_Input_Sources.LeftHand) > 0.5f)
        {
            PulseLeft(1f, 150, 75);
        }
    }

    public void PulseLeft(float duration, float frequency, float amplitude)
    {
        Pulse(duration, frequency, amplitude, SteamVR_Input_Sources.LeftHand);
    }

    public void PulseRight(float duration, float frequency, float amplitude)
    {
        Pulse(duration, frequency, amplitude, SteamVR_Input_Sources.RightHand);
    }

    public void Pulse(float duration, float frequency, float amplitude, SteamVR_Input_Sources source) {
        haptic_action.Execute(0, duration, frequency, amplitude, source);
        //print("Pulse: " + source.ToString());
    }
}
