using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameEvent {
    public delegate void Handler(GameEvent e);
}

public class EventManager {

    private readonly Dictionary<System.Type, GameEvent.Handler> _handlersDict = new Dictionary<System.Type, GameEvent.Handler>();

    public void Register<Event>(GameEvent.Handler handler) where Event: GameEvent {
        System.Type t = typeof(Event);
        if(_handlersDict.ContainsKey(t)){
            _handlersDict[t] += handler;
        }else{
            _handlersDict[t] = handler;
        }
    }

    public void UnRegister<Event>(GameEvent.Handler handler) where Event : GameEvent {
        System.Type t = typeof(Event);
        GameEvent.Handler handlers;
        if(_handlersDict.TryGetValue(t,out handlers)){
            handlers -= handler;
            if(handlers == null){
                _handlersDict.Remove(t);
            }else{
                _handlersDict[t] = handlers;
            }
        }
    }

    public void Fire(GameEvent e) {
        System.Type t = e.GetType();
        GameEvent.Handler handlers;
        if(_handlersDict.TryGetValue(t,out handlers)){
            handlers(e);
        }
    }
}

