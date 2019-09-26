using UnityEngine;
using System.Collections;

public class CarController : MonoBehaviour {
	[System.Serializable]
	public class Wheel {
		public Transform transform;
		public WheelCollider collider;
		public float rotationValue;
		public float skidmarkTime;
		public int lastSkidmark;
		public ParticleSystem psSmoke;
		public bool isFront;
	}
	
	[System.Serializable]
	public class Engine {
		public float[] gearRatios; // {15, 10, 7.5, 5, 3}
		public int currentGear = 0; 
		public float force; // 35
		public float maxRPM; // 5000
		public float minRPM; // 1000
		public float RPM = 0;
	}
	
	[System.Serializable]
	public class BreakSystem {
		public float breakForce; //50
		public float handBreakForce; //50
	}
	
	public class Inputer {
		public bool handbreak;
		public float vertical;
		public float horizontal;
		
		public void SetInput() {
			handbreak = Input.GetButton("Jump");
			vertical = Input.GetAxis("Vertical");
			horizontal = Input.GetAxis("Horizontal");
		}
	}
	
	class Speedometer {
		Vector3 lastPos;
		Vector3 currentPos;
		public float KPH;
		public float MPH;
		
		public void CalcSpeed(Vector3 currentPosition) {
			lastPos = currentPos;
			currentPos = currentPosition;
			KPH = (Vector3.Distance(lastPos, currentPos) / Time.deltaTime) * 3.6f;
			MPH = KPH * 1.609344f;
		}
	}
	
	public Transform steeringWheel;
	
	public Wheel wheelFL, wheelFR, wheelRL, wheelRR;
	public Engine engine;
	public BreakSystem breakSystem;
	public Light[] rearLights;
	
	Speedometer speedometer;
	Inputer inputer;
	SkidMarker skidmarker;
	
	
	void Awake () {
		GetComponent<Rigidbody>().centerOfMass = transform.Find("CenterOfMass").transform.localPosition;
		speedometer = new Speedometer();
		inputer = new Inputer();
		skidmarker = transform.Find("Skidmarker").GetComponent<SkidMarker>();
		transform.Find("Skidmarker").parent = null;
	}
	
	void Update () {
		inputer.SetInput();
		
		speedometer.CalcSpeed(transform.position);
		engine.RPM = CalcEngineRPM(engine.currentGear);
		ShiftGears();

		GetComponent<AudioSource>().pitch = Mathf.Abs(engine.RPM / engine.maxRPM) + 0.25f;

		if (GetComponent<AudioSource>().pitch > 2) {
			GetComponent<AudioSource>().pitch = 2;
		}
		
		GetComponent<AudioSource>().pitch *= Time.timeScale;
			
		// Rotate steering wheel
		if(steeringWheel)
			steeringWheel.localRotation = Quaternion.Lerp(steeringWheel.localRotation, Quaternion.Euler(steeringWheel.localRotation.eulerAngles.x, 0, -30 * Input.GetAxis("Horizontal")), 15 * Time.deltaTime);
		
		wheelFL.collider.steerAngle = Mathf.DeltaAngle(0, 30 * inputer.horizontal);
		wheelFR.collider.steerAngle = Mathf.DeltaAngle(0, 30 * inputer.horizontal);
		
		WheelActions(wheelFL);
		WheelActions(wheelFR);
		WheelActions(wheelRL);
		WheelActions(wheelRR);
		
		// Break lights
		if (wheelFL.collider.brakeTorque > 0 || wheelFR.collider.brakeTorque > 0) 
			ColorRearLights(Color.red);
		else if (inputer.vertical < 0 && engine.RPM < 0) 
			ColorRearLights(Color.white);
		else  
			ColorRearLights(Color.black);
	}
	
	float CalcEngineRPM(int gearIndex) {
		return (wheelRL.collider.rpm + wheelRR.collider.rpm) * 0.5f * engine.gearRatios[gearIndex];
	}
	
