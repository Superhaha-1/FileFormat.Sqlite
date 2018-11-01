using FluentValidation;

namespace FileFormat.Sqlite.Demo.Validators
{
    public sealed class NameValidator : AbstractValidator<string>
    {
        public static NameValidator Instance { get; } = new NameValidator();

        private NameValidator()
        {
            RuleFor(name => name).NotEmpty().MaximumLength(20).WithName("名称");
        }
    }
}
