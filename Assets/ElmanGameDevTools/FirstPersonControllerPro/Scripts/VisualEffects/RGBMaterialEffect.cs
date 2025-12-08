using UnityEngine;

namespace ElmanGameDevTools.VisualEffects
{
    /// <summary>
    /// Creates dynamic RGB color effects on materials with various animation modes
    /// </summary>
    public class RGBMaterialEffect : MonoBehaviour
    {
        [Header("Material Reference")]
        public Material targetMaterial;

        [Header("Animation Settings")]
        [Range(0.1f, 10f)]
        public float speed = 1f;
        public ColorMode colorMode = ColorMode.SineWave;

        [Header("Color Properties")]
        [Range(0f, 1f)]
        public float saturation = 0.8f;
        [Range(0f, 1f)]
        public float brightness = 1f;

        [Header("Color Channels")]
        public bool useRed = true;
        public bool useGreen = true;
        public bool useBlue = true;

        [Header("Advanced Settings")]
        public string colorPropertyName = "_Color";

        // Private variables
        private float hue = 0f;
        private Renderer objectRenderer;

        /// <summary>
        /// Defines the available color animation modes
        /// </summary>
        public enum ColorMode
        {
            SineWave,
            Linear,
            PingPong
        }

        /// <summary>
        /// Initializes the material reference and sets up the effect
        /// </summary>
        void Start()
        {
            InitializeMaterialReference();
        }

        /// <summary>
        /// Attempts to find and assign a target material for the effect
        /// </summary>
        private void InitializeMaterialReference()
        {
            if (targetMaterial == null)
            {
                objectRenderer = GetComponent<Renderer>();
                if (objectRenderer != null)
                {
                    targetMaterial = objectRenderer.material;
                }
            }

            // Create new material if none found
            if (targetMaterial == null)
            {
                Debug.LogWarning("No material assigned! Creating new material.");
                targetMaterial = new Material(Shader.Find("Standard"));

                if (GetComponent<Renderer>() != null)
                {
                    GetComponent<Renderer>().material = targetMaterial;
                }
            }
        }

        /// <summary>
        /// Updates the color animation each frame
        /// </summary>
        void Update()
        {
            if (targetMaterial == null) return;

            UpdateHueValue();
            ApplyColorToMaterial();
        }

        /// <summary>
        /// Calculates the current hue value based on selected color mode
        /// </summary>
        private void UpdateHueValue()
        {
            switch (colorMode)
            {
                case ColorMode.SineWave:
                    hue = Mathf.Sin(Time.time * speed * 0.5f) * 0.5f + 0.5f;
                    break;

                case ColorMode.Linear:
                    hue += Time.deltaTime * speed * 0.1f;
                    if (hue > 1f) hue -= 1f;
                    break;

                case ColorMode.PingPong:
                    hue = Mathf.PingPong(Time.time * speed * 0.5f, 1f);
                    break;
            }
        }

        /// <summary>
        /// Applies the calculated color to the target material
        /// </summary>
        private void ApplyColorToMaterial()
        {
            Color rgbColor = Color.HSVToRGB(hue, saturation, brightness);
            ApplyChannelMask(ref rgbColor);
            targetMaterial.SetColor(colorPropertyName, rgbColor);
        }

        /// <summary>
        /// Applies channel masking to the color based on user settings
        /// </summary>
        /// <param name="color">Color to modify with channel masking</param>
        private void ApplyChannelMask(ref Color color)
        {
            if (!useRed) color.r = 0f;
            if (!useGreen) color.g = 0f;
            if (!useBlue) color.b = 0f;
        }

        /// <summary>
        /// Sets the animation speed
        /// </summary>
        /// <param name="newSpeed">New speed value (0.1 to 10)</param>
        public void SetSpeed(float newSpeed)
        {
            speed = Mathf.Clamp(newSpeed, 0.1f, 10f);
        }

        /// <summary>
        /// Sets the color saturation
        /// </summary>
        /// <param name="newSaturation">New saturation value (0 to 1)</param>
        public void SetSaturation(float newSaturation)
        {
            saturation = Mathf.Clamp01(newSaturation);
        }

        /// <summary>
        /// Sets the color brightness
        /// </summary>
        /// <param name="newBrightness">New brightness value (0 to 1)</param>
        public void SetBrightness(float newBrightness)
        {
            brightness = Mathf.Clamp01(newBrightness);
        }

        /// <summary>
        /// Changes the target material for the effect
        /// </summary>
        /// <param name="newMaterial">New material to apply effects to</param>
        public void SetTargetMaterial(Material newMaterial)
        {
            targetMaterial = newMaterial;
        }

        /// <summary>
        /// Enables or disables specific color channels
        /// </summary>
        /// <param name="red">Enable red channel</param>
        /// <param name="green">Enable green channel</param>
        /// <param name="blue">Enable blue channel</param>
        public void SetColorChannels(bool red, bool green, bool blue)
        {
            useRed = red;
            useGreen = green;
            useBlue = blue;
        }

        /// <summary>
        /// Changes the color animation mode
        /// </summary>
        /// <param name="mode">New color animation mode</param>
        public void SetColorMode(ColorMode mode)
        {
            colorMode = mode;
        }

        /// <summary>
        /// Resets all settings to their default values
        /// </summary>
        public void ResetSettings()
        {
            speed = 1f;
            saturation = 0.8f;
            brightness = 1f;
            useRed = useGreen = useBlue = true;
            colorMode = ColorMode.SineWave;
        }

        /// <summary>
        /// Gets the current color being displayed on the material
        /// </summary>
        /// <returns>Current RGB color</returns>
        public Color GetCurrentColor()
        {
            Color currentColor = Color.HSVToRGB(hue, saturation, brightness);
            ApplyChannelMask(ref currentColor);
            return currentColor;
        }
    }
}