using System;

namespace StartinhsMart.CoreService.Shared;

public class GetFileInput
{
    public string DownloadToken { get; set; } = null!;

    public Guid FileId { get; set; }
}

public class GetImageInput
{
    public Guid ImageId { get; set; }
}