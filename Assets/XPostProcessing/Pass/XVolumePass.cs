namespace XPostProcessing
{
    public class XVolumePass : VolumePassBase
    {
        protected override string PostProcessingTag => "Render X PostProcessing Effects";

        protected override void OnInit()
        {
            AddEffects(typeof(XVolumePass).Assembly);
        }

    }
}
