﻿using ApplicationManagers;
using Cameras;
using System;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using Utility;

namespace Settings
{
    class GraphicsSettings : SaveableSettingsContainer
    {
        protected override string FileName { get { return "Graphics.json"; } }
        public IntSetting PresetQuality = new IntSetting((int)PresetQualityLevel.VeryHigh);
        public IntSetting FullScreenMode = new IntSetting((int)FullScreenLevel.Borderless);
        public IntSetting ScreenResolution = new IntSetting(0);
        public IntSetting FPSCap = new IntSetting(144, minValue: 0);
        public IntSetting MenuFPSCap = new IntSetting(60, minValue: 0);
        public BoolSetting VSync = new BoolSetting(false);
        public BoolSetting InterpolationEnabled = new BoolSetting(true);
        public BoolSetting ShowFPS = new BoolSetting(false);
        public IntSetting RenderDistance = new IntSetting(10000, minValue: 10, maxValue: 1000000);
        public IntSetting TextureQuality = new IntSetting((int)TextureQualityLevel.High);
        public IntSetting ShadowQuality = new IntSetting((int)ShadowQualityLevel.High);
        public IntSetting ShadowDistance = new IntSetting(1000, minValue: 0, maxValue: 3000);
        public IntSetting LightDistance = new IntSetting(1000, minValue: 0, maxValue: 3000);
        public IntSetting DetailDistance = new IntSetting(500, minValue: 0, maxValue: 1000);  // Added by Snake for Terrain Detail Slider 26 may 24
        public IntSetting DetailDensity = new IntSetting(500, minValue: 0, maxValue: 1000);  // Added by Snake for Terrain Detail Slider 27 may 24
        public IntSetting TreeDistance = new IntSetting(5000, minValue: 0, maxValue: 5000);  // Added by Snake for Terrain Detail Slider 28 may 24
        public IntSetting AntiAliasing = new IntSetting((int)AntiAliasingLevel.On, minValue: 0, maxValue: (int)Util.EnumMaxValue<AntiAliasingLevel>());
        public IntSetting AnisotropicFiltering = new IntSetting((int)AnisotropicLevel.Low);
        public IntSetting WeatherEffects = new IntSetting((int)WeatherEffectLevel.High);
        public BoolSetting WeaponTrailEnabled = new BoolSetting(true);
        public BoolSetting WindEffectEnabled = new BoolSetting(true);
        public BoolSetting BloodSplatterEnabled = new BoolSetting(true);
        public BoolSetting NapeBloodEnabled = new BoolSetting(true);
        public BoolSetting MipmapEnabled = new BoolSetting(true);
        public BoolSetting LensFlare = new BoolSetting(true); // added by Ata for Flash Flare Settings //

        // Post Processing
        public IntSetting AmbientOcclusion = new IntSetting((int)AmbientOcclusionLevel.Off, minValue: 0, maxValue: (int)Util.EnumMaxValue<AmbientOcclusionLevel>());
        public IntSetting Bloom = new IntSetting((int)BloomLevel.Low, minValue: 0, maxValue: (int)Util.EnumMaxValue<BloomLevel>());
        public IntSetting ChromaticAberration = new IntSetting((int)ChromaticAberrationLevel.Low, minValue: 0, maxValue: (int)Util.EnumMaxValue<ChromaticAberrationLevel>());
        public IntSetting ColorGrading = new IntSetting((int)ColorGradingLevel.Off, minValue: 0, maxValue: (int)Util.EnumMaxValue<ColorGradingLevel>());
        public IntSetting AutoExposure = new IntSetting((int)AutoExposureLevel.On, minValue: 0, maxValue: (int)Util.EnumMaxValue<AutoExposureLevel>());
        public IntSetting DepthOfField = new IntSetting((int)DepthOfFieldLevel.Off, minValue: 0, maxValue: (int)Util.EnumMaxValue<DepthOfFieldLevel>());
        public IntSetting MotionBlur = new IntSetting((int)MotionBlurLevel.Off, minValue: 0, maxValue: (int)Util.EnumMaxValue<MotionBlurLevel>());
        public IntSetting WaterFX = new IntSetting((int)WaterFXLevel.High, minValue: 0, maxValue: (int)Util.EnumMaxValue<WaterFXLevel>());

