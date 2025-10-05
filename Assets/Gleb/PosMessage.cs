using Mirror;
using UnityEngine;

namespace Gleb
{
    public struct PosMessage : NetworkMessage //наследуемся от интерфейса NetworkMessage, чтобы система поняла какие данные упаковывать
    {
        public Vector3 position; //нельзя использовать Property
    }
}