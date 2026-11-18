# 自写脚本索引

| 文件名 | 放入时间 | 用途 | 依赖 | 备注 |
|---|---|---|---|---|
| `verify-version.ps1` | 2026-11-18 19:26 | 校验构建产物 manifest 版本、程序集版本、`min_game_version`、`CHANGELOG.md` 与 `7.ChangeLog/` 是否一致 | PowerShell 5.1+、.NET 9 产物 | 默认检查 `10.OUTPUT/01-构建产物/CorpseExplosionReplacementMod v8（0.111.1-0.113.1兼容，打包后mod）`；可用 `-ArtifactDir` 指定其他目录 |
