using FluentValidation;
using System.IO;
using System.Linq;

namespace FileFormat.Sqlite.Demo.Validators
{
    public sealed class NameValidator : AbstractValidator<string>
    {
        public static NameValidator Instance { get; } = new NameValidator();

        private NameValidator()
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            RuleFor(name => name).NotEmpty().WithMessage("名称不能为空");
            RuleFor(name => name).MaximumLength(30).WithMessage("名称的最大长度为30");
            foreach (var c in FileFormatHelper.InvalidPathChars)
            {
                RuleFor(name => name).Must(name => !name.Contains(c)).WithMessage($"名称中不能含有'{c}'");
            }
        }
    }
}
