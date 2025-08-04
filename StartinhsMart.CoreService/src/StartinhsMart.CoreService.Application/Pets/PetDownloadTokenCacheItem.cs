using System;

namespace StartinhsMart.CoreService.Pets;

[Serializable]
public class PetDownloadTokenCacheItem
{
    public string Token { get; set; } = null!;
}