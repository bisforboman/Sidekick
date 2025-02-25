using System.ComponentModel.DataAnnotations;

namespace Sidekick.Common.Blazor.Validation;

public class KeybindAlreadyInUseValidator(HashSet<string> keybindsInUse) : ValidationAttribute
{
    private readonly HashSet<string> keybindsInUse = keybindsInUse;

    public override string FormatErrorMessage(string keybind)
        => $"The keybind {keybind} is already in use!";

    public override bool IsValid(object? value) => 
        value is string s && keybindsInUse.Contains(s);
}