	void ShiftGears() {
		if (engine.RPM >= engine.maxRPM) {
			for (int i=0; i<engine.gearRatios.Length; i++) {
				if (CalcEngineRPM(i) < engine.maxRPM) {
					engine.currentGear = i;
					return;
				}
			}
		}
		if (engine.RPM <= engine.minRPM) {
			for (int i=engine.gearRatios.Length-1; i>= 0; i--) {
				if (CalcEngineRPM(i) > engine.minRPM ) {
					engine.currentGear = i;
					return;
				}
			}
		}
	}
	
	void WheelActions(Wheel wheel) {
		// Calc engine force on wheel
		float engineTorque = 0;
		
		if (!wheel.isFront && Mathf.Abs(engine.RPM) < engine.maxRPM) {
			if (inputer.vertical > 0 && engine.RPM >= 0) 
				engineTorque += engine.force * engine.gearRatios[engine.currentGear] /((float)(engine.gearRatios.Length - engine.currentGear)) * inputer.vertical;
			else if (inputer.vertical < 0 && engine.RPM <= 0) 
				engineTorque += engine.force * engine.gearRatios[engine.currentGear] /((float)(engine.gearRatios.Length - engine.currentGear)) * inputer.vertical;
		}
		
		wheel.collider.motorTorque = engineTorque;
		
		// Calc break force on wheel
		float breakTorque = 0;
		
		if (wheel.isFront && (inputer.vertical < 0 && engine.RPM > 0 || inputer.vertical > 0 && engine.RPM < 0)) {
			if (engine.RPM > 0)
				breakTorque += -breakSystem.breakForce * inputer.vertical;
			else 
				breakTorque += breakSystem.breakForce * inputer.vertical;
		}
		else if (inputer.handbreak) {
			breakTorque += breakSystem.handBreakForce;
		}
		
		wheel.collider.brakeTorque = breakTorque;
		
		// Set wheel always on ground if suspense allows
		RaycastHit hit;
		Vector3 colliderCenterPoint = wheel.collider.transform.TransformPoint(wheel.collider.center);
	
		if (Physics.Raycast(colliderCenterPoint, -wheel.collider.transform.up, out hit, wheel.collider.suspensionDistance + wheel.collider.radius)) {
			wheel.transform.position = hit.point + (wheel.collider.transform.up * wheel.collider.radius);
		}
		else {
			wheel.transform.position = colliderCenterPoint - (wheel.collider.transform.up * wheel.collider.suspensionDistance);
		}
		
		// Rotate wheel 
		wheel.transform.rotation = wheel.collider.transform.rotation * Quaternion.Euler(wheel.rotationValue, wheel.collider.steerAngle, 0);
		wheel.rotationValue += wheel.collider.rpm * (360f/60f) * Time.deltaTime;

		WheelHit correspondingGroundHit;
		wheel.collider.GetGroundHit(out correspondingGroundHit);
		
		float slipRate = Mathf.Abs(correspondingGroundHit.sidewaysSlip);
		
		// Emit smoke if available and slipping
		if (wheel.psSmoke != null) {
			if (slipRate > 2) {
				if (!wheel.psSmoke.isPlaying) {
					wheel.psSmoke.Play();
				}
			}
			else {
				if (wheel.psSmoke.isPlaying) {
					wheel.psSmoke.Stop();
				}
			}
		}
			
		// Draw skidmark
		if (slipRate > 2) {
			if(wheel.skidmarkTime < 0.02f && wheel.lastSkidmark != -1)	{
				wheel.skidmarkTime += Time.deltaTime;
			}
			else {
				wheel.lastSkidmark = skidmarker.AddSkidMark(correspondingGroundHit.point, correspondingGroundHit.normal, Mathf.Abs(correspondingGroundHit.sidewaysSlip) * 0.025f, wheel.lastSkidmark);
			}
		}	
	}
	
	void OnCollisionEnter(Collision col) {
		Vector3 correctHitPoint;
		//correctHitPoint = (col.contacts[0].point);
		correctHitPoint = (col.contacts[0].thisCollider.ClosestPointOnBounds(col.transform.position - transform.position) * 1);
		GetComponent<Deformer>().DeformMesh(correctHitPoint, -col.relativeVelocity.magnitude * 0.01f, 20f);
	}
	
	void ColorRearLights(Color color) {
		for (int i=0; i<rearLights.Length; i++) {
			rearLights[i].color = color;
		}
	}
}