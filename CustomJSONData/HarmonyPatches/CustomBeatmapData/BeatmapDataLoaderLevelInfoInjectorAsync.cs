using System.Threading.Tasks;
using CustomJSONData.CustomBeatmap;
using HarmonyLib;

namespace CustomJSONData.HarmonyPatches;

[HarmonyPatch(typeof(BeatmapDataLoader), nameof(BeatmapDataLoader.LoadBeatmapDataAsync))]
internal static class BeatmapDataLoaderLevelInfoInjectorAsync
{
    [HarmonyPostfix]
    private static void InjectCustomData(
        IBeatmapLevelData beatmapLevelData,
        BeatmapKey beatmapKey,
        ref Task<IReadonlyBeatmapData?> __result)
    {
        __result = InjectAfterLoad(__result, beatmapLevelData, beatmapKey);
    }

    private static async Task<IReadonlyBeatmapData?> InjectAfterLoad(
        Task<IReadonlyBeatmapData?> load,
        IBeatmapLevelData beatmapLevelData,
        BeatmapKey beatmapKey)
    {
        IReadonlyBeatmapData? result = await load;
        if (beatmapLevelData is CustomFileBeatmapLevelData file && result is CustomBeatmapData data)
        {
            data.levelCustomData = file.customData;
            if (file.GetDifficultyBeatmap(beatmapKey) is CustomFileDifficultyBeatmap difficulty)
                data.beatmapCustomData = difficulty.customData;
        }
        return result;
    }
}
