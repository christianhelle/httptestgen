namespace HttpTestGen.Core;

public class HttpFileCondition
{
    public string RequestName { get; set; } = null!;
    public string Field { get; set; } = null!;
    public string ExpectedValue { get; set; } = null!;
    public bool Negated { get; set; }
}
