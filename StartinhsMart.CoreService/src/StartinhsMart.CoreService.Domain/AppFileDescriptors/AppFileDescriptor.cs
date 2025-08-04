using System;
using Volo.Abp.Domain.Entities;

namespace StartinhsMart.CoreService.AppFileDescriptors;

public class AppFileDescriptor : AggregateRoot<Guid>
{
    public string Name { get; set; } = null!;

    public string MimeType { get; set; } = null!;

    private AppFileDescriptor()
    {

    }

    public AppFileDescriptor(Guid id, string name, string mimeType)
    {
        Id = id;
        Name = name;
        MimeType = mimeType;
    }
}