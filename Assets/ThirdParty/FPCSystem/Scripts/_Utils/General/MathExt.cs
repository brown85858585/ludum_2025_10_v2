
using UnityEngine;
using System;

public class MathExt{
	
	// Makes a lerp based on a ratio (witch is based in a time range).
	// instead of doing it based on speed.
	//
	// the ratio = 1.0/moveTime;
	// given a index 'i' if I increase it by (ratio * deltaTime), i'll get
	// that this index is going to increase to ! in the given time 'movetime'.
	
	public static float timedLerpRatio = 0;
	public static float timedLerpRatio2 = 0;
	
	public static float TimedLerp (ref float i, float origin, float dest, float time) {
		if(timedLerpRatio == 0)
			timedLerpRatio = 1 / time;
		
		float val = dest;
		if (i < 1.0f){
			i += Time.deltaTime * timedLerpRatio;
			if(Mathf.Abs(origin - dest) > 0.01f)
				val = Mathf.Lerp(origin, dest, i);
			else{
				i = 1;
				timedLerpRatio = 0;
			}
		}
		return val;
	}
	
	// Another TimedLerp function, just the same than the regular TimeLerp.
	// Is used to make the camera move at the same time doing to diffrent things,
	// for example, when climbing a ledge, the camera can do down/up and perform a roll at the same time
	// using two timelerped funcions at once.
	public static float TimedLerp2 (ref float i, float origin, float dest, float time) {
		if(timedLerpRatio2 == 0)
			timedLerpRatio2 = 1 / time;
		
		float val = dest;
		if (i < 1.0f){
			i += Time.deltaTime * timedLerpRatio2;
			if(Mathf.Abs(origin - dest) > 0.01f)
				val = Mathf.Lerp(origin, dest, i);
			else{
				i = 1;
				timedLerpRatio2 = 0;
			}
		}
		return val;
	}
	
	// Generates a BOB movement (for the camera or maybe weapons) using a sinus function.
	// It checks if totalAxes are presed to return Zero (no Bob at all)
	public static float BobGenerator (ref float timer, float val, float totalAxes, float amount, float speed) {
		if (totalAxes == 0){
			timer = 0.0f;
			val = 0;
		}
		else{
			val = Mathf.Sin(timer) * amount * totalAxes;
			timer += speed * Time.deltaTime;
			if (timer > Mathf.PI * 2)
				timer -= Mathf.PI * 2;
		}
		return val;
	}
	
	// Generates a BOB movement that moves at double speed.
	// It is twice quickly than the normal camera bob, thats why the sinus is multiplied by 2.
	// It can be used if you want to crate a weapon that bobs twice quickly, normally in the Y axis.
	public static float BobGeneratorX2 (ref float timer, float val, float totalAxes, float amount, float speed, bool updateTimer) {
		if (totalAxes == 0){
			timer = 0.0f;
			val = 0;
		}
		else{
			val = Mathf.Sin(timer*2) * amount * totalAxes; // Twice quickly function is done right here.
			if(updateTimer){
				timer += speed * Time.deltaTime;
				if (timer > Mathf.PI * 2)
					timer -= Mathf.PI * 2;
			}
		}
		return val;
	}
}