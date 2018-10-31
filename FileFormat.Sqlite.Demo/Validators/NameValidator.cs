using FluentValidation;

namespace FileFormat.Sqlite.Demo.Validators
{
    public sealed class NameValidator : AbstractValidator<string>
    {
        public static NameValidator Instance { get; } = new NameValidator();

        private NameValidator()
        {
            RuleFor(name => name).NotNull();
            RuleFor(name => name).MaximumLength(20);
        }
    }
}
