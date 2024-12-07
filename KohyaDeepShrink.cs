using Newtonsoft.Json.Linq;
using SwarmUI.Builtin_ComfyUIBackend;
using SwarmUI.Core;
using SwarmUI.Text2Image;

// NOTE: Namespace must NOT contain "SwarmUI" (this is reserved for built-ins)
namespace DerrianDistro.Extensions.KohyaDeepShrink;

// NOTE: Classname must match filename
public class KohyaDeepShrink : Extension // extend the "Extension" class in Swarm Core
{
    // Generally define parameters as "public static" to make them easy to access in other code, actual registration is done in OnInit
    public static T2IRegisteredParam<int> BlockNumber;
    public static T2IRegisteredParam<double> DownscaleFactor,
        StartPercent,
        EndPercent;
    public static T2IRegisteredParam<bool> DownscaleAfterSkip;
    public static T2IRegisteredParam<string> DownscaleMethod,
        UpscaleMethod;
    public static List<string> UpscaleDownscaleMethods =
    [
        "bicubic",
        "nearest-exact",
        "bilinear",
        "area",
        "bislerp",
    ];

    public static T2IParamGroup KohyaDeepShrinkParamGroup;

    // OnInit is called when the extension is loaded, and is the general place to register most things
    public override void OnInit()
    {
        ComfyUIBackendExtension.NodeToFeatureMap["PatchModelAddDownscale"] =
            "PatchModelAddDownscale";
        KohyaDeepShrinkParamGroup = new(
            "Kohya Deep Shrink",
            Toggles: true,
            Open: false,
            IsAdvanced: false
        );

        BlockNumber = T2IParamTypes.Register<int>(
            new(
                "Block Number",
                "The block number to use for the downscale.",
                "8",
                Group: KohyaDeepShrinkParamGroup,
                FeatureFlag: "comfyui",
                Min: 1,
                Max: 32,
                Step: 1,
                OrderPriority: 10
            )
        );

        DownscaleFactor = T2IParamTypes.Register<double>(
            new(
                "Downscale Factor",
                "The factor to downscale the image by.",
                "2",
                Group: KohyaDeepShrinkParamGroup,
                FeatureFlag: "comfyui",
                Min: 0.1,
                Max: 9.0,
                Step: 0.001,
                ViewMin: 0.1,
                ViewMax: 9.0,
                ViewType: ParamViewType.SLIDER,
                OrderPriority: 20
            )
        );

        StartPercent = T2IParamTypes.Register<double>(
            new(
                "Start Percent",
                "The percentage to start the downscale at.",
                "0.0",
                Group: KohyaDeepShrinkParamGroup,
                FeatureFlag: "comfyui",
                Min: 0.0,
                Max: 1.0,
                Step: 0.001,
                ViewMin: 0.0,
                ViewMax: 1.0,
                ViewType: ParamViewType.SLIDER,
                OrderPriority: 30
            )
        );

        EndPercent = T2IParamTypes.Register<double>(
            new(
                "End Percent",
                "The percentage to end the downscale at.",
                "0.35",
                Group: KohyaDeepShrinkParamGroup,
                FeatureFlag: "comfyui",
                Min: 0.0,
                Max: 1.0,
                Step: 0.001,
                ViewMin: 0.0,
                ViewMax: 1.0,
                ViewType: ParamViewType.SLIDER,
                OrderPriority: 40
            )
        );

        DownscaleMethod = T2IParamTypes.Register<string>(
            new(
                "Downscale Method",
                "The method to use for the downscale.",
                "bicubic",
                Group: KohyaDeepShrinkParamGroup,
                FeatureFlag: "comfyui",
                GetValues: (_) => UpscaleDownscaleMethods,
                OrderPriority: 50
            )
        );

        UpscaleMethod = T2IParamTypes.Register<string>(
            new(
                "Upscale Method",
                "The method to use for the upscale.",
                "bicubic",
                Group: KohyaDeepShrinkParamGroup,
                FeatureFlag: "comfyui",
                GetValues: (_) => UpscaleDownscaleMethods,
                OrderPriority: 60
            )
        );
        DownscaleAfterSkip = T2IParamTypes.Register<bool>(
            new(
                "Downscale After Skip",
                "Whether to downscale after the skip.",
                "true",
                Group: KohyaDeepShrinkParamGroup,
                FeatureFlag: "comfyui",
                OrderPriority: 70
            )
        );

        // AddStep for custom Comfy steps. Can also use AddModelGenStep for custom model configuration steps.
        WorkflowGenerator.AddStep(
            g =>
            {
                // Check that all required parameters exist
                if (!g.UserInput.TryGet(BlockNumber, out int blockNumber))
                    return;
                if (!g.UserInput.TryGet(DownscaleFactor, out double downscaleFactor))
                    return;
                if (!g.UserInput.TryGet(StartPercent, out double startPercent))
                    return;
                if (!g.UserInput.TryGet(EndPercent, out double endPercent))
                    return;
                if (!g.UserInput.TryGet(DownscaleAfterSkip, out bool downscaleAfterSkip))
                    return;
                if (!g.UserInput.TryGet(DownscaleMethod, out string downscaleMethod))
                    return;
                if (!g.UserInput.TryGet(UpscaleMethod, out string upscaleMethod))
                    return;
                // Generally always check that your parameter exists before doing anything (so you don't infect unrelated generations unless the user wants your feature running)
                // Create the node we want...
                string deepShrinkNode = g.CreateNode(
                    "PatchModelAddDownscale",
                    new JObject()
                    {
                        // And configure all the inputs to that node...
                        ["model"] = g.LoadingModel,
                        ["block_number"] = blockNumber,
                        ["downscale_factor"] = downscaleFactor,
                        ["start_percent"] = startPercent,
                        ["end_percent"] = endPercent,
                        ["downscale_after_skip"] = downscaleAfterSkip,
                        ["downscale_method"] = downscaleMethod,
                        ["upscale_method"] = upscaleMethod,
                    }
                );
                g.LoadingModel = [deepShrinkNode, 0];
                g.FinalModel = [deepShrinkNode, 0];
            },
            -12
        );
    }
}
