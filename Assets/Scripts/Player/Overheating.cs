using Auxiliars;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Overheating : MonoBehaviour {

	private const float MAX_HEAT = 100f;
	private readonly Color COLD_COLOR = Color.cyan;
	private readonly Color OVERHEAT_COLOR = Color.red;

	[SerializeField]
	private float heat;
	[SerializeField]
	private float cooldownSpeed;
	[SerializeField]
	private float naturalCooldownSpeed;
	[SerializeField]
	private float heatUpSpeed;
	[SerializeField]
	private float timeToDecrease;
	private SpartanTimer reloadTimer;
	private SpartanTimer heatDecreaseTimer;
	private PlayerMovement movRef;
	[SerializeField]
	private Image heatIndicator;

	public bool IsOverheated => this.heat >= MAX_HEAT;
	private float FillAmount => this.heat / MAX_HEAT;

	private void Start() {
		this.reloadTimer = new SpartanTimer(TimeMode.Framed);
		this.heatDecreaseTimer = new SpartanTimer(TimeMode.Framed);
		this.movRef = GetComponent<PlayerMovement>();
	}

	private void Update() {
		//Debug.Log($"Heat capacity: {this.heat} / {MAX_HEAT}");
		this.HandleOverheat();
		this.CooldownNaturally();
		this.ChangeColorByHeat();
		this.heat = Mathf.Clamp(this.heat, 0f, MAX_HEAT);
	}

	private void CooldownNaturally() {
		float currDecreaseTime = this.heatDecreaseTimer.GetCurrentTime(TimeScaleMode.Seconds);
		Debug.Log($"Curr dec time {currDecreaseTime}");
		bool canCooldown = this.heat > 0 && !this.movRef.MovingUpwards && !this.IsOverheated && currDecreaseTime >= this.timeToDecrease;
		if (canCooldown) {
			this.heat -= Time.deltaTime * this.naturalCooldownSpeed;
			//Round down now that we're close enough
			this.heat = this.heat - 5f <= 0f ? 0f : this.heat;
			Debug.Log($"Final: {this.heat}");
		}
	}

	private void HandleOverheat() {
		if (this.IsOverheated && !this.reloadTimer.Started) {
			//Start the reload timer
			this.reloadTimer.Reset();
		}
		float currSeconds = this.reloadTimer.GetCurrentTime(TimeScaleMode.Seconds);
		if (currSeconds >= this.cooldownSpeed) {
			this.reloadTimer.Stop();
			this.heat = 0;
		}
	}

	private void ChangeColorByHeat() {
		this.heatIndicator.fillAmount = this.FillAmount;
		this.heatIndicator.color = Color.Lerp(this.COLD_COLOR, this.OVERHEAT_COLOR, this.FillAmount);
	}

	public void IncreaseHeat() {
		this.heat += Time.deltaTime * heatUpSpeed;
		this.heatDecreaseTimer.Reset();
	}
}