        public override void Apply()
        {
            // Added by Snake for Terrain Detail Slider 28 may 24 
            SetTerrainDetails(DetailDistance.Value, DetailDensity.Value, TreeDistance.Value);

            if (ShadowQuality.Value == (int)ShadowQualityLevel.Off)
                QualitySettings.shadows = UnityEngine.ShadowQuality.Disable;
            else if (ShadowQuality.Value == (int)ShadowQualityLevel.Low)
            {
                QualitySettings.shadows = UnityEngine.ShadowQuality.HardOnly;
                QualitySettings.shadowResolution = ShadowResolution.Medium;
                QualitySettings.shadowCascades = 0;
            }
            else if (ShadowQuality.Value == (int)ShadowQualityLevel.Medium)
            {
                QualitySettings.shadows = UnityEngine.ShadowQuality.All;
                QualitySettings.shadowResolution = ShadowResolution.High;
                QualitySettings.shadowCascades = 2;
            }
            else if (ShadowQuality.Value == (int)ShadowQualityLevel.High)
            {
                QualitySettings.shadows = UnityEngine.ShadowQuality.All;
                QualitySettings.shadowResolution = ShadowResolution.VeryHigh;
                QualitySettings.shadowCascades = 4;
            }
            QualitySettings.vSyncCount = Convert.ToInt32(VSync.Value);
            if (SceneLoader.SceneName == SceneName.InGame || SceneLoader.SceneName == SceneName.MapEditor)
                Application.targetFrameRate = FPSCap.Value > 0 ? FPSCap.Value : -1;
            else
                Application.targetFrameRate = MenuFPSCap.Value > 0 ? MenuFPSCap.Value : -1;
            QualitySettings.globalTextureMipmapLimit = 3 - TextureQuality.Value;
            QualitySettings.anisotropicFiltering = (AnisotropicFiltering)AnisotropicFiltering.Value;
            QualitySettings.antiAliasing = 0;
            QualitySettings.shadowDistance = ShadowDistance.Value;
            if (SceneLoader.CurrentCamera is InGameCamera)
                ((InGameCamera)SceneLoader.CurrentCamera).ApplyGraphicsSettings();
            ScreenResolution.Value = FullscreenHandler.SanitizeResolutionSetting(ScreenResolution.Value);
            FullscreenHandler.Apply(ScreenResolution.Value, (FullScreenLevel)FullScreenMode.Value);
            PostProcessingManager postProcessingManager = GameObject.FindFirstObjectByType<PostProcessingManager>();
            if (postProcessingManager != null)
                postProcessingManager.ApplySettings(
                    (AmbientOcclusionLevel)AmbientOcclusion.Value,
                    (BloomLevel)Bloom.Value,
                    (ChromaticAberrationLevel)ChromaticAberration.Value,
                    (ColorGradingLevel)ColorGrading.Value,
                    (AutoExposureLevel)AutoExposure.Value,
                    (DepthOfFieldLevel)DepthOfField.Value,
                    (MotionBlurLevel)MotionBlur.Value,
                    (WaterFXLevel)WaterFX.Value
                );
        }

        // Added by Snake for Terrain Detail Slider 28 may 24 
        public void SetTerrainDetails(int DetailDistance, int DetailDensity, int TreeDistance)
        {
            Terrain[] terrains = GameObject.FindObjectsOfType<Terrain>();
            foreach (Terrain terrain in terrains)
            {
                terrain.detailObjectDistance = DetailDistance;
                terrain.detailObjectDensity = DetailDensity /1000f; 
                terrain.treeDistance = TreeDistance;
            }
        }

