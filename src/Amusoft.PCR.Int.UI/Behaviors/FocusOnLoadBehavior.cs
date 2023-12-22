namespace Amusoft.PCR.Int.UI.Behaviors;

public class FocusOnLoadBehavior : Behavior<VisualElement>
{
	protected override void OnAttachedTo(VisualElement bindable)
	{
		bindable.Loaded += BindableOnLoaded;

		base.OnAttachedTo(bindable);
	}

	private static void BindableOnLoaded(object? sender, EventArgs e)
	{
		if (sender is VisualElement ve)
		{
			ve.Focus();
		}
	}

	protected override void OnDetachingFrom(VisualElement bindable)
	{
		bindable.Loaded -= BindableOnLoaded;
		base.OnDetachingFrom(bindable);
	}
}