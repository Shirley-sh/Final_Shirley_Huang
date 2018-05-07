using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Services{

    private static EvolutionManager _evolutionManager;
    public static EvolutionManager EvolutionManager{
        get{
            return _evolutionManager;
        }
        set {
            _evolutionManager = value;
        }
    }

    private static EventManager _eventManager;
    public static EventManager EventManager {
        get {
            return _eventManager;
        }
        set {
            _eventManager = value;
        }
    }

    private static InputManager _inputManager;
    public static InputManager InputManager {
        get {
            return _inputManager;
        }
        set {
            _inputManager = value;
        }
    }

    public static Resources Resources;
    public static GameManager GameManager;

}
