# DJI_motionController-UCR-plugin


This custom C# plugin for **Universal Control Remapper (UCR)** transforms the unique sensor data of the **DJI Motion Controller** into a standard 5-axis virtual joystick.

## 🚀 Key Features

### 1. Relative Frame Rotation (Trigonometric Logic)
This plugin uses the controller's Heading to rotate the input coordinates. 
* **Tilting forward** always results in **Pitch**, no matter which direction you are facing. Same for Roll angle
* Uses real-time calculations to translate your hand's tilt/twist into the drone's relative frame.



### 2. Intelligent Yaw Tracking & Latching
* **Dynamic Yaw:** Calculates the difference between your current heading and a reference point.
* **Heading Lock (Controlled by gimbal slider Ry axis):**
    * **Engage (Ry top):** Starts tracking your rotation to command the Yaw.
    * **Lock (Ry down):** Freezes the current heading reference, allowing you to move your arm freely without yaw.
* **South Pole Correction:** Prevents sudden 360-degree snaps when crossing the South heading (wraparound logic).

### 3. Signal Optimization
* **Throttle Scaling:** DJI controllers cap the Z-axis at +/- 0.64. This plugin re-scales it to +/- 1.0 for full thrust.
* **Sensitivity Boost:** Pitch, Roll, and Yaw are boosted for high-performance maneuvers with comfortable wrist movements.
* **Overflow Protection:** Custom Clamp function ensures stability by preventing signal clipping.

---

## 🛠 Installation & Setup

1. **Deploy:** Copy `UCR.Plugins.dll` into your UCR installation folder (inside plugins folder).
2. **Add Plugin:** In UCR, add the **"DJI MotionController"** plugin.
3. **Map Inputs:**
    * **Axis X / Y:** Wrist Tilt/twist
    * **Axis Z:** Throttle Trigger
    * **Axis Rx:** Heading/Compass (Required for Trig)
    * **Axis Ry:** Tracking Toggle / Gimbal



---

## 🎮 Axis Mapping Table

| Plugin Input | DJI Hardware | Plugin Output |
| :--- | :--- | :--- |
| Axis X | Tilt X | **Yaw** (0) |
| Axis Y | Tilt Y | **Pitch** (1) |
| Axis Z | Throttle | **Roll** (2) |
| Axis Rx | Heading | **Throttle** (3) |
| Axis Ry | Gimbal/Lock | **Gimbal** (4) |
