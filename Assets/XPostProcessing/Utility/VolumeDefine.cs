using System;

namespace XPostProcessing
{
    /// <summary>
    /// Volume的显示菜单.
    /// </summary>
    public class VolumeMenu
    {
        public const string Root = "X-Post-Processing/";
        public const string Blur = Root + "模糊 (Blur)/";
        public const string Glitch = Root + "故障效果 (Glitch)/";
        public const string Pixelate = Root + "像素化 (Pixelate)/";
        public const string Vignette = Root + "渐晕 (Vignette)/";
        public const string EdgeDetection = Root + "边缘检测 (Edge Detection)/";
        public const string ImageProcessing = Root + "图像处理 (Image Processing)/";
        public const string ColorAdjustment = Root + "色彩调整 (Color Adjustment)/";
        public const string Environment = Root + "环境后效 (Environment)/";
        public const string Skill = Root + "技能效果 (Skill)/";
        public const string Story = Root + "过场动画 (Story)/";
        public const string Test = Root + "测试 (Test)/";
    }

    /// <summary>
    /// Volume的渲染优先级，10、20、30...以此类推的叠加数值用于内置的Volume优先级.
    /// </summary>
    public class VolumePriority
    {
        public const int Before = 0;
        public const int Environment = 1000;
        public const int Glitch = 2000;
        public const int EdgeDetection = 3000;
        public const int Pixelate = 4000;
        public const int Blur = 5000;
        public const int ImageProcessing = 6000;
        public const int ColorAdjustment = 7000;
        public const int Skill = 8000;
        public const int Story = 9000;
        public const int Vignette = 10000;
        public const int After = 11000;
    }

    /// <summary>
    /// Volume的渲染优先级的属性标签，只有添加此属性标签的Renderer才会被执行.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class VolumeRendererPriority : Attribute
    {
        public readonly int priority;

        public VolumeRendererPriority(int priority)
        {
            this.priority = priority;
        }
    }

}