        public void OnSelectPreset()
        {
            if (PresetQuality.Value == (int)PresetQualityLevel.VeryLow)
            {
                TextureQuality.Value = (int)TextureQualityLevel.VeryLow;
                ShadowQuality.Value = (int)ShadowQualityLevel.Off;
                AntiAliasing.Value = (int)AntiAliasingLevel.Off;
                AnisotropicFiltering.Value = (int)AnisotropicLevel.Off;
                WeatherEffects.Value = (int)WeatherEffectLevel.Off;
                ShadowDistance.Value = 500;
                DetailDistance.Value = 0;
                DetailDensity.Value = 0;
                TreeDistance.Value = 400;
                LightDistance.Value = 0;
                Bloom.Value = (int)BloomLevel.Off;
                MotionBlur.Value = (int)MotionBlurLevel.Off;
                ColorGrading.Value = (int)ColorGradingLevel.Off;
                DepthOfField.Value = (int)DepthOfFieldLevel.Off;
                ChromaticAberration.Value = (int)ChromaticAberrationLevel.Off;
                AmbientOcclusion.Value = (int)AmbientOcclusionLevel.Off;
                WaterFX.Value = (int)WaterFXLevel.Low;
                AutoExposure.Value = (int)AutoExposureLevel.Off;
                RenderDistance.Value = 1000;
            }
            else if (PresetQuality.Value == (int)PresetQualityLevel.Low)
            {
                TextureQuality.Value = (int)TextureQualityLevel.High;
                ShadowQuality.Value = (int)ShadowQualityLevel.Off;
                AntiAliasing.Value = (int)AntiAliasingLevel.Off;
                AnisotropicFiltering.Value = (int)AnisotropicLevel.Off;
                WeatherEffects.Value = (int)WeatherEffectLevel.Low;
                ShadowDistance.Value = 500;
                DetailDistance.Value = 200;
                DetailDensity.Value = 100;
                TreeDistance.Value = 400;
                LightDistance.Value = 100;
                Bloom.Value = (int)BloomLevel.Off;
                MotionBlur.Value = (int)MotionBlurLevel.Off;
                ColorGrading.Value = (int)ColorGradingLevel.Off;
                DepthOfField.Value = (int)DepthOfFieldLevel.Off;
                ChromaticAberration.Value = (int)ChromaticAberrationLevel.Off;
                AmbientOcclusion.Value = (int)AmbientOcclusionLevel.Off;
                WaterFX.Value = (int)WaterFXLevel.Low;
                AutoExposure.Value = (int)AutoExposureLevel.On;
                RenderDistance.Value = 2000;
            }
            else if (PresetQuality.Value == (int)PresetQualityLevel.Medium)
            {
                TextureQuality.Value = (int)TextureQualityLevel.High;
                ShadowQuality.Value = (int)ShadowQualityLevel.Low;
                AntiAliasing.Value = (int)AntiAliasingLevel.On;
                AnisotropicFiltering.Value = (int)AnisotropicLevel.Low;
                WeatherEffects.Value = (int)WeatherEffectLevel.Medium;
                ShadowDistance.Value = 500;
                DetailDistance.Value = 500;
                DetailDensity.Value = 250;
                TreeDistance.Value = 1000;
                LightDistance.Value = 250;
                Bloom.Value = (int)BloomLevel.Low;
                MotionBlur.Value = (int)MotionBlurLevel.Off;
                ColorGrading.Value = (int)ColorGradingLevel.Off;
                DepthOfField.Value = (int)DepthOfFieldLevel.Off;
                ChromaticAberration.Value = (int)ChromaticAberrationLevel.Low;
                AmbientOcclusion.Value = (int)AmbientOcclusionLevel.Off;
                WaterFX.Value = (int)WaterFXLevel.Medium;
                AutoExposure.Value = (int)AutoExposureLevel.On;
                RenderDistance.Value = 5000;
            }
            else if (PresetQuality.Value == (int)PresetQualityLevel.High)
            {
                TextureQuality.Value = (int)TextureQualityLevel.High;
                ShadowQuality.Value = (int)ShadowQualityLevel.Medium;
                AntiAliasing.Value = (int)AntiAliasingLevel.On;
                AnisotropicFiltering.Value = (int)AnisotropicLevel.High;
                WeatherEffects.Value = (int)WeatherEffectLevel.High;
                ShadowDistance.Value = 1000;
                LightDistance.Value = 500;
                DetailDistance.Value = 800;
                DetailDensity.Value = 380;
                TreeDistance.Value = 2500;
                Bloom.Value = (int)BloomLevel.Low;
                MotionBlur.Value = (int)MotionBlurLevel.Off;
                ColorGrading.Value = (int)ColorGradingLevel.Off;
                DepthOfField.Value = (int)DepthOfFieldLevel.Off;
                ChromaticAberration.Value = (int)ChromaticAberrationLevel.Low;
                AmbientOcclusion.Value = (int)AmbientOcclusionLevel.Off;
                WaterFX.Value = (int)WaterFXLevel.High;
                AutoExposure.Value = (int)AutoExposureLevel.On;
                RenderDistance.Value = 10000;
            }
            else if (PresetQuality.Value == (int)PresetQualityLevel.VeryHigh)
            {
                TextureQuality.Value = (int)TextureQualityLevel.High;
                ShadowQuality.Value = (int)ShadowQualityLevel.High;
                AntiAliasing.Value = (int)AntiAliasingLevel.On;
                AnisotropicFiltering.Value = (int)AnisotropicLevel.High;
                WeatherEffects.Value = (int)WeatherEffectLevel.High;
                ShadowDistance.Value = 1000;
                LightDistance.Value = 1000;
                DetailDistance.Value = 1000;
                DetailDensity.Value = 500;
                TreeDistance.Value = 5000;
                Bloom.Value = (int)BloomLevel.Low;
                MotionBlur.Value = (int)MotionBlurLevel.Off;
                ColorGrading.Value = (int)ColorGradingLevel.Off;
                DepthOfField.Value = (int)DepthOfFieldLevel.Off;
                ChromaticAberration.Value = (int)ChromaticAberrationLevel.Low;
                AmbientOcclusion.Value = (int)AmbientOcclusionLevel.Off;
                WaterFX.Value = (int)WaterFXLevel.High;
                AutoExposure.Value = (int)AutoExposureLevel.On;
                RenderDistance.Value = 10000;
            }
        }
    }

