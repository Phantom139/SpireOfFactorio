using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;
using SpireOfFactorio.Libraries.ConstructLib.Hooks;
using System.Reflection;

namespace SpireOfFactorio.SpireOfFactorioCode
{
    [ModInitializer(nameof(Initialize))]
    public partial class MainFile : Node
    {
        public const string ModId = "SpireOfFactorio"; //Used for resource filepath
        public const string ResPath = $"res://{ModId}";

        public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

        public static void Initialize()
        {
            var assembly = Assembly.GetExecutingAssembly();
            Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(assembly);

            Harmony harmony = new(ModId);
            harmony.PatchAll();

            ConstructHooks.Initialize();

            Logger.Debug("SpireOfFactorio Mod Initialized");
        }
    }
}
