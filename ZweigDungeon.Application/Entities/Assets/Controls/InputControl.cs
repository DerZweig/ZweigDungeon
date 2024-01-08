using ZweigDungeon.Application.Entities.Assets.Constants;

namespace ZweigDungeon.Application.Entities.Assets.Controls;

public class InputControl : BasicControl
{
	public InputType Type         { get; set; } = InputType.String;
	public string?   DefaultValue { get; set; }
	public int?      MinimumValue { get; set; }
	public int?      MaximumValue { get; set; }
	public int?      MaximumLength { get; set; }
}