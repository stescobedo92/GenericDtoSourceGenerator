namespace GenericDto.Analyzers.Constants;

internal static class GeneratorConstants
{
    public const string GenericDtoCoreAttributeFullName = "GenericDto.Core.Attributes.GenericDtoAttribute";
    public const string DtoPropertyAttributeFullName = "GenericDto.Core.Attributes.DtoPropertyAttribute";
    public const string DtoIgnoreAttributeFullName = "GenericDto.Core.Attributes.DtoIgnoreAttribute";

    public const string DtoSuffix = "Dto";
    public const string DtoNamespaceSuffix = ".Dto";

    public static readonly string[] DefaultUsings = new[]
    {
        "System",
        "System.Collections.Generic",
        "System.ComponentModel.DataAnnotations"
    };
}
