using System.ComponentModel.DataAnnotations;

namespace DeviceRepoAspNetCore.Models.RestApi;

[AttributeUsage(AttributeTargets.Property)]
public class AllowedDeviceMessageTypesAttribute(params DeviceMessageType[] allowedTypes) : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        // ReSharper disable once InvertIf
        if (value is DeviceMessageType msgType)
        {
            if (Array.IndexOf(allowedTypes, msgType) < 0)
            {
                return new ValidationResult($"DeviceMessageType must be one of [{string.Join(", ", allowedTypes)}].");
            }
        }
        return ValidationResult.Success!;
    }
}