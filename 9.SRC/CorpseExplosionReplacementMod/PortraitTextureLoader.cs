using System;
using System.IO;
using System.Reflection;
using Godot;
using MegaCrit.Sts2.Core.Logging;

namespace CorpseExplosionReplacementMod;

/// <summary>
/// 加载尸爆术卡图。
/// pck 纹理可能被游戏资源清理机制（AssetCache.UnloadMissedCacheAssets）Dispose，
/// 因此每次获取都做有效性自检：失效则重新加载 pck，仍无效则回退 DLL 内嵌 PNG
/// 生成的运行时 ImageTexture（不进 Godot ResourceCache，永不被游戏清理）。
/// </summary>
public static class PortraitTextureLoader
{
    private static Texture2D? _pckTexture;
    private static Texture2D? _embeddedTexture;

    public static Texture2D Get()
    {
        // 1) pck 纹理仍有效则直接返回（GodotObject.IsInstanceValid(null) 为 false，无需判空）
        if (GodotObject.IsInstanceValid(_pckTexture))
        {
            return _pckTexture;
        }

        // 2) 重新尝试 pck：若 ResourceCache 已被毒化，Load 仍可能返回失效实例，IsInstanceValid 会拦住
        try
        {
            _pckTexture = ResourceLoader.Load<Texture2D>(ModEntry.PortraitPng);
            if (GodotObject.IsInstanceValid(_pckTexture))
            {
                return _pckTexture;
            }

            _pckTexture = null;
            Log.Warn($"{ModEntry.ModId}: pck portrait is invalid after reload, falling back to embedded texture");
        }
        catch (Exception e)
        {
            Log.Warn($"{ModEntry.ModId}: pck portrait load error: {e.Message}");
            _pckTexture = null;
        }

        // 3) DLL 内嵌资源兜底：运行时 ImageTexture 永不失效
        return GetEmbedded();
    }

    private static Texture2D GetEmbedded()
    {
        if (GodotObject.IsInstanceValid(_embeddedTexture))
        {
            return _embeddedTexture;
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
                    _embeddedTexture = ImageTexture.CreateFromImage(image);
                }
            }
        }
        catch (Exception e)
        {
            Log.Error($"{ModEntry.ModId}: embedded portrait fallback failed: {e}");
        }

        return _embeddedTexture!;
    }
}
