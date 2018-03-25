using UnityEngine;
using System.Collections;
using Valve.VR;

public class EventManager : MonoBehaviour 
{

  public delegate void TriggerDown(GameObject t);
  public static event TriggerDown OnTriggerDown;

  public delegate void TriggerUp(GameObject t);
  public static event TriggerUp OnTriggerUp;

  public delegate void TriggerStay(GameObject t);
  public static event TriggerStay StayTrigger;

  public delegate void GripDown(GameObject t);
  public static event GripDown OnGripDown;

  public delegate void GripUp(GameObject t);
  public static event GripUp OnGripUp;

  public delegate void GripStay(GameObject t);
  public static event GripStay StayGrip;

  public delegate void PadDown(GameObject t);
  public static event PadDown OnPadDown;

  public delegate void PadUp(GameObject t);
  public static event PadUp OnPadUp;

  public delegate void PadStay(GameObject t);
  public static event PadStay StayPad;


  public GameObject handL;
  public GameObject handR;

  SteamVR_TrackedObject trackedObjL;
  SteamVR_TrackedObject trackedObjR;

  void Start(){

    trackedObjL = handL.GetComponent<SteamVR_TrackedObject>();
    trackedObjR = handR.GetComponent<SteamVR_TrackedObject>();

  }
  
  void FixedUpdate(){

    getTrigger( handL , trackedObjL );
    getTrigger( handR , trackedObjR );

    getGripTrigger( handL , trackedObjL );
    getGripTrigger( handR , trackedObjR );

    getPadTrigger( handL , trackedObjL );
    getPadTrigger( handR , trackedObjR );

  }

  void getTrigger( GameObject go , SteamVR_TrackedObject tObj ){

    if((int) tObj.index < 0 ){ return; }
    var device = SteamVR_Controller.Input((int)tObj.index);

    if ( device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger)){
      device.TriggerHapticPulse(1000);
      if(OnTriggerDown != null) OnTriggerDown(go);
    }

    if ( device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger)){
      if(OnTriggerUp != null) OnTriggerUp(go);
    }


    if ( device.GetTouch(SteamVR_Controller.ButtonMask.Trigger)){
      if(StayTrigger != null) StayTrigger(go);
    }

  }
//SteamVR_Controller.Input(deviceIndex).TriggerHapticPulse(100);
  void getGripTrigger( GameObject go , SteamVR_TrackedObject tObj ){

       if((int) tObj.index < 0 ){ return; }
    var device = SteamVR_Controller.Input((int)tObj.index);

    if ( device.GetPressDown(SteamVR_Controller.ButtonMask.Grip)){
      if(OnGripDown != null) OnGripDown(go);
    }

    if ( device.GetPressUp(SteamVR_Controller.ButtonMask.Grip)){
      if(OnGripUp != null) OnGripUp(go);
    }


    if ( device.GetPress(SteamVR_Controller.ButtonMask.Grip)){
      if(StayGrip != null) StayGrip(go);
    }



  }

  void getPadTrigger( GameObject go , SteamVR_TrackedObject tObj ){

       if((int) tObj.index < 0 ){ return; }
    var device = SteamVR_Controller.Input((int)tObj.index);

    if ( device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad)){
      if(OnPadDown != null) OnPadDown(go);
    }

    if ( device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad)){
      if(OnPadUp != null) OnPadUp(go);
    }


    if ( device.GetPress(SteamVR_Controller.ButtonMask.Touchpad)){
      if(StayPad != null) StayPad(go);
    }


  }


}