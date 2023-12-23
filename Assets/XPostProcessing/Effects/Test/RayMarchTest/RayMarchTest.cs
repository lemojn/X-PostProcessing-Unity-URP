using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace XPostProcessing
{
    [VolumeComponentMenu(VolumeMenu.Test + "光线步进测试 (Ray March Test)")]
    public class RayMarchTest : VolumeSettingBase
    {
        public override bool IsActive() => enable.value;
        public BoolParameter enable = new(false);
        public BoolParameter frustumCornersRay = new(false);
    }

    [VolumeRendererPriority(VolumePriority.After + 1)]
    public class RayMarchTestRenderer : VolumeRendererBase<RayMarchTest>
    {
        public override string ProfilerTag => "Test-RayMarchTest";
        protected override string ShaderName => "Hidden/XPostProcessing/Test/RayMarchTest";

        static class ShaderIDs
        {
            internal static readonly int FrustumCornersRay = Shader.PropertyToID("_FrustumCornersRay");
        }

        public override void Render(CommandBuffer cmd, RTHandle source, RTHandle target, ref RenderingData renderingData)
        {
            if (m_Settings.frustumCornersRay.value)
            {
                var camera = renderingData.cameraData.camera;
                Matrix4x4 frustumCorners = Matrix4x4.identity;

                float fov = camera.fieldOfView;
                float near = camera.nearClipPlane;
                float aspect = camera.aspect;

                float halfHeight = near * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
                Vector3 toRight = aspect * halfHeight * camera.transform.right;
                Vector3 toTop = halfHeight * camera.transform.up;

                Vector3 topLeft = camera.transform.forward * near + toTop - toRight;
                float scale = topLeft.magnitude / near;
                topLeft.Normalize();
                topLeft *= scale;

                Vector3 topRight = camera.transform.forward * near + toTop + toRight;
                topRight.Normalize();
                topRight *= scale;

                Vector3 bottomLeft = camera.transform.forward * near - toTop - toRight;
                bottomLeft.Normalize();
                bottomLeft *= scale;

                Vector3 bottomRight = camera.transform.forward * near - toTop + toRight;
                bottomRight.Normalize();
                bottomRight *= scale;

                frustumCorners.SetRow(0, bottomLeft);
                frustumCorners.SetRow(1, bottomRight);
                frustumCorners.SetRow(2, topRight);
                frustumCorners.SetRow(3, topLeft);

                m_BlitMaterial.SetMatrix(ShaderIDs.FrustumCornersRay, frustumCorners);
                cmd.Blit(source, target, m_BlitMaterial, 0);
            }
            else
            {
                // 推荐使用这个方法，在RayMarchTest.shader文件中查看具体实现.
                Blitter.BlitCameraTexture(cmd, source, target, m_BlitMaterial, 1);
            }
        }

    }
}
