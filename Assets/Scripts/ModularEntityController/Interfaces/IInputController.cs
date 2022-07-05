using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInputController {

    public FrameInput Input { get; }
    

    bool GetJumpKeyDown();
    bool GetJumpKeyUp();


}