    public enum PresetQualityLevel
    {
        VeryLow,
        Low,
        Medium,
        High,
        VeryHigh
    }

    public enum TextureQualityLevel
    {
        VeryLow,
        Low,
        Medium,
        High
    }

    public enum ShadowQualityLevel
    {
        Off,
        Low,
        Medium,
        High
    }

    public enum AntiAliasingLevel
    {
        Off,
        On
    }

    public enum AnisotropicLevel
    {
        Off,
        Low,
        High
    }

    public enum AmbientOcclusionLevel
    {
        Off,
        Lowest,
        Low,
        Medium,
        High,
        Ultra
    }

    public enum BloomLevel
    {
        Off,
        Low,
        High
    }

    public enum ChromaticAberrationLevel
    {
        Off,
        Low,
        High
    }

    public enum ColorGradingLevel
    {
        Off,
        On
    }

    public enum DepthOfFieldLevel
    {
        Off,
        Low,
        Medium,
        High,
    }

    public enum MotionBlurLevel
    {
        Off,
        Low,
        Medium,
        High
    }

    public enum AutoExposureLevel
    {
        Off,
        On
    }

    public enum WaterFXLevel
    {
        Off,
        Low,
        Medium,
        High
    }

    public enum WeatherEffectLevel
    {
        Off,
        Low,
        Medium,
        High
    }

    public enum TitanSpawnEffectLevel
    {
        Off,
        Quarter,
        Half,
        Full
    }

    public enum FullScreenLevel
    {
        Windowed,
        Borderless,
        Exclusive
    }
}
