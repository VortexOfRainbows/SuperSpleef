using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public ControlDown Control = new ControlDown(); //Stores player input for the current frame
    public ControlDown LastControl = new ControlDown(); //Record player input from the previous frame
    public struct ControlDown
    {
        public bool LeftClick;
        public bool RightClick;
        public bool Left;
        public bool Right;
        public bool Forward;
        public bool Back;
        public bool Jump;

        public bool Hotkey1; //Structs cannot store an array without a reference. Doing these seperately is sadly easier than using an array, as the system im using would need to establish a reference to a new array every frame.
        public bool Hotkey2;
        public bool Hotkey3;
        public bool Hotkey4;
        public bool Hotkey5;
        public bool Hotkey6;
        public bool Hotkey7;
        public bool Hotkey8;
        public bool Hotkey9;
        public bool Hotkey0;
        public float XAxis;
        public float YAxis;
        public float ScrollDelta;
        public ControlDown(bool defaultState = false)
        {
            LeftClick = Left = Right = Forward = Back = Jump = defaultState;
            RightClick = defaultState;
            Hotkey1 = Hotkey2 = Hotkey3 = Hotkey4 = Hotkey5 = Hotkey6 = Hotkey7 = Hotkey8 = Hotkey9 = Hotkey0 = defaultState;
            XAxis = YAxis = ScrollDelta = 0f;
        }
    }
    public void UpdateKey(bool AssociatedInput, bool LastControl, ref bool ControlToUpdate)
    {
        if (AssociatedInput)
        {
            ControlToUpdate = true;
        }
        else
        {
            if (LastControl)
                ControlToUpdate = false;
        }
    }
    public void OnUpdate()
    {
        Control.ScrollDelta = Input.mouseScrollDelta.y;
        Control.XAxis = Input.GetAxisRaw("Mouse X");
        Control.YAxis = Input.GetAxisRaw("Mouse Y");
        UpdateKey(Input.GetKey(KeyCode.A), LastControl.Left, ref Control.Left);
        UpdateKey(Input.GetKey(KeyCode.D), LastControl.Right, ref Control.Right);
        UpdateKey(Input.GetKey(KeyCode.W), LastControl.Forward, ref Control.Forward);
        UpdateKey(Input.GetKey(KeyCode.S), LastControl.Back, ref Control.Back);

        UpdateKey(Input.GetKey(KeyCode.Space), LastControl.Jump, ref Control.Jump);
        //UpdateKey(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift), LastControl.Shift, ref Control.Shift);
        UpdateKey(Input.GetMouseButton(0), LastControl.LeftClick, ref Control.LeftClick);
        UpdateKey(Input.GetMouseButton(1), LastControl.RightClick, ref Control.RightClick);
        UpdateKey(Input.GetKey(KeyCode.Alpha1), LastControl.Hotkey1, ref Control.Hotkey1);
        UpdateKey(Input.GetKey(KeyCode.Alpha2), LastControl.Hotkey2, ref Control.Hotkey2);
        UpdateKey(Input.GetKey(KeyCode.Alpha3), LastControl.Hotkey3, ref Control.Hotkey3);
        UpdateKey(Input.GetKey(KeyCode.Alpha4), LastControl.Hotkey4, ref Control.Hotkey4);
        UpdateKey(Input.GetKey(KeyCode.Alpha5), LastControl.Hotkey5, ref Control.Hotkey5);
        UpdateKey(Input.GetKey(KeyCode.Alpha6), LastControl.Hotkey6, ref Control.Hotkey6);
        UpdateKey(Input.GetKey(KeyCode.Alpha7), LastControl.Hotkey7, ref Control.Hotkey7);
        UpdateKey(Input.GetKey(KeyCode.Alpha8), LastControl.Hotkey8, ref Control.Hotkey8);
        UpdateKey(Input.GetKey(KeyCode.Alpha9), LastControl.Hotkey9, ref Control.Hotkey9);
        UpdateKey(Input.GetKey(KeyCode.Alpha0), LastControl.Hotkey0, ref Control.Hotkey0);
    }
    public void OnFixedUpdate()
    {
        LastControl = Control; 
    }
}
