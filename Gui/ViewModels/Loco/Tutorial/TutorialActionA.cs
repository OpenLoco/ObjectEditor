namespace Gui.ViewModels.Loco.Tutorial;

public record TutorialActionA(KeyModifier KeyModifier, uint16_t MouseX, uint16_t MouseY, MouseButton MouseButton) : ITutorialAction;
