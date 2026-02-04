using System;

namespace StartinhsMart.CoreService.Categories;

[Serializable]
public class CategoryDownloadTokenCacheItem
{
    public string Token { get; set; } = null!;
}
