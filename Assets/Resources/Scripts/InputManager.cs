using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    /// <summary>
    /// Singleton
    /// </summary>
    private static InputManager _instance;
    public static InputManager instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = Camera.main.gameObject.AddComponent<InputManager>();
            }
            return _instance;
        }
    }

    /// <summary>
    /// Events declaration
    /// </summary>
    public delegate void MouseHandler();
    public delegate void KeyHandler(KeyCode key);
    public delegate void AxisHandler(float val);
    public delegate void ButtonHandler();
    
    public event MouseHandler MouseLeft;
    public event MouseHandler MouseLeftOnce;
    public event MouseHandler MouseLeftUp;
    public event MouseHandler MouseRight;
    public event MouseHandler MouseRightOnce;
    public event MouseHandler MouseRightUp;
    public event MouseHandler DoubleLeft;
    public event MouseHandler DoubleRight;
    public event MouseHandler MouseUp;
    public event MouseHandler MouseDown;

    public event AxisHandler Horizontal;
    public event AxisHandler Vertical;
    
    public event KeyHandler KeyPressed;

    public event ButtonHandler Jump;
    public event ButtonHandler Crouch;
    public event ButtonHandler Stand;

    
    private bool mouseClicked = false;
    private bool horizontalRelease = false;
    private bool verticalRelease = false;

    /// <summary>
    /// Events
    /// </summary>
    public void OnMouseLeft()
    {
        if(MouseLeft != null)
            MouseLeft();
        mouseClicked = true;
    }

    public void OnMouseLeftOnce()
    {
        if(MouseLeftOnce != null)
            MouseLeftOnce();
        mouseClicked = true;
    }

    public void OnMouseLeftUp()
    {
        if(MouseLeftUp != null)
            MouseLeftUp();
        mouseClicked = false;
    }

    public void OnMouseRight()
    {
        if(MouseRight != null)
            MouseRight();
        mouseClicked = true;
    }

    public void OnMouseRightOnce()
    {
        if(MouseRightOnce != null)
            MouseRightOnce();
        mouseClicked = true;
    }

    public void OnMouseRightUp()
    {
        if(MouseRightUp != null)
            MouseRightUp();
        mouseClicked = false;
    }

    public void OnMouseUp()
    {
        if(MouseUp != null)
            MouseUp();
    }
    public void OnMouseDown()
    {
        if(MouseDown != null)
            MouseDown();
    }

    public void OnKeyPressed(KeyCode key)
    {
        if(KeyPressed != null)
        {
            KeyPressed(key);
        }
    }

    public void OnAxisHorizontal(float val)
    {
        if(Horizontal != null)
            Horizontal(val);
    }

    public void OnAxisVertical(float val)
    {
        if(Vertical != null)
            Vertical(val);
    }

    public void OnJump()
    {
        if(Jump != null)
            Jump();
    }

    public void OnCrouch()
    {
        if(Crouch != null)
            Crouch();
    }

    public void OnStand()
    {
        if(Stand != null)
            Stand();
    }
   
    void Update () 
    {
        MouseHandling();
        AxisHandling();
        ButtonHandling();
    }

   
    private void PanCamera()
    {
        Vector2 mousePos = Input.mousePosition - new Vector3(Screen.width/2, Screen.height/2, 0);
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(Camera.main.transform.position.x + mousePos.x/10, Camera.main.transform.position.y, Camera.main.transform.position.z + mousePos.y/10), Time.deltaTime);
    }

    private void ZoomCameraIn()
    {
        if(Camera.main.transform.position.y > 1)
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - Time.deltaTime*1000, Camera.main.transform.position.z), Time.deltaTime);
    }

    private void ZoomCameraOut()
    {
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y + Time.deltaTime*1000, Camera.main.transform.position.z), Time.deltaTime);
    }

    /// <summary>
    /// Handling the mouse input
    /// </summary>
    private void MouseHandling()
    {
        if(Input.GetMouseButton(1))
        {
            if(!mouseClicked)
            {
                OnMouseRightOnce();
            }
            else
            {
                OnMouseRight();
            }
        }

        if(Input.GetMouseButtonUp(1))
        {
            OnMouseRightUp();
        }

        if(Input.GetMouseButtonUp(0))
        {
            OnMouseLeftUp();
        }

        if(Input.GetMouseButton(0))
        {
            if(!mouseClicked)
            {
                OnMouseLeftOnce();
            }
            else
            {
                OnMouseLeft();
            }
        }
    }

    /// <summary>
    /// Handling axis input
    /// </summary>
    private void AxisHandling()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            OnMouseUp();
        }
        
        if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            OnMouseDown();
        }
        
        float h = Input.GetAxis("Horizontal");
        if(h != 0)
        {
            OnAxisHorizontal(h);
            horizontalRelease = false;
        }
        else if(!horizontalRelease)
        {
            OnAxisHorizontal(0);
            horizontalRelease = true;
        }

        float v = Input.GetAxis("Vertical");
        if(v != 0)
        {
            OnAxisVertical(v);
            verticalRelease = false;
        }
        else if(!verticalRelease)
        {
            OnAxisVertical(0);
            verticalRelease = true;
        }
    }

    /// <summary>
    /// Handling button and key input
    /// </summary>
    private void ButtonHandling()
    {
        if(Input.anyKey)
        {
            foreach(KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
            {
                if(Input.GetKey(k))
                {
                    OnKeyPressed(k);
                }
            }
        }


        if(Input.GetButton("Jump"))
        {
            OnJump();
        }
        if(Input.GetButtonDown("Crouch"))
        {
            OnCrouch();
        }
        if(Input.GetButtonUp("Crouch"))
        {
            OnStand();
        }
    }
}
