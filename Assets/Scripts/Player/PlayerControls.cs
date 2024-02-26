using UnityEngine;
using UnityEngine.UIElements;

public class PlayerControls : MonoBehaviour ///Team members that contributed to this script: Ian Bunnell
{
    private GamepadControls GamepadControls;
    public bool UsingGamepad = false;
    public ControlDown Control = new ControlDown(); //Stores player input for the current frame
    public ControlDown LastControl = new ControlDown(); //Record player input from the previous frame
    private void Awake()
    {
        GamepadControls = new GamepadControls();
        GamepadControls.Enable();
    }
    public struct ControlDown
    {
        public bool LeftClick;
        public bool RightClick;
        public bool Left;
        public bool Right;
        public bool Forward;
        public bool Back;
        public bool Jump;
        public float XMove;
        public float YMove;

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
        public bool Shift;
        public ControlDown(bool defaultState = false)
        {
            LeftClick = Left = Right = Forward = Back = Jump  = Shift = defaultState;
            RightClick = defaultState;
            Hotkey1 = Hotkey2 = Hotkey3 = Hotkey4 = Hotkey5 = Hotkey6 = Hotkey7 = Hotkey8 = Hotkey9 = Hotkey0 = defaultState;
            XAxis = YAxis = ScrollDelta = XMove = YMove = 0f;
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
    public void DoKeyboardMouseControls()
    {
        Control.ScrollDelta = Input.mouseScrollDelta.y;
        Control.XAxis = Input.GetAxisRaw("Mouse X");
        Control.YAxis = Input.GetAxisRaw("Mouse Y");
        UpdateKey(Input.GetKey(KeyCode.A), LastControl.Left, ref Control.Left);
        UpdateKey(Input.GetKey(KeyCode.D), LastControl.Right, ref Control.Right);
        UpdateKey(Input.GetKey(KeyCode.W), LastControl.Forward, ref Control.Forward);
        UpdateKey(Input.GetKey(KeyCode.S), LastControl.Back, ref Control.Back);
        UpdateKey(Input.GetKey(KeyCode.LeftShift), LastControl.Shift, ref Control.Shift);

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
    public void OnUpdate()
    {
        //Debug.Log(GameStateManager.GameIsOver);
        if(GameStateManager.GameIsPausedOrOver) 
        {
            UnityEngine.Cursor.lockState = CursorLockMode.None; ///This code would be better in a different location, but works here for now.
            UnityEngine.Cursor.visible = true;
            return;
        }
        else
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;
        }
        /*if(GamepadControls.Input.Start.inProgress)
        {
            UsingGamepad = true;
        }
        else if(Input.GetMouseButton(0))
        {
            UsingGamepad = false;
        }*/
        if (UsingGamepad)
        {
            Control.ScrollDelta = 0;

            GamepadControls.InputActions pInput = GamepadControls.Input;
            Vector2 direction = pInput.RightJoystick.ReadValue<Vector2>();
            Control.XAxis = direction.x;
            Control.YAxis = direction.y / 2f;

            direction = pInput.LeftJoystick.ReadValue<Vector2>();
            Control.XMove = direction.x;
            Control.YMove = -direction.y;
            //Debug.Log(direction);

            Control.Left = Control.XMove < 0;
            Control.Right = Control.XMove > 0;
            Control.Forward = Control.YMove < 0;
            Control.Back = Control.YMove > 0;

            UpdateKey(pInput.LeftTrigger.inProgress || pInput.LeftTrigger.IsPressed(), LastControl.LeftClick, ref Control.LeftClick);
            UpdateKey(pInput.RightTrigger.inProgress || pInput.RightTrigger.IsPressed(), LastControl.RightClick, ref Control.RightClick);
            UpdateKey(pInput.LeftJoystickDown.inProgress || pInput.LeftJoystickDown.IsPressed(), LastControl.Shift, ref Control.Shift);
            UpdateKey(pInput.ButtonBottom.inProgress || pInput.ButtonBottom.IsPressed(), LastControl.Jump, ref Control.Jump);

            bool leftBumper = pInput.LeftBumper.WasPressedThisFrame();
            bool rightBumper = pInput.RightBumper.WasPressedThisFrame();
            Control.ScrollDelta += (leftBumper ? 1 : 0) + (rightBumper ? -1 : 0);
        }
        else
        {
            DoKeyboardMouseControls();
            if(Control.Right || Control.Left)
                Control.XMove = 1;
            else
                Control.XMove = 0;
            if (Control.Forward || Control.Back)
                Control.YMove = 1;
            else
                Control.YMove = 0;
        }
    }
    public void OnFixedUpdate()
    {
        LastControl = Control; 
    }
}
