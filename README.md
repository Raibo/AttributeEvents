## What is AttributeEvents
It is a Unity package, which allows to easily connect events and response methods in different MonoBehaviour scripts by simply marking them with attributes.

When you raise an event, all connected response methods are called. Generic GameEvents supprt parameters, which are passed to response methods when raised.

## How to use it
### EventLinker
In order for AttributeEvents to work, GameObject must contain EventLinker component.
This component also contains methods for subscription/unsubscription. (More details in "Local Events")

### Matching events and responses
A GameEvent and a response method are matching if they have same label (passed in attribute) and parameters.
It is recommended to use enums as labels, though, you can use any type that you could fit as Attribute parameter. (string, int, etc.)

For example:
```C#

public enum MyLabelEnum
{
    Label1,
    Label2,
}

[EventGlobal(MyLabelEnum.Label1)]
public GameEvent<int, int> SomeEvent;

[ResponseGlobal(MyLabelEnum.Label1)]
public void SomeResponse(int a, int b)
{
    . . .
}
```

Here `SomeEvent` and `SomeResponse` are matching event and response because they have same label and same parameters.
Event and response are typically expected to be declared in different MonoBehaviour scripts.

It is possible to connect event and response in the same MonoBehaviour script, but it have te be different instances of this script.  
AttributeEvents will not connect event and response on the same MonoBehaviour instance.

### Global events
AttributeEvents supports global events, which means that an event is connected to every matching response method in every currently active GameObject.  
Attribute for global events is `EventGlobal`.  
Attribute for global response methods is `ResponseGlobal`.

### Local events
AttributeEvent supports local events. Local events are connected only via subscription.  
Subscriptions are scoped by GameObjects. You can only subscribe a whole GameObject to another whole GameObject (or many).  

You can subscribe GameObjects to one another using EventLinker component's methods:  
`StartListeningTo(GameObject obj)`, `StartBroadcastingTo(GameObject obj)`  
and unsubscribe using:  
`StopListeningTo(GameObject obj)`, `StopBroadcastingTo(GameObject obj)`.  

Broadcasting means that EventLinker's GameObject's GameEvents will call `obj`'s response methods.  
Listening means that EventLinker's GameObject's response methods will be called by `obj`'s GameEvents.  

Subscriptions can be one-sided or mutual.  
A GameObject is considered subscripted to itself by default.

## Debugging
You can observe local subscriptions in `EventLinker` component.

You can also copy a full information report about all local or global connections in `EventLinker`'s context menu.  
![image](https://user-images.githubusercontent.com/8547320/227983346-01eb6a0a-1fe1-4dd8-ae08-1632fe6dfacb.png)


## Example
```c#
public enum UnitEvents
{
    DamageReceived,
    UnitKilled,
    . . .
}


public class ScoreManager : MonoBehaviour
{
    public int TotalScore;
    
    [ResponseGlobal(UnitEvents.UnitKilled)]
    public void AddScore(int addedScore)
    {
        TotalScore += addedScore;
        . . .
    }
}


public class UnitAnimator : MonoBehaviour
{
    [ResponseLocal(UnitEvents.DamageReceived)]
    public void PlayDamageAnimation()
    {
        . . .
    }
}


// This script is expected to be on the same GameObject as UnitAnimator script
// If GameObjects are different, then GameObject with Damageable must broadcast to GameObject with UnitAnimator
public class Damageable : MonoBehaviour
{
    [EventLocal(UnitEvents.DamageReceived)]
    public GameEvent DamageReceived;
    
    [EventGlobal(UnitEvents.UnitKilled)]
    public GameEvent<int> UnitKilled;
    
    public float Health;
    public int KillScore;
    
    public void ReceiveDamage(float damageAmount)
    {
        // Here we call a local event, because we want only this unit to play damage animation, not all units
        DamageReceived.Raise();
        Health -= damageAmount;
        
        if (Health <= 0)
        {
            // Here we call a global event so that a whatever score system could know
            // that a specific amount of points are scored for killing an enemy
            UnitKilled.Raise(KillScore);
            
            // Rest of the code that handles unit death
            . . .
        }
    }
}
```
