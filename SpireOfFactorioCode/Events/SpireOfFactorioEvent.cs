using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using SpireOfFactorio.SpireOfFactorioCode.Extensions;

namespace SpireOfFactorio.SpireOfFactorioCode.Events
{
    public abstract class SpireOfFactorioEvent : CustomEventModel
    {
        /*
        public override string CustomInitialPortraitPath => ImageHelper.GetImagePath($"events/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png");
        public override string CustomBackgroundScenePath => SceneHelper.GetScenePath("events/background_scenes/" + Id.Entry.RemovePrefix().ToLowerInvariant());
        */

        public override string CustomInitialPortraitPath
        {
            get
            {
                var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".EventImagePath();
                MainFile.Logger.Info("Event " + Id.Entry.RemovePrefix().ToLowerInvariant() + ", check path: " + path);
                return ResourceLoader.Exists(path) ? path : "event.png".EventImagePath();
            }
        }

        public override string CustomBackgroundScenePath
        {
            get
            {
                var path = $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BackgroundSceneImagePath();
                return ResourceLoader.Exists(path) ? path : "background_scene.png".BackgroundSceneImagePath();
            }
        }

    }
}
