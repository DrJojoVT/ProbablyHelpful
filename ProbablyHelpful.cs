namespace ProbablyHelpful
{
    public class ProbablyHelpful : IMod
    {
        public static ModLog Logger { get; private set; }
        public void Init(ModManifest manifest)
        {
            Logger = new ModLog(manifest);
            Logger.Log("Mod: Loaded: Probably Helpful!");
        }
        public void OnEnable()
        {
            ProbablyHelpfulItem.IsEnabled = true;
        }
        public void OnDisable()
        {
            ProbablyHelpfulItem.IsEnabled = false;
        }
    }
}