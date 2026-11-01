using System;
using System.IO;
using System.Reflection;
using Godot;
using MegaCrit.Sts2.Core.Logging;

namespace CorpseExplosionReplacementMod;

/// <summary>
/// 加载尸爆 Buff 图标（STS1 cExplosion）。
/// 与 PortraitTextureLoader 同策略：pck 纹理每次做有效性自检，
/// 失效则重载 pck，仍无效则回退 DLL 内嵌 PNG 生成的运行时 ImageTexture。
/// </summary>
public static class PowerIconTextureLoader
{
    private static Texture2D? _pck64;
    private static Texture2D? _pck256;
    private static Texture2D? _embedded64;
    private static Texture2D? _embedded256;

    public static Texture2D Get64()
    {
        return Load(
            ref _pck64,
            ModEntry.PowerIcon64Path,
            "CorpseExplosionReplacementMod.assets.corpse_explosion_power_64.png",
            ref _embedded64);
    }

    public static Texture2D Get256()
    {
        return Load(
            ref _pck256,
            ModEntry.PowerIcon256Path,
            "CorpseExplosionReplacementMod.assets.corpse_explosion_power_256.png",
            ref _embedded256);
    }

    private static Texture2D Load(
        ref Texture2D? pckTexture,
        string resPath,
        string embeddedName,
        ref Texture2D? embeddedTexture)
    {
        // 1) pck 纹理仍有效则直接返回
        if (GodotObject.IsInstanceValid(pckTexture))
        {
            return pckTexture;
        }

        // 2) 重新尝试 pck
        try
        {
            pckTexture = ResourceLoader.Load<Texture2D>(resPath);
            if (GodotObject.IsInstanceValid(pckTexture))
            {
                return pckTexture;
            }

            pckTexture = null;
            Log.Warn($"{ModEntry.ModId}: power icon invalid after reload ({resPath}), falling back to embedded texture");
        }
        catch (Exception e)
        {
            Log.Warn($"{ModEntry.ModId}: power icon load error ({resPath}): {e.Message}");
            pckTexture = null;
        }

        // 3) DLL 内嵌资源兜底：运行时 ImageTexture 永不失效
        if (GodotObject.IsInstanceValid(embeddedTexture))
        {
            return embeddedTexture;
        }

        try
        {
            using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedName);
            if (stream != null)
            {
                using var ms = new MemoryStream();
                stream.CopyTo(ms);
                var image = new Image();
                if (image.LoadPngFromBuffer(ms.ToArray()) == Error.Ok)
                {
                    embeddedTexture = ImageTexture.CreateFromImage(image);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error($"{ModEntry.ModId}: embedded power icon fallback failed ({embeddedName}): {e}");
        }

        return embeddedTexture!;
    }
}
