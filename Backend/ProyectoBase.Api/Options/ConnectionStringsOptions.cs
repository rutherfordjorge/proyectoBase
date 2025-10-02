namespace ProyectoBase.Api.Options;

public class ConnectionStringsOptions
{
    public const string SectionName = "ConnectionStrings";
    public const string DefaultConnectionName = "DefaultConnection";

    public string DefaultConnection { get; set; } = string.Empty;
}
