using System;
using System.IO;
using System.Reflection;
using Godot;
using MegaCrit.Sts2.Core.Logging;

namespace CorpseExplosionReplacementMod;

/// <summary>
/// 加载尸爆术卡图。优先从模组 pck 读取；若失败则回退到 DLL 内嵌的 PNG 资源。
/// </summary>
public static class PortraitTextureLoader
{
    private static Texture2D? _texture;
    private static bool _initialized;

    public static Texture2D Get()
    {
        if (_initialized)
        {
            return _texture!;
        }

        _initialized = true;

        try
        {
            _texture = ResourceLoader.Load<Texture2D>(ModEntry.PortraitPng);
            if (_texture != null)
            {
                return _texture!;
            }

            Log.Warn($"{ModEntry.ModId}: pck portrait not loadable, falling back to embedded texture");
        }
        catch (Exception e)
        {
            Log.Warn($"{ModEntry.ModId}: pck portrait load error: {e.Message}");
        }

        try
        {
            using Stream? stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("CorpseExplosionReplacementMod.assets.corpse_explosion_big.png");
            if (stream != null)
            {
                using var ms = new MemoryStream();
                stream.CopyTo(ms);
                var image = new Image();
                if (image.LoadPngFromBuffer(ms.ToArray()) == Error.Ok)
                {
                    _texture = ImageTexture.CreateFromImage(image);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error($"{ModEntry.ModId}: embedded portrait fallback failed: {e}");
        }

        return _texture!;
    }
}
