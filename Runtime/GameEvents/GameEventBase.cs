using UnityEngine;

namespace Hudossay.AttributeEvents.Assets.Runtime.GameEvents
{
    public abstract class GameEventBase
    {
        public static void InstantiateIfNull<T>(ref T gameEvent) where T : GameEventBase, new()
        {
            if (gameEvent == null)
                gameEvent = new T();
        }


        public abstract void RemoveLinksToGameObject(GameObject gameObject);
    }
}
