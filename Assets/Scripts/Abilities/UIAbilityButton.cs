using UnityEngine;
using UnityEngine.UI;
public class UIAbilityButton : MonoBehaviour {
	private IAbility Ability => PlayerController.instance.Ability;
	private Button _button;

	private bool IsRecharged() => Ability.IsRecharged();
	public void TriggerAbility() => Ability.TriggerAction();

	private void Start() => _button = GetComponent<Button>();
	private void Update() => _button.interactable = IsRecharged();
}