using System;
using System.IO;
using System.Reflection;
using Godot;
using MegaCrit.Sts2.Core.Logging;

namespace CorpseExplosionReplacementMod;

/// <summary>
/// 加载尸爆 Buff 图标（STS1 cExplosion）。优先从模组 pck 读取；失败则回退 DLL 内嵌资源。
/// </summary>
public static class PowerIconTextureLoader
{
    private static Texture2D? _icon64;
    private static Texture2D? _icon256;
    private static bool _initialized;

    public static Texture2D Get64()
    {
        EnsureLoaded();
        return _icon64!;
    }

    public static Texture2D Get256()
    {
        EnsureLoaded();
        return _icon256!;
    }

    private static void EnsureLoaded()
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;
        _icon64 = Load(ModEntry.PowerIcon64Path, "CorpseExplosionReplacementMod.assets.corpse_explosion_power_64.png");
        _icon256 = Load(ModEntry.PowerIcon256Path, "CorpseExplosionReplacementMod.assets.corpse_explosion_power_256.png");
    }

    private static Texture2D? Load(string resPath, string embeddedName)
    {
        try
        {
            Texture2D? texture = ResourceLoader.Load<Texture2D>(resPath);
            if (texture != null)
            {
                return texture;
            }
        }
        catch (Exception e)
        {
            Log.Warn($"{ModEntry.ModId}: power icon load error ({resPath}): {e.Message}");
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
                    return ImageTexture.CreateFromImage(image);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error($"{ModEntry.ModId}: embedded power icon fallback failed ({embeddedName}): {e}");
        }

        return null;
    }
}
