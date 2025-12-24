using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
using System;

namespace HidWizards.UCR.Plugins.Remapper
{
    [Plugin("DJI MotionController", Group = "Axis", Description = "Remap DJI Motion Controller to regular Joystick")]

    // Entrées
    [PluginInput(DeviceBindingCategory.Range, "Axis X")]  // 0
    [PluginInput(DeviceBindingCategory.Range, "Axis Y")]  // 1
    [PluginInput(DeviceBindingCategory.Range, "Axis Z")]  // 2 (Throttle)
    [PluginInput(DeviceBindingCategory.Range, "Axis Rx")] // 3 (Heading)
    [PluginInput(DeviceBindingCategory.Range, "Axis Ry")] // 4 (gimbal)

    // Sorties
    [PluginOutput(DeviceBindingCategory.Range, "Yaw")]    // 0
    [PluginOutput(DeviceBindingCategory.Range, "Pitch")]     // 1
    [PluginOutput(DeviceBindingCategory.Range, "Roll")] // 2
    [PluginOutput(DeviceBindingCategory.Range, "Throttle")]      // 3
    [PluginOutput(DeviceBindingCategory.Range, "Gimbal")]      // 4

    public class DjiMotionMapper : Plugin
    {
        private double headingRef = 0;
        private bool yawTrack = false;
        private const double pi = Math.PI;

        public DjiMotionMapper() { }

        public override void Update(params short[] values)
        {
            // 1. Normalisation (-1.0 à 1.0)
            double winX = values[0] / 32768.0;
            double winY = values[1] / 32768.0;
            double winRx = values[3] / 32768.0;
            double winRy = values[4] / 32768.0;

            //  Throttle : from 0.64 to 1.0
            double rawZ = values[2] / 32768.0;
            double winZ = rawZ / 0.64;

            // 2. from earth fixed frame to wrist frame
            double heading = winRx * pi;
            double Pitch = -(winY * Math.Cos(heading) - winX * Math.Sin(heading));
            double Roll = -(winY * Math.Sin(heading) + winX * Math.Cos(heading));

            // 3. heading to Yaw (compute delta from heading reference)
            double deltaHeading = heading - headingRef;
            if (deltaHeading > pi) deltaHeading -= (2.0 * pi);  //manage south
            else if (deltaHeading < -pi) deltaHeading += (2.0 * pi);

            // Trigger Yaw : Ry positive start tracking, Ry negative latch current heading and stop tracking
            if (winRy > 0.1) yawTrack = true;
            if (winRy < -0.1) yawTrack = false; // value 0 is reserved to "no change"

            double Yaw = 0;
            if (!yawTrack) headingRef = heading;
            else Yaw = deltaHeading / pi;


            // 4. outputs to virtual Joystick 
            // multiply by 32767 to go back to "short" and clamp to avoid overshoot
            WriteOutput(0, (short)Clamp(Yaw * 32767 * 2));  //mult by to 2 to increase sensitivity
            WriteOutput(1, (short)Clamp(Pitch * 32767 * 2));
            WriteOutput(2, (short)Clamp(Roll * 32767 * 2));
            WriteOutput(3, (short)Clamp(winZ * 32767));
            WriteOutput(4, (short)Clamp(winRy * 32767 /0.64));
        }

        // security to avoid overflow a type 'short'
        private double Clamp(double value)
        {
            if (value > 32767) return 32767;
            if (value < -32768) return -32768;
            return value;
        }
    }
}